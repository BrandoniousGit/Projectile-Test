using UnityEngine;
using static Globals;

public class GravityShots : BasicBulletModifier
{
    protected override void OnEvaluateModifiers(WeaponStats data)
    {
        base.OnEvaluateModifiers(data);

        data.UsesGravity = true;
    }
}
