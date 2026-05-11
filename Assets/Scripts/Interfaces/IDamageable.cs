using UnityEngine;

public interface IDamageable
{
    bool IsAlive { get; set; }
    void ReceiveDamage(float damage);
}
