using UnityEngine;
using Enums;

public class EnemyLimb : MonoBehaviour, IDamageable
{
    public LimbType limbType;
    public float limbHealth;
    public float limbMaxHealth;

    public bool IsAlive => enemyBase.IsAlive;
    private EnemyBase enemyBase;

    private void OnEnable()
    {
        enemyBase = transform.root.GetComponent<EnemyBase>();
    }

    public void ReceiveDamage(float damage)
    {
        if (transform.root.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.ReceiveDamage(damage * enemyBase.GetLimbMulti(limbType));
        }
    }
}
