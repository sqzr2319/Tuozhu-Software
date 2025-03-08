using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    public Button backButton;
    public ScrollRect scrollRect;
    public Button[] levelButtons;
    public GameObject lockedPrompt;
    public float scrollSpeed = 10f;

    public int currentLevel;

    private void Start()
    {
        GameData gameData = SaveManager.GetGameData();
        //currentLevel = gameData.currentLevel;

        backButton.onClick.AddListener(OnBackButtonClicked);

        //初始化关卡按钮状态
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;
            Button button = levelButtons[i];

            button.onClick.AddListener(() => OnLevelButtonClicked(levelIndex));

            /*//未解锁关卡：禁用按钮交互
            if (levelIndex > currentLevel)
            {
                button.interactable = false;
            }*/
        }

        //初始化滚动视图位置
        ScrollToCurrentLevel();
    }

    private void Update()
    {
        //监听键盘左右键滚动
        float horizontal = Input.GetAxis("Horizontal");
        if (horizontal != 0)
        {
            ScrollContent(horizontal * scrollSpeed * Time.deltaTime);
        }

        //监听鼠标滚轮滚动
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            ScrollContent(-scroll * scrollSpeed * Time.deltaTime * 10);//调整滚动灵敏度
        }
    }

    
    private void OnBackButtonClicked()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
    private void OnLevelButtonClicked(int levelIndex)
    {
        if (levelIndex <= currentLevel)
        {
            SceneManager.LoadScene("Level" + levelIndex + "Scene");
        }
        else
        {
            StartCoroutine(ShowLockedPrompt());
        }
    }

    private IEnumerator ShowLockedPrompt()
    {
        lockedPrompt.SetActive(true);
        yield return new WaitForSeconds(1f);
        lockedPrompt.SetActive(false);
    }

    //滚动到当前关卡
    private void ScrollToCurrentLevel()
    {
        if (currentLevel > 0 && currentLevel <= levelButtons.Length)
        {
            float normalizedPosition = (float)(currentLevel - 1) / (levelButtons.Length - 1);
            scrollRect.horizontalNormalizedPosition = normalizedPosition;
        }
    }

    //滚动内容
    private void ScrollContent(float amount)
    {
        scrollRect.horizontalNormalizedPosition += amount;
    }
}