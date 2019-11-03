using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Enemy;
using CosmicSpaceCommunication.Game.Player;
using CosmicSpaceCommunication.Game.Player.ClientToServer;
using CosmicSpaceCommunication.Game.Player.ServerToClient;
using CosmicSpaceCommunication.Game.Resources;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    public static bool DebugMode = false;

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

    [Header("Explosions")]
    public List<GameObject> ExplosionsGameObject = new List<GameObject>();

    [Header("Blasters")]
    public List<GameObject> BlastersGameObject = new List<GameObject>();

    [Header("Portals")]
    public Transform PortalTransform;


    private void Start()
    {
        ClearGameArea();

        LoadExplosions();
        LoadBlasters();

#if DEBUG
        //DebugMode = true;
#endif
    }

    private void Update()
    {
        if (Client.Pilot == null || LocalShipController == null)
            return;

        MouseControl();
        KeyboardControl();

        PlayerNewPosition();
    }

    #region Mouse / Keyboard
    public static Vector2 TargetPosition;
    float timer = 0;
    private void PlayerNewPosition()
    {
        timer += Time.deltaTime;
        if (timer >= 0.1f)
        {
            timer = 0;
            LocalShipController.TargetPosition = TargetPosition;
        }
    }

    private void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.GetMouseButtonDown(0))
                MouseController();
            else if (Input.GetMouseButton(0))
                MouseController(false);
        }
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

            TargetPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z * -1));
        }
    }

    private void KeyboardControl()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            if (Client.Pilot.Resources[Client.Pilot.AmmunitionId].Count < Client.Pilot.Items.Where(o => o.Item.ItemType == ItemTypes.Laser && o.IsEquipped).Count())
            {
                GuiScript.CreateLogMessage(new List<string>() { GameSettings.UserLanguage.NO_AMMO });
                return;
            }

            if (Client.Pilot.Items.Where(o => o.IsEquipped).Count() == 0)
            {
                GuiScript.CreateLogMessage(new List<string>() { GameSettings.UserLanguage.NO_EQUIP });
                return;
            }

            if (!LocalShipController.TargetIsNull)
            {
                if (!LocalShipController.TargetIsCover)
                    LocalShipController.Attack = !LocalShipController.Attack;
                else
                    GuiScript.CreateLogMessage(new List<string>() { GameSettings.UserLanguage.TARGET_IS_COVER });
            }
            else
                GuiScript.CreateLogMessage(new List<string>() { GameSettings.UserLanguage.TARGET_NOT_FOUND });
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            Portal portal = null;
            foreach (Transform p in PortalTransform.transform)
            {
                if (Vector2.Distance(LocalShipController.Position, p.position) < 15)
                {
                    portal = p.GetComponent<ClientPortal>().Portal;
                    break;
                }
            }

            if (portal != null && Client.Pilot.Map.Id == portal.Map.Id)
            {
                GuiScript.CreateLogMessage(new List<string>() { string.Format(GameSettings.UserLanguage.PORTAL_FOUND, portal.TargetMap.Name) });

                Client.SendToSocket(new CommandData()
                {
                    Command = Commands.ChangeMap,
                    SenderId = Client.Pilot.Id,
                    Data = new PlayerChangeMap()
                    {
                        Portal = portal
                    }
                });
            }
            else
                GuiScript.CreateLogMessage(new List<string>() { GameSettings.UserLanguage.PORTAL_NOT_FOUND });
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
            ChangeAmmunition(100);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangeAmmunition(101);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangeAmmunition(102);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            ChangeAmmunition(103);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            ChangeRocket(104);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            ChangeRocket(105);
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            ChangeRocket(106);
    }

    private void ChangeAmmunition(int key)
    {
        if (Client.Pilot.AmmunitionId == key)
            return;

        SwitchAmmoType(key, Client.Pilot.RocketId);
    }
    private void ChangeRocket(int key)
    {
        if (Client.Pilot.RocketId == key)
            return;

        SwitchAmmoType(Client.Pilot.AmmunitionId, key);
    }
    private void SwitchAmmoType(long ammunitionId, long rocketId)
    {
        if (!Client.Pilot.Resources.ContainsKey(ammunitionId) || !Client.Pilot.Resources.ContainsKey(rocketId))
            return;

        if (!Client.Pilot.ServerResources.ContainsKey(ammunitionId) || !Client.Pilot.ServerResources.ContainsKey(rocketId))
            return;

        Client.SendToSocket(new CommandData()
        {
            Command = Commands.ChangeAmmunition,
            SenderId = Client.Pilot.Id,
            Data = new ChangeAmmunition()
            {
                SelectedAmmunitionId = ammunitionId,
                SelectedRocketId = rocketId
            }
        });
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

        GuiScript.CreateLogMessage(new List<string>() { "Hello world!" });

        CreateBackground(Client.Pilot.Map, BackgroundTransform, PortalTransform, StarsGameObject);
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

        if (DebugMode)
            GuiScript.CreateLogMessage(new List<string>() { $"InitPlayer '{player.Nickname} [ID:{player.PlayerId}]' ship:'{player.Ship.Name} [ID:{player.Ship.Id}]'" });

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

        //if (DebugMode)
        //    GuiScript.CreateLogMessage(new List<string>() { $"InitEnemy '{enemy.ParentEnemy.Name} [ID:{enemy.ParentEnemy.Id}]' aggresive:'{enemy.ParentEnemy.IsAggressive}' shot_range:'{enemy.ParentEnemy.ShotDistance}'" });
    }

    public void LeaveEnemy(ulong enemyId)
    {
        if (!EnemiesController.ContainsKey(enemyId))
            return;

        Destroy(EnemiesController[enemyId]?.gameObject);
        EnemiesController.Remove(enemyId);
    }
    #endregion



    public static void CreateBackground(Map map, Transform backgroundTransform, Transform portalTransform, GameObject starsPrefab)
    {
        #region Background
        DestroyChilds(backgroundTransform);

        // Create stars - settings
        if (true && starsPrefab != null)
        {
            GameObject stars = Instantiate(starsPrefab, backgroundTransform);
            #pragma warning disable CS0618 // Type or member is obsolete
            stars.GetComponent<ParticleSystem>().maxParticles = 2000;
            #pragma warning restore CS0618 // Type or member is obsolete
        }

        // Create background - settings
        if (true)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>($"Maps/{map.Name}"), backgroundTransform);
            go.name = map.Name;
        }
        #endregion

        #region Portals
        DestroyChilds(portalTransform);

        foreach (Portal portal in map.Portals)
        {
            GameObject p = Instantiate(Resources.Load<GameObject>($"{portal.PrefabTypeName}/{portal.PrefabName}"), portalTransform);

            p.AddComponent<ClientPortal>().Portal = portal;
        }
        #endregion

        if (DebugMode)
            GuiScript.CreateLogMessage(new List<string>() { $"CreateBackground map:'{map.Name} [ID:{map.Id}]' pvp:'{map.IsPvp}' level:'{map.RequiredLevel}'" });
    }

    private void LoadExplosions()
    {
        ExplosionsGameObject = new List<GameObject>();
        ExplosionsGameObject.AddRange(Resources.LoadAll<GameObject>("Explosions"));
    }

    private void LoadBlasters()
    {
        BlastersGameObject = new List<GameObject>();
        BlastersGameObject.AddRange(Resources.LoadAll<GameObject>("Bullets"));
    }
    
    private void CreateExplosion(Vector3 position)
    {
        GameObject explosion = Instantiate(ExplosionsGameObject[Random.Range(0, ExplosionsGameObject.Count - 1)], position, Quaternion.identity);
        explosion.AddComponent<ParticleSystemAutoDestroy>();
        explosion.transform.localScale = new Vector3(3, 3, 3);
    }



    #region Events
    public void ChangePosition(NewPosition newPosition)
    {
        ShipLogic shipLogic = newPosition.IsPlayer ? FindPilot(newPosition.PlayerId) : FindEnemy(newPosition.PlayerId);

        if (shipLogic == null)
            return;

        if (!shipLogic.LocalPlayer)
        {
            shipLogic.Position = new Vector2(newPosition.PositionX, newPosition.PositionY);
            shipLogic.TargetPosition = new Vector2(newPosition.TargetPositionX, newPosition.TargetPositionY);
        }
        shipLogic.Speed = newPosition.Speed;
    }

    public void HitpointsOrShields(NewHitpointsOrShields newValue, bool hitpoints)
    {
        ShipLogic shipLogic = newValue.IsPlayer ? FindPilot(newValue.PlayerId) : FindEnemy(newValue.PlayerId);
        
        if (shipLogic == null)
            return;

        if (hitpoints)
        {
            shipLogic.MaxHitpoints = newValue.MaxValue;
            shipLogic.Hitpoints = newValue.Value;
        }
        else
        {
            shipLogic.MaxShields = newValue.MaxValue;
            shipLogic.Shields = newValue.Value;
        }
    }

    public ShipLogic SelectTarget(NewTarget newTarget)
    {
        ShipLogic targetShipLogic = null;
        if (newTarget.TargetIsPlayer != null)
        {
            targetShipLogic = newTarget.TargetIsPlayer == true ? FindPilot(newTarget.TargetId) : FindEnemy(newTarget.TargetId);
        }

        ShipLogic attackerShipLogic = newTarget.AttackerIsPlayer == true ? FindPilot(newTarget.PlayerId) : FindEnemy(newTarget.PlayerId);

        if (attackerShipLogic == null)
            return null;

        if (DebugMode)
            GuiScript.CreateLogMessage(new List<string>() { $"SelectTarget '{GuiScript.IsPlayer(targetShipLogic, newTarget.AttackerIsPlayer)}' by '{attackerShipLogic.name} {GuiScript.IsPlayer(attackerShipLogic, newTarget.TargetIsPlayer == true)}'" });

        attackerShipLogic.TargetGameObject = targetShipLogic?.gameObject;

        return attackerShipLogic;
    }

    public void AttackTarget(AttackTarget attackTarget)
    {
        ShipLogic shipLogic = SelectTarget(attackTarget);

        if (shipLogic == null)
            return;

        if (DebugMode)
            GuiScript.CreateLogMessage(new List<string>() { $"AttackTarget '{GuiScript.IsPlayer(shipLogic, attackTarget.TargetIsPlayer == true)}' [{attackTarget.Attack}]" });

        if (attackTarget.TargetIsPlayer == null)
            shipLogic.Attack = false;
        else
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

        if (DebugMode)
            GuiScript.CreateLogMessage(new List<string>() { $"SomeoneTakeDamage '{GuiScript.IsPlayer(fromShipLogic, takeDamage.FromIsPlayer)}' shot to '{GuiScript.IsPlayer(toShipLogic, takeDamage.ToIsPlayer)}' [{takeDamage.Damage}]" });

        fromShipLogic.TargetGameObject = toShipLogic.gameObject;
        fromShipLogic.Attack = true;

        int blasterId = takeDamage.AmmunitionId - 100;
        if (blasterId < BlastersGameObject.Count)
            fromShipLogic.ShotToTarget(takeDamage.Damage, BlastersGameObject[blasterId]);
        else
            Debug.Log($"Brak lasera: {blasterId}");
    }

    public void SomeoneDead(SomeoneDead someoneDead)
    {
        ShipLogic whoShipLogic = someoneDead.WhoIsPlayer == true ? FindPilot(someoneDead.WhoId) : FindEnemy(someoneDead.WhoId);

        if (whoShipLogic == null)
            return;

        List<ShipLogic> byShipLogic = someoneDead.Killers.Select(o => o.IsPlayer ? FindPilot(o.Id) : FindEnemy(o.Id)).ToList();

        whoShipLogic.IsDead = true;
        whoShipLogic.KillerBy = someoneDead.KillersToString;
        CreateExplosion(whoShipLogic.transform.position);

        if (DebugMode)
            GuiScript.CreateLogMessage(new List<string>() { $"SomeoneDead '{GuiScript.IsPlayer(whoShipLogic, someoneDead.WhoIsPlayer)}' by '{byShipLogic.Count}'" });

        UnselectDeadTarget(PlayersController, whoShipLogic);
        UnselectDeadTarget(EnemiesController, whoShipLogic);
    }

    private void UnselectDeadTarget(Dictionary<ulong, ShipLogic> list, ShipLogic whoDead)
    {
        foreach (ShipLogic shipLogic in list.Values)
        {
            if (shipLogic.TargetGameObject == whoDead.gameObject)
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

        if (DebugMode)
            GuiScript.CreateLogMessage(new List<string>() { $"SomeoneAlive '{GuiScript.IsPlayer(whoShipLogic, true)}'" });

        whoShipLogic.IsDead = false;
    }

    public void TakeReward(ServerReward reward)
    {
        List<string> messages = new List<string>();

        switch(reward.Reason)
        {
            case RewardReasons.KillPlayer:
                messages.Add(string.Format(GameSettings.UserLanguage.DEFEATED_PLAYER, reward.Data));
                break;
            case RewardReasons.KillEnemy:
                messages.Add(string.Format(GameSettings.UserLanguage.DEFEATED_ENEMY, reward.Data));
                break;
            case RewardReasons.SellItem:
                messages.Add(string.Format(GameSettings.UserLanguage.ITEM_SOLD, reward.Data));
                break;
            case RewardReasons.BuyItem:
                messages.Add(string.Format(GameSettings.UserLanguage.ITEM_PURCHASED, reward.Data));
                break;
            case RewardReasons.CompleteQuest:
                messages.Add(string.Format(GameSettings.UserLanguage.TASK_COMPLETED, reward.Data));
                break;

            default:
                Debug.LogError(nameof(System.NotImplementedException));
                break;
        }

        if (reward.Experience != null)
        {
            Client.Pilot.Experience += (ulong)reward.Experience;
            messages.Add($"{GameSettings.UserLanguage.EXPERIENCE} {reward.Experience}");
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
        if (reward.AmmunitionId != null && reward.AmmunitionQuantity > 0)
        {
            Client.Pilot.Resources[(int)reward.AmmunitionId].Count += (long)reward.AmmunitionQuantity;
            messages.Add(string.Format(GameSettings.UserLanguage.RECEIVE_RESOURCE, reward.AmmunitionId, reward.AmmunitionQuantity));
        }

        if (reward.Items?.Count > 0)
        {
            foreach (ItemPilot item in reward.PilotItems)
            {
                messages.Add(string.Format(GameSettings.UserLanguage.RECEIVE_ITEM, item.Item.Name));
            }

            Client.Pilot.Items.AddRange(reward.PilotItems);

            if (GuiScript.Windows[WindowTypes.HangarWindow].Active)
                GuiScript.Windows[WindowTypes.HangarWindow].Script.Refresh();
        }

        GuiScript.CreateLogMessage(messages, 6);

        MainThread.Instance().Enqueue(() => GuiScript.Windows[WindowTypes.UserInterface].Script.Refresh());
    }

    public void ChangeMap(Pilot pilot)
    {
        Client.Pilot.Map = pilot.Map;

        LocalShipController.Position = LocalShipController.TargetPosition = TargetPosition = new Vector2(pilot.PositionX, pilot.PositionY);

        CreateBackground(pilot.Map, BackgroundTransform, PortalTransform, StarsGameObject);
    }

    public void SafeZone(SafeZone safeZone)
    {
        ShipLogic whoShipLogic = safeZone.IsPlayer == true ? FindPilot(safeZone.PilotId) : FindEnemy(safeZone.PilotId);

        if (whoShipLogic == null)
            return;

        whoShipLogic.IsCover = safeZone.Status;

        foreach (ShipLogic ship in PlayersController.Values)
        {
            if(!ship.TargetIsNull && ship.TargetGameObject == whoShipLogic.gameObject)
            {
                ship.Attack = false;
                if (ship.ID == Client.Pilot.Id)
                    GuiScript.CreateLogMessage(new List<string>() { GameSettings.UserLanguage.TARGET_IS_COVER });
            }
        }
        foreach (ShipLogic ship in EnemiesController.Values)
        {
            if (!ship.TargetIsNull && ship.TargetGameObject == whoShipLogic.gameObject)
            {
                ship.Attack = false;
            }
        }
    }

    public void SomeoneChangeShip(ulong pilotId, Ship ship)
    {
        ShipLogic player = FindPilot(pilotId);

        if (player == null)
            return;

        if (pilotId == Client.Pilot.Id)
            Client.Pilot.Ship = ship;

        player.InitShip(ship.Prefab);
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



    public void ClearGameArea(bool localPlayer = true)
    {
        for (int i = localPlayer ? 2 : 3; i < transform.childCount; i++)
        {
            foreach (Transform t in transform.GetChild(i))
                Destroy(t.gameObject);
        }

        PlayersController.Clear();
        EnemiesController.Clear();

        if(!localPlayer)
            PlayersController.Add(Client.Pilot.Id, LocalShipController);
    }

    public static void DestroyChilds(Transform t)
    {
        foreach (Transform child in t)
            Destroy(child.gameObject);
    }
}