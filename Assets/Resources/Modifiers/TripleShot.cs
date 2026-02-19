using UnityEngine;
using static Globals;

public class TripleShot : BulletBasedModifier
{
    protected override void OnBulletFiredAction(BulletData data)
    {
        if (TryProcModifier())
        {
            ShootBullet(new BulletData()
            {
                AngularOffsetX = 10,
                AngularOffsetY = 0,
                BulletSpeed = 35,
                Damage = data.Damage * 0.50f
            });
        
            ShootBullet(new BulletData()
            {
                AngularOffsetX = -10,
                AngularOffsetY = 0,
                BulletSpeed = 35,
                Damage = data.Damage * 0.50f
            });
        }
    }
}
