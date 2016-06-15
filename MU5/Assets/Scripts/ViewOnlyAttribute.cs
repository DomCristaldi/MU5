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

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        /*
        string displayValue = "";

        switch (property.type) {
            case "int":
                displayValue = property.intValue.ToString();
                break;

            case "float":
                displayValue = property.floatValue.ToString();
                break;

            case "bool":
                displayValue = property.boolValue.ToString();
                break;

            case "string":
                displayValue = property.stringValue;
                break;

            //case ""
        }
        */
        
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
        
        //EditorGUI.SelectableLabel(position, displayValue);

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

}
#endif
