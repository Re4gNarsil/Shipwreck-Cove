using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class FlightCharacter : MonoBehaviour {

    [Header("Check with Input Manager if you want to change either of these")]
    public string firstXAxis = "Horizontal";
    public string firstYAxis = "Vertical";
    [Header("Controls Forward, and Backward, speed; needs to be set up in Input Manager")]
    [Header("Speed, Roll, and/or Reset can be changed to an input already in Input Manager")]
    public string speed = "Speed";
    [Header("Controls whether you roll left/right as opposed to turning left/right;")]
	[Header("needs to be set up in Input Manager")]
    public string Roll = "Roll";
    public bool canReverse = true, canTurnInPlace = true;
    [Header("These are not hooked up to anything currently")]
    public float ourHealth = 100;
    public float weaponDamage = 10, objectDamage = 50, healRegen = 30, amountSubmerged = 0, waterPressure = 10f, ourAirSpeed = 50, ourTurnSpeed = 10;

    private float ourMass, ourDrag;
    private Rigidbody rigidBody;
    private Collider myCollider;
    private WhereIllBe whereIllBe;

    // Use this for initialization
    void Start()
    {
        gameObject.tag = "Player";
        rigidBody = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        whereIllBe = GetComponent<WhereIllBe>();
        if (whereIllBe && myCollider) { myCollider.isTrigger = true; }
        ourMass = rigidBody.mass;
        ourDrag = rigidBody.drag;
    }

    // Update is called once every .02 seconds
    void FixedUpdate() {

        //'Horizontal' and 'Vertical' controls operate control rotation from side to side and up and down, while 
        //separate roll and speed inputs control the remaining flight controls; the Mouse X/Y controls operate
        //the players camera.

        float ourValueX = CrossPlatformInputManager.GetAxisRaw(firstXAxis);
        float ourValueY = CrossPlatformInputManager.GetAxisRaw(firstYAxis);
        float ourSpeed  = CrossPlatformInputManager.GetAxisRaw(speed);

        //check to see what conditions need to be met in order to rotate our gameObject, and then see if they're fulfilled

        if ((canTurnInPlace == true) && (ourValueX != 0) || (ourValueY != 0)) { RotateOurShip(ourValueX, ourValueY); }
        else if (((ourValueX != 0) || (ourValueY != 0)) && ourSpeed != 0)
        {
            if (canReverse == true) { RotateOurShip(ourValueX, ourValueY); }
            else if (ourSpeed > 0) { RotateOurShip(ourValueX, ourValueY); }
        }

        //only apply a force if we are actively moving; otherwise we just 'glide' along

        if (ourSpeed != 0)
        {
            Vector3 ourForce = ((transform.forward * ourSpeed * ourMass * ourAirSpeed) / ((1 * (1 - amountSubmerged)) + (amountSubmerged * waterPressure)));
            if (canReverse)
            {
                rigidBody.AddForce(ourForce, ForceMode.Force);
            } else {
                if (ourSpeed > 0) { rigidBody.AddForce(ourForce, ForceMode.Force); }
            }
        }
    }

    //rotate our ship around or flip over depending on if a certain button is being held down

    void RotateOurShip(float valueX, float valueY) {
        Vector3 newRotation = transform.rotation.eulerAngles;
        float waterPressure = 1;                            //1 is normal, ideal conditions

        newRotation.x -= ((valueY * Time.deltaTime * ourTurnSpeed) / ((1 * (1 - amountSubmerged)) + (amountSubmerged * waterPressure)));
        if (CrossPlatformInputManager.GetButton(Roll)) { newRotation.z -= ((valueX * Time.deltaTime * ourTurnSpeed) / ((1 * (1 - amountSubmerged)) + (amountSubmerged * waterPressure))); }
        else              { newRotation.y += ((valueX * Time.deltaTime * ourTurnSpeed) / ((1 * (1 - amountSubmerged)) + (amountSubmerged * waterPressure))); }
        transform.rotation = Quaternion.Euler(newRotation);
    }

    //all publicly exposed methods are here

    public Vector2 GetMassDrag()
    {
        return new Vector2(ourMass, ourDrag);
    }

    public void ChangeOurSpeed(float newSpeed)
    {
        ourAirSpeed = newSpeed;
    }

    public void ChangeOurTurnSpeed(float newSpeed)
    {
        ourTurnSpeed = newSpeed;
    }
}
