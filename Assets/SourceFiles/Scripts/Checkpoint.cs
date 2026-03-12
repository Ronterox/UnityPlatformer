using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object has the "Player" tag
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.checkpoint = transform;
            gameObject.SetActive(false); 
        }
    }
}
