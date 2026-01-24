using System.Collections;
using UnityEngine;

public class BulletBase : MonoBehaviour
{
    public float deathTimer;
    public LayerMask collisionMask;

    private void OnEnable()
    {
        StartCoroutine("DestroyAfter", deathTimer);
    }

    public IEnumerator DestroyAfter(float deathTimer)
    {
        yield return new WaitForSeconds(deathTimer);
        Destroy(gameObject);
    }
}
