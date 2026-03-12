using UnityEngine;

public class Hearts : MonoBehaviour
{
    public GameObject heartPrefab;
    
    [Header("Animation Settings")]
    public float pulseSpeed = 5f;    // How fast it pulses
    public float pulseAmount = 0.2f; // How much it grows (0.2 = 20% larger)

    private GameObject[] hearts;
    private int activeCount;
    
    void Start() {
        int totalLives = GameManager.Instance.Lives;
        hearts = new GameObject[totalLives];
        
        for (int i = 0; i < totalLives; i++) {
            hearts[i] = Instantiate(heartPrefab, transform); 
        }
        
        heartPrefab.SetActive(false);
        activeCount = totalLives;
    }

    void Update()
    {
        int currentLives = GameManager.Instance.Lives;

        // --- Handle Life Changes ---
        while (activeCount > currentLives && activeCount > 0) {
            activeCount--;
            // Reset scale before hiding so it's normal if it turns back on
            hearts[activeCount].transform.localScale = Vector3.one;
            hearts[activeCount].SetActive(false);
        }
        
        while (activeCount < currentLives && activeCount < hearts.Length) {
            hearts[activeCount].SetActive(true);
            activeCount++;
        }

        // --- Handle Pulse Animation ---
        AnimateLastHeart();
    }

    void AnimateLastHeart()
    {
        if (activeCount <= 0) return;

        // The "Last Active" heart is at index [activeCount - 1]
        int lastIndex = activeCount - 1;

        // Calculate scale using a Sine wave: 
        // 1.0 is base scale, we add a value that oscillates between 0 and pulseAmount
        float scaleOffset = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f * pulseAmount;
        float finalScale = 1f + scaleOffset;

        hearts[lastIndex].transform.localScale = new Vector3(finalScale, finalScale, 1f);

        // Optional: Ensure all other hearts stay at their default scale
        if (activeCount > 1) {
             hearts[activeCount - 2].transform.localScale = Vector3.one;
        }
    }
}
