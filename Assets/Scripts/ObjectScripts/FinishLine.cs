using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using Zenject;

public class FinishLine : MonoBehaviour
{
    private GameObject _parentGameObject;
    private List<Rigidbody> _scorePlatforms;
    private Coroutine _moveCoroutine;
    [Inject] AudioManager _audioManager;
    [Inject] LevelManager _levelManager;

    private void Awake()
    {
        _parentGameObject = transform.parent.gameObject;
        _scorePlatforms = _parentGameObject.GetComponentsInChildren<Rigidbody>().ToList();
        _scorePlatforms.RemoveAll(rb => rb.gameObject.name == "Finish");
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.GetComponent<PlayerBall>()  != null)
        { 
            other.GetComponent<HorizontalDragController>().enabled = false;
            other.GetComponent<PlayerMovement>().enabled = false;
            int ballScore = other.GetComponent<PlayerBall>().ReturnBallNumber();
            if (ballScore > 2048)
            {
                ballScore = 2048;
            }
            var scorePlatformPosition = _scorePlatforms.Where(rb => rb.gameObject.name == $"{ballScore}").FirstOrDefault().position;
            Debug.Log(scorePlatformPosition);
            _audioManager.playLevelPassedSound();
            _moveCoroutine = StartCoroutine(MoveToTarget(other.GetComponent<Rigidbody>(), new Vector3(scorePlatformPosition.x, 1, scorePlatformPosition.z)));
        }

        IEnumerator MoveToTarget(Rigidbody playerRigidbody, Vector3 targetPosition)
        {
            playerRigidbody.velocity = Vector3.zero;
            float moveSpeed = 5f;

            while (Vector3.Distance(playerRigidbody.position, targetPosition) > 0.1f)
            {
                Vector3 newPosition = Vector3.MoveTowards(playerRigidbody.position, targetPosition, moveSpeed * Time.deltaTime);
                playerRigidbody.MovePosition(newPosition);
                yield return null;
            }

            playerRigidbody.isKinematic = true;

            // ќжидаем 2 секунды после достижени€ цели
            yield return new WaitForSeconds(2f);

            _levelManager.NextLevel();

            _moveCoroutine = null;
        }

    }
}
