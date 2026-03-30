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

    private BulletData Data;
    private Rigidbody rb;
    private float homingRange;
    private int bounces;
    private float currentGravity, gravity;
    private GameObject closestEnemy;

    public static event Action<BulletBase> UpdateBulletBase;
    public static event Action<bool> DidShotHitSomething;

    #region setup
    private void OnEnable()
    {
        StartCoroutine("DestroyAfter", deathTimer);
        rb = GetComponent<Rigidbody>();

        UpdateBulletBase?.Invoke(this);
    }
    #endregion

    #region public methods
    public void SetData(BulletData data)
    {
        Data = data;
    }

    public void ModifyGravity(float gravity)
    {
        this.gravity = -gravity;
    }      
    
    public void ModifyHoming(float homingRange)
    {
        this.homingRange = homingRange;
    }    
    
    public void ModifyBounces(int bounces)
    {
        this.bounces += bounces;
    }
    #endregion

    #region private methods
    private void Update()
    {
        currentGravity += gravity * Time.deltaTime;

        rb.linearVelocity = Data.BulletSpeed * transform.forward;
        rb.linearVelocity += new Vector3(0, currentGravity, 0);

        if (homingRange > 0)
        {
            Homing();
        }
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
                transform.forward = Vector3.RotateTowards(transform.forward, direction, Mathf.PI * Time.deltaTime, 0);
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
        if (Data != null)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                collision.gameObject.GetComponent<IDamageable>().RecieveDamage(Data.Damage);
                Destroy(gameObject);

                DidShotHitSomething?.Invoke(true);
            }
            else if (bounces > 0)
            {
                var hitNormal = collision.GetContact(0).normal;
                var newForward = transform.forward - 2 * Vector3.Dot(transform.forward, hitNormal) * hitNormal;
                transform.forward = newForward;

                bounces--;
            }
            else
            {
                Destroy(gameObject);
                DidShotHitSomething?.Invoke(false);
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
