using UnityEngine;

public class CubeInteractTest : Interactable
{
    public override void OnInteract()
    {
        GetComponent<Rigidbody>().AddForce(0, 1500, 0);
    }
}
