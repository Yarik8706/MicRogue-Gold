using System;
using System.Collections;
using UnityEngine;

namespace RoomObjects
{
    public class BoxWithRiched : MonoBehaviour
    {
        private Player _player;

        private void Start()
        {
            _player = GameManager.player.GetComponent<Player>();
        }

        private IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) yield break;
            yield return new WaitUntil(() => _player.isTurnOver);
            var playerPosition = _player.GetComponent<Transform>().position;
            if (playerPosition != transform.position) yield break;
            _player.playerHasRiched = true;
            Destroy(gameObject);
        }
    }
}
