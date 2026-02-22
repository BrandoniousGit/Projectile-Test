using UnityEngine;

public class ChainShot : BulletBasedModifier
{
    public int hitsInARow = 0;

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

        EffectAmount = Data.EffectAmountPerLevel[ModifierLevel - 1] * hitsInARow;

        if (EffectAmount >= EffectAmountCap)
        {
            EffectAmount = EffectAmountCap;
        }
    }
}
