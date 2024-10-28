using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelState
{
    public List<SegmentData> Segments = new List<SegmentData>();
    public List<BallData> Balls = new List<BallData>();
    public List<TrapData> Traps = new List<TrapData>();
    public Vector3 PlayerPosition;

    [System.Serializable]
    public class SegmentData
    {
        public string SegmentName;
        public Vector3 Position;

        public SegmentData(string segmentName, Vector3 position)
        {
            this.SegmentName = segmentName;
            this.Position = position;
        }
    }

    [System.Serializable]
    public class BallData
    {
        public Vector3 position;
        public int value;

        public BallData(Vector3 position, int value)
        {
            this.position = position;
            this.value = value;
        }
    }

    [System.Serializable]
    public class TrapData
    {
        public Vector3 position;

        public TrapData(Vector3 position)
        {
            this.position = position;
        }
    }
}
