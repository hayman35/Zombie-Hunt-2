using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoardUI : MonoBehaviour
{
    private Camera playerCamera;

    private void Start()
    {
        playerCamera = Camera.main;
    }

    private void FixedUpdate() 
    {
        LookAtPlayer();
    }

    private void LookAtPlayer()
    {
        Vector3 v = playerCamera.transform.position - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt(playerCamera.transform.position - v);
        transform.Rotate(0, 180, 0);
    }
}
