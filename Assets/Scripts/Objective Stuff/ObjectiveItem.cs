using System;

public class ObjectiveItem : Interactable
{
    public static event Action<int> OnObjectiveProgress;
    public int progressAmount;

    public string InteractString;
    public string InteractedWithString;

    public override string GetDescription()
    {
        if (isOn)
        {
            return InteractedWithString;
        }

        return $"{InteractString}";
    }

    public override void OnInteract()
    {
        if (isOn)
        {
            return;
        }

        isOn = true;
        OnObjectiveProgress?.Invoke(progressAmount);
    }
}