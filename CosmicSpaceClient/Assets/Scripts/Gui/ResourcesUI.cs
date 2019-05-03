using CosmicSpaceCommunication.Game.Resources;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesUI : MonoBehaviour
{
    public GameObject ResourceCamera;
    public RenderTexture RenderTexture;

    public Dictionary<string, Sprite> ShipSprites;



    public GameObject GalacticCamera;
    public RenderTexture GalacticRenderTexture;
    public Transform GalacticTransform;
    public Transform PortalsGalacticTransform;

    public Dictionary<string, Sprite> GalacticSprites;



    public static ResourcesUI Instance;

    public int SpriteSize = 256;
    public int GalacticSpriteSize = 256;

    private void Start()
    {
        Instance = this;
        ShipSprites = new Dictionary<string, Sprite>();
        GalacticSprites = new Dictionary<string, Sprite>();

        ResourceCamera.SetActive(false);
        GalacticCamera.SetActive(false);
        foreach (Transform t in transform)
            t.gameObject.SetActive(false);
        foreach (Transform t in GalacticTransform)
            Destroy(t.gameObject);
    }

    public void LoadImages()
    {
        if (DisposeCoroutine?.Current != null && ShipSprites.Count > 0)
        {
            StopCoroutine(DisposeCoroutine);
            DisposeCoroutine = null;
            return;
        }

        if (RotatingGameObject != null)
        {
            RotatingGameObject.transform.rotation = new Quaternion();
            RotatingGameObject.SetActive(false);
            RotatingGameObject = null;
        }

        ResourceCamera.SetActive(true);
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(true);

            ResourceCamera.transform.position = new Vector3(t.gameObject.transform.position.x - 9, t.gameObject.transform.position.y + 12, t.gameObject.transform.position.z + 10);
            ResourceCamera.GetComponent<Camera>().Render();
            Texture2D texture = new Texture2D(SpriteSize, SpriteSize, TextureFormat.ARGB32, false);
            RenderTexture.active = RenderTexture;
            texture.ReadPixels(new Rect(0, 0, SpriteSize, SpriteSize), 0, 0);
            texture.alphaIsTransparency = true;
            texture.Apply();
            ShipSprites.Add(t.name, Sprite.Create(texture, new Rect(0, 0, SpriteSize, SpriteSize), new Vector2(0.5f, 0.5f), 100.0f));

            t.gameObject.SetActive(false);
        }
        ResourceCamera.SetActive(false);
    }
    
    public IEnumerator LoadMaps(Dictionary<long, Map> maps, GalacticWindow galacticWindow)
    {
        GalacticSprites.Clear();

        foreach (Transform tr in GalacticTransform)
            Destroy(tr.gameObject);
        foreach (Transform tr in PortalsGalacticTransform)
            Destroy(tr.gameObject);

        GalacticCamera.SetActive(true);
        foreach (Map map in maps.Values)
        {
            Player.CreateBackground(map, GalacticTransform, PortalsGalacticTransform, null);

            yield return new WaitForEndOfFrame();

            GalacticCamera.GetComponent<Camera>().Render();
            Texture2D texture = new Texture2D(GalacticSpriteSize, GalacticSpriteSize, TextureFormat.ARGB32, false);
            RenderTexture.active = GalacticRenderTexture;
            texture.ReadPixels(new Rect(0, 0, GalacticSpriteSize, GalacticSpriteSize), 0, 0);
            texture.Apply();
            GalacticSprites.Add(map.Name, Sprite.Create(texture, new Rect(0, 0, GalacticSpriteSize, GalacticSpriteSize), new Vector2(0.5f, 0.5f), 100.0f));

            foreach (Transform tr in GalacticTransform)
                Destroy(tr.gameObject);
            foreach (Transform tr in PortalsGalacticTransform)
                Destroy(tr.gameObject);
        }
        GalacticCamera.SetActive(false);

        float divider = 10.417f;
        float subtract = 48;
        foreach (Transform t in galacticWindow.MapsGameObject.transform)
        {
            if (GalacticSprites.ContainsKey(t.name))
            {
                t.GetComponent<Image>().sprite = Instance.GalacticSprites[t.name];
                foreach (Portal portal in maps.Values.First(o => o.Name == t.name).Portals)
                {
                    Transform targetMap = galacticWindow.MapsGameObject.transform.Find(portal.TargetMap.Name);

                    Vector2 fromPosition = new Vector2(t.position.x - subtract + portal.PositionX / divider, t.position.y + subtract + portal.PositionY / divider);
                    Vector2 toPosition = new Vector2(targetMap.position.x - subtract + portal.TargetPositionX / divider, targetMap.position.y + subtract + portal.TargetPositionY / divider);
                    //Vector2 fromPosition = new Vector2(t.position.x, t.position.y);
                    //Vector2 toPosition = new Vector2(targetMap.position.x, targetMap.position.y);

                    if (galacticWindow.PortalsTransform.Find($"{portal.TargetMap.Name}:{portal.Map.Name}") == null)
                    {
                        GameObject line = new GameObject()
                        {
                            name = $"{portal.Map.Name}:{portal.TargetMap.Name}"
                        };
                        line.transform.parent = galacticWindow.PortalsTransform;
                        line.AddComponent<Image>();
                        RectTransform rect = line.GetComponent<RectTransform>();

                        Vector3 differenceVector = toPosition - fromPosition;
                        rect.sizeDelta = new Vector2(differenceVector.magnitude, 2);
                        rect.pivot = new Vector2(0, 0);
                        rect.position = fromPosition;
                        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
                        rect.rotation = Quaternion.Euler(0, 0, angle);
                    }
                }
                t.gameObject.SetActive(true);

                Text text = Instantiate(galacticWindow.MapNameGameObject, t).GetComponent<Text>();
                text.text = t.name;
            }
        }
    }

    public void UnloadMaps()
    {
        GalacticSprites.Clear();
    }

    private void FixedUpdate()
    {
        if(RotatingGameObject != null)
            RotatingGameObject.transform.Rotate(0, 0.5f, 0);
    }

    private GameObject RotatingGameObject;
    public void RotateItem(string key)
    {
        if (key == null)
        {
            if(RotatingGameObject != null)
            {
                RotatingGameObject.transform.rotation = new Quaternion();
                RotatingGameObject.SetActive(false);
                RotatingGameObject = null;
            }
            ResourceCamera.SetActive(false);
            return;
        }

        Transform child = transform.Find(key);
        if (child == null)
            return;

        if (RotatingGameObject != null)
        {
            RotatingGameObject.transform.rotation = new Quaternion();
            RotatingGameObject.SetActive(false);
        }
        RotatingGameObject = child.gameObject;
        RotatingGameObject.SetActive(true);

        ResourceCamera.transform.position = new Vector3(RotatingGameObject.transform.position.x - 9, RotatingGameObject.transform.position.y + 12, RotatingGameObject.transform.position.z + 10);

        ResourceCamera.SetActive(true);
    }

    public void Dispose()
    {
        if (DisposeCoroutine?.Current != null)
        {
            StopCoroutine(DisposeCoroutine);
        }

        DisposeCoroutine = DisposeTextures();
        try
        {
            StartCoroutine(DisposeCoroutine);
        }
        catch (System.Exception) { }
    }

    IEnumerator DisposeCoroutine;

    IEnumerator DisposeTextures()
    {
        yield return new WaitForSeconds(60);
        ShipSprites.Clear();
    }
}