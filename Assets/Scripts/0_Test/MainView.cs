using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class MainView : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset _elementTemplate;

    private SliderInt _qualitySlider;
    private SliderInt _volumeSlider;
    private SliderInt _mouseSensitivitySlider;
    private SliderInt _padSensitivitySlider;
    private SliderInt _mapSensitivitySlider;
    private Button _applyButton;

    private UIDocument _uiDocument;

    void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None; // カーソルをロックしない

        // UIDocumentコンポーネントについては次節で説明
        var uiDocument = GetComponent<UIDocument>();
        uiDocument.enabled = true;

        _volumeSlider = uiDocument.rootVisualElement.Q<SliderInt>("VolumeSlider");
        _qualitySlider = uiDocument.rootVisualElement.Q<SliderInt>("QualitySlider");
        _mouseSensitivitySlider = uiDocument.rootVisualElement.Q<SliderInt>("MouseSlider");
        _padSensitivitySlider = uiDocument.rootVisualElement.Q<SliderInt>("PadSlider");
        _mapSensitivitySlider = uiDocument.rootVisualElement.Q<SliderInt>("MapSlider");
        _applyButton = uiDocument.rootVisualElement.Q<Button>("ApplyButton");
        _qualitySlider.RegisterValueChangedCallback(evt => OnQualitySliderChanged(evt.newValue));
        _mouseSensitivitySlider.RegisterValueChangedCallback(evt => OnMouseSensitivitySliderChanged(evt.newValue));
        _padSensitivitySlider.RegisterValueChangedCallback(evt => OnPadSensitivitySliderChanged(evt.newValue));
        _applyButton.clicked += OnApplyButtonClicked;

        _qualitySlider.value = QualitySettings.GetQualityLevel();
        _volumeSlider.value = (int)(AudioListener.volume * 100f); // 音量を0〜100の範囲に変換
        _mouseSensitivitySlider.value = (int)GameManager.playerManager.FieldCameraController.MouseSensitivity;
        _padSensitivitySlider.value = (int)GameManager.playerManager.FieldCameraController.PadSensitivity;
        _mapSensitivitySlider.value = (int)GameManager.playerManager.MapCameraController.MouseSensitivity;
    }

    private void OnDisable()
    {
        _uiDocument.enabled = false; // UIを閉じる
        UnityEngine.Cursor.lockState = CursorLockMode.Locked; // カーソルをロックする
    }

    private void OnQualitySliderChanged(int newValue)
    {
        //QualitySettings.SetQualityLevel(newValue);
    }

    private void OnMouseSensitivitySliderChanged(int newValue)
    {
        // マウス感度の設定を行う処理
        //Debug.Log($"Mouse Sensitivity changed to: {newValue}");
    }

    private void OnPadSensitivitySliderChanged(int newValue)
    {
        // パッド感度の設定を行う処理
        //Debug.Log($"Pad Sensitivity changed to: {newValue}");
    }

    private void OnApplyButtonClicked()
    {
        QualitySettings.SetQualityLevel(_qualitySlider.value);
        AudioListener.volume = _volumeSlider.value / 100f; // 音量を0〜1の範囲に変換
        GameManager.qualityLevel = _qualitySlider.value;
        GameManager.playerManager.FieldCameraController.MouseSensitivity = _mouseSensitivitySlider.value;
        GameManager.playerManager.FieldCameraController.PadSensitivity = _padSensitivitySlider.value;
        GameManager.playerManager.MapCameraController.MouseSensitivity = _mapSensitivitySlider.value;
        _uiDocument.enabled = false; // UIを閉じる
        this.enabled = false; // UIを閉じる
        UnityEngine.Cursor.lockState = CursorLockMode.Locked; // カーソルをロックする
    }
}
