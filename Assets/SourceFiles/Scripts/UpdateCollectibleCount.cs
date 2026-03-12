using UnityEngine;
using TMPro;
using System;

public class UpdateCollectibleCount : MonoBehaviour
{
    private TextMeshProUGUI collectibleText;
    private GameObject parkourArea, chickenDinner;
    
    [Header("Audio Settings")]
    public AudioSource audioSource; // Drag your AudioSource here in the Inspector
    public AudioClip winSound;      // Drag cheering.mp3 here
    private bool hasPlayedWinSound = false; // Prevents the sound from looping/restarting
    private int maxCollectibles = 0;

    void Start()
    {
        collectibleText = GetComponent<TextMeshProUGUI>();
        if (collectibleText == null)
        {
            Debug.LogError("UpdateCollectibleCount script requires a TextMeshProUGUI component.");
            return;
        }

        parkourArea = GameObject.FindGameObjectWithTag("ParkourArea");
        chickenDinner = GameObject.FindGameObjectWithTag("ChickenDinner");
        
        if (chickenDinner != null) chickenDinner.SetActive(false);
        
        // Try to get AudioSource if not assigned
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

	maxCollectibles = GetTotalCollectibles();
        UpdateCollectibleDisplay();
    }

    void Update()
    {
        UpdateCollectibleDisplay();
    }
    
    int GetTotalCollectibles() {
    	int totalCollectibles = 0;

        // Count Pickup objects
        Type collectibleType = Type.GetType("Pickup");
        if (collectibleType != null)
        {
            totalCollectibles += UnityEngine.Object.FindObjectsByType(collectibleType, FindObjectsSortMode.None).Length;
        }
        
        return totalCollectibles;
    }

    private void UpdateCollectibleDisplay()
    {
        int totalCollectibles = GetTotalCollectibles();

        // Logic for Winning
        if (totalCollectibles <= 0)
        {
            collectibleText.text = "WIIINNNERRR!!!";
            
            if (parkourArea != null) parkourArea.SetActive(false);
            if (chickenDinner != null) chickenDinner.SetActive(true);

            // Play the cheering sound once
            if (!hasPlayedWinSound && audioSource != null && winSound != null)
            {
                audioSource.PlayOneShot(winSound);
                hasPlayedWinSound = true; 
            }
        }
        else
        {
            collectibleText.text = $"{maxCollectibles - totalCollectibles}/{maxCollectibles}";
        }
    }
}
