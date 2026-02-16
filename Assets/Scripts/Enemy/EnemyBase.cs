using UnityEngine;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour, IDamageable
{
    public float MaxHealth;
    public float Health;

    public Image HealthBar;

    public void RecieveDamage(float damage)
    {
        Health -= damage;
    }

    private void OnEnable()
    {
        Health = MaxHealth;
    }

    private void Update()
    {
        HealthBar.fillAmount = Health / MaxHealth;
    }
}