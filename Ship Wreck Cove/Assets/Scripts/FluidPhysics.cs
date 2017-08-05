using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FluidPhysics : MonoBehaviour {

    [Header("This manages up to two 'fluids' in the scene;")]
    [Header("they are labeled thick and thin for simplicity")]
    [Header("Maximum force plus how much it can change by per frame")]
    public float maxThinFluidForce = 6;
    public float maxThinFluidChange = 1;
    public float maxThickFluidForce = 12;
    public float maxThickFluidChange = .5f;

    private float thinFluidEulerAngle, thickFluidEulerAngle, thinFluidForce, thickFluidForce;
    private List<PhysicsEngine> physicsEngineArray;
    private Vector3 totalThinFluidForce, totalThickFluidForce;

    // Use this for initialization
    void Start()
    {
        physicsEngineArray = FindObjectsOfType<PhysicsEngine>().ToList();

        thinFluidEulerAngle = Random.Range(0, 359);
        thinFluidForce = Random.Range((maxThinFluidChange), (maxThinFluidForce));

        thickFluidEulerAngle = Random.Range(0, 359);
        thickFluidForce = Random.Range((maxThickFluidChange), (maxThickFluidForce));
    }

    // Update is called once per frame
    void FixedUpdate () {

        thinFluidEulerAngle = ChangeFluid(thinFluidEulerAngle, thinFluidForce, maxThinFluidChange, maxThinFluidForce).x;
        thinFluidForce = ChangeFluid(thinFluidEulerAngle, thinFluidForce, maxThinFluidChange, maxThickFluidForce).y;
        thickFluidEulerAngle = ChangeFluid(thickFluidEulerAngle, thickFluidForce, maxThickFluidChange, maxThickFluidForce).x;
        thickFluidForce = ChangeFluid(thickFluidEulerAngle, thickFluidForce, maxThickFluidChange, maxThickFluidForce).y;

        foreach (PhysicsEngine physicsEngineA in physicsEngineArray)
        {
            foreach (PhysicsEngine physicsEngineB in physicsEngineArray)
            {
                if ((physicsEngineA.gameObject != physicsEngineB.gameObject) && (physicsEngineA.useFluidCurrents))
                {
                    Vector3 fluidVector = ConvertToVector(new Vector3(0, thinFluidEulerAngle, 0));
                    Vector3 objectsDragDirection = DetermineDirection(physicsEngineA);
                    Vector3 objectsVector = ConvertToVector(objectsDragDirection);
                    Vector3 directionalVector = (fluidVector - objectsVector);
                    //physicsEngineA.AddForce(fluidVector * thinFluidForce * (1 + physicsEngineA.myDrag) * (1 - physicsEngineA.amountSubmerged));
                    //physicsEngineA.AddForce(fluidVector * thickFluidForce * (1 + physicsEngineA.myDrag) * physicsEngineA.amountSubmerged);


                    //Vector3 objectsDragVector = objectsDragDirection.normalized;
                    
                    Vector3 directionVector = normalizeVector(new Vector3(0, thinFluidEulerAngle, 0) - objectsDragDirection);
                    float ourFriction = physicsEngineA.gameObject.GetComponent<Collider>().sharedMaterial.dynamicFriction;

                    //if (directionalVector.magnitude < .5f) { } // do pretty much nothing

                    //print(directionalVector + " " + totalThinFluidForce.y);
                    if (((directionalVector.y < 0) && (totalThinFluidForce.y < 0)) || ((directionalVector.y > 0) && (totalThinFluidForce.y > 0)))
                    {
                        print("hey");
                        physicsEngineA.AddAngularForce(-totalThinFluidForce);
                        totalThinFluidForce = Vector3.zero;
                    } else
                    {
                        physicsEngineA.AddAngularForce(directionVector * thinFluidForce * ourFriction * (1 + physicsEngineA.myDrag) * (1 - physicsEngineA.amountSubmerged));
                        totalThinFluidForce += (directionVector * thinFluidForce * ourFriction * (1 + physicsEngineA.myDrag) * (1 - physicsEngineA.amountSubmerged));
                    }


                    //print(directionVector);

                    physicsEngineA.AddAngularForce(directionVector * thickFluidForce * ourFriction * (1 + physicsEngineA.myDrag) * physicsEngineA.amountSubmerged);
                    totalThickFluidForce += (directionVector * thickFluidForce * ourFriction * (1 + physicsEngineA.myDrag) * physicsEngineA.amountSubmerged);
                }
            }
        }
    }

    Vector3 DetermineDirection(PhysicsEngine physicsEngine)
    {
        string dragEnd = physicsEngine.ourDragEnd;
        Vector3 dragDirection = physicsEngine.transform.rotation.eulerAngles;

        if      (dragEnd == "backward")  { dragDirection += new Vector3(0, 180, 0); }
        else if (dragEnd == "left")      { dragDirection += new Vector3(0,  90, 0); }
        else if (dragEnd == "right")     { dragDirection += new Vector3(0, -90, 0); }
        else if (dragEnd == "up")        { dragDirection += new Vector3(90,  0, 0); }
        else if (dragEnd == "down")      { dragDirection += new Vector3(-90, 0, 0); }
        else { Debug.LogWarning("Not a valid direction"); }

        dragDirection.x = Mathf.DeltaAngle(dragDirection.x, 0);
        dragDirection.y = Mathf.DeltaAngle(dragDirection.y, 0);
        dragDirection.z = Mathf.DeltaAngle(dragDirection.z, 0);

        return dragDirection;
    }

    Vector3 ConvertToVector(Vector3 ourAngle)
    {
        Vector3 newVector = Vector3.zero;

        ourAngle.x = Mathf.DeltaAngle(ourAngle.x, 0);
        ourAngle.y = Mathf.DeltaAngle(ourAngle.y, 0);
        ourAngle.z = Mathf.DeltaAngle(ourAngle.z, 0);

        if ((ourAngle.y > 0) && (ourAngle.y <= 90)) { newVector.x = (Mathf.Sqrt((ourAngle.y / 90) * 100) / 10); }
        else if ((ourAngle.y > 90) && (ourAngle.y <= 180)) { newVector.x = (Mathf.Sqrt(((ourAngle.y - 90) / 90) * 100) / 10); }
        else if ((ourAngle.y < 0) && (ourAngle.y >= -90)) { newVector.x = -(Mathf.Sqrt((-ourAngle.y / 90) * 100) / 10); }
        else if ((ourAngle.y < -90) && (ourAngle.y >= -180)) { newVector.x = -(Mathf.Sqrt(((-ourAngle.y - 90) / 90) * 100) / 10); }

        newVector.z = (Mathf.Sqrt(Mathf.Abs(10000 - ((newVector.x * 100) * (newVector.x * 100)))) / 100);
        if (((ourAngle.y > 90) && (ourAngle.y <= 180)) || ((ourAngle.y < -90) && (ourAngle.y >= -180))) { ourAngle.z = -ourAngle.z; }

        return newVector;
    }

    Vector3 normalizeVector(Vector3 ourAngle)
    {
        Vector3 newVector = Vector3.zero;

        ourAngle.x = Mathf.DeltaAngle(ourAngle.x, 0);
        ourAngle.y = Mathf.DeltaAngle(ourAngle.y, 0);
        ourAngle.z = Mathf.DeltaAngle(ourAngle.z, 0);

        newVector.x = (ourAngle.x / 180);
        newVector.y = (ourAngle.y / 180);
        newVector.z = (ourAngle.z / 180);
        if (newVector.magnitude > 1) { newVector = newVector.normalized; }
        //print(newVector.magnitude);
        return newVector;
    }

    Vector2 ChangeFluid(float fluidEulerAngle, float fluidForce, float fluidChange, float fluidMax)
    {
        float changeDirection = (Random.Range(-60, 60) * Time.deltaTime);
        float changeForce = (Random.Range(-fluidChange, fluidChange) * Time.deltaTime);
        fluidEulerAngle += changeDirection;

        if (fluidEulerAngle > 180) { fluidEulerAngle -= 360; }
        else if (fluidEulerAngle < -180) { fluidEulerAngle += 360; }

        fluidForce += changeForce;
        if (fluidForce < fluidChange) { fluidForce = fluidChange; }
        if (fluidForce > fluidMax) { fluidForce = fluidMax; }
        return new Vector2(fluidEulerAngle, fluidForce);
    }

    public void AddNewObject(PhysicsEngine engine)
    {
        physicsEngineArray.Add(engine);
    }

    public void RemoveObject(PhysicsEngine engine)
    {
        physicsEngineArray.Remove(engine);
    }
}
