using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/Create Weapon Data")]
public class WeaponData : ScriptableObject
{
	public GameObject m_model;
	//public GameObject m_projectile;

	public enum GunType
	{
		Burst,        //Burst Firing Weapons
		FullAuto,     //Full Automatic
		SemiAuto,     //Semi Automatic
		Shotgun,      //Shotgun Type
		BarrelSpin    //Mini-gun Type
	}

	public GunType m_gunType;
	[Header("String")]
	public string m_gunName;
	public string m_gunDescription;
	[Header("Integer")]
	public int m_clipSize;
	public int m_reserveMax;
	[Header("Float")]
	public float m_chargeTime;
	public float m_damage;
	public float m_reloadTime;
	public float m_timeBetweenShot;
	public float m_aimSpread;
	[Header("Other")]
	[Tooltip("X = Min Distance | Y = Max Distance")]
	public Vector2 m_damageFalloff;
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