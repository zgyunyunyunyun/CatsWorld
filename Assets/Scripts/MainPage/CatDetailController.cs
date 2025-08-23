using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WeChatWASM;

public class CatDetailController : MonoBehaviour
{
    public Image catPicture;
    public TMP_Text catName;
    public TMP_Text state;
    public TMP_Text rank;
    public TMP_Text introduciton;
    public TMP_Text capacity;
    public TMP_Text cultivation;
    public TMP_Text consumeStone;
    public TMP_Text hadStone;

    public Image rankPicture;//

    public Button upRankBtn;//晋级按钮

    public Image upRedPoint;//升级的红点
    public Image stoneRedPoint;//灵石不足的红点

    public GameObject toast;

    private Cat cat;


    public static CatDetailController instance;
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
        if(cat != null)
        {
            hadStone.text = "灵石：" + cat.had_stone;
            cultivation.text = "修为：" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();

            //判断小猫是否可展示晋级红点和按钮
            if (cat.cultivation >= CatController.instance.levelNeedCul(cat.small_level, cat.big_level))
            {
                upRankBtn.gameObject.SetActive(true);
                upRedPoint.gameObject.SetActive(true);
            }
            else
            {
                upRedPoint.gameObject.SetActive(false);
            }
        }


    }

    //在小猫详情页里展示小猫的样式
    public void showCatUI(int catID)
    {
        for(int i=0;i< CatController.instance.cats.Count; i++)
        {
            if(catID == CatController.instance.cats[i].cat_id)
            {
                cat = CatController.instance.cats[i];
            }
        }
        

        //对获得的UI进行赋值
        string path = "Materials/BigCat/cat" + cat.cat_icon.ToString();
        Debug.Log("成功诞生小猫，小猫头像 path:" + path);
        Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
        catPicture.sprite = sprite;

        catName.text = cat.cat_name;
        //state.text = cat.work;
        
        hadStone.text = "灵石：" + cat.had_stone;

        introduciton.text = "性格：" + cat.introuction;

        //判断是否展示红点
        if (cat.had_stone <= 0)
        {
            stoneRedPoint.gameObject.SetActive(true);
        }
        else
        {
            stoneRedPoint.gameObject.SetActive(false);
        }

        if (cat.big_level == "练气期")
        {
            capacity.text = "能力：炼制一阶丹药";
            cultivation.text = "修为：" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
            consumeStone.text = "消耗：" + cat.lingshi_consume.ToString() +"灵石/秒";
            rank.text = "<color=#000000>" + cat.big_level + "</color> " + cat.small_level + "层";

            Color color = ParseHexColor("#000000");
            rankPicture.color = color;
        }
        else if(cat.big_level == "筑基期")
        {
            capacity.text = "能力：炼制二阶丹药";
            cultivation.text = "修为：" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
            consumeStone.text = "消耗：" + cat.lingshi_consume.ToString() + "灵石/秒";
            rank.text = "<color=#19932D>" + cat.big_level + "</color> " + cat.small_level + "层";

            Color color = ParseHexColor("#19932D");
            rankPicture.color = color;
        }
        else if (cat.big_level == "金丹期")
        {
            capacity.text = "能力：炼制三阶丹药";
            cultivation.text = "修为：" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
            consumeStone.text = "消耗：" + cat.lingshi_consume.ToString() + "灵石/秒";
            rank.text = "<color=#C3A010>" + cat.big_level + "</color> " + cat.small_level + "层";

            Color color = ParseHexColor("#C3A010");
            rankPicture.color = color;
        }
        else if (cat.big_level == "元婴期")
        {
            capacity.text = "能力：炼制四阶丹药";
            cultivation.text = "修为：" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
            consumeStone.text = "消耗：" + cat.lingshi_consume.ToString() + "灵石/秒";
            rank.text = "<color=#A72EB0>" + cat.big_level + "</color> " + cat.small_level + "层";

            Color color = ParseHexColor("#A72EB0");
            rankPicture.color = color;
        }
        else if (cat.big_level == "化神期")
        {
            capacity.text = "能力：炼制五阶丹药";
            cultivation.text = "修为：" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
            consumeStone.text = "消耗：" + cat.lingshi_consume.ToString() + "灵石/秒";
            rank.text = "<color=#FF1010>" + cat.big_level + "</color> " + cat.small_level + "层";

            Color color = ParseHexColor("#FF1010");
            rankPicture.color = color;
        }

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

    //升级小猫等级函数
    /*小猫境界：
     * 大境界：练气到化神
     * 小境界：10个
     * 
     */
    public void upCatRank()
    {
        if (cat != null && cat.cultivation >= CatController.instance.levelNeedCul(cat.small_level, cat.big_level))
        {
            //如果小境界不足10，则晋级小境界，否则晋级大境界
            if (cat.small_level < 10)
            {
                cat.small_level++;
            }
            else
            {
                cat.big_level = CatController.instance.numberToCatLevelString(CatController.instance.levelStringToNumber(cat.big_level) + 1);
                cat.small_level = 1;
            }

            /*晋级后刷新属性
             * 
             */

            //改变灵石消耗所需
            cat.lingshi_consume = (int)(cat.small_level * Mathf.Pow(4, 1 + CatController.instance.levelStringToNumber(cat.big_level)));
            consumeStone.text = "消耗：" + cat.lingshi_consume.ToString() + "灵石/秒";


            //修为清空，并改变晋级的修为和所需修为文案
            cat.cultivation = 0;
            if (cat.big_level == "练气期")
            {
                capacity.text = "能力：炼制一阶丹药";
                cultivation.text = "修为：" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
                consumeStone.text = "消耗：" + cat.lingshi_consume.ToString() + "灵石/秒";
                rank.text = "<color=#000000>" + cat.big_level + "</color> " + cat.small_level + "层";

                Color color = ParseHexColor("#000000");
                rankPicture.color = color;
            }
            else if (cat.big_level == "筑基期")
            {
                capacity.text = "能力：炼制二阶丹药";
                cultivation.text = "修为：" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
                consumeStone.text = "消耗：" + cat.lingshi_consume.ToString() + "灵石/秒";
                rank.text = "<color=#19932D>" + cat.big_level + "</color> " + cat.small_level + "层";

                Color color = ParseHexColor("#19932D");
                rankPicture.color = color;
            }
            else if (cat.big_level == "金丹期")
            {
                capacity.text = "能力：炼制三阶丹药";
                cultivation.text = "修为：" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
                consumeStone.text = "消耗：" + cat.lingshi_consume.ToString() + "灵石/秒";
                rank.text = "<color=#C3A010>" + cat.big_level + "</color> " + cat.small_level + "层";

                Color color = ParseHexColor("#C3A010");
                rankPicture.color = color;
            }
            else if (cat.big_level == "元婴期")
            {
                capacity.text = "能力：炼制四阶丹药";
                cultivation.text = "修为：" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
                consumeStone.text = "消耗：" + cat.lingshi_consume.ToString() + "灵石/秒";
                rank.text = "<color=#A72EB0>" + cat.big_level + "</color> " + cat.small_level + "层";

                Color color = ParseHexColor("#A72EB0");
                rankPicture.color = color;
            }
            else if (cat.big_level == "化神期")
            {
                capacity.text = "能力：炼制五阶丹药";
                cultivation.text = "修为：" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
                consumeStone.text = "消耗：" + cat.lingshi_consume.ToString() + "灵石/秒";
                rank.text = "<color=#FF1010>" + cat.big_level + "</color> " + cat.small_level + "层";

                Color color = ParseHexColor("#FF1010");
                rankPicture.color = color;
            }

            //隐藏红点
            upRedPoint.gameObject.SetActive(false);


            Debug.Log("小猫完成晋升");
        }
        else
        {
            //弹出灵石分配失败
            toast.SetActive(true);
            toast.GetComponent<Toast>().setText("修为不足，无法晋级");
            Debug.Log("灵石不足，分配失败");
        }
    }

    //给予灵石函数
    public void giveStone(int stoneNumber)
    {
        //判断打开了详情页的小猫非空
        if(cat != null)
        {
            //判断是否提供全部灵石：stoneNUmber==-1
            if (stoneNumber < 0)
            {
                cat.had_stone += PropertyController.instance.lingshiNumber;
                hadStone.text = "灵石：" + cat.had_stone;

                PropertyController.instance.lingshiNumber = 0;
            }
            else
            {   
                //判断是否有足够灵石分配
                if(stoneNumber > PropertyController.instance.lingshiNumber)
                {
                    //弹出灵石分配失败
                    toast.SetActive(true);
                    toast.GetComponent<Toast>().setText("灵石不足");
                    Debug.Log("灵石不足，分配失败");
                }
                else//最后成功正常分配
                {
                    cat.had_stone += stoneNumber;
                    hadStone.text = "灵石：" + cat.had_stone;

                    PropertyController.instance.lingshiNumber -= stoneNumber;
                }

            }

        }
        else
        {
            Debug.Log("给予灵石的小猫为空");
        }

        //分配以后，判断灵石足不足
        if (cat.had_stone <= 0)
        {
            stoneRedPoint.gameObject.SetActive(true);
        }
        else
        {
            stoneRedPoint.gameObject.SetActive(false);

        }
    }


}
