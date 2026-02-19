using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Globals;

public class GunBase : EnhanceableBase
{
    private BulletData BulletData;
    private GameObject BulletGO;

    [SerializeField]
    private bool canShoot;

    public WeaponData WeaponData;
    public LayerMask hitLayers;

    public static event Action<BulletData> OnBulletFiredEvent;

    protected override void Update()
    {
        base.Update();

        if (canShoot && BulletData != null)
        {
            if (WeaponData.m_fullAuto)
            {
                if (Input.GetMouseButton(0))
                {
                    EvaluateShot();
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    EvaluateShot();
                }
            }
            
        }
    }

    private void EvaluateShot()
    {
        switch (WeaponData.m_gunType)
        {
            case WeaponData.GunType.Single:
                ShootBullet(BulletData);
                StartCoroutine("ShotDelay");
                break;
            case WeaponData.GunType.Burst:
                StartCoroutine("StartBurst");
                break;
            case WeaponData.GunType.Shotgun:
                for (int i = 0; i < WeaponData.m_pelletCount; i++)
                {
                    ShootBullet(BulletData);
                }
                StartCoroutine("ShotDelay");
                break;
        }
    }

    private void OnEnable()
    {
        canShoot = true;

        ClearNullModifiers();
        OnModifiersChanged += SetupWeapon;

        BulletGO = (GameObject)Resources.Load("Bullet");

        SetupWeapon();
    }

    private void OnDisable()
    {
        canShoot = false;

        OnModifiersChanged -= SetupWeapon;
    }

    public void SetupWeapon()
    {
        BulletData = new BulletData()
        {
            Damage = CalculateWeaponDamage(),
            Spread = WeaponData.m_aimSpread,
            BulletSpeed = 15,
        };
    }

    public float CalculateWeaponDamage()
    {
        float calculatedDamage = WeaponData.m_baseDamage;

        foreach (var modifier in modifiers.OrderBy(modifierBase => modifierBase.Data.ApplyType))
        {
            switch (modifier.Data.ModifiesStat)
            {
                case ModifiesStat.Damage:
                    if (modifier.Data.ApplyType == ModifierApplyType.Additive)
                    {
                        calculatedDamage += WeaponData.m_baseDamage * (modifier.Data.EffectAmountPerLevel[modifier.ModifierLevel - 1] / 100);
                    }
                    else
                    {
                        calculatedDamage *= modifier.Data.EffectAmountPerLevel[modifier.ModifierLevel - 1] / 100;
                    }
                    break;
            }
        }

        return calculatedDamage;
    }

    public void ShootBullet(BulletData data)
    {
        //Setting up spread if using
        Vector3 forward = transform.forward;
        Vector2 unitCircle = UnityEngine.Random.insideUnitCircle;
        unitCircle *= data.Spread;

        forward = Quaternion.AngleAxis(data.AngularOffsetX + unitCircle.x, transform.up) * forward;
        forward = Quaternion.AngleAxis(data.AngularOffsetY + unitCircle.y, transform.right) * forward;

        //Projectile shots
        if (!data.IsHitscan)
        {
            var pos = data.Position.HasValue ? data.Position.Value : transform.position;
            var rot = data.Normal.HasValue ? Quaternion.Euler(data.Normal.Value) : transform.rotation;

            var bulletObj = Instantiate(BulletGO, pos, rot, null);
            var bulletRb = bulletObj.GetComponent<Rigidbody>();
            var bulletBase = bulletObj.GetComponent<BulletBase>();
            bulletBase.SetData(data);

            bulletRb.linearVelocity = data.BulletSpeed * forward;
        }

        //Hitscan shots
        else
        {
            if (Physics.Raycast(transform.position, forward, out RaycastHit hit, Mathf.Infinity, hitLayers, QueryTriggerInteraction.Ignore))
            {
                WhatDidIHit(hit, data);

                TrailRenderer trail = Instantiate(WeaponData.m_bulletTrail, /*weaponMuzzlePos*/ transform.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, hit.point));
            }
            else //Bullet miss
            {
                TrailRenderer trail = Instantiate(WeaponData.m_bulletTrail, /*weaponMuzzlePos*/ transform.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, transform.position + (forward) * 50));
            }
        }

        OnBulletFiredEvent?.Invoke(data);
    }

    public void WhatDidIHit(RaycastHit _hit, BulletData data)
    {
        //Instantiate(weaponData.m_ImpactParticleSystem, _hit.point, Quaternion.LookRotation(_hit.normal));

        if (_hit.transform.gameObject.GetComponent<IDamageable>() != null)
        {
            _hit.transform.gameObject.GetComponent<IDamageable>().RecieveDamage(data.Damage);
        }
    }

    #region coroutines
    IEnumerator StartBurst()
    {
        canShoot = false;
        for (int i = 0; i < WeaponData.m_burstCount; i++)
        {
            ShootBullet(BulletData);
            yield return new WaitForSeconds(WeaponData.m_timeBetweenBurst);
        }

        yield return new WaitForSeconds(WeaponData.m_timeBetweenShot);
        canShoot = true;
    }    
    
    IEnumerator ShotDelay()
    {
        canShoot = false;
        yield return new WaitForSeconds(WeaponData.m_timeBetweenShot);
        canShoot = true;
    }

    IEnumerator SpawnTrail(TrailRenderer _trail, Vector3 _hit)
    {
        Vector3 startVec = _trail.transform.position;
        float remainingTime = Vector3.Distance(_trail.transform.position, _hit);
        float endTime = remainingTime;

        //Sets speed of trail to be the same for any distance
        while (remainingTime > 0)
        {
            _trail.transform.position = Vector3.Lerp(startVec, _hit, 1 - (remainingTime / endTime));
            remainingTime -= Time.deltaTime * 200;

            yield return null;
        }

        _trail.transform.position = _hit;

        Destroy(_trail.gameObject, _trail.time);
    }
    #endregion
}
