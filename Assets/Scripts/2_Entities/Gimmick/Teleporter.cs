using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public static List<Teleporter> Teleporters = new List<Teleporter>();


    [SerializeField]
    private string _id;
    public string Id
    {
        get => _id;
        set => _id = value;
    }

    [SerializeField]
    private bool _isIn = false;
    public bool IsIn
    {
        get => _isIn;
        set => _isIn = value;
    }

    [SerializeField]
    private bool _isOut = false;
    public bool IsOut
    {
        get => _isOut;
        set => _isOut = value;
    }




    [SerializeField]
    private bool _isActive = true;
    public bool IsActive
    {
        get => _isActive;
        set => _isActive = value;
    }

    private void Awake()
    {
        Teleporters.Add(this);
    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("Teleporter OnTriggerEnter: " + other.gameObject.name);

        if (!IsActive || !IsIn) return;

        if (other.gameObject.tag == "Player")
        {
            IsActive = false;
            foreach (var teleporter in Teleporters)
            {
                if (teleporter == this) continue;

                if (teleporter.Id != Id) continue;

                if (teleporter.IsOut && teleporter.IsActive)
                {
                    teleporter.IsActive = false;
                    GetComponent<AudioSource>().Play();
                    GameManager.playerManager.SetPosition(teleporter.transform.position+new Vector3(0, 0.3f, 0));
                    break;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            IsActive = true;
        }
    }
}