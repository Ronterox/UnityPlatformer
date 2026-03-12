using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CartoonUI
{
    public class MainMenu : MonoBehaviour
    {
        public string sceneName; 
        
        public void Play()
        {
            SceneManager.LoadScene(sceneName);
        }
        
        public void Close() 
        {
            Application.Quit();
            Debug.Log("Game is exiting..."); 
        }
    }
}
