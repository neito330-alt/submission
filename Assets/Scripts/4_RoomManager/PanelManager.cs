using System;
using System.Collections.Generic;
using Rooms.Auto;
using Rooms.RoomSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.HighDefinition;

namespace Rooms.PanelSystem
{
    public static class RoomPanelModeEnum
    {
        public static Color ToColor(this Vector3 vector)
        {
            return new Color(vector.x, vector.y, vector.z);
        }

        public static Vector3 ToVector3(this Color color)
        {
            return new Vector3(color.r, color.g, color.b);
        }

        public static Color SetEmission(this Color color, float intensity)
        {
            return new Color(color.r * intensity, color.g * intensity, color.b * intensity, color.a);
        }


    }



    public class PanelManager : MonoBehaviour
    {
        public class SlotDataHolder
        {
            public SlotDataHolder()
            {
            }

            public bool isExist;
            public bool isSlot;
            public PanelDataController panelDataController;
            public RoomMode roomMode = RoomMode.Normal;


            public bool IsEmpty => isExist && isSlot && panelDataController == null;
            public bool IsNotEmpty => isExist && isSlot && panelDataController != null;

            public bool IsMoveable => isExist && isSlot && (panelDataController == null || roomMode.CanMove());
            public bool IsRotatable => isExist && isSlot && panelDataController != null && roomMode.CanRotate();
            public bool IsHoldable => isExist && isSlot && panelDataController != null && roomMode.CanHold();

        }


        [SerializeField]
        private StageManager _stageManager
        {
            get => GameManager.stageManager;
        }


        public PanelDataController[,] data;


        [SerializeField,CustomLabel("フレーム格納オブジェクト")]
        public GameObject _frameParent;

        [SerializeField,CustomLabel("スロット格納オブジェクト")]
        public GameObject _slotParent;

        [SerializeField,CustomLabel("部屋パネル格納オブジェクト")]
        public GameObject _panelParent;

        [SerializeField,CustomLabel("部屋パネルカメラ")]
        public Camera _panelCamera;


        /// <summary>
        /// オブジェクトの中心から左上のスロット座標までのベクトル。自動で設定される
        /// </summary>
        [SerializeField,CustomLabel("オフセット")]
        public Vector3 _offset = new Vector3(0,0,0);
        public Vector3 Offset => _offset;

        [SerializeField,CustomLabel("マップ")]
        public bool isMap = false;




        // Start is called before the first frame update
        void Start()
        {
            RoomsManager.RoomsUpdated += PanelUpdate;
            RoomsManager.DecalAdded += DecalAdd;
            RoomsManager.DecalRemoved += DecalRemove;

            Vector2Int size = _stageManager.stageDataController.Size;

            // パネルボードのスケールを計算
            float scale = 1f/(1+Mathf.Max(size.x,size.y));
            transform.localScale = Vector3.one * scale;
            if (_panelCamera != null)
            {
                // パネルカメラのスケールを設定
                _panelCamera.orthographicSize = scale * 0.5f;
            }
            
            // オフセットを計算
            _offset = GetWorldPosition(new Vector2(-(size.x-1)*0.5f,-(size.y-1)*0.5f));

            data = new PanelDataController[size.y,size.x];

            // パネルを配置する。外周にフレームを生成するため-1, -1を追加
            for (int y = -1; y < size.y + 1; y++)
            {
                for (int x = -1; x < size.x + 1; x++)
                {
                    float rot = 0;

                    GameObject slot;
                    GameObject parentObj;

                    // フレームならtrue
                    if (x < 0 || y < 0 || x >= size.x || y >= size.y)
                    {
                        rot = Mathf.Atan2(
                            y < 0 ? 1 : (y >= size.y ? -1 : 0),
                            x < 0 ? 1 : (x >= size.x ? -1 : 0)) * Mathf.Rad2Deg;

                        parentObj = _frameParent;

                        // コーナーかサイドか
                        if ((x < 0 || x >= size.x) && (y < 0 || y >= size.y))
                        {
                            slot = RoomsAssetsManager.PanelAssets.framePrefab.frameCorner;
                            rot -= 135;
                        }
                        else
                        {
                            slot = RoomsAssetsManager.PanelAssets.framePrefab.frameSide;
                            rot += 0;
                        }
                    }
                    else
                    {


                        // スロットの場合はスロットとパネルを生成
                        slot = _stageManager.stageDataController.Data[y][x].isSlot ? RoomsAssetsManager.PanelAssets.framePrefab.slotNormal : RoomsAssetsManager.PanelAssets.framePrefab.slotVoid;
                        parentObj = _slotParent;

                        if (_stageManager.stageDataController.Data[y][x].IsNotEmpty)
                        {
                            GameObject panelObj = Instantiate(
                                _stageManager.stageDataController.Data[y][x].RoomSetData.RoomSetController.panelDataController.gameObject,
                                Vector3.zero,
                                Quaternion.identity,
                                _panelParent.transform
                            );

                            panelObj.transform.localPosition = GetWorldPosition(new Vector2(x, y), 0.1f) + _offset;
                            panelObj.transform.localScale = Vector3.one;


                            data[y,x] = panelObj.GetComponent<PanelDataController>();

                            data[y,x].slotData = _stageManager.stageDataController.Data[y][x];
                            data[y,x].roomSetData = _stageManager.stageDataController.Data[y][x].RoomSetData;
                            data[y,x].Rotation = _stageManager.stageDataController.Data[y][x].RoomSetData.Rotation;

                            data[y,x].decalManager.ClearDecals();
                            foreach (DecalData decal in data[y,x].slotData.RoomSetData.RoomSetController.DecalDataList)
                            {
                                data[y,x].AddDecal(decal);
                            }

                            if (isMap) data[y,x].slotData.RoomSetData.PanelDataController = data[y,x];
                        }
                        else
                        {
                            data[y,x] = null;
                        }
                    }

                    slot = Instantiate(
                        slot,
                        Vector3.zero,
                        Quaternion.identity,
                        parentObj.transform
                    );

                    slot.transform.parent = parentObj.transform;
                    slot.transform.localPosition = GetWorldPosition(new Vector2(x, y)) + _offset;
                    slot.transform.localScale = Vector3.one;
                    slot.transform.localEulerAngles = new Vector3(0, rot, 0);
                }
            }

            //RoomsManager.OnRoomsUpdated();
        }

        void OnDestroy()
        {
            RoomsManager.RoomsUpdated -= PanelUpdate;
            RoomsManager.DecalAdded -= DecalAdd;
            RoomsManager.DecalRemoved -= DecalRemove;
        }

        public void DecalAdd(Vector2Int position, DecalItem decalItem)
        {
            PanelDataController panel = GetPanelData(position);
            if (panel != null)
            {
                panel.AddDecal(decalItem);
            }
        }

        public void DecalRemove(Vector2Int position, Vector2 decalPosition)
        {
            PanelDataController panel = GetPanelData(position);
            if (panel != null)
            {
                panel.decalManager.UpdateDecal();
            }
        }

        // 便利関数

        public SlotDataHolder GetPanelSlotData(Vector2Int position)
        {
            if (
                Mathf.Clamp(position.y, 0, data.GetLength(0) - 1) == position.y &&
                Mathf.Clamp(position.x, 0, data.GetLength(1) - 1) == position.x
            )
            {
                return new SlotDataHolder()
                {
                    isExist = true,
                    isSlot = _stageManager.stageDataController.Data[position.y][position.x].isSlot,
                    panelDataController = data[position.y, position.x],
                    roomMode = data[position.y, position.x]?.roomSetData?.RoomSetController?.roomMode ?? RoomMode.Normal
                };
            }
            return new SlotDataHolder()
            {
                isExist = false,
                isSlot = false,
                panelDataController = null,
                roomMode = RoomMode.Normal
            };
        }

        /// <summary>
        /// 指定した位置のパネルスロットデータを取得する
        /// <para>スロット座標が範囲外の場合は空のSlotDataを返す</para>
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public PanelDataController GetPanelData(Vector2Int position)
        {
            if (
                Mathf.Clamp(position.y, 0, data.GetLength(0)) == position.y &&
                Mathf.Clamp(position.x, 0, data.GetLength(1)) == position.x
            )
            {
                return data[position.y,position.x];
            }
            return null;
        }




        /// <summary>
        /// 指定した2D座標をワールド座標に変換する.ワールド座標だがローカル座標系である.
        /// <para>y_offsetはワールド座標のy座標に加算されるオフセット</para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="y_offset"></param>
        /// <returns></returns>
        public Vector3 GetWorldPosition(Vector2 position,float y_offset = 0)
        {
            return new Vector3(position.x  ,y_offset , -position.y);
        }



        // パネル操作関数

        /// <summary>
        /// 指定した位置のパネルを移動させる
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>移動に成功した場合はTrue</returns>
        public bool PanelMove(Vector2Int from, Vector2Int to)
        {
            SlotData fromSlot = _stageManager.stageDataController.GetPanelSlotData(from);
            SlotData toSlot = _stageManager.stageDataController.GetPanelSlotData(to);

            SlotDataHolder fromPanel = GetPanelSlotData(from);
            SlotDataHolder toPanel = GetPanelSlotData(to);

            // fromDataとtoDataがスロットであり、かつ移動可能な場合のみ移動を行う
            if (
                fromSlot.isSlot && toSlot.isSlot &&
                (fromPanel.IsEmpty || fromPanel.IsMoveable) &&
                (toPanel.IsEmpty || toPanel.IsMoveable)&&
                (from != to)
            )
            {
                if (fromPanel.IsNotEmpty)
                {
                    data[to.y, to.x] = fromPanel.panelDataController;
                    fromPanel.panelDataController.transform.localPosition = GetWorldPosition(to, 0.1f) + Offset;
                }
                else
                {
                    // fromPanelがnullの場合はtoPanelをnullにする
                    data[to.y, to.x] = null;
                }
                if (toPanel.IsNotEmpty)
                {
                    data[from.y, from.x] = toPanel.panelDataController;
                    toPanel.panelDataController.transform.localPosition = GetWorldPosition(from, 0.1f) + Offset;
                }
                else
                {
                    // toPanelがnullの場合はfromPanelをnullにする
                    data[from.y, from.x] = null;
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// 指定した位置のパネルスロットデータを回転させる
        /// </summary>
        /// <param name="position">回転させるパネルのスロット座標</param>
        /// <param name="rotate">回転角度。デフォルトは90度</param>
        /// <returns>回転に成功した場合はTrue</returns>
        public bool PanelRotate(Vector2Int position, int rotate = 90)
        {
            SlotDataHolder slotPanel = GetPanelSlotData(position);
            if (!slotPanel.isSlot || slotPanel.IsEmpty || !slotPanel.IsRotatable)
            {
                return false;
            }

            slotPanel.panelDataController.Rotation = rotate;
            return true;
        }


        public void PanelApply()
        {
            Debug.Log("panelaaply");
            for (int y = 0; y < data.GetLength(0); y++)
            {
                for (int x = 0; x < data.GetLength(1); x++)
                {
                    if (data[y,x] != null)
                    {
                        //Debug.Log(new Vector2Int(x,y)+ "PanelApply: " + data[y,x].slotData.Position);
                        data[y,x].roomSetData.Rotation = data[y,x].Rotation;
                        _stageManager.stageDataController.Data[y][x].RoomSetData = data[y,x].roomSetData;
                        //_stageManager.stageDataController.data[y][x].RoomSetData.Position = new Vector2Int(x, y);
                    }
                    else
                    {
                        _stageManager.stageDataController.Data[y][x].RoomSetData = new RoomSetData();
                        //_stageManager.stageDataController.data[y][x].RoomSetData.Position = new Vector2Int(x, y);
                    }
                }
            }
            //RoomsManager.OnRoomsUpdated();
        }

        public void PanelUpdate()
        {
            Debug.Log("panelupdate");
            PanelDataController[,] newData = new PanelDataController[data.GetLength(0), data.GetLength(1)];
            for (int y = 0; y < data.GetLength(0); y++)
            {
                for (int x = 0; x < data.GetLength(1); x++)
                {
                    if (data[y,x] != null)
                    {
                        Vector2Int pos = data[y,x].roomSetData.RoomDataController.roomSetData.Position;
                        newData[pos.y,pos.x] = data[y,x];
                        newData[pos.y,pos.x].Rotation = data[y,x].roomSetData.RoomDataController.roomSetData.Rotation;
                        newData[pos.y,pos.x].transform.localPosition = GetWorldPosition(pos, 0.1f) + Offset;
                    }
                }
            }
            data = newData;
        }
    }
}
