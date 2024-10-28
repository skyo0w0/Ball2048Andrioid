using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


public class HorizontalDragController : MonoBehaviour
{
    private PointerInputManager inputManager;

    [SerializeField]
    private Transform _targetObject;
    private Rigidbody _rb;

    [SerializeField]
    private float _dragMultiplier = 1f;

    private Camera _cachedCamera;
    private Vector3 _previousPosition;
    private double _previousTime;
    private Vector3 _previousDirection = Vector3.zero;

    [Inject]
    public void Construct(PointerInputManager inputManager)
    {
        this.inputManager = inputManager;
    }

    protected virtual void Awake()
    {
        Debug.Log("sss");
        // �������� �������� ������ � ��������� � �������
        _cachedCamera = Camera.main;
        _targetObject = gameObject.GetComponent<Transform>();
        _rb = gameObject.GetComponent<Rigidbody>();
        if (_cachedCamera == null)
        {
            Debug.LogError("�������� ������ �� �������. ����������, ���������, ��� � ��� ���� Main Camera � �����.");
        }

    }

    private void OnEnable()
    {
        inputManager.Dragged += OnDragged;
        inputManager.Pressed += OnPressed;
        inputManager.Released += OnReleased;
    }

    private void OnDisable()
    {
        inputManager.Dragged -= OnDragged;
        inputManager.Pressed -= OnPressed;
        inputManager.Released -= OnReleased;
    }

    private void OnPressed(PointerInput input, double time)
    {
        if (_cachedCamera == null) return;
        _previousPosition = _cachedCamera.ScreenToWorldPoint(new Vector3(input.Position.x, input.Position.y, _cachedCamera.nearClipPlane));
        _previousTime = time;
    }

    private void OnReleased(PointerInput input, double time)
    {
        _rb.velocity = new Vector3(0, 0, _rb.velocity.z);
        _rb.angularVelocity = Vector3.zero;  // ������������� ������� ��������, ����� �� ���� ����������������� ��������
    }

    private void OnDragged(PointerInput input, double time)
    {
        // ����������� ������� �������� ���������� � �������
        Vector3 worldCurrent = _cachedCamera.ScreenToWorldPoint(new Vector3(input.Position.x, input.Position.y, _cachedCamera.nearClipPlane));

        // ��������� �������� �� X
        Vector3 delta = worldCurrent - _previousPosition;

        // ��������� �����, ��������� ����� �������
        double deltaTime = time - _previousTime;

        // ��������� �������� ����� (�������� ����� �� �����)
        float dragSpeed = (float)(delta.x / deltaTime);

        Vector3 currentDirection = delta.normalized;

        if (Vector3.Dot(currentDirection, _previousDirection) < 0)
        {
            OnReleased(input, time);
        }

        // ������������� �������� ������� �� ��� X, �������� ������� �������� �� ������ ����
        _rb.velocity = new Vector3(dragSpeed * _dragMultiplier, _rb.velocity.y, _rb.velocity.z);

        // ��������� ���������� ������� � ����� ��� ���������� �����
        _previousPosition = worldCurrent;
        _previousTime = time;
    }
}
