using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Enums;
using static Globals;

public class BulletBase : MonoBehaviour
{
    public float deathTimer;
    public LayerMask collisionMask;
    public Team Team;

    private BulletData Data;
    private Rigidbody rb;
    private Vector3 currentLinearVelocity;
    private float homingRange;
    private int bounces;
    private GameObject closestEnemy;
    private bool initialized;

    public static event Action<BulletBase> UpdateBulletBase;
    public static event Action<bool> DidShotHitSomething;

    private Vector3 previousPosition;

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
    public void Init(BulletData data)
    {
        Data = data;
        bounces = (int)Data.Bounces - 1;

        rb.linearVelocity = Data.BulletSpeed * transform.forward;

        initialized = true;
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
        if (!initialized)
        {
            return;
        }

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

    void FixedUpdate()
    {
        Vector3 currentPosition = transform.position;
        Vector3 delta = currentPosition - previousPosition;

        if (Physics.Raycast(previousPosition, delta.normalized, out RaycastHit hit, delta.magnitude, collisionMask))
        {
            if (hit.transform.TryGetComponent<IDamageable>(out var damageable) || hit.transform.root.TryGetComponent<IDamageable>(out damageable))
            {
                damageable.ReceiveDamage(Data.Damage);

                Destroy(gameObject);

                DidShotHitSomething?.Invoke(true);
            }
            else if (bounces > 0)
            {
                var hitNormal = hit.normal;

                rb.linearVelocity = Vector3.Reflect(currentLinearVelocity, hitNormal);
                transform.forward = rb.linearVelocity.normalized;
                transform.position = hit.point;

                bounces--;
            }
            else
            {
                Destroy(gameObject);
                DidShotHitSomething?.Invoke(false);
            }
        }

        previousPosition = currentPosition;
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
    #endregion

    #region coroutines
    public IEnumerator DestroyAfter(float deathTimer)
    {
        yield return new WaitForSeconds(deathTimer);
        Destroy(gameObject);
    }
    #endregion
}
