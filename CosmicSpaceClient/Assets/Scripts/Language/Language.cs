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
}