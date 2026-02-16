using System;
using System.Collections;
using UnityEngine;
using static Globals;

public class BulletBase : MonoBehaviour
{
    public float deathTimer;
    public LayerMask collisionMask;

    private BulletData Data;
    private Rigidbody rb;
    private float gravity = 0;

    #region setup
    private void OnEnable()
    {
        StartCoroutine("DestroyAfter", deathTimer);
        rb = GetComponent<Rigidbody>();
    }
    #endregion

    #region public methods
    public void SetGravity(float newGravity)
    {
        gravity = newGravity;
    }    
    public void SetData(BulletData data)
    {
        Data = data;
    }
    #endregion

    #region private methods
    private void Update()
    {
        rb.linearVelocity += new Vector3(0, gravity * Time.deltaTime, 0);
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
