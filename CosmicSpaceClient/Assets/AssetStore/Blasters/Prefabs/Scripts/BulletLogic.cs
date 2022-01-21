using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogic : MonoBehaviour
{
    public GameObject TargetGameObject;
    private Vector3 TargetPosition;
    private TrailRenderer trailRenderer;

    private void Start()
    {
        TargetPosition = TargetGameObject.transform.position;
        trailRenderer = GetComponent<TrailRenderer>();
    }

    float t;
    void Update()
    {
        if (TargetGameObject != null)
            TargetPosition = TargetGameObject.transform.position;
        
        t += Time.deltaTime / 0.3f;
        transform.position = Vector3.Lerp(transform.position, TargetPosition, t);
        
        if (Vector3.Distance(transform.position, TargetPosition) < 1)
        {
            Destroy(gameObject);
            return;
        }
    }
}