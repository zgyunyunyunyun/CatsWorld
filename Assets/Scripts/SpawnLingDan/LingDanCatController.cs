using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;
using Random = UnityEngine.Random;

public class LingDanCatController : MonoBehaviour
{
    public GameObject cat;//待实例化的小猫
    public GameObject catParent;//父节点

    public int catTypeNumber;//小猫的类型数量-1

    public List<GameObject> catList = new List<GameObject>();//产生小猫的列表

    public GameObject barParent;//stayBar的父节点
    public List<StayBarCat> stayCatList = new List<StayBarCat>();//stayBar的小猫列表

    public List<int> catTypeAmount = new List<int>();//生成的各种小猫类型数量，最后和生产的任务要保持一致

    public AudioClip clickCatClip;//点击小猫的音效

    private bool canRefresh = false;//是否能重置小猫
    private bool canRemove = false;//是否能移出小猫

    public GameObject refreshRedPoint;//可重置小猫的红点
    public GameObject removeRedPoint;//可移出小猫的红点

    public GameObject refreshObj;//可重置小猫的按钮
    public GameObject removeObj;//可移出小猫的按钮

    WXRewardedVideoAd refreshVideoAd;//广告位初始化
    WXRewardedVideoAd removeVideoAd;//广告位初始化

    public Image refreshIcon;//刷新的icon
    public Image removeIcon;//移走的icon


    public static LingDanCatController instance;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //重置的广告位
        refreshVideoAd = WX.CreateRewardedVideoAd(
        new WXCreateRewardedVideoAdParam()
        {
            adUnitId = "adunit-20bf7db8fdd5abf9",
            multiton = true
        });

        refreshVideoAd.OnClose(ReFreshAdClose);

        //移除的广告位
        removeVideoAd = WX.CreateRewardedVideoAd(
        new WXCreateRewardedVideoAdParam()
        {
            adUnitId = "adunit-4c910c9b8601c06c",
            multiton = true
        });

        removeVideoAd.OnClose(RemoveAdClose);
    }

    // Update is called once per frame
    void Update()
    {


    }

    //根据关卡难度产生小猫（参数：当前关卡生成小猫的数量）
    public void SpawnCat(int catNumber)
    {
        //广告按钮展示出来
        if (!refreshObj.activeSelf)
        {
            refreshObj.gameObject.SetActive(true);
        }
        if (!removeObj.activeSelf)
        {
            removeObj.gameObject.SetActive(true);
        }

        //初始化数据（stayBar数据，小猫类型数据，小猫数据）
        initStayBarList();

        //初始化各类小猫的数量
        catTypeAmount.Clear();
        int amount = 0;
        for (int i = 0; i < TaskController.instance.cType; i++)
        {
            catTypeAmount.Add(amount);
        }

        //重置小猫的列表
        if (catList.Count > 0)
        {
            Debug.Log("重置小猫，原本列表数量为：" + catList.Count);
            for (int i = 0; i < catList.Count; i++)
            {
                if (catList[i] != null)
                {
                    Destroy(catList[i]);
                }
            }
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            catList.Clear();
        }


        //用于记录每个小猫图片需要往下的位置
        int[,] siteFlag = new int[4, 4];
        for (int a = 0; a < 4; a++)
        {
            for (int b = 0; b < 4; b++)
            {
                siteFlag[a, b] = 0;
            }
        }

        //产生新的小猫，一次生成3只
        int initCatNumber = 0;
        for (int j = 0; j < catNumber / 3; j++)
        {

            int catType = Random.Range(0, catTypeNumber);
            Debug.Log("生成的小猫类型：" + catType);
            catTypeAmount[catType]++;

            for (int i = 0; i < 3; i++)
            {
                GameObject temp = Instantiate(cat);
                catList.Add(temp);

                //挂在父节点
                temp.transform.SetParent(catParent.transform, false);

                //随机产生小猫位置
                int x = Random.Range(-2, 2);
                int y = Random.Range(-2, 2);
                temp.transform.localPosition = new Vector3(250 * x + siteFlag[x + 2, y + 2] * 3, 250 * y + siteFlag[x + 2, y + 2] * 3, 0);

                siteFlag[x + 2, y + 2] += 1;

                //展示图片
                Image catIcon = temp.GetComponent<Image>();
                string path = "Materials/Cat/cat" + catType.ToString();
                Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
                catIcon.sprite = sprite;


                //添加点击事件
                int num = initCatNumber;
                int ctype = catType;
                temp.GetComponent<Button>().onClick.AddListener(() => clickCat(ctype, num));

                initCatNumber++;
            }
        }

        Debug.Log("最终产生了" + catNumber + "只小猫");

    }

    void initStayBarList()
    {
        Debug.Log("初始化stayBar");
        if (stayCatList.Count > 0)
        {
            Debug.Log("初始化stayBar，判断list数量：" + stayCatList.Count);
            //初始化stayBar的数据及样式
            for (int i = 0; i < 6; i++)
            {
                stayCatList[i].catType = -1;
                stayCatList[i].catNumber = -1;
                stayCatList[i].hasCat = false;
                if (stayCatList[i].cat != null)
                {
                    Debug.Log("初始化stayBar，清理的小猫对象序号：" + i);
                    Destroy(stayCatList[i].cat);
                }
            }
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
        else
        {
            Debug.Log("首次初始化stayBar");
            //初始化stayBar的数据及样式
            for (int i = 0; i < 6; i++)
            {
                StayBarCat tempCat = new StayBarCat();
                tempCat.emptyObj = barParent.transform.GetChild(i).gameObject;
                tempCat.catType = -1;
                tempCat.catNumber = -1;
                tempCat.hasCat = false;
                stayCatList.Add(tempCat);
            }
        }

    }

    //点击了小猫（类型，第n位）
    void clickCat(int type, int catNumber)
    {
        //播放小猫音效
        AudioManager.instance.PlayAudio(clickCatClip);

        //初始化小猫数据及信息
        catList[catNumber].GetComponent<Button>().interactable = false;

        List<Task> taskDataList = TaskController.instance.taskDataList;

        //判断点击的小猫是否在任务中，输出任务的位置和任务内小猫的位置（第几个丹炉，丹炉的第几只小猫）
        bool isTask = false;
        int fSite = 0;
        GameObject furnace = null;
        int cSite = 0;
        for (int i = 0; i < taskDataList.Count; i++)
        {
            //Debug.Log("i的顺序：" + fSite);
            if (taskDataList[i].catType == type && taskDataList[i].catchNumber < 3)
            {
                isTask = true;
                fSite = i;
                cSite = taskDataList[i].catchNumber;//实际上代表有多少个小猫命中了任务
                furnace = TaskController.instance.taskList[i];
                Debug.Log("丹炉位置：" + fSite);
                Debug.Log("小猫位置：" + cSite);

                break;
            }
        }

        //获得丹炉的小猫该去的位置
        Transform temp = TaskController.instance.taskList[fSite].transform.Find("MainObject").Find("CatPos" + cSite.ToString());

        //若小猫在任务中，则播放移动到丹炉的动画，去任务中；如果不在则去掉stayBar
        if (isTask && temp != null && TaskController.instance.taskDataList[fSite].catchNumber < 3)
        {
            //获得在哪个任务的某个位置的小猫            
            Transform catTran = temp.transform;

            //移动小猫，并隐藏后续移除小猫在当前列表的对象
            catList[catNumber].GetComponent<LingdanCat>().moveCat(catTran.position, type, fSite, furnace, cSite, catNumber);
            //catList[catNumber].GetComponent<LingdanCat>().moveCat(catTran.position, type, cSite, catNumber);

            //增加完成任务的小猫数据
            TaskController.instance.taskDataList[fSite].catchNumber++;

        }
        else
        {
            //小猫可以落在哪个位置            
            int stayCatNumber = 0;//用于判断第几个位置没有小猫，并记录下来
            for (int i = 0; i < stayCatList.Count; i++)
            {
                //记录有小猫的数量
                if (stayCatList[i].hasCat)
                {
                    stayCatNumber++;
                }
                else//如果存在没有小猫的情况，则中断遍历（默认后面列表也没有小猫了），因此，stayCatNumber就是没有小猫的位置，如果大于等于6，则说明小猫满了
                {
                    break;
                }
            }

            //数量小于6，则列表没有满，可添加小猫。同时stayCatNumber即可添加的位置
            if (stayCatNumber < 6)
            {
                Transform catTran = stayCatList[stayCatNumber].emptyObj.transform;

                //移动小猫，并隐藏后续移除小猫在当前列表的对象
                catList[catNumber].GetComponent<LingdanCat>().moveCatToStay(catTran.position, type, stayCatNumber, catNumber);
            }
            else//否则，游戏结束
            {
                Debug.Log("未收集的小猫达到6个");
                TaskLevelController.instance.gameFailed();

            }


        }


    }

    //将小猫添加到stayBar上
    public void addStayCat(int catType, int stayCatNumber, int catNumber)
    {
        //赋予第pos个位置小猫的数据
        stayCatList[stayCatNumber].hasCat = true;
        stayCatList[stayCatNumber].catType = catType;
        stayCatList[stayCatNumber].catNumber = catNumber;
        stayCatList[stayCatNumber].cat = catList[catNumber];

        //重新判断一遍游戏是否结束
        int sNumber = 0;
        for (int i = 0; i < stayCatList.Count; i++)
        {
            //记录有小猫的数量
            if (stayCatList[i].hasCat)
            {
                sNumber++;
            }
            else//如果存在没有小猫的情况，则中断遍历（默认后面列表也没有小猫了），因此，stayCatNumber就是没有小猫的位置，如果大于等于6，则说明小猫满了
            {
                break;
            }
        }

        if (sNumber >= 6)
        {
            Debug.Log("未收集的小猫达到6个");
            TaskLevelController.instance.gameFailed();
        }
    }


    //点击了重置按钮
    public void ClickRefreshBtn()
    {
        if (canRefresh)
        {
            RefreshCatPosition();

            string path = "Materials/Logo/vedio";
            Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
            refreshIcon.sprite = sprite;

            canRefresh = false;
            refreshObj.gameObject.SetActive(false);
            refreshRedPoint.gameObject.SetActive(false);
        }
        else
        {
            WatchAddToRefresh();
        }
    }

    //点击了移出按钮
    public void ClickRemoveBtn()
    {
        if (canRemove)
        {
            RemoveCatOutStayBar();


            string path = "Materials/Logo/vedio";
            Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
            removeIcon.sprite = sprite;

            canRemove = false;
            removeObj.gameObject.SetActive(false);
            removeRedPoint.gameObject.SetActive(false);
        }
        else
        {
            WatchAddToRemove();

        }
    }

    //为了重置而看视频
    public void WatchAddToRefresh()
    {
        if (refreshVideoAd != null)
        {
            refreshVideoAd.Show();
            Debug.Log("激励广告展示");
        }

    }

    //为了移除而看视频
    public void WatchAddToRemove()
    {
        if (refreshVideoAd != null)
        {
            removeVideoAd.Show();
            Debug.Log("激励广告展示");
        }

    }

    //关闭广告事件监听-重置
    void ReFreshAdClose(WXRewardedVideoAdOnCloseResponse res)
    {
        if ((res != null && res.isEnded) || res == null)
        {
            // 正常播放结束，可以下发游戏奖励
            canRefresh = true;
            refreshRedPoint.gameObject.SetActive(true);
            string path = "Materials/Logo/use";
            Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
            refreshIcon.sprite = sprite;
            Debug.Log("测试广告成功");
        }
        else
        {
            // 播放中途退出，不下发游戏奖励
            Debug.Log("广告中途退出");
        }
    }

    //关闭广告事件监听-移除
    void RemoveAdClose(WXRewardedVideoAdOnCloseResponse res)
    {
        if ((res != null && res.isEnded) || res == null)
        {
            // 正常播放结束，可以下发游戏奖励
            canRemove = true;
            removeRedPoint.gameObject.SetActive(true);
            string path = "Materials/Logo/use";
            Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
            removeIcon.sprite = sprite;
            Debug.Log("测试广告成功");
        }
        else
        {
            // 播放中途退出，不下发游戏奖励
            Debug.Log("广告中途退出");
        }
    }

    //为了重置而分享
    public void ShareToRefresh()
    {
        WX.ShareAppMessage(new ShareAppMessageOption
        {
            //imageUrl = imageUrl, // 图片的URL，也可以不填（自动截屏）
            title = "嘿嘿，我正在让小猫帮忙炼制灵丹", // 显示文本
            //query = query, // 附带参数，限制2k长度
        });
    }

    //为了移出小猫而分享
    public void ShareToRemove()
    {
        WX.ShareAppMessage(new ShareAppMessageOption
        {
            //imageUrl = imageUrl, // 图片的URL，也可以不填（自动截屏）
            title = "嘿嘿，我正在让小猫帮忙炼制灵丹", // 显示文本
            //query = query, // 附带参数，限制2k长度
        });
    }

    //重新布局小猫的位置
    public void RefreshCatPosition()
    {
        //用于记录每个小猫图片需要往下的位置
        int[,] siteFlag = new int[4, 4];
        for (int a = 0; a < 4; a++)
        {
            for (int b = 0; b < 4; b++)
            {
                siteFlag[a, b] = 0;
            }
        }

        for (int i = 0; i < catList.Count; i++)
        {
            GameObject temp = catList[i];

            bool ifNext = false;
            for (int j = 0; j < stayCatList.Count; j++)
            {
                if (stayCatList[j].cat == temp)
                {
                    ifNext = true;
                    break;
                }
            }

            if (!ifNext)
            {
                //随机产生小猫位置
                int x = Random.Range(-2, 2);
                int y = Random.Range(-2, 2);
                temp.transform.localPosition = new Vector3(250 * x + siteFlag[x + 2, y + 2] * 8, 250 * y + siteFlag[x + 2, y + 2] * 8, 0);

                siteFlag[x + 2, y + 2] += 1;
            }
        }

    }

    //将小猫移出stayBar
    public void RemoveCatOutStayBar()
    {
        for (int i = 0; i < stayCatList.Count; i++)
        {
            if (stayCatList[i].cat != null)
            {
                stayCatList[i].cat.GetComponent<LingdanCat>().moveToStay = false;
                stayCatList[i].cat.transform.localPosition += new Vector3(0, -200, 0);

                stayCatList[i].cat.GetComponent<Button>().interactable = true;

                stayCatList[i].catType = -1;
                stayCatList[i].catNumber = -1;
                stayCatList[i].hasCat = false;
                if (stayCatList[i].cat != null)
                {
                    Debug.Log("初始化stayBar，清理的小猫对象序号：" + i);
                    stayCatList[i].cat = null;
                }
            }
        }
    }
}



[Serializable]
public class StayBarCat
{
    public int catNumber;//小猫的顺序
    public int catType;//小猫icon类型
    public GameObject emptyObj;//小猫的各个节点对象，主要用来存储位置
    public bool hasCat;//该节点是否有小猫
    public GameObject cat;//当前所在的小猫对象
}