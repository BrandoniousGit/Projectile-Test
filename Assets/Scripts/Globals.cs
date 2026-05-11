using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class Globals
{
    public class BulletData
    {
        public float Damage { get; set; }
        public float Spread { get; set; }
        public float BulletSpeed { get; set; }
        public float AngularOffsetX { get; set; }
        public float AngularOffsetY { get; set; }
        public bool UseGravity { get; set; }
        public float Gravity { get; set; }
        public float Bounces { get; set; }
        public Vector3? Position { get; set; }
        public Team BulletTeam { get; set; }
    }

    public enum ModifiesStat
    {
        None,
        Damage,
        BulletCount,
        BulletSpeed,
        FireRate,
        Spread,
        CooldownDuration,
        Bounces,
        HomingRange,
        Gravity
    }

    public class WeaponStats
    {
        public StackMod Damage;
        public StackMod BulletCount;
        public StackMod BulletSpeed;
        public StackMod FireRate;
        public StackMod Spread;
        public StackMod CooldownDuration;
        public StackMod Bounces;
        public StackMod HomingRange;
        public StackMod Gravity;

        public bool UsesGravity;
        public bool IsHitscan;
    }

    public static Dictionary<Rarity, string> RarityLookup = new()
    {
        { Rarity.Common, "#DBF6E1" },
        { Rarity.Uncommon, "#42CF60" },
        { Rarity.Rare, "#27C0DB" },
        { Rarity.Epic, "#A537E5" },
        { Rarity.Legendary, "#A537E5" }
    };

    #region enums
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }    
    
    public enum EnhanceableItem
    {
        Gun,
        Sword
    }

    public enum ModifierApplyType
    {
        None,
        Additive,
        Multiplicative
    }        
    
    public enum GearType
    {
        Head = 1,
        Chest = 2,
        Legs = 3,
        Feet = 4,
        Accessory = 5
    }

    public enum GunType
    {
        Burst,        //Burst Firing
        Single,       //Semi Automatic
        BarrelSpin,   //Mini-gun Type
        ChargeUp      //Charge before shot
    }

    public enum Team
    {
        Player,
        Enemy,
        Chaotic
    }

    public enum EnemyState
    {
        Idle,
        Aggressive
    }
    #endregion

    #region public static methods
    public static string RemoveFileExtension(this string fileName, int extensionLength)
    {
        return fileName.Remove(fileName.Length - extensionLength);
    }

    public static Color HexCodeToColour(string hex, float alpha)
    {
        hex = new string(hex.Where(character => character != '#').ToArray());

        float r = (hex[1] + hex[0] * 16) / 255;
        float g = (hex[3] + hex[2] * 16) / 255;
        float b = (hex[5] + hex[4] * 16) / 255;

        return new Color(r, g, b, alpha);
    }

    public static bool TryGetDamageableComponent(this Transform go, out IDamageable damageable)
    {
        damageable = null;

        if (go.GetComponent<IDamageable>() != null)
        {
            damageable = go.GetComponent<IDamageable>();
            return true;
        }

        return false;
    }

    public static Vector3 ResetVerticalComponent(this Vector3 inputVec)
    {
        return new Vector3(inputVec.x, 0, inputVec.z);
    }

    public static class WordsList
    {
        public static string[] Prefixes = new string[]
        {
            "Dark", "Light", "Damp", "Orange", "Small", "Fluffy", "Hard", "Elongated", "Right-Handed", "Left-Handed"
        };

        public static string[] Nouns = new string[]
        {
            "Cave", "Lamp", "Pebble", "Sewer", "Lift", "Cavern", "Factory", "Swamp", "Island", "Boat"
        };
    }

    public static string IntToRomalNumeral(this int num)
    {
        switch (num)
        {
            case 1:
                return "I";
            case 2:
                return "II";
            case 3:
                return "III";
            case 4:
                return "IV";
            case 5:
                return "V";
            case 6:
                return "VI";
            case 7:
                return "VII";
            case 8:
                return "VIII";
            case 9:
                return "IX";
            case 10:
                return "X";
            default:
                return "";
        }
    }
    #endregion
}