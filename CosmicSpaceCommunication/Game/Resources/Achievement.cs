using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CosmicSpaceCommunication.Game.Resources
{
    [Serializable]
    public class Achievement
    {
        /// <summary>
        /// Ilosc zniszczen poszczegolnego NPC
        /// ID NPC : ILOSC ZABIC
        /// </summary>
        public Dictionary<ulong, ulong> KillNPC { get; set; } = new Dictionary<ulong, ulong>(); // OK

        /// <summary>
        /// Ilosc zniszczen poszczegolnego Rodzaju statku (Gracza)
        /// ID RODZAJU STATKU : ILOSC ZABIC
        /// </summary>
        public Dictionary<ulong, ulong> KillPlayer { get; set; } = new Dictionary<ulong, ulong>(); // TEST



        /// <summary>
        /// Ilosc zebran poszczegolnego Rodzaju surowca
        /// ID RODZAJU SUROWCA : ILOSC ZEBRAN
        /// </summary>
        public Dictionary<ulong, ulong> CollectResource { get; set; } = new Dictionary<ulong, ulong>();



        /// <summary>
        /// Pokonany dystans w grze
        /// </summary>
        public ulong TravelDistance { get; set; }



        /// <summary>
        /// Czas w grze w sekundach
        /// </summary>
        public ulong TimeInGame { get; set; } // TEST



        /// <summary>
        /// Zabicia przez NPC
        /// </summary>
        public ulong DeadByNPC { get; set; }

        /// <summary>
        /// Zabicia przez Gracza
        /// </summary>
        public ulong DeadByPlayer { get; set; }



        /// <summary>
        /// Zadane obrazenia w NPC
        /// </summary>
        public ulong DamageDealNPC { get; set; }

        /// <summary>
        /// Przyjete obrazenia od NPC
        /// </summary>
        public ulong DamageReceiveNPC { get; set; }



        /// <summary>
        /// Zadane obrazenia w Gracza
        /// </summary>
        public ulong DamageDealPlayer { get; set; }

        /// <summary>
        /// Przyjete obrazenia od Gracza
        /// </summary>
        public ulong DamageReceivePlayer { get; set; }



        /// <summary>
        /// Naprawiona ilosc poszycia
        /// </summary>
        public ulong HitpointRepair { get; set; }

        /// <summary>
        /// Zniszczona ilosc poszycia
        /// </summary>
        public ulong HitpointDestroy { get; set; }



        /// <summary>
        /// Naprawiona ilosc oslony
        /// </summary>
        public ulong ShieldRepair { get; set; }

        /// <summary>
        /// Zniszczona ilosc oslony
        /// </summary>
        public ulong ShieldDestroy { get; set; }



        /// <summary>
        /// Zakupiony przedmiot za Scrap
        /// </summary>
        public ulong ItemBuyScrap { get; set; }

        /// <summary>
        /// Sprzedany przedmiot za Scrap
        /// </summary>
        public ulong ItemSellScrap { get; set; }



        /// <summary>
        /// Zakupiony przedmiot za Metal
        /// </summary>
        public ulong ItemBuyMetal { get; set; }

        /// <summary>
        /// Sprzedany przedmiot za Metal
        /// </summary>
        public ulong ItemSellMetal { get; set; }



        /// <summary>
        /// Zdobyty Scrap
        /// </summary>
        public ulong ScrapReceive { get; set; }

        /// <summary>
        /// Wydany Scrap
        /// </summary>
        public ulong ScrapSpend { get; set; }



        /// <summary>
        /// Zdobyty Metal
        /// </summary>
        public ulong MetalReceive { get; set; }

        /// <summary>
        /// Wydany Metal
        /// </summary>
        public ulong MetalSpend { get; set; }



        /// <summary>
        /// Zdobyta ilosc doswiadczenia
        /// </summary>
        public ulong ExpReceive { get; set; }



        /// <summary>
        /// Ilosc odwiedzi
        /// ID MAPY : ILOSC ODWIEDZIN
        /// </summary>
        public Dictionary<ulong, ulong> Map { get; set; } = new Dictionary<ulong, ulong>();



        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Achievement GetAchievement(string json)
        {
            return string.IsNullOrWhiteSpace(json) ? new Achievement() : JsonConvert.DeserializeObject<Achievement>(json);
        }
    }
}