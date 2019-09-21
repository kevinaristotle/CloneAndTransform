using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CloneAndTransformWindow : EditorWindow
{
    private static readonly Rect windowRect = new Rect(0, 0, 400, 180);

    private static readonly string windowTitleText = "Clone And Transform";
    private static readonly string targetObjectText = "Target Object";
    private static readonly string totalNumberText = "Total Number";
    private static readonly string translateText = "Translate";
    private static readonly string rotateText = "Rotate";
    private static readonly string scaleText = "Scale";
    private static readonly string cloneButtonText = "Clone";
    private static readonly string undoName = "Create Clone";

    private GameObject targetObject;
    private int totalNumber = 2;
    private Vector3 translate = Vector3.zero;
    private Vector3 rotate = Vector3.zero;
    private Vector3 scale = Vector3.one;

    [MenuItem("Tools/Clone And Transform")]
    public static void ShowWindow()
    {
        GetWindowWithRect(typeof(CloneAndTransformWindow), windowRect, true, windowTitleText);
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            targetObject = EditorGUILayout.ObjectField(targetObjectText, targetObject, typeof(GameObject), true) as GameObject;
            totalNumber = Mathf.Max(2, EditorGUILayout.IntField(totalNumberText, totalNumber));
            translate = EditorGUILayout.Vector3Field(translateText, translate);
            rotate = EditorGUILayout.Vector3Field(rotateText, rotate);
            scale = EditorGUILayout.Vector3Field(scaleText, scale);
            EditorGUILayout.Space();
            CloneButton();

        }
        EditorGUILayout.EndVertical();
    }

    private void CloneButton()
    {
        if (GUILayout.Button(cloneButtonText))
        {
            if (targetObject == null)
            {
                return;
            }

            if (EditorUtility.IsPersistent(targetObject))
            {
                return;
            }

            Vector3 cloneTranslation = targetObject.transform.localPosition;
            Vector3 cloneRotation = targetObject.transform.localRotation.eulerAngles;
            Vector3 cloneScale = targetObject.transform.localScale;

            for (int i = 1; i < totalNumber; i++)
            {
                cloneTranslation += translate;
                cloneRotation += rotate;
                cloneScale = Vector3.Scale(cloneScale, scale);

                GameObject clone = Instantiate(targetObject, targetObject.transform.parent);
                clone.transform.localPosition = cloneTranslation;
                clone.transform.localEulerAngles = cloneRotation;
                clone.transform.localScale = cloneScale;
                Undo.RegisterCreatedObjectUndo(clone, undoName);
            }
        }
    }
}
