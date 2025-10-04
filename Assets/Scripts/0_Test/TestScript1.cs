using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class TestScript1 : MonoBehaviour
{
    public Material material;

    void Start()
    {
        DecalProjector decal = transform.gameObject.AddComponent<DecalProjector>();
        decal.material = new Material(material);
    }

}