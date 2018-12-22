using System;

namespace CosmicSpaceCommunication.Account
{
    [Serializable]
    public class LogInUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}