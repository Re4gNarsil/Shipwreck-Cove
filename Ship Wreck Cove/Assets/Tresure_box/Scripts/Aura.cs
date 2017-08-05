using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aura : MonoBehaviour {

	private Color myColor;
	private float auraDelay = 7;
	private float delayTime = 7;
	private float auraDuration = 1;
	private float auraTime = 1;
	private bool fading = false;


	// Use this for initialization
	void Start () {
		myColor = GetComponent<Renderer>().material.color;
	}
	
	// Update is called once per frame
	void Update () {
		if (fading)
		{
			if (auraTime < 0)
			{
				fading = false;
				delayTime = auraDelay;
			}
			else
			{
				auraTime -= Time.deltaTime;
				GetComponent<Renderer>().material.color = new Color(myColor.r, myColor.g, myColor.b, (1 - Mathf.Abs((auraTime - .5f) * 2)));
				}
		} else
		{
			if (delayTime < 0)
			{
				fading = true;
				auraTime = auraDuration;
			}
			else
			{
				delayTime -= Time.deltaTime;
			}
		}
	}
}
