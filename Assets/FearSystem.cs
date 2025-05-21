using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FearSystem : MonoBehaviour
{
    public Slider fearSlider;
    public Transform player;
    public float fearRadius = 10f; // Max distance at which enemies affect fear
    public float fearIncreaseRate = 10f; // How quickly fear rises
    public float fearDecreaseRate = 5f;  // How quickly fear falls when safe

    private List<EnemyFollowPlayer> enemies;
    private float fearValue = 0f;
    private float maxFear = 100f;

    void Start()
    {
        enemies = new List<EnemyFollowPlayer>(
            FindObjectsOfType<EnemyFollowPlayer>()
        );
    }

    void Update()
    {
        float totalFear = 0f;

        foreach (var enemy in enemies)
        {
            if (enemy.IsAwake()) // We'll add this method shortly
            {
                float distance = Vector3.Distance(player.position, enemy.transform.position);
                if (distance < fearRadius)
                {
                    float fearContribution = (1 - (distance / fearRadius)) * fearIncreaseRate;
                    totalFear += fearContribution;
                }
            }
        }

        // Increase or decrease fear value
        if (totalFear > 0)
            fearValue += totalFear * Time.deltaTime;
        else
            fearValue -= fearDecreaseRate * Time.deltaTime;

        // Clamp and assign to slider
        fearValue = Mathf.Clamp(fearValue, 0f, maxFear);
        fearSlider.value = fearValue / maxFear;
    }
}