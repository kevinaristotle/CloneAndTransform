using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CloneAndTransformWindow : EditorWindow
{
    private static readonly Rect windowRect = new Rect(0, 0, 400, 210);

    private static readonly string windowTitleText = "Clone And Transform";
    private static readonly string targetObjectText = "Target Object";
    private static readonly string totalNumberText = "Total Number";
    private static readonly string translateText = "Translate";
    private static readonly string rotateText = "Rotate";
    private static readonly string scaleText = "Scale";
    private static readonly string pivotTranslateText = "Pivot Translate";
    private static readonly string cloneButtonText = "Clone";
    private static readonly string undoName = "Create Clone";

    private GameObject targetObject;
    private int totalNumber = 2;
    private Vector3 translate = Vector3.zero;
    private Vector3 rotate = Vector3.zero;
    private Vector3 scale = Vector3.one;
    private Vector3 pivotTranslate = Vector3.zero;

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
            pivotTranslate = EditorGUILayout.Vector3Field(pivotTranslateText, pivotTranslate);
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

            for (int i = 1; i < totalNumber; i++)
            {
                GameObject pivotGameObject = new GameObject("ClonePivot");
                pivotGameObject.transform.parent = targetObject.transform.parent;
                pivotGameObject.transform.position = targetObject.transform.position + pivotTranslate;
                pivotGameObject.transform.rotation = targetObject.transform.rotation;
                pivotGameObject.transform.localScale = targetObject.transform.localScale;
                pivotGameObject.hideFlags = HideFlags.HideAndDontSave;

                Vector3 cloneTranslation = pivotGameObject.transform.localPosition;
                Vector3 cloneRotation = pivotGameObject.transform.localRotation.eulerAngles;
                Vector3 cloneScale = scale;

                cloneTranslation += translate * i;
                cloneRotation += rotate * i;
                cloneScale = new Vector3(
                    Mathf.Pow(cloneScale.x, i),
                    Mathf.Pow(cloneScale.y, i),
                    Mathf.Pow(cloneScale.z, i)
                );

                GameObject clone = Instantiate(targetObject, targetObject.transform.parent);
                clone.transform.parent = pivotGameObject.transform;
                pivotGameObject.transform.localPosition = cloneTranslation;
                pivotGameObject.transform.localEulerAngles = cloneRotation;
                pivotGameObject.transform.localScale = Vector3.Scale(pivotGameObject.transform.localScale, cloneScale);
                clone.transform.parent = targetObject.transform.parent;
                clone.transform.hideFlags = HideFlags.None;
                Undo.RegisterCreatedObjectUndo(clone, undoName);
            }
        }
    }
}
