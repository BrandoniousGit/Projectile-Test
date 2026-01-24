using System.Net.Sockets;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float xSens;
    public float ySens;

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
    }

    private void UserInput()
    {
        x += Input.GetAxis("Mouse X") * xSens;
        y -= Input.GetAxis("Mouse Y") * ySens;

        y = Mathf.Clamp(y, -80.0f, 80.0f);

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