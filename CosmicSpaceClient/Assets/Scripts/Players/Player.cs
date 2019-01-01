using CosmicSpaceCommunication.Game.Enemy;
using CosmicSpaceCommunication.Game.Player.ClientToServer;
using CosmicSpaceCommunication.Game.Player.ServerToClient;
using CosmicSpaceCommunication.Game.Resources;
using System;
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
    public Transform EnemiesTransform;
    public GameObject PlayerPrefab;

    public static ShipLogic LocalShipController;
    public static Dictionary<ulong, ShipLogic> PlayersController = new Dictionary<ulong, ShipLogic>();
    public static Dictionary<ulong, ShipLogic> EnemiesController = new Dictionary<ulong, ShipLogic>();

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
        if (Client.Pilot == null || LocalShipController == null)
            return;

        MouseControl();
        KeyboardControl();
        
        PlayerNewPosition();
    }

    #region Update / Mouse / Keyboard
    Vector2 TargetPosition;
    float timer = 0;
    private void PlayerNewPosition()
    {
        if (timer <= 0.1f)
        {
            timer += Time.deltaTime;
            return;
        }
        timer = 0;

        LocalShipController.TargetPosition = TargetPosition;
    }

    private void MouseControl()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
            MouseController();
        else if (Input.GetMouseButton(0))
            MouseController(false);
    }

    private void MouseController(bool press = true)
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

            if (hit.transform.tag == "Player")
            {
                ShipLogic target = hit.transform.gameObject.GetComponent<ShipLogic>();
                if (!target.IsDead)
                    LocalShipController.TargetGameObject = target.gameObject;
                else
                    Debug.Log(target.Nickname + " nie zyje.");
            }
            else if (hit.transform.tag == "Enemy")
            {

            }
        }
        else
        {
            float x = (mousePosition.x - Screen.width / 2) / 5;
            float y = (mousePosition.y - Screen.height / 2) / 5;
            TargetPosition = new Vector2(LocalShipController.Position.x + x, LocalShipController.Position.y + y);
        }
    }

    private void KeyboardControl()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            if (!LocalShipController.TargetIsNull)
                LocalShipController.Attack = !LocalShipController.Attack;
            else
                Debug.Log("Brak celu do ataku");
        }
    }
    #endregion



    #region LocalPlayer / Player
    public void InitLocalPlayer()
    {
        ShipLogic shipController = CreatePlayer(
            LocalPlayerTransform,
            true,
            PlayerJoin.GetNewJoin(Client.Pilot),
            Color.white);

        shipController.LocalPlayer = true;
        LocalShipController = shipController;
        TargetPosition = LocalShipController.TargetPosition;

        PlayersController.Add(Client.Pilot.Id, shipController);
        
        PlayerCamera.TargetGameObject = shipController.gameObject;

        CreateBackground(Client.Pilot.Map);
    }

    public void InitPlayer(PlayerJoin player)
    {
        if (PlayersController.ContainsKey(player.PlayerId))
            return;

        ShipLogic shipController = CreatePlayer(
            PlayersTransform,
            false,
            player,
            Color.blue);

        PlayersController.Add(player.PlayerId, shipController);
    }

    private ShipLogic CreatePlayer(Transform spawnTransform, bool localPlayer, PlayerJoin player, Color nicknameColor)
    {
        GameObject go = Instantiate(
            PlayerPrefab,
            spawnTransform);

        ShipLogic shipController = go.GetComponent<ShipLogic>();
        
        shipController.InitShip(
            player,
            nicknameColor,
            localPlayer);

        return shipController;
    }
    
    public void LeavePlayer(ulong playerId)
    {
        if (!PlayersController.ContainsKey(playerId))
            return;

        Destroy(PlayersController[playerId].gameObject);
        PlayersController.Remove(playerId);
    }
    #endregion



    #region Enemy
    public void InitEnemy(EnemyJoin enemy)
    {
        if (EnemiesController.ContainsKey(enemy.Id))
            return;

        GameObject go = Instantiate(
                    PlayerPrefab,
                    EnemiesTransform);

        ShipLogic shipController = go.GetComponent<ShipLogic>();

        shipController.InitShip(enemy, Color.red);

        EnemiesController.Add(enemy.Id, shipController);
    }

    public void LeaveEnemy(ulong enemyId)
    {
        if (!EnemiesController.ContainsKey(enemyId))
            return;

        Destroy(EnemiesController[enemyId].gameObject);
        EnemiesController.Remove(enemyId);
    }
    #endregion



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



    #region Events
    public void PlayerChangePosition(NewPosition newPosition)
    {
        Vector2 position = new Vector2(newPosition.PositionX, newPosition.PositionY);
        Vector2 targetPosition = new Vector2(newPosition.TargetPositionX, newPosition.TargetPositionY);

        ShipLogic pilot = FindPilot(newPosition.PlayerId);
        if (pilot == null)
            return;

        pilot.Position = position;
        pilot.TargetPosition = targetPosition;
        pilot.Player.Speed = newPosition.Speed;
    }

    public void PlayerHitpointsOrShields(NewHitpointsOrShields newValue, bool hitpoints)
    {
        ShipLogic pilot = FindPilot(newValue.PlayerId);
        if (pilot == null)
            return;

        if (hitpoints)
        {
            pilot.Hitpoints = newValue.Value;
            pilot.MaxHitpoints = newValue.MaxValue;
        }
        else
        {
            pilot.Shields = newValue.Value;
            pilot.MaxShields = newValue.MaxValue;
        }
    }

    public void PlayerSelectTarget(NewTarget newTarget)
    {
        ShipLogic targetShipLogic = null;
        if (newTarget.TargetIsPlayer == true) // Target = Pilot
        {
            if(newTarget.TargetId != null)
            {
                targetShipLogic = FindPilot(newTarget.TargetId);
            }
        }
        else if (newTarget.TargetIsPlayer == false) // Target = Enemy
        {
            // Enemy
        }

        GameObject target = null;
        if (targetShipLogic != null)
            target = targetShipLogic.gameObject;

        ShipLogic pilot = FindPilot(newTarget.PlayerId);
        if (pilot == null)
            return;

        pilot.TargetGameObject = target;
    }

    public void PlayerAttackTarget(AttackTarget attackTarget)
    {
        PlayerSelectTarget(attackTarget);

        ShipLogic pilot = FindPilot(attackTarget.PlayerId);
        if (pilot == null)
            return;

        pilot.Attack = attackTarget.Attack;
    }

    public void SomeoneTakeDamage(TakeDamage takeDamage)
    {
        ShipLogic fromShipLogic = null;
        if (takeDamage.FromIsPlayer == true) // Target = Pilot
        {
            if (PlayersController.ContainsKey(takeDamage.FromId))
            {
                fromShipLogic = PlayersController[takeDamage.FromId];
            }
        }
        else if (takeDamage.FromIsPlayer == false) // Target = Enemy
        {
            // Enemy
        }

        ShipLogic toShipLogic = null;
        if (takeDamage.ToIsPlayer == true) // Target = Pilot
        {
            if (PlayersController.ContainsKey(takeDamage.ToId))
            {
                toShipLogic = PlayersController[takeDamage.ToId];
            }
        }
        else if (takeDamage.ToIsPlayer == false) // Target = Enemy
        {
            // Enemy
        }

        Debug.Log(fromShipLogic.name + " zadaje " + takeDamage.Damage + " obrazen " + toShipLogic.name);
    }

    public void SomeoneDead(SomeoneDead someoneDead)
    {
        ShipLogic whoShipLogic = null;
        if (someoneDead.WhoIsPlayer == true) // Target = Pilot
        {
            whoShipLogic = FindPilot(someoneDead.WhoId);
        }
        else if (someoneDead.WhoIsPlayer == false) // Target = Enemy
        {
            // Enemy
        }

        ShipLogic byShipLogic = null;
        if (someoneDead.ByIsPlayer == true) // Target = Pilot
        {
            byShipLogic = FindPilot(someoneDead.ById);
        }
        else if (someoneDead.ByIsPlayer == false) // Target = Enemy
        {
            // Enemy
        }

        if (whoShipLogic == null)
            return;

        whoShipLogic.IsDead = true;

        foreach(ShipLogic shipLogic in PlayersController.Values)
        {
            if (shipLogic.TargetGameObject == whoShipLogic.gameObject || shipLogic.TargetGameObject == byShipLogic.gameObject)
            {
                shipLogic.Attack = false;
                shipLogic.TargetGameObject = null;
            }
        }
        
        //if(whoShipLogic == LocalShipController)
        //{
        //    Debug.Log("Zostales zniszczony przez " + someoneDead.ByName);
        //}
        //else
        //{
        //    Debug.Log(whoShipLogic.name + " zostal zabity przez " + byShipLogic.name);
        //}
    }

    public void SomeoneAlive(ulong userId)
    {
        ShipLogic whoShipLogic = FindPilot(userId);

        if (whoShipLogic == null)
            return;

        whoShipLogic.IsDead = false;
    }
    #endregion



    private ShipLogic FindPilot(ulong? pilotId)
    {
        if (!PlayersController.ContainsKey((ulong)pilotId))
            return null;
        return PlayersController[(ulong)pilotId];
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