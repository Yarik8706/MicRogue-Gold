using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Enemies;
using UnityEngine;

namespace Canvas
{
    public class CanvasTurnOrder : MonoBehaviour
    {
        public GameObject orderEnemy;
        public Sprite[] numbers;
        internal bool isEnded = true;
        
        private int _countCreateEnemyOrder;
        private bool _isActiveEnemies;

        private void Start()
        {
            GetComponent<RectTransform>().position = new Vector3(Screen.width / 2f, -Screen.height / 2f);
        }

        private void FixedUpdate()
        {
            if (Input.anyKey && !isEnded)
            {
                Invoke(nameof(EndTurnOrder), 0.1f);
            }
        }

        public void EndTurnOrder()
        {
            isEnded = true;
        }

        private IEnumerator ActiveDialog()
        {
            Move(new Vector3(Screen.width / 2f, Screen.height * 0.2f));
            yield return new WaitUntil(() => isEnded);
            Move(new Vector3(Screen.width / 2f, -Screen.height / 2f));
        }

        private void Move(Vector2 targetPosition)
        {
            transform.DOMove(targetPosition, 1f).SetEase(Ease.Linear);
        }

        private IEnumerator TurnOrderCertainEnemy(int type) // помечает какой очерьдью пойдут враги
        {
            _isActiveEnemies = true;
            var enemiesPositions =
                (from enemy in GameObject.FindGameObjectsWithTag("Enemy")
                    where enemy != null
                    where enemy.GetComponent<TheEnemy>().enemyType == type
                    select enemy.transform.position).ToList();

            foreach (var position in enemiesPositions)
            {
                yield return new WaitForSeconds(0.5f);
                var order = Instantiate(orderEnemy, new Vector3(position.x, position.y + 1, 0), Quaternion.identity);
                order.GetComponent<SpriteRenderer>().sprite = numbers[_countCreateEnemyOrder - 1];
                _countCreateEnemyOrder++;
            }

            _isActiveEnemies = false;
        }

        public void StartTurnOrder() // эта функция выполнят событие нажатия на кнопку
        {
            if (_isActiveEnemies || RoomController.enemiesActive) return;
            isEnded = false;
            
            StartCoroutine(TurnOrderEnemy());
            StartCoroutine(ActiveDialog());
        }

        private IEnumerator TurnOrderEnemy()
        {
            _countCreateEnemyOrder = 1;
            var allEnemiesType = new List<int>();
            allEnemiesType.AddRange(
                from enemy in GameObject.FindGameObjectsWithTag("Enemy")
                where enemy != null
                select enemy.GetComponent<TheEnemy>()
                into enemyClass
                select enemyClass.enemyType);
            var maxType = allEnemiesType.Max();
            var minType = allEnemiesType.Min();
            for (var i = minType; i <= maxType; i++)
            {
                StartCoroutine(TurnOrderCertainEnemy(i));
                yield return new WaitUntil(() => !_isActiveEnemies);
            }
        }
    }
}
