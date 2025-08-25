using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutCatTips : MonoBehaviour
{
    private bool active;//物体展示了

    private float timer;
    private float gap = 7;

    // Start is called before the first frame update
    void Start()
    {
        timer = gap;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            active = true;
        }

        if (active)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                gameObject.SetActive(false);

                active = false;
                timer = gap;
            }
        }
    }
}
