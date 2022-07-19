using System.Collections;
using System.Linq;
using UnityEngine;

namespace Enemies
{
    public class TrollWithStick : TheEnemy
    {
        public GameObject enemyShield;
        private bool _startSpawnShield;
        private GameObject _enemyShieldNow;

        protected override void Start()
        {
            base.Start();
            GameplayEventManager.OnNextRoom.AddListener(NextRoom);
        }

        private void NextRoom()
        {
            Destroy(_enemyShieldNow);
        }

        protected override void TurnOver()
        {
            StartCoroutine(InstantiateEnemyShieldAndTurnOver());
        }

        public void StartSpawnShield()
        {
            _startSpawnShield = true;
        }

        private IEnumerator InstantiateEnemyShieldAndTurnOver()
        {
            // троль берет случайного монстра завершившего ход и ставит на нем щит
            var allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            var enemiesNotTurnOver = RoomController.allEnemies;
            if(allEnemies == null || enemiesNotTurnOver == null)
            {
                TurnOver();
                yield break;
            }
            var availablePositionEnemies = (from enemy in allEnemies
                    where enemy != gameObject
                    select enemy.GetComponent<TheEnemy>() 
                    into enemyClass
                    where enemyClass.enemyType < enemyType && enemyClass.isActive
                    select enemyClass.transform.position
                ).Select(dummy => (Vector2) dummy).ToArray();
            if (availablePositionEnemies.Length != 0)
            {
                // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
                animator.SetTrigger("SpawnEnemyShield");
                yield return new WaitUntil(() => _startSpawnShield);
                _enemyShieldNow = Instantiate(enemyShield,
                    SelectionOfTheNearestPosition(GameManager.player.transform.position, availablePositionEnemies),
                    Quaternion.identity);
            }
            yield return null;
            _startSpawnShield = false;
            base.TurnOver();
        }
    }
}
