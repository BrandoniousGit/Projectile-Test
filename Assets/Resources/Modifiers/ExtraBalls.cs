using UnityEngine;
using static Globals;

public class ExtraBalls : BulletBasedModifier
{
    protected override void OnActionPerformedEvent()
    {
        if (TryProcModifier())
        {
            var data = new BulletData()
            {
                Spread = 15,
                CustomGravity = -10,
                BulletSpeed = 30,
                Damage = AttachedEnhanceableBase.CalculatedDamage * 0.2f
            };

            for (int i = 0; i < EffectAmount; i++)
            {
                ShootBullet(data);
            }
        }
    }
}
