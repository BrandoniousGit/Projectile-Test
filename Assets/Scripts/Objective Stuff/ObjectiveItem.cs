using System;

public class ObjectiveItem : Interactable
{
    public static event Action<int> OnObjectiveProgress;
    public int progressAmount;

    private bool hasBeenInteractedWith;

    public override string GetDescription()
    {
        if (hasBeenInteractedWith)
        {
            return "You have already interacted with this cube!";
        }

        return $"Press 'E' to advance objective by {progressAmount}";
    }

    public override void OnInteract()
    {
        if (hasBeenInteractedWith)
        {
            return;
        }

        hasBeenInteractedWith = true;
        OnObjectiveProgress?.Invoke(progressAmount);
    }
}