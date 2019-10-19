using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Users
    {
        public Users()
        {
            Userslog = new HashSet<Userslog>();
        }

        public long Userid { get; set; }
        public string Usernamehash { get; set; }
        public string Passwordhash { get; set; }
        public string Email { get; set; }
        public byte Emailnewsletter { get; set; }
        public byte Acceptrules { get; set; }
        public byte Ban { get; set; }
        public DateTime? Registerdate { get; set; }

        public virtual Pilots Pilots { get; set; }
        public virtual ICollection<Userslog> Userslog { get; set; }
    }
}
