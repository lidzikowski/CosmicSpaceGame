using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game;
using CosmicSpaceCommunication.Game.Enemy;
using CosmicSpaceCommunication.Game.Interfaces;
using CosmicSpaceCommunication.Game.Player.ClientToServer;
using CosmicSpaceCommunication.Game.Player.ServerToClient;
using System.Collections.Generic;
using UnityEngine;

public class ShipLogic : MonoBehaviour
{
    public ulong ID;
    public string ObjectName;

    public List<Transform> Lasers => ChildInChild("Lasers");
    public List<Transform> Gears => ChildInChild("Gears");

    [Header("Ship transform rotate/spawn")]
    public Transform RotationTransform;
    private float RotateAngle = 0;
    public Transform ModelTransform;

    [Header("Ship name")]
    public TextMesh ModelNameText;
    public MeshRenderer PointerMeshRenderer;

    [Header("Hitpoints and shields")]
    private bool targetInterface = false;
    public bool TargetInterface
    {
        get => targetInterface;
        set
        {
            if (targetInterface == value)
                return;

            targetInterface = value;

            ShowTargetInterface(value);
        }
    }
    public GameObject TargetInterfaceGameObject;
    public TextMesh HitpointsText;
    public TextMesh ShieldsText;
    
    public void ShotToTarget(long? damage, GameObject bullet)
    {
        if (TargetIsNull)
            return;

        if (damage == null)
            return;

        long dmg = (long)damage;

        foreach (Transform t in Lasers)
        {
            GameObject go = Instantiate(bullet, t.position, Quaternion.identity);
            go.AddComponent<BulletLogic>().TargetGameObject = TargetGameObject;
        }

        Debug.Log(dmg);
    }

    

    private void Start()
    {
        TargetInterface = LocalPlayer;
    }



    public void ShowTargetInterface(bool status)
    {
        TargetInterfaceGameObject.SetActive(status);
        if (status)
        {
            HitpointsText.text = string.Format("{0}/{1}", Hitpoints, MaxHitpoints);
            ShieldsText.text = string.Format("{0}/{1}", Shields, MaxShields);
        }
    }



    #region Position / New Position / Gear status
    public Vector2 Position
    {
        get => transform.position;
        set
        {
            transform.position = value;

            if (!LocalPlayer)
                return;

            Client.Pilot.PositionX = value.x;
            Client.Pilot.PositionY = value.y;
            
            GuiScript.RefreshAllActiveWindow();
        }
    }
    private Vector2 targetPosition;
    public Vector2 TargetPosition
    {
        get => targetPosition;
        set
        {
            Vector2 position = new Vector2((int)value.x, (int)value.y);

            if (targetPosition == position)
                return;

            targetPosition = position;

            if (!LocalPlayer)
                return;

            if (IsDead)
                return;

            Client.SendToSocket(new CommandData()
            {
                Command = Commands.NewPosition,
                Data = new NewPosition()
                {
                    PlayerId = Client.Pilot.Id,
                    IsPlayer = true,
                    PositionX = position.x,
                    PositionY = position.y
                }
            });
        }
    }

    protected bool gearsStatus = true;
    public bool GearsStatus
    {
        get
        {
            return gearsStatus;
        }
        set
        {
            gearsStatus = value;

            if (value)
            {
                foreach (Transform t in Gears)
                    t.GetComponent<ParticleSystem>().Play();
            }
            else
            {
                foreach (Transform t in Gears)
                    t.GetComponent<ParticleSystem>().Stop();
            }
        }
    }
    #endregion

    #region Target
    private GameObject targetGameObject;
    public bool TargetIsNull => targetGameObject == null;
    private bool? TargetIsPlayer => !TargetIsNull ? targetGameObject.tag == "Player" : (bool?)null;
    private ulong? TargetId => !TargetIsNull ? ulong.Parse(targetGameObject.name) : (ulong?)null;
    public GameObject TargetGameObject
    {
        get => targetGameObject;
        set
        {
            if (targetGameObject == value)
                return;

            GameObject oldTarget = targetGameObject;
            targetGameObject = value;
            Attack = false;
            
            if (IsDead)
                return;

            if (!LocalPlayer)
                return;

            if (oldTarget != null)
                oldTarget.GetComponent<ShipLogic>().TargetInterface = false;

            if (value != null)
                value.GetComponent<ShipLogic>().TargetInterface = true;

            Client.SendToSocket(new CommandData()
            {
                Command = Commands.SelectTarget,
                Data = new NewTarget()
                {
                    PlayerId = Client.Pilot.Id,
                    AttackerIsPlayer = true,

                    TargetId = TargetId,
                    TargetIsPlayer = TargetIsPlayer,
                }
            });
        }
    }
    #endregion

    #region Attack
    private bool attack;
    public bool Attack
    {
        get => attack;
        set
        {
            if (attack == value)
                return;

            attack = value;
            
            if (TargetIsNull)
                attack = false;

            if (IsDead)
                return;

            if (!LocalPlayer)
                return;

            Client.SendToSocket(new CommandData()
            {
                Command = Commands.AttackTarget,
                Data = new AttackTarget()
                {
                    PlayerId = Client.Pilot.Id,
                    AttackerIsPlayer = true,

                    TargetId = TargetId,
                    TargetIsPlayer = TargetIsPlayer,

                    Attack = value,

                    SelectedAmmunition = 0,
                    SelectedRocket = null,
                }
            });
        }
    }
    #endregion

    #region Hitpoints / Shields
    private long hitpoints;
    public long Hitpoints
    {
        get => hitpoints;
        set
        {
            var x = hitpoints;
            OnChangeHitpointsOrShields(ref x, ref value);
            hitpoints = x;
        }
    }

    private long maxHitpoints;
    public long MaxHitpoints
    {
        get => maxHitpoints;
        set
        {
            var x = maxHitpoints;
            OnChangeHitpointsOrShields(ref x, ref value);
            maxHitpoints = x;
        }
    }

    private long shields;
    public long Shields
    {
        get => shields;
        set
        {
            var x = shields;
            OnChangeHitpointsOrShields(ref x, ref value);
            shields = x;
        }
    }

    private long maxShields;
    public long MaxShields
    {
        get => maxShields;
        set
        {
            var x = maxShields;
            OnChangeHitpointsOrShields(ref x, ref value);
            maxShields = x;
        }
    }
    
    private void OnChangeHitpointsOrShields(ref long variable, ref long value)
    {
        variable = value;

        ShowTargetInterface(TargetInterface);

        if (!LocalPlayer)
            return;

        if (Player.LocalShipController != null)
            GuiScript.RefreshAllActiveWindow();
    }
    #endregion

    #region IsCover
    private bool isCover;
    public bool IsCover
    {
        get => isCover;
        set
        {
            if (isCover == value)
                return;

            isCover = value;

            if (!LocalPlayer)
                return;

            UserInterfaceWindow script = (GuiScript.Windows[WindowTypes.UserInterface]?.Script as UserInterfaceWindow);
            if (value)
                script.SafeZoneText.text = GameSettings.UserLanguage.SAFE_ZONE_ACTIVE;
            else
                StartCoroutine(HideSafeZone(script));
        }
    }
    private System.Collections.IEnumerator HideSafeZone(UserInterfaceWindow script)
    {
        script.SafeZoneText.text = GameSettings.UserLanguage.SAFE_ZONE_INACTIVE;
        yield return new WaitForSeconds(4);
        if (!IsCover)
            script.SafeZoneText.text = string.Empty;
    }
    #endregion

    #region LocalPlayer
    private bool localPlayer = false;
    public bool LocalPlayer
    {
        get => localPlayer;
        set
        {
            localPlayer = value;

            if(value)
            {
                tag = "LocalPlayer";
                PointerMeshRenderer.gameObject.layer = 10;
                PointerMeshRenderer.material = Resources.Load<Material>("MapPointers/White");
            }
            else
            {
                tag = "Player";
                PointerMeshRenderer.gameObject.layer = 11;
                PointerMeshRenderer.material = Resources.Load<Material>("MapPointers/Blue");
            }
        }
    }
    #endregion

    #region IsDead / KillerBy / Nickname / Ship / Speed
    private bool isDead = false;
    public bool IsDead
    {
        get => isDead;
        set
        {
            isDead = value;

            TargetGameObject = null;
            Attack = false;

            if (!LocalPlayer)
                return;

            Client.Pilot.IsDead = value;

            if (value)
                GuiScript.OpenWindow(WindowTypes.RepairShip);
            else
            {
                KillerBy = string.Empty;
                GuiScript.CloseWindow(WindowTypes.RepairShip);
            }
        }
    }
    public string KillerBy { get; set; }
    public float Speed { get; set; }
    #endregion



    private void Update()
    {
        if (IsDead)
            return;

        Rotate();
        Fly();
    }



    public void Rotate()
    {
        if (!TargetIsNull && Attack)
            RotateAngle = Mathf.Atan2(TargetGameObject.transform.position.y - Position.y, TargetGameObject.transform.position.x - Position.x) * Mathf.Rad2Deg + 180;
        else if (TargetPosition != Position)
        {
            float angle = Mathf.Atan2(TargetPosition.y - Position.y, TargetPosition.x - Position.x);
            if (angle == 0)
                return;
            RotateAngle = angle * Mathf.Rad2Deg + 180;
        }

        RotationTransform.rotation = Quaternion.Lerp(RotationTransform.rotation, Quaternion.Euler(RotateAngle, -90, 90), Time.deltaTime * 10);
    }

    public void Fly()
    {
        if (TargetPosition != Position)
        {
            Position = Vector3.MoveTowards(Position, TargetPosition, Time.deltaTime * Speed);
            if (!GearsStatus)
                GearsStatus = true;
        }
        else if (GearsStatus)
            GearsStatus = false;
    }



    public void InitShip(PlayerJoin player, Color nameColor, bool localPlayer)
    {
        InitPosition(player);
        InitHitpointsShields(player);
        InitName(player.Nickname, player.PlayerId, player.Ship, nameColor);

        LocalPlayer = localPlayer;
        IsDead = player.IsDead;
        KillerBy = player.KillerBy;
    }
    
    public void InitShip(EnemyJoin enemy, Color nameColor)
    {
        InitPosition(enemy);
        InitHitpointsShields(enemy);
        InitName(enemy.ParentEnemy.Name, enemy.Id, enemy.ParentEnemy, nameColor);

        PointerMeshRenderer.gameObject.layer = 12;
        PointerMeshRenderer.material = Resources.Load<Material>("MapPointers/Red");
    }

    private void InitPosition(NewPosition newPosition)
    {
        transform.position = new Vector2(newPosition.PositionX, newPosition.PositionY);
        TargetPosition = new Vector2(newPosition.TargetPositionX, newPosition.TargetPositionY);

        transform.name = newPosition.PlayerId.ToString();
        tag = newPosition.IsPlayer ? "Player" : "Enemy";
        Speed = newPosition.Speed;
    }

    private void InitHitpointsShields(HitpointsShields hitpointsShields)
    {
        Hitpoints = hitpointsShields.Hitpoints;
        MaxHitpoints = hitpointsShields.MaxHitpoints;
        Shields = hitpointsShields.Shields;
        MaxShields = hitpointsShields.MaxShields;
    }

    private void InitShip(IPrefab prefab)
    {
        foreach (Transform t in ModelTransform)
        {
            Destroy(t.gameObject);
        }

        Instantiate(Resources.Load<GameObject>($"{prefab.PrefabTypeName}/{prefab.PrefabName}"), ModelTransform);

        //if (Player.DebugMode)
        //    GuiScript.CreateLogMessage(new List<string>() { $"InitShip '{ship.Name} [{ship.Id}]' ship_type:'{ship.PrefabName} [{ship.PrefabId}]'" });
    }

    private void InitName(string name, ulong id, IShip ship, Color color)
    {
        ID = id;

        InitShip(ship.Prefab);

        ObjectName = name;
        ModelNameText.text = ObjectName;
        ModelNameText.color = color;

        if (Player.DebugMode)
        {
            ModelNameText.text = $"{ObjectName} [ID:{ID}]";
            ModelNameText.text += $"\n({ship.Prefab.PrefabName} [ID:{ship.Id}])";
        }
    }



    private List<Transform> ChildInChild(string parent)
    {
        foreach (Transform child in ModelTransform.GetChild(0))
        {
            if (child.name == parent)
            {
                List<Transform> objects = new List<Transform>();
                foreach (Transform t in child)
                {
                    objects.Add(t);
                }
                return objects;
            }
        }
        return null;
    }
}