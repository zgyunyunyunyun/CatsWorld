using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpRaceControllerNew : MonoBehaviour
{
    public int raceLevel = 0;//当前种族等级，从0开始，即一阶

    public TMP_Text levelMain;//主界面展示种族等级
    public TMP_Text level;//展示种族等级
    public TMP_Text requestLevel;//展示要求达到的小猫等级
    public TMP_Text requestFish;//展示要求拥有的灵石数量

    public Button upBtn;//升级按钮
    public TMP_Text upBtnText;//升级按钮的文案
    public TMP_Text maxCatNumber;//最大小猫数量
    public TMP_Text nextMaxCatNumber;//升级后最大小猫数量
    public TMP_Text catNumber;//小猫数量及最大数量

    public Image btn1;//第一个条件是否达成
    public Image btn2;//第二个条件是否达成
    public Image btn3;//第三个条件是否达成

    public GameObject upPanel;//升级面板

    private List<Cat> cats;//遍历小猫的等级

    //晋级条件
    private bool levelCondition = false;
    private bool fishCondition = false;

    public TMP_Text nextRankText;//升级后当前等级
    public TMP_Text nextMaxCNumberText;//升级后当前小猫数量

    private float tipsTimer = 1.0f;//判断提醒的计时器

    public Image redPoint;//种族晋级的红点提示

    public static UpRaceControllerNew instance;
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
        //如果升级面板展示了，则不断刷新UI
        if (upPanel.gameObject.activeSelf)
        {

            level.text = changeLevel(raceLevel);
            requestLevel.text = RequestLevel();//要求的下一等级
            requestFish.text = RequestFish().ToString();

            maxCatNumber.text = $"小猫上限：{MaxCatNumber(raceLevel)}";
            nextMaxCatNumber.text = $"(升级：{MaxCatNumber(raceLevel + 1)})";

            if (raceLevel >= 4)
            {
                upBtnText.text = "已满级";
                upBtn.interactable = false;
            }

        }

        levelMain.text = changeLevel(raceLevel);
        catNumber.text = CatController.instance.cats.Count + " / " + MaxCatNumber(raceLevel).ToString();

        //发送可以升级的消息
        tipsTimer -= Time.deltaTime;
        if (tipsTimer < 0)
        {
            cats = CatController.instance.cats;

            //刷新等级
            bool ifLevelCondition = false;
            //遍历小猫，对比小猫等级是否符合条件
            for (int i = 0; i < cats.Count; i++)
            {
                //小猫等级符合下一等级的条件
                if (cats[i].level / 20 > raceLevel)
                {
                    ifLevelCondition = true;
                    break;

                }
                else
                {
                    ifLevelCondition = false;
                }
            }

            if (ifLevelCondition)
            {
                levelCondition = true;
            }
            else
            {
                levelCondition = false;
            }

            //是否满足鱼的条件
            if (PropertyController.instance.lingshiNumber >= RequestFish())
            {
                //改变色块
                fishCondition = true;
            }
            else
            {
                fishCondition = false;
            }

            //小猫数量达到最大
            if (CatController.instance.cats.Count >= MaxCatNumber(raceLevel))
            {
                Debug.Log("小猫数量达到最大");
                //Tips.instance.setTip(8);
            }

            tipsTimer = 1.0f;
        }


        //是否打开升级面板
        if (upPanel.gameObject.activeSelf)
        {

            //小猫等级符合下一等级的条件
            if (levelCondition)
            {
                //改变色块
                btn1.GetComponent<Image>().color = Color.green;
            }
            else
            {
                btn1.GetComponent<Image>().color = Color.grey;
            }

            //是否满足灵石条件
            if (fishCondition)
            {
                //改变色块
                btn2.GetComponent<Image>().color = Color.green;
            }
            else
            {
                btn2.GetComponent<Image>().color = Color.grey;
            }

            //3个条件都满足，按钮改变
            if (fishCondition && levelCondition)
            {
                upBtnText.text = "未满足";
                upBtn.interactable = true;
            }
            else
            {
                upBtnText.text = "未满足";
                upBtn.interactable = false;
            }
        }


    }



    //升级小猫
    public void upLevel()
    {
        //消耗鱼
        PropertyController.instance.lingshiNumber -= RequestFish();

        //升级成功，等级加一，同时改变最大小猫数量和最大灵丹数量
        raceLevel++;

        //改变成功面板的UI

        nextRankText.text = "当前等级：" + changeLevel(raceLevel);
        nextMaxCNumberText.text = "最大小猫数量:+" + (MaxCatNumber(raceLevel) - MaxCatNumber(raceLevel - 1)).ToString();
    }

    //将等级转为中文
    string changeLevel(int rLevel)
    {
        return (rLevel + 1).ToString() + "级";
    }

    //输出下一等级的要求1
    string RequestLevel()
    {
        if (raceLevel == 0)
        {
            return "20级";
        }
        else if (raceLevel == 1)
        {
            return "40级";
        }
        else if (raceLevel == 2)
        {
            return "60级";
        }
        else if (raceLevel == 3)
        {
            return "80级";
        }

        return " ";
    }

    //输出下一等级的吃鱼数量要求2
    int RequestFish()
    {
        return 100 * (int)Mathf.Pow(100, raceLevel + 1);
    }

    //输出当前等级最大小猫数量
    public int MaxCatNumber(int rLevel)
    {
        return 10 * (int)Mathf.Pow(2, rLevel);
    }
}
