using UnityEngine;
using static Globals;

public class ShotgunBlast : BulletBasedModifier
{
    protected override void OnActionPerformedEvent()
    {
        var data = new BulletData()
        {
            Spread = 15,
            BulletSpeed = 30,
            Damage = AttachedEnhanceableBase.CalculatedDamage * EffectAmount
        };

        for (int i = 0; i < 6; i++)
        {
            ShootBullet(data);
        }
    }
}
