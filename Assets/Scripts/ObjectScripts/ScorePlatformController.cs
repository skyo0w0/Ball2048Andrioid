using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ScorePlatformController : MonoBehaviour
{
    [Inject] private AudioManager _audioManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerBall>() != null)
        {
            _audioManager.playLevelPassedSound();
        }
    }
}
