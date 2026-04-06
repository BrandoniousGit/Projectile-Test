using System.Collections;
using UnityEngine;
using static Globals;

public class ExtraShot : BulletBasedModifier<ExtraShotData>
{
    public int shotsFired = 0;

    protected override void OnBulletFired(WeaponStats data)
    {
        shotsFired += 1;

        if (shotsFired >= Data.ShotsUntilExtraPerLevel[ModifierLevel - 1])
        {
            var halfShotDelay = (1 / BaseWeapon.BaseWeaponData.m_fireRate) / 4;
            StartCoroutine("WaitForHalfShotDelay", halfShotDelay);

            shotsFired = -1;
        }
    }

    public IEnumerator WaitForHalfShotDelay(float shotDelay)
    {
        yield return new WaitForSeconds(shotDelay);
        BaseWeapon.TryShoot(false);
    }
}
