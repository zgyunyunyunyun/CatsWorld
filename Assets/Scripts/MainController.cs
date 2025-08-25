using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WeChatWASM;

public class MainController : MonoBehaviour
{
    public bool isFirstTimeGaming = true;//判断是否第一次进入游戏

    public bool bgIsOpened = false;//是否打开过背景面板

    //初始的三个面板，用于判断开头如何展示
    public GameObject beginBG;
    public GameObject beginBornCat;
    public GameObject beginChooseCat;

    //开局展示免费生产小猫
    public GameObject freeNewCat;


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
        //Debug.Log("首次进入游戏判断：" + isFirstTimeGaming);

        //判断玩家是第一次打开游戏，并初始化相关数据，最后置为false
        if (isFirstTimeGaming && !bgIsOpened)
        {
            //展示背景介绍
            beginBG.SetActive(true);

            //Debug.Log("首次进入游戏");

            freeNewCat.SetActive(true);
        }
        else
        {
            beginBG.SetActive(false);

            //Debug.Log("非首次进入游戏");
        }
    }

    //在关闭背景面板时，设置面板已经被打开过了
    public void setBGIsOpened()
    {
        bgIsOpened = true;
    }
}
