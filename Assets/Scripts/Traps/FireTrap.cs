using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Traps
{
    public class FireTrap : Trap
    {
        private List<GameObject> _theEssencesForDied;

        protected override void Start()
        {
            base.Start();
            _theEssencesForDied = new List<GameObject>{Capacity = 0};
        }

        protected override IEnumerator Attack()
        {
            animator.SetTrigger(attack);
            while (_theEssencesForDied.Count != 0)
            {
                _theEssencesForDied[0].GetComponent<TheEssence>().GetDamage(this, AttackType.Fire);
                if(_theEssencesForDied.Count != 0) _theEssencesForDied.RemoveAt(0);
            }
            yield return base.Attack();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.gameObject.tag is "Enemy" or "Player")
            {
                _theEssencesForDied.Add(other.gameObject);
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.tag is not ("Enemy" or "Player")) return;
            if (_theEssencesForDied.Count != 0)
            {
                _theEssencesForDied.Remove(other.gameObject);
            }
        }
    }
}
