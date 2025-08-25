using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpLianDanController : MonoBehaviour
{
    //需要初始化的数据
    private int purityPercentage;//完成的纯度，影响最后提升炼丹的效率
    private float schedule;//完成的进度，决定了当局游戏是否结束。即完整吸收所有灵液的比例

    private float purityPoint;//灵液分数
    private float impurityPoint;//杂质分数

    public float point = 1f;//每个灵液或者杂质的分数，暂定1分

    public int level = 2;//游戏的关卡，决定了游戏的难度
    private int purityNumber;//根据关卡，需要生成的总灵液数量
    //private int impurityNumber;//根据关卡，需要生成的总杂质数量

    public float upLianDan;//炼丹加成
    public float duration;//持续时间

    //public bool isFinish = false;//当前游戏是否完成


    //需要赋值的对象
    public GameObject purity;//待实例化的灵液
    public GameObject impurity;//待实例化的杂质

    public GameObject purities;//灵液控制器
    public GameObject impurities;//杂质控制器

    public Slider slider;//游戏进度条
    public TMP_Text purityPerText;//展示完成纯度的百分比，从100%开始
    public GameObject gameResult;//展示游戏结果的面板
    public TMP_Text purityReward;//完成游戏的加速奖励
    public TMP_Text durationTime;//完成奖励加速的持续时间



    public int gameHard = 2;//游戏难度



    public static UpLianDanController instance;
    private void Awake()
    {
        instance = this;        
    }

    // Start is called before the first frame update
    void Start()
    {
        startGame();
    }

    // Update is called once per frame
    void Update()
    {
        //刷新当前丹药的纯度
        if (impurityPoint == 0 && purityPoint > 0)
        {
            purityPercentage = 100;
            purityPerText.text = purityPercentage.ToString() + "%";
        }
        else if(purityPoint > 0)
        {
            purityPercentage = ((int)(purityPoint / (purityPoint + impurityPoint)*100));
            purityPerText.text = purityPercentage.ToString()+ "%";
        }

        //如果完成的进度达到100%，则游戏结束
        if (schedule >= 1)
        {
            //暂停游戏，展示结果面板
            stopGame();
            gameResult.SetActive(true);


            //计算并展示奖励
            upLianDan = purityPercentage * (level + 1) * 2;
            duration = (level + 1) * 10;

            purityReward.text = upLianDan.ToString() + "%";
            durationTime.text = duration.ToString() + "分钟";

            ResultData.instance.upLianDan = upLianDan;
            ResultData.instance.duration = duration;
        }

    }

    public void stopGame()
    {
        Time.timeScale = 0; // 暂停游戏
    }

    public void keepGame()
    {
        Time.timeScale = 1; // 继续游戏
    }

    //开始游戏时，初始化数据，同时用于重新进入新的关卡
    public void startGame()
    {
        //清空杂质和灵液
        clearThings();

        //初始化基础数据
        purityPoint = 0;
        impurityPoint = 0;
        purityPercentage = 0;
        schedule = 0;
        changeSchedule(schedule);

        upLianDan = 0;
        duration = 0;

        //产生杂质和灵液
        spawnPurities(level);

        purityNumber = gameHard * (level + 1);

        Time.timeScale = 1; // 继续游戏(重新进入游戏时）
    }

    //进入游戏的下一关
    public void nextLevelGame()
    {
        level++;

        startGame();
    }

    public void finishGame()
    {
        ResultData.instance.isFinish = true;

        //TaskManager.instance.ldResult.upSpeed = upLianDan;
        //TaskManager.instance.ldResult.duration =  duration;
        //TaskManager.instance.rewardChanged = true;
        //Debug.Log("结束当前凝练灵液游戏，返回主界面: " + TaskManager.instance.rewardChanged);
    }

    //清空当前游戏的杂质和灵液
    public void clearThings()
    {
        Debug.Log("清空节点："+ purity.transform.childCount);
        if (purities.transform.childCount > 0)
        {
            Debug.Log("清空节点");
            for (int j = 0; j < purities.transform.childCount; j++)
            {
                Destroy(purities.transform.GetChild(j).gameObject);

                Debug.Log("清空原父节点的子节点");
            }
        }

        if (impurities.transform.childCount > 0)
        {
            for (int j = 0; j < impurities.transform.childCount; j++)
            {
                Destroy(impurities.transform.GetChild(j).gameObject);

                Debug.Log("清空原父节点的子节点");
            }
        }
    }

    //根据关卡增加生成灵液或杂质的数量
    void spawnPurities(int level)
    {
        //一次性生成的数量 = 关卡难度 * 关卡数
        for(int i=0; i< gameHard*(level+1); i++)
        {
            //挂在父节点上
            GameObject tempP = Instantiate(purity);
            tempP.transform.SetParent(purities.transform, false);
            tempP.transform.localPosition = new Vector3(Random.Range(-2.8f, 2.8f), Random.Range(-5, 5), 0);

            GameObject tempImP = Instantiate(impurity);
            tempImP.transform.SetParent(impurities.transform, false);
            tempImP.transform.localPosition = new Vector3(Random.Range(-2.8f, 2.8f), Random.Range(-5, 5), 0);
        }
    }

    //灵液增加分数
    public void addPurityPoint(float point)
    {
        purityPoint += point;

        //计算完成的进度
        schedule = (purityPoint / point) / purityNumber;
        changeSchedule(schedule);
    }

    //杂质加分
    public void addImpurityPoint(float point)
    {
        impurityPoint += point;
    }

    //改变进度
    public void changeSchedule(float schedule)
    {
        slider.value = schedule;
    }



}
