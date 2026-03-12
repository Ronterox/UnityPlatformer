using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{    
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null) {
                Debug.LogError("GameManager is missing from the scene!");
            }
            return _instance;
        }
    }
    
    public string loseScene;
    
    [Header("Effects")]
    public GameObject particleEffectPrefab;
    
    [Header("Player")]
    public Transform player, checkpoint;
       
    public int maxLives = 3;
    public int Lives
    {
      get { return lives; }
    }
    
    private int lives;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        lives = maxLives;
        Debug.Log("Lives " + lives);
    }
    
    public void LoseLife() 
    {
    	lives--;
    	
    	StartCoroutine(RespawnPlayer(2.0f));
    	
    	Debug.Log("Lives " + lives);
    }
    
    IEnumerator RespawnPlayer(float delay)
    {
    	if (particleEffectPrefab == null || player == null || checkpoint == null) yield break;
        
        Instantiate(particleEffectPrefab, player.position + Vector3.up, Quaternion.identity);
        player.gameObject.SetActive(false);

        yield return new WaitForSeconds(delay);
        
        if (lives <= 0) {
            SceneManager.LoadScene(loseScene);
            yield break;
        }
        
        player.gameObject.SetActive(true);

        var controller = player.GetComponent<StarterAssets.ThirdPersonController>();
        if (controller != null)
        {

            controller.TeleportAndReset(transform.position, transform.eulerAngles.y);
        }
    }
}
