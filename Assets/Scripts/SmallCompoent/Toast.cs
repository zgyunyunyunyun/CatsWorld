using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Toast : MonoBehaviour
{
    private float timer = 3.0f;//计时器 3s

    public TMP_Text text;//文本

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {

            timer -= Time.deltaTime;
            if (timer < 0)
            {
                gameObject.SetActive(false);
                timer = 3;
            }           
            
        }
    }

    //输入改变提示的文案
    public void setText(string content)
    {
        text.text = content;
    }

}
