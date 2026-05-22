using System.Collections.Generic;
using UnityEngine;
using static Globals;

public class BasicBulletModifier : BulletBasedModifier<BasicModifierData>
{
    private void Start()
    {
        FormattedDescription = FormatDescription(Data.Description);
    }

    protected override void OnEvaluateModifiers(WeaponStats data)
    {
        switch (Data.ModifiesStat)
        {
            case ModifiesStat.Damage:
                data.Damage.AddMod(1 + (0.01f * Data.ValuePerLevel[ModifierLevel - 1]), Data.ApplyType);
                break;               
            case ModifiesStat.FireRate:
                data.FireRate.AddMod(1 + (0.01f * Data.ValuePerLevel[ModifierLevel - 1]), Data.ApplyType);
                break;            
            case ModifiesStat.Bounces:
                data.Bounces.AddMod(1 + (1f * Data.ValuePerLevel[ModifierLevel - 1]), Data.ApplyType);
                break;            
            case ModifiesStat.Gravity:
                data.Gravity.AddMod(1 + (1f * Data.ValuePerLevel[ModifierLevel - 1]), Data.ApplyType);
                break;
        }
    }

    protected override string GetDescription() 
    {
        return FormatDescription(Data.Description);
    }

    private string FormatDescription(string description)
    {
        description = description.Replace("{VALUE}", Data.ValuePerLevel[ModifierLevel - 1].ToString());
        return description;
    }
}