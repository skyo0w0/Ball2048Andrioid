using UnityEngine;
using Zenject;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using System.Drawing;
using System.IO;



public class LevelGenerator : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private LevelSegment[] _levelSegmentPrefabs;
    [SerializeField] private int _initialSegmentCount = 6;

    #endregion

    #region Prefabs

    private ScoreBall _scoreBallPrefab;
    private SpikeTrap _spikeTrapPrefab;

    #endregion

    #region Spawned Objects

    private List<LevelSegment> _spawnedSegments = new List<LevelSegment>();
    private List<ScoreBall> _spawnedBalls = new List<ScoreBall>();
    private List<SpikeTrap> _spawnedSpikes = new List<SpikeTrap>();

    #endregion

    #region Zenject Variables

    private DiContainer _container;

    #endregion

    #region Level Generation Variables

    private Vector3 _lastSegmentPosition;
    private List<List<Vector3>> _possibleBallPoints;
    private List<List<Vector3>> _possibleTrapPoints;
    private List<LevelSegment> _playableSegments;
    private LevelSegment _finishSegment;

    #endregion

    [Inject]
    public void Construct(DiContainer container)
    {
        this._container = container;
    }

    private void Start()
    {
        _levelSegmentPrefabs = Resources.LoadAll<LevelSegment>("Prefabs/LevelSegments");
        _scoreBallPrefab = Resources.Load<ScoreBall>("Prefabs/Balls/ScoreBall");
        _spikeTrapPrefab = Resources.Load<SpikeTrap>("Prefabs/Traps/Spike");
        _possibleTrapPoints = new List<List<Vector3>>();
        _playableSegments = _levelSegmentPrefabs.Where(segment => segment.GetSegmentName() != "Finish").ToList();
        _finishSegment = _levelSegmentPrefabs.Where(segment => segment.GetSegmentName() == "Finish").FirstOrDefault();

    }

    private void GenerateNextSegment()
    {
        var segmentPrefab = PickSegment();
        InitialSegment(segmentPrefab);
    }

    private LevelSegment PickSegment()
    {
        if (_spawnedSegments.Count == 0 || _spawnedSegments.Count == _initialSegmentCount - 2)
        {
            return _playableSegments.Where(segment => segment.GetSegmentName() == "Platform").ToList()[0];
        }
        if (_spawnedSegments.Count == _initialSegmentCount - 1)
        {
            return _finishSegment;
        }
        switch (_spawnedSegments.Last<LevelSegment>().GetSegmentName())
        {

            case "Rails":
                var availableSegments = _playableSegments.Where(segment => segment.GetSegmentName() == "Platform").ToList();
                return availableSegments[Random.Range(0,availableSegments.Count)];
            default:
                break;
        }
        return _playableSegments[Random.Range(0,_playableSegments.Count)];
    }

    private void InitialSegment(LevelSegment segmentPrefab)
    {
        LevelSegment newSegment = _container.InstantiatePrefabForComponent<LevelSegment>(segmentPrefab,Vector3.zero,Quaternion.identity, null);
        Vector3 newSegmentPosition = new Vector3(_lastSegmentPosition.x, _lastSegmentPosition.y, (_lastSegmentPosition.z + (float)(newSegment.GetLengthZ() / 2f)));
        newSegment.transform.position = newSegmentPosition;
        _spawnedSegments.Add(newSegment);
        _lastSegmentPosition = new Vector3(newSegmentPosition.x, newSegmentPosition.y, newSegmentPosition.z + ( newSegment.GetLengthZ() / 2f));
        Debug.Log(_lastSegmentPosition.z);
    }

    private void InitialSegment(LevelSegment segmentPrefab, Vector3 position)
    {
        LevelSegment newSegment = _container.InstantiatePrefabForComponent<LevelSegment>(segmentPrefab, position, Quaternion.identity, null);
        _spawnedSegments.Add(newSegment);
        _lastSegmentPosition = new Vector3(position.x, position.y, position.z + (newSegment.GetLengthZ() / 2f));
    }

    public void SpawnBalls()
    {
        var allPlatforms = _spawnedSegments.Where(segment => segment.GetSegmentName() == "Platform").ToList();
        int requiredScore = 16;
        int ballScore = 2;
        foreach (LevelSegment platform in allPlatforms) 
        {
            int halfScore = requiredScore / 2;
            Vector3 centerPos = platform.transform.position;
            Vector3 topLeftPos = new Vector3(centerPos.x - platform.GetLengthX()/2, centerPos.y, centerPos.z + platform.GetLengthZ()/2);
            Vector3 topRightPos = new Vector3(centerPos.x + platform.GetLengthX() / 2, centerPos.y, centerPos.z + platform.GetLengthZ() / 2);
            Vector3 bottomLeftPos = new Vector3(centerPos.x - platform.GetLengthX() / 2, centerPos.y, centerPos.z - platform.GetLengthZ() / 2);
            Vector3 bottomRightPos = new Vector3(centerPos.x + platform.GetLengthX()/2, centerPos.y, centerPos.z - platform.GetLengthZ() / 2);
            List<Vector3> topPoints = new List<Vector3>();
            List<Vector3> bottomPoints = new List<Vector3>();
            List<Vector3> leftSidePoints = new List<Vector3>();
            List<Vector3> rightSidePoints = new List<Vector3>();
            _possibleBallPoints = new List<List<Vector3>>();
            for (float i = 0.25f; i < 0.8; i += 0.25f)
            {
                topPoints.Add(Vector3.Lerp(topLeftPos, topRightPos, i));
                bottomPoints.Add(Vector3.Lerp(bottomLeftPos, bottomRightPos, i));
                leftSidePoints.Add(Vector3.Lerp(bottomLeftPos, topLeftPos, i));
                rightSidePoints.Add(Vector3.Lerp(bottomRightPos, topRightPos, i));
            }
            for (int i = 0; i < topPoints.Count; i++)
            {
                for (float j = 0.25f; j < 0.8; j += 0.25f)
                {
                    _possibleBallPoints.Add(new List<Vector3>());
                    _possibleBallPoints[i].Add(Vector3.Lerp(leftSidePoints[i], rightSidePoints[i], j));
                }
            }
            int currentScore = 0;
            //Debug.Log(possibleBallPoints.Count);
            foreach (List<Vector3> points in _possibleBallPoints)
            {
                _possibleTrapPoints.Add(points);
                if (currentScore + ballScore > requiredScore)
                {
                    break;
                }
                var randPoint = points[Random.Range(0,points.Count)];

                ScoreBall scoreBall = _container.InstantiatePrefabForComponent<ScoreBall>(_scoreBallPrefab, new Vector3(randPoint.x, 1, randPoint.z), Quaternion.identity, null);
                scoreBall.SetNumber(ballScore);
                _spawnedBalls.Add(scoreBall);

                points.Remove(randPoint);

                foreach (Vector3 point in points)
                {
                    int number = Random.Range(0,2);
                    if (number == 1)
                    {
                        ScoreBall newScoreBall = _container.InstantiatePrefabForComponent<ScoreBall>(_scoreBallPrefab, new Vector3(point.x, 1, point.z), Quaternion.identity, null);
                        _spawnedBalls.Add(newScoreBall);
                        newScoreBall.SetNumber(ballScore * (int) Mathf.Pow(2, Random.Range(0, 3)));
                    }

                }
                currentScore += ballScore;
                ballScore *= 2;
            }   
            requiredScore *= 8;
        }
    }

    public void SpawnTraps()
    {
        foreach (List<Vector3> points in _possibleTrapPoints)
        {
            foreach (Vector3 point in points) 
            {
                if (Random.Range(0, 2) == 0)
                {
                    float offsetX = point.x > 0 ? -1 : point.x == 0 ? 0 : 1;
                    float offsetZ = point.x == 0 ? -2 : 0;
                    SpikeTrap newTrap = _container.InstantiatePrefabForComponent<SpikeTrap>(_spikeTrapPrefab, new Vector3(point.x + offsetX, 1, point.z + offsetZ), Quaternion.identity, null);
                    _spawnedSpikes.Add(newTrap);
                }
            }
        }
    }

    public void GenerateInitialSegments()
    {
        for (int i = 0; i < _initialSegmentCount; i++)
        {
            GenerateNextSegment();
        }
    }

    // Получение данных для сохранения
    public List<LevelState.SegmentData> GetSegmentData()
    {
        var segmentDataList = new List<LevelState.SegmentData>();
        foreach (var segment in _spawnedSegments)
        {
            var segmentData = new LevelState.SegmentData(segment.GetSegmentName(), segment.transform.position);
            segmentDataList.Add(segmentData);
        }
        return segmentDataList;
    }

    public List<LevelState.BallData> GetBallData()
    {
        var ballDataList = new List<LevelState.BallData>();
        foreach (var ball in _spawnedBalls)
        {
            var ballData = new LevelState.BallData(ball.transform.position, ball.ReturnBallNumber());
            ballDataList.Add(ballData);
        }
        return ballDataList;
    }

    public List<LevelState.TrapData> GetTrapData()
    {
        var trapDataList = new List<LevelState.TrapData>();
        foreach (var trap in _spawnedSpikes)
        {
            var trapData = new LevelState.TrapData(trap.transform.position);
            trapDataList.Add(trapData);
        }
        return trapDataList;
    }

    // Восстановление объектов из сохранённых данных
    public void RestoreSegments(List<LevelState.SegmentData> segmentDataList)
    {
        foreach (var segmentData in segmentDataList)
        {
            var segmentPrefab = _levelSegmentPrefabs.First(prefab => prefab.GetSegmentName() == segmentData.SegmentName);
            InitialSegment(segmentPrefab, segmentData.Position);
        }
    }

    public void RestoreBalls(List<LevelState.BallData> ballDataList)
    {
        foreach (var ballData in ballDataList)
        {
            ScoreBall newBall = _container.InstantiatePrefabForComponent<ScoreBall>(_scoreBallPrefab, ballData.position, Quaternion.identity, null);
            newBall.SetNumber(ballData.value);
            _spawnedBalls.Add(newBall);
        }
    }

    public void RestoreTraps(List<LevelState.TrapData> trapDataList)
    {
        foreach (var trapData in trapDataList)
        {
            SpikeTrap newTrap = _container.InstantiatePrefabForComponent<SpikeTrap>(_spikeTrapPrefab, trapData.position, Quaternion.identity, null);
            _spawnedSpikes.Add(newTrap);
        }
    }

}
