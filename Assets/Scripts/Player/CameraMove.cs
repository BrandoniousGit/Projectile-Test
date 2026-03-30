using System.Net.Sockets;
using TMPro;
using UnityEngine;
using static Globals;

public class CameraMove : MonoBehaviour
{
    public float xSens;
    public float ySens;
    public GameObject playerModel;
    public LayerMask interactMask;
    public float interactDistance;
    public TextMeshProUGUI interactionText;

    private float x, y;
    private bool isLocked;

    private void Start()
    {
        FreeMe();
    }

    private void Update()
    {
        if (isLocked)
        {
            UserInput();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            FreeMe();
        }

        UserInteract();
    }

    void UserInteract()
    {
        if (!isLocked)
        {
            return;
        }

        var interactionTextString = string.Empty;

        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z), transform.forward, Color.red, Time.deltaTime);

        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z), transform.forward, out var hit, interactDistance, interactMask))
        {
            //Checks to see if it is an interactable
            if (hit.transform.TryGetComponent<Interactable>(out var component))
            {
                Interactable iOScript = component;

                HandleInteraction(iOScript);
                interactionTextString = iOScript.GetDescription();
            }
        }

        if (interactionText != null)
        {
            interactionText.text = interactionTextString;
        }
    }

    void HandleInteraction(Interactable interactable)
    {
        //Need to replace with global interact key
        KeyCode interactKeyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), "E");

        switch (interactable.interactionType)
        {
            case Interactable.InteractionType.Press:
                if (Input.GetKeyDown(interactKeyCode))
                {
                    interactable.OnInteract();
                }
                break;

            default:
                throw new System.Exception("Unimplemented type of interactable.");
        }
    }

    private void UserInput()
    {
        x += Input.GetAxis("Mouse X") * xSens;
        y -= Input.GetAxis("Mouse Y") * ySens;

        y = Mathf.Clamp(y, -75.0f, 75.0f);

        playerModel.transform.localRotation = Quaternion.Euler(0, x, 0);
        transform.localRotation = Quaternion.Euler(y, x, 0);
    }

    private void FreeMe()
    {
        isLocked = !isLocked;

        if (isLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}