using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    [Serializable]
    public class Achievement
    {
        /// <summary>
        /// Ilosc zniszczen poszczegolnego NPC
        /// ID NPC : ILOSC ZABIC
        /// </summary>
        public Dictionary<decimal, decimal> KillNPC { get; set; } = new Dictionary<decimal, decimal>(); // OK

        /// <summary>
        /// Ilosc zniszczen poszczegolnego Rodzaju statku (Gracza)
        /// ID RODZAJU STATKU : ILOSC ZABIC
        /// </summary>
        public Dictionary<decimal, decimal> KillPlayer { get; set; } = new Dictionary<decimal, decimal>(); // TEST



        /// <summary>
        /// Ilosc zebran poszczegolnego Rodzaju surowca
        /// ID RODZAJU SUROWCA : ILOSC ZEBRAN
        /// </summary>
        public Dictionary<decimal, decimal> CollectResource { get; set; } = new Dictionary<decimal, decimal>();



        /// <summary>
        /// Pokonany dystans w grze
        /// </summary>
        public decimal TravelDistance { get; set; } // OK



        /// <summary>
        /// Czas w grze w sekundach
        /// </summary>
        public decimal TimeInGame { get; set; } // OK ???



        /// <summary>
        /// Zabicia przez NPC
        /// </summary>
        public decimal DeadByNPC { get; set; } // OK

        /// <summary>
        /// Zabicia przez Gracza
        /// </summary>
        public decimal DeadByPlayer { get; set; } // TEST



        /// <summary>
        /// Zadane obrazenia w NPC
        /// </summary>
        public decimal DamageDealNPC { get; set; } // OK

        /// <summary>
        /// Przyjete obrazenia od NPC
        /// </summary>
        public decimal DamageReceiveNPC { get; set; } // OK



        /// <summary>
        /// Zadane obrazenia w Gracza
        /// </summary>
        public decimal DamageDealPlayer { get; set; } // TEST

        /// <summary>
        /// Przyjete obrazenia od Gracza
        /// </summary>
        public decimal DamageReceivePlayer { get; set; } // TEST



        /// <summary>
        /// Naprawiona ilosc poszycia
        /// </summary>
        public decimal HitpointRepair { get; set; } // OK

        /// <summary>
        /// Zniszczona ilosc poszycia
        /// </summary>
        public decimal HitpointDestroy { get; set; } // OK



        /// <summary>
        /// Naprawiona ilosc oslony
        /// </summary>
        public decimal ShieldRepair { get; set; } // OK

        /// <summary>
        /// Zniszczona ilosc oslony
        /// </summary>
        public decimal ShieldDestroy { get; set; } // OK



        /// <summary>
        /// Zakupiona liczba przedmiotow za Scrap
        /// </summary>
        public decimal ItemBuyScrap { get; set; } // OK

        /// <summary>
        /// Sprzedana liczba przedmiotow za Scrap
        /// </summary>
        public decimal ItemSellScrap { get; set; } // OK



        /// <summary>
        /// Zakupiona liczba przedmiotow za Metal
        /// </summary>
        public decimal ItemBuyMetal { get; set; } // OK

        /// <summary>
        /// Sprzedana liczba przedmiotow za Metal
        /// </summary>
        public decimal ItemSellMetal { get; set; } // OK



        /// <summary>
        /// Zdobyty Scrap
        /// </summary>
        public decimal ScrapReceive { get; set; } // SELL - OK, REWARD - OK

        /// <summary>
        /// Wydany Scrap
        /// </summary>
        public decimal ScrapSpend { get; set; } // SHOP - OK



        /// <summary>
        /// Zdobyty Metal
        /// </summary>
        public decimal MetalReceive { get; set; } // REWARD - OK

        /// <summary>
        /// Wydany Metal
        /// </summary>
        public decimal MetalSpend { get; set; } // SHOP - OK



        /// <summary>
        /// Zdobyta ilosc doswiadczenia
        /// </summary>
        public decimal ExpReceive { get; set; } // REWARD - OK



        /// <summary>
        /// Ilosc odwiedzi
        /// ID MAPY : ILOSC ODWIEDZIN
        /// </summary>
        public Dictionary<decimal, decimal> Map { get; set; } = new Dictionary<decimal, decimal>(); // OK



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