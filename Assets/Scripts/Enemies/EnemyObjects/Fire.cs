using UnityEngine;

namespace Enemies.EnemyObjects
{
    public class Fire : MonoBehaviour, ICauseOfDied
    {
        public string[] causeOfDied;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer != 8 && other.gameObject.layer != 3) return;
            var theEssence = other.gameObject.GetComponent<TheEssence>();
            theEssence.GetDamage(this, AttackType.Fire);
        }

        public void Died()
        {
            Destroy(gameObject);
        }

        public string CauseOfDied()
        {
            return causeOfDied[Random.Range(0, causeOfDied.Length)];
        }
    }
}
