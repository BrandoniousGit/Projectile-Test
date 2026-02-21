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
    public float HomingStrength;
    private float HomingRange;
    private float gravity;
    private GameObject closestEnemy;

    public static event Action<BulletBase> EvaluateShotModifiers;

    #region setup
    private void OnEnable()
    {
        StartCoroutine("DestroyAfter", deathTimer);
        rb = GetComponent<Rigidbody>();

        EvaluateShotModifiers?.Invoke(this);
    }
    #endregion

    #region public methods
    public void SetData(BulletData data)
    {
        Data = data;
    }    

    public void ModifyHoming(float homingStrength, float homingRange)
    {
        HomingStrength = homingStrength;
        HomingRange = homingRange;
    }
    #endregion

    #region private methods
    private void Update()
    {
        gravity += Data.Gravity * Time.deltaTime;

        rb.linearVelocity = Data.BulletSpeed * transform.forward;
        rb.linearVelocity += new Vector3(0, gravity, 0);

        if (HomingStrength > 0)
        {
            var colliders = Physics.OverlapSphere(transform.position, HomingRange, collisionMask, QueryTriggerInteraction.Ignore);

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
    }

    private void FindClosest(Collider[] colliders)
    {
        float currentClosestDistance = HomingRange; 

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
            var normal = (collision.transform.position - transform.position).normalized;

            if (collision.collider.tag == "Enemy")
            {
                collision.collider.GetComponent<IDamageable>().RecieveDamage(Data.Damage);
            }

            Destroy(gameObject);
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
