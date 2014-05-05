using UnityEngine;
using System.Collections;

public class CameraView : MonoBehaviour {
	//25 speed is nice :)
	public float rotateSpeed = 0;

	void Update(){
		this.transform.RotateAround(Vector3.zero, Vector3.up, rotateSpeed * Time.deltaTime);
	}
}
