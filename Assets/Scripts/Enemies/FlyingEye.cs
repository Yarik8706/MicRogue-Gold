using System;
using System.Collections;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Enemies
{
    public class FlyingEye : TheEnemy
    {
        private bool _isCenterAttack;
        private bool _isEndAttack;
        private static readonly int TriggerAttack = Animator.StringToHash("Attack");
        private static readonly int RayNearObject = Animator.StringToHash("rayNearObject");
        private static readonly int FlyingEyeRay = Animator.StringToHash("flyingEyeRay");

        protected override Vector2 SelectMovePosition(Vector2 playerPosition, Vector2[] theVariantsPositions)
        {
            var centerVariantsPositions = theVariantsPositions.Where(
                variantPosition => transform.position.y - variantPosition.y != 0
                ).ToList();
            var position = SelectionOfTheNearestPosition(
                playerPosition,
                centerVariantsPositions.Count < 2 ? theVariantsPositions : centerVariantsPositions.ToArray(), 
                transform.position);
            return position;
        }

        protected override void SelectAction(Vector2 nextPosition, Vector2 playerPosition)
        {
            Debug.Log(playerPosition.y);
            Debug.Log(transform.position.y);
            Debug.Log(playerPosition.y + transform.position.y);
            if(nextPosition.x < transform.position.x && turnedRight || nextPosition.x > transform.position.x && !turnedRight)
            {
                Flip();
            }
            if (Vector2.Distance(transform.position, playerPosition) <= 1)
            {
                StartCoroutine(base.Attack(playerPosition));
                return;
            }
            if (Convert.ToString(transform.position.y) == Convert.ToString(playerPosition.y))
            {
                boxCollider2D.enabled = false;
                var hit = Physics2D.Linecast(transform.position, playerPosition, blockingLayer);
                boxCollider2D.enabled = true;
                if (hit.collider == null)
                {
                    StartCoroutine(Attack(playerPosition));
                    return;
                }
            }
            if (nextPosition == (Vector2)transform.position)
            {
                TurnOver();
                return;
            }
            
            StartCoroutine(Move(nextPosition));
        }

        protected override IEnumerator Attack(Vector2 playerPosition)
        {
            animator.SetTrigger(TriggerAttack);
            yield return new WaitUntil(() => _isCenterAttack);
            var object1 = Instantiate(baseAnimationsObj, transform.position, Quaternion.identity);
            var object2 = Instantiate(baseAnimationsObj, playerPosition, Quaternion.identity);
            var scaler = transform.localScale;
            object2.transform.localScale = scaler;
            scaler.x *= -1;
            object1.transform.localScale = scaler;
            object1.GetComponent<Animator>().SetTrigger(RayNearObject);
            object2.GetComponent<Animator>().SetTrigger(RayNearObject);
            yield return new WaitForSeconds(0.3f);
            var calculations = playerPosition.x - transform.position.x;
            var spawnPosition = (Vector3)playerPosition;
            var direction = calculations / Mathf.Abs(calculations);
            var positionTo = new Vector3(transform.position.x + direction, transform.position.y);
            while (spawnPosition != positionTo)
            {
                spawnPosition = new Vector3(
                    spawnPosition.x - direction, 
                    spawnPosition.y, 
                    0); 
                Instantiate(baseAnimationsObj, spawnPosition, quaternion.identity)
                    .GetComponent<Animator>().SetTrigger(FlyingEyeRay);;
            }
            yield return new WaitUntil(() => _isEndAttack);
            GameManager.player.transform.position = transform.position;
            transform.position = playerPosition;
            transform.localScale = scaler;
            turnedRight = !(transform.localScale.x > 0);
            TurnOver();
        }

        public void CenterAttack()
        {
            _isCenterAttack = true;
        }

        public void EndAttack()
        {
            _isEndAttack = true;
        }
    }
}
