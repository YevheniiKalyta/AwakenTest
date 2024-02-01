using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DiceSides))]
public class DiceSideEditor : Editor
{
    SerializedProperty diceSides;

    void OnEnable()
    {
        diceSides = serializedObject.FindProperty("Sides");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        ShowDiceSideInspectorGUI();

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Calculate Sides"))
        {
            CalculateSides();
        }
    }

    void ShowDiceSideInspectorGUI()
    {
        EditorGUILayout.LabelField("Dice Editor", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        if (diceSides != null)
        {
            for (int i = 0; i < diceSides.arraySize; i++)
            {
                ShowDiceSideUI(i);
            }
        }
        EditorGUI.indentLevel--;
    }

    void ShowDiceSideUI(int index)
    {
        SerializedProperty side = diceSides.GetArrayElementAtIndex(index);
        SerializedProperty value = side.FindPropertyRelative("Value");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(value, new GUIContent("Side " + (index + 1)));

        if (GUILayout.Button("Set Text", GUILayout.Width(70)))
        {
            RotateDiceToSide(index);
        }

        EditorGUILayout.EndHorizontal();
    }

    void RotateDiceToSide(int index)
    {

        DiceSides sides = target as DiceSides;
        sides.Sides[index].textMesh.gameObject.SetActive(false);
        sides.Sides[index].textMesh.text = sides.Sides[index].Value.ToString();
        sides.Sides[index].textMesh.gameObject.SetActive(true);
        sides.transform.rotation = sides.GetWorldRotationFor(index);
        SceneView.RepaintAll();
    }

    void CalculateSides()
    {
        DiceSides sides = target as DiceSides;
        Mesh mesh = GetMesh(sides);

        List<DiceSide> foundSides = FindDiceSides(mesh, sides);
        sides.Sides = new DiceSide[foundSides.Count];
        serializedObject.Update();

        for (int i = 0; i < foundSides.Count; i++)
        {
            DiceSide side = foundSides[i];
            SerializedProperty sideProperty = diceSides.GetArrayElementAtIndex(i);
            sideProperty.FindPropertyRelative("Center").vector3Value = side.Center;
            sideProperty.FindPropertyRelative("Normal").vector3Value = side.Normal;
            sideProperty.FindPropertyRelative("textMesh").objectReferenceValue = side.textMesh;
            sideProperty.FindPropertyRelative("Value").intValue = side.Value;

            side.textMesh.gameObject.SetActive(false);
            side.textMesh.text = side.Value.ToString();
            side.textMesh.gameObject.SetActive(true);
        }

        serializedObject.ApplyModifiedProperties();
    }

    List<DiceSide> FindDiceSides(Mesh mesh, DiceSides diceSides)
    {
        List<DiceSide> allTris = new List<DiceSide>();

        int[] triangles = mesh.GetTriangles(0);
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 a = vertices[triangles[i]];
            Vector3 b = vertices[triangles[i + 1]];
            Vector3 c = vertices[triangles[i + 2]] ;

            allTris.Add(new DiceSide((a + b + c) / 3f, Vector3.Cross(b - a, c - a).normalized));
        }

        Canvas canvas = diceSides.GetComponentInChildren<Canvas>();


        List<DiceSide> result = new List<DiceSide>();
        for (int i = 0; i < allTris.Count; i++)
        {
            if (result.Where(x => Vector3.Dot(x.Normal, allTris[i].Normal) > Consts.matchValue).ToList().Count > 0)
            {
                continue;
            }

            var normalGroup = allTris.Where(x => Vector3.Dot(x.Normal, allTris[i].Normal) > Consts.matchValue);
            Vector3 newCenter = Vector3.zero;
            foreach (var item in normalGroup)
            {
                newCenter += item.Center;
            }
            newCenter /= 3f;
            result.Add(new DiceSide(newCenter, allTris[i].Normal));
        }

        for (int i = canvas.transform.childCount; i > 0; --i)
            DestroyImmediate(canvas.transform.GetChild(0).gameObject);

        for (int i = 0; i < result.Count; i++)
        {
            GameObject textObj = Instantiate(diceSides.textPrefab, canvas.transform);
            textObj.transform.position = diceSides.transform.rotation * (result[i].Center + result[i].Normal * Consts.diceTextsOffset) + diceSides.transform.position;
            textObj.transform.forward = diceSides.transform.rotation * (-result[i].Normal);

            result[i].textMesh = textObj.GetComponent<TextMeshProUGUI>();
            result[i].Value = i + 1;
        }

        return result;
    }


    Mesh GetMesh(DiceSides sides)
    {
        MeshCollider meshCollider = sides.GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            return meshCollider.sharedMesh;
        }
        else
        {
            MeshFilter meshFilter = sides.GetComponent<MeshFilter>();
            return meshFilter.sharedMesh;
        }
    }
}
