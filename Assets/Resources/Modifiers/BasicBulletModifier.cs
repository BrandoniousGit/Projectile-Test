using System.Collections.Generic;
using UnityEngine;
using static Globals;

public class BasicBulletModifier : BulletBasedModifier<BasicModifierData>
{
    protected override void OnEvaluateModifiers(WeaponStats data)
    {
        switch (Data.ModifiesStat)
        {
            case ModifiesStat.Damage:
                data.Damage.AddMod(1 + (0.01f * Data.ValuePerLevel[ModifierLevel - 1]), Data.ApplyType);
                break;
        }
    }

}

[CreateAssetMenu(fileName = "New Modifier", menuName = "New Modifier/Basic Modifier")]
public class BasicModifierData : ModifierData
{
    public ModifiesStat ModifiesStat;
    public ModifierApplyType ApplyType;
    public List<float> ValuePerLevel;
}