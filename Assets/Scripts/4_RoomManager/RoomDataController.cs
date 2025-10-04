using System;
using Rooms.Auto;
using UnityEngine;
using Rooms.DoorSystem;
using Rooms.PanelSystem;




#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Rooms.RoomSystem
{
    public enum RoomDistanceState
    {
        Far = 2,
        Near = 1,
        Current = 0,
    }


    public class RoomDataController : MonoBehaviour
    {
        #if UNITY_EDITOR
        [SerializeField, Readonly]
        #endif
        public RoomSetData roomSetData;

        [Space(5)]
        [Header("Room Distance State")]

        [SerializeField]
        private RoomDistanceState _roomDistanceState = RoomDistanceState.Far;
        public RoomDistanceState RoomDistanceState
        {
            get => _roomDistanceState;
            set
            {
                if (_roomDistanceState == value) return;
                _roomDistanceState = value;
                lightParentController.LightState = value;
            }
        }

        [Space(5)]
        [Header("Controllers")]

        public LightParentController lightParentController;
        public DoorParentController doorParentController;
        public ColliderParentController colliderParentController;
        public GimmickParentController gimmickParentController;
        public FbxParentController fbxParentController;


        void Awake()
        {

        }

        public void ResetRoom()
        {
            fbxParentController.ResetRoom();
            doorParentController.ResetRoom();
            colliderParentController.ResetRoom();
            gimmickParentController.ResetRoom();
            lightParentController.ResetRoom();
        }

        public void RefreshRoom()
        {
            fbxParentController.RefreshRoom();
            doorParentController.RefreshRoom();
            colliderParentController.RefreshRoom();
            gimmickParentController.RefreshRoom();
            lightParentController.RefreshRoom();
        }


        #if UNITY_EDITOR
        public void SetUp(RoomSetController roomSetController)
        {
            if (fbxParentController == null)
            {
                foreach (Transform child in transform)
                {
                    if (PrefabUtility.IsPartOfAnyPrefab(child.gameObject))
                    {
                        string assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(child.gameObject);
                        if (assetPath.ToLower().EndsWith(".fbx"))
                        {
                            if (child.gameObject.GetComponent<FbxParentController>() != null)
                            {
                                fbxParentController = child.gameObject.GetComponent<FbxParentController>();
                            }
                            else
                            {
                                fbxParentController = child.gameObject.AddComponent<FbxParentController>();
                                EditorUtility.SetDirty(child.gameObject);
                            }
                            break;
                        }
                    }
                }
            }




            fbxParentController.SetUp(roomSetController);
            doorParentController.SetUp(roomSetController);
            colliderParentController.SetUp(roomSetController);
            gimmickParentController.SetUp(roomSetController);
            lightParentController.SetUp(roomSetController);

            EditorUtility.SetDirty(this);
        }
        #endif
    }
}
