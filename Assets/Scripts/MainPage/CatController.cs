using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CatController : MonoBehaviour
{
    //控制所有小猫的list和小猫的属性，产生，选择等行为

    public List<Cat> cats = new List<Cat>(); //目前玩家具备的所有小猫
    public List<CatLogic> catLogics = new List<CatLogic>();

    List<string[]> catNameList = new List<string[]>();

    private float timer = 1.0f;//秒计时器


    public GameObject tips;//小猫灵石消耗完的提示
    public int catTypeNumber = 5;//小猫类型数量

    public Image catListRedPoint;//小猫列表的红点

    //小猫列表面板
    public GameObject catUI;//用来生成小猫的prefab
    private List<GameObject> catUIList = new List<GameObject>();//所有产生小猫的UI
    public GameObject content;//挂在内容组件上



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

        //如果小猫数量大于0，则展示小猫列表
        //展示所有小猫并处理对应的位置
        for (int i = 0; i < cats.Count; i++)
        {
            ShowOneCat(cats[i]);
        }
    }

    private void Update()
    {
        //     timer -= Time.deltaTime;
        //     if (timer <= 0)
        //     { 
        //         autoConsumeStone();

        //         bool redPointShowed = false;//红点是否展示了?
        //         //遍历触发提示
        //         for (int i = 0; i < cats.Count; i++)
        //         {
        //             //灵石消耗完了
        //             if (cats[i].had_stone <= 0)
        //             {
        //                 Debug.Log("发送灵石消耗完的消息，展示红点");

        //                 //发送消息
        //                 //Tips.instance.setTip(3, i);

        //                 redPointShowed = true;
        //             }

        //             //小猫可以升级了
        //             bool canUp = cats[i].cultivation + cats[i].lingshi_consume > levelNeedCul(cats[i].small_level, cats[i].big_level);
        //             if (canUp)
        //             {
        //                 Debug.Log("发送可升级的消息，展示红点");

        //                 //发送消息
        //                 //Tips.instance.setTip(7, i);
        //                 cats[i].canUp = true;

        //                 redPointShowed = true;
        //             }
        //             else
        //             {
        //                 cats[i].canUp = false;
        //             }
        //         }

        //         //遍历完了以后，还不展示红点，则隐藏起来
        //         catListRedPoint.gameObject.SetActive(redPointShowed);


        //         timer = 1.0f;
        //     }

        //修改小猫列表面板前
        // timer -= Time.deltaTime;
        // if (timer <= 0)
        // {
        //     autoEatFish();

        //     bool redPointShowed = false;//红点是否展示了?
        //     //遍历触发提示
        //     for (int i = 0; i < cats.Count; i++)
        //     {
        //         //灵石消耗完了
        //         if (cats[i].has_fish <= 0)
        //         {
        //             Debug.Log("发送鱼吃完的消息，展示红点");

        //             //发送消息
        //             //Tips.instance.setTip(3, i);


        //             redPointShowed = true;
        //         }
        //     }

        //     //遍历完了以后，还不展示红点，则隐藏起来
        //     catListRedPoint.gameObject.SetActive(redPointShowed);

        //     timer = 1.0f;
        // }

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            autoEatFish();

            bool redPointShowed = false;//红点是否展示了?
            //遍历触发提示
            for (int i = 0; i < cats.Count; i++)
            {
                GameObject redPoint = catUIList[i].transform.Find("RedPoint").gameObject;
                //灵石消耗完了
                if (cats[i].has_fish <= 0)
                {
                    redPoint.gameObject.SetActive(true);

                    redPointShowed = true;
                }
                else
                {
                    redPoint.gameObject.SetActive(false);
                }
            }

            //遍历完了以后，还不展示红点，则隐藏起来
            catListRedPoint.gameObject.SetActive(redPointShowed);
            timer = 1.0f;
        }

    }


    //新增随机属性的小猫（并未加入小猫的列表中）
    //参数:当前新增小猫的次数（用于判断是否要增加小猫的等级）
    public CatLogic newCat(int newCatTime)
    {
        Cat newCat = new Cat();

        //给新增的小猫随机赋值
        newCat.cat_id = cats.Count;//小猫id从0开始

        newCat.cat_icon = Random.Range(7, 58);//icon即小猫类型
        int rName = Random.Range(1, catNameList[0].Length - 1);//随机的名字类型
        int rrName = Random.Range(0, 7);//icon即小猫类型

        Debug.Log("寻找的小猫类型：" + newCat.cat_icon);
        Debug.Log("寻找的小猫名字序号：" + rName);

        newCat.cat_name = catNameList[rrName][rName];

        string[] intro = { "正经", "懦弱", "调皮", "乖巧", "友善", "高傲", "内向", "冷漠" };

        newCat.introuction = intro[Random.Range(0, intro.Length - 1)];

        //根据新增小猫的次数来提高小猫的等级

        newCat.big_level = "练气期";
        newCat.level = Random.Range(1, 3);


        if ((newCatTime >= 7 && Random.Range(0, 100) >= 70) || (newCatTime >= 12 && Random.Range(0, 100) >= 60) || (newCatTime >= 20 && Random.Range(0, 100) >= 0))
        {
            newCat.big_level = "筑基期";
            Debug.Log("寻找的小猫等级??：" + newCat.big_level);
            newCat.level = Random.Range(3, 8);
        }

        if ((newCatTime >= 12 && Random.Range(0, 100) >= 80) || (newCatTime >= 20 && Random.Range(0, 100) >= 60))
        {
            newCat.big_level = "金丹期";
            newCat.level = Random.Range(8, 16);
        }

        if (newCatTime >= 20 && Random.Range(0, 100) >= 80)
        {
            newCat.big_level = "元婴期";
            newCat.level = Random.Range(16, 27);
        }
        Debug.Log("寻找的小猫次数：" + newCatTime);
        Debug.Log("寻找的小猫等级：" + newCat.big_level);

        newCat.small_level = Random.Range(1, 5);
        newCat.cultivation = 0;
        newCat.canUp = false;
        newCat.lingshi_consume = (int)(newCat.small_level * Mathf.Pow(4, 1 + levelStringToNumber(newCat.big_level)));
        newCat.had_stone = Random.Range(1, 10) * (int)(Mathf.Pow(10, levelStringToNumber(newCat.big_level) + 2));
        newCat.has_fish = Random.Range(5, 10) * (long)Mathf.Pow(5, newCat.level);
        newCat.currentExp = 0;

        CatLogic newCatLogic = new CatLogic(newCat);
        return newCatLogic;
    }

    //将小猫添加入list
    public void chooseCat(Cat cat)
    {
        catLogics.Add(new CatLogic(cat));
        cats.Add(cat);

        ShowOneCat(cat);

        //在面板上新增小猫的UI

        Debug.Log("玩家选择了小猫，小猫id为：" + cat.cat_id.ToString());
    }

    //在小猫列表面板上展示一只小猫
    public void ShowOneCat(Cat cat)
    {
        //当小猫的数量展示范围超过5行时，扩大可展示的content范围
        if (cats.Count <= 8)
        {
            content.GetComponent<GridLayoutGroup>().cellSize = new Vector2(240, 245);
        }
        else
        {
            content.GetComponent<GridLayoutGroup>().cellSize = new Vector2(215, 245);

            RectTransform contentRect = content.GetComponent<RectTransform>();
            if ((cats.Count / 2.0f) == (cats.Count / 2))
            {
                contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1107 + ((cats.Count / 2) - 4) * 270);
                Debug.Log("尝试改变UI1");
            }
            else
            {
                contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1107 + ((cats.Count / 2) - 3) * 270);
                Debug.Log("尝试改变UI2");
            }
        }

        GameObject tempObj = Instantiate(catUI);
        catUIList.Add(tempObj);

        //给小猫添加点击监听事件，用在这里是因为筛选会重新更新UI
        int catID = cat.cat_id;
        tempObj.GetComponent<Button>().onClick.AddListener(() => CatDetailControllerNew.instance.showCatUI(catID));

        //将小猫属性列表挂着父节点上
        tempObj.transform.SetParent(content.transform, false);

        Image catIcon = tempObj.transform.Find("Image").GetComponent<Image>();
        TMP_Text catName = tempObj.transform.Find("Name").GetComponent<TMP_Text>();
        GameObject redPoint = tempObj.transform.Find("RedPoint").gameObject;

        if (cat.canUp)
        {
            redPoint.gameObject.SetActive(true);
        }

        if (cat.has_fish <= 0)
        {
            redPoint.gameObject.SetActive(true);
        }

        //对获得的UI进行赋值
        string path = "Materials/BigCat/cat" + cat.cat_icon.ToString();
        Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
        catIcon.sprite = sprite;

        catName.text = cat.cat_name;
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
        return (int)(small_level * Mathf.Pow(10, levelStringToNumber(big_level) + 3)) * 2;
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
            cats[i].lingshi_consume = (int)(cats[i].small_level * Mathf.Pow(4, 1 + levelStringToNumber(cats[i].big_level)));

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

    // 自动吃鱼
    public void autoEatFish()
    {
        // 计算挂机、离线时小猫吃鱼获得经验
        int minutesDifference = 0; // 相差的总分钟数

        if (!SceneTransferData.instance.isEatFish)
        {
            //更新时间倒计时
            DateTime currentTime = DateTime.Now;
            TimeSpan difference = currentTime - StorageController.instance.endTime; // 计算时间差
            minutesDifference = (int)difference.TotalMinutes; // 相差的总分钟数

            Debug.Log("距离上次打开的相差分钟数为：" + minutesDifference.ToString());

            SceneTransferData.instance.isEatFish = true;
        }

        // 常规吃鱼
        for (int i = 0; i < cats.Count; i++)
        {
            if (minutesDifference > 0)
            {
                catLogics[i].AddExp(catLogics[i].EatFishPerMin * minutesDifference);
            }
            else
            {
                catLogics[i].AddExp();
            }
        }
    }

    //获得小猫当前最高等级
    public int getCatMaxLevel()
    {
        int level = 0;

        for (int i = 0; i < cats.Count; i++)
        {

            if (levelStringToNumber(cats[i].big_level) > level)
            {

                level = levelStringToNumber(cats[i].big_level);
            }
        }

        return level;
    }

}