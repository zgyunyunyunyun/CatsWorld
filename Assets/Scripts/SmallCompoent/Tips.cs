using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.IO;
using System.Text;

public class Tips : MonoBehaviour
{
    public GameObject tipsUIObj;//tipsUI的对象
    public TMP_Text text;//文本
    public Image catIcon;//小猫头像

    public GameObject catDetailPanel;//小猫详情页

    public float detectTimeGap = 1.0f;//隔ns检测一次tips的状态（发送消息的间隔）
    private float detectTimer = 1.0f;//隔ns检测一次tips的状态

    public float clearTimeGap = 30.0f;//隔30s清理一次被点击过的数据
    private float clearTimer = 30.0f;//隔30s清理一次被点击过的数据

    private List<TipsData> tipsList = new List<TipsData>();

    private bool isShowed = false;//是否展示第一次的任务故事

    public static Tips instance;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        if (MainController.instance.isFirstTimeGaming && !isShowed)
        {
            /*每次开局，自动提示如下消息：
            * 0：小猫炼制灵丹说明提醒
            * 1：小猫出售灵丹提醒
            * 4、剩余流浪小猫提醒
            */

            setTip(0);
            setTip(1);
            //setTip(4);

            isShowed = true;
        }

        //遍历tips列表，展示高优的内容
        detectTimer -= Time.deltaTime;
        if (MainController.instance.isFirstTimeGaming && detectTimer < 0)
        {
            Debug.Log("测试tip的时间周期：" + detectTimer);
            Debug.Log("当前tip的列表数量：" + tipsList.Count);
            for (int i = tipsList.Count - 1; i >= 0; i--)
            {
                Debug.Log("当前tip第" + i + "个是否被点击过：" + tipsList[i].clicked);
            }

            TipsData tipToShow = null;//记录需要展示的tip数据
            int tipPos = -1;//记录tip的位置，用于清理后删除数据
            int unclicked = -1;//记录后边第一个没有被点击过的数据

            if (tipsList.Count > 0)
            {
                //从后边遍历列表，找到第一个没有被点击过的tip
                for (int i = tipsList.Count - 1; i >= 0; i--)
                {
                    if (!tipsList[i].clicked)
                    {
                        tipToShow = tipsList[i];
                        tipPos = i;
                        tipsUIObj.gameObject.SetActive(true);

                        unclicked = i;
                        Debug.Log("当前有tip提示语事件发生，其序号为：" + i);
                        break;
                    }
                }


            }
            else
            {
                tipsUIObj.gameObject.SetActive(false);
                Debug.Log("当前没有tip提示语事件发生");
            }

            //从没有被点击过的数据往前找
            for (int i = unclicked - 1; i >= 0; i--)
            {
                if (!tipsList[i].clicked && tipsList[i].priority > tipToShow.priority)
                {
                    tipToShow = tipsList[i];
                    tipPos = i;
                }
            }

            if (tipToShow != null)
            {
                //设置文本
                setText(tipToShow.content);

                //设置头像
                if (tipToShow.catNumber >= 0)
                {
                    setIcon(tipToShow.catNumber);
                }


                //清空点击事件的绑定
                tipsUIObj.transform.GetComponent<Button>().onClick.RemoveAllListeners();

                //设置点击事件
                if (tipToShow.catNumber >= 0)
                {
                    //进入详情页并关闭toast
                    tipsUIObj.transform.GetComponent<Button>().onClick.AddListener(() => showCatDetail(tipToShow.catNumber));
                    tipsUIObj.transform.GetComponent<Button>().onClick.AddListener(() => closeTips(tipPos));
                }
                else
                {
                    //关闭toast
                    tipsUIObj.transform.GetComponent<Button>().onClick.AddListener(() => closeTips(tipPos));

                }

            }

            detectTimer = detectTimeGap;
        }

        //清理被点击过的tips
        clearTimer -= Time.deltaTime;
        if (clearTimer < 0)
        {
            for (int i = tipsList.Count - 1; i >= 0; i--)
            {
                //被点击过
                if (tipsList[i].clicked)
                {
                    tipsList.RemoveAt(i);
                }
            }

            clearTimer = clearTimeGap;
        }
    }

    //设置tips的内容及跳转
    public void setTip(int type, int catNumber = -1)
    {

        bool ifSet = true;
        for (int i = 0; i < tipsList.Count; i++)
        {
            //判断该消息是否已在list里
            if (type == tipsList[i].type && catNumber == tipsList[i].catNumber)
            {
                //Debug.Log("该消息已存在列表中，不再收纳");

                ifSet = false;
                break;
            }

        }

        if (ifSet)
        {
            spawnTipData(type, catNumber);
        }

    }

    public void spawnTipData(int type, int catNumber)
    {
        Debug.Log("新增消息类型为：" + type);
        Debug.Log("新增消息的小猫编号为：" + catNumber);

        //初始化中间参数
        string content = "";
        int priority = -1;

        /*提示类型：
        * 0：小猫炼制灵丹说明提醒.。
        * 1：小猫出售灵丹提醒.。
        * 2：灵丹数量达到上限，打折出售提醒。
        * 3：小猫灵石不足.。
        * 4、剩余流浪小猫提醒.。
        * 5、玩家灵石不足提醒。
        * 6、灵丹价格上涨提醒
        * 7、小猫晋级提醒。
        * 8、小猫数量达到上限提醒。
        * 9、种群达到晋升条件提醒。
        */
        //判断消息类型
        if (type == 0)
        {
            content = "小猫会把炼制的灵丹放到藏丹阁";
            priority = 100;
        }
        else if (type == 1)
        {
            content = "点击灵丹，可帮助小猫 <b><color=#F32D2D>原价</color></b> 出售灵丹";
            priority = 99;

        }
        else if (type == 2)
        {
            content = "灵丹数量超过上限，小猫正在 <b><color=#F32D2D>打折</color></b> 出售";
            priority = 1;

        }
        else if (type == 3)
        {
            content = "小鱼不足";
            priority = 2;

        }
        /*else if (type == 4)//每次打开游戏仅触发1次
        {
            
            content = "发现了 <b><color=#F32D2D>" + NewCatController.instance.outCatNumber.ToString() + "</color></b> 只小猫在外流浪";
            priority = 1;
        }*/
        else if (type == 5)
        {
            content = "灵石数量较少，<b><color=#F32D2D>开始炼丹</color></b>可以帮助小猫炼丹，快速获得灵石";
            priority = 1;

        }
        else if (type == 6)
        {
            content = "<b><color=#F32D2D>15分钟内</color></b>灵丹价格飙升，快呼唤小猫炼制灵丹";
            priority = 2;

        }
        else if (type == 7)
        {
            content = "喵喵，小猫修为已满，可以升级了";
            priority = 1;
        }
        else if (type == 8)
        {
            content = "小猫数量达到上限，提高世界等级才能收养更多小猫";
            priority = 1;
        }
        else if (type == 9)
        {
            content = "喵喵，猫群已满足晋级条件，快去升级";
            priority = 1;
        }

        //产生tip数据对象
        TipsData tip = new TipsData();
        tip.type = type;
        tip.catNumber = catNumber;
        tip.content = content;
        tip.priority = priority;
        tip.clicked = false;

        //添加到数据列表里
        tipsList.Add(tip);
    }

    //设置tips的文本
    public void setText(string content)
    {
        text.text = content;
    }

    //输入改变小猫icon的类型（参数：小猫类型，提示类型，小猫顺序）
    public void setIcon(int catNumber)
    {
        Debug.Log("输入改变消息的小猫icon的编号：" + catNumber);
        string path = "Materials/BigCat/cat" + CatController.instance.cats[catNumber].cat_icon;
        Debug.Log("小猫头像 path:" + path);
        Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
        catIcon.sprite = sprite;

    }

    //绑定点击事件，可打开小猫详情页
    public void showCatDetail(int catNumber)
    {
        Debug.Log("点击的小猫序号：" + catNumber);
        catDetailPanel.gameObject.SetActive(true);
        CatDetailController.instance.showCatUI(catNumber);
    }

    public void closeTips(int tipPos)
    {
        Debug.Log("被点击的tip顺序为：" + tipPos);
        tipsList[tipPos].clicked = true;
        tipsUIObj.gameObject.SetActive(false);
    }

}

[Serializable]
public class TipsData
{
    /*提示类型：
     * 0：小猫炼制灵丹说明提醒
     * 1：小猫出售灵丹提醒
     * 2：灵丹数量达到上限，打折出售提醒
     * 3：小猫灵石不足
     * 4、剩余流浪小猫提醒
     * 5、玩家灵石不足提醒
     * 6、灵丹价格上涨提醒
     * 7、小猫晋级提醒
     * 8、小猫数量达到上限提醒
     * 9、种群达到晋升条件提醒
     */
    public int type;//提示类型
    public int catNumber;//小猫顺序
    public string content;//提示内容

    /*数值越大，优先级越高
     * 
     * 新手引导提示：从100开始
     * 
     * 常规提醒：从0开始
     * 0：非重要提醒
     * 1：促进玩家行为提醒
     * 2：重要晋级的提醒
     */
    public int priority;//展示的优先级

    public bool clicked;//是否已经被点击过
}