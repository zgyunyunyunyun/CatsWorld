using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using WeChatWASM;

public class PropertyController : MonoBehaviour///————遗留问题：小游戏内的灵石怎么带出来？切换场景前面怎么保留数据？切换场景出来怎么正常生产灵丹？
{
    public float timeToLingdan = 5;//需要n秒炼制1颗灵丹
    private float timer = 0;//灵丹计时器
    private float lingshiTimer = 0;//灵石计时器

    public float lingdanNumber = 0;//灵丹的数量
    private float lingdanSpeed;//灵丹产生的速度

    public int lingshiNumber = 0;//灵石的数量
    private float beforlingshiNumber;//上一秒灵石数量
    private float lingshiSpeed;//灵石产生的速度

    public long fishNumber = 0; // 鱼的数量
    private long beforFishNumber; // 上一秒鱼的数量

    public TMP_Text lingdanText;//灵丹数量的文本
    public TMP_Text lingdanSpeedText;//灵丹产生速度的文本

    public TMP_Text lingshiText;//灵石数量的文本
    public TMP_Text lingshiSpeedText;//灵石产生速度的文本

    public GameObject lingdanToSpawn;//用于实例化的灵丹
    public GameObject lingdanParent;//用户存储实例化灵丹的父节点
    public GameObject destoryLingdanParent;//销毁时展示的父节点，因为这scrollview里，看不到动画，放出来

    //private List<GameObject> lingdanUIList = new List<GameObject>();//用于存储灵丹实例化UI的列表
    private List<LingDan> lingdanList = new List<LingDan>();//用于存储灵丹数据的列表
    public int maxLingdanNumber;//一次性最多存储的灵丹数量
    public int maxLingdanUINumber = 20;//一次性最多存储的灵丹数量

    public float territoryArea;//领土面积
    public TMP_Text areaText;

    private float tipsTimer = 1.0f;//tips计时器

    public AudioClip clickLingdanClip;//点击灵丹的音效

    public int waitToGetLingDan = 0;//等待领取的灵丹数量
    public int waitToGetLingshi = 0;//等待领取的灵丹转化为灵石的数量
    public GameObject waitToGetObj;//待领取的UI对象
    public TMP_Text waitToGetLingshiText;//待领取灵石数量的文本

    public TMP_Text bottomTips;//底部的灵丹提示

    public GameObject offlinePanelObj;//离线收益面板
    public TMP_Text waitLingshiNumberText;//底部的灵丹提示

    WXCustomAd BannerAd;//banner广告

    WXRewardedVideoAd doubleVideoAd;//广告位初始化

    private Queue<GameObject> pool = new Queue<GameObject>();

    public static PropertyController instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //lingshiNumber += SceneTransferData.instance.getLingshiNumber;

        //领土面积
        areaText.text = NumberController.instance.NumberToChinaString((int)territoryArea) + " 平方米";

        //双倍的广告位
        doubleVideoAd = WX.CreateRewardedVideoAd(
        new WXCreateRewardedVideoAdParam()
        {
            adUnitId = "adunit-ab24dd73b4185365",
            multiton = true
        });

        doubleVideoAd.OnClose(DoubleAdClose);

        //获得实际屏幕的宽高
        //float screenWidth = canvas.pixelRect.width;
        //float screenHeight = canvas.pixelRect.height;

        //double screenWidth = WX.GetSystemInfoSync().screenWidth;
        //double screenHeight = WX.GetSystemInfoSync().screenHeight;

        // Debug.Log("屏幕尺寸评估-宽：" + screenWidth.ToString());
        // Debug.Log("屏幕尺寸评估-高：" + screenHeight.ToString());

        /*
        //获得panel的高度
        RectTransform objectTransform = panel.GetComponent<RectTransform>();

        // 获取物体到屏幕顶部的距离
        float distanceToTop = objectTransform.offsetMax.y;
        // 获取物体的高度
        float height = objectTransform.sizeDelta.y;

        int l = (int)(screenWidth * 0.1);
        int t = (int)((distanceToTop + height) * 1.1);
        int w = (int)(screenWidth * 0.8);
        Debug.Log("屏幕尺寸评估-l、t、w 分别是：" + l.ToString() + " " + t.ToString() + " " + w.ToString());
        */

        // int t = (int)(screenHeight * 4 / 5);
        // int w = (int)(screenWidth * 0.9);
        // int l = (int)(screenWidth - 380) / 2;

        // Debug.Log("屏幕尺寸评估-l、t、w 分别是：" + l.ToString() + " " + t.ToString() + " " + w.ToString());

        // //banner广告位
        // BannerAd = WX.CreateCustomAd(new WXCreateCustomAdParam()
        // {
        //     adUnitId = "adunit-032744d39c795888",
        //     adIntervals = 30,
        //     style = new CustomStyle() { left = l, top = t, width = 380 }
        // });
    }

    // 每帧判断资源的刷新，包括：玩家灵草、灵石、灵丹及产生速度，猫咪的产生，任务的产生等
    void Update()
    {
        /*灵丹产生逻辑：
         * 1、每只小猫N秒产生1颗
         * 2、每10秒产生一次，每次产生N颗*小猫
         * 3、小猫的境界仅影响产生灵石的阶数和品数，不影响速度
         * 4、只有通过完成任务才能提高速度
         * 
         * 举例：10s产生一颗，3只小猫，一分钟就有6*3=18,2小时就有18*3600；
         * 同时会消耗丹药成为灵石；玩家主动收获，可提高丹药与灵石的兑换比例
         * 灵石会消耗成为小猫的修为
         * 晋级需要消耗小猫修为；如果不晋级，也需要灵石来维持小猫的修为
         * 
         * 
         * !!!!遗留问题：如果是离线的本地游戏，如果根据前后上线的时间随机提供灵石和灵丹的结果
         */

        //产生Lingdan计时器
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            //请求小猫控制器，获得每n秒小猫产生的灵丹数量
            List<int> lingdan = CatController.instance.spawnLingdan();
            int temp = 0;
            for (int i = 0; i < lingdan.Count; i++)
            {
                temp += lingdan[i];
            }
            lingdanNumber += temp;
            lingdanSpeed = temp;

            //刷新灵丹的文本
            lingdanText.text = NumberController.instance.NumberToChinaString((int)lingdanNumber);
            lingdanSpeedText.text = "+" + NumberController.instance.NumberToChinaString((int)lingdanSpeed);

            //在底部产生灵丹图标
            if (lingdan.Count > 0 && lingdanParent != null)
            {
                //spawnLingdan(lingdan);

                //lingshiNumber += (int)(Mathf.Pow(10, lingdan.rank + 1) * (lingdan.quality + 1) * PriceController.instance.price);
            }



            //重新赋值计时器
            timer = timeToLingdan;
        }

        if (MainController.instance.isFirstTimeGaming)
        {
            bottomTips.gameObject.SetActive(true);
            bottomTips.text = "提示：小猫会自行炼丹，放到藏丹阁";
        }

        if (lingdanNumber >= 3 && lingdanNumber <= 9)
        {
            bottomTips.gameObject.SetActive(true);
            bottomTips.text = "提示：点击灵丹可帮助小猫出售灵丹";
        }
        else
        {
            bottomTips.gameObject.SetActive(false);
        }

        if (maxLingdanNumber <= lingdanNumber)
        {
            bottomTips.gameObject.SetActive(true);
            bottomTips.text = "提示：灵丹已满，小猫会偷偷打折出售";
        }

        //每秒更新文本
        lingshiTimer -= Time.deltaTime;
        if (lingshiTimer <= 0)
        {
            //将灵石数量传给场景数据记录
            SceneTransferData.instance.getLingshiNumber = lingshiNumber;

            //更新灵石文本
            lingshiSpeed = lingshiNumber - beforlingshiNumber;
            lingshiText.text = NumberController.instance.NumberToChinaString((int)lingshiNumber);
            if (lingshiSpeed >= 0)
            {
                //如果灵丹价格处于高位，则高亮展示比例，否则正常展示 
                if (lingshiSpeed > 0 && PriceController.instance.isHighPirce)
                {
                    lingshiSpeedText.text = "+" + NumberController.instance.NumberToChinaString((int)lingshiSpeed);
                }
                else
                {
                    lingshiSpeedText.text = "+" + NumberController.instance.NumberToChinaString((int)lingshiSpeed);
                }
            }
            else
            {
                lingshiSpeedText.text = "-" + NumberController.instance.NumberToChinaString((int)lingshiSpeed);
            }
            beforlingshiNumber = lingshiNumber;


            //更新领土面积文本
            areaText.text = NumberController.instance.NumberToChinaString((int)territoryArea) + " 平方米";


            lingshiTimer = 1f;
        }

        //每秒判断是否需要做提醒
        tipsTimer -= Time.deltaTime;
        if (tipsTimer < 0)
        {
            //灵丹是否满了
            if (lingdanNumber >= maxLingdanNumber)
            {
                //Tips.instance.setTip(2);
            }

            //灵石是否过低（按10的2次方来算）
            if (lingshiNumber < Mathf.Pow(10, CatController.instance.getCatMaxLevel() + 2))
            {
                //Tips.instance.setTip(5);
            }

            tipsTimer = 1.0f;
        }

        //如果等待领取的灵丹数量大于0，说明产生了多余的灵丹未领取
        if (waitToGetLingDan > 0)
        {
            waitToGetObj.SetActive(true);
            waitToGetLingshiText.text = "离线收益\n" + NumberController.instance.NumberToChinaString(waitToGetLingshi);
        }
        else
        {
            //waitToGetObj.SetActive(false);
            waitToGetLingshiText.text = "离线挂机可获得海量收益";
        }

        //跟随弹窗展示banner广告
        if (BannerAd != null)
        {
            if (offlinePanelObj.activeSelf)
            {
                BannerAd.Show();
            }
            else
            {
                BannerAd.Hide();
            }
        }

    }


    //控制灵丹的生产、销售和布局。（传入参数为小猫一次性生产的灵丹数量，因此新增的数据也为一次性生产的数量）
    // void spawnLingdan(List<int> ldList)
    // {
    //     //先遍历灵石列表每阶的坑位，判断哪里产生了灵石，然后再判断数量
    //     for (int i = 0; i < ldList.Count; i++)
    //     {
    //         if (ldList[i] > 0)
    //         {
    //             //处理完溢出的灵丹后，继续产生新的灵丹
    //             for (int j = 0; j < ldList[i]; j++)
    //             {

    //                 //如果产生的灵丹数量超过最高数量，则需要把一个给卖掉
    //                 while (lingdanList.Count >= maxLingdanNumber)
    //                 {
    //                     saleLingdan(lingdanList[0], 0, 0);//清理第一个灵丹对象、位置在第一个、属于自动销售
    //                     lingdanList.RemoveAt(0);

    //                     GameObject t = lingdanUIList[0];
    //                     lingdanUIList.RemoveAt(0);
    //                     StartCoroutine(AnimLingdan(t));//协程运行销毁灵丹UI

    //                     //刷新灵丹的文本
    //                     lingdanNumber--;
    //                     lingdanText.text = NumberController.instance.NumberToChinaString((int)lingdanNumber);
    //                     lingdanSpeedText.text = "+" + NumberController.instance.NumberToChinaString((int)lingdanSpeed);
    //                 }


    //                 //生产灵丹的数据
    //                 LingDan temp = new LingDan();
    //                 temp.rank = i;

    //                 int probability = Random.Range(1, 100);
    //                 if (probability <= 10)
    //                 {
    //                     temp.quality = 2;
    //                 }
    //                 else if (10 < probability && probability <= 30)
    //                 {
    //                     temp.quality = 1;
    //                 }
    //                 else if (probability > 30)
    //                 {
    //                     temp.quality = 0;
    //                 }

    //                 lingdanList.Add(temp);


    //                 //生产灵丹的UI，控制一个个生产的
    //                 //spawnOneLingdanUI(temp.rank);
    //             }
    //         }
    //     }
    // }

    //产生灵丹UI，后面要根据灵丹的等级情况展示不同的UI（每次产生1个）
    void spawnOneLingdanUI(int rank)
    {
        //对目前的灵丹UI列表进行布局
        //showLingdan();


        // if (lingdanUIList.Count < maxLingdanUINumber)
        // {
        //     int needToSpawn;

        //     //maxLingdanUINumber - lingdanUIList.Count;——能够产生的数量
        //     //lingdanList.Count - lingdanUIList.Count;——需要产生的数量
        //     //需要产生的数量更多时，再产生最多能够产生的灵丹UI数量
        //     if (lingdanList.Count - lingdanUIList.Count > maxLingdanUINumber - lingdanUIList.Count)
        //     {
        //         //needToSpawn = maxLingdanUINumber - lingdanUIList.Count;
        //     }
        //     //需要产生的数量比能产生的数量小，或者相等时，最后生产实际需要的即可
        //     else
        //     {
        //         //needToSpawn = lingdanList.Count - lingdanUIList.Count;
        //     }
        //     Debug.Log("每次需要生产的灵丹数量" + needToSpawn);

        //     //多次产生灵丹UI
        //     for (int i = 0; i < needToSpawn; i++)
        //     {
        //         //产生一个灵丹在底部

        //         GameObject tempLingdan = Instantiate(lingdanToSpawn);


        //         //lingdanUIList.Add(tempLingdan);

        //         //将灵丹列表挂着父节点上，对获得的UI进行赋值
        //         tempLingdan.transform.SetParent(lingdanParent.transform, false);
        //         //tempLingdan.transform.localPosition = new Vector3(0, 0, 0);

        //         Image catIcon = tempLingdan.GetComponent<Image>();
        //         string path = "Materials/Logo/" + "灵丹" + rank.ToString();
        //         Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
        //         catIcon.sprite = sprite;

        //         //对目前的灵丹UI列表进行布局
        //         //showLingdan();

        //     }
        // }


    }

    //在底部面板展示灵丹logo，在此之前判断灵丹数量是否超过最大值。
    //同时，将最后每个灵丹logo可点击的事件进行绑定，在此之前把所有原来的绑定给解绑
    void showLingdan()
    {
        //将产生的灵丹对象进行排布
        // for (int i = 0; i < lingdanUIList.Count; i++)
        // {
        //     //lingdanUIList[i].transform.localPosition = new Vector3(60 + (i % 10) * 110, -(i / 10) * 110 - 70, 0);
        // }

        // //清空灵丹UI的点击事件，并重新绑定
        // for (int i = 0; i < lingdanUIList.Count; i++)
        // {
        //     int temp = i;
        //     // lingdanUIList[i].GetComponent<Button>().onClick.RemoveAllListeners();
        //     // lingdanUIList[i].GetComponent<Button>().onClick.AddListener(() => clickLingdan(temp));
        // }


    }

    //点击了第n个灵丹
    // public void clickLingdan(int number)
    // {
    //     if (number < lingdanList.Count)
    //     {

    //         //播放灵丹点击音效
    //         AudioManager.instance.PlayAudio(clickLingdanClip);

    //         Debug.Log("被点击命中的灵丹编号为：" + number);
    //         Debug.Log("灵丹列表数量：" + lingdanList.Count);
    //         /*
    //         for(int i = 0; i < lingdanList.Count; i++)
    //         {
    //             Debug.Log("遍历灵丹数据列表（等级）：" + lingdanList[i].rank);
    //             Debug.Log("遍历灵丹数据列表（质量）：" + lingdanList[i].quality);
    //         }
    //         */

    //         saleLingdan(lingdanList[number], number, 1);//销售被点中的灵丹、位置是number、属于手动点击
    //         lingdanList.RemoveAt(number);

    //         GameObject t = lingdanUIList[number];
    //         lingdanUIList.RemoveAt(number);

    //         StartCoroutine(AnimLingdan(t));//协程运行销毁灵丹UI


    //         //刷新灵丹的文本
    //         lingdanNumber--;
    //         lingdanText.text = NumberController.instance.NumberToChinaString((int)lingdanNumber);
    //         lingdanSpeedText.text = "+" + NumberController.instance.NumberToChinaString((int)lingdanSpeed);

    //         //刷新灵丹的UI
    //         if (number < lingdanList.Count)
    //         {
    //             //spawnOneLingdanUI(lingdanList[number].rank);
    //         }

    //     }

    // }

    //协程执行动画
    IEnumerator AnimLingdan(GameObject lingdan)
    {
        Debug.Log("播放动画，销毁一颗灵石UI");

        lingdan.transform.SetParent(destoryLingdanParent.transform, false);

        //lingdan.transform.localPosition
        lingdan.GetComponent<LingdanFadeAnimation>().isAnimate = true;//播放动画


        //等待2s执行下一语句
        yield return new WaitForSeconds(2.0f);
        Destroy(lingdan);



        Resources.UnloadUnusedAssets();
        System.GC.Collect();

    }



    //销售某个灵丹，根据阶数、品级销售不同的灵石
    /*灵丹和灵石的兑换关系，按10倍的关系算：
     * 一阶灵丹：1灵石
     * 二阶灵丹：10灵石
     * 三阶灵丹：100灵石
     * 四阶灵丹：1000灵石
     * 五阶灵丹：10000灵石
     * 
     * 低品：n * 1
     * 中品：n * 2
     * 高品：n * 5
     * 
     * type：销售的类型
     * 自动销售：0
     * 手动销售：1
     */
    // void saleLingdan(LingDan lingdan, int pos, int type)
    // {
    //     int plus = 0;

    //     plus = (int)(Mathf.Pow(10, lingdan.rank + 1) * (lingdan.quality + 1) * PriceController.instance.price);

    //     Transform temp = lingdanUIList[pos].transform.Find("PlusNumber(Clone)");

    //     if (temp != null)
    //     {
    //         if (type == 0)
    //         {
    //             plus = plus / 2;
    //             temp.GetComponent<TMP_Text>().text = "<color=#F32D2D>半价出售</color>";
    //         }
    //         else if (type == 1)
    //         {
    //             temp.GetComponent<TMP_Text>().text = "+" + plus.ToString();
    //         }


    //     }
    //     else
    //     {
    //         Debug.Log("灵石销售的数字有问题");
    //     }

    //     Debug.Log("本次销售获得灵石数：" + plus);

    //     lingshiNumber += plus;

    //     Debug.Log("销售了一个灵丹阶数为：" + lingdan.rank + "  品级为：" + lingdan.quality + "  最后总共灵石为：" + lingshiNumber);

    // }

    //生成等待领取的灵丹数量
    public void saleLingdanToWaitGet(int lingdanNumber)
    {
        int lingdanToSale = 0;
        for (int i = 0; i < lingdanNumber; i++)
        {

            lingdanToSale += (int)(10 * (CatController.instance.getCatMaxLevel() + 1)
                 * PriceController.instance.price);
        }

        waitToGetLingshi = lingdanToSale;
    }

    //获得离线收益的灵丹（默认获得1倍收益，可获得多倍收益）
    public void getWaitingLingdan(int time = 1)
    {
        lingshiNumber += waitToGetLingshi * time;
        waitToGetLingshi = 0;
        waitToGetLingDan = 0;
        offlinePanelObj.SetActive(false);
    }

    public void consumeLingShi(int number)
    {
        lingshiNumber -= number;
    }


    //点击离线收益，打开面板和广告
    public void ShowOfflinePanel()
    {
        //存在灵石待收获
        if (waitToGetLingDan > 0)
        {
            //打开面板
            offlinePanelObj.SetActive(true);
            waitLingshiNumberText.text = NumberController.instance.NumberToChinaString(waitToGetLingshi);


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
            // 正常播放结束，可以下发游戏奖励
            getWaitingLingdan(2);

            Debug.Log("测试广告成功");
        }
        else
        {
            // 播放中途退出，不下发游戏奖励
            Debug.Log("广告中途退出");
        }
    }

    public void CloseBannerAD()
    {
        //BannerAd.Destroy();
    }

    //获得资源池的对象，如果资源池为空，则返回1
    public GameObject GetObject()
    {
        if (pool.Count == 0)
        {
            return null;
        }

        GameObject obj = pool.Dequeue();
        obj.SetActive(true);
        return obj;
    }

    //将使用过的物体放回资源池
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

}

[Serializable]
public class LingDan
{
    public int rank;//阶数
    public int quality;//品阶
}