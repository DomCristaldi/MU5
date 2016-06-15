using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CullingGroupBehavior), true)]
[CanEditMultipleObjects]
public class CullingGroupBehavior_Editor : Editor {

    Color drawColor = Color.yellow;
    

    void OnSceneGUI() {
        CullingGroupBehavior selfScript = (CullingGroupBehavior)target;

        //don't allow editing of radius while in Play mode
        if (Application.isPlaying) { return; }

        //for resetting Handle color to what it was originally
        Color originalColor = Handles.color;
        Handles.color = drawColor;
   

        //switch case handles drawing Handles at all
         //intersections of the disks that are drawn by the Gizmo
         //in the Monobehavior
        Vector3 posFromCenter = Vector3.zero;
        Vector3 vecDirec = Vector3.zero;
        for (int i = 0; i < 6; ++i) {
            switch (i) {
                case 0:
                    vecDirec = Vector3.up;
                    break;
                case 1:
                    vecDirec = Vector3.down;
                    break;
                case 2:
                    vecDirec = Vector3.right;
                    break;
                case 3:
                    vecDirec = Vector3.left;
                    break;
                case 4:
                    vecDirec = Vector3.forward;
                    break;
                case 5:
                    vecDirec = Vector3.back;
                    break;
            }

            //DRAW HANDLE
            posFromCenter = Handles.Slider(selfScript.transform.position + (vecDirec * selfScript.boundingSphereRad), //position
                                           vecDirec,                                                                  //direction
                                           HandleUtility.GetHandleSize(selfScript.transform.position) * 0.1f,        //handle size
                                           Handles.CubeCap,                                                           //handle shape
                                           0.0f                                                                       //snap amount
                                           );

            float calcDist = Vector3.Distance(selfScript.transform.position, posFromCenter);

            //if radius was changed via Handle, record this frame in the Undo Stack
             //then apply the change
            if (calcDist != selfScript.boundingSphereRad) { 
                Undo.RecordObject(selfScript, "CullingGroupBehavior_ModifyRadius");

                selfScript.boundingSphereRad = calcDist;
                EditorUtility.SetDirty(target);//needs to be called so Undo Stack knows something was changed
            }
        }

        Handles.color = originalColor;

    }
    

}
