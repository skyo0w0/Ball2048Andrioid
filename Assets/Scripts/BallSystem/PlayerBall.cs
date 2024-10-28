using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class PlayerBall : BallBase
{
    private LevelManager _levelManager;
    private CinemachineVirtualCamera _virtualCamera;
    private Coroutine _onLoseCorutine;
    [Inject] private AudioManager _audioManager;
    private PlayerMovement _playerMovement => GetComponent<PlayerMovement>();

    [Inject]
    public void Construct(LevelManager levelManager, CinemachineVirtualCamera virtualCamera)
    {
        this._levelManager = levelManager;
        this._virtualCamera = virtualCamera;
    }

    private void OnEnable()
    {
        _virtualCamera.Follow = transform;
    }
    private void DecreaceNumber()
    {
        if (base._ballData.Number == 2)
        {
            OnLose();
        }
        else
        {
            base.SetNumber(_ballData.Number / 2);
            _playerMovement.DecreaseMoveSpeed();
        }
    }

    public override void IncreaseNumber()
    {
        base.IncreaseNumber();
        _playerMovement.IncreaseMoveSpeed();
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<ScoreBall>() != null)
        {
            if (collision.gameObject.GetComponent<ScoreBall>().ReturnBallNumber() == _ballData.Number)
            {
                Destroy(collision.gameObject);
                _audioManager.PlayScoreSound();
                IncreaseNumber();
            }
        }
        if (collision.gameObject.GetComponent<SpikeTrap>() != null)
        {
            DecreaceNumber();
            _audioManager.PlayDamageSound();
        }
    }

    public void OnLose()
    {
        _audioManager.playLevelDeathSound();
        _onLoseCorutine = StartCoroutine(OnLoseCorutine());
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    IEnumerator OnLoseCorutine()
    {
        yield return new WaitForSeconds(1f);
        _levelManager.RestartLevel();
    }
}
