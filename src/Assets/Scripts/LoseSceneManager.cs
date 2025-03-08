using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseSceneManager : MonoBehaviour
{
    public void OnBackButtonClicked()
    {
        SceneManager.LoadScene("LevelSelectScene");
    }

    void OnGUI()
    {
        // 增大字体显示n的值
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 48;

        GUI.Label(new Rect((Screen.width - 900) / 2, (Screen.height - 300) / 2 - 100, 900, 300), "Fall out of the World!",style);
    }
}
