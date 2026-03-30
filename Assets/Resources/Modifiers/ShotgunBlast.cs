using UnityEngine;
using static Globals;

public class ShotgunBlast : BasicBulletModifier
{
    protected override void OnBulletFired(WeaponStats data)
    {
        if (TryProcModifier(Data.ProcChance))
        {
            var newData = new BulletData()
            {
                Spread = 40,
                BulletSpeed = data.BulletSpeed.Value,
                Damage = data.Damage.Value * 0.3f
            };

            for (int i = 0; i < 6; i++)
            {
                ShootBullet(newData);
            }
        }
    }
}
