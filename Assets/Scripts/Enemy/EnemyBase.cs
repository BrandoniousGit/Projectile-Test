using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour, IDamageable
{
    public float MaxHealth;
    public float Health;

    private float timer;
    private bool hitRecently;

    public Image HealthBar;
    public Image WhiteBar;

    public void RecieveDamage(float damage)
    {
        Health -= damage;
        timer = 0.4f;
    }

    private void OnEnable()
    {
        Health = MaxHealth;
    }

    private void Update()
    {
        HealthBar.fillAmount = Health / MaxHealth;

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0)
        {
            hitRecently = false;
            WhiteBar.fillAmount = Mathf.Lerp(WhiteBar.fillAmount, Health / MaxHealth, 0.1f);
        }

        if (Health < 0)
        {
            Destroy(gameObject);
        }
    }
}