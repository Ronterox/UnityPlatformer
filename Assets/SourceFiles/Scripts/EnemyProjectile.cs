using System.Collections;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [Tooltip("Speed in m/s")]
    public float speed = 20f;
    
    [Tooltip("Auto-destroy after this many seconds if no collision")]
    public float lifetime = 5f;
    
    [Tooltip("Damage dealt to player on hit")]
    public int damage = 1;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("EnemyProjectile requires a Rigidbody component");
            return;
        }

        rb.AddForce(transform.forward * speed, ForceMode.Impulse);

        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.LoseLife();
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}