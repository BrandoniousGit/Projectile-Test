using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using static Globals;

public class BulletBase : MonoBehaviour
{
    public float deathTimer;
    public LayerMask collisionMask;

    private BulletData Data;
    private Rigidbody rb;
    private float gravity = 0;

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
    public void ModifyGravity(float newGravity)
    {
        gravity = newGravity;
    }    

    public void SetData(BulletData data)
    {
        Data = data;
    }    

    public void ModifyHoming(float homingStrength, float homingRange)
    {
        //DATA IS NULL HERE <-------------------------
        Data.HomingStrength = homingStrength;
        Data.HomingRange = homingRange;
    }
    #endregion

    #region private methods
    private void Update()
    {
        rb.linearVelocity += new Vector3(0, gravity * Time.deltaTime, 0);

        if (Data.HomingStrength > 0)
        {
            var colliders = Physics.OverlapSphere(transform.position, Data.HomingRange, collisionMask, QueryTriggerInteraction.Ignore);

            foreach (var collider in colliders.Where(collider => collider.tag == "Enemy"))
            {
                
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Data != null)
        {
            var normal = (transform.position - collision.transform.position).normalized;

            if (collision.collider.tag == "Enemy")
            {
                collision.collider.GetComponent<IDamageable>().RecieveDamage(Data.Damage);
                Destroy(gameObject);
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
