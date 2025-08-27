using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransferData : MonoBehaviour
{
    public int getLingshiNumber;//从游戏里获得的灵石

    public int maxCatLevel;//小猫最高等级
    private float putTimer = 1;//每1s传递一次等级数据


    public float price = 1;//当前灵丹价格系数

    public float priceRestTimer = -1;//价格需要进行倒计时的时间
    private float priceRestGap = 900;//价格剩余时间的倒计时

    public bool isHighPirce = false;//当时是否高价格

    public float expendRestTimer = -1;//拓展领土需要进行倒计时的时间
    private float expendRestGap = 900;//领土剩余时间的倒计时

    public bool hasAttackEnemy = false;//当时是否进攻的敌人


    public int outCatNumber = 0;//每次打开游戏随机产生的小猫数量

    public bool isConsumeStone = false;//判断是否已经消耗了灵石
    public bool isEatFish = false;//判断是否已经吃了鱼(进游戏只触发一次)


    public static SceneTransferData instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            // 保持这个对象不被销毁
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        outCatNumber = Random.Range(20, 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (NewCatController.instance != null && NewCatController.instance.outCatNumber <= 0)
        {
            NewCatController.instance.outCatNumber = outCatNumber;
        }

        //展示价格UI时，倒计时展示数字
        if (isHighPirce && priceRestTimer >= 0)
        {
            priceRestTimer -= Time.deltaTime;

            if (priceRestTimer <= 0)
            {
                priceRestTimer = priceRestGap;

                isHighPirce = false;

                price = 1;
            }
        }

        //展示领土UI时，倒计时展示数字
        if (hasAttackEnemy && expendRestTimer >= 0)
        {
            expendRestTimer -= Time.deltaTime;

            if (expendRestTimer <= 0)
            {
                expendRestTimer = expendRestGap;

                hasAttackEnemy = false;
            }
        }

        putTimer -= Time.deltaTime;
        if (putTimer <= 0)
        {
            putCatMaxLevel();
        }
    }

    public void putCatMaxLevel()
    {
        maxCatLevel = CatController.instance.getCatMaxLevel();
    }
}
