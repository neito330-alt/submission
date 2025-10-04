using System;
using System.Collections.Generic;
using Rooms;
using UnityEngine;


[Serializable]
public class PlayerItem
{
    public string Name;
    public bool IsStackable;
    public int Count;
    public Sprite Icon;
    public Color IconColor = Color.white;

    public PlayerItem(string name="", bool isStackable=false, int count=0, Sprite icon=null, Color iconColor = default(Color))
    {
        Name = name;
        IsStackable = isStackable;
        Count = count;
        Icon = icon;
        IconColor = iconColor == default(Color) ? Color.white : iconColor;
    }
}




public class PlayerStatus : SingletonMonoBehaviour<PlayerStatus>
{
    [Serializable]
    public class ItemSet
    {
        public PlayerItem Item;
        public ItemUIController ItemUI;
    }

    [SerializeField]
    private FieldUIController _fieldUIController;

    [SerializeField]
    private AudioSource _audioSource;

    private List<ItemSet> _inventory = new List<ItemSet>();


    void Start()
    {
        RoomsManager.RoomsUpdated += ClearInventory;
    }


    public bool HaveCheck(string key)
    {
        for (int i = 0; i < _inventory.Count; i++)
        {
            if (_inventory[i].Item.Name == key && (_inventory[i].Item.IsStackable || _inventory[i].Item.Count > 0)) return true;
        }
        return false;
    }

    public PlayerItem GetItem(string key)
    {
        for (int i = 0; i < _inventory.Count; i++)
        {
            if (_inventory[i].Item.Name == key) return _inventory[i].Item;
        }
        return new PlayerItem();
    }

    public void AddItem(PlayerItem item,AudioClip sound = null)
    {
        for (int i = 0; i < _inventory.Count; i++)
        {
            if (_inventory[i].Item.Name == item.Name)
            {
                _inventory[i].Item.Count += item.Count;
                _inventory[i].ItemUI.ItemData = _inventory[i].Item;
                return;
            }
        }
        _inventory.Add(new ItemSet(){
            Item = item,
            ItemUI = _fieldUIController.AddItem(item)
        });

        if (sound != null)
        {
            _audioSource.PlayOneShot(sound);
        }
    }

    public void UseItem(string key)
    {
        for (int i = 0; i < _inventory.Count; i++)
        {
            if (_inventory[i].Item.Name == key)
            {
                _inventory[i].Item.Count--;
                _inventory[i].ItemUI.ItemData = _inventory[i].Item;
                if(_inventory[i].Item.Count <= 0)
                {
                    Destroy(_inventory[i].ItemUI.gameObject);
                    _inventory.RemoveAt(i);
                }
                return;
            }
        }
    }

    public void ClearInventory()
    {
        foreach (var itemSet in _inventory)
        {
            if (itemSet.ItemUI != null)
            {
                Destroy(itemSet.ItemUI.gameObject);
            }
        }
        _inventory.Clear();
    }
}
