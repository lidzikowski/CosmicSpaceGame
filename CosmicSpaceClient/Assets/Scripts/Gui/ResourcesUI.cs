using System.Collections.Generic;
using UnityEngine;

public class ResourcesUI : MonoBehaviour
{
    public GameObject ResourceCamera;
    public RenderTexture RenderTexture;

    public Dictionary<string, Sprite> ShipSprites;

    public static ResourcesUI Instance;

    public int SpriteSize = 256;

    private void Start()
    {
        Instance = this;
        ShipSprites = new Dictionary<string, Sprite>();

        ResourceCamera.SetActive(false);
        foreach (Transform t in transform)
            t.gameObject.SetActive(false);
    }

    public void LoadImages()
    {
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
            Texture2D texture = new Texture2D(SpriteSize, SpriteSize, TextureFormat.RGB24, false);
            RenderTexture.active = RenderTexture;
            texture.ReadPixels(new Rect(0, 0, SpriteSize, SpriteSize), 0, 0);
            texture.Apply();
            ShipSprites.Add(t.name, Sprite.Create(texture, new Rect(0, 0, SpriteSize, SpriteSize), new Vector2(0.5f, 0.5f), 100.0f));

            t.gameObject.SetActive(false);
        }
        ResourceCamera.SetActive(false);
    }

    private void FixedUpdate()
    {
        if(RotatingGameObject != null)
            RotatingGameObject.transform.Rotate(0, 0.5f, 0);
    }

    private GameObject RotatingGameObject;
    public void RotateItem(string key)
    {
        if(key == null)
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
}