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

    
    #endregion
}