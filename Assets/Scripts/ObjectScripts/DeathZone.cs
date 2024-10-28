using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DeathZone : MonoBehaviour
{
    [Inject] private AudioManager _audioManager;
    [Inject] private PlayerBall _playerBall;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerBall>() != null)
        {
            _audioManager.playLevelDeathSound();
            _playerBall.OnLose();
        }
        else if (other.GetComponent<ScoreBall>() != null)
        {
            Destroy(other.gameObject);
        }
    }


}
