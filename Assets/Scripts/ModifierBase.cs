using System.Linq;
using UnityEngine;
using static Globals;

public abstract class ModifierBase : MonoBehaviour
{
    public ModifierData Data;
    public GunBrain GunBrain;
    public int ModifierLevel = 1;

    public float ProcChance;
    public float Cooldown;
    public float EffectAmount;

    public string DisplayName;

    #region abstract methods
    protected abstract void OnBulletFiredAction();
    #endregion

    public void ShootBullet(BulletData bData)
    {
        GunBrain.ShootBullet(bData);
    }

    private void OnEnable()
    {
        GunBrain.OnBulletFired += OnBulletFiredAction;
        GunBrain.OnModifierChanged += UpdateData;

        if (TryGetComponent<GunBrain>(out var brain))
        {
            GunBrain = brain;
            return;
        }

        Debug.Log("Could not find gun brain");
    }

    private void OnDisable()
    {
        GunBrain.OnBulletFired -= OnBulletFiredAction;
        GunBrain.OnModifierChanged -= UpdateData;
    }

    public void UpdateData()
    {
        if (Data != null)
        {
            if (Data.ProcChancePerLevel.Count >= ModifierLevel)
            {
                ProcChance = Data.ProcChancePerLevel[ModifierLevel - 1];
            }
 
            if (Data.CooldownPerLevel.Count >= ModifierLevel)
            {
                Cooldown = Data.CooldownPerLevel[ModifierLevel - 1];
            }

            if (Data.EffectAmountPerLevel.Count >= ModifierLevel)
            {
                EffectAmount = Data.EffectAmountPerLevel[ModifierLevel - 1];
            }

            DisplayName = $"{Data.ModifierName} {(ModifierLevel).IntToRomalNumeral()}";
        }
    }

    public bool TryProcModifier()
    {
        return ProcChance > 100 - Random.Range(1, 101);
    }
}
