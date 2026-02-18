using UnityEngine;
using static Globals;

public class TripleShot : BulletBasedModifier
{
    protected override void OnActionPerformedEvent()
    {
        if (TryProcModifier())
        {
            ShootBullet(new BulletData()
            {
                AngularOffsetX = 10,
                AngularOffsetY = 0,
                BulletSpeed = 35,
                Damage = AttachedEnhanceableBase.CalculatedDamage * 0.50f
            });
        
            ShootBullet(new BulletData()
            {
                AngularOffsetX = -10,
                AngularOffsetY = 0,
                BulletSpeed = 35,
                Damage = AttachedEnhanceableBase.CalculatedDamage * 0.50f
            });
        }
    }
}
