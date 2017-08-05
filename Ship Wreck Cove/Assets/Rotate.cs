using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

	private float speed = 10;

	private float gameSpeed = 1;

	public void SetGameSpeed(float newSpeed)
	{
		gameSpeed = newSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 newRotation = transform.rotation.eulerAngles;
		newRotation.y += speed * Time.deltaTime * gameSpeed;
		transform.rotation = Quaternion.Euler(newRotation);
	}
}
