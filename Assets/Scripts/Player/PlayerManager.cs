using System;
using UnityEngine;
using static Globals;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public GameObject PlayerInstance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(this);
        Instance = this;

        if (PlayerInstance == null) PlayerInstance = GameObject.FindGameObjectWithTag("Player");
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
}
