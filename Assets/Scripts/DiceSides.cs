using System;
using System.Collections;
using TMPro;
using UnityEngine;

[System.Serializable]
public class DiceSide
{
    public Vector3 Center;
    public Vector3 Normal;
    public TextMeshProUGUI TextMesh;
    public int Value;

    public DiceSide(Vector3 Center, Vector3 Normal)
    {
        this.Center = Center;
        this.Normal = Normal;
    }
}

public class DiceSides : MonoBehaviour
{
    [HideInInspector] public DiceSide[] sides;
    public GameObject textPrefab;
    public DiceSide highlightedSide { get; private set; }

    /// <summary>
    /// Helper to rotate the dice
    /// </summary>
    public Quaternion GetWorldRotationFor(int index)
    {
        Vector3 worldNormalToMatch = transform.TransformDirection(sides[index].Normal);
        return Quaternion.FromToRotation(worldNormalToMatch, Vector3.up) * transform.rotation;
    }
    /// <summary>
    /// Return upper number of the dice
    /// </summary>
    public int GetResult()
    {
        Vector3 localVectorToMatch = transform.InverseTransformDirection(Vector3.up);

        DiceSide closestSide = null;
        float closestDot = -1f;

        for (int i = 0; i < sides.Length; i++)
        {
            DiceSide side = sides[i];
            float dot = Vector3.Dot(side.Normal, localVectorToMatch);

            if (closestSide == null || dot > closestDot)
            {
                closestSide = side;
                closestDot = dot;
            }

            if (dot > Consts.matchValue)
            {
                StartCoroutine(HighlightNumber(side));
                return side.Value;
            }
        }

        return closestSide?.Value ?? -1;
    }

    private IEnumerator HighlightNumber(DiceSide diceSide)
    {
        highlightedSide = diceSide;
        diceSide.TextMesh.color = Color.yellow;
        yield return new WaitForSecondsRealtime(2f);
        diceSide.TextMesh.color = Color.white;
        highlightedSide = null;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < sides.Length; i++)
        {
            Gizmos.DrawSphere(transform.rotation * sides[i].Center + transform.position, 0.1f);
            Gizmos.DrawRay(transform.rotation * sides[i].Center + transform.position, transform.rotation * sides[i].Normal );
        }
    }

    internal void TurnOffHighlight()
    {
        StopAllCoroutines();
        highlightedSide.TextMesh.color = Color.white;
    }
}
