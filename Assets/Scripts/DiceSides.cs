using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
[System.Serializable]
public class DiceSide
{
    public Vector3 Center;
    public Vector3 Normal;
    public TextMeshProUGUI textMesh;
    public int Value;

    public DiceSide(Vector3 Center, Vector3 Normal)
    {
        this.Center = Center;
        this.Normal = Normal;
    }
}
public class DiceSides : MonoBehaviour
{
    [SerializeField] public DiceSide[] Sides = new DiceSide[0];
    public GameObject textPrefab;


    public DiceSide GetDiceSide(int index) => Sides[index];

    public Quaternion GetWorldRotationFor(int index)
    {
        Vector3 worldNormalToMatch = transform.TransformDirection(GetDiceSide(index).Normal);
        return Quaternion.FromToRotation(worldNormalToMatch, Vector3.up) * transform.rotation;
    }
    public int GetMatch()
    {
        int sideCount = Sides.Length;

        Vector3 localVectorToMatch = transform.InverseTransformDirection(Vector3.up);

        DiceSide closestSide = null;
        float closestDot = -1f;

        for (int i = 0; i < sideCount; i++)
        {
            DiceSide side = Sides[i];
            float dot = Vector3.Dot(side.Normal, localVectorToMatch);

            if (closestSide == null || dot > closestDot)
            {
                closestSide = side;
                closestDot = dot;
            }

            if (dot > Consts.matchValue)
            {
                return side.Value;
            }
        }

        return closestSide?.Value ?? -1;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < Sides.Length; i++)
        {
            Gizmos.DrawSphere(transform.rotation * Sides[i].Center + transform.position, 0.1f);
            Gizmos.DrawRay(transform.rotation * Sides[i].Center + transform.position, transform.rotation * Sides[i].Normal );
        }
    }
}
