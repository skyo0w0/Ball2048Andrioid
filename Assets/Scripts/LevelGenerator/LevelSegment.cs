using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LevelSegment : MonoBehaviour
{
    private float _lengthX;
    private float _lengthY;
    private float _lengthZ;
    private Renderer _renderer;
    [SerializeField] private string _SegmentName;

    public void Awake()
    {
        if (TryGetComponent<Renderer>(out _renderer))
        {
            _renderer = GetComponent<Renderer>();
            _lengthX = _renderer.bounds.size.x;
            _lengthY = _renderer.bounds.size.y;
            _lengthZ = _renderer.bounds.size.z;
        }
        else
        {
            _renderer = GetComponentInChildren<Renderer>();
        }
        _lengthX = _renderer.bounds.size.x;
        _lengthY = _renderer.bounds.size.y;
        _lengthZ = _renderer.bounds.size.z;
    }

    public float GetLengthX()
    {
        return _lengthX;
    }

    public float GetLengthY()
    {
        return _lengthY;
    }
    public float GetLengthZ() 
    {
        return _lengthZ;
    }
    public string GetSegmentName()
    {
        return _SegmentName;
    }
}
