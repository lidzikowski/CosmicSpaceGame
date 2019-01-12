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

            if (hit.transform.tag == "Player" || hit.transform.tag == "Enemy")
            {
                ShipLogic target = hit.transform.gameObject.GetComponent<ShipLogic>();
                if (!target.IsDead)
                    LocalShipController.TargetGameObject = target.gameObject;
            }
        }
        else
        {
            if (Camera.current == null)
                return;

            Vector3 tmp = Camera.current.ScreenToViewportPoint(mousePosition);
            Vector2 position = new Vector2(tmp.x - 0.5f, tmp.y - 0.5f) * 100;
            TargetPosition = new Vector2(LocalShipController.Position.x + position.x, LocalShipController.Position.y + position.y);
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
        LocalShipController = CreatePlayer(
            LocalPlayerTransform,
            true,
            PlayerJoin.GetNewJoin(Client.Pilot),
            Color.white);

        LocalShipController.LocalPlayer = true;
        TargetPosition = LocalShipController.TargetPosition;

        PlayersController.Add(Client.Pilot.Id, LocalShipController);
        
        PlayerCamera.TargetGameObject = LocalShipController.gameObject;

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
    public void ChangePosition(NewPosition newPosition)
    {
        ShipLogic shipLogic = newPosition.IsPlayer ? FindPilot(newPosition.PlayerId) : FindEnemy(newPosition.PlayerId);

        if (shipLogic == null)
            return;

        Vector2 position = new Vector2(newPosition.PositionX, newPosition.PositionY);
        Vector2 targetPosition = new Vector2(newPosition.TargetPositionX, newPosition.TargetPositionY);

        shipLogic.Position = position;
        shipLogic.TargetPosition = targetPosition;
        shipLogic.Speed = newPosition.Speed;
    }

    public void HitpointsOrShields(NewHitpointsOrShields newValue, bool hitpoints)
    {
        ShipLogic shipLogic = newValue.IsPlayer ? FindPilot(newValue.PlayerId) : FindEnemy(newValue.PlayerId);
        
        if (shipLogic == null)
            return;

        if (hitpoints)
        {
            shipLogic.Hitpoints = newValue.Value;
            shipLogic.MaxHitpoints = newValue.MaxValue;
        }
        else
        {
            shipLogic.Shields = newValue.Value;
            shipLogic.MaxShields = newValue.MaxValue;
        }
    }

    public ShipLogic SelectTarget(NewTarget newTarget)
    {
        if (newTarget.TargetIsPlayer == null)
            return null;

        ShipLogic targetShipLogic = newTarget.TargetIsPlayer == true ? FindPilot(newTarget.PlayerId) : FindEnemy(newTarget.PlayerId);

        if (targetShipLogic == null)
            return null;
        
        ShipLogic attackerShipLogic = newTarget.AttackerIsPlayer == true ? FindPilot(newTarget.TargetId) : FindEnemy(newTarget.TargetId);

        if (attackerShipLogic == null)
            return null;

        attackerShipLogic.TargetGameObject = targetShipLogic.gameObject;

        return attackerShipLogic;
    }

    public void AttackTarget(AttackTarget attackTarget)
    {
        ShipLogic shipLogic = null;
        if (attackTarget.Attack)
            shipLogic = SelectTarget(attackTarget);
        else
            shipLogic = attackTarget.TargetIsPlayer == true ? FindPilot(attackTarget.PlayerId) : FindEnemy(attackTarget.PlayerId);

        if (shipLogic == null)
            return;

        shipLogic.Attack = attackTarget.Attack;
    }

    public void SomeoneTakeDamage(TakeDamage takeDamage)
    {
        ShipLogic fromShipLogic = takeDamage.FromIsPlayer == true ? FindPilot(takeDamage.FromId) : FindEnemy(takeDamage.FromId);

        if (fromShipLogic == null)
            return;

        ShipLogic toShipLogic = takeDamage.ToIsPlayer == true ? FindPilot(takeDamage.ToId) : FindEnemy(takeDamage.ToId);

        if (toShipLogic == null)
            return;
        
        fromShipLogic.TargetGameObject = toShipLogic.gameObject;
        fromShipLogic.Attack = true;

        //Debug.Log(fromShipLogic.name + " zadaje " + takeDamage.Damage + " obrazen " + toShipLogic.name);
    }

    public void SomeoneDead(SomeoneDead someoneDead)
    {
        ShipLogic whoShipLogic = someoneDead.WhoIsPlayer == true ? FindPilot(someoneDead.WhoId) : FindEnemy(someoneDead.WhoId);

        if (whoShipLogic == null)
            return;

        ShipLogic byShipLogic = someoneDead.ByIsPlayer == true ? FindPilot(someoneDead.ById) : FindEnemy(someoneDead.ById);
        
        whoShipLogic.IsDead = true;

        foreach(ShipLogic shipLogic in PlayersController.Values)
        {
            if (shipLogic.TargetGameObject == whoShipLogic.gameObject || shipLogic.TargetGameObject == byShipLogic?.gameObject)
            {
                shipLogic.TargetGameObject = null;
                shipLogic.Attack = false;
            }
        }

        foreach (ShipLogic shipLogic in EnemiesController.Values)
        {
            if (shipLogic.TargetGameObject == whoShipLogic.gameObject || shipLogic.TargetGameObject == byShipLogic?.gameObject)
            {
                shipLogic.TargetGameObject = null;
                shipLogic.Attack = false;
            }
        }
    }

    public void SomeoneAlive(ulong userId)
    {
        ShipLogic whoShipLogic = FindPilot(userId);

        if (whoShipLogic == null)
            return;

        whoShipLogic.IsDead = false;
    }

    public void TakeReward(ServerReward reward)
    {
        List<string> messages = new List<string>();

        if (reward.Experience != null)
        {
            Client.Pilot.Experience += (ulong)reward.Experience;
            messages.Add($"Experience {reward.Experience}");
        }
        if (reward.Metal != null)
        {
            Client.Pilot.Metal += (double)reward.Metal;
            messages.Add($"Metal {reward.Metal}");
        }
        if (reward.Scrap != null)
        {
            Client.Pilot.Scrap += (double)reward.Scrap;
            messages.Add($"Scrap {reward.Scrap}");
        }

        GuiScript.CreateLogMessage(messages);

        MainThread.Instance().Enqueue(() => GuiScript.RefreshAllActiveWindow());
    }
    #endregion



    private ShipLogic FindPilot(ulong? pilotId)
    {
        if (!PlayersController.ContainsKey((ulong)pilotId))
            return null;
        return PlayersController[(ulong)pilotId];
    }
    private ShipLogic FindEnemy(ulong? enemyId)
    {
        if (!EnemiesController.ContainsKey((ulong)enemyId))
            return null;
        return EnemiesController[(ulong)enemyId];
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