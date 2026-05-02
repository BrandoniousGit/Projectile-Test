using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public Vector3 launchDirection;
    public float power;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var rb = other.GetComponent<Rigidbody>();
            rb.linearVelocity = rb.linearVelocity.ResetVerticalComponent();
            rb.AddForce(launchDirection * power, ForceMode.Impulse);
        }
    }
}
