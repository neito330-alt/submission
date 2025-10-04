using UnityEngine;
# if UNITY_EDITOR
using UnityEditor;
# endif

[ExecuteAlways]
[RequireComponent(typeof(MeshRenderer))]
public class MaterialPropertyChanger : MonoBehaviour
{
    MeshRenderer meshRenderer;
    [SerializeField]Material defaultMaterial;
    protected Material material;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (meshRenderer.sharedMaterial == null)
        {
            meshRenderer.material = new Material(defaultMaterial);
        }
        if (material == null)
            {
#if UNITY_EDITOR
                if (EditorApplication.isPlaying)
                {
                    material = meshRenderer.material;
                }
                else
                {
                    material = new Material(meshRenderer.sharedMaterial);
                    meshRenderer.sharedMaterial = material;
                }
#else
            material = meshRenderer.material;
#endif
            }
        SetProperties();
    }

    void OnDestroy()
    {
        if (material != null)
        {
# if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                Destroy(material);
            }
            else
            {
                DestroyImmediate(material);
            }
# else
            Destroy(material);
# endif
        }
    }

    protected virtual void SetProperties()
    {

    }
}
