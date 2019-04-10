using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game;
using CosmicSpaceCommunication.Game.Player;
using CosmicSpaceCommunication.Game.Resources;
using System.Data;
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
            if (Pilot.Hitpoints.Equals(value))
                return;

            Pilot.Hitpoints = value;
        }
    }
    public override long MaxHitpoints
    {
        get => Pilot.MaxHitpoints;
    }
    #endregion

    #region Shields / MaxShields
    protected override long shields
    {
        get => Pilot.Shields;
        set
        {
            if (Pilot.Shields.Equals(value))
                return;

            Pilot.Shields = value;
        }
    }
    public override long MaxShields
    {
        get => Pilot.MaxShields;
    }
    #endregion

    #region Speed
    public override int Speed
    {
        get => Pilot.Speed;
    }
    #endregion

    #region Calculate MaxHitpoints / MaxShields / Speed
    protected void CalculateStatistics()
    {
        Pilot.MaxHitpoints = Pilot.Ship.Hitpoints;
        Pilot.MaxShields = 1000;
        Pilot.Speed = Pilot.Ship.Speed;
    }
    #endregion



    #region Damage / ShotDistance
    public override long Damage
    {
        get => 300;
    }
    protected override int ShotDistance
    {
        get => 50;
    }
    #endregion
    


    #region Reward / TakeReward
    public override Reward Reward
    {
        get => Pilot.Ship.Reward;
    }
    public override RewardReasons RewardReason => RewardReasons.KillPlayer;
    public override void TakeReward(ServerReward reward)
    {
        if (!reward.Experience.Equals(null))
            Pilot.Experience += (ulong)reward.Experience;

        if (!reward.Metal.Equals(null))
            Pilot.Metal += (double)reward.Metal;

        if (!reward.Scrap.Equals(null))
            Pilot.Scrap += (double)reward.Scrap;

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

        PilotResources resources = await Database.GetPilotResources(pilot.Id);

        pilot.Ammunitions = resources.Ammunitions;
        pilot.Rockets = resources.Rockets;
        
        return pilot;
    }
    #endregion
}