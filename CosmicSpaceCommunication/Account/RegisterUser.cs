using System;

namespace CosmicSpaceCommunication.Account
{
    [Serializable]
    public class RegisterUser : LogInUser
    {
        public string Email { get; set; }
        public string Nickname { get; set; }
        public bool EmailNewsletter { get; set; }
        public bool Rules { get; set; }
    }
}