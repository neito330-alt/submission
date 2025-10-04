using UnityEngine;

public class ItemGlowEffect : MaterialPropertyChanger
{
    [SerializeField]
    private Color _color;

    [SerializeField]
    private float _power;

    [SerializeField]
    private float _emission;

    protected override void SetProperties()
    {
        material.SetColor("_Color",_color);
        material.SetFloat("_Power",_power);
        material.SetFloat("_Emission",_emission);
    }
}
