using System.Collections.Generic;
using UnityEngine;

public class ShipLogic : MonoBehaviour
{
    public List<GameObject> Lasers => ChildInChild("Lasers");
    public List<GameObject> Gears => ChildInChild("Gears");

    [Header("Ship transform rotate/spawn")]
    public Transform RotationTransform;
    public Transform ModelTransform;

    [Header("Ship name")]
    public TextMesh ModelNameText;


    public void LaserShot(GameObject target)
    {

    }

    public void InitShip(string shipName, string modelName, Color nameColor)
    {
        if (ModelNameText != null)
        {
            ModelNameText.text = modelName;
            ModelNameText.color = nameColor;
        }

        foreach(Transform t in ModelTransform)
        {
            Destroy(t.gameObject);
        }

        Instantiate(Resources.Load<GameObject>("Ships/" + shipName), ModelTransform);
    }
    
    private List<GameObject> ChildInChild(string parent)
    {
        foreach (Transform child in transform)
        {
            if (child.name == parent)
            {
                List<GameObject> objects = new List<GameObject>();
                foreach (Transform t in child)
                {
                    objects.Add(t.gameObject);
                }
                return objects;
            }
        }
        return null;
    }
}