using UnityEngine;
using UnityEngine.AI;

public class EnemyStateMachine : MonoBehaviour
{
    private enum State
    {
        Idle,
        Chasing,
        Shooting
    }

    public float chaseRange = 15f;
    public float attackRange = 8f;
    public float shootCooldown = 2f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public NavMeshAgent agent;

    private State _currentState = State.Idle;
    private float _shootTimer;

    private Transform Player => GameManager.Instance.player;

    private void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        _shootTimer = 0f;
    }

    private void Update()
    {
        if (Player == null || agent == null) return;

        float distance = Vector3.Distance(transform.position, Player.position);

        switch (_currentState)
        {
            case State.Idle:
                Idle_Update(distance);
                break;
            case State.Chasing:
                Chasing_Update(distance);
                break;
            case State.Shooting:
                Shooting_Update(distance);
                break;
        }
    }

    private void Idle_Update(float distance)
    {
        if (distance < chaseRange)
        {
            _currentState = State.Chasing;
            agent.isStopped = false;
        }
    }

    private void Chasing_Update(float distance)
    {
        agent.SetDestination(Player.position);

        if (distance < attackRange)
        {
            _currentState = State.Shooting;
            agent.isStopped = true;
            _shootTimer = 0f;
        }
        else if (distance > chaseRange)
        {
            _currentState = State.Idle;
            agent.isStopped = true;
        }
    }

    private void Shooting_Update(float distance)
    {
        _shootTimer -= Time.deltaTime;
        if (_shootTimer <= 0f)
        {
            FireProjectile();
            _shootTimer = shootCooldown;
        }

        if (distance > attackRange)
        {
            _currentState = State.Chasing;
            agent.isStopped = false;
        }
    }

    private void FireProjectile()
    {
        if (bulletPrefab == null || firePoint == null) return;

        Vector3 direction = (Player.position - firePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * 20f;
        }
    }
}