using System.Collections.Generic;
using Rooms.Auto;
using UnityEngine;

namespace Rooms.RoomSystem
{
    public class GimmickParentController : MonoBehaviour
    {
        public List<GimmickController> gimmicks;

#if UNITY_EDITOR
        public void SetUp(RoomSetController roomSetController)
        {
            gimmicks = new List<GimmickController>(GetComponentsInChildren<GimmickController>());
            foreach (var gimmick in gimmicks)
            {
                if (gimmick == null) continue; // Skip if the gimmick is null
                gimmick.SetUp(roomSetController);
            }
            foreach (var gimmick in gimmicks)
            {
                roomSetController._decalDataListInput.list.Add(gimmick.decalData);
            }
        }
#endif

        public void ResetRoom()
        {
            foreach (var gimmick in gimmicks)
            {
                if (gimmick == null) continue; // Skip if the gimmick is null
                gimmick.ResetGimmick();
            }
        }

        public void RefreshRoom()
        {
            foreach (var gimmick in gimmicks)
            {
                if (gimmick == null) continue; // Skip if the gimmick is null
                gimmick.RefreshGimmick();
            }
        }
    }
}
