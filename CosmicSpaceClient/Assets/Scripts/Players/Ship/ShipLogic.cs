using CosmicSpaceCommunication.Game.Resources;
using System.Collections.Generic;
using UnityEngine;

public class ShipLogic : MonoBehaviour
{
    public List<GameObject> Lasers => ChildInChild("Lasers");
    public List<GameObject> Gears => ChildInChild("Gears");

    [Header("Ship transform rotate/spawn")]
    public Transform RotationTransform;
    private float RotateAngle = 0;
    public Transform ModelTransform;

    [Header("Ship name")]
    public TextMesh ModelNameText;

    

    public Vector2 Position
    {
        get => transform.position;
        set => transform.position = value;
    }
    private Vector2 targetPosition;
    public Vector2 TargetPosition
    {
        get => targetPosition;
        set
        {
            if (targetPosition == value)
                return;

            targetPosition = value;

            Debug.Log(value);
        }
    }

    public GameObject TargetGameObject;
    public bool Attack = false;

    protected bool gearsStatus;
    public bool GearsStatus
    {
        get
        {
            return gearsStatus;
        }
        set
        {
            if (value == gearsStatus)
                return;

            gearsStatus = value;

            //if (value)
            //{
            //    foreach (ParticleSystem engine in engines)
            //        engine.Play();
            //}
            //else
            //{
            //    foreach (ParticleSystem engine in engines)
            //        engine.Stop();
            //}
        }
    }

    public Ship Ship;
    public int Speed => Ship.Speed;



    private void Update()
    {
        Rotate();
        Fly();
    }

    public void Rotate()
    {
        if (TargetGameObject != null && Attack)
            RotateAngle = Mathf.Atan2(TargetGameObject.transform.position.y - Position.y, TargetGameObject.transform.position.x - Position.x) * Mathf.Rad2Deg + 90;
        else
        {
            float angle = Mathf.Atan2(TargetPosition.y - Position.y, TargetPosition.x - Position.x);
            if (angle == 0)
                return;
            RotateAngle = angle * Mathf.Rad2Deg + 180;
        }

        RotationTransform.rotation = Quaternion.Lerp(RotationTransform.rotation, Quaternion.Euler(RotateAngle, -90, 90), Time.deltaTime * 10);
    }

    public void Fly()
    {
        if (TargetPosition != Position)
        {
            Position = Vector3.MoveTowards(Position, TargetPosition, Time.deltaTime * Speed / 10);
            if (!GearsStatus)
                GearsStatus = true;
        }
        else if (GearsStatus)
            GearsStatus = false;
    }

    public void InitShip(Ship ship, string modelName, Color nameColor)
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

        Ship = ship;

        Instantiate(Resources.Load<GameObject>("Ships/" + ship.Name), ModelTransform);
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