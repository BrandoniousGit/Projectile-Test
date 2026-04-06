using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using static Globals;

public class GunBase : EnhanceableBase
{
    public GameObject BulletGO;
    private bool canShoot;
    private float spinUp;
    private int hitscanBouncesUsed;

    public WeaponData BaseWeaponData;
    public LayerMask hitLayers;
    public Transform CameraTransform => Camera.main.transform;

    public static event Action<bool> DidShotHitSomething;
    public static event Action<WeaponStats> OnBulletFired;
    public static event Action<WeaponStats> OnEvaluateModifiers;

    public WeaponStats WeaponStats = new WeaponStats();

    public float Damage;

    #region protected methods
    protected override void OnEnable()
    {
        base.OnEnable();

        if (BulletGO == null) BulletGO = (GameObject)Resources.Load("Bullet");
        spinUp = BaseWeaponData.m_shotSpeedRampRange.x;
        canShoot = true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        canShoot = false;
    }

    protected override void Update()
    {
        base.Update();

        if (canShoot)
        {
            if (BaseWeaponData.m_fullAuto)
            {
                if (Input.GetMouseButton(0))
                {
                    TryShoot();
                }                
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    TryShoot();
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            spinUp = BaseWeaponData.m_shotSpeedRampRange.x;
        }
    }
    #endregion

    #region private methods
    public void TryShoot(bool doShotDelay = true)
    {
        #region modifier setup
        WeaponStats.Damage = new StackMod(BaseWeaponData.m_baseDamage);
        WeaponStats.FireRate = new StackMod(BaseWeaponData.m_fireRate);
        WeaponStats.Spread = new StackMod(BaseWeaponData.m_aimSpread);
        WeaponStats.BulletSpeed = new StackMod(BaseWeaponData.m_bulletSpeed);
        WeaponStats.BulletCount = new StackMod(BaseWeaponData.m_pelletCount);
        WeaponStats.Gravity = new StackMod(1, float.MinValue);
        WeaponStats.Bounces = new StackMod(1);

        OnEvaluateModifiers?.Invoke(WeaponStats);
        #endregion

        Damage = WeaponStats.Damage.Value;

        Vector3 forward = (CameraTransform.position + CameraTransform.forward * 100 - transform.position).normalized;

        if (BaseWeaponData.m_gunType == GunType.BarrelSpin)
        {
            CalculateSpinUpShotDelay();
        }

        for (int i = 0; i < WeaponStats.BulletCount.Value; i++)
        {
            switch (BaseWeaponData.m_gunType)
            {
                case GunType.Single:
                case GunType.BarrelSpin:

                    ShootBullet(forward);

                    if (doShotDelay)
                    {
                        StartCoroutine("WaitShotDelay");
                    }

                    break;
                case GunType.Burst:
                    StartCoroutine("StartBurst", forward);
                    break;
            }
        }

        OnBulletFired?.Invoke(WeaponStats);
        Damage = WeaponStats.Damage.Value;
    }

    private void CalculateSpinUpShotDelay()
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

        WeaponStats.FireRate.AddMod(spinUp, ModifierApplyType.Multiplicative);
    }

    private void ShootBullet(Vector3 forwardVector, Vector3? origin = null)
    {
        var data = new BulletData()
        {
            Damage = WeaponStats.Damage.Value,
            Spread = WeaponStats.Spread.Value,
            BulletSpeed = WeaponStats.BulletSpeed.Value,
            UseGravity = WeaponStats.UsesGravity,
            Gravity = WeaponStats.Gravity.Value,
            Bounces = WeaponStats.Bounces.Value
        };

        //Projectile shots
        if (!BaseWeaponData.m_isHitscan)
        {
            bool somethingOnCrosshair = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, Mathf.Infinity, hitLayers, QueryTriggerInteraction.Ignore);

            if (somethingOnCrosshair)
            {
                forwardVector = (hit.point - transform.position).normalized;
            }

            //Setting up spread if using
            Vector2 unitCircle = UnityEngine.Random.insideUnitCircle;
            unitCircle *= data.Spread;

            forwardVector = Quaternion.AngleAxis(data.AngularOffsetX + unitCircle.x, transform.up) * forwardVector;
            forwardVector = Quaternion.AngleAxis(data.AngularOffsetY + unitCircle.y, transform.right) * forwardVector;

            var pos = data.Position.HasValue ? data.Position.Value : transform.position;

            var bulletObj = Instantiate(BulletGO, pos, Quaternion.LookRotation(forwardVector), null);
            bulletObj.transform.localScale *= BaseWeaponData.m_bulletSize;
            var bulletBase = bulletObj.GetComponent<BulletBase>();
            bulletBase.SetData(data);
        }

        //Hitscan shots
        else
        {
            Vector2 unitCircle = UnityEngine.Random.insideUnitCircle;
            unitCircle *= origin != null ? 0 : data.Spread;

            forwardVector = Quaternion.AngleAxis(data.AngularOffsetX + unitCircle.x, transform.up) * forwardVector;
            forwardVector = Quaternion.AngleAxis(data.AngularOffsetY + unitCircle.y, transform.right) * forwardVector;

            bool hitCheck = Physics.Raycast(origin != null ? origin.Value : Camera.main.transform.position, forwardVector, out RaycastHit hit, Mathf.Infinity, hitLayers, QueryTriggerInteraction.Ignore);

            if (hitCheck)
            {
                WhatDidIHit(hit, data, forwardVector);

                LineRenderer trail = Instantiate(BaseWeaponData.m_bulletTrail, /*weaponMuzzlePos*/ origin != null ? origin.Value : transform.position, Quaternion.identity);
                Vector3[] positions = new Vector3[] { trail.transform.position, hit.point };
                trail.SetPositions(positions);
            }
            else //Bullet miss
            {
                LineRenderer trail = Instantiate(BaseWeaponData.m_bulletTrail, /*weaponMuzzlePos*/ origin != null ? origin.Value : transform.position, Quaternion.identity);
                Vector3[] positions = new Vector3[] { trail.transform.position, CameraTransform.position + forwardVector * 100};
                trail.SetPositions(positions);

                DidShotHitSomething?.Invoke(false);
            }
        }
    }

    private void WhatDidIHit(RaycastHit _hit, BulletData data, Vector3 forward = new Vector3())
    {
        //Instantiate(weaponData.m_ImpactParticleSystem, _hit.point, Quaternion.LookRotation(_hit.normal));

        if (_hit.transform.gameObject.GetComponent<IDamageable>() != null)
        {
            _hit.transform.gameObject.GetComponent<IDamageable>().RecieveDamage(data.Damage);
            DidShotHitSomething?.Invoke(true);
        }
        else if (_hit.transform.gameObject != null && BaseWeaponData.m_isHitscan && hitscanBouncesUsed < WeaponStats.Bounces.Value - 1)
        {
            hitscanBouncesUsed += 1;
            var newForward = forward - 2 * Vector3.Dot(transform.forward, _hit.normal) * _hit.normal;

            ShootBullet(newForward, _hit.point);
        }
        else
        {
            hitscanBouncesUsed = 0;
            DidShotHitSomething?.Invoke(false);
        }
    }
    #endregion

    #region coroutines
    IEnumerator StartBurst(Vector3 forward)
    {
        canShoot = false;
        for (int i = 0; i < BaseWeaponData.m_burstCount; i++)
        {
            ShootBullet(forward);
            yield return new WaitForSeconds(BaseWeaponData.m_timeBetweenBurst);
        }

        yield return new WaitForSeconds(1 / WeaponStats.FireRate.Value);
        canShoot = true;
    }    
    
    IEnumerator WaitShotDelay()
    {
        canShoot = false;
        yield return new WaitForSeconds(1 / WeaponStats.FireRate.Value);
        canShoot = true;
    }
    #endregion
}
