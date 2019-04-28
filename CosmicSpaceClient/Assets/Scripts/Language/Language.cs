using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class Language
{
    public static Language Load(Languages language)
    {
        TextAsset textAsset = (TextAsset)Resources.Load($"Languages/{language}");

        Language lang;
        using (TextReader sr = new StringReader(textAsset.text))
        {
            var serializer = new XmlSerializer(typeof(Language));
            lang = (Language)serializer.Deserialize(sr);
        }
        return lang;
    }

    #region StartWindow
    #region SignInWindow
    [XmlElement("SIGN_IN")]
    public string SIGN_IN { get; set; }
    [XmlElement("USERNAME")]
    public string USERNAME { get; set; }
    [XmlElement("PASSWORD")]
    public string PASSWORD { get; set; }
    [XmlElement("REMEMBER")]
    public string REMEMBER { get; set; }
    [XmlElement("LOG_IN")]
    public string LOG_IN { get; set; }
    [XmlElement("CREATE_ACCOUNT")]
    public string CREATE_ACCOUNT { get; set; }
    #endregion

    #region CreateAccountWindow
    [XmlElement("EMAIL")]
    public string EMAIL { get; set; }
    [XmlElement("NICKNAME")]
    public string NICKNAME { get; set; }
    [XmlElement("RULES")]
    public string RULES { get; set; }
    [XmlElement("REGISTER")]
    public string REGISTER { get; set; }
    #endregion

    #region Newsletter
    [XmlElement("NEWSLETTER")]
    public string NEWSLETTER { get; set; }
    #endregion

    [XmlElement("GAME_VERSION")]
    public string GAME_VERSION { get; set; }
    [XmlElement("SERVER_STATUS")]
    public string SERVER_STATUS { get; set; }
    #endregion


    #region RepairShipWindow
    [XmlElement("REPAIR")]
    public string REPAIR { get; set; }
    [XmlElement("DESTROYED")]
    public string DESTROYED { get; set; }
    #endregion


    [XmlElement("MINIMAP")]
    public string MINIMAP { get; set; }

    #region Game

    [XmlElement("DEFEATED_PLAYER")]
    public string DEFEATED_PLAYER { get; set; }

    [XmlElement("DEFEATED_ENEMY")]
    public string DEFEATED_ENEMY { get; set; }

    [XmlElement("EXPERIENCE")]
    public string EXPERIENCE { get; set; }

    [XmlElement("COMMAND_NOT_FOUND")]
    public string COMMAND_NOT_FOUND { get; set; }

    [XmlElement("EMPTY_MESSAGE")]
    public string EMPTY_MESSAGE { get; set; }

    [XmlElement("CONNECTING_TO_CHAT")]
    public string CONNECTING_TO_CHAT { get; set; }

    [XmlElement("CHAT_CONNECTED")]
    public string CHAT_CONNECTED { get; set; }

    [XmlElement("CHAT_DISCONNECTED")]
    public string CHAT_DISCONNECTED { get; set; }

    [XmlElement("YOU")]
    public string YOU { get; set; }

    [XmlElement("CHAT_USER_NOT_FOUND")]
    public string CHAT_USER_NOT_FOUND { get; set; }

    [XmlElement("CHAT_HELP_LIST")]
    public string CHAT_HELP_LIST { get; set; }

    [XmlElement("CHAT_HELP_ONLINE")]
    public string CHAT_HELP_ONLINE { get; set; }

    [XmlElement("CHAT_HELP_PRIVATE")]
    public string CHAT_HELP_PRIVATE { get; set; }

    [XmlElement("PORTAL_NOT_FOUND")]
    public string PORTAL_NOT_FOUND { get; set; }

    [XmlElement("PORTAL_FOUND")]
    public string PORTAL_FOUND { get; set; }

    [XmlElement("TARGET_NOT_FOUND")]
    public string TARGET_NOT_FOUND { get; set; }

    [XmlElement("SAFE_ZONE_ACTIVE")]
    public string SAFE_ZONE_ACTIVE { get; set; }

    [XmlElement("SAFE_ZONE_INACTIVE")]
    public string SAFE_ZONE_INACTIVE { get; set; }

    [XmlElement("TARGET_IS_COVER")]
    public string TARGET_IS_COVER { get; set; }

    [XmlElement("QUIT")]
    public string QUIT { get; set; }

    [XmlElement("SETTINGS")]
    public string SETTINGS { get; set; }

    [XmlElement("MISSIONS")]
    public string MISSIONS { get; set; }

    [XmlElement("HANGAR")]
    public string HANGAR { get; set; }

    [XmlElement("EXIT")]
    public string EXIT { get; set; }

    [XmlElement("EXIT_QUESTION")]
    public string EXIT_QUESTION { get; set; }

    [XmlElement("YES")]
    public string YES { get; set; }

    [XmlElement("NO")]
    public string NO { get; set; }

    [XmlElement("LANGUAGE")]
    public string LANGUAGE { get; set; }

    [XmlElement("LASER")]
    public string LASER { get; set; }

    [XmlElement("LASERS")]
    public string LASERS { get; set; }

    [XmlElement("GENERATOR")]
    public string GENERATOR { get; set; }

    [XmlElement("GENERATORS")]
    public string GENERATORS { get; set; }

    [XmlElement("EXTRA")]
    public string EXTRA { get; set; }

    [XmlElement("EXTRAS")]
    public string EXTRAS { get; set; }

    [XmlElement("WAREHOUSE")]
    public string WAREHOUSE { get; set; }

    [XmlElement("DAMAGE")]
    public string DAMAGE { get; set; }

    [XmlElement("DAMAGE_PVP")]
    public string DAMAGE_PVP { get; set; }

    [XmlElement("DAMAGE_PVE")]
    public string DAMAGE_PVE { get; set; }

    [XmlElement("SHOT_RANGE")]
    public string SHOT_RANGE { get; set; }

    [XmlElement("SHOT_DISPERSION")]
    public string SHOT_DISPERSION { get; set; }

    [XmlElement("SPEED")]
    public string SPEED { get; set; }

    [XmlElement("SHIELD")]
    public string SHIELD { get; set; }

    [XmlElement("SHIELD_DIVISION")]
    public string SHIELD_DIVISION { get; set; }

    [XmlElement("SHIELD_REPAIR")]
    public string SHIELD_REPAIR { get; set; }

    [XmlElement("RECEIVE_ITEM")]
    public string RECEIVE_ITEM { get; set; }

    [XmlElement("NO_EQUIP")]
    public string NO_EQUIP { get; set; }

    [XmlElement("SHOP")]
    public string SHOP { get; set; }

    [XmlElement("SHIPS")]
    public string SHIPS { get; set; }

    [XmlElement("CARGO")]
    public string CARGO { get; set; }

    [XmlElement("HITPOINTS")]
    public string HITPOINTS { get; set; }

    [XmlElement("BUY_FOR")]
    public string BUY_FOR { get; set; }

    [XmlElement("UNAVAILABLE")]
    public string UNAVAILABLE { get; set; }


    #endregion
}