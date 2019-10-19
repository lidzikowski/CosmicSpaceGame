using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Userslog
    {
        public long Id { get; set; }
        public long? Userid { get; set; }
        public DateTime Datetime { get; set; }
        public string Action { get; set; }
        public byte Result { get; set; }
        public string Useragent { get; set; }
        public string Host { get; set; }

        public virtual Users User { get; set; }
    }
}
