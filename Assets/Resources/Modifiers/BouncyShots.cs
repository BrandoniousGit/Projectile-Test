using UnityEngine;
using static Globals;

public class BouncyShots : BulletBasedModifier
{
    protected override void EvaluateShotModifiers(BulletBase bulletBase)
    {
        bulletBase.ModifyBounces((int)EffectAmount);
    }
}
