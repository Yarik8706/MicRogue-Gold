using System.Collections;
using UnityEngine;

namespace Traps
{
    public class FloorTrap : Trap
    {
        private GameObject _theEssenceForDied;

        protected override IEnumerator Attack()
        {
            animator.SetTrigger(attack);
            if(_theEssenceForDied != null) _theEssenceForDied.GetComponent<TheEssence>().GetDamage(this, AttackType.Melee);
            yield return base.Attack();
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
