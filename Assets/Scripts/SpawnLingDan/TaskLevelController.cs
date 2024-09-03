using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskLevelController : MonoBehaviour
{
    /*关卡数据
    * 
    */
    public int initMaxLevel = 5;//初始最大关卡数
    public int spawnCatNumber = 10;//小猫的初始数量：30,60，120,240,480(1,2,4,8,16)
    private int currLevel;//当前关卡，从0开始

    private int maxLevelCat;//小猫最高等级0-5
    private int currCatNumber;//当前关卡小猫的数量


    public GameObject gameFailedPanel;//游戏失败面板——未完成
    public GameObject gamePassPanel;//当局游戏通过面板
    public GameObject gameFinishedPanel;//游戏全部通关面板

    public float judgeTime = 1f;//判断游戏是否结束时间


    public static TaskLevelController instance;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currLevel = 0;
        currCatNumber = spawnCatNumber * 3 * (int)Mathf.Pow( 2, currLevel);
        maxLevelCat = 0;

        //初始化对局————需要先生产小猫，否则上一局的小猫未清理，可能被添加进了任务里
        LingDanCatController.instance.SpawnCat(currCatNumber);
        TaskController.instance.startGame(currCatNumber, maxLevelCat);

        SceneTransferData.instance.getLingshiNumber = 0;//将可传递的灵石置为0.

    }

    // Update is called once per frame
    void Update()
    {

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
        Debug.Log("获得灵石奖励（含奖励系数）：" + TaskController.instance.sNumber);
        SceneTransferData.instance.getLingshiNumber += TaskController.instance.sNumber;
    }
}
