using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour {

	[Header("Rises from 0 to 6")]
	public float retrieveSpeed = 2;
	public float increasedSize = 2;

	private float originalDepth, gameSpeed;
	private Vector3 originalScale;
	private bool retrieved;

	// Use this for initialization
	void Start () {
		originalDepth = transform.position.y;
		originalScale = transform.localScale;
		gameSpeed = FindObjectOfType<LevelManager>().GetGameSpeed();
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			Vector3 newPosition = transform.position;
			newPosition.y += retrieveSpeed * Time.deltaTime;
			transform.position = newPosition;

			Vector3 newScale = transform.localScale;
			newScale += Vector3.one * (increasedSize * retrieveSpeed * gameSpeed / 6) * Time.deltaTime;
			transform.localScale = newScale;
			GetComponent<Rigidbody>().useGravity = false;

			if (newPosition.y >= 6  && !retrieved)
			{
				retrieved = true;
				FindObjectOfType<LevelManager>().RetrieveChest();
				Destroy(gameObject);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			Vector3 newPosition = transform.position;
			newPosition.y = originalDepth;
			transform.position = newPosition;
			transform.localScale = originalScale;
			GetComponent<Rigidbody>().useGravity = true;
		}
	}
}
