using System.Collections;
using UnityEngine;

namespace RoomObjects
{
    public class UpdateShieldsObj : MonoBehaviour
    {
        private IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) yield break;
            yield return new WaitUntil(() => GameManager.player.isTurnOver);
            if (GameManager.player.transform.position != transform.position) yield break;
            GameManager.player.RestorationOfShields();
            Destroy(gameObject);
        }
    }
}
