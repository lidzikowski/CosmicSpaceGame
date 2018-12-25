using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

        MouseControl();
    }

    protected void MouseControl()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
            Controller();
        else if (Input.GetMouseButton(0))
            Controller(false);
    }

    private void Controller(bool press = true)
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (!press)
                return;

            if (hit.transform.tag == "LocalPlayer")
                return;

            if (hit.transform.tag == "Player" || hit.transform.tag == "Enemy")
            {
                //Attack = false;
                //Target = hit.transform.gameObject;
            }
        }
        else
        {
            float x = (mousePosition.x - Screen.width / 2) / 20;
            float y = (mousePosition.y - Screen.height / 2) / 20;
            LocalPlayerShipLogic.TargetPosition = new Vector2(LocalPlayerShipLogic.Position.x + x, LocalPlayerShipLogic.Position.y + y);
        }
    }

    public void InitLocalPlayer()
    {
        ClearGameArea();

        GameObject ship = Instantiate(
            PlayerPrefab, 
            LocalPlayerTransform);
        ship.tag = "LocalPlayer";

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