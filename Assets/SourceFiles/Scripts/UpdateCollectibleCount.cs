using UnityEngine;
using TMPro;
using System;

public class UpdateCollectibleCount : MonoBehaviour
{
    private TextMeshProUGUI collectibleText;
    private GameObject parkourArea;
    
    [Header("Audio Settings")]
    public AudioSource audioSource; // Drag your AudioSource here in the Inspector
    public AudioClip winSound;      // Drag cheering.mp3 here
    private bool hasPlayedWinSound = false; // Prevents the sound from looping/restarting

    void Start()
    {
        collectibleText = GetComponent<TextMeshProUGUI>();
        if (collectibleText == null)
        {
            Debug.LogError("UpdateCollectibleCount script requires a TextMeshProUGUI component.");
            return;
        }

        parkourArea = GameObject.FindGameObjectWithTag("ParkourArea");
        
        // Try to get AudioSource if not assigned
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        UpdateCollectibleDisplay();
    }

    void Update()
    {
        UpdateCollectibleDisplay();
    }

    private void UpdateCollectibleDisplay()
    {
        int totalCollectibles = 0;

        // Count Pickup objects
        Type collectibleType = Type.GetType("Pickup");
        if (collectibleType != null)
        {
            totalCollectibles += UnityEngine.Object.FindObjectsByType(collectibleType, FindObjectsSortMode.None).Length;
        }

        // Count Collectible2D objects
        Type collectible2DType = Type.GetType("Collectible2D");
        if (collectible2DType != null)
        {
            totalCollectibles += UnityEngine.Object.FindObjectsByType(collectible2DType, FindObjectsSortMode.None).Length;
        }

        // Logic for Winning
        if (totalCollectibles <= 0)
        {
            collectibleText.text = "WIIINNNERRR!!!";
            
            if (parkourArea != null) parkourArea.SetActive(false);

            // Play the cheering sound once
            if (!hasPlayedWinSound && audioSource != null && winSound != null)
            {
                audioSource.PlayOneShot(winSound);
                hasPlayedWinSound = true; 
            }
        }
        else
        {
            collectibleText.text = $"Collectibles remaining: {totalCollectibles}";
        }
    }
}
