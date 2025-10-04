using Rooms.Auto;
using Rooms.PanelSystem;
using UnityEngine;


namespace Rooms.RoomSystem
{
    public class GimmickController : MonoBehaviour
    {
        [SerializeField] public DecalData decalData;

        public virtual void ResetGimmick()
        {
            // Override this method in derived classes to reset specific gimmick states.
        }

        public virtual void RefreshGimmick()
        {
            // Override this method in derived classes to refresh specific gimmick states.
        }

        public virtual void SetUp(RoomSetController roomSetController)
        {
            // Override this method in derived classes to handle interactions with the gimmick.
        }
    }
}
