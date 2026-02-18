using com.cyborgAssets.inspectorButtonPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Globals;

public class GunBrain : EnhanceableBase
{
    public WeaponData WeaponData;
    public GameObject BulletGO;
    public LayerMask hitLayers;

    public float bulletGravity;

    private float BaseDamage => WeaponData?.m_damage ?? 1;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CallMyActionPerformedEvent();

            ShootBullet(new BulletData()
            { 
                BulletSpeed = 10,
                Damage = CalculatedDamage
            });
        }
    }

    private void OnEnable()
    {
        ClearNullModifiers();
        OnModifiersChanged += CalculateWeaponStats;

        BulletGO = (GameObject)Resources.Load("Bullet");

        if (modifiers.Any())
        {
            CalculateWeaponStats();
        }
    }

    private void OnDisable()
    {
        OnModifiersChanged -= CalculateWeaponStats;
    }

    public void CalculateWeaponStats()
    {
        CalculatedDamage = BaseDamage;

        foreach (var modifier in modifiers.OrderBy(modifierBase => modifierBase.Data.ApplyType))
        {
            switch (modifier.Data.ModifiesStat)
            {
                case ModifiesStat.Damage:
                    if (modifier.Data.ApplyType == ModifierApplyType.Additive)
                    {
                        CalculatedDamage += BaseDamage * (modifier.Data.EffectAmountPerLevel[modifier.ModifierLevel - 1] / 100);
                    }
                    else
                    {
                        CalculatedDamage *= modifier.Data.EffectAmountPerLevel[modifier.ModifierLevel - 1] / 100;
                    }
                    break;
            }
        }
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
