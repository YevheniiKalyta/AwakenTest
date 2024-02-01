using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consts
{
    public const float matchValue = 0.96f;
    public const float diceTextsOffset = 0.01f;
    public const float mouseRaycastDistance = 150f;

    public static LayerMask tableLayerMask = LayerMask.GetMask("Table");
    public static LayerMask grabbableLayerMask = LayerMask.GetMask("Draggable");
}
