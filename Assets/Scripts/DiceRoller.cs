using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiceRoller : MonoBehaviour, IDraggable
{
    [SerializeField] Rigidbody rb;
    [Range(15,150)]
    [SerializeField] float rollForceMin;
    [Range(15, 150)]
    [SerializeField] float rollForceMax;


    [SerializeField] DiceSides diceSides;
    bool rollStarted;
    public Action<int> OnRollOver;
    [SerializeField] float speedLimit = 1000f;

    //Automatic Roll
    public async void PerformRoll()
    {
        if (!rollStarted)
        {
            Vector3 targetPostition = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f));
            Vector3 direction = (targetPostition - transform.position).normalized;
            direction.y = 0f;
            rb.AddForce(direction * Random.Range(rollForceMin, rollForceMax), ForceMode.Impulse);
            await Task.Delay(500);
            rollStarted = true;
        }
    }

    private void Update()
    {
        if (rollStarted)
        {
            if (rb.velocity == Vector3.zero && rb.angularVelocity == Vector3.zero)
            {
                Debug.Log(diceSides.GetResult());
                OnRollOver?.Invoke(diceSides.GetResult());
                rollStarted = false;
            }
        }
        if(transform.position.y <= -10f)
        {
            RespawnDice(); //Just in case the Dice will leave the box
        }
    }

    private void RespawnDice()
    {
        transform.position = Vector3.zero;
    }

    public void OnDrag()
    {
        //It's not used, yet, but it'll become handy if there will be somthing else to drag
    }

    public void OnDrop()
    {
        rb.mass = 1f;
        if (rb.velocity.sqrMagnitude + rb.angularVelocity.sqrMagnitude >= speedLimit)
        {
         
            rollStarted = true;
        }
        else
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = RaycastManager.PerformRaycastAtMousePos(Consts.tableLayerMask).point;
        }
    }

    public void OnStartDrag()
    {
        rb.mass = 10f;
    }

    
    public bool CanBeDragged()
    {
        return !rollStarted;
    }
}
