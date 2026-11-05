using Enums;

public class HomingShots : BasicBulletModifier
{
    protected override void UpdateBulletBase(BulletBase bulletBase)
    {
        if (TryProcModifier(Data.ValuePerLevel[ModifierLevel - 1]))
        {
            bulletBase.ModifyHoming(30);
        }
    }
}
