using CosmicSpaceCommunication.Game.Player.ClientToServer;
using CosmicSpaceCommunication.Game.Player.ServerToClient;
using CosmicSpaceCommunication.Game.Resources;
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

    private ShipLogic LocalShipController;

    public static Dictionary<ulong, ShipLogic> PlayersController = new Dictionary<ulong, ShipLogic>();



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
            LocalShipController.TargetPosition = new Vector2(LocalShipController.Position.x + x, LocalShipController.Position.y + y);
        }
    }



    public void InitLocalPlayer()
    {
        ShipLogic shipController = CreatePlayer(
            LocalPlayerTransform,
            "LocalPlayer",
            new Vector2(Client.Pilot.PositionX, Client.Pilot.PositionY),
            Client.Pilot.Ship,
            Client.Pilot.Nickname,
            Color.white);
        shipController.LocalPlayer = true;
        LocalShipController = shipController;

        PlayerCamera.TargetGameObject = shipController.gameObject;
    }

    public void InitPlayer(PlayerJoin player)
    {
        ShipLogic shipController = CreatePlayer(
            PlayersTransform,
            "Player",
            new Vector2(player.PositionX, player.PositionY),
            player.Ship,
            player.Nickname,
            Color.blue);
        
        PlayersController.Add(player.PlayerId, shipController);
    }

    private ShipLogic CreatePlayer(Transform spawnTransform, string tag, Vector2 position, Ship ship, string nickname, Color nicknameColor)
    {
        GameObject go = Instantiate(
            PlayerPrefab,
            spawnTransform);
        go.tag = tag;
        go.transform.position = position;

        ShipLogic shipController = go.GetComponent<ShipLogic>();
        shipController.TargetPosition = shipController.Position;

        shipController.InitShip(
            ship,
            nickname,
            nicknameColor);
        return shipController;
    }
    
    public void LeavePlayer(ulong playerId)
    {
        if (!PlayersController.ContainsKey(playerId))
            return;

        Destroy(PlayersController[playerId].gameObject);
        PlayersController.Remove(playerId);
    }



    public void PlayerChangePosition(NewPosition newPosition)
    {
        if (newPosition == null)
            return;

        Vector2 position = new Vector2(newPosition.PositionX, newPosition.PositionY);
        Vector2 targetPosition = new Vector2(newPosition.TargetPositionX, newPosition.TargetPositionY);

        if (newPosition.PlayerId == Client.Pilot.Id)
        {
            LocalShipController.Position = position;
            LocalShipController.TargetPosition = targetPosition;
        }
        else
        {
            if (!PlayersController.ContainsKey(newPosition.PlayerId))
                return;
            
            PlayersController[newPosition.PlayerId].Position = position;
            PlayersController[newPosition.PlayerId].TargetPosition = targetPosition;
        }
    }

    public void ClearGameArea()
    {
        foreach (Transform t in LocalPlayerTransform)
            Destroy(t.gameObject);

        foreach (Transform t in PlayersTransform)
            Destroy(t.gameObject);

        PlayersController.Clear();
    }
}