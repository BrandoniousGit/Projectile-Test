using DamageNumbersPro;
using UnityEngine;
using Enums;
using static Globals;

public class PrismoidK : EnemyBase
{
    private float attackTimer;
    private GameObject enemyBullet;
    public float bulletDamage;
    public float bulletSpeed;

    private void ShootAtPlayer()
    {
        Agent.ResetPath();

        if (attackTimer < 0)
        {
            attackTimer = attackCooldown;

            var bulletRb = Instantiate(enemyBullet, transform.position, Quaternion.LookRotation(PlayerManager.Instance.GetPlayerPosition() - transform.position)).GetComponent<BulletBase>();
            bulletRb.Init(new BulletData
            {
                Damage = bulletDamage,
                BulletSpeed = bulletSpeed,
                UseGravity = false
            });
        }
    }

    private void ChasePlayer()
    {
        Agent.SetDestination(PlayerManager.Instance.GetPlayerPosition());
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        enemyBullet = (GameObject)Resources.Load("EnemyBullet");
    }

    protected override void Update()
    {
        if (IsAlive && !isBlind)
        {
            base.Update();

            if (attackTimer >= 0)
            {
                attackTimer -= Time.deltaTime;
            }

            if (currentState == EnemyState.Idle)
            {
                //TODO: Random walk while idle
                Agent.ResetPath();
            }

            if (currentState == EnemyState.Chasing)
            {
                ChasePlayer();
            }

            if (currentState == EnemyState.Attacking)
            {
                ShootAtPlayer();
            }    
        }
    }
}
