using UnityEngine;
using UnityEngine.UI;

public class LogScript : GameWindow
{
    float timer = 0;
    float timeLimit = 10;

    private void LateUpdate()
    {
        if (timer > timeLimit)
            Destroy(gameObject);

        timer += Time.deltaTime;
    }

    public void SetText(string message, float time)
    {
        timer = 0;
        timeLimit = time;
        GetComponent<Text>().text = message;
    }
}