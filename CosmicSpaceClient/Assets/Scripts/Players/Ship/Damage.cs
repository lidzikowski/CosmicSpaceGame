using UnityEngine;

public class Damage : MonoBehaviour
{
    float timer = 0.5f;
    private void LateUpdate()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
            Destroy(gameObject);

        transform.Translate((Vector3.up * 20) * Time.deltaTime, Space.World);
    }
}