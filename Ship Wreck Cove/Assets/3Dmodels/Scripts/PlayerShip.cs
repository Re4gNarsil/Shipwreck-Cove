using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour {

	private void OnTriggerEnter(Collider other)
	{
	if (other.gameObject.tag == "Convoy")
		{
			FindObjectOfType<LevelManager>().StartLevel(true);
		}
	}
}
