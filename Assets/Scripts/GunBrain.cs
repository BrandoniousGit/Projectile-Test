using com.cyborgAssets.inspectorButtonPro;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Globals;

public class GunBrain : MonoBehaviour
{
    public List<ModifierBase> modifiers;

    private Camera Cam;
    public GameObject BulletGO;

    public static event Action OnBulletFired;
    public static event Action OnModifierChanged;

    void Start()
    {
        Cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnBulletFired?.Invoke();

            ShootBullet(new BulletData()
            { 
                BulletSpeed = 35
            });
        }
    }

    public GameObject ShootBullet(BulletData data)
    {
        var bulletObj = Instantiate(BulletGO, transform.position, transform.rotation, null);

        Vector3 forward = transform.forward;
        forward = Quaternion.AngleAxis(data.SpreadX, transform.up) * forward;
        forward = Quaternion.AngleAxis(data.SpreadY, transform.right) * forward;

        bulletObj.GetComponent<Rigidbody>().linearVelocity = forward * data.BulletSpeed;

        return bulletObj;
    }

    [ProButton]
    public void UpdateModifier(bool IsUpgrading, string ModifierType)
    {
        if (string.IsNullOrWhiteSpace(ModifierType)) return;

        var type = Type.GetType(ModifierType);

        if (type != null)
        {
            if (modifiers.Select(item => item.Data.InternalName).Contains(ModifierType))
            {
                ModifierBase existingModifier = (ModifierBase)gameObject.GetComponent(type);

                if (IsUpgrading)
                {
                    if (existingModifier.ModifierLevel < existingModifier.Data.MaxLevel)
                    {
                        existingModifier.ModifierLevel++;
                    }
                }
                else 
                {
                    if (existingModifier.ModifierLevel > 1)
                    {
                        existingModifier.ModifierLevel--;
                    }
                    else
                    {
                        modifiers.Remove(existingModifier);
                        Destroy(existingModifier);
                    }
                }
            }
            else if (IsUpgrading)
            {
                var newModifier = (ModifierBase)gameObject.AddComponent(type);
                newModifier.Data = (ModifierData)Resources.Load($"Modifiers/{ModifierType}_data");
                modifiers.Add(newModifier);
            }
        }

        OnModifierChanged?.Invoke();
    }
}
