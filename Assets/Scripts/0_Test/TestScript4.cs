using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

#if UNITY_EDITOR
public class TestScript4 : MonoBehaviour
{
    [SerializeField,Range(0,1)]private float _shadow;
    [SerializeField,Range(0,1)]private float _radius;
    [SerializeField]private bool _isOn = true;
    [SerializeField,Range(0,1)]private float _dimmer = 1.0f;
    [SerializeField,Range(0,1)]private float _volumetricDimmer = 1.0f;
    void Start()
    {
        Debug.Log("TestScript4");
    }

    void Update()
    {
        Light light = GetComponent<Light>();
        light.shadowStrength = _shadow;
        light.shadowRadius = _radius;
        HDAdditionalLightData hdLightData = GetComponent<HDAdditionalLightData>();
        hdLightData.affectsVolumetric = _isOn;
        hdLightData.shadowDimmer = _dimmer;
        hdLightData.volumetricShadowDimmer = _volumetricDimmer;
    }
}
#endif