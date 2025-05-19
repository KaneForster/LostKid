using UnityEngine;

public class RotateY : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(0, 90 * Time.deltaTime, 0);
        Debug.Log("Rotating...");
    }
}