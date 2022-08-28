using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        public void playGame()
        {
            SceneManager.LoadScene("Scenes/GameScene");
        }
        
        public void quitGame()
        {
            Debug.Log("Quiting the App!!!");
            Application.Quit();
        }
    }
}
