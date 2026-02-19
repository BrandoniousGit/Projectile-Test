using UnityEngine;
using static Globals;

public class ExtraBalls : BulletBasedModifier
{
    protected override void OnBulletFiredAction(BulletData data)
    {
        if (TryProcModifier())
        {
            var newData = new BulletData()
            {
                Spread = 15,
                CustomGravity = -10,
                BulletSpeed = 30,
                Damage = data.Damage * 0.2f
            };

            for (int i = 0; i < EffectAmount; i++)
            {
                ShootBullet(data);
            }
        }
    }
}
