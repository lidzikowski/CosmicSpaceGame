using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game;
using CosmicSpaceCommunication.Game.Player;
using CosmicSpaceCommunication.Game.Player.ClientToServer;
using CosmicSpaceCommunication.Game.Player.ServerToClient;
using CosmicSpaceCommunication.Game.Quest;
using CosmicSpaceCommunication.Game.Resources;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
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



    #region Equipment / Shop
    public void ItemsChange(List<ItemPilot> items)
    {
        foreach (ItemPilot item in items)
        {
            ItemPilot localItem = item.RelationId > 0 ? Pilot.Items.FirstOrDefault(o => o.RelationId == item.RelationId && o.Item.ItemType == item.Item.ItemType && !o.IsSold) : null;

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

    public ShoppingStatus BuyItem(BuyShopItem buyItem)
    {
        IShopItem item = null;
        ShopStatus status;

        if (buyItem.ItemType == ItemTypes.Ship)
        {
            if (!Server.Ships.ContainsKey(buyItem.ItemId) || buyItem.Count != 1)
                status = ShopStatus.Error;
            else
            {
                item = Server.Ships[buyItem.ItemId];
                status = BuyItem(item, buyItem);
            }
        }
        else if(buyItem.ItemType == ItemTypes.Ammunition)
        {
            if (!Server.ServerResources.ContainsKey(buyItem.ItemId) || buyItem.Count < 1 || Pilot.Resources[buyItem.ItemId].Count + buyItem.Count > 100000000)
                status = ShopStatus.Error;
            else
            {
                item = Server.ServerResources[buyItem.ItemId];
                status = BuyItem(item, buyItem);
            }
        }
        else
        {
            if (!Server.Items.ContainsKey(buyItem.ItemId) || buyItem.Count < 1 || Pilot.Items.Count + buyItem.Count > 1000)
                status = ShopStatus.Error;
            else
            {
                item = Server.Items[buyItem.ItemId];
                status = BuyItem(item, buyItem);
            }
        }

        return new ShoppingStatus() { Status = status, ShopItem = item };
    }

    public bool SellItem(ItemPilot itemPilot)
    {
        if (itemPilot.RelationId == 0 || itemPilot.IsSold)
            return false;

        ItemPilot localItem = Pilot.Items.FirstOrDefault(o => o.RelationId == itemPilot.RelationId && o.Item.ItemType == itemPilot.Item.ItemType);

        if (localItem == null)
            return false;

        localItem.IsEquipped = false;
        localItem.IsSold = true;

        double? scrap = itemPilot.Item.ScrapPrice / 30;
        double? metal = itemPilot.Item.MetalPrice / 5;

        TakeReward(new ServerReward()
        {
            Scrap = scrap ?? metal,
            Reason = RewardReasons.SellItem,
            Data = itemPilot.Item.Name
        });

        if (scrap.HasValue)
            AddAchievement(o => o.ItemSellScrap);
        else if (metal.HasValue)
            AddAchievement(o => o.ItemSellMetal);

        AddAchievement(o => o.ScrapReceive, new decimal((double)(scrap ?? metal)));

        CalculateStatistics();

        return true;
    }

    private ShopStatus BuyItem(IShopItem item, BuyShopItem buyItem)
    {
        if (item.RequiredLevel > Pilot.Level)
            return ShopStatus.WrongRequiredLevel;

        if (buyItem.Scrap)
        {
            double price = (double)(item.ScrapPrice * buyItem.Count);
            if (item.ScrapPrice > 0 && Pilot.Scrap >= price)
            {
                TakeReward(new ServerReward()
                {
                    Scrap = -price,
                    Reason = RewardReasons.BuyItem,
                    Data = item.Name
                });

                if (item is Item it)
                    SaveItems(it, buyItem.Count);
                else if (item is Ship ship)
                    SaveShip(ship);
                else if (item is Ammunition ammunition)
                    SaveAmmunition(ammunition, buyItem.Count);

                AddAchievement(o => o.ItemBuyScrap, buyItem.Count);
                AddAchievement(o => o.ScrapSpend, new decimal(price));

                return ShopStatus.Buy;
            }
            else
                return ShopStatus.NoScrap;
        }
        else
        {
            double price = (double)(item.MetalPrice * buyItem.Count);
            if (item.MetalPrice > 0 && Pilot.Metal >= price)
            {
                TakeReward(new ServerReward()
                {
                    Metal = -price,
                    Reason = RewardReasons.BuyItem,
                    Data = item.Name
                });

                if (item is Item it)
                    SaveItems(it, buyItem.Count);
                else if (item is Ship ship)
                    SaveShip(ship);
                else if (item is Ammunition ammunition)
                    SaveAmmunition(ammunition, buyItem.Count);

                AddAchievement(o => o.ItemBuyMetal, buyItem.Count);
                AddAchievement(o => o.MetalSpend, new decimal(price));

                return ShopStatus.Buy;
            }
            else
                return ShopStatus.NoMetal;
        }
    }

    private async void SaveItems(Item item, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Pilot.Items.Add(await Database.SavePlayerItem(new ItemPilot()
            {
                PilotId = Id,
                ItemId = item.Id,
                Item = item,
                UpgradeLevel = 1
            }));
        }
    }

    private void SaveShip(Ship ship)
    {
        Pilot.Ship = ship;

        SendToPilotsInArea(new CommandData()
        {
            Command = Commands.ChangeShip,
            SenderId = Id,
            Data = ship
        }, true);

        CalculateStatistics();

        Hitpoints = MaxHitpoints / 10;
    }

    private void SaveAmmunition(Ammunition ammunition, int count)
    {
        Pilot.Resources[ammunition.Id].Count += count;
        OnChangeResource(Pilot.Resources[ammunition.Id]);
    }
    #endregion


    #region Pilot / Id / Name / isDead
    public Pilot Pilot { get; set; }

    public override ulong Id
    {
        get => Pilot.Id;
        protected set { }
    }
    public override ulong ShipId
    {
        get => (ulong)Pilot.Ship.Id;
        protected set { }
    }

    public override string Name => Pilot.Nickname;
    protected override bool isDead
    {
        get => Pilot.IsDead;
        set
        {
            if (Pilot.IsDead == value)
                return;

            Pilot.IsDead = value;

            if (value)
                Pilot.KillerBy = KillerByString;
            else
                Pilot.KillerBy = string.Empty;
        }
    }
    protected string KillerByString
    {
        get
        {
            string by = string.Empty;
            for (int i = 0; i < AttackOpponents.Count; i++)
            {
                by += AttackOpponents[i].Opponent.Name;
                if (i < AttackOpponents.Count - 1)
                    by += ", ";
            }
            return by;
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
                Data = new AuthUserData()
                {
                    Pilot = Pilot,
                    Enemies = Server.Enemies,
                    Maps = Server.Maps,
                    Resources = Server.ServerResources,
                    Ships = Server.Ships
                }
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
        int generatorShield = 0;
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
                    generatorShield++;
                generatorCount++;
            }
        }
        OnChangePosition();

        Division(ref ShieldDivision, ref generatorShield);
        Division(ref ShieldRepair, ref generatorShield);
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

    #region Ammunition / Rocket
    public override long? Ammunition
    {
        get => Pilot.AmmunitionId;
        set
        {
            if (Pilot.AmmunitionId == value)
                return;

            if (value == null)
                return;

            if (!Server.ServerResources.ContainsKey((long)value) || !Server.ServerResources[(long)value].IsAmmunition)
                return;

            Pilot.AmmunitionId = (long)value;

            OnChangeResource(Pilot.Resources[(long)value]);
        }
    }
    public override long? Rocket
    {
        get => Pilot.RocketId;
        set
        {
            if (Pilot.RocketId == value)
                return;

            if (value == null)
                return;

            if (!Server.ServerResources.ContainsKey((long)value) || Server.ServerResources[(long)value].IsAmmunition)
                return;

            Pilot.RocketId = (long)value;

            OnChangeResource(Pilot.Resources[(long)value]);
        }
    }

    public bool SubtractAmmunition(PilotResource pilotResource)
    {
        long lasers = GetItemsLasers.Count();
        if (pilotResource.Count >= lasers)
        {
            pilotResource.Count -= lasers;
            OnChangeResource(pilotResource);
            return true;
        }
        return false;
    }

    private void OnChangeResource(PilotResource pilotResource)
    {
        Send(new CommandData()
        {
            Command = Commands.ChangeAmmunition,
            Data = new ChangeAmmunition()
            {
                ResourceId = pilotResource.AmmunitionId,
                Count = pilotResource.Count,
                SelectedAmmunitionId = (long)Ammunition,
                SelectedRocketId = (long)Rocket,
            }
        });
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
        if (reward.Experience > 0)
        {
            AddAchievement(o => o.ExpReceive, reward.Experience);
            Pilot.Experience += (ulong)reward.Experience;
        }

        if (reward.Metal > 0)
        {
            reward.Metal = System.Math.Round((double)reward.Metal, 2);
            AddAchievement(o => o.MetalReceive, new decimal((double)reward.Metal));
            Pilot.Metal += (double)reward.Metal;
        }

        if (reward.Scrap > 0)
        {
            reward.Scrap = System.Math.Round((double)reward.Scrap, 2);
            AddAchievement(o => o.ScrapReceive, new decimal((double)reward.Scrap));
            Pilot.Scrap += (double)reward.Scrap;
        }

        if (reward.AmmunitionId > 0 && reward.AmmunitionQuantity > 0)
        {
            Pilot.Resources[(int)reward.AmmunitionId].Count += (long)reward.AmmunitionQuantity;
            OnChangeResource(Pilot.Resources[(int)reward.AmmunitionId]);
        }
        else
            reward.AmmunitionId = null;

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

    #region Achievement / Sync
    public void AddAchievement(Expression<System.Func<Achievement, decimal>> expression, decimal? value = null)
    {
        var property = expression.Body.ToString().Remove(0, 2);

        decimal startValue = (decimal)Pilot.Achievements.GetType().GetProperty(property).GetValue(Pilot.Achievements);

        Pilot.Achievements.GetType().GetProperty(property).SetValue(Pilot.Achievements, value.HasValue ? startValue + (decimal)value : startValue + 1);

        SyncQuestByAchievementAsync(property, null, value);

        //if (property != nameof(Achievement.TravelDistance))
        //    Debug.Log($"[AddAchievement] {property} = {startValue} + {value ?? 1}");
    }
    public void AddAchievement(Expression<System.Func<Achievement, Dictionary<decimal, decimal>>> expression, decimal key, decimal? value = null)
    {
        var property = expression.Body.ToString().Remove(0, 2);

        Dictionary<decimal, decimal> dictionary = (Dictionary<decimal, decimal>)Pilot.Achievements.GetType().GetProperty(property).GetValue(Pilot.Achievements);

        if (key > 0)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value.HasValue ? dictionary[key] + (decimal)value : dictionary[key] + 1;
            }
            else
            {
                dictionary.Add(key, value.HasValue ? dictionary[key] + (decimal)value : 1);
            }
        }
        else
        {
            Server.Log("Nieprawidłowy format klucza kolekcji.", key);
            return;
        }

        SyncQuestByAchievementAsync(property, key, value);

        //Debug.Log($"[AddAchievement] {property} : {key} = {dictionary[key]} + {value ?? 1}");
    }

    private async Task SyncQuestByAchievementAsync(string property, decimal? key, decimal? value)
    {
        //Debug.Log($"[SyncAchievement] {property} [{key}] += {value ?? 1}");

        foreach (PilotTask pilotTask in Pilot.Tasks)
        {
            if (pilotTask.End.HasValue)
                continue;

            foreach (PilotTaskQuest pilotTaskQuest in pilotTask.TaskQuest)
            {
                if (pilotTaskQuest.IsDone)
                    continue;

                if (CheckQuestAchievement(property, pilotTaskQuest, key))
                {
                    if (pilotTaskQuest.Quest.Maps == null ||
                        pilotTaskQuest.Quest.Maps.Count == 0 || 
                        pilotTaskQuest.Quest.Maps.Contains((ulong)Pilot.Map.Id))
                    {
                        pilotTaskQuest.Progress += value ?? 1;

                        if (pilotTaskQuest.Progress >= pilotTaskQuest.Quest.Quantity)
                        {
                            pilotTaskQuest.IsDone = true;
                        }

                        Send(new CommandData()
                        {
                            Command = Commands.QuestProgress,
                            Data = pilotTask
                        });

                        await Database.UpdatePilotTaskQuest(pilotTaskQuest);
                    }
                }
            }

            if (pilotTask.TaskQuest.All(o => o.IsDone))
            {
                //Debug.Log($"[QUEST] {property} => All(o => o.IsDone) [{pilotTask.End}]");

                pilotTask.End = System.DateTime.Now;

                Send(new CommandData()
                {
                    Command = Commands.QuestEndTask,
                    Data = pilotTask
                });

                TakeReward(ServerReward.GetReward(
                        pilotTask.Task.Reward,
                        RewardReasons.CompleteQuest,
                        pilotTask.Task.Name));

                await Database.UpdatePilotTask(pilotTask);
                Pilot.Tasks.Remove(pilotTask);
            }
        }
    }

    private bool CheckQuestAchievement(string property, PilotTaskQuest pilotTaskQuest, decimal? key)
    {
        QuestTypes questType = pilotTaskQuest.Quest.QuestType;

        if (property == nameof(Pilot.Achievements.KillNPC) && 
            questType == QuestTypes.ZabijNPC)
        {
            return pilotTaskQuest.Quest.TargetId == key;
        }
        else if (property == nameof(Pilot.Achievements.KillPlayer) && 
            questType == QuestTypes.ZabijGracz)
        {
            return pilotTaskQuest.Quest.TargetId == key;
        }

        else if (property == nameof(Pilot.Achievements.CollectResource) && 
            questType == QuestTypes.ZbierzSurowiec)
        {
            return pilotTaskQuest.Quest.TargetId == key;
        }

        else if (property == nameof(Pilot.Achievements.TravelDistance) && 
            questType == QuestTypes.PokonajOdleglosc)
        {

        }

        else if (property == nameof(Pilot.Achievements.TimeInGame) && 
            questType == QuestTypes.Spedz)
        {

        }

        else if (property == nameof(Pilot.Achievements.DeadByNPC) && 
            questType == QuestTypes.ZniszczonyPrzezNPC)
        {

        }
        else if (property == nameof(Pilot.Achievements.DeadByPlayer) && 
            questType == QuestTypes.ZniszczonyPrzezGracz)
        {

        }

        else if (property == nameof(Pilot.Achievements.DamageDealNPC) && 
            questType == QuestTypes.ZadajObrazeniaNPC)
        {

        }
        else if (property == nameof(Pilot.Achievements.DamageReceiveNPC) && 
            questType == QuestTypes.OtrzymajObrazeniaNPC)
        {

        }

        else if (property == nameof(Pilot.Achievements.DamageDealPlayer) && 
            questType == QuestTypes.ZadajObrazeniaGracz)
        {

        }
        else if (property == nameof(Pilot.Achievements.DamageReceivePlayer) && 
            questType == QuestTypes.OtrzymajObrazeniaGracz)
        {

        }

        else if (property == nameof(Pilot.Achievements.HitpointRepair) && 
            questType == QuestTypes.NaprawPoszycie)
        {

        }
        else if (property == nameof(Pilot.Achievements.HitpointDestroy) && 
            questType == QuestTypes.UszkodzPoszycie)
        {

        }

        else if (property == nameof(Pilot.Achievements.ShieldRepair) && 
            questType == QuestTypes.NaprawOslone)
        {

        }
        else if (property == nameof(Pilot.Achievements.ShieldDestroy) && 
            questType == QuestTypes.UszkodzOslone)
        {

        }

        else if (property == nameof(Pilot.Achievements.ItemBuyScrap) && 
            questType == QuestTypes.ZakupPrzedmiotScrap)
        {

        }
        else if (property == nameof(Pilot.Achievements.ItemSellScrap) && 
            questType == QuestTypes.SprzedajPrzedmiotScrap)
        {

        }

        else if (property == nameof(Pilot.Achievements.ItemBuyMetal) && 
            questType == QuestTypes.ZakupPrzedmiotMetal)
        {

        }
        else if (property == nameof(Pilot.Achievements.ItemSellMetal) && 
            questType == QuestTypes.SprzedajPrzedmiotMetal)
        {

        }

        else if (property == nameof(Pilot.Achievements.ScrapReceive) && 
            questType == QuestTypes.ZdobadzScrap)
        {

        }
        else if (property == nameof(Pilot.Achievements.ScrapSpend) && 
            questType == QuestTypes.WydajScrap)
        {

        }

        else if (property == nameof(Pilot.Achievements.MetalReceive) && 
            questType == QuestTypes.ZdobadzMetal)
        {

        }
        else if (property == nameof(Pilot.Achievements.MetalSpend) && 
            questType == QuestTypes.WydajMetal)
        {

        }

        else if (property == nameof(Pilot.Achievements.ExpReceive) && 
            questType == QuestTypes.ZdobadzDoswiadczenie)
        {

        }
        else if (property == nameof(Pilot.Achievements.Map) && 
            questType == QuestTypes.PrzejdzNaMape)
        {
            return pilotTaskQuest.Quest.TargetId == key;
        }

        else
            return false;

        return true;
    }
    #endregion


    #region Komunikacja z graczem
    public void Send(CommandData commandData)
    {
        if (Headers == null)
        {
            //Server.Log("Brak odbiorcy dla kanalu Headers", commandData.Command, commandData.Data?.GetType());
            return;
        }

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
            Server.Log("Blad komunikacji z gniazdem.", ex.Message, channel);
        }
    }

    public void SendChat(CommandData commandData)
    {
        if (ChatHeaders == null)
        {
            Server.Log("Brak odbiorcy dla kanalu ChatHeaders");
            return;
        }

        Send(commandData, ChatHeaders, "/Chat");
    }

    public static async Task<Pilot> GetPilot(DataRow row)
    {
        Pilot pilot = Pilot.GetPilot(row);

        pilot.Map = Server.Maps[ConvertRow.Row<int>(row["mapid"])];
        pilot.Ship = Server.Ships[ConvertRow.Row<int>(row["shipid"])];
        pilot.Items = await Database.GetPilotItems(pilot.Id) ?? new List<ItemPilot>();
        pilot.Resources = await Database.GetPilotResources(pilot.Id);
        pilot.ServerResources = Server.ServerResources;
        pilot.Tasks = await Database.GetPilotTasks(pilot.Id) ?? new List<PilotTask>();

        return pilot;
    }
    #endregion
}