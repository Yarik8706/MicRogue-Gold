using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public interface ICold
    {
        void Cold(GameObject cold);
    }
    
    public class SnowMan : TheEnemy, ICold
    {
        private readonly List<TheEnemy> _freezingEnemies = new();
        private bool _isColdEventStart;
        private int _timeColdEvent;
        
        protected override void Start()
        {
            base.Start();
            attackActions[AttackType.Fire] = DiedByFire;
        }

        public override void Active()
        {
            if (_isColdEventStart)
            {
                _timeColdEvent++;
                if (_timeColdEvent == 3)
                {
                    EndEnemiesCold();
                }
                TurnOver();
                return;
            }
            base.Active();
        }

        private void EndEnemiesCold()
        {
            isActive = false;
            foreach (var enemy in _freezingEnemies)
            {
                enemy.isActive = true;
            }
        }

        private IEnumerator StartEnemyCold(GameObject enemy)
        {
            var enemyClass = enemy.GetComponent<TheEnemy>();
            var animationObj = Instantiate(baseAnimationsObj, enemy.transform.position, Quaternion.identity);
            var animationObjClass = animationObj.GetComponent<BaseAnimations>();
            animationObj.transform.SetParent(enemy.transform);
            if (enemyClass is ICold iCold)
            {
                iCold.Cold(animationObj);
                yield break;
            }
            animationObjClass.isDied = false;
            animationObjClass.FreezingAnimation();
            animationObj.transform.SetParent(enemy.transform);
            var baseAnimatorSpeed = enemyClass.animator.speed;
            var baseColorSprite = enemyClass.spriteRenderer.color;
            enemyClass.spriteRenderer.color = Color.cyan;
            enemyClass.animator.speed = 0;
            yield return new WaitUntil(() => enemyClass == null || enemyClass.isMove);
            if(enemyClass == null) yield break;
            enemyClass.animator.speed = baseAnimatorSpeed;
            animationObjClass.isDied = true;
            enemyClass.spriteRenderer.color = baseColorSprite;
            animationObjClass.FrostbiteAnimation();
        }

        public override void Died()
        {
            if (_isColdEventStart)
            {
                return;
            }
            var allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in allEnemies)
            {
                var enemyClass = enemy.GetComponent<TheEnemy>();
                if (enemy == gameObject || !enemyClass.isActive)
                {
                    continue;
                }
                enemyClass.isActive = false;
                _freezingEnemies.Add(enemyClass);
                StartCoroutine(StartEnemyCold(enemy));
            }
            _isColdEventStart = true;
            boxCollider2D.enabled = false;
            spriteRenderer.enabled = false;
            Instantiate(afterDied, transform.position, Quaternion.identity);
            Instantiate(baseAnimationsObj, transform.position, Quaternion.identity)
                .GetComponent<BaseAnimations>().DiedAnimation();
            StartCoroutine(ColdBlackount());
        }

        private void DiedByFire()
        {
            if (_isColdEventStart)
            {
                return;
            }
            base.Died();
        }

        private IEnumerator ColdBlackount()
        {
            GameManager.coldBlackount.SetActive(true);
            yield return new WaitForSeconds(0.7f);
            GameManager.coldBlackount.SetActive(false);
            TurnOver();
        }

        public void Cold(GameObject cold)
        {
            Destroy(cold);
        }
    }
}
