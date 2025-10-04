using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCameraController : MonoBehaviour
{
    public enum MenuState
    {
        MainMenu,
        StageMenu,
        GameSetting,
    }

    private Transform _target;
    public Transform target
    {
        get => _target;
        set
        {
            if (_target != null) _target.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
            _target = value;
            if (_target != null) _target.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.yellow;
        }
    }

    public Vector2Int _last;

    public GameObject mainMenu;
    public GameObject stageMenu;
    public GameObject backGround;

    public int targetIndex;
    public Vector2Int stageIndex;

    public MenuState currentMenuState;

    public GameObject[] mainMenuObjects;

    [System.Serializable]
    public class StageData
    {
        public GameObject button;
        public string scene;
    }

    [System.Serializable]
    public class StageList
    {
        public List<StageData> _list;
        public StageData this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public int Count => _list.Count;
    }

    public List<StageList> stageMenuObjects;


    public MainView mainView;

    public Camera menuCamera;

    void Awake()
    {
        GameManager.UIObject = this;
        this.enabled = false;
    }


    void OnEnable()
    {
        menuCamera.gameObject.SetActive(true);
        backGround.SetActive(true);


        mainMenu.SetActive(true);
        stageMenu.SetActive(false);

        target = mainMenuObjects[0].transform;
        targetIndex = 0;
        currentMenuState = MenuState.MainMenu;
    }



    void Update()
    {
        Vector2Int delta = Vector2Int.zero;
        if (0.1f < Input.GetAxis("Horizontal"))
        {
            delta.x = 1;
        }
        if (-0.1f > Input.GetAxis("Horizontal"))
        {
            delta.x = -1;
        }

        if (0.1f < Input.GetAxis("Vertical"))
        {
            delta.y = -1;
        }
        if (-0.1f > Input.GetAxis("Vertical"))
        {
            delta.y = +1;
        }

        if (currentMenuState == MenuState.MainMenu)
        {
            if (delta != Vector2Int.zero && delta != _last)
            {
                targetIndex = Mathf.Clamp(targetIndex + delta.y, 0, 3);
                target = mainMenuObjects[targetIndex].transform;
            }
            if (Input.GetButtonDown("Action"))
            {
                switch (targetIndex)
                {
                    case 0:
                        GameManager.playerManager.Mode = PlayerMode.Field;
                        break;
                    case 1:
                        GameManager.stageManager.RespawnPlayer();
                        GameManager.playerManager.Mode = PlayerMode.Field;
                        break;
                    case 2:
                        currentMenuState = MenuState.StageMenu;
                        mainMenu.SetActive(false);
                        stageMenu.SetActive(true);
                        target = stageMenuObjects[stageIndex.y][stageIndex.x].button.transform;
                        break;
                    case 3:
                        currentMenuState = MenuState.GameSetting;
                        mainMenu.SetActive(false);
                        mainView.enabled = true;
                        break;
                }
            }
            else if (Input.GetButtonDown("Cancel"))
            {
                GameManager.playerManager.Mode = PlayerMode.Field;
            }
        }
        else if (currentMenuState == MenuState.StageMenu)
        {
            if (delta != Vector2Int.zero  && delta != _last)
            {
                stageIndex += delta;
                stageIndex.y = Mathf.Clamp(stageIndex.y, 0, stageMenuObjects.Count - 1);
                stageIndex.x = Mathf.Clamp(stageIndex.x, 0, stageMenuObjects[stageIndex.y].Count - 1);
                target = stageMenuObjects[stageIndex.y][stageIndex.x].button.transform;
            }
            if (Input.GetButtonDown("Action"))
            {
                SceneManager.LoadScene(stageMenuObjects[stageIndex.y][stageIndex.x].scene);
                GameManager.playerManager.Mode = PlayerMode.Field;
            }
            else if (Input.GetButtonDown("Cancel"))
            {
                currentMenuState = MenuState.MainMenu;
                mainMenu.SetActive(true);
                stageMenu.SetActive(false);
                target = mainMenuObjects[0].transform;
            }
        }
        else if (currentMenuState == MenuState.GameSetting)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                mainView.enabled = false;
                currentMenuState = MenuState.MainMenu;
                mainMenu.SetActive(true);
            }
            else if (!mainView.enabled)
            {
                currentMenuState = MenuState.MainMenu;
                mainMenu.SetActive(true);
            }
        }
        _last = delta;
    }




    void OnDisable()
    {
        menuCamera.gameObject.SetActive(false);
        backGround.SetActive(false);
        mainMenu.SetActive(false);
        stageMenu.SetActive(false);
    }
}
