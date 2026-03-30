using UnityEngine;
using static Globals;

public class ChainShot : BulletBasedModifier<ChainShotData>
{
    public int hitsInARow = 0;
    public float EffectAmount;

    protected override void OnBulletFired(WeaponStats data)
    {
        data.Damage.AddMod(EffectAmount, ModifierApplyType.Additive);
    }

    protected override void OnBulletExpire(bool hasHit)
    {
        if (hasHit)
        {
            hitsInARow += 1;
        }
        else
        {
            hitsInARow = 0;
            EffectAmount = 0;
        }

        EffectAmount = 1 + (0.01f * Data.EffectAmountPerLevel[ModifierLevel - 1] * hitsInARow);

        if (EffectAmount >= Data.EffectAmountCap)
        {
            EffectAmount = Data.EffectAmountCap;
        }
    }

}

[CreateAssetMenu(fileName = "New Modifier", menuName = "New Modifier/Chain Shot")]
public class ChainShotData : ModifierData
{
    public float EffectAmountCap;
    public float[] EffectAmountPerLevel;
}