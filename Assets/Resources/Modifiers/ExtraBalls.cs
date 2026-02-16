using UnityEngine;
using static Globals;

public class ExtraBalls : ModifierBase
{
    protected override void OnBulletFiredAction()
    {
        if (TryProcModifier())
        {
            var data = new BulletData()
            {
                Spread = 15,
                CustomGravity = -10,
                BulletSpeed = 30,
                Damage = GunBrain.CalculatedDamage * 0.2f
            };

            for (int i = 0; i < EffectAmount; i++)
            {
                GunBrain.ShootBullet(data);
            }
        }
    }
}
