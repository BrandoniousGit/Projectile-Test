using System.Net.Sockets;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using static Globals;

public class CameraMove : MonoBehaviour
{
    public float xSens;
    public float ySens;
    public float zoomSpeed;
    public LayerMask interactMask;
    public float interactDistance;
    public TextMeshProUGUI interactionText;

    public float fov;

    private float x, y;
    private bool isLocked;
    private float zoom = 30;
    private float zoomMin = 20;

    private void Start()
    {
        FreeMe();
    }

    private void Update()
    {
        if (isLocked)
        {
            UserInput();
            Zoom();
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

        if (Physics.Raycast(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + 0.4f, Camera.main.transform.position.z), Camera.main.transform.forward, out var hit, interactDistance, interactMask))
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

    private void Zoom()
    {
        if (Input.mouseScrollDelta.y > 0) zoom -= zoomSpeed;
        if (Input.mouseScrollDelta.y < 0) zoom += zoomSpeed;

        zoom = Mathf.Clamp(zoom, zoomMin, fov);

        if (Input.GetMouseButton(1))
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, zoom, 0.1f);
        }
        else if (!Mathf.Approximately(Camera.main.fieldOfView, fov))
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, fov, 0.1f);
        }
        else
            Camera.main.fieldOfView = fov;

        if (Input.GetMouseButtonUp(1))
        {
            zoom = 30;
        }
    }

    private void UserInput()
    {
        x += Input.GetAxis("Mouse X") * xSens;
        y -= Input.GetAxis("Mouse Y") * ySens;

        y = Mathf.Clamp(y, -80.0f, 80.0f);

        transform.rotation = Quaternion.Euler(0, x, 0);

        Camera.main.transform.localRotation = Quaternion.Euler(y, 0, 0);
    }

    public void FreeMe()
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