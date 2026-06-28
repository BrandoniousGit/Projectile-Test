using UnityEngine;

public abstract class ModifierBase : MonoBehaviour
{
    public EnhanceableBase Enhanceable;
    public int ModifierLevel = 1;
    public string DisplayName;

    public abstract ModifierData GetData();

    #region virtual methods
    protected virtual void OnActionPerformed() { return; }
    protected virtual void OnActiveAbilityPressed() { return; }
    protected virtual void OnSetData() { return; }
    public abstract string GetDescription();
    #endregion

    protected virtual void OnEnable()
    {
        EnhanceableBase.OnActiveAbilityButtonPressed += OnActiveAbilityPressed;
    }

    protected virtual void OnDisable()
    {
        EnhanceableBase.OnActiveAbilityButtonPressed -= OnActiveAbilityPressed;
    }

    public virtual void SetData(ModifierData data) { }

    public bool TryProcModifier(float procChance)
    {
        return procChance > 100 - Random.Range(1, 101);
    }


    public void UpdateDisplayName()
    {
        DisplayName = $"{GetData().ModifierName} {ModifierLevel.IntToRomalNumeral()}";
    }
}

public abstract class ModifierBase<T> : ModifierBase where T : ModifierData
{
    public T Data;
    public override ModifierData GetData() => this.Data;

    public override void SetData(ModifierData data)
    {
        if (data is T tData)
        {
            Data = tData;

            OnSetData();

            DisplayName = $"{Data.ModifierName} {ModifierLevel.IntToRomalNumeral()}";
        }
    }
}
