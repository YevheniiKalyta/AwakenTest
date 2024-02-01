using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastManager : MonoBehaviour
{
    public static RaycastHit PerformRaycastAtMousePos(LayerMask layerMask)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hitInfo, Consts.mouseRaycastDistance, layerMask);
        return hitInfo;
    }
}
