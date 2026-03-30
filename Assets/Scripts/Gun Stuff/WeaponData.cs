using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Globals;

[CreateAssetMenu(fileName = "New WeaponData", menuName = "Gear/Weapons/Create Weapon Data")]
public class WeaponData : ScriptableObject
{
	public GameObject m_model;
	//public GameObject m_projectile;

	public GunType m_gunType;
	public bool m_fullAuto;
	public bool m_isHitscan;
	[Header("String")]
	public string m_gunName;
	public string m_gunDescription;
	[Header("Integer")]
	public int m_clipSize;
	public int m_reserveMax;
	[Header("Float")]
	public float m_chargeTime;
	public float m_shotsUntilFullSpeed;
	public float m_baseDamage;
	public float m_bulletSpeed;
	public float m_bulletSize = 1;
	public float m_reloadTime;
	public float m_timeBetweenShot;
	public float m_aimSpread;
	[Header("Other")]
	[Tooltip("X = Min Distance | Y = Max Distance")]
	public Vector2 m_damageFalloff;
    [Tooltip("X = Slowest Shot Speed | Y = Fastest Shot Speed")]
    public Vector2 m_shotSpeedRampRange;
	public TrailRenderer m_bulletTrail;
	public ParticleSystem m_ImpactParticleSystem;
	public AudioClip m_shotSound;

	[Header("Shotgun Only")]
	public int m_pelletCount = 1;
	//public float m_coneSize;
	[Header("Burst Only")]
	public float m_timeBetweenBurst;
	public int m_burstCount;
}