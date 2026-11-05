using UnityEngine;
using Enums;

public class LaserTripmine : MonoBehaviour, IDamageable
{
    public bool IsAlive => health > 0;
    public LayerMask layermask;

    public float health;

    private LineRenderer lr;

    public void ReceiveDamage(float damage)
    {
        health -= damage;
    }

    public void OnEnable()
    {
        lr = gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsAlive)
        {
            if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, Mathf.Infinity, layermask))
            {
                if (hitInfo.transform.root.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    ExplosionManager.Instance.CreateExplosion(transform.position, radius: 1, team: Team.Enemy);
                    hitInfo.transform.root.GetComponent<IDamageable>().ReceiveDamage(10);

                    Destroy(gameObject);
                }

                lr.SetPositions(new Vector3[] {transform.position, hitInfo.point});
            }
            else
            {
                lr.SetPositions(new Vector3[] { transform.position, transform.position + transform.forward * 50 });
            }
        }
        else
        {
            if (lr != null)
            {
                ExplosionManager.Instance.CreateExplosion(transform.position, radius: 1, team: Team.Enemy);
            }

            Destroy(lr);
        }
    }
}
