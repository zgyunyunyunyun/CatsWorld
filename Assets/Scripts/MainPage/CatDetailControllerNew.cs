using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CatDetailControllerNew : MonoBehaviour
{
    public Image catPicture;
    public TMP_Text catName;
    public TMP_Text state;
    public TMP_Text rank;
    public TMP_Text exp;
    public TMP_Text eatFish;
    public TMP_Text catchFish;
    public TMP_Text hasFish;

    public Image rankPicture;//

    public Image fishRedPoint;//鱼数量不足的红点

    public GameObject toast;

    private CatLogic catLogic;
    private Cat cat;


    public static CatDetailControllerNew instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {


    }

    private void Update()
    {
        //更新小猫拥有的灵石数量
        if (cat != null)
        {
            hasFish.text = cat.has_fish.ToString();
            exp.text = $"{catLogic.CurrentExp.Value}/{catLogic.MaxExp}";
            eatFish.text = $"{catLogic.EatFishPerMin}/分";
        }
    }

    //在小猫详情页里展示小猫的样式
    public void showCatUI(int catID)
    {
        catLogic = CatController.instance.catLogics.Find(x => x.CatData.cat_id == catID);
        if (catLogic == null)
        {
            Debug.LogError("未找到小猫逻辑");
            return;
        }

        cat = catLogic.ToData();

        //对获得的UI进行赋值
        string path = "Materials/BigCat/cat" + cat.cat_icon.ToString();
        Debug.Log("成功诞生小猫，小猫头像 path:" + path);
        Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
        catPicture.sprite = sprite;

        catName.text = cat.cat_name;
        //state.text = cat.work;

        rank.text = $"{catLogic.Level.Value}级";
        hasFish.text = $"{cat.has_fish}";
        exp.text = $"{catLogic.CurrentExp.Value}/{catLogic.MaxExp}";
        eatFish.text = $"{catLogic.EatFishPerMin}/分";
        catchFish.text = $"{catLogic.CatchFishPerSec}/秒";


        //判断是否展示红点
        if (cat.has_fish <= 0)
        {
            fishRedPoint.gameObject.SetActive(true);
        }
        else
        {
            fishRedPoint.gameObject.SetActive(false);
        }
    }

    //给予鱼函数
    public void giveFish(int fishNumber)
    {
        //判断打开了详情页的小猫非空
        if (cat != null)
        {
            //判断是否提供全部鱼：fishNumber==-1
            if (fishNumber < 0)
            {
                cat.has_fish += PropertyController.instance.fishNumber;
                hasFish.text = $"{cat.has_fish}";

                PropertyController.instance.fishNumber = 0;
            }
            else
            {
                //判断是否有足够鱼分配
                if (fishNumber > PropertyController.instance.fishNumber)
                {
                    //弹出鱼分配失败
                    toast.SetActive(true);
                    toast.GetComponent<Toast>().setText("鱼儿不够了，快去抓鱼吧~");
                    Debug.Log("鱼不足，分配失败");
                }
                else//最后成功正常分配
                {
                    cat.has_fish += fishNumber;
                    hasFish.text = $"{cat.has_fish}";

                    PropertyController.instance.fishNumber -= fishNumber;
                }

            }

        }
        else
        {
            Debug.Log("给予鱼儿的小猫为空");
        }

        //分配以后，判断鱼足不足
        if (cat.has_fish <= 0)
        {
            fishRedPoint.gameObject.SetActive(true);
        }
        else
        {
            fishRedPoint.gameObject.SetActive(false);

        }
    }


}
