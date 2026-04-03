using UnityEngine;
using static Globals;

public class BouncyShots : BasicBulletModifier
{
    protected override void UpdateBulletBase(BulletBase bulletBase)
    {
        bulletBase.ModifyBounces((int)Data.ValuePerLevel[ModifierLevel - 1]);
    }
}
