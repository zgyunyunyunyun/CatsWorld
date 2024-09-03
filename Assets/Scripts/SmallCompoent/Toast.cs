using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Toast : MonoBehaviour
{
    private float timer = 3.0f;//��ʱ�� 3s

    public TMP_Text text;//�ı�

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

    //����ı���ʾ���İ�
    public void setText(string content)
    {
        text.text = content;
    }

}
