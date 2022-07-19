using System.Collections;
using UnityEngine;

namespace RoomObjects
{
    public class Exit : MonoBehaviour
    {
        public bool isStairDown;
        public Vector3 nextPositionPlayer;
        private Player _player;
        
        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }

        private IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) yield break;
            yield return new WaitUntil(() => RoomController.enemiesActive);
            yield return new WaitForSeconds(1f);
            if(_player == null || _player.transform.position != GetNextPositionPlayer()) yield break;
            var playerHasRiched = _player.playerHasRiched;
            switch (isStairDown)
            {
                case true when playerHasRiched:
                    StartCoroutine(GameManager.gameManager.NextRoom(-1));
                    break;
                case false when !playerHasRiched:
                    StartCoroutine(GameManager.gameManager.NextRoom(1));
                    break;
            }
        }

        public Vector3 GetNextPositionPlayer()
        {
            return new Vector3(transform.position.x + nextPositionPlayer.x,
                            nextPositionPlayer.y + transform.position.y);
        }
    }
}
