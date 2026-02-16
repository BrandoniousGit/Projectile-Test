using static Globals;

public class TripleShot : ModifierBase
{
    protected override void OnBulletFiredAction()
    {
        if (TryProcModifier())
        {
            ShootBullet(new BulletData()
            {
                AngularOffsetX = 10,
                AngularOffsetY = 0,
                BulletSpeed = 35,
                Damage = GunBrain.CalculatedDamage * 0.50f
            });
        
            ShootBullet(new BulletData()
            {
                AngularOffsetX = -10,
                AngularOffsetY = 0,
                BulletSpeed = 35,
                Damage = GunBrain.CalculatedDamage * 0.50f
            });
        }
    }
}
