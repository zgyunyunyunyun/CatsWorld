using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpRaceController : MonoBehaviour
{
    public int raceLevel = 0;//当前种族等级，从0开始，即一阶

    public TMP_Text levelMain;//主界面展示种族等级
    public TMP_Text level;//展示种族等级
    public TMP_Text requestLevel;//展示要求达到的小猫等级
    public TMP_Text requestStone;//展示要求拥有的灵石数量
    public TMP_Text requestArea;//展示要求拥有的领土面积

    public Button upBtn;//升级按钮
    public TMP_Text upBtnText;//升级按钮的文案
    public TMP_Text maxCatNumber;//最大小猫数量
    public TMP_Text maxLingDanNumber;//最大灵丹数量
    public TMP_Text catNumber;//小猫数量及最大数量

    public Image btn1;//第一个条件是否达成
    public Image btn2;//第二个条件是否达成
    public Image btn3;//第三个条件是否达成

    public GameObject upPanel;//升级面板

    private List<Cat> cats;//遍历小猫的等级

    //晋级条件
    private bool levelCondition = false;
    private bool stoneCondition = false;
    private bool areaCondition = false;

    public TMP_Text nextRankText;//升级后当前等级
    public TMP_Text newtMaxLDNumberText;//升级后当前灵丹数量
    public TMP_Text nextMaxCNumberText;//升级后当前小猫数量

    private float tipsTimer = 1.0f;//判断提醒的计时器

    public Image redPoint;//种族晋级的红点提示

    public static UpRaceController instance;
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
            
            level.text = changeLevel(raceLevel) + "种族";
            requestLevel.text = RequestLevel();//要求的下一等级
            requestStone.text = RequestStone().ToString() + "灵石";
            requestArea.text = RequestTerritoryArea().ToString() + "平方米";

            maxCatNumber.text = "小猫最大数量：" + MaxCatNumber(raceLevel).ToString();
            maxLingDanNumber.text = "灵丹最大数量：" + MaxLingDanNumber(raceLevel).ToString();

            if(raceLevel >= 4)
            {
                upBtnText.text = "已达最高等级";
                upBtn.interactable = false;
            }
            
        }

        levelMain.text = changeLevel(raceLevel) + "种族";
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
                if (CatController.instance.levelStringToNumber(cats[i].big_level) > raceLevel)
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

            //是否满足灵石条件
            if (PropertyController.instance.lingshiNumber >= RequestStone())
            {
                //改变色块
                stoneCondition = true;
            }
            else
            {
                stoneCondition = false;
            }

            //是否满足领土条件
            if (PropertyController.instance.territoryArea >= RequestTerritoryArea())
            {
                areaCondition = true;
            }
            else
            {
                areaCondition = false;
            }

            //小猫数量达到最大
            if (CatController.instance.cats.Count >= MaxCatNumber(raceLevel))
            {
                Debug.Log("小猫数量达到最大");
                //Tips.instance.setTip(8);
            }

            //达到种族晋级条件
            if (areaCondition && levelCondition && stoneCondition)
            {
                Debug.Log("达到种族晋级的条件");
                //Tips.instance.setTip(9);

                redPoint.gameObject.SetActive(true);
            }
            else
            {
                redPoint.gameObject.SetActive(false);
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
            if (stoneCondition)
            {
                //改变色块
                btn2.GetComponent<Image>().color = Color.green;
            }
            else
            {
                btn2.GetComponent<Image>().color = Color.grey;
            }

            //是否满足领土条件
            if (areaCondition)
            {
                //改变色块
                btn3.GetComponent<Image>().color = Color.green;
            }
            else
            {
                btn3.GetComponent<Image>().color = Color.grey;
            }

            //3个条件都满足，按钮改变
            if (stoneCondition && levelCondition && areaCondition)
            {
                upBtn.interactable = true;
            }
            else
            {
                upBtn.interactable = false;
            }
        }


    }



    //升级小猫
    public void upLevel()
    {
        //消耗灵石
        PropertyController.instance.lingshiNumber -= RequestStone();

        //升级成功，等级加一，同时改变最大小猫数量和最大灵丹数量
        raceLevel++;
        PropertyController.instance.maxLingdanNumber = MaxLingDanNumber(raceLevel);
        PropertyController.instance.maxLingdanUINumber = MaxLingDanNumber(raceLevel);

        //改变成功面板的UI

        nextRankText.text = "当前等级：" + changeLevel(raceLevel) + "种族";
        newtMaxLDNumberText.text = "最大灵丹数量：+" + (MaxLingDanNumber(raceLevel) - MaxLingDanNumber(raceLevel-1)).ToString() ;
        nextMaxCNumberText.text = "最大小猫数量:+" + (MaxCatNumber(raceLevel) - MaxCatNumber(raceLevel - 1)).ToString();

        
    }

    //将等级转为中文
    string changeLevel(int rLevel)
    {
        if(rLevel == 0)
        {
            return "一阶";
        }else if(rLevel == 1)
        {
            return "二阶";
        }
        else if (rLevel == 2)
        {
            return "三阶";
        }
        else if (rLevel == 3)
        {
            return "四阶";
        }
        else if (rLevel == 4)
        {
            return "五阶";
        }

        return " ";
    }

    //输出下一等级的要求1
    string RequestLevel()
    {
        if (raceLevel == 0)
        {
            return "筑基期";
        }
        else if (raceLevel == 1)
        {
            return "金丹期";
        }
        else if (raceLevel == 2)
        {
            return "元婴期";
        }
        else if (raceLevel == 3)
        {
            return "化神期";
        }

        return " ";
    }

    //输出下一等级的灵石要求2
    int RequestStone()
    {
        return 5000 * (int)Mathf.Pow(10, raceLevel);
    }

    //输出下一等级的领土面积要求
    int RequestTerritoryArea()
    {
        return 10 * (int)Mathf.Pow(5, raceLevel + 1);
    }

    //输出当前等级最大小猫数量
    public int MaxCatNumber(int rLevel)
    {
        return 10 * (rLevel + 1);
    }

    //输出当前等级最大Lingdan数量
    int MaxLingDanNumber(int rLevel)
    {
        return 10 * (rLevel + 2);
    }
}
