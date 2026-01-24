using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Globals
{
    public class BulletData
    {
        public float SpreadX { get; set; }
        public float SpreadY { get; set; }
        public float BulletSpeed { get; set; }
    }

    public static Dictionary<Rarity, string> RarityLookup = new()
    {
        { Rarity.Common, "#DBF6E1" },
        { Rarity.Uncommon, "#42CF60" },
        { Rarity.Rare, "#27C0DB" },
        { Rarity.Epic, "#A537E5" },
        { Rarity.Legendary, "#A537E5" }
    };

    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public static Color HexCodeToColour(string hex, float alpha)
    {
        hex = new string(hex.Where(character => character != '#').ToArray());

        float r = (hex[0] + hex[1] * 16) / 255;
        float g = (hex[2] + hex[3] * 16) / 255;
        float b = (hex[4] + hex[5] * 16) / 255;

        Debug.Log($"{r},{g},{b},{alpha}");

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
}