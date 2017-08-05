using UnityEngine;
using System.Collections;

public class DayNight : MonoBehaviour {

	//private Sky sky;

    private LensFlare lensFlare;
	private Vector3 centerPosition; //center of the world we make
	private bool day;
	private float maxDegrees = 360, secsPerDay = 88400, speed, degPerSecond;
	private int timeScale = 36;

	// Use this for initialization
	void Start () {
        lensFlare = GetComponent<LensFlare>();
		//sky = FindObjectOfType<Sky>();
		centerPosition = new Vector3(500f, 0f, 600f);
		degPerSecond = ((maxDegrees / secsPerDay) * timeScale * speed); //full circle / length of a day * how fast we speed things up
	}

	// Update is called once per frame
	void Update () {
        transform.RotateAround(centerPosition, Vector3.left, (degPerSecond * Time.deltaTime));
	}

	public void SetSpeed(float newSpeed)
	{
		speed = newSpeed;
	}
}