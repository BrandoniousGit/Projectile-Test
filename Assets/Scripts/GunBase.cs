using System;
using System.Collections;
using System.Data;
using System.Linq;
using UnityEngine;
using static Globals;

public class GunBase : EnhanceableBase
{
    private GameObject BulletGO;
    private bool canShoot;
    public float spinUp;

    public float publicDamage;

    public WeaponData WeaponData;
    public LayerMask hitLayers;

    public static event Action<BulletData> OnBulletFiredEvent;

    protected override void Update()
    {
        base.Update();

        if (canShoot)
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

        if (Input.GetMouseButtonUp(0))
        {
            spinUp = WeaponData.m_shotSpeedRampRange.x;
        }
    }

    private void EvaluateShot()
    {
        switch (WeaponData.m_gunType)
        {
            case GunType.Single:
                ShootBullet();
                StartCoroutine("ShotDelay");
                break;
            case GunType.Burst:
                StartCoroutine("StartBurst");
                break;
            case GunType.Shotgun:
                for (int i = 0; i < WeaponData.m_pelletCount; i++)
                {
                    ShootBullet();
                }
                StartCoroutine("ShotDelay");
                break;            
            case GunType.BarrelSpin:
                ShootBullet();
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

    public void SetupWeapon()
    {
        spinUp = WeaponData.m_shotSpeedRampRange.x;
    }

    public float CalculateWeaponDamage()
    {
        float calculatedDamage = WeaponData.m_baseDamage;

        var filteredModifiers = modifiers
            .Where(modifierBase => modifierBase.Data != null && modifierBase.Data.ModifiesStat == ModifiesStat.Damage)
            .OrderBy(modifierBase => modifierBase.Data.ApplyType).ToList();

        foreach (var modifier in filteredModifiers)
        {
            if (modifier.Data.ApplyType == ModifierApplyType.Additive)
            {
                calculatedDamage += WeaponData.m_baseDamage * (modifier.EffectAmount / 100);
            }
            else
            {
                calculatedDamage *= modifier.EffectAmount / 100;
            }
        }

        return calculatedDamage;
    }    
    
    public float CalculateShotDelay()
    {
        float calculatedShotDelay = WeaponData.m_timeBetweenShot;

        var filteredModifiers = modifiers
            .Where(modifierBase => modifierBase.Data != null && modifierBase.Data.ModifiesStat == ModifiesStat.ShotDelay)
            .OrderBy(modifierBase => modifierBase.Data.ApplyType).ToList();

        foreach (var modifier in filteredModifiers)
        {
            if (modifier.Data.ApplyType == ModifierApplyType.Additive)
            {
                calculatedShotDelay -= WeaponData.m_timeBetweenShot * (modifier.EffectAmount / 100);
            }
            else
            {
                calculatedShotDelay /= modifier.EffectAmount / 100;
            }
        }

        if (WeaponData.m_gunType == GunType.BarrelSpin)
        {
            if (spinUp < WeaponData.m_shotSpeedRampRange.y)
            {
                var increase = (WeaponData.m_shotSpeedRampRange.y - WeaponData.m_shotSpeedRampRange.x) / WeaponData.m_shotsUntilFullSpeed;
                spinUp += increase;

                if (spinUp > WeaponData.m_shotSpeedRampRange.y)
                {
                    spinUp = WeaponData.m_shotSpeedRampRange.y;
                }
            }

            calculatedShotDelay /= spinUp;
        }

        return calculatedShotDelay;
    }

    public void ShootBullet()
    {
        var data = new BulletData()
        {
            Damage = CalculateWeaponDamage(),
            Spread = WeaponData.m_aimSpread,
            BulletSpeed = WeaponData.m_bulletSpeed,
        };

        publicDamage = data.Damage;

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

            var bulletObj = Instantiate(BulletGO, pos, Quaternion.LookRotation(forward), null);
            var bulletBase = bulletObj.GetComponent<BulletBase>();
            bulletBase.SetData(data);
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
                StartCoroutine(SpawnTrail(trail, transform.position + forward * 50));
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

    private void OnDisable()
    {
        canShoot = false;

        OnModifiersChanged -= SetupWeapon;
    }

    #region coroutines
    IEnumerator StartBurst()
    {
        var shotDelay = CalculateShotDelay();

        canShoot = false;
        for (int i = 0; i < WeaponData.m_burstCount; i++)
        {
            ShootBullet();
            yield return new WaitForSeconds(WeaponData.m_timeBetweenBurst);
        }

        yield return new WaitForSeconds(shotDelay);
        canShoot = true;
    }    
    
    IEnumerator ShotDelay()
    {
        var shotDelay = CalculateShotDelay();

        canShoot = false;
        yield return new WaitForSeconds(shotDelay);
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
