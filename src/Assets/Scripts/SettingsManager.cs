using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider musicVolumeSlider;
    public Slider soundVolumeSlider;
    public Button resetProgressButton;
    public Button backButton;

    // Start is called before the first frame update
    void Start()
    {
        GameData gameData = SaveManager.GetGameData();
        musicVolumeSlider.value = gameData.musicVolume;
        soundVolumeSlider.value = gameData.soundVolume;

        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        soundVolumeSlider.onValueChanged.AddListener(OnSoundVolumeChanged);

        resetProgressButton.onClick.AddListener(OnResetProgressButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
    }
    private void OnMusicVolumeChanged(float volume)
    {
        GameData gameData = SaveManager.GetGameData();
        gameData.musicVolume = volume;
        SaveManager.UpdateGameData(gameData);
        AudioManager.Instance.SetMusicVolume(volume);
    }

    private void OnSoundVolumeChanged(float volume)
    {
        GameData gameData = SaveManager.GetGameData();
        gameData.soundVolume = volume;
        SaveManager.UpdateGameData(gameData);
        AudioManager.Instance.SetSoundVolume(volume);
    }

    private void OnResetProgressButtonClicked()
    {
        GameData gameData = SaveManager.GetGameData();
        //gameData.currentLevel = 1;
        gameData.isGameFinished = false;
        SaveManager.UpdateGameData(gameData);
        Debug.Log("��Ϸ����������");
    }

    private void OnBackButtonClicked() 
    {
        SceneManager.LoadScene("MainMenuScene");
        Debug.Log("����������");
    }
    // Update is called once per frame
    void Update()
    {
        SaveManager.LoadGame();
    }
}
