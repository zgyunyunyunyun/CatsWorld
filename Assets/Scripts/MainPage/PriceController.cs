using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PriceController : MonoBehaviour
{
    /// <summary>
    /// 倒计时的计算放在的跨场景的对象里
    /// </summary>

    public GameObject highPriceUI;//展示高价格的UI

    public TMP_Text titlePercentageText;//标题及百分比
    public TMP_Text restTimeText;//剩余倒计时

    public float price = 1;//当前灵丹价格系数

    public float restTimer = 900;//需要进行倒计时的时间
    private float restGap = 900;//剩余时间的倒计时

    private float highPirceTimer = 5.0f;//价格飙升的计时器
    public float priceCheckGap = 5.0f;//价格飙升的gap

    public bool isHighPirce = false;//当时是否高价格？

    public static PriceController instance;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        isHighPirce = false;
    }

    // Update is called once per frame
    void Update()
    {
        //第二次及以后，从其他场景回来以后。如果处于倒计时状态，则刷新数据
        if (SceneTransferData.instance.isHighPirce)
        {
            isHighPirce = SceneTransferData.instance.isHighPirce;
            restTimer = SceneTransferData.instance.priceRestTimer;
        }

        //每个gap时间判断是否价格飙升
        highPirceTimer -= Time.deltaTime;
        //if (highPirceTimer <= 0)
        if (false)
        {
            //如果当前价格不处于高状态，随机判断是否需要增加价格
            if (!isHighPirce)
            {
                int x = Random.Range(0, 100);
                if (x > 90)
                {
                    //价格在20%-80%之间徘徊
                    int p = Random.Range(10, 30);
                    price = (float)(1 + p * 0.1);

                    isHighPirce = true;
                    Debug.Log("灵丹价格上涨，当前价格系数为：" + price);
                }
            }
            highPirceTimer = priceCheckGap;
        }

        //展示UI时，倒计时展示数字
        //if (isHighPirce && highPriceUI != null && restTimer>=0)//遗留：价格存在为0的情况——————————
        if(false)
        {
            

            highPriceUI.gameObject.SetActive(true);
            titlePercentageText.text = "注意,灵丹价格上涨 " + ((int)((price - 1) * 100)).ToString() + "%";

            restTimer -= Time.deltaTime;
            restTimeText.text = ((int)restTimer / 60).ToString() + " : " + ((int)restTimer % 60).ToString();
            if (restTimer <= 0)
            {
                restTimer = restGap;
                isHighPirce = false;
                highPriceUI.gameObject.SetActive(false);
                price = 1;
            }

            //初始化时，把剩余时间传递给场景记录data
            if (SceneTransferData.instance.priceRestTimer < 0 || SceneTransferData.instance.priceRestTimer < restTimer)
            {
                SceneTransferData.instance.priceRestTimer = restTimer;
                SceneTransferData.instance.isHighPirce = isHighPirce;
            }
        }
    }
}
