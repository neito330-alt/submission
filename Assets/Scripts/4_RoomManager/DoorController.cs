using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using System.Security.Cryptography;
using System.Collections.Generic;
using System;


namespace Rooms.DoorSystem
{
    public class DoorController : MonoBehaviour
    {
        [SerializeField]
        private SkinnedMeshRenderer _wallMesh;
        public SkinnedMeshRenderer WallMesh
        {
            get => _wallMesh;
            set => _wallMesh = value;
        }

        [SerializeField]
        private int _shapeKey = 0;
        public int ShapeKey
        {
            get => _shapeKey;
            set => _shapeKey = value;
        }

        [SerializeField]
        private InteractiveDoor _door;
        public InteractiveDoor Door
        {
            get => _door;
            set => _door = value;
        }

        [SerializeField]
        private DoorLockDataList _lockList;
        public DoorLockDataList LockList
        {
            get => _lockList;
            set => _lockList = value;
        }

        private bool _isChecked = true;
        public bool IsChecked
        {
            get => _isChecked;
            set => _isChecked = value;
        }




        void Awake()
        {
            Door.IsDefaultActive = _wallMesh == null;
        }



        public void ResetDoor()
        {
            LockList.ResetLock();

            gameObject.SetActive(true);
            Door.gameObject.SetActive(true);

            _isChecked = true;
            SetWallShape(0);

            Door.ResetDoor();
        }

        public void RefreshDoor()
        {
            _isChecked = true;
            SetWallShape(0);
        }



        public void CollisionCheck()
        {
            // IsCheckedがfalseの時はドアの状態を更新しない
            if (_isChecked)
            {
                // ドアの座標から上方向にRayを飛ばしてドアを探す
                RaycastHit[] hits = Physics.RaycastAll(
                    transform.position,
                    new Vector3(0,1,0),
                    2,
                    LayerMask.GetMask("Gimmick")
                );

                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.tag != "Door") continue;

                    if (transform != hit.transform)
                    {
                        DoorController hitDoor = hit.collider.GetComponent<DoorController>();

                        if (hitDoor == null) continue;

                        // ドアのレベルを比較して、どちらのドアをアクティブにするか決定
                        if (Door.Level >= hitDoor.Door.Level)
                        {
                            DoorSetting(this, hitDoor);
                        }
                        else
                        {
                            DoorSetting(hitDoor, this);
                        }
                        return;
                    }
                }

                // Rayが何も当たらなかった場合
                SetWallShape(0);
                Door.gameObject.SetActive(true);
                Door.IsActive = false;
            }
        }


        private void SetWallShape(int value)
        {
            if (_wallMesh != null)
            {
                _wallMesh.SetBlendShapeWeight(_shapeKey, value);
            }
        }


        private void DoorSetting(DoorController door1, DoorController door2)
        {
            door1._isChecked = false;
            door2._isChecked = false;

            door1.SetWallShape(100);
            door2.SetWallShape(100);

            door1.Door.gameObject.SetActive(true);
            door2.Door.gameObject.SetActive(false);

            door1.Door.ResetDoor();

            door1.Door.DoorController1 = door1;
            door1.Door.DoorController2 = door2;
            door1.Door.LockData = door1.LockList + door2.LockList;

            door1.Door.IsActive = true;
        }
    }
}
