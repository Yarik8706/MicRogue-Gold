using System.Collections;
using UnityEngine;

namespace Traps
{
    public class TrapWall : Trap
    {
        public string[] animations2;
        public string endAnimation2;
        public bool isSecondPhase;
        
        private string[] _animations1;
        private GameObject _theEssenceForDied;
        private SpriteRenderer _spriteRenderer;

        protected override void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animations1 = (string[]) stagesAttack.Clone();
            if (isSecondPhase)
            {
                gameObject.layer = 6;
                stagesAttack = (string[]) animations2.Clone();
            }
            base.Start();
        }

        protected override IEnumerator Attack()
        {
            animator.SetTrigger(attack);
            
            if (_theEssenceForDied == null) return base.Attack();
            var essence = _theEssenceForDied.GetComponent<TheEssence>();
            essence.GetDamage(this, AttackType.Melee);

            return base.Attack();
        }

        private IEnumerator EndAnimation()
        {
            animator.SetTrigger(endAnimation2);
            yield return new WaitForSeconds(waitTime);
            SetStageAttack(0);
        }

        public override void SetStageAttack(int i)
        {
            if(stageNow + i >= stagesAttack.Length)
            {
                if (isSecondPhase)
                {
                    stageNow = 0;
                    isSecondPhase = false;
                    gameObject.layer = 0;
                    _spriteRenderer.sortingLayerName = "Default";
                    stagesAttack = (string[]) _animations1.Clone();
                    StartCoroutine(EndAnimation());
                }
                else
                {
                    StartCoroutine(Attack());
                    _spriteRenderer.sortingLayerName = "Decoration";
                    gameObject.layer = 6;
                    isSecondPhase = true;
                    stagesAttack = (string[]) animations2.Clone();
                }
            }
            else {
                stageNow += i;
                animator.SetTrigger(stagesAttack[stageNow]); 
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        { 
            if(other.gameObject.tag is "Enemy" or "Player")
            {
                _theEssenceForDied = other.gameObject;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if(_theEssenceForDied == other.gameObject)
            {
                _theEssenceForDied = null;
            }
        }
    }
}
