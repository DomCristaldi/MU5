using UnityEngine;

public class CullingGroupBehavior : MonoBehaviour {

    [ViewOnly]
    public int boundingSphereIndex;

    public float boundingSphereRad = 0.5f;

    //Retrieve a reference to the Bounding Sphere stored in the Culling Group that this Behaviour references
    public BoundingSphere boundSphere {
        get {
            if (CullingGroupHandler.singleton.hasCullGroup) {
                return CullingGroupHandler.singleton.GetSphereAtIndex(boundingSphereIndex);
            }
            else {
                Debug.LogWarning("No Culling Group initialized this frame. Returning new Bounding Sphere to prevent crash");
                return new BoundingSphere();
            }
        }
    }

    protected virtual void Awake() {

    }

    protected virtual void Start() {
        AddToHandler();
	}

    protected virtual void Update() {
        UpdateBoundingSphere();

	}

    /// <summary>
    /// Creates a Bounding Sphere for this GameObject in the Culling Group Handler and binds callbacks implemented in this script
    /// </summary>
    protected void AddToHandler() {

        //Make sure a Culling Group exists
        if (CullingGroupHandler.singleton == null) {
            Debug.LogError("No Culling Group Handler in Scene. Please add a single Culling Group Handler to a GameObject, preferably one that acts as a manager");
            return;
        }

        //Create a Bounding Sphere for this GameObject
        boundingSphereIndex = CullingGroupHandler.singleton.AddBoundingSphere(transform.position, boundingSphereRad);

        //Bind this Script's callbacks into the Culling Group Handler
        CullingGroupHandler.singleton.AddComponentRef(boundingSphereIndex, this);

        //Update the Culling Group so it knows about the Bounding Sphere for this GameObject
        CullingGroupHandler.singleton.RefreshCullGroup();
    }

    /// <summary>
    /// Updates the Position and Radius of the Bounding Sphere. Should be called every frame
    /// </summary>
    protected void UpdateBoundingSphere() {
        CullingGroupHandler.singleton.UpdateBoundingSphereAtIndex(boundingSphereIndex,
                                                                  transform.position,
                                                                  boundingSphereRad);
    }

    /// <summary>
    /// Define what happens when the Bounding Sphere enters the View Frustrum
    /// </summary>
    public virtual void OnVisible() {
        //Debug.LogFormat("OnVis {0}", boundingSphereIndex);

    }

    /// <summary>
    /// Define what happens when the Bounding Sphere leaves the View Frustrum
    /// </summary>
    public virtual void OnInvisible() {
        //Debug.LogFormat("OnInvis {0}", boundingSphereIndex);

    }

    public virtual void OnDistanceChange(int newDistanceBand) {

    }

    //Cleanup when Destroyed
    void OnDestroy() {
        if (CullingGroupHandler.singleton != null) {
            CullingGroupHandler.singleton.RemoveSphere(boundingSphereIndex);
        }
    }

    //Visual Representation of Bounding Sphere in the Scene view
    void OnDrawGizmos() {
        //CullingGroupBehavior cGB = (CullingGroupBehavior)target;
        Color drawColor = Color.yellow;

        Color originalGizmoColor = Gizmos.color;

        Gizmos.color = drawColor;


        Gizmos.DrawWireSphere(transform.position, boundingSphereRad);


        Gizmos.color = originalGizmoColor;
    }
    
}
