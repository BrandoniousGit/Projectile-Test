using DamageNumbersPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Enums;

public class ExplosionManager : MonoBehaviour
{
    public static ExplosionManager Instance;
    public GameObject explosionPrefab;

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        explosionPrefab = (GameObject)Resources.Load("Explosion");
    }

    public void CreateExplosion(Vector3 pos, float radius = 0, float damage = 0, Team team = Team.None)
    {
        var overlapSphereResults = Physics.OverlapSphere(pos, radius);
        var colliders = overlapSphereResults.Select(collider => collider.transform.root).Distinct().ToArray();

        if (colliders.Any())
        {
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    switch (team)
                    {
                        case Team.Enemy:
                            if (damageable.IsAlive && collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                            {
                                damageable.ReceiveDamage(damage);
                            }
                            break;
                        case Team.Player:
                            if (damageable.IsAlive && collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                            {
                                damageable.ReceiveDamage(damage);
                            }
                            break;
                        case Team.None:
                            if (damageable.IsAlive)
                            {
                                damageable.ReceiveDamage(damage);
                            }
                            break;
                    }
                }
            }
        }

        if (explosionPrefab == null)
        {
            Debug.Log("Explosion Prefab is not set!");
            return;
        }

        var effect = Instantiate(explosionPrefab, pos, Quaternion.identity);
        effect.transform.localScale = Vector3.one * radius;
    }
}
