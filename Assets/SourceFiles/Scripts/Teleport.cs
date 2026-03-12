using UnityEngine.SceneManagement;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public string sceneName;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
