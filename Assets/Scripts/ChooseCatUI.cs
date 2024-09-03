using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WeChatWASM;

public class ChooseCatUI : MonoBehaviour
{
    public Image catIcon;//小猫头像
    public TMP_Text catName;//小猫名称
    public TMP_Text catIntro;//小猫简介
    public TMP_Text catCapacity;//小猫能力
    public TMP_Text catLevel;//小猫境界
    public TMP_Text stoneNumber;//携带灵石的数量
    public TMP_Text freshBtnText;//刷新按钮文案
    public Button freshBtn;//刷新按钮文案
    public Button shareBtn;//分享按钮

    public TMP_Text freshTips;//刷新用完提示

    private Cat currCat;//当前处理的小猫UI（单只）

    private int currFreshTime = 3;//当前可刷新的次数

    public static ChooseCatUI instance;
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
        if(currFreshTime <= 0)
        {

            shareBtn.gameObject.SetActive(true);
            freshTips.gameObject.SetActive(true);
            freshBtn.gameObject.SetActive(false);

            //freshBtn.interactable = false;
        }
        else
        {
            shareBtn.gameObject.SetActive(false);
            freshTips.gameObject.SetActive(false);
            freshBtn.gameObject.SetActive(true);
        }

        freshBtnText.text = "刷新(" + currFreshTime.ToString() + ")";
    }

    public void shareCat()
    {
        WX.ShareAppMessage(new ShareAppMessageOption
        {
            //imageUrl = imageUrl, // 图片的URL，也可以不填（自动截屏）
            title = "嘿嘿，我领养了1只猫猫", // 显示文本
            //query = query, // 附带参数，限制2k长度
        });

        currFreshTime = 5;
    }


    //在诞生小猫的面板上，展示小猫完整信息（传入参数：0，新增小猫；1，刷新小猫）
    public void newCatAndCatUI(int type)
    {
        if(type == 0)
        {
            //新增小猫
            currCat = CatController.instance.newCat(NewCatController.instance.time);

            currFreshTime = 3;
            freshBtn.interactable = true;
        }
        else if(type == 1)
        {
            //刷新小猫
            currCat = CatController.instance.newCat(NewCatController.instance.time);

            currFreshTime--;
        }
        else
        {
            Debug.Log("新增小猫出错!");
        }
        

        //更新面板小猫的信息
        string path = "Materials/BigCat/cat" + currCat.cat_icon;
        Debug.Log("成功诞生小猫，小猫头像 path:" + path);
        Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
        catIcon.sprite = sprite;

        catName.text = currCat.cat_name;
        catIntro.text = "性格：" + currCat.introuction;

        string capa = "";
        if (currCat.big_level == "练气期")
        {
            capa = "炼制一阶丹药";
            catLevel.text = "境界：<color=#000000>" + currCat.big_level + "</color> " + currCat.small_level.ToString() + " 层";
        }
        else if (currCat.big_level == "筑基期")
        {
            capa = "炼制二阶丹药";
            catLevel.text = "境界：<color=#19932D>" + currCat.big_level + "</color> " + currCat.small_level.ToString() + " 层";
        }
        else if (currCat.big_level == "金丹期")
        {
            capa = "炼制三阶丹药";
            catLevel.text = "境界：<color=#805C00>" + currCat.big_level + "</color> " + currCat.small_level.ToString() + " 层";
        }
        else if (currCat.big_level == "元婴期")
        {
            capa = "炼制四阶丹药";
            catLevel.text = "境界：<color=#F21BEA>" + currCat.big_level + "</color> " + currCat.small_level.ToString() + " 层";
        }
        else if (currCat.big_level == "化神期")
        {
            capa = "炼制五阶丹药";
            catLevel.text = "境界：<color=#FF1010>" + currCat.big_level + "</color> " + currCat.small_level.ToString() + " 层";
        }
        catCapacity.text = "能力：" + capa;
        
        stoneNumber.text = "灵石：<color=#2D5AFD>" + currCat.had_stone.ToString() + "</color>"; 

    }
    public void chooseCat()
    {
        CatController.instance.chooseCat(currCat);

        NewCatController.instance.outCatNumber--;
    }

}
