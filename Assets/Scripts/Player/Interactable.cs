using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    //Different types of interaction
    public enum InteractionType
    {
        Press,
        Hold,
        Spam
    }

    public InteractionType interactionType;

    [Header("Hold Type")]
    public float holdTime;
    public float maxHoldTime = 5;
    public float progressPerHold = 1;

    [Header("Other")]
    //Selected and On for various referencing
    public bool isOn;
    public bool lockPlayerWhileInteract;

    public abstract void OnInteract();
    public virtual string GetDescription() { return "I am unset"; }

    public void ResetHoldTime() { holdTime = 0.0f; }
}