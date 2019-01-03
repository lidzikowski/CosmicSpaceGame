using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmicSpaceCommunication.Game.Resources
{
    [Serializable]
    public class ServerReward : Reward
    {
        public RewardReasons Reason { get; set; }
        public object Data { get; set; }

        public static ServerReward GetReward(Reward reward, RewardReasons reason, object data)
        {
            return new ServerReward()
            {
                Experience = reward.Experience,
                Metal = reward.Metal,
                Scrap = reward.Scrap,
                Reason = reason,
                Data = data,
            };
        }
    }
}
