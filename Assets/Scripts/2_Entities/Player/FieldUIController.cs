using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;






public class FieldUIController : MonoBehaviour
{
    [SerializeField, CustomLabel("ポインター")]
    private Image _pointer;

    [SerializeField, CustomLabel("インベントリ―")]
    private Transform _inventory;

    [SerializeField, CustomLabel("アイテムプレハブ")]
    private GameObject _itemPrefab;


    [SerializeField, CustomLabel("ポインターカラー")]
    private Color _pointerColor = Color.white;
    public Color PointerColor
    {
        get => _pointerColor;
        set
        {
            _pointerColor = value;
            if (_pointer != null) _pointer.color = _pointerColor;
        }
    }


    public ItemUIController AddItem(PlayerItem item)
    {
        if (_itemPrefab == null) return null;

        var itemObject = Instantiate(_itemPrefab, _inventory.transform);
        var itemController = itemObject.GetComponent<ItemUIController>();
        if (itemController != null)
        {
            itemController.ItemData = item;
            return itemController;
        }
        else
        {
            Destroy(itemObject);
            return null;
        }
    }
}