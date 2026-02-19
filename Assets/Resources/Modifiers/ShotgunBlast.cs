using UnityEngine;
using static Globals;

public class ShotgunBlast : BulletBasedModifier
{
    protected override void OnBulletFiredAction(BulletData data)
    {
        var newData = new BulletData()
        {
            Spread = 15,
            BulletSpeed = 30,
            Damage = data.Damage * EffectAmount
        };

        for (int i = 0; i < 6; i++)
        {
            ShootBullet(data);
        }
    }
}
