using System;
using System.Collections;
using UnityEngine;
using static Globals;

public class GunBase : EnhanceableBase
{
    private GameObject BulletGO;
    private bool canShoot;
    private float spinUp;

    public WeaponData BaseWeaponData;
    public LayerMask hitLayers;
    public Transform CameraTransform => Camera.main.transform;

    public static event Action<WeaponStats> OnBulletFired;
    public static event Action<WeaponStats> OnEvaluateModifiers;

    public WeaponStats WeaponStats = new WeaponStats();

    public float Damage;

    protected override void Update()
    {
        base.Update();

        if (canShoot)
        {
            if (BaseWeaponData.m_fullAuto)
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
            spinUp = BaseWeaponData.m_shotSpeedRampRange.x;
        }
    }

    private void EvaluateShot()
    {
        BeforeShot();

        if (BaseWeaponData.m_gunType == GunType.BarrelSpin)
        {
            CalculateSpinUpShotDelay();
        }

        for (int i = 0; i < WeaponStats.BulletCount.Value; i++)
        {
            switch (BaseWeaponData.m_gunType)
            {
                case GunType.Single:
                    ShootBullet();
                    StartCoroutine("WaitShotDelay");
                    break;
                case GunType.Burst:
                    StartCoroutine("StartBurst");
                    break;
                case GunType.BarrelSpin:
                    ShootBullet();
                    StartCoroutine("WaitShotDelay");
                    break;
            }
        }
    }

    private void OnEnable()
    {
        OnModifiersChanged += SetupWeapon;
        BulletGO = (GameObject)Resources.Load("Bullet");

        ClearNullModifiers();
        SetupWeapon();
    }

    public void BeforeShot()
    {
        WeaponStats.Damage = new StackMod(BaseWeaponData.m_baseDamage);
        WeaponStats.ShotDelay = new StackMod(BaseWeaponData.m_timeBetweenShot);
        WeaponStats.Spread = new StackMod(BaseWeaponData.m_aimSpread);
        WeaponStats.BulletSpeed = new StackMod(BaseWeaponData.m_bulletSpeed);
        WeaponStats.BulletCount = new StackMod(BaseWeaponData.m_pelletCount);

        OnEvaluateModifiers?.Invoke(WeaponStats);

        Damage = WeaponStats.Damage.Value;
    }

    public void SetupWeapon()
    {
        BeforeShot();

        spinUp = BaseWeaponData.m_shotSpeedRampRange.x;
        canShoot = true;
    }
    
    public void CalculateSpinUpShotDelay()
    {
        if (spinUp < BaseWeaponData.m_shotSpeedRampRange.y)
        {
            var increase = (BaseWeaponData.m_shotSpeedRampRange.y - BaseWeaponData.m_shotSpeedRampRange.x) / BaseWeaponData.m_shotsUntilFullSpeed;
            spinUp += increase;

            if (spinUp > BaseWeaponData.m_shotSpeedRampRange.y)
            {
                spinUp = BaseWeaponData.m_shotSpeedRampRange.y;
            }
        }

        WeaponStats.ShotDelay.AddMod(spinUp, ModifierApplyType.Multiplicative);
    }

    public void ShootBullet()
    {
        var data = new BulletData()
        {
            Damage = WeaponStats.Damage.Value,
            Spread = WeaponStats.Spread.Value,
            BulletSpeed = WeaponStats.BulletSpeed.Value
        };

        Vector3 forward = (CameraTransform.position + CameraTransform.forward * 100 - transform.position).normalized;
        bool hasHitSomething = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, Mathf.Infinity, hitLayers, QueryTriggerInteraction.Ignore);

        if (hasHitSomething)
        {
            forward = (hit.point - transform.position).normalized;
        }

        //Setting up spread if using
        Vector2 unitCircle = UnityEngine.Random.insideUnitCircle;
        unitCircle *= data.Spread;

        forward = Quaternion.AngleAxis(data.AngularOffsetX + unitCircle.x, transform.up) * forward;
        forward = Quaternion.AngleAxis(data.AngularOffsetY + unitCircle.y, transform.right) * forward;

        //Projectile shots
        if (!BaseWeaponData.m_isHitscan)
        {
            var pos = data.Position.HasValue ? data.Position.Value : transform.position;

            var bulletObj = Instantiate(BulletGO, pos, Quaternion.LookRotation(forward), null);
            bulletObj.transform.localScale *= BaseWeaponData.m_bulletSize;
            var bulletBase = bulletObj.GetComponent<BulletBase>();
            bulletBase.SetData(data);
        }

        //Hitscan shots
        else
        {
            if (hasHitSomething)
            {
                WhatDidIHit(hit, data);

                TrailRenderer trail = Instantiate(BaseWeaponData.m_bulletTrail, /*weaponMuzzlePos*/ transform.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, hit.point));
            }
            else //Bullet miss
            {
                TrailRenderer trail = Instantiate(BaseWeaponData.m_bulletTrail, /*weaponMuzzlePos*/ transform.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, transform.position + forward * 50));
            }
        }

        OnBulletFired?.Invoke(WeaponStats);
        Damage = WeaponStats.Damage.Value;
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
        OnModifiersChanged -= SetupWeapon;
        canShoot = false;
    }

    #region coroutines
    IEnumerator StartBurst()
    {
        canShoot = false;
        for (int i = 0; i < BaseWeaponData.m_burstCount; i++)
        {
            ShootBullet();
            yield return new WaitForSeconds(BaseWeaponData.m_timeBetweenBurst);
        }

        yield return new WaitForSeconds(WeaponStats.ShotDelay.Value);
        canShoot = true;
    }    
    
    IEnumerator WaitShotDelay()
    {
        canShoot = false;
        yield return new WaitForSeconds(WeaponStats.ShotDelay.Value);
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
