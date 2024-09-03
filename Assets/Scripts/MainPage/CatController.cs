using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CatController : MonoBehaviour
{
    //控制所有小猫的list和小猫的属性，产生，选择等行为

    public List<Cat> cats = new List<Cat>(); //目前玩家具备的所有小猫
    List<string[]> catNameList = new List<string[]>();

    private float timer = 1.0f;//秒计时器


    public GameObject tips;//小猫灵石消耗完的提示
    public int catTypeNumber = 5;//小猫类型数量

    public Image catListRedPoint;//小猫列表的红点


    public static CatController instance;
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

    private void Start()
    {

        /*
        Debug.Log("开始读取小猫名字文件");

        string csvUrl = "https://catgame-v0001-4g0nvm54abfb04ae-1258905158.tcloudbaseapp.com/Cat/Cat_0005/Resources/DataFile/catName.csv"; // 替换为你的CSV文件路径
        //string[] lines = File.ReadAllLines(path, Encoding.GetEncoding(936));

        using (UnityWebRequest www = UnityWebRequest.Get(csvUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error downloading CSV: " + www.error);
            }
            else
            {
                string[] lines = www.downloadHandler.text.Split('\n');
                foreach (string line in lines)
                {
                    string[] columns = line.Split(',');

                    catNameList.Add(columns);
                    foreach (string column in columns)
                    {
                        Debug.Log(column);
                    }
                }
            }
        }

        foreach (string line in lines)
        {
            

            string[] values = line.Split(',');
            catNameList.Add(values);

            //Debug.Log("输出读取文件每行的结果：" + values);
        }

        // 输出读取的数据
        for (int i = 0; i < catNameList.Count; i++)
        {
            for (int j = 0; j < catNameList[i].Length; j++)
            {
                //Debug.Log("输出读取小猫名称文件第" + i + "行的结果为：" + catNameList[i][j]);
            }
        }
        */

        string[] lines0 = { "白猫", "蛋蛋", "布丁", "小白", "白云", "奶油", "肉肉", "喵喵", "甜甜", "啵啵", "毛毛", "招财", "钱多多", "老板", "富贵", "铁蛋", "二丫", "狗蛋", "翠花", "大牛", "二爷", "果冻", "大壮", "嘟嘟", "小丸子", "叮当", "小球", "旺财", "皇上", "大宝", "二宝", "汤圆", "陛下", "咪咪" };
        catNameList.Add(lines0);
        string[] lines1 = { "黑猫", "小黑", "黑土", "警长", "肉肉", "喵喵", "甜甜", "啵啵", "毛毛", "招财", "钱多多", "老板", "富贵", "铁蛋", "二丫", "狗蛋", "翠花", "大牛", "二爷", "果冻", "大壮", "嘟嘟", "小丸子", "叮当", "小球", "旺财", "皇上", "大宝", "二宝", "汤圆", "陛下", "咪咪", "妹妹", "可乐" };
        catNameList.Add(lines1);
        string[] lines2 = { "橘猫", "布丁", "小黄", "胖虎", "小橘", "芒果", "饼饼", "小太阳", "大黄", "大橘", "肉肉", "喵喵", "甜甜", "啵啵", "毛毛", "招财", "钱多多", "老板", "富贵", "铁蛋", "二丫", "狗蛋", "翠花", "大牛", "二爷", "果冻", "大壮", "嘟嘟", "小丸子", "叮当", "小球", "旺财", "皇上", "大宝" };
        catNameList.Add(lines2);
        string[] lines3 = { "花猫", "花花", "小灰", "小花", "肉肉", "喵喵", "甜甜", "啵啵", "毛毛", "招财", "钱多多", "老板", "富贵", "铁蛋", "二丫", "狗蛋", "翠花", "大牛", "二爷", "果冻", "大壮", "嘟嘟", "小丸子", "叮当", "小球", "旺财", "皇上", "大宝", "二宝", "汤圆", "陛下", "咪咪", "妹妹", "可乐" };
        catNameList.Add(lines3);
        string[] lines4 = { "三花猫", "花花", "小花", "桃子", "奶油", "肉肉", "喵喵", "甜甜", "啵啵", "毛毛", "招财", "钱多多", "老板", "富贵", "铁蛋", "二丫", "狗蛋", "翠花", "大牛", "二爷", "果冻", "大壮", "嘟嘟", "小丸子", "叮当", "小球", "旺财", "皇上", "大宝", "二宝", "汤圆", "陛下", "咪咪", "妹妹" };
        catNameList.Add(lines4);
        string[] lines5 = { "黑猫", "小黑", "黑土", "警长", "肉肉", "喵喵", "甜甜", "啵啵", "毛毛", "招财", "钱多多", "老板", "富贵", "铁蛋", "二丫", "狗蛋", "翠花", "大牛", "二爷", "果冻", "大壮", "嘟嘟", "小丸子", "叮当", "小球", "旺财", "皇上", "大宝", "二宝", "汤圆", "陛下", "咪咪", "妹妹", "可乐" };
        catNameList.Add(lines5);
        string[] lines6 = { "橘黑猫", "花花", "小花", "警长", "肉肉", "喵喵", "甜甜", "啵啵", "毛毛", "招财", "钱多多", "老板", "富贵", "铁蛋", "二丫", "狗蛋", "翠花", "大牛", "二爷", "果冻", "大壮", "嘟嘟", "小丸子", "叮当", "小球", "旺财", "皇上", "大宝", "二宝", "汤圆", "陛下", "咪咪", "妹妹", "可乐" };
        catNameList.Add(lines6);

        catTypeNumber = catNameList.Count;

    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            autoConsumeStone();

            bool redPointShowed = false;//红点是否展示了?
            //遍历触发提示
            for (int i = 0; i < cats.Count; i++)
            {
                //灵石消耗完了
                if (cats[i].had_stone <= 0)
                {
                    Debug.Log("发送灵石消耗完的消息，展示红点");

                    //发送消息
                    //Tips.instance.setTip(3, i);

                    redPointShowed = true;
                }

                //小猫可以升级了
                bool canUp = cats[i].cultivation + cats[i].lingshi_consume > levelNeedCul(cats[i].small_level, cats[i].big_level);
                if (canUp)
                {
                    Debug.Log("发送可升级的消息，展示红点");

                    //发送消息
                    Tips.instance.setTip(7, i);
                    cats[i].canUp = true;

                    redPointShowed = true;
                }
                else
                {
                    cats[i].canUp = false;
                }
            }

            //遍历完了以后，还不展示红点，则隐藏起来
            catListRedPoint.gameObject.SetActive(redPointShowed);
            

            timer = 1.0f;
        }

    }


    //新增随机属性的小猫（并未加入小猫的列表中）
    //参数:当前新增小猫的次数（用于判断是否要增加小猫的等级）
    public Cat newCat(int newCatTime)
    {
        Cat newCat = new Cat();

        //给新增的小猫随机赋值
        newCat.cat_id = cats.Count;//小猫id从0开始
                                  
        newCat.cat_icon = Random.Range(0, catTypeNumber - 1);//icon即小猫类型
        int rName = Random.Range(1, catNameList[0].Length - 1);//随机的名字类型

        Debug.Log("寻找的小猫类型：" + newCat.cat_icon);
        Debug.Log("寻找的小猫名字序号：" + rName);

        newCat.cat_name = catNameList[newCat.cat_icon][rName];

        string[] intro = { "正经", "懦弱", "调皮", "乖巧", "友善", "高傲", "内向", "冷漠" };

        newCat.introuction = intro[Random.Range(0, intro.Length - 1)];

        //根据新增小猫的次数来提高小猫的等级
        newCat.big_level = "练气期";

        if(newCatTime < 8 && newCatTime >= 5 && Random.Range(0, 100) >= 80)
        {
            newCat.big_level = "筑基期";
            Debug.Log("寻找的小猫等级??：" + newCat.big_level);
        } 

        if (newCatTime < 12 && newCatTime >= 8 && Random.Range(0, 100) >= 80)
        {
            newCat.big_level = "金丹期";
        }

        if (newCatTime >= 12 && Random.Range(0, 100) >= 80)
        {
            newCat.big_level = "元婴期";
        }
        Debug.Log("寻找的小猫次数：" + newCatTime);
        Debug.Log("寻找的小猫等级：" + newCat.big_level);

        newCat.small_level = Random.Range(1, 5);
        newCat.cultivation = 0;
        newCat.canUp = false;
        newCat.lingshi_consume = (int)(newCat.small_level * Mathf.Pow(10, levelStringToNumber(newCat.big_level) + 1));
        newCat.had_stone = Random.Range(1,10)* (int)(Mathf.Pow(10, levelStringToNumber(newCat.big_level) + 2));

        return newCat;
    }

    //将小猫添加入list
    public void chooseCat(Cat cat)
    {
        
        cats.Add(cat);

        Debug.Log("玩家选择了小猫，小猫id为：" + cat.cat_id.ToString());
    }

    //根据当前所有小猫，每隔一段时间产生灵石
    public List<int> spawnLingdan()
    {
        //各阶灵石的数量
        List<int> lingdan = new List<int>() { 0, 0, 0, 0, 0 };
        
        for (int i = 0; i < cats.Count; i++)
        {
            if (cats[i].big_level == "练气期")
            {
                lingdan[0] += 1;
            }
            else if (cats[i].big_level == "筑基期")
            {
                lingdan[1] += 1;
            }
            else if (cats[i].big_level == "金丹期")
            {
                lingdan[2] += 1;
            }
            else if (cats[i].big_level == "元婴期")
            {
                lingdan[3] += 1;
            }
            else if (cats[i].big_level == "化神期")
            {
                lingdan[4] += 1;
            }
        }

        return lingdan;
    }
    
    //将等级字符串自动转化为数字
    public int levelStringToNumber(string level)
    {
        if (level == "练气期")
        {
            return 0;
        }
        else if (level == "筑基期")
        {
            return 1;
        }
        else if (level == "金丹期")
        {
            return 2;
        }
        else if (level == "元婴期")
        {
            return 3;
        }
        else if (level == "化神期")
        {
            return 4;
        }

        return 0;
    }

    public string numberToCatLevelString(int level)
    {
        if (level == 0)
        {
            return "练气期";
        }
        else if (level == 1)
        {
            return "筑基期";
        }
        else if (level == 2)
        {
            return "金丹期";
        }
        else if (level == 3)
        {
            return "元婴期";
        }
        else if (level == 4)
        {
            return "化神期";
        }

        return "";
    }

    //将等级转化为修为的上限
    public int levelNeedCul(int small_level, string big_level)
    {
        return (int)(small_level * Mathf.Pow(10, levelStringToNumber(big_level) + 3));
    }


    //自动消耗灵石，并控制小猫修为达到上限
    public void autoConsumeStone()
    {           
        int secondsDifference = 1; // 相差的总秒数

        if (!SceneTransferData.instance.isConsumeStone)
        {
            //更新时间倒计时
            DateTime currentTime = DateTime.Now;
            TimeSpan difference = currentTime - StorageController.instance.endTime; // 计算时间差
            secondsDifference = (int)difference.TotalSeconds; // 相差的总秒数

            Debug.Log("距离上次打开的相差秒数为：" + secondsDifference.ToString());

            SceneTransferData.instance.isConsumeStone = true;
        }

        //常规消耗灵石
        for (int i = 0; i < cats.Count; i++)
        {
            cats[i].lingshi_consume = (int)(cats[i].small_level * Mathf.Pow(10, levelStringToNumber(cats[i].big_level)));

            //灵石是否足够消耗
            bool enoughStone = cats[i].had_stone >= cats[i].lingshi_consume * secondsDifference;
            
            //判断小猫当前灵石数量是否足够消耗，如果不足，则仅消耗目前有的
            if (enoughStone)
            {
                //消耗的灵石是否超过小猫的修为上限
                bool canUp = cats[i].cultivation + cats[i].lingshi_consume * secondsDifference > levelNeedCul(cats[i].small_level, cats[i].big_level);
                //消耗灵石后超过修为上限，且该小猫符合晋级条件
                if (canUp)
                {
                    cats[i].had_stone -= (int)(levelNeedCul(cats[i].small_level, cats[i].big_level) - cats[i].cultivation);
                    cats[i].cultivation = levelNeedCul(cats[i].small_level, cats[i].big_level);

                    Debug.Log("小猫可晋级，序号为：" + i);
                }
                else
                {
                    
                    cats[i].had_stone -= cats[i].lingshi_consume * secondsDifference;
                    cats[i].cultivation += cats[i].lingshi_consume * secondsDifference;

                }
                
            }
            else
            {
                //消耗的灵石是否超过小猫的修为上限
                bool canUp = cats[i].cultivation + cats[i].had_stone > levelNeedCul(cats[i].small_level, cats[i].big_level);
                //消耗灵石后超过修为上限，且该小猫符合晋级条件
                if (canUp)
                {
                    cats[i].had_stone -= (int)(levelNeedCul(cats[i].small_level, cats[i].big_level) - cats[i].cultivation);
                    cats[i].cultivation = levelNeedCul(cats[i].small_level, cats[i].big_level);

                    Debug.Log("小猫可晋级，序号为：" + i);
                }
                else
                {
                    cats[i].cultivation += cats[i].had_stone;
                    cats[i].had_stone = 0;
                    
                }
                
            }

        }
    }

    //获得小猫当前最高等级
    public int getCatMaxLevel()
    {
        int level = 0;

        for(int i = 0; i < cats.Count; i++)
        {

            if (levelStringToNumber(cats[i].big_level) > level)
            {

                level = levelStringToNumber(cats[i].big_level);
            }
        }

        return level;
    }

}

[Serializable]
public class Cat
{
    public int cat_id;//小猫的id，从0开始

    public int cat_icon;//小猫头像

    public string cat_name;//小猫名称

    public string introuction;//小猫简介

    //public string work;//工作状态：炼丹中、探索中、空闲中

    public string big_level;//大境界
    public int small_level;//小境界

    public float cultivation;//修为

    public bool canUp;//小猫是否能升级

    public int lingshi_consume;//灵石消耗速度x/s

    public int had_stone;//拥有的灵石数量

    /*
    public static implicit operator GameObject(Cat v)
    {
        throw new NotImplementedException();
    }
    */
}