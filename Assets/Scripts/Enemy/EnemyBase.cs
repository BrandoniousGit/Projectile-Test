using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Globals;

public class EnemyBase : MonoBehaviour, IDamageable
{
    public bool IsAlive { get; set; }
    public Rigidbody rb;

    public float MaxHealth;
    public float Health;

    public float DetectionRange = -1;
    public bool ShouldLookAtPlayer;

    private float timer;
    private GameObject EnemyBullet;
    public EnemyState CurrentState;

    public Image HealthBar;
    public Image WhiteBar;

    public void ReceiveDamage(float damage)
    {
        Health -= damage;
        timer = 0.4f;
    }

    protected virtual void OnEnable()
    {
        Health = MaxHealth;
        IsAlive = true;
        //EnemyBullet = (GameObject)Resources.Load("EnemyBullet");
    }

    protected virtual void Update()
    {
        if (ShouldLookAtPlayer)
        {
            TryLookAtPlayer();
        }

        if (HealthBar != null && WhiteBar != null)
        {
            HealthBar.fillAmount = Health / MaxHealth;

            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }

            if (timer <= 0)
            {
                WhiteBar.fillAmount = Mathf.Lerp(WhiteBar.fillAmount, Health / MaxHealth, 0.1f);
            }
        }

        Health = Mathf.Clamp(Health, 0, MaxHealth);

        if (Health <= 0 && IsAlive)
        {
            IsAlive = false;
            StartCoroutine("CleanUpAfter", 5.0f);

            Rigidbody[] rigidbodies = gameObject.GetComponentsInChildren<Rigidbody>();

            foreach (var rb in rigidbodies)
            {
                rb.isKinematic = false;
                rb.AddForce(2, 10, 2);
            }
        }
    }

    private void TryLookAtPlayer()
    {
        var playerPos = PlayerManager.Instance.GetPlayerPosition();
        LayerMask mask = LayerMask.GetMask("Player");

        Debug.DrawRay(transform.position, playerPos - transform.position, Color.red, Time.deltaTime);

        if (Physics.Raycast(transform.position, playerPos - transform.position, Mathf.Infinity, mask, QueryTriggerInteraction.Ignore))
        {
            var targetPos = new Vector3(playerPos.x, transform.position.y, playerPos.z);
            transform.LookAt(targetPos);
        }
    }

    public IEnumerator CleanUpAfter(float timer)
    {
        yield return new WaitForSeconds(timer);
        Destroy(gameObject);
    }
}