using UnityEngine;
using UnityEngine.Events;

public static class EnemyEventManager
{
    public static readonly UnityEvent<AttackType> OnAllEnemiesDamage = new();

    public static void AllEnemiesDamageEvent(AttackType type)
    {
        OnAllEnemiesDamage.Invoke(type);
    }
}
