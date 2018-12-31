using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Player.ClientToServer;
using CosmicSpaceCommunication.Game.Player.ServerToClient;
using CosmicSpaceCommunication.Game.Resources;
using System.Collections.Generic;
using UnityEngine;

public class ShipLogic : MonoBehaviour
{
    public List<GameObject> Lasers => ChildInChild("Lasers");
    public List<GameObject> Gears => ChildInChild("Gears");

    [Header("Ship transform rotate/spawn")]
    public Transform RotationTransform;
    private float RotateAngle = 0;
    public Transform ModelTransform;

    [Header("Ship name")]
    public TextMesh ModelNameText;


    #region Position / New Position / Gear status
    public Vector2 Position
    {
        get => transform.position;
        set => transform.position = value;
    }
    private Vector2 targetPosition;
    public Vector2 TargetPosition
    {
        get => targetPosition;
        set
        {
            Vector2 position = new Vector2(float.Parse(value.x.ToString("F2")), float.Parse(value.y.ToString("F2")));

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

    protected bool gearsStatus;
    public bool GearsStatus
    {
        get
        {
            return gearsStatus;
        }
        set
        {
            if (value == gearsStatus)
                return;

            gearsStatus = value;

            //if (value)
            //{
            //    foreach (ParticleSystem engine in engines)
            //        engine.Play();
            //}
            //else
            //{
            //    foreach (ParticleSystem engine in engines)
            //        engine.Stop();
            //}
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

            targetGameObject = value;
            Attack = false;
            
            if (IsDead)
                return;

            if (!LocalPlayer)
                return;

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
    public bool attack;
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
    private ulong localHitpoints;
    private ulong hitpoints
    {
        get
        {
            if(LocalPlayer)
                return Client.Pilot.Hitpoints;
            return localHitpoints;
        }
        set
        {
            if (LocalPlayer)
            {
                Client.Pilot.Hitpoints = value;
                return;
            }
            localHitpoints = value;
        }
    }
    public ulong Hitpoints
    {
        get => hitpoints;
        set
        {
            var x = hitpoints;
            OnChangeHitpointsOrShields(ref x, ref value);
            hitpoints = x;
        }
    }

    private ulong maxHitpoints;
    public ulong MaxHitpoints
    {
        get => maxHitpoints;
        set
        {
            var x = maxHitpoints;
            OnChangeHitpointsOrShields(ref x, ref value);
            maxHitpoints = x;
        }
    }
    
    private ulong localShields;
    private ulong shields
    {
        get
        {
            if (LocalPlayer)
                return Client.Pilot.Shields;
            return localShields;
        }
        set
        {
            if (LocalPlayer)
            {
                Client.Pilot.Shields = value;
                return;
            }
            localShields = value;
        }
    }
    public ulong Shields
    {
        get => shields;
        set
        {
            var x = shields;
            OnChangeHitpointsOrShields(ref x, ref value);
            shields = x;
        }
    }

    private ulong maxShields;
    public ulong MaxShields
    {
        get => maxShields;
        set
        {
            var x = maxShields;
            OnChangeHitpointsOrShields(ref x, ref value);
            maxShields = x;
        }
    }
    
    private void OnChangeHitpointsOrShields(ref ulong variable, ref ulong value)
    {
        variable = value;
        
        GuiScript.RefreshAllActiveWindow();
    }
    #endregion



    public bool LocalPlayer = false;

    #region Player / IsDead, KillerBy / Nickname / Ship / Speed
    public PlayerJoin Player;

    public bool IsDead
    {
        get => Player.IsDead;
        set
        {
            Player.IsDead = value;

            TargetGameObject = null;
            Attack = false;

            if(LocalPlayer)
            {
                Client.Pilot.IsDead = value;

                if (value)
                    GuiScript.OpenWindow(WindowTypes.RepairShip);
                else
                {
                    Player.KillerBy = string.Empty;
                    GuiScript.CloseWindow(WindowTypes.RepairShip);
                }
            }
        }
    }
    public string KillerBy => Player.KillerBy;
    public string Nickname => Player.Nickname;
    public Ship Ship => Player.Ship;
    public int Speed => Player.Speed;
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
        Player = player;
        LocalPlayer = localPlayer;

        IsDead = player.IsDead;
        
        transform.position = new Vector2(player.PositionX, player.PositionY);
        TargetPosition = new Vector2(player.TargetPositionX, player.TargetPositionY);

        transform.name = player.PlayerId.ToString();
        
        ModelNameText.text = player.Nickname;
        ModelNameText.color = nameColor;

        foreach (Transform t in ModelTransform)
        {
            Destroy(t.gameObject);
        }

        Instantiate(Resources.Load<GameObject>("Ships/" + player.Ship.Name), ModelTransform);
    }
    
    private List<GameObject> ChildInChild(string parent)
    {
        foreach (Transform child in transform)
        {
            if (child.name == parent)
            {
                List<GameObject> objects = new List<GameObject>();
                foreach (Transform t in child)
                {
                    objects.Add(t.gameObject);
                }
                return objects;
            }
        }
        return null;
    }
}