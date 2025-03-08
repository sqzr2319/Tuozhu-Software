using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static GameData gameData;

    private static SaveManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void LoadGame()
    {
        if (PlayerPrefs.HasKey("GameData"))//存在存档，则加载
        {
            string jsonData = PlayerPrefs.GetString("GameData");
            gameData = JsonUtility.FromJson<GameData>(jsonData);
        }
        else//不存在，初始化新存档
        {
            gameData = new GameData();
            SaveGame();
        }
        ApplyAudioSettings();
    }

    public static void SaveGame()
    {
        string jsonData = JsonUtility.ToJson(gameData);
        PlayerPrefs.SetString("GameData", jsonData);
        PlayerPrefs.Save();
    }

    public static void ResetGame()
    {
        gameData = new GameData();
        SaveGame();
    }

    public static GameData GetGameData()
    {
        return gameData;
    }

    public static void UpdateGameData(GameData newData)
    {
        gameData = newData;
        SaveGame();
    }
    private static void ApplyAudioSettings()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(gameData.musicVolume);
            AudioManager.Instance.SetSoundVolume(gameData.soundVolume);
        }
    }

}
