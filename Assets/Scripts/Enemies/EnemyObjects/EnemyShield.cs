using UnityEngine;

namespace Enemies.EnemyObjects
{
    public class EnemyShield : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Start()
        {
            GameplayEventManager.OnNextMove.AddListener(NextMove);
        }

        private void NextMove()
        {
            // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
            GetComponent<Animator>().SetTrigger("Died");
        }
		
		public void Died()
		{
			Destroy(gameObject);
		}
    }
}
