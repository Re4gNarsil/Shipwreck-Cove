﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsEngine : MonoBehaviour {

    [Header("netForce is all outside forces added together")]
    public Vector3 netForceVector = Vector3.zero;
    public Vector3 netAngularForceVector = Vector3.zero;
    [Header("this is our own vector force")]
    public Vector3 ourForceVector = Vector3.zero;       //9.8137 in the Y axis offsets gravity
    public Vector3 ourAngularForceVector = Vector3.zero;
    public Vector3 ourVelocity, ourAngularVelocity = Vector3.zero;   //this is only for visual purpurposes

    public const float gravityStrength = 6.67e-11f;   //G is a constant equal to 6.67 × 10-11 N-m2/kg2
    public bool useGravity = true, useFluidCurrents = true, accelerated, maintainAcceleration;
    public float amountSubmerged = 0;                 //used by FluidPhysics and other scripts
    [Header("these values are kept separate from our actual rigidBody")]
    [Header("just to make sure we don't make undesired changes")]
    public float myMass;
    public float myDrag, myAngularDrag;
    [Header("used for fluid physics; must be 'up', 'right', or 'forward' etc")]
    public string ourDragEnd = "backward";

    private Rigidbody rigidBody;

    void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody>();
        rigidBody.useGravity = false;
    }

    // this is called once every .02 seconds
    void FixedUpdate()
    {
        if (maintainAcceleration) { accelerated = true; }
        ApplyAllForces();
        ApplyAllAngularForces();
    }

    void ApplyAllAngularForces()
    {
        if (accelerated) {
            netAngularForceVector += ourAngularForceVector;
            accelerated = false;
        }

        float myVelocity = (rigidBody.velocity.magnitude + ((netForceVector * Time.deltaTime) / myMass).magnitude);
        if (myVelocity < 1) { myVelocity = 1; }

        print(netAngularForceVector + " " + myVelocity);
        //rigidBody.AddTorque((netAngularForceVector / myVelocity), ForceMode.Force);
        ourAngularVelocity = rigidBody.angularVelocity;
        netAngularForceVector = Vector3.zero;
    }

    void ApplyAllForces()
    {
        if (accelerated) { netForceVector += ourForceVector; }
        rigidBody.AddForce(netForceVector, ForceMode.Force);
        ourVelocity = rigidBody.velocity;
        netForceVector = Vector3.zero;
    }

    public void AddForce(Vector3 newForce) {
        netForceVector += newForce;
    }

    public void AddAngularForce(Vector3 newForce)
    {
        netAngularForceVector += newForce;
    }
}