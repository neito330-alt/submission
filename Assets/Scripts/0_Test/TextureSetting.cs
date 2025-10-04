using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class TextureSetting : MonoBehaviour
{
    [SerializeField] private Texture2D _texture;
    [SerializeField] private Renderer _renderer;

    private Material _material;
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    private void Awake()
    {
#if UNITY_EDITOR
        _material = EditorApplication.isPlaying
            ? _renderer.material
            : new Material(_renderer.sharedMaterial);
#else
            _material = _renderer.material;
#endif

        _material.SetTexture(MainTex, _texture);
        _renderer.material = _material;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (EditorApplication.isPlaying || _material == null) // Material生成前に呼ばれることがある
        {
            return;
        }

        _material.SetTexture(MainTex, _texture);
        _renderer.material = _material;
    }
#endif

    private void OnDestroy()
    {
        if (_material != null)
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                Destroy(_material);
            }
            else
            {
                DestroyImmediate(_material);
            }
#else
            Destroy(_material);
#endif
        }
    }
}
