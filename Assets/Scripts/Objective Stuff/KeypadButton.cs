using System;
using UnityEngine;

public class KeypadButton : Interactable
{
    public Keypad keypad;

    private void OnEnable()
    {
        keypad = transform.root.GetComponent<Keypad>();
    }

    public override string GetDescription()
    {
        return gameObject.name;
    }

    public override void OnInteract()
    {
        if (keypad != null)
        {
            keypad.ReceiveKeyPress(gameObject.name);
        }
        else
        {
            keypad = transform.root.GetComponent<Keypad>();
        }
    }
}
