using System.Collections;
using UnityEngine;
using Enums;
using static Globals;

public class ExtraShot : BulletBasedModifier<ExtraShotData>
{
    public int shotsFired = 0;

    protected override void OnBulletFired(WeaponStats data)
    {
        shotsFired += 1;

        if (shotsFired >= Data.ShotsUntilExtraPerLevel[ModifierLevel - 1])
        {
            var quarterShotDelay = (1 / BaseWeapon.BaseWeaponData.m_fireRate) / 4;
            StartCoroutine("WaitForQuarterShotDelay", quarterShotDelay);

            shotsFired = -1;
        }
    }

    public IEnumerator WaitForQuarterShotDelay(float shotDelay)
    {
        yield return new WaitForSeconds(shotDelay);
        BaseWeapon.TryShoot(false);
    }
}
