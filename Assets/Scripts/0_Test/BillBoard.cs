using UnityEngine;

public class BillBoard : MonoBehaviour {

	void Update () {
        if (Camera.main == null) return;
		Vector3 p = Camera.main.transform.position;
		p.y = transform.position.y;
		transform.LookAt (p);
	}
}
