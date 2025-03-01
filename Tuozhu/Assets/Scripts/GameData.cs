[System.Serializable]
public class GameData
{
    public bool isFirstTimePlaying = true;//是否初次进入游戏
    public int currentLevel = 3;//当前关卡进度
    public float musicVolume = 1.0f;//背景音乐音量
    public float soundVolume = 1.0f;//音效音量
    public bool isGameFinished = false;//是否完成游戏
}
