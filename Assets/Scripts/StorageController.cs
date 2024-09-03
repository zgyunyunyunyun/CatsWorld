using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeChatWASM;

public class StorageController : MonoBehaviour
{
    private float storeTimer = 60.0f;//60s�洢1������

    private BasicData basicData = new BasicData();//��������
    private PropertyData propertData = new PropertyData();//�ʲ�����
    private List<Cat> catsDataList;//Сè���б�����

    private string basicJson;//�������ݵ�json
    private string propertyJson;//�ʲ����ݵ�json
    private string catsJson;//Сè�б����ݵ�json

    public DateTime endTime;//�ϴν�����Ϸ��ʱ��
    public DateTime lastFreeTime;//�ϴ����Ѱ��Сè��ʱ��

    public static StorageController instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            // ����������󲻱�����
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
        //��ʼ��΢��С��Ϸsdk
        WX.InitSDK((code)=>
        {
            Debug.Log("����Ϸ����΢�Ŵ洢�������");
            GetDataFromWXStorage();

        });

        //չʾ��ǰ̨
        WX.OnShow((res) => {
            Debug.Log("��Ϸչʾ��ǰ̨����΢�Ŵ洢�������");

           GetDataFromWXStorage();

        });

        //�˵���̨
        WX.OnHide((res) => {
            Debug.Log("��Ϸ���ص���̨������Ϸ���ݴ洢��΢��");

           SetDataToWXStorage();

        });
    }

    // Update is called once per frame
    void Update()
    {
        //ÿ60s�洢1������
        storeTimer -= Time.deltaTime;
        if (storeTimer < 0)
        {
            Debug.Log("ʱ�����60�룬����Ϸ���ݴ洢��΢��");

            SetDataToWXStorage();
            storeTimer = 60.0f;
        }
    }

    //չʾǰ̨ʱ����΢�ŵĴ洢������ݣ������ص���Ϸ��
    public void GetDataFromWXStorage()
    {
        if(WX.GetStorageInfoSync() != null && WX.GetStorageInfoSync().keys.Length > 0)
        {
            //��û������ݣ������ص���Ϸ
            string bJson = " ";
            bJson = WX.StorageGetStringSync("basicData", bJson);

            Debug.Log("�ɹ���ʼ��ȡ΢��С��Ϸ������");

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

            //����ʲ����ݣ������ص���Ϸ
            string pJson = " ";
            pJson = WX.StorageGetStringSync("propertyData", pJson);
            propertData = JsonUtility.FromJson<PropertyData>(pJson);

            //����ʱ�䵹��ʱ
            DateTime currentTime = DateTime.Now;
            TimeSpan difference = currentTime - endTime; // ����ʱ���
            double secondsDifference = difference.TotalSeconds; // ����������   

            PropertyController.instance.lingshiNumber = propertData.lingshiNumber;
            PropertyController.instance.maxLingdanNumber = propertData.maxLingdanNumber;
            PropertyController.instance.territoryArea = propertData.territoryArea;
            UpRaceController.instance.raceLevel = propertData.raceLevel;
            NewCatController.instance.lastNewTime = lastFreeTime;
            NewCatController.instance.time = basicData.newCatTime;

            //���Сè���ݣ������ص���Ϸ
            string cJson = " ";

            if (CatController.instance.cats != null)
            {
                CatController.instance.cats.Clear();
            }

            for (int i = 0; i < propertData.catNumber; i++)
            {
                cJson = WX.StorageGetStringSync("cat" + i.ToString(), cJson);
                Cat catData = JsonUtility.FromJson<Cat>(cJson);
                
                CatController.instance.cats.Add(catData);
            }

            //���ʱ������0��˵���������鵤
            int newLingdan = 0;
            if (secondsDifference > 0)
            {
                newLingdan = (int)(secondsDifference / PropertyController.instance.timeToLingdan * CatController.instance.cats.Count);

                Debug.Log("���߲������鵤����" + newLingdan);
            }

            newLingdan = propertData.waitToGetLingDan + newLingdan;

            if (newLingdan >= Mathf.Pow(10, CatController.instance.getCatMaxLevel() + 2))
            {
                PropertyController.instance.waitToGetLingDan = (int)Mathf.Pow(10, CatController.instance.getCatMaxLevel() + 2);
            }
            else
            {
                PropertyController.instance.waitToGetLingDan = newLingdan;
            }
            
            PropertyController.instance.saleLingdanToWaitGet(PropertyController.instance.waitToGetLingDan);
            Debug.Log("�������棺" + PropertyController.instance.waitToGetLingDan);
        }
        else
        {
            Debug.Log("�״δ�΢��С��Ϸ��δ�ܶ�ȡ������");
        }
        
    }

    //�˵���̨ʱ������Ϸ�ڻ�����ݣ�����΢�ŵĴ洢��������
    public void SetDataToWXStorage()
    {
        //��û������ݲ��洢
        basicData.endTime = DateTime.Now.ToString();
        basicData.lastFreeTime = NewCatController.instance.lastNewTime.ToString();
        basicData.newCatTime = NewCatController.instance.time;
        basicData.isFirstTimeGaming = MainController.instance.isFirstTimeGaming;
        basicData.bgIsOpened = MainController.instance.bgIsOpened;
        basicJson = JsonUtility.ToJson(basicData);

        WX.StorageSetStringSync("basicData", basicJson);

        //����ʲ����ݲ��洢
        propertData.waitToGetLingDan = PropertyController.instance.waitToGetLingDan;
        propertData.lingshiNumber = PropertyController.instance.lingshiNumber;
        propertData.maxLingdanNumber = PropertyController.instance.maxLingdanNumber;
        propertData.territoryArea = PropertyController.instance.territoryArea;
        propertData.catNumber = CatController.instance.cats.Count;
        propertData.raceLevel = UpRaceController.instance.raceLevel;
        propertyJson = JsonUtility.ToJson(propertData);

        WX.StorageSetStringSync("propertyData", propertyJson);

        //��Сè�����ݴ洢��΢����
        catsDataList = CatController.instance.cats;
        for (int i = 0; i < catsDataList.Count; i++)
        {
            catsJson = JsonUtility.ToJson(catsDataList[i]);

            WX.StorageSetStringSync("cat"+i.ToString(), catsJson);
        }
       
    }
}

//����������
[System.Serializable]
public class BasicData
{
    public string endTime;//��һ����Ϸ����ʱ��
    public string lastFreeTime;//��һ����ѻ��Сè��ʱ��
    public int newCatTime;//��ǰ����Сè�Ĵ���
    public bool isFirstTimeGaming;//�Ƿ��һ�ν�����Ϸ
    public bool bgIsOpened;//�״δ���Ϸ�ı������Ƿ��Ѵ򿪹�
}

//�ʲ�������
/*˵����
 * 1��ÿ����������С��Ϸʱ������鵤������UI������
 * 2������ʱ�����õ�����ʯ
 * 3�����Сè��ϸ�����ݣ���ͨ��Сè��������0��ʼ���һֻ��ͨ��catx�ķ�ʽ����ú�����Сè����
 */
[System.Serializable]
public class PropertyData
{
    public int waitToGetLingDan;//��ǰӵ�е��鵤����
    public float lingshiNumber;//��ǰӵ�е���ʯ����
    public int maxLingdanNumber;//�����鵤����
    public float territoryArea;//��ǰ�������
    public int catNumber;//��ǰСè������
    public int raceLevel;//��ǰ����ȼ�
}
