using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class ObstacleStructDrawer : OdinValueDrawer<ObstacleStruct>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        Rect rect = EditorGUILayout.GetControlRect();

        if (label != null)
        {
            rect = EditorGUI.PrefixLabel(rect, label);
        }

        ObstacleStruct value = this.ValueEntry.SmartValue;
        GUIHelper.PushLabelWidth(20);
        value.obstacleType = (NormalObstacleType)EditorGUI.EnumPopup(rect.AlignLeft(rect.width * 0.5f),value.obstacleType);
        value.obstaclePoint = EditorGUI.IntField( rect.AlignRight(rect.width * 0.5f),"ObstaclePoint", value.obstaclePoint); 
        GUIHelper.PopLabelWidth();

        this.ValueEntry.SmartValue = value;
        
    }
}
