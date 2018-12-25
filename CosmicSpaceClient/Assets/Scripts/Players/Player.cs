using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Players")]
    public Transform LocalPlayerTransform;
    public Transform PlayersTransform;
    public GameObject PlayerPrefab;

    private ShipLogic LocalPlayerShipLogic;



    private void Start()
    {
        ClearGameArea();
    }

    private void Update()
    {
        if (Client.Pilot == null)
            return;


    }



    public void InitLocalPlayer()
    {
        ClearGameArea();

        GameObject ship = Instantiate(
            PlayerPrefab, 
            LocalPlayerTransform);

        LocalPlayerShipLogic = ship.GetComponent<ShipLogic>();

        LocalPlayerShipLogic.InitShip(
            Client.Pilot.Ship.Name, 
            Client.Pilot.Nickname, 
            Color.blue);

        PlayerCamera.TargetGameObject = ship;
    }

    public void ClearGameArea()
    {
        foreach (Transform t in LocalPlayerTransform)
            Destroy(t.gameObject);

        foreach (Transform t in PlayersTransform)
            Destroy(t.gameObject);
    }
}