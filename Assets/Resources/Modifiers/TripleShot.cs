using UnityEngine;
using static Globals;

public class TripleShot : BulletBasedModifier<ModifierData>
{
    protected override void OnBulletFired(WeaponStats data)
    {
        if (TryProcModifier(20))
        {
            ShootBullet(new BulletData()
            {
                AngularOffsetX = 10,
                AngularOffsetY = 0,
                BulletSpeed = data.BulletSpeed.Value,
                Damage = data.Damage.Value * 0.5f
            });
        
            ShootBullet(new BulletData()
            {
                AngularOffsetX = -10,
                AngularOffsetY = 0,
                BulletSpeed = data.BulletSpeed.Value,
                Damage = data.Damage.Value * 0.5f
            });
        }
    }
}
