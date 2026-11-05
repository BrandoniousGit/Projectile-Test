using UnityEngine;
using Enums;
using static Globals;

public class BulletBasedModifier<T> : ModifierBase<T> where T : ModifierData
{
    private GameObject BulletGO;
    public GunBase BaseWeapon => (GunBase)Enhanceable;

    protected virtual void OnBulletFired(WeaponStats data) { return; }
    protected virtual void OnEvaluateModifiers(WeaponStats data) { return; }
    protected virtual void UpdateBulletBase(BulletBase bulletBase) { return; }
    protected virtual void OnBulletExpire(bool hasHit) { return; }

    public override string GetDescription() { return ""; }

    protected override void OnEnable()
    {
        base.OnEnable();

        BulletGO = (GameObject)Resources.Load("Bullet");

        BulletBase.UpdateBulletBase += UpdateBulletBase;
        BulletBase.DidShotHitSomething += OnBulletExpire;
        GunBase.DidShotHitSomething += OnBulletExpire;
        GunBase.OnBulletFired += OnBulletFired;
        GunBase.OnEvaluateModifiers += OnEvaluateModifiers;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        BulletBase.UpdateBulletBase -= UpdateBulletBase;
        BulletBase.DidShotHitSomething -= OnBulletExpire;
        GunBase.DidShotHitSomething -= OnBulletExpire;
        GunBase.OnBulletFired -= OnBulletFired;
        GunBase.OnEvaluateModifiers -= OnEvaluateModifiers;
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

        var bulletObj = Instantiate(BulletGO, pos, Quaternion.LookRotation(forward), null);
        var bulletBase = bulletObj.GetComponent<BulletBase>();
        bulletBase.Init(data);
    }
}
