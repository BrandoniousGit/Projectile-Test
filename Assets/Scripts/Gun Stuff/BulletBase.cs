using DamageNumbersPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Globals;

public class BulletBase : MonoBehaviour
{
    public float deathTimer;
    public LayerMask collisionMask;
    public Team Team;
    public DamageNumber numberPrefab;

    private BulletData Data;
    private Rigidbody rb;
    private Vector3 currentLinearVelocity;
    private float homingRange;
    private int bounces;
    private GameObject closestEnemy;

    public static event Action<BulletBase> UpdateBulletBase;
    public static event Action<bool> DidShotHitSomething;

    #region setup
    private void OnEnable()
    {
        if (Team == Team.Player)
        {
            gameObject.layer = LayerMask.NameToLayer("Bullet");
        }
        else if (Team == Team.Enemy)
        {
            gameObject.layer = LayerMask.NameToLayer("EnemyBullet");
        }

        StartCoroutine("DestroyAfter", deathTimer);
        rb = GetComponent<Rigidbody>();

        UpdateBulletBase?.Invoke(this);
    }
    #endregion

    #region public methods
    public void SetData(BulletData data)
    {
        Data = data;
        bounces = (int)Data.Bounces - 1;

        rb.linearVelocity = Data.BulletSpeed * transform.forward;
    }    
    
    public void ModifyHoming(float homingRange)
    {
        this.homingRange = homingRange;
    }

    public void SetBulletTeam(Team team)
    {
        Team = team;
    }
    #endregion

    #region private methods
    private void Update()
    {
        if (Team == Team.Player)
        {
            gameObject.layer = LayerMask.NameToLayer("Bullet");
        }
        else if (Team == Team.Enemy)
        {
            gameObject.layer = LayerMask.NameToLayer("EnemyBullet");
        }

        if (Data != null && Data.UseGravity)
        {
            rb.linearVelocity += new Vector3(0, Data.Gravity * Time.deltaTime, 0);
            transform.forward = rb.linearVelocity.normalized;
        }

        if (homingRange > 0)
        {
            Homing();
        }

        currentLinearVelocity = rb.linearVelocity;
    }

    private void Homing()
    {
        var colliders = Physics.OverlapSphere(transform.position, homingRange, collisionMask, QueryTriggerInteraction.Ignore);

        if (colliders.Any())
        {
            FindClosest(colliders.Where(collider => collider.tag == "Enemy").ToArray());
        }
        else
        {
            closestEnemy = null;
        }

        if (closestEnemy != null)
        {
            var direction = closestEnemy.transform.position - transform.position;

            if (Vector3.Angle(transform.forward, direction) < 60)
            {
                rb.linearVelocity = Vector3.RotateTowards(rb.linearVelocity, direction, Mathf.PI * Time.deltaTime, 0);
            }
        }
    }

    private void FindClosest(Collider[] colliders)
    {
        float currentClosestDistance = homingRange; 

        foreach (var collider in colliders)
        {
            var distanceToEnemy = (transform.position - collider.transform.position).magnitude;

            if (distanceToEnemy < currentClosestDistance)
            {
                closestEnemy = collider.gameObject;
                currentClosestDistance = distanceToEnemy;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.root.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.ReceiveDamage(Data.Damage);

            if (numberPrefab != null && damageable.IsAlive && collision.transform.tag != "Player")
            {
                DamageNumber damageNumber = numberPrefab.Spawn(collision.transform.position, Data.Damage);
            }

            Destroy(gameObject);

            DidShotHitSomething?.Invoke(true);
        }
        else if (bounces > 0)
        {
            var hitNormal = collision.GetContact(0).normal;

            rb.linearVelocity = Vector3.Reflect(currentLinearVelocity, hitNormal);
            transform.forward = rb.linearVelocity.normalized;

            bounces--;
        }
        else
        {
            Destroy(gameObject);
            DidShotHitSomething?.Invoke(false);
        }
    }
    #endregion

    #region coroutines
    public IEnumerator DestroyAfter(float deathTimer)
    {
        yield return new WaitForSeconds(deathTimer);
        Destroy(gameObject);
    }
    #endregion
}
