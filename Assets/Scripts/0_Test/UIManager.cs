using System;
using UnityEngine;

namespace UIManager
{
    [Serializable]
    public class FieldUI
    {
        public GameObject Pointer;
    }

    [Serializable]
    public class MapUI
    {
        public GameObject DecalViewer;
    }



    public class UIManager : MonoBehaviour
    {
        [SerializeField] public FieldUI fieldUI;
        [SerializeField] public MapUI mapUI;

        void Awake()
        {
            GameManager.uiManager = this;
        }
    }
}





