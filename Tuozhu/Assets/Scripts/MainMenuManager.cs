using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void OnLevelSelectButtonClicked()
    {
        SceneManager.LoadScene("LevelSelectScene");
    }

    public void OnSettingsButtonClicked()
    {
        SceneManager.LoadScene("SettingsScene");
    }

    public void OnTutorialButtonClicked()
    {
      SceneManager.LoadScene("TutorialScene");
    }

    public void OnAboutButtonClicked()
    {
        SceneManager.LoadScene("AboutScene");
    }
}