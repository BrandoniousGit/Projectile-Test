using static Globals;

public class TripleShot : ModifierBase
{
    public override void OnBulletFiredAction()
    {
        ShootBullet(new BulletData()
        {
            SpreadX = 10,
            SpreadY = 0,
            BulletSpeed = 35
        });
        
        ShootBullet(new BulletData()
        {
            SpreadX = -10,
            SpreadY = 0,
            BulletSpeed = 35
        });
    }
}
