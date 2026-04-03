using UnityEngine;

[CreateAssetMenu(fileName = "New Modifier", menuName = "New Modifier/Chain Shot")]
public class ChainShotData : ModifierData
{
    public float EffectAmountCap;
    public float[] EffectAmountPerLevel;
}