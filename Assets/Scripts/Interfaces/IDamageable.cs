using UnityEngine;

public interface IDamageable
{
    bool IsAlive { get; }
    void ReceiveDamage(float damage);
}
