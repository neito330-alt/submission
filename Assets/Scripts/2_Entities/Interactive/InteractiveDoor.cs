using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.ProBuilder.Shapes;

using Rooms.DoorSystem;


public class InteractiveDoor : InteractiveObject
{
    [Serializable]
    public class DoorBodyData
    {
        public GameObject DoorBody;
        public int materialIndex = 0;

        public Material baseMaterial;
        public Material newMat;
    }

    [Serializable]
    public class DoorKeyHoleData
    {
        public enum DoorKeyHoleType
        {
            Hole,
            Frame,
        }

        public DoorKeyHoleType KeyHoleType = DoorKeyHoleType.Frame;
        public GameObject DoorKeyHole;
        public int materialIndex = 0;

        public Material baseMaterial;
        public Material newMat;
    }


    [Header("親")]
    [SerializeField]
    private DoorController _doorController1;
    public DoorController DoorController1
    {
        get => _doorController1;
        set => _doorController1 = value;
    }

    [SerializeField]
    private DoorController _doorController2;
    public DoorController DoorController2
    {
        get => _doorController2;
        set => _doorController2 = value;
    }


    [Header("ユーザー設定")]
    [SerializeField]
    public GameObject DoorObj;

    [SerializeField]
    public List<DoorBodyData> DoorBody = new List<DoorBodyData>();

    [SerializeField]
    public List<DoorKeyHoleData> DoorKeyBody1 = new List<DoorKeyHoleData>();
    [SerializeField]
    public List<DoorKeyHoleData> DoorKeyBody2 = new List<DoorKeyHoleData>();

    private Dictionary<string, List<DoorKeyHoleData>> _doorKeyHoleDataDict = new Dictionary<string, List<DoorKeyHoleData>>();

    [SerializeField]
    public GameObject PliminaryWall;

    [SerializeField]
    private int _level = 5;
    public int Level
    {
        get => _level;
        set
        {
            if (value < 0) value = 0;
            if (value > 10) value = 10;
            _level = value;
        }
    }


    [Header("サウンド")]
    [SerializeField]
    private AudioClip _openSound;
    [SerializeField]
    private AudioClip _closeSound;
    [SerializeField]
    private AudioClip _unLockSound;
    [SerializeField]
    private AudioClip _lockSound;
    [SerializeField]
    private AudioClip _knockSound;


    [Header("マテリアル")]
    [SerializeField]
    private Material _keyHoleMaterial;

    [SerializeField]
    private Material _keyHoleLockMaterial;
    [SerializeField]
    private Material _keyHoleUnLockMaterial;

    private List<Material> _doorBodyMaterials = new List<Material>();
    private Material[] _keyHoleMaterialInstances = new Material[2];


    [Header("コンポーネント")]
    [SerializeField]
    private DoorEventHandler _doorEventHandler;

    [SerializeField]
    private BoxCollider _collider;

    [SerializeField]
    private Animator _animator;

    private AudioSource _audioSource;


    [Header("編集禁止(閲覧用)")]

    [SerializeField]
    private DoorLockDataList _lockData = new DoorLockDataList();
    public virtual DoorLockDataList LockData
    {
        get => _lockData;
        set
        {
            // 既存のイベントハンドラを解除
            foreach (DoorLockData data in _lockData)
            {
                if (data.LockType == DoorLockType.Event && ((EventData)data.Data).EventObject != null)
                {
                    // イベントオブジェクトからドアを削除
                    ((EventData)data.Data).EventObject.Objects.Remove(_doorEventHandler);
                }
            }


            _lockData = value;
            _lockData.LockedEvent += LockEvent;
            _lockData.UnLockedEvent += UnLockedEvent;
            _lockData.LockStateChangedEvent += LockStateChangedEvent;


            Color color = _lockData.GetLockColor().ToMaterialColor();

            List<Material> tmpList = new List<Material>(_doorBodyMaterials);

            _doorBodyMaterials.Clear();

            foreach (DoorBodyData doorBody in DoorBody)
            {
                Renderer renderer = doorBody.DoorBody.GetComponent<Renderer>();
                Material oldMat = renderer.sharedMaterials[doorBody.materialIndex];
                Material newMat;
                if (LockData.IsExist(DoorLockType.Color))
                {
                    newMat = new Material(doorBody.baseMaterial);
                    doorBody.newMat = newMat;

                    newMat.color = color;
                    if (newMat.HasProperty("_Saturation"))newMat.SetFloat("_Saturation", 0f);
                }
                else
                {
                    newMat = doorBody.baseMaterial;
                }

                Material[] materials = doorBody.DoorBody.GetComponent<Renderer>().sharedMaterials;
                materials[doorBody.materialIndex] = newMat;
                doorBody.DoorBody.GetComponent<Renderer>().sharedMaterials = materials;
                if (oldMat != doorBody.baseMaterial) Destroy(oldMat);
            }

            _doorKeyHoleDataDict.Clear();
            SetKeyHoleMaterial(DoorKeyBody1, 0);
            SetKeyHoleMaterial(DoorKeyBody2, 1);


            foreach (DoorLockData data in _lockData.GetLockDatas(DoorLockType.Event))
            {
                if (((EventData)data.Data).EventObject != null)
                {
                    ((EventData)data.Data).EventObject.Objects.Add(_doorEventHandler);
                }
            }
        }
    }

    [SerializeField]
    private bool _isDefaultActive = false;
    public bool IsDefaultActive
    {
        get => _isDefaultActive;
        set => _isDefaultActive = value;
    }

    [SerializeField]
    private bool _isActive = false;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            IsInteractable = value;
            _isActive = value;

            if (PliminaryWall == null || !_isDefaultActive)
            {
                DoorObj.SetActive(value || IsDefaultActive);
            }
            else
            {
                DoorObj.SetActive(value);
                PliminaryWall.SetActive(!value);
            }
        }
    }

    [SerializeField]
    private bool _isOpen = false;
    public bool IsOpen
    {
        get => _isOpen;
        set
        {
            _isOpen = value;
            _collider.isTrigger = value;
            _animator.SetBool("IsOpen", value);
        }
    }


    void Awake()
    {
        foreach (DoorBodyData doorBody in DoorBody)
        {
            doorBody.baseMaterial = doorBody.DoorBody.GetComponent<Renderer>().sharedMaterials[doorBody.materialIndex];
        }
        foreach (DoorKeyHoleData doorKeyHole in DoorKeyBody1)
        {
            doorKeyHole.baseMaterial = doorKeyHole.DoorKeyHole.GetComponent<Renderer>().sharedMaterials[doorKeyHole.materialIndex];
        }
        foreach (DoorKeyHoleData doorKeyHole in DoorKeyBody2)
        {
            doorKeyHole.baseMaterial = doorKeyHole.DoorKeyHole.GetComponent<Renderer>().sharedMaterials[doorKeyHole.materialIndex];
        }
    }



    void Start()
    {
        _audioSource = transform.parent.GetComponent<AudioSource>();

        Interact += InteractEvent;

        InteractionEvents.Instance.EventInteracted += EventInteracted;
        InteractionEvents.Instance.ColorButtonInteracted += ColorButtonEvent;
    }

    void OnDestroy()
    {
        if (InteractionEvents.IsExist())
        {
            InteractionEvents.Instance.ColorButtonInteracted -= ColorButtonEvent;
            InteractionEvents.Instance.EventInteracted -= EventInteracted;
        }

        foreach (DoorBodyData doorBody in DoorBody)
        {
            Material oldMat = doorBody.DoorBody.GetComponent<Renderer>().sharedMaterials[doorBody.materialIndex];
            if (oldMat != null && oldMat != doorBody.baseMaterial)
            {
                Destroy(oldMat);
            }
        }

        if (Application.isPlaying)
        {
            foreach (Material mat in _keyHoleMaterialInstances)
            {
                if (mat != null)
                {
                    Destroy(mat);
                }
            }

            foreach (Material mat in _doorBodyMaterials)
            {
                if (mat != null)
                {
                    Destroy(mat);
                }
            }
        }
    }


    public void InteractEvent()
    {
        if (IsActive)
        {
            if(LockData.IsLocked)
            {
                List<string> items = new List<string>();
                foreach (DoorLockData doorLockData in LockData)
                {
                    if (doorLockData.LockType == DoorLockType.Item && PlayerStatus.Instance.HaveCheck((string)doorLockData.LockVal))
                    {
                        items.Add((string)doorLockData.LockVal);
                    }
                    if (doorLockData.LockType == DoorLockType.Key && PlayerStatus.Instance.HaveCheck((string)doorLockData.LockVal))
                    {
                        items.Add((string)doorLockData.LockVal);
                    }
                }

                if (items.Count == 0)
                {
                    _audioSource.GetComponent<AudioSource>().PlayOneShot(_knockSound);
                }
                else
                {
                    foreach (string item in items)
                    {
                        PlayerStatus.Instance.UseItem(item);
                        LockData.SetLock(DoorLockType.Item,item);
                        LockData.SetLock(DoorLockType.Key, item);
                    }
                }
            }
            else
            {
                IsOpen = !IsOpen;
                _audioSource.PlayOneShot(IsOpen ? _openSound : _closeSound);
            }
        }
    }


    public void ResetDoor()
    {
        LockData = new DoorLockDataList();
        IsActive = false;
        IsOpen = false;
        _animator.SetTrigger("FastEvent");
    }


    public void LockEvent()
    {
        IsOpen = false;
        _animator.SetTrigger("FastEvent");
    }

    public void UnLockedEvent()
    {
    }

    public void LockStateChangedEvent(DoorLockData data)
    {
        if (data.IsLocked)
        {
            if (data.LockSound != null)
            {
                _audioSource.PlayOneShot(data.LockSound);
            }
            else
            {
                _audioSource.PlayOneShot(_lockSound);
            }
        }
        else
        {
            if (data.UnlockSound != null)
            {
                _audioSource.PlayOneShot(data.UnlockSound);
            }
            else
            {
                _audioSource.PlayOneShot(_unLockSound);
            }

            if (data.LockType == DoorLockType.Key)
            {
                if (_doorKeyHoleDataDict.TryGetValue((string)data.LockVal, out List<DoorKeyHoleData> doorKeyHoleDatas))
                {
                    foreach (DoorKeyHoleData keyHoleData in doorKeyHoleDatas)
                    {
                        if (keyHoleData.KeyHoleType == DoorKeyHoleData.DoorKeyHoleType.Hole)
                        {
                            Material[] materials = keyHoleData.DoorKeyHole.GetComponent<Renderer>().sharedMaterials;
                            materials[keyHoleData.materialIndex] = _keyHoleUnLockMaterial;
                            keyHoleData.DoorKeyHole.GetComponent<Renderer>().sharedMaterials = materials;
                        }
                    }
                }

                if (_doorController1 != null && _doorController1.LockList.SetActive(data.LockType, data.LockVal, false)) return;
                if (_doorController2 != null && _doorController2.LockList.SetActive(data.LockType, data.LockVal, false)) return;
            }
        }
    }

    public void EventInteracted(string eventName,bool state)
    {
        LockData.SetLock(DoorLockType.Event, eventName,state);
    }

    public void ColorButtonEvent(DoorLockColor color)
    {
        DoorLockColor c = LockData.GetLockColor();
        LockData.SetLock(DoorLockType.Color, LockData.GetLockColor(), c != color);
    }


    private void SetKeyHoleMaterial(List<DoorKeyHoleData> doorKeyHoleDatas, int index)
    {
        DoorLockData[] keyLocks = _lockData.GetLockDatas(DoorLockType.Key);

        if (keyLocks.Length > index)
        {
            _doorKeyHoleDataDict[(string)keyLocks[index].LockVal] = doorKeyHoleDatas;

            _keyHoleMaterialInstances[index] = Instantiate(_keyHoleMaterial);
            _keyHoleMaterialInstances[index].color = ((KeyData)keyLocks[index].Data).KeyHoleColor;

            foreach (DoorKeyHoleData keyHoleData in doorKeyHoleDatas)
            {
                keyHoleData.DoorKeyHole.SetActive(true);

                if (keyHoleData.KeyHoleType == DoorKeyHoleData.DoorKeyHoleType.Frame)
                {
                    Material[] materials = keyHoleData.DoorKeyHole.GetComponent<Renderer>().sharedMaterials;
                    materials[keyHoleData.materialIndex] = _keyHoleMaterialInstances[index];
                    keyHoleData.DoorKeyHole.GetComponent<Renderer>().sharedMaterials = materials;
                }
                else if (keyHoleData.KeyHoleType == DoorKeyHoleData.DoorKeyHoleType.Hole)
                {
                    Material[] materials = keyHoleData.DoorKeyHole.GetComponent<Renderer>().sharedMaterials;
                    materials[keyHoleData.materialIndex] = keyLocks[index].IsLocked ? _keyHoleLockMaterial : _keyHoleUnLockMaterial;
                    keyHoleData.DoorKeyHole.GetComponent<Renderer>().sharedMaterials = materials;
                }
            }
        }
        else
        {
            foreach (DoorKeyHoleData keyHoleData in doorKeyHoleDatas)
            {
                keyHoleData.DoorKeyHole.SetActive(false);
            }
        }


    }

}

