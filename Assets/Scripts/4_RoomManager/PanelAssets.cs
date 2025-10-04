using System;
using UnityEngine;

namespace Rooms.PanelSystem
{
    [Serializable]
    public class PanelCornerMaterial
    {
        public Material Normal;
        public Material Static;
        public Material Rotate;
    }

    [Serializable]
    public class PanelDoorMaterial
    {
        public Material Open;
        public Material Lock;
        public Material Key;
        public Material Danger;
        public Material Red;
        public Material Blue;
        public Material Green;
        public Material Yellow;
        public Material Purple;
        public Material Orange;
        public Material Black;
    }

    [Serializable]
    public class FramePrefab
    {
        [SerializeField, CustomLabel("横フレーム")]
        public GameObject frameSide;

        [SerializeField, CustomLabel("角フレーム")]
        public GameObject frameCorner;

        [SerializeField, CustomLabel("通常スロット")]
        public GameObject slotNormal;

        [SerializeField, CustomLabel("空スロット")]
        public GameObject slotVoid;
    }

    [Serializable]
    public class PanelSound
    {
        [SerializeField, CustomLabel("ピックアップ音")]
        public AudioClip pickSound;

        [SerializeField, CustomLabel("配置音")]
        public AudioClip placeSound;

        [SerializeField, CustomLabel("回転音")]
        public AudioClip rotateSound;

        [SerializeField, CustomLabel("移動音")]
        public AudioClip moveSound;

        [SerializeField, CustomLabel("停止音")]
        public AudioClip stopSound;

        [SerializeField, CustomLabel("エラー音")]
        public AudioClip errorSound;

        [SerializeField, CustomLabel("エンター音")]
        public AudioClip enterSound;

        [SerializeField, CustomLabel("エグジット音")]
        public AudioClip exitSound;
    }

    [Serializable]
    public class PanelSystemMaterials
    {
        [SerializeField, CustomLabel("カーソルマテリアル")]
        public Material cursorMaterial;

        [SerializeField, CustomLabel("パネルエフェクト：ホログラム")]
        public Material hologramMaterial;

        [SerializeField, CustomLabel("パネルエフェクト：発光")]
        public Material glowMaterial;
    }




    /// <summary>
    /// Represents a panel asset with a name and a prefab.
    /// </summary>
    [CreateAssetMenu(fileName = "PanelAsset", menuName = "ScriptableObjects/Panel Asset", order = 1)]
    public class PanelAssets : ScriptableObject
    {
        [SerializeField, CustomLabel("システムマテリアル")]
        public PanelSystemMaterials systemMaterials;
        
        [SerializeField, CustomLabel("パネル：コーナーマテリアル")]
        public PanelCornerMaterial cornerMaterial;

        [SerializeField, CustomLabel("パネル：ドアマテリアル")]
        public PanelDoorMaterial doorMaterial;

        [SerializeField, CustomLabel("パネル：フレームプレハブ")]
        public FramePrefab framePrefab;

        [SerializeField, CustomLabel("パネル：サウンド")]
        public PanelSound panelSound;
    }
}
