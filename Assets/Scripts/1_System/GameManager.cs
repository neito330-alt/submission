using UnityEngine;


public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public static Rooms.StageManager stageManager;
    public static PlayerManager playerManager;
    public static UIManager.UIManager uiManager;
    public static MenuCameraController UIObject;

    public static int qualityLevel = 0;

    void Awake()
    {
        qualityLevel = QualitySettings.GetQualityLevel();
    }
}
