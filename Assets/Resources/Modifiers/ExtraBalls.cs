using UnityEngine;
using static Globals;

public class ExtraBalls : BasicBulletModifier
{
    protected override void OnBulletFired(WeaponStats data)
    {
        if (TryProcModifier(Data.ProcChance))
        {
            var newData = new BulletData()
            {
                Spread = 40,
                BulletSpeed = 30,
                Damage = data.Damage.Value * 0.3f
            };

            for (int i = 0; i < Data.ValuePerLevel[ModifierLevel - 1]; i++)
            {
                ShootBullet(newData);
            }
        }
    }

    protected override void UpdateBulletBase(BulletBase bulletBase)
    {
        bulletBase.ModifyGravity(10);
    }
}
