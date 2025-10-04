using UnityEngine;
using UnityEngine.UIElements;

public class UISliderController
{
    private readonly SliderInt _slider;

    protected string _sliderName = "";

    public UISliderController(VisualElement visualElement)
    {
        // VisualElementからSliderIntを取得
        _slider = visualElement.Q<SliderInt>(_sliderName);

        // スライダーの値が変更されたときのイベントを登録
        _slider.RegisterValueChangedCallback(evt =>
        {
            SetSliderValue(evt.newValue);
        });
        
    }

    public virtual void SetSliderValue(int value)
    {
    }
}

public sealed class QualitySliderController : UISliderController
{
    public QualitySliderController(VisualElement visualElement) : base(visualElement)
    {
        _sliderName = "QualitySlider"; // スライダーの名前を設定
    }

    public override void SetSliderValue(int value)
    {
        QualitySettings.SetQualityLevel(value);
    }
}