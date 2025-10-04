using UnityEngine;
using UnityEditor;

[ExecuteAlways]
[RequireComponent(typeof(SkinnedMeshRenderer))]
public class MultiMaterialPropertyChanger : MonoBehaviour
{
    [SerializeField]
    protected SkinnedMeshRenderer meshRenderer;
    [SerializeField]Material defaultMaterial;
    [SerializeField]
    protected Material material;
    [SerializeField]
    protected Material[] materials;
    [SerializeField]protected int materialIndex = 0;

    void Start()
    {
        Debug.Log("start");
    }

    void Update()
    {
        int i = 0;
        foreach (Material m in meshRenderer.sharedMaterials)
        {
            if (m == null)i++;
        }
        Debug.Log("update:"+i);
        if (!(materials.Length > materialIndex) || materials[materialIndex] == null)
        {
            #if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                materials = meshRenderer.materials;
                material = materials[materialIndex];
                meshRenderer.materials = materials;
            }
            else
            {
                materials = meshRenderer.sharedMaterials;
                if (materials[materialIndex] == null)
                {
                    materials[materialIndex] = new Material(defaultMaterial);
                }
                else
                {
                    materials[materialIndex] = new Material(materials[materialIndex]);
                }
                meshRenderer.materials = materials;
            }
            #else
            materials = meshRenderer.materials;
            #endif
        }
        SetProperties();
        #if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            meshRenderer.materials = materials;
        }
        else
        {
            meshRenderer.sharedMaterials = materials;
        }
        #endif
    }

    void OnDestroy()
    {
        Debug.Log("destroy");
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

        if (materials != null)
        {
# if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] != null && !AssetDatabase.Contains(materials[i]))
                    {
                        Destroy(materials[i]);
                    }
                    {
                        Destroy(materials[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] != null && !AssetDatabase.Contains(materials[i]))
                    {
                        DestroyImmediate(materials[i]);
                    }
                }
            }
# else
            for (int i = 0; i < materials.Length; i++)
            {
                # if UNITY_EDITOR
                if (materials[i] != null && !AssetDatabase.Contains(materials[i]))
                #endif
                {
                    Destroy(materials[i]);
                }
            }
# endif
        }
    }

    protected virtual void SetProperties()
    {
    }
}
