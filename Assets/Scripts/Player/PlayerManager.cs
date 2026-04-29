using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using static Globals;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    private GameObject PlayerInstance;
    private float timer;

    public Image healthBar, whiteBar;
    public float maxHealth, health;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(this);
        Instance = this;

        if (PlayerInstance == null) PlayerInstance = GameObject.FindGameObjectWithTag("Player");

        health = maxHealth;
    }

    public void SetPlayerFrozen()
    {
        if (PlayerInstance.TryGetComponent<LocalPlayerMove>(out var instance))
        {
            instance.FreeMe();
        }        
        
        if (PlayerInstance.TryGetComponent<CameraMove>(out var camInstance))
        {
            camInstance.FreeMe();
        }
    }

    public Vector3 GetPlayerPosition()
    {
        return PlayerInstance.transform.position;
    }    
    
    public void SetPlayerPosition(Vector3 pos)
    {
        PlayerInstance.transform.position = pos;
    }

    public void RecieveDamage(float damage)
    {
        health -= damage;
        timer = 0.4f;
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null || whiteBar == null)
        {
            return;
        }

        healthBar.fillAmount = health / maxHealth;

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0)
        {
            whiteBar.fillAmount = Mathf.Lerp(whiteBar.fillAmount, health / maxHealth, 0.1f);
        }

        health = Mathf.Clamp(health, 0, maxHealth);
    }

    void Update()
    {
        UpdateHealthBar();
    }
}
