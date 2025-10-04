using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUIController : MonoBehaviour
{
    [SerializeField, CustomLabel("Image")]
    private Image _image;

    [SerializeField, CustomLabel("Text")]
    private TextMeshProUGUI _text;

    [SerializeField, CustomLabel("アイテムデータ")]
    private PlayerItem _itemData;
    public PlayerItem ItemData
    {
        get => _itemData;
        set => SetItem(value);
    }

    
    private void SetItem(PlayerItem item)
    {
        _itemData = item;
        
        if (_image != null)
        {
            _image.sprite = item.Icon;
            _image.color = item.IconColor;
            _image.enabled = item.Icon != null;
        }

        if (_text != null)
        {
            _text.enabled = item.IsStackable;
            _text.text = $"x{item.Count}";
        }
    }
}
