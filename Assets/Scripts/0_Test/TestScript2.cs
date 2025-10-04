using UnityEngine;

public class TestScript2 : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Collider[] colliders = Physics.OverlapSphere(new Vector3(0,1,8), 1f);
            Debug.Log(colliders.Length);
        }
    }

}
