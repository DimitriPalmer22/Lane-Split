using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// This script is used to control the menu buttons
public class MenuController : MonoBehaviour
{
    // Loads Play scene
   public void LoadScene(string sceneName)
   {
       SceneManager.LoadScene(sceneName);
   }
}
