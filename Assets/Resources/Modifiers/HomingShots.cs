using static Globals;

public class HomingShots : BasicBulletModifier
{
    protected override void UpdateBulletBase(BulletBase bulletBase)
    {
        if (TryProcModifier(Data.ProcChance))
        {
            bulletBase.ModifyHoming(Data.ValuePerLevel[ModifierLevel - 1]);
        }
    }
}
