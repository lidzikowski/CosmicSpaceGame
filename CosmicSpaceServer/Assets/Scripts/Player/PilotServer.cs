using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game;
using CosmicSpaceCommunication.Game.Player;
using CosmicSpaceCommunication.Game.Resources;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using WebSocketSharp.Server;

public class PilotServer : Opponent
{
    public PilotServer(Pilot pilot)
    {
        Pilot = pilot;

        NewPostion = Position;
        CalculateStatistics();
    }

    public void ItemsChange(List<ItemPilot> items)
    {
        foreach (ItemPilot item in items)
        {
            ItemPilot localItem = item.RelationId > 0 ? Pilot.Items.FirstOrDefault(o => o.RelationId == item.RelationId && o.Item.ItemType == item.Item.ItemType) : null;

            if (localItem == null)
            {
                Pilot.Items.Add(item);
            }
            else
            {
                if (item.IsEquipped)
                {
                    switch (localItem.Item.ItemType)
                    {
                        case ItemTypes.Laser:
                            if (GetItemsLasers.Count < Pilot.Ship.Lasers)
                                localItem.IsEquipped = true;
                            break;
                        case ItemTypes.Generator:
                            if (GetItemsGenerators.Count < Pilot.Ship.Generators)
                                localItem.IsEquipped = true;
                            break;
                        case ItemTypes.Extra:
                            if (GetItemsExtras.Count < Pilot.Ship.Extras)
                                localItem.IsEquipped = true;
                            break;
                    }
                }
                else
                {
                    localItem.IsEquipped = false;
                }
            }
        }

        //localItem.IsSold = newItem.IsSold; //server
        //localItem.UpgradeLevel = newItem.UpgradeLevel; //server

        CalculateStatistics();
    }



    #region Pilot / Id / Name / isDead
    public Pilot Pilot { get; set; }

    public override ulong Id
    {
        get => Pilot.Id;
        protected set { }
    }
    public override string Name => Pilot.Nickname;
    protected override bool isDead
    {
        get => Pilot.IsDead;
        set
        {
            if (Pilot.IsDead.Equals(value))
                return;

            Pilot.IsDead = value;

            if(value)
                Pilot.KillerBy = DeadOpponent?.Name;
            else
                Pilot.KillerBy = string.Empty;
        }
    }
    #endregion
    
    #region Socket gracza / Wyslanie danych przy inicjalizacji
    private Headers headers;
    public Headers Headers
    {
        get => headers;
        set
        {
            if (headers == value)
                return;

            headers = value;

            if (value == null)
                return;

            Send(new CommandData()
            {
                Command = Commands.UserData,
                Data = Pilot
            });
        }
    }

    public Headers ChatHeaders;
    #endregion



    #region Position
    protected override Vector2 position
    {
        get => new Vector2(Pilot.PositionX, Pilot.PositionY);
        set
        {
            if (!Pilot.PositionX.Equals(value.x))
                Pilot.PositionX = value.x;

            if (!Pilot.PositionY.Equals(value.y))
                Pilot.PositionY = value.y;
        }
    }
    #endregion



    #region Hitpoints / MaxHitpoints
    protected override long hitpoints
    {
        get => Pilot.Hitpoints;
        set
        {
            if (Pilot.Hitpoints == value)
                return;

            Pilot.Hitpoints = value;
        }
    }
    protected override long maxHitpoints
    {
        get => Pilot.MaxHitpoints;
        set
        {
            if (Pilot.MaxHitpoints == value)
                return;

            Pilot.MaxHitpoints = value;
        }
    }
    #endregion

    #region Shields / MaxShields
    protected override long shields
    {
        get => Pilot.Shields;
        set
        {
            if (Pilot.Shields == value)
                return;

            Pilot.Shields = value;
        }
    }
    protected override long maxShields
    {
        get => Pilot.MaxShields;
        set
        {
            if (Pilot.MaxShields == value)
                return;

            Pilot.MaxShields = value;
        }
    }
    #endregion

    #region Speed
    protected override float speed
    {
        get => Pilot.Speed;
        set
        {
            if (Pilot.Speed == value)
                return;

            Pilot.Speed = value;
        }
    }
    #endregion

    #region Calculate EQUIPMENT

    private List<ItemPilot> GetItemsLasers => GetItems(ItemTypes.Laser);
    private List<ItemPilot> GetItemsGenerators => GetItems(ItemTypes.Generator);
    private List<ItemPilot> GetItemsExtras => GetItems(ItemTypes.Extra);
    private List<ItemPilot> GetItems(ItemTypes itemTypes) => Pilot.Items.Where(o => o.Item.ItemType == itemTypes && o.IsEquipped).ToList();

    protected void CalculateStatistics()
    {
        MaxHitpoints = Pilot.Ship.Hitpoints;

        #region Lasers
        damagePvp = damagePve = shotDistance = 0;
        shotDispersion = 0;

        int laserCount = 0;
        foreach (ItemPilot itemPilot in GetItemsLasers)
        {
            if(laserCount >= Pilot.Ship.Lasers)
                itemPilot.IsEquipped = false;
            else
            {
                damagePvp += itemPilot.Item.LaserDamagePvp ?? 0;
                damagePve += itemPilot.Item.LaserDamagePve ?? 0;
                shotDistance += itemPilot.Item.LaserShotRange ?? 0;
                shotDispersion += itemPilot.Item.LaserShotDispersion ?? 0;
                laserCount++;
            }
        }
        Division(ref shotDistance, ref laserCount);
        Division(ref shotDispersion, ref laserCount);
        #endregion

        #region Generators
        maxShields = 0;
        speed = Pilot.Ship.Speed;
        ShieldDivision = ShieldRepair = 0;
        int generatorCount = 0;
        foreach (ItemPilot itemPilot in GetItemsGenerators)
        {
            if (generatorCount >= Pilot.Ship.Generators)
                itemPilot.IsEquipped = false;
            else
            {
                maxShields += itemPilot.Item.GeneratorShield ?? 0;
                speed += itemPilot.Item.GeneratorSpeed ?? 0;
                ShieldDivision += itemPilot.Item.GeneratorShieldDivision ?? 0;
                ShieldRepair += itemPilot.Item.GeneratorShieldRepair ?? 0;
                if (itemPilot.Item.GeneratorShield > 0)
                    generatorCount++;
            }
        }
        OnChangePosition();

        Division(ref ShieldDivision, ref generatorCount);
        Division(ref ShieldRepair, ref generatorCount);
        #endregion

        #region Extras
        int extrasCount = 0;
        foreach (ItemPilot itemPilot in GetItemsExtras)
        {
            if (generatorCount >= Pilot.Ship.Extras)
                itemPilot.IsEquipped = false;
            else
            {
                extrasCount++;
            }
        }
        Division(ref ShieldDivision, ref generatorCount);
        Division(ref ShieldRepair, ref generatorCount);
        #endregion


    }

    private void Division(ref float number, ref int divider)
    {
        if (divider > 1)
            number /= divider;
    }
    private void Division(ref int number, ref int divider)
    {
        if (divider > 1)
            number /= divider;
    }
    #endregion



    #region Damage / ShotDistance
    protected long damagePvp;
    public override long DamagePvp => damagePvp;
    protected long damagePve;
    public override long DamagePve => damagePve;
    protected int shotDistance;
    protected override int ShotDistance => shotDistance;
    protected float shotDispersion;
    protected override float ShotDispersion => shotDispersion;
    #endregion



    #region Reward / TakeReward
    public override Reward Reward
    {
        get => Pilot.Ship.Reward;
    }
    public override RewardReasons RewardReason => RewardReasons.KillPlayer;
    public override async void TakeReward(ServerReward reward)
    {
        if (!reward.Experience.Equals(null))
            Pilot.Experience += (ulong)reward.Experience;

        if (!reward.Metal.Equals(null))
            Pilot.Metal += (double)reward.Metal;

        if (!reward.Scrap.Equals(null))
            Pilot.Scrap += (double)reward.Scrap;

        if (reward.Items != null && reward.Items.Count > 0)
        {
            List<ItemPilot> items = new List<ItemPilot>();
            foreach (ItemReward item in reward.Items)
            {
                if (item.Chance == 1000 || item.Chance >= Random.Range(0, 1000))
                {
                    items.Add(await Database.SavePlayerItem(new ItemPilot()
                    {
                        PilotId = Id,
                        ItemId = item.ItemId,
                        Item = Server.Items[item.ItemId],
                        UpgradeLevel = item.UpgradeLevel,
                        IsEquipped = false,
                        IsSold = false,
                    }));
                }
            }

            if (items.Count > 0)
                ItemsChange(items);

            reward.PilotItems = items;
        }

        Send(new CommandData()
        {
            Command = Commands.NewReward,
            Data = reward
        });
    }
    #endregion



    #region Komunikacja z graczem
    public void Send(CommandData commandData)
    {
        if (Headers == null)
            return;

        Send(commandData, Headers);
    }
    public static void Send(CommandData commandData, Headers headers, string channel = "/Game")
    {
        try
        {
            IWebSocketSession session;
            if (Server.WebSocket.WebSocketServices[channel].Sessions.TryGetSession(headers.SocketId, out session))
                Server.WebSocket.WebSocketServices[channel].Sessions.SendTo(GameData.Serialize(commandData), headers.SocketId);
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void SendChat(CommandData commandData)
    {
        if (ChatHeaders == null)
            return;

        Send(commandData, ChatHeaders, "/Chat");
    }

    public static async Task<Pilot> GetPilot(DataRow row)
    {
        Pilot pilot = Pilot.GetPilot(row);

        pilot.Map = Server.Maps[ConvertRow.Row<int>(row["mapid"])];
        pilot.Ship = Server.Ships[ConvertRow.Row<int>(row["shipid"])];
        pilot.Items = await Database.GetPilotItems(pilot.Id);

        PilotResources resources = await Database.GetPilotResources(pilot.Id);

        pilot.Ammunitions = resources.Ammunitions;
        pilot.Rockets = resources.Rockets;
        
        return pilot;
    }
    #endregion
}