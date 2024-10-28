using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;   // Скорость движения вперед

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        AlwaysForward();
    }

    private void AlwaysForward()
    {
        _rb.velocity = new Vector3(_rb.velocity.x, _rb.velocity.y, moveSpeed);
    }

    public void IncreaseMoveSpeed()
    {
        moveSpeed += 0.5f;
    }

    public void DecreaseMoveSpeed()
    {
        moveSpeed -= 0.5f;
    }
}
