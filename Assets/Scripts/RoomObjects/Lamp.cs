using UnityEngine;

namespace RoomObjects
{
    public class Lamp : MonoBehaviour
    {
        private Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            EnemyEventManager.OnAllEnemiesDamage.AddListener(FreezingEvent);
        }

        private void FreezingEvent(AttackType type)
        {
            // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
            if (type != AttackType.Cold && !_animator.GetBool("isFreezing")) return;
            // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
            _animator.SetTrigger("Freezing");
            // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
            _animator.SetBool("isFreezing", true);
        }
    }
}
