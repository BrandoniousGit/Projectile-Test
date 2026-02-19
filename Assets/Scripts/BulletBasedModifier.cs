using UnityEngine;
using static Globals;

public class BulletBasedModifier : ModifierBase
{
    private GameObject BulletGO;

    protected virtual void OnBulletFiredAction(BulletData data) { return; }
    protected virtual void EvaluateShotModifiers(BulletBase data) { return; }

    protected override void OnEnable()
    {
        base.OnEnable();

        BulletGO = (GameObject)Resources.Load("Bullet");

        BulletBase.EvaluateShotModifiers += EvaluateShotModifiers;
        GunBase.OnBulletFiredEvent += OnBulletFiredAction;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        BulletBase.EvaluateShotModifiers -= EvaluateShotModifiers;
        GunBase.OnBulletFiredEvent -= OnBulletFiredAction;
    }

    public void ShootBullet(BulletData data)
    {
        if (BulletGO == null)
        {
            return;
        }

        //Setting up spread if using
        Vector3 forward = transform.forward;
        Vector2 unitCircle = Random.insideUnitCircle;
        unitCircle *= data.Spread;

        forward = Quaternion.AngleAxis(data.AngularOffsetX + unitCircle.x, transform.up) * forward;
        forward = Quaternion.AngleAxis(data.AngularOffsetY + unitCircle.y, transform.right) * forward;

        var pos = data.Position.HasValue ? data.Position.Value : transform.position;
        var rot = data.Normal.HasValue ? Quaternion.Euler(data.Normal.Value) : transform.rotation;

        var bulletObj = Instantiate(BulletGO, pos, rot, null);
        var bulletRb = bulletObj.GetComponent<Rigidbody>();
        var bulletBase = bulletObj.GetComponent<BulletBase>();
        bulletBase.SetData(data);

        bulletRb.linearVelocity = data.BulletSpeed * forward;
    }
}
