using System.Collections;
using UnityEngine;

namespace Enemies
{
    public interface IStuckInSlime
    {
        public void Stuck();
    }
    
    public class Slime : TheEnemy
    {
        public GameObject slimeRoad;
        
        private GameObject _slimeRoadNow;
        private bool _isDied;

        protected override void Start()
        {
            base.Start();
            GameplayEventManager.OnNextRoom.AddListener(() =>
            {
                Destroy(_slimeRoadNow);
            });
        }

        public override void Active()
        {
            if (_isDied)
            {
                TurnOver();
                return;
            }
            StartCoroutine(DisappearanceSlimeRoad());
            base.Active();
        }

        private IEnumerator DisappearanceSlimeRoad()
        {
            if (_slimeRoadNow == null)
            {
                yield break;
            }
            var component = _slimeRoadNow.GetComponent<SpriteRenderer>();
            var value = 1f;
            while (value > 0)
            {
                value -= Time.deltaTime / 2;
                if(component != null) component.color = new Color(255, 255, 255, value);
                else yield break;
                yield return null;
            }
            Destroy(component.gameObject);
        }

        public override IEnumerator Move(Vector3 @where)
        {
            StartCoroutine(SpawnSlimeRoad(@where, transform.position));
            return base.Move(@where);
        }

        public override void Died()
        {
            isActive = false;
            _isDied = true;
            spriteRenderer.enabled = false;
            boxCollider2D.enabled = false;
            if (essenceHeDied is IStuckInSlime stuck)
            {
                stuck.Stuck();
            }
            if(_slimeRoadNow != null) Destroy(_slimeRoadNow);
            StartAnimationTrigger(diedAnimation);
            Instantiate(baseAnimationsObj, transform.position, Quaternion.identity)
                .GetComponent<BaseAnimations>().DiedAnimation();
            Instantiate(afterDied, transform.position, Quaternion.identity);
            TurnOver();
            StartCoroutine(DiedSlimeRoadAndSlime());
        }
        
        private IEnumerator DiedSlimeRoadAndSlime()
        {
            yield return DisappearanceSlimeRoad();
            TurnOver();
            Destroy(gameObject);
        }

        private IEnumerator SpawnSlimeRoad(Vector3 nextPosition, Vector3 slimePosition)
        { // если ты посмотрел просто знай что это спавнит слизь от слизьня в нужной позиции с нужным поворотом
            var quaternion = Quaternion.identity;
            if (nextPosition.x == slimePosition.x)
            { 
                quaternion = Quaternion.Euler(0,0,90);
            }
            yield return new WaitForSeconds(0.3f);
            _slimeRoadNow = Instantiate(
                slimeRoad, 
                new Vector2(
                    (slimePosition.x + nextPosition.x) / 2, 
                    (slimePosition.y + nextPosition.y) / 2), 
                quaternion);
        }
    }
}
