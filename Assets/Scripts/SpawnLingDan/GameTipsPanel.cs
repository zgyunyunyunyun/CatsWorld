using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTipsPanel : MonoBehaviour
{
    private float timer = 3.0f;
    private float gap = 3.0f;

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                gameObject.SetActive(false);
                timer = gap;
            }
        }

    }
}
