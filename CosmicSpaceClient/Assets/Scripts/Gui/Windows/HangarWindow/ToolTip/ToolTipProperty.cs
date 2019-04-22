using UnityEngine;
using UnityEngine.UI;

public class ToolTipProperty : MonoBehaviour
{
    public Text PropertyName;
    public Text PropertyValue;

    public void SetProperty(string name, string value)
    {
        PropertyName.text = name;
        PropertyValue.text = value;
    }
}