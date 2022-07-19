using System.Collections;
using UnityEngine;

namespace Traps
{
    public abstract class Trap : MonoBehaviour, ICauseOfDied
    {
        public float waitTime;
        public int stageNow;
        public string[] stagesAttack;
        public string attack;
        public string[] causeOfDied;
        private protected Animator animator;
        
        // Start is called before the first frame update
        protected virtual void Start()
        {
            animator = GetComponent<Animator>();
            SetStageAttack(0);
        }
    
        public virtual void SetStageAttack(int i)
        {
            if(stageNow + i >= stagesAttack.Length)
            {
                StartCoroutine(Attack());
            }
            else {
                stageNow += i;
                animator.SetTrigger(stagesAttack[stageNow]); 
            }
        }

        protected virtual IEnumerator Attack()
        {
            yield return new WaitForSeconds(waitTime);
            stageNow = 0;
            SetStageAttack(0);
        }

        public string CauseOfDied()
        {
            return causeOfDied[Random.Range(0, causeOfDied.Length)];
        }
    }
}
