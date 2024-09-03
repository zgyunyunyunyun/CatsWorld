using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PriceController : MonoBehaviour
{
    /// <summary>
    /// ����ʱ�ļ�����ڵĿ糡���Ķ�����
    /// </summary>

    public GameObject highPriceUI;//չʾ�߼۸��UI

    public TMP_Text titlePercentageText;//���⼰�ٷֱ�
    public TMP_Text restTimeText;//ʣ�൹��ʱ

    public float price = 1;//��ǰ�鵤�۸�ϵ��

    public float restTimer = 900;//��Ҫ���е���ʱ��ʱ��
    private float restGap = 900;//ʣ��ʱ��ĵ���ʱ

    private float highPirceTimer = 5.0f;//�۸�����ļ�ʱ��
    public float priceCheckGap = 5.0f;//�۸������gap

    public bool isHighPirce = false;//��ʱ�Ƿ�߼۸�

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
        //�ڶ��μ��Ժ󣬴��������������Ժ�������ڵ���ʱ״̬����ˢ������
        if (SceneTransferData.instance.isHighPirce)
        {
            isHighPirce = SceneTransferData.instance.isHighPirce;
            restTimer = SceneTransferData.instance.priceRestTimer;
        }

        //ÿ��gapʱ���ж��Ƿ�۸����
        highPirceTimer -= Time.deltaTime;
        //if (highPirceTimer <= 0)
        if (false)
        {
            //�����ǰ�۸񲻴��ڸ�״̬������ж��Ƿ���Ҫ���Ӽ۸�
            if (!isHighPirce)
            {
                int x = Random.Range(0, 100);
                if (x > 90)
                {
                    //�۸���20%-80%֮���ǻ�
                    int p = Random.Range(10, 30);
                    price = (float)(1 + p * 0.1);

                    isHighPirce = true;
                    Debug.Log("�鵤�۸����ǣ���ǰ�۸�ϵ��Ϊ��" + price);
                }
            }
            highPirceTimer = priceCheckGap;
        }

        //չʾUIʱ������ʱչʾ����
        //if (isHighPirce && highPriceUI != null && restTimer>=0)//�������۸����Ϊ0�������������������������
        if(false)
        {
            

            highPriceUI.gameObject.SetActive(true);
            titlePercentageText.text = "ע��,�鵤�۸����� " + ((int)((price - 1) * 100)).ToString() + "%";

            restTimer -= Time.deltaTime;
            restTimeText.text = ((int)restTimer / 60).ToString() + " : " + ((int)restTimer % 60).ToString();
            if (restTimer <= 0)
            {
                restTimer = restGap;
                isHighPirce = false;
                highPriceUI.gameObject.SetActive(false);
                price = 1;
            }

            //��ʼ��ʱ����ʣ��ʱ�䴫�ݸ�������¼data
            if (SceneTransferData.instance.priceRestTimer < 0 || SceneTransferData.instance.priceRestTimer < restTimer)
            {
                SceneTransferData.instance.priceRestTimer = restTimer;
                SceneTransferData.instance.isHighPirce = isHighPirce;
            }
        }
    }
}
