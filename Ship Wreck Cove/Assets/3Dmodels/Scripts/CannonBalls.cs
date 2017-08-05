using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBalls : MonoBehaviour {

	public float lifeSpan = 3;
	private float gameSpeed = 1;

	private void Start()
	{
		//gameSpeed = FindObjectOfType<LevelManager>().GetGameSpeed();
		lifeSpan = lifeSpan / gameSpeed;
	}

	// Update is called once per frame
	void Update () {
		lifeSpan -= Time.deltaTime;
		if (lifeSpan <= 0) { Destroy(gameObject); }
	}

}
