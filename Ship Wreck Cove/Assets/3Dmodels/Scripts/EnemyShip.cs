using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShip : MonoBehaviour {

	public float viewRange = 300;
	public float firingRange = 200;
	public float engageRange = 100;

	private GameObject target;
	private int breakNow = 0;
	private bool engaging = false;

	// Use this for initialization
	void Start () {
		target = GameObject.FindWithTag("Target");
	}

	// Update is called every .02 seconds
	void FixedUpdate()
	{
		if (engaging)
		{
			Vector3 rotationToTarget = Quaternion.LookRotation(target.transform.position - transform.position).eulerAngles;
			float angleDifference = Mathf.DeltaAngle(transform.rotation.eulerAngles.y, rotationToTarget.y);
			int direction = (angleDifference > 0) ? 1 : -1;

			float desiredAngleDifference = Mathf.DeltaAngle(transform.rotation.eulerAngles.y, rotationToTarget.y) + (90 * -direction);

			if (breakNow > 0)
			{
				float directionTwo = (Mathf.Abs(angleDifference) < 90) ? -1 : 1;
				GetComponent<GroundCharacter>().MoveForwards(directionTwo);
				breakNow--;
			}

			if (Mathf.Abs(desiredAngleDifference) > 2){	GetComponent<GroundCharacter>().MoveSideways(Mathf.Clamp(desiredAngleDifference, -1, 1)); }
			if (Mathf.Abs(desiredAngleDifference) < 2)
			{
				if (direction > 0) { GetComponent<CharacterMultiFiring>().FireOtherShot(); }
				else { GetComponent<CharacterMultiFiring>().FireShot(); }
			}
			if ((transform.position - target.transform.position).magnitude > firingRange) {	engaging = false; }
		}
		else
		{
			if ((transform.position - target.transform.position).magnitude < viewRange)
			{

				Vector3 rotationToTarget = Quaternion.LookRotation(target.transform.position - transform.position).eulerAngles;
				float angleDifference = Mathf.DeltaAngle(transform.rotation.eulerAngles.y, rotationToTarget.y);
				int direction = (angleDifference > 0) ? 1 : -1;

				if (Mathf.Abs(angleDifference) > 2) { GetComponent<GroundCharacter>().MoveSideways(direction); }
				if (Mathf.Abs(angleDifference) < 2)
				{
					float directionTwo = (Mathf.Abs(angleDifference) > 90) ? -1 : 1;
					GetComponent<GroundCharacter>().MoveForwards(directionTwo);
					if (breakNow < 45) { breakNow++; }
				}
			}
			if ((transform.position - target.transform.position).magnitude < engageRange) { engaging = true; }
		}
	}
}
