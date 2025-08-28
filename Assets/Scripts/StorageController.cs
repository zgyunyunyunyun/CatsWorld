using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeChatWASM;

public class StorageController : MonoBehaviour
{
    private float storeTimer = 60.0f;//60s存储1次数据

    private BasicData basicData = new BasicData();//基础数据
    private PropertyData propertData = new PropertyData();//资产数据
    private List<Cat> catsDataList;//小猫的列表数据

    private string basicJson;//基础数据的json
    private string propertyJson;//资产数据的json
    private string catsJson;//小猫列表数据的json

    public DateTime endTime;//上次结束游戏的时间
    public DateTime lastFreeTime;//上次免费寻找小猫的时间

    public static StorageController instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            // 保持这个对象不被销毁
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //初始化微信小游戏sdk
        WX.InitSDK((code) =>
        {
            Debug.Log("打开游戏，从微信存储获得数据");
            GetDataFromWXStorage();

        });

        //展示在前台
        WX.OnShow((res) =>
        {
            Debug.Log("游戏展示到前台，从微信存储获得数据");

            GetDataFromWXStorage();

        });

        //退到后台
        WX.OnHide((res) =>
        {
            Debug.Log("游戏隐藏到后台，将游戏数据存储到微信");

            SetDataToWXStorage();

        });
    }

    // Update is called once per frame
    void Update()
    {
        //每60s存储1次数据
        storeTimer -= Time.deltaTime;
        if (storeTimer < 0)
        {
            Debug.Log("时间隔了60秒，将游戏数据存储到微信");

            SetDataToWXStorage();
            storeTimer = 60.0f;
        }
    }

    //展示前台时，从微信的存储获得数据，并加载到游戏中
    public void GetDataFromWXStorage()
    {
        if (WX.GetStorageInfoSync() != null && WX.GetStorageInfoSync().keys.Length > 0)
        {
            //获得基础数据，并加载到游戏
            string bJson = " ";
            bJson = WX.StorageGetStringSync("basicData", bJson);

            Debug.Log("成功开始读取微信小游戏的数据");

            basicData = JsonUtility.FromJson<BasicData>(bJson);
            MainController.instance.isFirstTimeGaming = basicData.isFirstTimeGaming;
            MainController.instance.bgIsOpened = basicData.bgIsOpened;

            DateTime dateValue;
            string dateString = basicData.endTime;
            DateTime.TryParse(dateString, out dateValue);
            endTime = dateValue;

            DateTime lastDateValue;
            string lastDateString = basicData.lastFreeTime;
            DateTime.TryParse(lastDateString, out lastDateValue);
            lastFreeTime = lastDateValue;

            //获得资产数据，并加载到游戏
            string pJson = " ";
            pJson = WX.StorageGetStringSync("propertyData", pJson);
            propertData = JsonUtility.FromJson<PropertyData>(pJson);

            //更新时间倒计时
            DateTime currentTime = DateTime.Now;
            TimeSpan difference = currentTime - endTime; // 计算时间差
            double secondsDifference = difference.TotalSeconds; // 相差的总秒数   

            if (SceneTransferData.instance != null && SceneTransferData.instance.getLingshiNumber > 0)
            {
                PropertyController.instance.lingshiNumber = SceneTransferData.instance.getLingshiNumber;
            }
            else
            {
                PropertyController.instance.lingshiNumber = propertData.lingshiNumber;
            }


            PropertyController.instance.maxLingdanNumber = propertData.maxLingdanNumber;
            PropertyController.instance.territoryArea = propertData.territoryArea;
            UpRaceControllerNew.instance.raceLevel = propertData.raceLevel;
            NewCatController.instance.lastNewTime = lastFreeTime;
            NewCatController.instance.time = basicData.newCatTime;

            //获得小猫数据，并加载到游戏
            string cJson = " ";

            if (CatController.instance.cats != null && CatController.instance.cats.Count > 0)
            {
                CatController.instance.cats.Clear();
            }
            if (CatController.instance.catLogics != null && CatController.instance.catLogics.Count > 0)
            {
                CatController.instance.catLogics.Clear();
            }

            for (int i = 0; i < propertData.catNumber; i++)
            {
                cJson = WX.StorageGetStringSync("cat" + i.ToString(), cJson);
                Cat catData = JsonUtility.FromJson<Cat>(cJson);

                CatController.instance.cats.Add(catData);
                CatController.instance.catLogics.Add(new CatLogic(catData));
            }

            //如果时间差大于0，说明新增了灵丹
            int newLingdan = 0;
            if (secondsDifference > 0)
            {
                newLingdan = (int)(secondsDifference / PropertyController.instance.timeToLingdan * CatController.instance.cats.Count);

                Debug.Log("离线产生的灵丹数：" + newLingdan);
            }

            newLingdan = propertData.waitToGetLingDan + newLingdan;

            if (newLingdan >= Mathf.Pow(10, CatController.instance.getCatMaxLevel() + 1) * 100)
            {
                PropertyController.instance.waitToGetLingDan = (int)(Mathf.Pow(10, CatController.instance.getCatMaxLevel() + 1) * 100);
            }
            else
            {
                PropertyController.instance.waitToGetLingDan = newLingdan;
            }

            PropertyController.instance.saleLingdanToWaitGet(PropertyController.instance.waitToGetLingDan);
            Debug.Log("离线收益：" + PropertyController.instance.waitToGetLingDan);
        }
        else
        {
            Debug.Log("首次打开微信小游戏，未能读取到数据");
        }

    }

    //退到后台时，从游戏内获得数据，并给微信的存储存入数据
    public void SetDataToWXStorage()
    {
        //获得基础数据并存储
        basicData.endTime = DateTime.Now.ToString();
        basicData.lastFreeTime = NewCatController.instance.lastNewTime.ToString();
        basicData.newCatTime = NewCatController.instance.time;
        basicData.isFirstTimeGaming = MainController.instance.isFirstTimeGaming;
        basicData.bgIsOpened = MainController.instance.bgIsOpened;
        basicJson = JsonUtility.ToJson(basicData);

        WX.StorageSetStringSync("basicData", basicJson);

        //获得资产数据并存储
        propertData.waitToGetLingDan = PropertyController.instance.waitToGetLingDan;
        propertData.lingshiNumber = PropertyController.instance.lingshiNumber;
        propertData.maxLingdanNumber = PropertyController.instance.maxLingdanNumber;
        propertData.territoryArea = PropertyController.instance.territoryArea;
        propertData.catNumber = CatController.instance.cats.Count;
        propertData.raceLevel = UpRaceControllerNew.instance.raceLevel;
        propertyJson = JsonUtility.ToJson(propertData);

        WX.StorageSetStringSync("propertyData", propertyJson);

        //将小猫的数据存储到微信里
        catsDataList = CatController.instance.cats;
        for (int i = 0; i < catsDataList.Count; i++)
        {
            catsJson = JsonUtility.ToJson(catsDataList[i]);

            WX.StorageSetStringSync("cat" + i.ToString(), catsJson);
        }

    }
}

//基础属性类
[System.Serializable]
public class BasicData
{
    public string endTime;//上一次游戏结束时间
    public string lastFreeTime;//上一次免费获得小猫的时间
    public int newCatTime;//当前产生小猫的次数
    public bool isFirstTimeGaming;//是否第一次进入游戏
    public bool bgIsOpened;//首次打开游戏的背景板是否已打开过
}

//资产属性类
/*说明：
 * 1、每次重新启动小游戏时，清空灵丹数量，UI，对象
 * 2、根据时间计算得到的灵石
 * 3、获得小猫详细的数据，可通过小猫的数量从0开始算第一只，通过catx的方式，获得后续的小猫数据
 */
[System.Serializable]
public class PropertyData
{
    public int waitToGetLingDan;//当前拥有的灵丹数量
    public int lingshiNumber;//当前拥有的灵石数量
    public int maxLingdanNumber;//最大的灵丹数量
    public float territoryArea;//当前领土面积
    public int catNumber;//当前小猫的数量
    public int raceLevel;//当前种族等级
}
