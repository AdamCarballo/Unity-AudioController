using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
public class MinMaxRangeDrawer : PropertyDrawer {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return base.GetPropertyHeight(property, label) + 16;
    } 

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
       
        // Now draw the property as a Slider or an IntSlider based on whether it’s a float or integer.
        if (property.type != "MinMaxRange") {
            Debug.LogWarning("Use only with MinMaxRange type");
            return;
        }
        
        var range = attribute as MinMaxRangeAttribute;
        var minValue = property.FindPropertyRelative("_rangeStart");
        var maxValue = property.FindPropertyRelative("_rangeEnd");
        var newMin = minValue.floatValue;
        var newMax = maxValue.floatValue;
    
        var xDivision = position.width * 0.33f;
        var yDivision = position.height * 0.5f;
        EditorGUI.LabelField(new Rect(position.x, position.y, xDivision, yDivision), label);
        EditorGUI.LabelField(new Rect(position.x, position.y + yDivision, position.width, yDivision), range.MinLimit.ToString("0.##"));
        EditorGUI.LabelField(new Rect(position.x + position.width - 48f, position.y + yDivision, position.width, yDivision), range.MaxLimit.ToString("0.##"));
        EditorGUI.MinMaxSlider(new Rect(position.x + 24f, position.y + yDivision, position.width - 48f, yDivision), ref newMin, ref newMax, range.MinLimit, range.MaxLimit);
    
        EditorGUI.LabelField(new Rect(position.x + xDivision - 10, position.y, xDivision, yDivision), "From: ");
        var rect = new Rect(position.x + xDivision + 30, position.y, xDivision - 30, yDivision);
        newMin = Mathf.Clamp(EditorGUI.FloatField(rect, newMin), range.MinLimit, newMax);
        EditorGUI.LabelField(new Rect(position.x + xDivision * 2f, position.y, xDivision, yDivision), "To: ");
        rect = new Rect(position.x + xDivision * 2f + 24, position.y, xDivision - 24, yDivision);
        newMax = Mathf.Clamp(EditorGUI.FloatField(rect, newMax), newMin, range.MaxLimit);
    
        minValue.floatValue = newMin;
        maxValue.floatValue = newMax;
    }
}