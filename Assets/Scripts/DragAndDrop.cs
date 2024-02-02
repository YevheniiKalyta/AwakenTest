using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    public IDraggable objectToDrag;
    [SerializeField] ConfigurableJoint hook;
    [SerializeField] Rigidbody hookRb;
    [SerializeField] float floorOffset;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Collider raycastCollider = RaycastManager.PerformRaycastAtMousePos(Consts.grabbableLayerMask).collider;
            if (raycastCollider != null)
            {
                var potentialObjToDrag = raycastCollider.GetComponent<IDraggable>();
                if (potentialObjToDrag == null || !potentialObjToDrag.CanBeDragged()) return;
                objectToDrag = potentialObjToDrag;
                objectToDrag.OnStartDrag();
                hook.connectedBody = raycastCollider.GetComponent<Rigidbody>();
                hook.connectedAnchor = Vector3.zero;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            objectToDrag?.OnDrop();
            objectToDrag = null;
            hook.connectedBody = null;
        }
    }
    private void FixedUpdate()
    {
        if (objectToDrag != null)
        {
            RaycastHit raycastHit = RaycastManager.PerformRaycastAtMousePos(Consts.tableLayerMask);
            if (raycastHit.collider != null)
            {
                Vector3 positionToMoveTo = raycastHit.point - (raycastHit.point - Camera.main.transform.position).normalized * floorOffset;
                hookRb.MovePosition(positionToMoveTo);

            }
            else
            {
                hookRb.MovePosition(hook.connectedBody.position);
            }
            objectToDrag.OnDrag();
        }
    }


}
