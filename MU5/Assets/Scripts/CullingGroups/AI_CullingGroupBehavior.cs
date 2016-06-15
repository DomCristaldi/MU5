using UnityEngine;

public class AI_CullingGroupBehavior : CullingGroupBehavior {

    MeshRenderer meshRen;

    protected override void Awake() {
        base.Awake();

        meshRen = GetComponent<MeshRenderer>();

        meshRen.enabled = false;
    }


    public override void OnVisible() {
        base.OnVisible();
        
        
        meshRen.enabled = true;
    }

    public override void OnInvisible() {
        base.OnInvisible();

        meshRen.enabled = false;
    }
	
}
