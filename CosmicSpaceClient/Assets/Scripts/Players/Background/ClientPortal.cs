﻿using CosmicSpaceCommunication.Game.Resources;
using UnityEngine;

public class ClientPortal : MonoBehaviour
{
    private Portal portal;
    public Portal Portal
    {
        get => portal;
        set
        {
            portal = value;

            transform.position = new Vector3(value.PositionX, value.PositionY, transform.position.z);
        }
    }
}