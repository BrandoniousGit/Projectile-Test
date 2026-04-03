using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour, IDamageable
{
    public float MaxHealth;
    public float Health;

    private float timer;
    private GameObject EnemyBullet;

    public Image HealthBar;
    public Image WhiteBar;

    public void RecieveDamage(float damage)
    {
        Health -= damage;
        timer = 0.4f;
    }

    protected virtual void OnEnable()
    {
        Health = MaxHealth;
        EnemyBullet = (GameObject)Resources.Load("EnemyBullet");
    }

    protected virtual void Update()
    {
        HealthBar.fillAmount = Health / MaxHealth;

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0)
        {
            WhiteBar.fillAmount = Mathf.Lerp(WhiteBar.fillAmount, Health / MaxHealth, 0.1f);
            Health += 50 * Time.deltaTime;
        }

        Health = Mathf.Clamp(Health, 0, MaxHealth);

        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}