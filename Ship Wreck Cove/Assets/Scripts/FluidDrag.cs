using UnityEngine;
using System.Collections;

public class FluidDrag : MonoBehaviour {

    [Range(1, 2)]
    public float velocityExponent = 1.5f;
    [Header("thin is for stuff like air, thick for things like water")]
    public float dragConstantThin = .05f;
    public float dragConstantThick = 1;

    private PhysicsEngine physicsEngine;

    // Use this for initialization
    void Start()
    {
        physicsEngine = GetComponent<PhysicsEngine>();
    }

    // Update is called once every .02 seconds
    void FixedUpdate () {
        //first we take care of velocity, and then angular velocity

        Vector3 velocityVector = ((physicsEngine.netForceVector * Time.deltaTime) / physicsEngine.myMass);
        float mySpeed = velocityVector.magnitude;
        Vector3 dragVector = (-velocityVector.normalized * CalculateDrag(mySpeed, physicsEngine.myDrag));
        physicsEngine.AddForce(dragVector);

        Vector3 angularVelocityVector = ((physicsEngine.netAngularForceVector * Time.deltaTime) / physicsEngine.myMass);
        float myAngularSpeed = angularVelocityVector.magnitude;
        //print(myAngularSpeed);
        Vector3 angularDragVector = (-angularVelocityVector.normalized * (CalculateDrag((myAngularSpeed), physicsEngine.myAngularDrag) + CalculateDrag(mySpeed, physicsEngine.myDrag)));
        //print(CalculateDrag(myAngularSpeed, physicsEngine.myAngularDrag) + " " + CalculateDrag(mySpeed, physicsEngine.myDrag));
        physicsEngine.AddAngularForce(angularDragVector);
    }
    
    float CalculateDrag(float mySpeed, float myDrag)
    {
        float totalDrag = (dragConstantThin * (1 - physicsEngine.amountSubmerged) * (1 + myDrag) * Mathf.Pow(mySpeed, velocityExponent));
        return (totalDrag + (dragConstantThick * physicsEngine.amountSubmerged * (1 + myDrag) * Mathf.Pow(mySpeed, velocityExponent)));
    }
}
