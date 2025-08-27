using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using WeChatWASM;
using Random = UnityEngine.Random;

public class NewCatController : MonoBehaviour//虽然这里写新增小猫，但对玩家是探索、寻找
{
    public TMP_Text consumeText;//消耗提示文案
    public TMP_Text freeTimeText;//时间倒计时
    public TMP_Text freeTipsText;//首次打开游戏时，免费获得小猫的倒计时
    public TMP_Text sucessRateText;//成功率文案
    public TMP_Text outCatNumberText;//当前流浪小猫的数量提醒

    private DateTime currentTime;//记录当前时间
    private string hour = "00";
    private string minute = "00";
    private string second = "00";

    public DateTime lastNewTime;//记录上一次招募的时间

    private int freeHour = 4;

    private bool isFree = false;

    public int consumeStone = 200;//下一次消耗的灵石数量

    public int time = 1;//当前第几次诞生小猫，约到后面，生成的小猫等级越高

    public int freeNewTime = 3;//免费诞生小猫次数
    public int shareTime = 3;//可分享的次数

    public Button newBtn;//诞生小猫按钮
    public Button shareBtn;//分享按钮
    public Button watchAddBtn;//看广告按钮
    public GameObject chooseCatUI;//选择小猫的UI
    public GameObject newCatUI;//寻找小猫的UI
    public GameObject toast;//灵石不足toast

    public Image redPoint;//红点是否展示

    public int probabilityToNewCat;//生成小猫的概率

    public int outCatNumber = 0;//流浪小猫的数量

    private float checkShowTipsTime = 10.0f;//每ns检测一次是否需要判断展示tips
    private float checkShowTipsGap = 10.0f;//中间间隔ns

    string[] Name = { "秋水长天", "天涯赤子心", "旧哑扇的夏天 ", "迅羽", "云兰云",
        "傲娇小仙女", "何为浪漫诗", "迅羽", "浅瞳", "绿水无忧", "毛毛", "浅色夏末", "神魂颠倒",
        "众生", "撩汉", "月华之光", "二丫", "狗蛋", "翠花", "地狱之主", "熬夜选手第一名", "果冻爷",
        "暖港少女", "星空女神", "小丸子", "温柔小仙女", "萤火之歌", "旺财", "冰雪少女", "月泠烟", "二宝", "雪梦晴", "西瓜天", "浮伤年华",
        "少女", "女神", "兔兔睡醒了", "小仙女", "姬如月神歌", "爱一半", "冰雪女", "烟", "宝儿", "梦", "西瓜", "年华",
        "一直在做素颜", "拼命女郎", "小兔乖乖", "你是最可爱猪", "徒手摘星", "小布丁儿", "星恋影随", "泪舞清纯", "暖阳", "小胖", "似水流年", "捧猫少女",
        "攒一口袋星星", "打野萝莉", "打不倒的小乖兽", " 舔奶盖的小仙女", "家住魔仙堡", "紫罗兰的秘密", "姐霸道范", "自然萌", "甜七 ", "雪儿", "萝莉", "年华"};

    string[] level = { "金丹期", "金丹期", "金丹期", "金丹期", "金丹期", "元婴期", "元婴期", "元婴期", "化神期" };

    public GameObject outCatTips;//tips对象

    WXRewardedVideoAd renewVideoAd;//广告位初始化


    public static NewCatController instance;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //重新回到主场景，会有影响的地方主要在于，原来有个累计的数据，需要通过update来更新，而不是被start初始化掉了

        //lastNewTime = DateTime.Now;

        //重置的广告位
        renewVideoAd = WX.CreateRewardedVideoAd(
        new WXCreateRewardedVideoAdParam()
        {
            adUnitId = "adunit-3b02ba33ecacb9d0",
            multiton = true
        });

        renewVideoAd.OnClose(RenewAdClose);

    }

    // Update is called once per frame
    void Update()
    {
        //每次诞生逐步叠加所需灵石
        consumeStone = int.Parse((Mathf.Pow(2, time)).ToString() + "00");

        //当处于正常场景时，如果小猫发生变化，则把数值赋予给场景传递的数值
        if (outCatNumber > 0)
        {
            SceneTransferData.instance.outCatNumber = outCatNumber;
        }

        checkShowTipsTime -= Time.deltaTime;
        if (checkShowTipsTime <= 0)
        {
            int pro = Random.Range(0, 110);

            if (pro >= 90 && pro < 110)
            {
                outCatTips.SetActive(true);
                string n = Name[Random.Range(0, Name.Length - 1)];
                string l = level[Random.Range(0, level.Length - 1)];
                outCatNumberText.text = "恭喜：玩家" + n + "寻找到了<color=#9800FF>" + l + "</color>境界的小猫";
            }
            else if (pro >= 80 && pro < 90)
            {
                outCatTips.SetActive(true);
                outCatNumberText.text = "注意：附近区域发现了<color=#9800FF>" + outCatNumber.ToString() + "</color>只小猫在外流浪，快去寻小猫";
            }
            else if (pro >= 70 && pro < 80)
            {
                outCatTips.SetActive(true);
                outCatNumberText.text = "提示：小猫会自动炼丹，偷偷把灵丹收藏到藏丹阁";
            }
            else if (pro >= 60 && pro < 70)
            {
                outCatTips.SetActive(true);
                outCatNumberText.text = "提示：点击灵丹即可出售，藏丹阁满后，小猫会偷偷半价出售";
            }
            else if (pro >= 50 && pro < 60)
            {
                outCatTips.SetActive(true);
                outCatNumberText.text = "提示：点击左上角小猫头像可升级种族等级，容纳更多小猫";
            }
            else if (pro >= 40 && pro < 50)
            {
                outCatTips.SetActive(true);
                outCatNumberText.text = "提示：当灵石不足时，可以炼制丹药快速获得大量灵石";
            }
            else if (pro >= 30 && pro < 40)
            {
                outCatTips.SetActive(true);
                outCatNumberText.text = "提示：升级种族需要灵石和领土，同时记得升级小猫哦";
            }
            else if (pro >= 20 && pro < 30)
            {
                outCatTips.SetActive(true);
                outCatNumberText.text = "提示：小猫修炼需要大量灵石，加油！！";
            }
            else if (pro >= 10 && pro < 20)
            {
                outCatTips.SetActive(true);
                outCatNumberText.text = "提示：小猫通过修炼可获得修为，从而晋级";
            }
            else if (pro >= 0 && pro < 10)
            {
                outCatTips.SetActive(true);
                outCatNumberText.text = "提示：小猫晋级后，可以打败更高等级的敌人";
            }
            else
            {
                outCatTips.SetActive(false);
                Debug.Log("附近小猫tips为空");
            }

            checkShowTipsTime = checkShowTipsGap;
        }


        //如果是第一次打开游戏，免费3次：展示文案，并免费
        if (MainController.instance.isFirstTimeGaming)
        {
            sucessRateText.gameObject.SetActive(true);
            isFree = true;
            //freeTipsText.text = "首次玩游戏，可收养" + freeNewTime.ToString() + "只流浪的小猫";

            if (freeNewTime <= 0)
            {
                MainController.instance.isFirstTimeGaming = false;
                isFree = false;
                time = 1;
            }
        }


        //更新消耗文案提醒
        if (isFree)
        {
            consumeText.text = "免费";
            sucessRateText.gameObject.SetActive(true);
            freeTimeText.gameObject.SetActive(false);
            newBtn.gameObject.SetActive(true);

            if (CatController.instance.cats.Count >= UpRaceController.instance.MaxCatNumber(UpRaceController.instance.raceLevel))
            {
                consumeText.text = "<color=#E52222>升级猫世界可收养更多小猫</color>";
            }

            freeTimeText.text = " ";
            time = 1;

            //更新红点提醒
            redPoint.gameObject.SetActive(true);

            shareBtn.gameObject.SetActive(false);
            watchAddBtn.gameObject.SetActive(false);
            newBtn.transform.localPosition = new Vector3(0, -458, 0);
            newBtn.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text = "找小猫(" + freeNewTime.ToString() + ")";
        }
        else
        {
            //更新红点提醒
            redPoint.gameObject.SetActive(false);

            //隐藏免费文案
            freeTimeText.gameObject.SetActive(true);
            sucessRateText.gameObject.SetActive(false);

            consumeText.text = "<color=#E52222>看视频</color>可找免费3次小猫";


            if (CatController.instance.cats.Count >= UpRaceController.instance.MaxCatNumber(UpRaceController.instance.raceLevel))
            {
                consumeText.text = "<color=#E52222>升级猫世界可收养更多小猫</color>";
            }


            //更新时间倒计时
            currentTime = DateTime.Now;

            TimeSpan difference = currentTime - lastNewTime; // 计算时间差

            float secondsDifference = (float)difference.TotalSeconds; // 相差的总秒数

            //如果时间都是0（倒计时结束），则展示免费按钮；否则，正常展示时间倒计时
            if (secondsDifference >= 3600 * freeHour)
            {
                isFree = true;
                freeNewTime = 3;
                Debug.Log("重置免费");
            }
            else
            {
                //倒计时剩余时间
                float rest = 3600 * freeHour - secondsDifference;
                int h = ((int)rest) / 3600;
                int m = ((int)rest - h * 3600) / 60;
                int s = ((int)rest) - m * 60 - h * 3600;

                //Debug.Log("M" + m);
                //Debug.Log("S" + s);

                if (h < 10)
                {
                    hour = "0" + h.ToString();
                }
                else
                {
                    hour = h.ToString();
                }

                if (m < 10)
                {
                    minute = "0" + m.ToString();
                }
                else
                {
                    minute = m.ToString();
                }

                if (s < 10)
                {
                    second = "0" + s.ToString();
                }
                else
                {
                    second = s.ToString();
                }

                if (shareTime > 0)
                {
                    freeTimeText.text = hour + ":" + minute + ":" + second + " 后免费找3次小猫";

                    if (!watchAddBtn.gameObject.activeSelf)
                    {
                        newBtn.gameObject.SetActive(false);
                    }
                    watchAddBtn.gameObject.SetActive(true);

                }
                else
                {
                    freeTimeText.text = hour + ":" + minute + ":" + second + " 后免费找3次小猫";
                }

            }

        }


    }

    //免费次数减少1，如果免费次数为0，则倒计时重置
    public void minusOneFreeTime()
    {
        freeNewTime--;
        if (freeNewTime <= 0)
        {
            isFree = false;
            lastNewTime = DateTime.Now;
        }
        else
        {
            isFree = true;
        }
    }

    //打开小猫详情页
    public void newCatAndOpenCatDetail()
    {


        if (UpRaceController.instance.MaxCatNumber(UpRaceController.instance.raceLevel) <= CatController.instance.cats.Count)
        {
            //弹出提示小猫数量达到峰值
            toast.SetActive(true);
            toast.GetComponent<Toast>().setText("小猫数量已达到最大值，请升级种族等级");
            Debug.Log("小猫数量达到峰值");
        }
        //如果满足诞生小猫的条件（免费或消耗足够灵石——暂定100灵石诞生1次），则增加小猫；否则提示灵石不足
        else if (isFree || PropertyController.instance.lingshiNumber >= consumeStone && UpRaceController.instance.MaxCatNumber(UpRaceController.instance.raceLevel) > CatController.instance.cats.Count)
        {
            //消耗所需灵石
            if (!isFree)
            {
                PropertyController.instance.consumeLingShi(consumeStone);
            }

            // //如果是第一次玩游戏，必得3只小猫；否则按灵石逐步增加去获得小猫
            // if (MainController.instance.isFirstTimeGaming)
            // {
            //     ChooseCatUI.instance.newCatAndCatUI(0);//新增小猫
            //     chooseCatUI.gameObject.SetActive(true);

            //     minusOneFreeTime();
            // }
            // else
            // {
            //概率list：容易获得，不太可能获得，中间值
            int[] proList =
                { 90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 99, 95, 95, 95,
                    90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 99, 95, 95, 95,
                    90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 99, 95, 95, 95,
                     70, 60, 50, 50,70, 70, 60, 50, 70, 90, 80, 70, 70, 60, 50, 50,70, 70, 60, 50, 50, 20, 20, 10,
                     90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 99, 95, 95, 95,
                     70, 60, 50, 50,70, 70, 60, 50, 70, 90, 80, 70, 70, 60, 50, 50,70, 70, 60, 50, 50, 20, 20, 10,
                    70, 60, 50, 50,70, 70, 60, 50, 70, 90, 80, 70, 70, 60, 50, 50,70, 70, 60, 50, 50, 20, 20, 10,
                    70, 60, 50, 50,70, 70, 60, 50, 70, 90, 80, 70, 70, 60, 50, 50,70, 70, 60, 50, 50, 20, 20, 10,
                    70, 60, 50, 50,70, 70,70, 60, 50, 50,70, 70,70, 60, 50, 50,70, 70,70, 60, 50, 50,70, 70,70, 60, 50, 50,
                    50, 50,70, 70, 60, 50, 50, 20, 20, 10,30, 30, 10, 10, 10, 15, 10, 30, 40, 50, 10,
                    50, 50,70, 70, 60, 50, 50, 20, 20, 10,30, 30, 10, 10, 10, 15, 10, 30, 40, 50, 10};

            int r = Random.Range(0, 100);
            probabilityToNewCat = proList[Random.Range(0, proList.Length - 1)];//随机选择上述list的概率

            Debug.Log("产生小猫的次数：" + time);
            //如果成功生产小猫，则走生成小猫的条件；否则弹出提示生成失败
            if (r >= probabilityToNewCat)//一定通过，在后面等级处控制是否生成小猫
            {
                ChooseCatUI.instance.newCatAndCatUI(0);//新增小猫
                                                       //newCatUI.gameObject.SetActive(false);
                chooseCatUI.gameObject.SetActive(true);
            }
            else if (MainController.instance.isFirstTimeGaming && freeNewTime <= 1)
            {
                ChooseCatUI.instance.newCatAndCatUI(0);//新增小猫
                chooseCatUI.gameObject.SetActive(true);
                //newCatUI.gameObject.SetActive(false);
            }
            else
            {
                //弹出诞生失败
                toast.SetActive(true);
                toast.GetComponent<Toast>().setText("本次探索未能寻找到小猫");
                Debug.Log("本次探索未能寻找到小猫");

            }

            if (MainController.instance.isFirstTimeGaming && freeNewTime <= 1)
            {
                newCatUI.gameObject.SetActive(false);
            }

            //重新免费小猫倒计时
            if (isFree)
            {
                minusOneFreeTime();
                Debug.Log("重新免费倒计时");
            }

            //当前轮次里，诞生小猫次数+1
            time++;

            // }


        }
        else if (PropertyController.instance.lingshiNumber < consumeStone)
        {
            //弹出提示灵石不足
            toast.SetActive(true);
            toast.GetComponent<Toast>().setText("灵石不足，请获得更多灵石");

            //consumeText.text = "灵石不足，获得更多灵石作为探索资金";



            Debug.Log("弹出灵石不足toast");
        }



    }


    //分享函数
    public void Share()
    {
        WX.ShareAppMessage(new ShareAppMessageOption
        {
            //imageUrl = imageUrl, // 图片的URL，也可以不填（自动截屏）
            title = "嘿嘿，我马上就能找到" + CatController.instance.cats.Count.ToString() + "只猫猫了", // 显示文本
            //query = query, // 附带参数，限制2k长度
        });


        isFree = true;
        freeNewTime = 3;
        shareTime--;
        Debug.Log("重置免费");
    }

    //点击了重置按钮
    public void ClickRenewBtn()
    {
        WatchAddToRenew();
    }

    //为了移除而看视频
    void WatchAddToRenew()
    {
        if (renewVideoAd != null)
        {
            renewVideoAd.Show();
            Debug.Log("激励广告展示");
        }

    }

    //关闭广告事件监听-重置
    void RenewAdClose(WXRewardedVideoAdOnCloseResponse res)
    {
        if ((res != null && res.isEnded) || res == null)
        {
            // 正常播放结束，可以下发游戏奖励
            isFree = true;
            freeNewTime = 3;
            shareTime--;

            Debug.Log("测试广告成功");
        }
        else
        {
            // 播放中途退出，不下发游戏奖励
            Debug.Log("广告中途退出");
        }
    }
}
