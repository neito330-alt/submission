using UnityEngine;
using UnityEngine.UI;

public class MapUIController : MonoBehaviour
{
    [SerializeField] private Image decalViewer;

    [SerializeField] private Sprite _sprite;
    public Sprite Sprite
    {
        get => _sprite;
        set
        {
            _sprite = value;
            if (decalViewer != null)
            {
                decalViewer.sprite = _sprite;
            }
        }
    }

    [SerializeField] private Color _color;
    public Color Color
    {
        get => _color;
        set
        {
            _color = value;
            if (decalViewer != null)
            {
                decalViewer.color = _color;
            }
        }
    }

}
