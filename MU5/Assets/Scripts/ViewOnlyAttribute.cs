using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ViewOnlyAttribute : PropertyAttribute {

    public ViewOnlyAttribute() { }
	
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ViewOnlyAttribute))]
public class ViewOnlyAttribute_Drawer : PropertyDrawer {

    void OnEnable() {
        Debug.Log("TestDrawer");
    }

    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.LabelField(position, property.name, property.intValue.ToString());
    }
    

}
#endif
