using UnityEngine;

public class FearMeter : MonoBehaviour
{
    public static float fearValue = 0f;

    void Update()
    {
        // Optional: decay fear over time
        fearValue = Mathf.Max(0f, fearValue - 1f * Time.deltaTime);
    }
}
