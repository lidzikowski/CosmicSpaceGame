using CosmicSpaceCommunication.Game.Resources;
using System.Collections.Generic;
using UnityEngine;

public class GalacticWindow : GameWindow
{
    public static Dictionary<long, Map> ServerMaps;

    public GameObject MapsGameObject;
    public Transform PortalsTransform;
    public GameObject MapNameGameObject;


    public override void Start()
    {
        base.Start();
    }

    public override void Refresh()
    {
        base.Refresh();
    }

    public override void ChangeLanguage()
    {
        base.ChangeLanguage();
    }

    private void OnEnable()
    {
        foreach (Transform t in MapsGameObject.transform)
        {
            t.gameObject.SetActive(false);
        }

        if (ServerMaps != null)
            ResourcesUI.Instance.LoadMaps(ServerMaps, this);
    }

    private void OnDisable()
    {
        ResourcesUI.Instance.DisposeMaps();
    }
}