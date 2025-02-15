using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    public void OnStartButtonClicked()
    {
        GameData gameData = SaveManager.GetGameData();

        //判断是否第一次进入游戏
        /*if (gameData.isFirstTimePlaying)
        {
            SceneManager.LoadScene("MainMenuScene");
            gameData.isFirstTimePlaying = false;
            SaveManager.UpdateGameData(gameData);
        }
        else
        {
            SceneManager.LoadScene("MainMenuScene");
        }*/
        if (gameData.isFirstTimePlaying)
        {
            SceneManager.LoadScene("TutorialScene");
            gameData.isFirstTimePlaying = false;
            SaveManager.UpdateGameData(gameData);
        }
        else
        {
            SceneManager.LoadScene("MainMenuScene");
        }

    }
    private void Awake()
    {
        SaveManager.LoadGame();
    }
}
