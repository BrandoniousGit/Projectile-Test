using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Globals
{
    public class BulletData
    {
        public float HomingStrength { get; set; }
        public float HomingRange { get; set; }
        public float AngularOffsetX { get; set; }
        public float AngularOffsetY { get; set; }
        public float Spread { get; set; }
        public float BulletSpeed { get; set; }
        public float Damage { get; set; }
        public float? CustomGravity { get; set; }
        public bool IsHitscan { get; set; }
        public Vector3? Normal { get; set; }
        public Vector3? Position { get; set; }
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
    
    public enum ModifiesStat
    {
        None,
        Damage,
        BulletSpeed
    }    
    #endregion

    #region public static methods
    public static Color HexCodeToColour(string hex, float alpha)
    {
        hex = new string(hex.Where(character => character != '#').ToArray());

        float r = (hex[1] + hex[0] * 16) / 255;
        float g = (hex[3] + hex[2] * 16) / 255;
        float b = (hex[5] + hex[4] * 16) / 255;

        return new Color(r, g, b, alpha);
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