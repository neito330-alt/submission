using System;
using System.Collections.Generic;
using Rooms.Auto;
using Rooms.RoomSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Rendering.HighDefinition;

namespace Rooms.RoomSystem
{
    public class LightParentController : MonoBehaviour
    {

        [Serializable]
        public class HDLightData
        {
            public HDAdditionalLightData hdLight;
            public Light light;
            [HideInInspector] public float intensity;
        }
        [SerializeField]
        private RoomDistanceState _lightState = RoomDistanceState.Far;
        public RoomDistanceState LightState
        {
            get => _lightState;
            set
            {
                _lightState = value;
                if (_animator == null) return;
                _animator.SetInteger("State", (int)_lightState);
                IsMove = true;
            }
        }

        //[SerializeField, Range(0f, 2f)] private float distance = 2f;


        [SerializeField]
        private float _current;
        public float current
        {
            get => _current;
            set
            {
                _current = value;
            }
        }
        // private float nextLight;
        // private float dlta;

        [SerializeField]
        private bool isMove = false;
        public bool IsMove
        {
            get => isMove;
            set => isMove = value;
        }

        [SerializeField] public GameObject farLightParent;
        [SerializeField] public GameObject nearLightParent;
        [SerializeField] public GameObject currentLightParent;

        [SerializeField] private List<HDLightData> farLights;
        public List<HDLightData> FarLights
        {
            get => farLights;
            set => farLights = value;
        }
        [SerializeField] private List<HDLightData> nearLights;
        public List<HDLightData> NearLights
        {
            get => nearLights;
            set => nearLights = value;
        }
        [SerializeField] private List<HDLightData> currentLights;
        public List<HDLightData> CurrentLights
        {
            get => currentLights;
            set => currentLights = value;
        }

        [SerializeField]
        private Animator _animator;

        private float _latestTime = 0f;


        private const float _distanceFactor = 2f; // Factor to control the distance change speed
        [SerializeField]
        private float _distance = 0f;
        public float Distance
        {
            get => _distance;
            set
            {
                if (_distance == value)
                {
                    return; // No change needed
                }
                else if (_distance < value)
                {
                    _distance = Mathf.Min(_distance + _distanceFactor * Time.fixedDeltaTime, value);
                }
                else if (_distance > value)
                {
                    _distance = Mathf.Max(_distance - _distanceFactor * Time.fixedDeltaTime, value);
                }
                SetDistance();
            }
        }


        // [SerializeField] private bool isEditorView = false;

        // [Button("SetLight")] private bool button1 = false;

        private void Awake()
        {
            farLightParent.SetActive(true);
            nearLightParent.SetActive(true);
            currentLightParent.SetActive(true);

            farLights = new List<HDLightData>();
            nearLights = new List<HDLightData>();
            currentLights = new List<HDLightData>();
            if (farLightParent != null)
            {
                foreach (Transform child in farLightParent.transform)
                {
                    HDAdditionalLightData lightData = child.GetComponent<HDAdditionalLightData>();
                    if (lightData != null)
                    {
                        farLights.Add(new HDLightData { light = child.GetComponent<Light>(), hdLight = lightData, intensity = lightData.intensity });
                    }
                }
                farLightParent.SetActive(true);
            }
            if (nearLightParent != null)
            {
                foreach (Transform child in nearLightParent.transform)
                {
                    HDAdditionalLightData lightData = child.GetComponent<HDAdditionalLightData>();
                    //child.GetComponent<Light>().enabled = false;
                    if (lightData != null)
                    {
                        nearLights.Add(new HDLightData { light = child.GetComponent<Light>(), hdLight = lightData, intensity = lightData.intensity });
                    }
                }
                nearLightParent.SetActive(false);
            }
            if (currentLightParent != null)
            {
                foreach (Transform child in currentLightParent.transform)
                {
                    //child.GetComponent<Light>().enabled = false;
                    HDAdditionalLightData lightData = child.GetComponent<HDAdditionalLightData>();
                    if (lightData != null)
                    {
                        currentLights.Add(new HDLightData { light = child.GetComponent<Light>(), hdLight = lightData, intensity = lightData.intensity });
                    }
                }
                currentLightParent.SetActive(false);
            }
        }

        void Start()
        {
        }

        void FixedUpdate()
        {
            if (GameManager.qualityLevel < 2)
            {
                SetDistance(current);
            }
            else
            {
                Distance = GetDistance();
            }
        }


        public float GetDistance()
        {
            float distance = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(GameManager.playerManager.transform.position.x, GameManager.playerManager.transform.position.z)
            );
            if (distance <= 5.657f)
            {
                return 0;
            }
            else if (distance <= 12)
            {
                return (distance - 5.657f) * 0.15765f;
            }
            else if (distance <= 12.65)
            {
                return 1;
            }
            else if (distance <= 20)
            {
                return 1 + (distance - 12.65f) * 0.14f;
            }
            else
            {
                return 2;
            }
        }

        void SetDistance()
        {
            float factor;
            factor = Mathf.Max(0, _distance - 1);
            farLightParent.SetActive(factor > 0);
            foreach (var light in farLights)
            {
                light.hdLight.intensity = factor * light.intensity;
            }
            factor = 1 - Mathf.Abs(1 - _distance);
            nearLightParent.SetActive(factor > 0);
            foreach (var light in nearLights)
            {
                light.hdLight.intensity = factor * light.intensity;
            }
            factor = Mathf.Max(1 - _distance, 0);
            currentLightParent.SetActive(factor > 0);
            foreach (var light in currentLights)
            {
                light.hdLight.intensity = factor * light.intensity;
            }
        }

        void SetDistance(float distance)
        {
            _distance = Mathf.Clamp(distance,0,2);
            SetDistance();
        }

        public void SetDistance(RoomDistanceState state)
        {
            LightState = state;
            _animator.SetTrigger("FastEvent");
            SetDistance((int)state);
        }

        public void ResetRoom()
        {

        }

        public void RefreshRoom()
        {
            // This method can be used to refresh the room's light settings if needed.
            // Currently, it does nothing but can be extended in the future.
        }

        void OnValidate()
        {
            farLightParent.SetActive(LightState == RoomDistanceState.Far);
            nearLightParent.SetActive(LightState == RoomDistanceState.Near);
            currentLightParent.SetActive(LightState == RoomDistanceState.Current);
        }

#if UNITY_EDITOR
        public void SetUp(RoomSetController roomSetController)
        {
        }
        #endif
    }
}
