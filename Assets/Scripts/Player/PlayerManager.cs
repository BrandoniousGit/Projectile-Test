using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour, IDamageable
{
    public static PlayerManager Instance { get; private set; }
    public bool IsAlive { get; set; }

    public PhysicsMaterial noFricc;

    private PlayerMove PlayerInstance;
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

        if (PlayerInstance == null) PlayerInstance = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMove>();

        IsAlive = true;
        health = maxHealth;
    }

    public void SetPlayerFrozen(bool isFrozen)
    {
        PlayerInstance.ToggleFreeze(isFrozen);
        
        if (PlayerInstance.TryGetComponent<CameraMove>(out var camInstance))
        {
            camInstance.ToggleFreeze(isFrozen);
        }
    }

    public bool GetPlayerFrozen()
    {
        return PlayerInstance.GetPlayerFrozen();
    }

    public Vector3 GetPlayerLookDirection()
    {
        return Camera.main.transform.forward;
    }

    public Vector3 GetPlayerPosition()
    {
        return PlayerInstance.transform.position;
    }    
    
    public void SetPlayerPosition(Vector3 pos)
    {
        PlayerInstance.transform.position = pos;
    }

    public void ReceiveDamage(float damage)
    {
        health -= damage;
        timer = 0.4f;
    }

    private void RespawnPlayer()
    {
        if (!IsAlive)
        {
            PlayerInstance.GetComponent<Rigidbody>().freezeRotation = true;
            PlayerInstance.GetComponent<MeshCollider>().material = noFricc;

            health = maxHealth;
            IsAlive = true;

            SetPlayerFrozen(false);
        }
    }

    private void KillPlayer()
    {
        health = 0;
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

        if (health <= 0 && IsAlive)
        {
            IsAlive = false;
            //Run death anim

            var playerRb = PlayerInstance.GetComponent<Rigidbody>();
            playerRb.freezeRotation = false;
            SetPlayerFrozen(true);
        }

        health = Mathf.Clamp(health, 0, maxHealth);
    }

    void Update()
    {
        UpdateHealthBar();

        if (Input.GetKeyDown(KeyCode.O))
        {
            RespawnPlayer();
        }        
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            KillPlayer();
        }
    }
}
