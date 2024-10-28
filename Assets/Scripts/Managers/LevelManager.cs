using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System.IO;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private LevelGenerator _levelGenerator;
    private string _saveFilePath;

    [Inject]
    public void Construct(LevelGenerator levelGenerator)
    {
        this._levelGenerator = levelGenerator;
    }

    private void Start()
    {
        _saveFilePath = Path.Combine(Application.persistentDataPath, "levelState.json");

        // ���������, ���� �� ����������� ������
        var loadedState = LoadLevel();
        if (loadedState != null)
        {
            RestoreLevel(loadedState);
        }
        else
        {
            // ���������� ����� �������, ���� ���������� �����������
            _levelGenerator.GenerateInitialSegments();
            _levelGenerator.SpawnBalls();
            _levelGenerator.SpawnTraps();
            SaveLevel();
        }
    }

    public void SaveLevel()
    {
        LevelState state = new LevelState();
        state.PlayerPosition = transform.position;

        // �������� ������ �� LevelGenerator � ��������� ��
        state.Segments = _levelGenerator.GetSegmentData();
        state.Balls = _levelGenerator.GetBallData();
        state.Traps = _levelGenerator.GetTrapData();

        // ����������� ������ � ��������� � ����
        string json = JsonUtility.ToJson(state, true);
        File.WriteAllText(_saveFilePath, json);
        Debug.Log("������� ������� � ����: " + _saveFilePath);
    }

    public LevelState LoadLevel()
    {
        if (File.Exists(_saveFilePath))
        {
            string json = File.ReadAllText(_saveFilePath);
            LevelState loadedState = JsonUtility.FromJson<LevelState>(json);
            Debug.Log("������� �������� �� �����: " + _saveFilePath);
            return loadedState;
        }

        Debug.Log("���� ���������� �� ������: " + _saveFilePath);
        return null;
    }

    public void RestoreLevel(LevelState state)
    {
        transform.position = state.PlayerPosition;

        // ��������������� �������� ������ ����� LevelGenerator
        _levelGenerator.RestoreSegments(state.Segments);
        _levelGenerator.RestoreBalls(state.Balls);
        _levelGenerator.RestoreTraps(state.Traps);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void DeleteSave()
    {
        if (File.Exists(_saveFilePath))
        {
            File.Delete(_saveFilePath);
        }
    }

    public void NextLevel()
    {
        DeleteSave();
        RestartLevel();
    }

}
