﻿using CosmicSpaceCommunication.Game.Player.ClientToServer;
using CosmicSpaceCommunication.Game.Player.ServerToClient;
using CosmicSpaceCommunication.Game.Resources;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    [Header("Local player and players")]
    public Transform LocalPlayerTransform;
    public Transform PlayersTransform;
    public GameObject PlayerPrefab;

    public static ShipLogic LocalShipController;
    public static Dictionary<ulong, ShipLogic> PlayersController = new Dictionary<ulong, ShipLogic>();

    [Header("Map and background")]
    public Transform BackgroundTransform;
    public GameObject StarsGameObject;
    public List<GameObject> MapsGameObject = new List<GameObject>();



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

        CreateBackground(Client.Pilot.Map);
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
        shipController.Speed = ship.Speed;

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



    public void CreateBackground(Map map)
    {
        foreach (Transform t in BackgroundTransform)
            Destroy(t.gameObject);

        // Create stars - settings
        if (true)
        {
            GameObject stars = Instantiate(StarsGameObject, BackgroundTransform);
            #pragma warning disable CS0618 // Type or member is obsolete
            stars.GetComponent<ParticleSystem>().maxParticles = 2000;
            #pragma warning restore CS0618 // Type or member is obsolete
        }

        // Create background - settings
        if (true)
        {
            GameObject background = MapsGameObject.FirstOrDefault(o => o?.GetComponent<Background>().MapId == map.Id);
            if (background != null)
                Instantiate(background, BackgroundTransform);
        }
    }



    public void PlayerChangePosition(NewPosition newPosition)
    {
        Vector2 position = new Vector2(newPosition.PositionX, newPosition.PositionY);
        Vector2 targetPosition = new Vector2(newPosition.TargetPositionX, newPosition.TargetPositionY);

        if (newPosition.PlayerId == Client.Pilot.Id)
        {
            LocalShipController.Position = position;
            LocalShipController.TargetPosition = targetPosition;
            LocalShipController.Speed = newPosition.Speed;
        }
        else
        {
            if (!PlayersController.ContainsKey(newPosition.PlayerId))
                return;
            
            PlayersController[newPosition.PlayerId].Position = position;
            PlayersController[newPosition.PlayerId].TargetPosition = targetPosition;
            PlayersController[newPosition.PlayerId].Speed = newPosition.Speed;
        }
    }

    public void PlayerHitpointsOrShields(NewHitpointsOrShields newValue, bool hitpoints)
    {
        if (newValue.PlayerId == Client.Pilot.Id)
        {
            if(hitpoints)
            {
                LocalShipController.Hitpoints = newValue.Value;
                LocalShipController.MaxHitpoints = newValue.MaxValue;
            }
            else
            {
                LocalShipController.Shields = newValue.Value;
                LocalShipController.MaxShields = newValue.MaxValue;
            }
            return;
        }

        if (!PlayersController.ContainsKey(newValue.PlayerId))
            return;

        if (hitpoints)
        {
            PlayersController[newValue.PlayerId].Hitpoints = newValue.Value;
            PlayersController[newValue.PlayerId].MaxHitpoints = newValue.MaxValue;
        }
        else
        {
            PlayersController[newValue.PlayerId].Shields = newValue.Value;
            PlayersController[newValue.PlayerId].MaxShields = newValue.MaxValue;
        }
    }


    public void ClearGameArea()
    {
        foreach (Transform t in LocalPlayerTransform)
            Destroy(t.gameObject);

        foreach (Transform t in PlayersTransform)
            Destroy(t.gameObject);

        foreach (Transform t in BackgroundTransform)
            Destroy(t.gameObject);

        PlayersController.Clear();
    }
}