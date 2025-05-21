using UnityEngine;
using UnityEngine.AI;

public class EnemyFollowPlayer : MonoBehaviour
{
    public float activationDistance = 5f;        // Distance to trigger follow
    public Transform player;                     // Assign your player transform

    private NavMeshAgent agent;
    private Animator animator;
    private bool isActive = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // Get the Animator
    }

    void Update()
    {
        if (!isActive)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= activationDistance)
            {
                ActivateEnemy();
            }
        }

        if (isActive)
        {
            agent.SetDestination(player.position);
            animator.SetBool("isWalking", true); // Start walk animation
        }
    }

    void ActivateEnemy()
    {
        isActive = true;
        // Optional: play sound or animation
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateEnemy();
        }
    }
    public bool IsAwake()
    {
        return isActive;
    }
}


