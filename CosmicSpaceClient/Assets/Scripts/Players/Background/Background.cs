using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [Header("Database ID")]
    public int MapId;

    [Header("Map settings")]
    public Transform PlanetTransform;
    public Transform DebrisTransform;
    public SpriteRenderer MapSprite;
}