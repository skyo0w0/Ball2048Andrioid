using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class AudioManager : MonoBehaviour
{
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _scoreUpdateSound;
    [SerializeField] private AudioClip _damagedSound;
    [SerializeField] private AudioClip _levelPassedSound;
    [SerializeField] private AudioClip _onDeathSound;


    private void Awake() 
    {
        _audioSource = Camera.main.GetComponent<AudioSource>();
    }

    public void PlayScoreSound()
    {
        _audioSource.pitch = Random.Range(0.5f, 1f);
        _audioSource.PlayOneShot(_scoreUpdateSound);
    }

    public void PlayDamageSound()
    {
        _audioSource.pitch = Random.Range(0.5f, 1f);
        _audioSource.PlayOneShot(_damagedSound);
    }

    public void playLevelPassedSound()
    {
        _audioSource.pitch = 1;
        _audioSource.PlayOneShot(_levelPassedSound);
    }

    public void playLevelDeathSound()
    {
        _audioSource.pitch = 1;
        _audioSource.PlayOneShot(_onDeathSound);
    }
}