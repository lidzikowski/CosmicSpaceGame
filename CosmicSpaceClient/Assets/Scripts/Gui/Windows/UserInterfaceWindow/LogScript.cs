using UnityEngine;
using UnityEngine.UI;

public class LogScript : GameWindow
{
    float timer = 0;
    private void LateUpdate()
    {
        if (timer > 5)
            Destroy(gameObject);
        timer += Time.deltaTime;
    }

    public void SetText(string message)
    {
        GetComponent<Text>().text = message;
    }
}