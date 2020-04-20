#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(HealthBarAndText))]
[CanEditMultipleObjects]
public class HealthBarEditor : Editor
{
    bool healthBarSettings = true;
    bool alphaSettings = true;
    bool hitTextSettings = true;

    public override void OnInspectorGUI()
    {
        HealthBarAndText myTarget = (HealthBarAndText)target;
        Undo.RecordObject(target, "Undo");


        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        hitTextSettings = EditorGUILayout.Foldout(hitTextSettings, "Hit Text Settings");
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        
        if (hitTextSettings)
        {
            EditorGUILayout.BeginVertical("Box");
            GUILayout.Space(5);
            myTarget.hitTextHolderTransform = (RectTransform)EditorGUILayout.ObjectField("HitTextHolderPrefab", myTarget.hitTextHolderTransform, typeof(RectTransform), false);
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 100;
            EditorGUIUtility.fieldWidth = 20;
            myTarget.hitFontSize = EditorGUILayout.IntField("Font Size", myTarget.hitFontSize);
            EditorGUIUtility.labelWidth = 100;
            myTarget.hitTexts_yOffset = EditorGUILayout.IntField("Y Offset", myTarget.hitTexts_yOffset);
            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = 150;
            myTarget.hitTextColor = EditorGUILayout.ColorField("Text Color", myTarget.hitTextColor);
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        healthBarSettings = EditorGUILayout.Foldout(healthBarSettings, "Health Bar Settings");
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        if (healthBarSettings)
        {
            EditorGUILayout.BeginVertical("Box");
            GUILayout.Space(5);
            EditorGUIUtility.labelWidth = 100;
            myTarget.healthbarRectTransform = (RectTransform)EditorGUILayout.ObjectField("HealthbarPrefab", myTarget.healthbarRectTransform, typeof(RectTransform), false);
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 75;
            EditorGUIUtility.fieldWidth = 20;
            myTarget.xOffset = EditorGUILayout.FloatField("X Offset", myTarget.xOffset);
            EditorGUIUtility.labelWidth = 75;
            EditorGUIUtility.fieldWidth = 20;
            myTarget.yOffset = EditorGUILayout.FloatField("Y Offset", myTarget.yOffset);
            EditorGUIUtility.labelWidth = 75;
            myTarget.zOffset = EditorGUILayout.FloatField("Z Offset", myTarget.zOffset);
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 75;
            EditorGUIUtility.fieldWidth = 20;
            myTarget.keepSize = EditorGUILayout.Toggle("Fixed Size", myTarget.keepSize);
            EditorGUIUtility.labelWidth = 75;
            myTarget.scale = EditorGUILayout.FloatField("Scale", myTarget.scale);
            EditorGUILayout.EndVertical();
            EditorGUIUtility.labelWidth = 150;
            myTarget.sizeOffsets = EditorGUILayout.Vector2Field("Size Offsets", myTarget.sizeOffsets);
            myTarget.healthBarDefaultColor = EditorGUILayout.ColorField("Health Bar Default Color", myTarget.healthBarDefaultColor);

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.BeginVertical();
            alphaSettings = EditorGUILayout.Foldout(alphaSettings, "Alpha Settings");
            if (alphaSettings)
            {
                EditorGUILayout.HelpBox("States alphas and it's fade speeds. Zero is no fade.", MessageType.Info);
                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 45;
                EditorGUIUtility.fieldWidth = 45;
                EditorGUIUtility.labelWidth = 75;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 45;
                EditorGUIUtility.fieldWidth = 45;
                myTarget.alphaSettings.fullAplpha = EditorGUILayout.Slider("Full ", myTarget.alphaSettings.fullAplpha, 0, 1);
                EditorGUIUtility.labelWidth = 75;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 45;
                EditorGUIUtility.fieldWidth = 45;
                myTarget.alphaSettings.nullAlpha = EditorGUILayout.Slider("Null ", myTarget.alphaSettings.nullAlpha, 0, 1);
                EditorGUIUtility.labelWidth = 75;
                myTarget.alphaSettings.nullFadeSpeed = EditorGUILayout.FloatField("Fade Speed", myTarget.alphaSettings.nullFadeSpeed);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        if (GUI.changed)
            EditorUtility.SetDirty(myTarget);
    }
}
#endif