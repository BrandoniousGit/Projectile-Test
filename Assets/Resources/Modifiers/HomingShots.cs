public class HomingShots : BulletBasedModifier
{
    protected override void EvaluateShotModifiers(BulletBase bulletBase)
    {
        if (TryProcModifier())
        {
            bulletBase.ModifyHoming(EffectAmount, EffectAmount);
        }
    }
}
