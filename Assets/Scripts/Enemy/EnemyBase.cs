using DamageNumbersPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static Globals;

public class EnemyBase : MonoBehaviour, IDamageable
{
    public bool IsAlive { get; private set; }

    public float maxHealth;
    public float health;

    public float detectionRange = -1;
    public float attackRange = -1;
    public float attackCooldown;
    public bool isBlind;
    public DamageNumber numberPrefab;

    public float headMulti = 1, armorMulti = 1, limbMulti = 1;

    public NavMeshAgent Agent;

    private float timer;
    public EnemyState currentState;
    public LayerMask layermask;

    public Image HealthBar;
    public Image WhiteBar;

    public void ReceiveDamage(float damage)
    {
        if (IsAlive)
        {
            DamageNumber damageNumber = numberPrefab.Spawn(transform.position, damage);
        }

        health -= damage;
        timer = 0.4f;
    }

    public float GetLimbMulti(LimbType limbType)
    {
        switch (limbType)
        {
            case LimbType.Head:
                return headMulti;            
            case LimbType.Body:
                return 1;            
            case LimbType.Armor:
                return armorMulti;            
            case LimbType.Limb:
                return limbMulti;
            default:
                return 1;
        }
    }

    protected virtual void OnEnable()
    {
        health = maxHealth;
        IsAlive = true;
        Agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Update()
    {
        if (!isBlind)
        {
            TryLookAtPlayer(out _);
        }

        if (HealthBar != null && WhiteBar != null)
        {
            HealthBar.fillAmount = health / maxHealth;

            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }

            if (timer <= 0)
            {
                WhiteBar.fillAmount = Mathf.Lerp(WhiteBar.fillAmount, health / maxHealth, 0.1f);
            }
        }

        health = Mathf.Clamp(health, 0, maxHealth);

        if (health <= 0 && IsAlive)
        {
            IsAlive = false;
            gameObject.layer = LayerMask.NameToLayer("EnemyDead");
            StartCoroutine("CleanUpAfter", 5.0f);

            Rigidbody[] rigidbodies = gameObject.GetComponentsInChildren<Rigidbody>();

            foreach (var rb in rigidbodies)
            {
                rb.isKinematic = false;
                rb.AddForce(2, 10, 2);
            }
        }
    }

    public void TryLookAtPlayer(out RaycastHit hitInfo)
    {
        var playerPos = PlayerManager.Instance.GetPlayerPosition();

        if (Physics.Raycast(transform.position, playerPos - transform.position, out hitInfo, detectionRange, layermask, QueryTriggerInteraction.Ignore))
        {
            if (hitInfo.collider.gameObject.tag == "Player")
            {
                var targetPos = new Vector3(playerPos.x, transform.position.y, playerPos.z);
                // TODO: Slow look
                transform.LookAt(targetPos);

                if (hitInfo.distance < attackRange) currentState = EnemyState.Attacking;
                else currentState = EnemyState.Chasing;
            }
        }
        else
        {
            currentState = EnemyState.Idle;
        }
    }

    public IEnumerator CleanUpAfter(float timer)
    {
        yield return new WaitForSeconds(timer);
        Destroy(gameObject);
    }
}