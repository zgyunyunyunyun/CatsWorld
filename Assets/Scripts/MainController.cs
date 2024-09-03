using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public bool isFirstTimeGaming = true;//�ж��Ƿ��һ�ν�����Ϸ

    public bool bgIsOpened = false;//�Ƿ�򿪹��������

    //��ʼ��������壬�����жϿ�ͷ���չʾ
    public GameObject beginBG;
    public GameObject beginBornCat;
    public GameObject beginChooseCat;


    public static MainController instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("�״ν�����Ϸ�жϣ�" + isFirstTimeGaming);

        //�ж�����ǵ�һ�δ���Ϸ������ʼ��������ݣ������Ϊfalse
        if (isFirstTimeGaming && !bgIsOpened)
        {
            //չʾ��������
            beginBG.SetActive(true);

            //Debug.Log("�״ν�����Ϸ");
        }
        else
        {
            beginBG.SetActive(false);

            //Debug.Log("���״ν�����Ϸ");
        }
    }

    //�ڹرձ������ʱ����������Ѿ����򿪹���
    public void setBGIsOpened()
    {
        bgIsOpened = true;
    }
}
