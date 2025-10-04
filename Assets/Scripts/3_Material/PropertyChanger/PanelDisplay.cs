using UnityEngine;

public class PanelDisplay : MultiMaterialPropertyChanger
{
    [SerializeField]
    private Texture _displayTex;
    [SerializeField]
    private float _emission;

    protected override void SetProperties()
    {
        materials[materialIndex].SetTexture("_DisplayTex",_displayTex);
        materials[materialIndex].SetFloat("_Emission",_emission);
    }
}
