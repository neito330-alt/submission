using UnityEngine;


public class InteractiveKey : InteractiveObject
{
    public AudioClip sound;

    public PlayerItem item;

    // public override void Interact()
    // {
    //     PlayerStatus.Instance.AddItem(item, sound);
    //     InteractionEvents.Instance.OnKeyInteracted();
    //     transform.gameObject.SetActive(false);
    // }
}
