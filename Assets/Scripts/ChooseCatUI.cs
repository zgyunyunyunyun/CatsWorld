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
    public TMP_Text eatFish;//小猫吃鱼数量
    public TMP_Text stoneNumber;//携带灵石的数量
    public TMP_Text freshBtnText;//刷新按钮文案
    public Button freshBtn;//刷新按钮文案
    public Button shareBtn;//分享按钮
    public Image rankPicture;

    public TMP_Text freshTips;//刷新用完提示

    private CatLogic currCatLogic;
    private Cat currCat;//当前处理的小猫UI（单只）

    private int currFreshTime = 3;//当前可刷新的次数

    public static ChooseCatUI instance;

    WXRewardedVideoAd refreshVideoAd;//广告位初始化

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
            adUnitId = "adunit-69b4b66718060505",
            multiton = true
        });

        refreshVideoAd.OnClose(RefreshAdClose);
    }

    // Update is called once per frame
    void Update()
    {
        if (currFreshTime <= 0)
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

        currFreshTime = 3;
    }


    //在诞生小猫的面板上，展示小猫完整信息（传入参数：0，新增小猫；1，刷新小猫）
    public void newCatAndCatUI(int type)
    {
        if (type == 0)
        {
            //新增小猫
            currCatLogic = CatController.instance.newCat(NewCatController.instance.time);
            currCat = currCatLogic.CatData;

            currFreshTime = 3;
            freshBtn.interactable = true;
        }
        else if (type == 1)
        {
            //刷新小猫
            currCatLogic = CatController.instance.newCat(NewCatController.instance.time);
            currCat = currCatLogic.CatData;

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

        catLevel.text = currCat.level.ToString() + " 级";

        eatFish.text = currCatLogic.EatFishPerMin.ToString() + " 小鱼/分钟";
        stoneNumber.text = currCat.has_fish.ToString() + " 小鱼";

    }

    //颜色转换
    Color ParseHexColor(string hexColor)
    {
        hexColor = hexColor.TrimStart('#');
        byte r = byte.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, 255); // 如果需要透明度，可以在这里添加alpha值
    }

    public void chooseCat()
    {
        CatController.instance.chooseCat(currCat);

        NewCatController.instance.outCatNumber--;
    }

    //点击了重置按钮
    public void ClickRefreshBtn()
    {
        WatchAddToRefresh();
    }

    //看视频
    void WatchAddToRefresh()
    {
        if (refreshVideoAd != null)
        {
            refreshVideoAd.Show();
            Debug.Log("激励广告展示");
        }

    }

    //关闭广告事件监听
    void RefreshAdClose(WXRewardedVideoAdOnCloseResponse res)
    {
        if ((res != null && res.isEnded) || res == null)
        {
            // 正常播放结束，可以下发游戏奖励
            currFreshTime = 3;

            Debug.Log("测试广告成功");
        }
        else
        {
            // 播放中途退出，不下发游戏奖励
            Debug.Log("广告中途退出");
        }
    }
}
