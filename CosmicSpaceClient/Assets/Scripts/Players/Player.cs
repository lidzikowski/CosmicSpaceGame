using CosmicSpaceCommunication.Game.Enemy;
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
    public List<GameObject> MapsGameObject = new List<GameObject>();

    [Header("Explosions")]
    public List<GameObject> ExplosionsGameObject = new List<GameObject>();

    [Header("Blasters")]
    public List<GameObject> BlastersGameObject = new List<GameObject>();
    


    private void Start()
    {
        ClearGameArea();

        LoadExplosions();
        LoadBlasters();

        #if DEBUG
        DebugMode = true;
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

            TargetPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z * -1));
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

        GuiScript.CreateLogMessage(new List<string>() { "Hello world!" });

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

        if (DebugMode)
            GuiScript.CreateLogMessage(new List<string>() { $"CreateBackground map:'{map.Name} [ID:{map.Id}]' pvp:'{map.IsPvp}' level:'{map.RequiredLevel}'" });
    }

    private void LoadExplosions()
    {
        ExplosionsGameObject = new List<GameObject>();
        ExplosionsGameObject.AddRange(Resources.LoadAll<GameObject>("100ExplosionPack/Prefabs"));
    }

    private void LoadBlasters()
    {
        BlastersGameObject = new List<GameObject>();
        BlastersGameObject.AddRange(Resources.LoadAll<GameObject>("Blasters/Prefabs/BulletsWithFlySFX"));
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

        ShipLogic targetShipLogic = newTarget.TargetIsPlayer == true ? FindPilot(newTarget.TargetId) : FindEnemy(newTarget.TargetId);

        if (targetShipLogic == null)
            return null;
        
        ShipLogic attackerShipLogic = newTarget.AttackerIsPlayer == true ? FindPilot(newTarget.PlayerId) : FindEnemy(newTarget.PlayerId);

        if (attackerShipLogic == null)
            return null;

        if (DebugMode)
            GuiScript.CreateLogMessage(new List<string>() { $"SelectTarget '{GuiScript.IsPlayer(targetShipLogic, newTarget.AttackerIsPlayer)}' by '{attackerShipLogic.name} {GuiScript.IsPlayer(attackerShipLogic, newTarget.TargetIsPlayer == true)}'" });

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

        if (DebugMode)
            GuiScript.CreateLogMessage(new List<string>() { $"AttackTarget '{GuiScript.IsPlayer(shipLogic, attackTarget.TargetIsPlayer == true)}' [{attackTarget.Attack}]" });

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
        fromShipLogic.ShotToTarget(takeDamage.Damage, BlastersGameObject[Random.Range(0, BlastersGameObject.Count - 1)]);
    }

    public void SomeoneDead(SomeoneDead someoneDead)
    {
        ShipLogic whoShipLogic = someoneDead.WhoIsPlayer == true ? FindPilot(someoneDead.WhoId) : FindEnemy(someoneDead.WhoId);

        if (whoShipLogic == null)
            return;

        ShipLogic byShipLogic = someoneDead.ByIsPlayer == true ? FindPilot(someoneDead.ById) : FindEnemy(someoneDead.ById);

        whoShipLogic.IsDead = true;
        CreateExplosion(whoShipLogic.transform.position);

        if (DebugMode)
            GuiScript.CreateLogMessage(new List<string>() { $"SomeoneDead '{GuiScript.IsPlayer(whoShipLogic, someoneDead.WhoIsPlayer)}' by '{GuiScript.IsPlayer(byShipLogic, someoneDead.ByIsPlayer)}'" });

        foreach (ShipLogic shipLogic in PlayersController.Values)
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
        }

        if (reward.Experience != null)
        {
            Client.Pilot.Experience += (ulong)reward.Experience;
            messages.Add(string.Format(GameSettings.UserLanguage.EXPERIENCE, reward.Experience));
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

        GuiScript.CreateLogMessage(messages, 6);

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
        for (int i = 1; i < transform.childCount; i++)
        {
            foreach (Transform t in transform.GetChild(i))
                Destroy(t.gameObject);
        }

        PlayersController.Clear();
        EnemiesController.Clear();
    }
}