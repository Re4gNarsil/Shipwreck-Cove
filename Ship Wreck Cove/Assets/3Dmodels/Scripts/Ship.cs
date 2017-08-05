using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {

	public GameObject audioChild;

	private AudioSource childAudioSource;
	private Animator myAnimator;

	void Start()
	{
		childAudioSource = transform.Find("pirateShip").GetComponent<AudioSource>();
		myAnimator = GetComponentInChildren<Animator>();
	} 

	private void OnTriggerEnter(Collider other)
	{
		if (childAudioSource)
		{
			if (other.gameObject.tag == "Terrain" || other.gameObject.tag == "Barrier")
			{
				Rigidbody myRigidBody = GetComponent<Rigidbody>();
				Vector3 oppositeForce = -(myRigidBody.velocity * myRigidBody.mass);
				myRigidBody.AddForce(oppositeForce, ForceMode.Impulse);
				if (other.gameObject.tag == "Terrain")
				{
					GetComponent<HealthBar>().DealDamage(30);
					CreateAudioChild();
				}
			}
			else if (other.gameObject.tag == "Rock" || other.gameObject.tag == "Ship" || other.gameObject.tag == "Player")
			{
				GetComponent<HealthBar>().DealDamage(20);
				CreateAudioChild();
			}
			else if (other.gameObject.tag == "Weapon")
			{
				GetComponent<HealthBar>().DealDamage(10);
				CreateAudioChild();
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Terrain" || other.gameObject.tag == "Barrier")
		{
			Rigidbody myRigidBody = GetComponent<Rigidbody>();
			Vector3 oppositeForce = -(myRigidBody.velocity * myRigidBody.mass);
			myRigidBody.AddForce(oppositeForce, ForceMode.Impulse);
		}
	}

	public void Sinking()
	{
		childAudioSource.Play();
		myAnimator.SetBool("Sinking", true);
		GetComponent<Rigidbody>().velocity = Vector3.zero;

		if (GetComponent<EnemyShip>()) { Destroy(GetComponent<EnemyShip>()); }
		Invoke("DestroyPlayer", 5);
	}

	void DestroyPlayer()
	{
		FindObjectOfType<LevelManager>().ReduceEnemyCount(1);
		Destroy(gameObject);
	}

	void CreateAudioChild()
	{
		GameObject newAudioChild = GameObject.Instantiate(audioChild, gameObject.transform);
		AudioSource audioSource = newAudioChild.GetComponent<AudioSource>();
		audioSource.time = 3.2f;
		audioSource.Play();
		Destroy(newAudioChild, 4.2f);
	}
}
