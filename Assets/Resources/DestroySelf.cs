using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    public float timer;

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            Destroy(gameObject);
        }
    }
}
