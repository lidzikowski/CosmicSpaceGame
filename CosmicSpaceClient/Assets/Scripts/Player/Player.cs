using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Camera")]
    public GameObject CameraGameObject;

    [Header("Player ship")]
    public GameObject ShipGameObject;



    private void Update()
    {
        if (Client.Pilot == null)
            return;


    }

    public void InitPlayer()
    {

    }
}