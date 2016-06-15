using UnityEngine;
using System.Collections.Generic;

public class CullingGroupHandler : MonoBehaviour {

    public static CullingGroupHandler singleton;

    CullingGroup cullGroup;
    BoundingSphere[] spheres;
    public Dictionary<int, CullingGroupBehavior> cullGroupBehaveDict;
    //private Queue<int> clearedSpaces;
    //public List<CullingGroupBehavior> cullGroupCompList;
    private List<BoundingSphere> boundingSphereList;
    private bool _initialized = false;

    public int maxNumBoundingSpheres = 5000;

    public Camera targetCam;
    public Transform distanceBandingReferenceTransform;
    public float[] distanceBands;

    //public int maxDistance = 40;

    public bool hasCullGroup {
        get {
            if (cullGroup != null) { return true; }
            else { return false; }
        }
    }

    void Awake() {
        if (singleton == null) {
            singleton = this;
        }

        //preallocate dictionary for memory efficientcy
        cullGroupBehaveDict = new Dictionary<int, CullingGroupBehavior>(maxNumBoundingSpheres);
        //clearedSpaces = new Queue<int>();

        boundingSphereList = new List<BoundingSphere>();

        
    }
	
	void Update () {
        if (boundingSphereList != null) {

            SetupCullingGroup(boundingSphereList);

            boundingSphereList = null;
            _initialized = true;
        }

        //Debug.Log(spheres.Length);
        //Debug.LogFormat("{0} elements in dictionary", cullGroupCompDict.Count);

        //CheckVisiblity();

	}

    /// <summary>
    /// Bootstrap for setting up Culling Group
    /// </summary>
    /// <param name="boundingSpheres">Initial Bounding Spheres for the Culling Group</param>
    private void SetupCullingGroup(List<BoundingSphere> boundingSpheres) {

        cullGroup = new CullingGroup();

        //setup cam if none was assigned
        if (targetCam != null) {
            cullGroup.targetCamera = targetCam;
        }
        else {
            //cullGroup.targetCamera = Camera.main;
            Debug.LogError("Culling Group Handler: No Assigned Target Camera. Please assign a Camera for this Culling Group", this.gameObject);
        }

        //set reference transform for distance banding
        if (distanceBandingReferenceTransform != null) {
            cullGroup.SetDistanceReferencePoint(distanceBandingReferenceTransform);
        }
        //FALLBACK - Use transform of targetCam if no reference point was assigned
        else {
            if (targetCam != null) {
                distanceBandingReferenceTransform = targetCam.transform;
                cullGroup.SetDistanceReferencePoint(distanceBandingReferenceTransform);
            }
            else {
                Debug.LogError("Culling Group Handler: Fallback to targetCam.transform as distanceBandingReferenceTransform failed. Please assign a Transform for the refrence point, or just assign a Camera to targetCam if you want it to be used for Distance Banding", this.gameObject);
            }
        }


        //need to preallocate a lot of room in array b/c resizing is expensive
        spheres = new BoundingSphere[maxNumBoundingSpheres];

        //add bounding spheres
        for (int i = 0; i < boundingSpheres.Count; ++i) {
            spheres[i] = boundingSpheres[i];
        }

        //assign bounding spheres (internally stores spheres as a pointer)
        cullGroup.SetBoundingSpheres(spheres);
        cullGroup.SetBoundingSphereCount(boundingSpheres.Count);

        //assign the distance bands
        cullGroup.SetBoundingDistances(distanceBands);

        //assign state change delegate
        cullGroup.onStateChanged = StateChangedMethod;
    }

    /// <summary>
    /// Update the Culling Group to the most recent representation of Bounding Spheres
    /// </summary>
    public void RefreshCullGroup() {
        if (cullGroup == null) { return; }

        cullGroup.SetBoundingSpheres(spheres);
        cullGroup.SetBoundingSphereCount(cullGroupBehaveDict.Count);
    }
    /*
    private void CheckVisiblity() {
        if (cullGroup == null) { return; }

        int[] visibleAndDistanceIndices = new int[arrayHardLimit];
        int[] visibleIndices = new int[arrayHardLimit];
        int[] distanceIndices = new int[arrayHardLimit];
        int numResults = 0;

        //visible & distance
        //numResults = cullGroup.QueryIndices(true, maxDistance, visibleIndices, 0);

        //visible
        numResults = cullGroup.QueryIndices(true, visibleIndices, 0);

        //distance
        //numResults = cullGroup.QueryIndices(maxDistance, distanceIndices, 0);

        //if (numResults == null) { Debug.Log("oh shit"); }

        //Debug.LogFormat("{0} result(s) found", numResults);
    }
    */

    /// <summary>
    /// Add a Bounding Sphere to this Culling Group
    /// </summary>
    /// <param name="position">Position of the Bounding Sphere to add in World Space</param>
    /// <param name="radius">Radius of the Bounding Sphere to add</param>
    /// <returns>Index of the sphere that was just added. Returns -1 if at Max Number of allowed Bounding Spheres</returns>
    public int AddBoundingSphere(Vector3 position, float radius) {
        /*
        if (spheres. == maxNumBoundingSpheres) {
            Debug.LogError("Culling Group Handler: Reached maximum number of allotted Bounding Spheres. More can be added if maxNumBoundingSpheres is increased. Returning -1", this);

            return -1;
        }
        */

        //if we're still initializing or we're initialized and have no recorded free spaces
        if (_initialized == false) {
            boundingSphereList.Add(new BoundingSphere(position, radius));
            return (boundingSphereList.Count - 1);
        }
        //else {//get a free space from the queue

        //int index = clearedSpaces.Count == 0 ? cullGroupBehaveDict.Count : clearedSpaces.Dequeue();

        int index = cullGroupBehaveDict.Count;

        if (cullGroupBehaveDict.Count == 0) {
            index = 0;
        }

        spheres[index] = new BoundingSphere(position, radius);
        //RefreshCullGroup();
        return index;
        //}

    }

    /// <summary>
    /// Record a reference to the Culling Group Behaviour so this Culling Group can have proper callbacks to the User-Defined script
    /// </summary>
    /// <param name="index">Index of Bounding Sphere in this Culling Group's Bounding Sphere Array</param>
    /// <param name="comp">Culling Group Behaviour linked to this Bounding Sphere</param>
    public void AddComponentRef(int index, CullingGroupBehavior comp) {
        cullGroupBehaveDict.Add(index, comp);
    }

    /// <summary>
    /// Update the Location and Radius of the Bounding Sphere at the specified Index
    /// </summary>
    /// <param name="index">Index of Bounding Sphere in this Culling Group's Bounding Sphere Array</param>
    /// <param name="position">New Position of the Bounding Sphere in World Space</param>
    /// <param name="radius">New Radius of the Bounding Sphere</param>
    public void UpdateBoundingSphereAtIndex(int index, Vector3 position, float radius) {
        if (cullGroup == null) { return; }
        
        spheres[index].position = position;
        spheres[index].radius = radius;
    }

    /// <summary>
    /// Returns the Bounding Sphere at the specified Index
    /// </summary>
    /// <param name="index">WARNING - Segment Faults are not guarded against</param>
    /// <returns></returns>
    public BoundingSphere GetSphereAtIndex(int index) { 
        return spheres[index];
    }


    /// <summary>
    /// Removes Bounding Sphere at specified Index and records that array position as available
    /// </summary>
    /// <param name="index">Index of Bounding Sphere to remove</param>
    public void RemoveSphere(int index) {

        //only do this if the index exists
        if (!cullGroupBehaveDict.ContainsKey(index)) { return; }

        //clearedSpaces.Enqueue(index);//record that this space is open if we add a new Bounding Sphere later
        //cullGroupBehaveDict.Remove(index);

        //remove the desired Bounding Sphere by moving the last Bounding Sphere on top of it
        cullGroup.EraseSwapBack(index);


        //update the information on the Culling Group Behaviour to preserve callback functionality
        cullGroupBehaveDict[index] = cullGroupBehaveDict[cullGroupBehaveDict.Count - 1];
        cullGroupBehaveDict[index].boundingSphereIndex = index;


        //remove the old Culling Group Behaviour from the callback Dictionary
        //cullGroupBehaveDict[cullGroupBehaveDict.Count - 1] = null;
        cullGroupBehaveDict.Remove(cullGroupBehaveDict.Count - 1);
    }

    /// <summary>
    /// Function that provides callbacks to Culling Group Behaviours for Visibility and Distance Banding
    /// </summary>
    /// <param name="evt">Event that information gets pulled out of</param>
    private void StateChangedMethod(CullingGroupEvent evt) {
        if (evt.hasBecomeVisible) {
            //Debug.LogFormat("Spherer {0} has become visible!", evt.index);
            cullGroupBehaveDict[evt.index].OnVisible();
        }

        if (evt.hasBecomeInvisible) {
            cullGroupBehaveDict[evt.index].OnInvisible();
        }

        //***TODO: IMPLEMENT DISTANCE CHECKS***
        if (evt.currentDistance != evt.previousDistance) {
            cullGroupBehaveDict[evt.index].OnDistanceChange(evt.currentDistance);
        }

        //***TODO: IMPLEMENT VISIBILITY AND DISTNACE CHECKS*** (needed b/c QueryIndices can do both at once, it's probalby more optimized
    }

    /// <summary>
    /// Auto-Cleanup the Culling Group when GameObject it lives on is Destroyed
    /// </summary>
    void OnDestroy() {
        //need to cleanup to avoid possible crashes
        cullGroup.Dispose();
        cullGroup = null;
    }
}
