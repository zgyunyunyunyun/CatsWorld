using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WeChatWASM;

public class TaskLevelController : MonoBehaviour
{
    /*关卡数据
    * 
    */
    public int initMaxLevel = 5;//初始最大关卡数
    public int spawnCatNumber = 10;//小猫的初始数量：30,60，120,240,480(1,2,4,8,16)
    public int currLevel;//当前关卡，从0开始

    private int maxLevelCat;//小猫最高等级0-5
    private int currCatNumber;//当前关卡小猫的数量


    public GameObject gameFailedPanel;//游戏失败面板——未完成
    public GameObject gamePassPanel;//当局游戏通过面板
    public GameObject gameFinishedPanel;//游戏全部通关面板
    public GameObject gameStopdPanel;//游戏暂停面板

    public float judgeTime = 1f;//判断游戏是否结束时间

    public GameObject gameTipsPanel;//游戏开局的提醒
    public TMP_Text levelTitle;//关卡提醒
    public TMP_Text gameTips;//游戏提醒


    //广告组件
    WXCustomAd BannerAd;//banner广告

    WXRewardedVideoAd doubleVideoAd;//广告位初始化
    WXRewardedVideoAd doubleLastVideoAd;//广告位初始化


    public static TaskLevelController instance;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currLevel = 0;
        currCatNumber = spawnCatNumber * 3 * (int)Mathf.Pow(2, currLevel);
        maxLevelCat = 0;

        //初始化对局————需要先生产小猫，否则上一局的小猫未清理，可能被添加进了任务里
        LingDanCatController.instance.SpawnCat(currCatNumber);
        TaskController.instance.startGame(currCatNumber, maxLevelCat);

        //SceneTransferData.instance.getLingshiNumber = 0;//将可传递的灵石置为0.
        gameTipsPanel.SetActive(true);


        //双倍的广告位 - 最后
        doubleLastVideoAd = WX.CreateRewardedVideoAd(
        new WXCreateRewardedVideoAdParam()
        {
            adUnitId = "adunit-f81bd6ad1b21f15e",
            multiton = true
        });

        doubleLastVideoAd.OnClose(DoubleLastAdClose);

        //双倍的广告位
        doubleVideoAd = WX.CreateRewardedVideoAd(
        new WXCreateRewardedVideoAdParam()
        {
            adUnitId = "adunit-91e1c33b90182c3f",
            multiton = true
        });

        doubleVideoAd.OnClose(DoubleAdClose);


        double screenWidth = WX.GetSystemInfoSync().screenWidth;
        double screenHeight = WX.GetSystemInfoSync().screenHeight;

        Debug.Log("屏幕尺寸评估-宽：" + screenWidth.ToString());
        Debug.Log("屏幕尺寸评估-高：" + screenHeight.ToString());

        int t = (int)(screenHeight * 3.5 / 5);
        int w = (int)(screenWidth * 0.9);
        int l = (int)(screenWidth - 380) / 2;

        Debug.Log("屏幕尺寸评估-l、t、w 分别是：" + l.ToString() + " " + t.ToString() + " " + w.ToString());

        //banner广告位
        BannerAd = WX.CreateCustomAd(new WXCreateCustomAdParam()
        {
            adUnitId = "adunit-31ab5f63566b4997",
            adIntervals = 30,
            style = new CustomStyle() { left = l, top = t, width = 380 }
        });
    }

    // Update is called once per frame
    void Update()
    {
        //跟随弹窗展示banner广告
        if (BannerAd != null)
        {
            if (gameFailedPanel.activeSelf || gamePassPanel.activeSelf || gameFinishedPanel.activeSelf || gameStopdPanel.activeSelf)
            {
                BannerAd.Show();
            }
            else
            {
                BannerAd.Hide();
            }
        }
    }

    //通关当前关卡
    public void passedCurrentGame()
    {
        //是否完成所有关卡任务，游戏结束；否则，当局游戏结束
        if (currLevel + 1 >= initMaxLevel)
        {
            //Debug.Log("完成所有关卡的小猫炼丹任务，游戏结束！！");
            gameFinishedPanel.SetActive(true);
            //Time.timeScale = 0;
        }
        else
        {
            //Debug.Log("完成当前关卡所有小猫炼丹任务，游戏结束！！");
            gamePassPanel.SetActive(true);
            //Time.timeScale = 0;
        }

    }

    //下一关
    public void nextLevel()
    {
        Debug.Log("进入下一关");

        Time.timeScale = 1;


        currLevel++;
        currCatNumber = spawnCatNumber * 3 * (int)Mathf.Pow(2, currLevel);

        //初始化下一对局
        LingDanCatController.instance.SpawnCat(currCatNumber);
        TaskController.instance.startGame(currCatNumber, maxLevelCat);

        gameTipsPanel.SetActive(true);
        levelTitle.text = "第" + (currLevel + 1).ToString() + "关";
        if (currLevel > 0)
        {
            gameTips.text = "本关小鱼奖励增加" + currLevel.ToString() + "倍";
        }

    }

    //重新开始
    public void restartGame()
    {
        Debug.Log("重新开始游戏");

        Time.timeScale = 1;

        //判断是否存在上一局的通用设置
        if (gameFailedPanel.activeSelf)
        {
            gameFailedPanel.SetActive(false);
        }



        //初始化对局
        currCatNumber = spawnCatNumber * 3 * (int)Mathf.Pow(2, currLevel);
        Debug.Log("开始游戏需要生产的小猫数量：" + currCatNumber);
        LingDanCatController.instance.SpawnCat(currCatNumber);
        TaskController.instance.startGame(currCatNumber, maxLevelCat);

        gameTipsPanel.SetActive(true);
        levelTitle.text = "第" + (currLevel + 1).ToString() + "关";
        if (currLevel > 0)
        {
            gameTips.text = "本关小鱼奖励增加" + currLevel.ToString() + "倍";
        }
    }

    //游戏失败，结束
    public void gameFailed()
    {
        Debug.Log("游戏结束！！");

        StartCoroutine(waitEnd(judgeTime));
    }

    IEnumerator waitEnd(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        gameFailedPanel.SetActive(true);
        //Time.timeScale = 0;
    }

    //把灵石奖励添加到资产里
    public void getLingshiAward()
    {
        Debug.Log("获得奖励（含奖励系数）：" + TaskController.instance.sNumber);
        SceneTransferData.instance.getLingshiNumber += TaskController.instance.sNumber;
    }

    //看视频 - 最后
    public void WatchAddToDoubleLast()
    {
        if (doubleVideoAd != null)
        {
            doubleLastVideoAd.Show();
            Debug.Log("激励广告展示");
        }

    }

    //关闭广告事件监听 - 最后
    void DoubleLastAdClose(WXRewardedVideoAdOnCloseResponse res)
    {
        if ((res != null && res.isEnded) || res == null)
        {
            // 正常播放结束，可以下发游戏奖励??
            TaskController.instance.sNumber *= 2;
            BackToMainScene();

            Debug.Log("测试广告成功");
        }
        else
        {
            // 播放中途退出，不下发游戏奖励
            Debug.Log("广告中途退出");
        }
    }

    //看视频
    public void WatchAddToDouble()
    {
        if (doubleVideoAd != null)
        {
            doubleVideoAd.Show();
            Debug.Log("激励广告展示");
        }

    }

    //关闭广告事件监听-
    void DoubleAdClose(WXRewardedVideoAdOnCloseResponse res)
    {
        if ((res != null && res.isEnded) || res == null)
        {
            // 正常播放结束，可以下发游戏奖励??
            TaskController.instance.sNumber *= 2;
            nextLevel();
            gamePassPanel.SetActive(false);

            Debug.Log("测试广告成功");
        }
        else
        {
            // 播放中途退出，不下发游戏奖励
            Debug.Log("广告中途退出");
        }
    }

    //返回主场景
    public void BackToMainScene()
    {
        BannerAd.Destroy();
        Debug.Log("关闭banner广告");

        //获得灵石
        getLingshiAward();

        SceneController.instance.changeScene(0);
    }
}
