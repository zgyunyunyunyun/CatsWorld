using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class PropertyController : MonoBehaviour///���������������⣺С��Ϸ�ڵ���ʯ��ô���������л�����ǰ����ô�������ݣ��л�����������ô���������鵤��
{
    public float timeToLingdan = 5;//��Ҫn������1���鵤
    private float timer = 0;//�鵤��ʱ��
    private float lingshiTimer = 0;//��ʯ��ʱ��

    public float lingdanNumber = 0;//�鵤������
    private float lingdanSpeed;//�鵤�������ٶ�

    public float lingshiNumber = 0;//��ʯ������
    private float beforlingshiNumber;//��һ����ʯ����
    private float lingshiSpeed;//��ʯ�������ٶ�

    public TMP_Text lingdanText;//�鵤�������ı�
    public TMP_Text lingdanSpeedText;//�鵤�����ٶȵ��ı�

    public TMP_Text lingshiText;//��ʯ�������ı�
    public TMP_Text lingshiSpeedText;//��ʯ�����ٶȵ��ı�

    public GameObject lingdanToSpawn;//����ʵ�������鵤
    public GameObject lingdanParent;//�û��洢ʵ�����鵤�ĸ��ڵ�
    public GameObject destoryLingdanParent;//����ʱչʾ�ĸ��ڵ㣬��Ϊ��scrollview��������������ų���

    private List<GameObject> lingdanUIList = new List<GameObject>();//���ڴ洢�鵤ʵ����UI���б�
    private List<LingDan> lingdanList = new List<LingDan>();//���ڴ洢�鵤���ݵ��б�
    public int maxLingdanNumber;//һ�������洢���鵤����
    public int maxLingdanUINumber = 20;//һ�������洢���鵤����

    public float territoryArea;//�������
    public TMP_Text areaText;

    private float tipsTimer = 1.0f;//tips��ʱ��

    public AudioClip clickLingdanClip;//����鵤����Ч

    public int waitToGetLingDan = 0;//�ȴ���ȡ���鵤����
    public int waitToGetLingshi = 0;//�ȴ���ȡ���鵤ת��Ϊ��ʯ������
    public GameObject waitToGetObj;//����ȡ��UI����
    public TMP_Text waitToGetLingshiText;//����ȡ��ʯ�������ı�

    public static PropertyController instance;
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

    // Start is called before the first frame update
    void Start()
    {
        lingshiNumber += SceneTransferData.instance.getLingshiNumber;

        //�������
        areaText.text = NumberController.instance.NumberToChinaString((int)territoryArea) + " ƽ����";
    }

    // ÿ֡�ж���Դ��ˢ�£������������ݡ���ʯ���鵤�������ٶȣ�è��Ĳ���������Ĳ�����
    void Update()
    {
        /*�鵤�����߼���
         * 1��ÿֻСèN�����1��
         * 2��ÿ10�����һ�Σ�ÿ�β���N��*Сè
         * 3��Сè�ľ����Ӱ�������ʯ�Ľ�����Ʒ������Ӱ���ٶ�
         * 4��ֻ��ͨ����������������ٶ�
         * 
         * ������10s����һ�ţ�3ֻСè��һ���Ӿ���6*3=18,2Сʱ����18*3600��
         * ͬʱ�����ĵ�ҩ��Ϊ��ʯ����������ջ񣬿���ߵ�ҩ����ʯ�Ķһ�����
         * ��ʯ�����ĳ�ΪСè����Ϊ
         * ������Ҫ����Сè��Ϊ�������������Ҳ��Ҫ��ʯ��ά��Сè����Ϊ
         * 
         * 
         * !!!!�������⣺��������ߵı�����Ϸ���������ǰ�����ߵ�ʱ������ṩ��ʯ���鵤�Ľ��
         */

        //����Lingdan��ʱ��
        timer -= Time.deltaTime;
        if (timer <= 0)
        { 
            //����Сè�����������ÿn��Сè�������鵤����
            List<int> lingdan = CatController.instance.spawnLingdan();
            int temp = 0;
            for(int i = 0; i < lingdan.Count; i++)
            {
                temp += lingdan[i];
            }
            lingdanNumber += temp;
            lingdanSpeed = temp;

            //ˢ���鵤���ı�
            lingdanText.text = NumberController.instance.NumberToChinaString((int)lingdanNumber);
            lingdanSpeedText.text = "+" + NumberController.instance.NumberToChinaString((int)lingdanSpeed); 

            //�ڵײ������鵤ͼ��
            if (lingdan.Count > 0 && lingdanParent != null)
            {
                spawnLingdan(lingdan);
            }
            


            //���¸�ֵ��ʱ��
            timer = timeToLingdan;
        }

        //ÿ������ı�
        lingshiTimer -= Time.deltaTime;
        if (lingshiTimer <= 0)
        {
            //������ʯ�ı�
            lingshiSpeed = lingshiNumber - beforlingshiNumber;
            lingshiText.text = NumberController.instance.NumberToChinaString((int)lingshiNumber);
            if (lingshiSpeed >= 0)
            {
                //����鵤�۸��ڸ�λ�������չʾ��������������չʾ 
                if (lingshiSpeed > 0 && PriceController.instance.isHighPirce)
                {
                    lingshiSpeedText.text = "+" + NumberController.instance.NumberToChinaString((int)lingshiSpeed);
                }
                else
                {
                    lingshiSpeedText.text = "+" + NumberController.instance.NumberToChinaString((int)lingshiSpeed);
                }
            }
            else
            {
                lingshiSpeedText.text = "-" + NumberController.instance.NumberToChinaString((int)lingshiSpeed);
            }
            beforlingshiNumber = lingshiNumber;


            //������������ı�
            areaText.text =NumberController.instance.NumberToChinaString((int)territoryArea) + " ƽ����";

            
            lingshiTimer = 1f;
        }

        //ÿ���ж��Ƿ���Ҫ������
        tipsTimer -= Time.deltaTime;
        if(tipsTimer < 0)
        {
            //�鵤�Ƿ�����
            if (lingdanNumber >= maxLingdanNumber)
            {
                //Tips.instance.setTip(2);
            }

            //��ʯ�Ƿ���ͣ���10��2�η����㣩
            if (lingshiNumber < Mathf.Pow(10, CatController.instance.getCatMaxLevel()+2 ))
            {
                //Tips.instance.setTip(5);
            }

            tipsTimer = 1.0f;
        }

        //����ȴ���ȡ���鵤��������0��˵�������˶�����鵤δ��ȡ
        if(waitToGetLingDan > 0)
        {
            waitToGetObj.SetActive(true);
            waitToGetLingshiText.text = NumberController.instance.NumberToChinaString(waitToGetLingshi);
        }
        else
        {
            waitToGetObj.SetActive(false);
        }

    }


    //�����鵤�����������ۺͲ��֡����������ΪСèһ�����������鵤�������������������ҲΪһ����������������
    void spawnLingdan(List<int> ldList)
    {       
        //�ȱ�����ʯ�б�ÿ�׵Ŀ�λ���ж������������ʯ��Ȼ�����ж�����
        for(int i = 0; i < ldList.Count; i++)
        {
            if (ldList[i] > 0)
            {
                //������������鵤�󣬼��������µ��鵤
                for (int j = 0; j < ldList[i]; j++)
                {

                    //����������鵤���������������������Ҫ��һ��������
                    while (lingdanList.Count >= maxLingdanNumber)
                    {
                        saleLingdan(lingdanList[0], 0 , 0);//�����һ���鵤����λ���ڵ�һ���������Զ�����
                        lingdanList.RemoveAt(0);

                        GameObject t = lingdanUIList[0];
                        lingdanUIList.RemoveAt(0);
                        StartCoroutine(AnimLingdan(t));//Э�����������鵤UI

                        //ˢ���鵤���ı�
                        lingdanNumber--;
                        lingdanText.text = NumberController.instance.NumberToChinaString((int)lingdanNumber);
                        lingdanSpeedText.text = "+" + NumberController.instance.NumberToChinaString((int)lingdanSpeed);
                    }
        

                    //�����鵤������
                    LingDan temp = new LingDan();
                    temp.rank = i;

                    int probability = Random.Range(1, 100);
                    if (probability <= 10)
                    {
                        temp.quality = 2;
                    }
                    else if (10 < probability && probability <= 30)
                    {
                        temp.quality = 1;
                    }
                    else if (probability > 30)
                    {
                        temp.quality = 0;
                    }

                    lingdanList.Add(temp);


                    //�����鵤��UI������һ����������
                    spawnOneLingdanUI(temp.rank);
                }
            }
        }
    }

    //�����鵤UI������Ҫ�����鵤�ĵȼ����չʾ��ͬ��UI��ÿ�β���1����
    void spawnOneLingdanUI(int rank)
    {
        //��Ŀǰ���鵤UI�б���в���
        showLingdan();

        
        if (lingdanUIList.Count < maxLingdanUINumber)
        {
            int needToSpawn;

            //maxLingdanUINumber - lingdanUIList.Count;�����ܹ�����������
            //lingdanList.Count - lingdanUIList.Count;������Ҫ����������
            //��Ҫ��������������ʱ���ٲ�������ܹ��������鵤UI����
            if(lingdanList.Count - lingdanUIList.Count > maxLingdanUINumber - lingdanUIList.Count)
            {
                needToSpawn = maxLingdanUINumber - lingdanUIList.Count;
            }
            //��Ҫ�������������ܲ���������С���������ʱ���������ʵ����Ҫ�ļ���
            else
            {
                needToSpawn = lingdanList.Count - lingdanUIList.Count;
            }
            Debug.Log("ÿ����Ҫ�������鵤����" + needToSpawn);

            //��β����鵤UI
            for(int i = 0; i < needToSpawn; i++)
            {
                //����һ���鵤�ڵײ�
                GameObject tempLingdan = Instantiate(lingdanToSpawn);
                lingdanUIList.Add(tempLingdan);

                //���鵤�б���Ÿ��ڵ��ϣ��Ի�õ�UI���и�ֵ
                tempLingdan.transform.SetParent(lingdanParent.transform, false);
                //tempLingdan.transform.localPosition = new Vector3(0, 0, 0);

                Image catIcon = tempLingdan.GetComponent<Image>();
                string path = "Materials/Logo/" + "�鵤" + rank.ToString();
                Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
                catIcon.sprite = sprite;

                //��Ŀǰ���鵤UI�б���в���
                showLingdan();

            }
        }
        

    }
       
    //�ڵײ����չʾ�鵤logo���ڴ�֮ǰ�ж��鵤�����Ƿ񳬹����ֵ��
    //ͬʱ�������ÿ���鵤logo�ɵ�����¼����а󶨣��ڴ�֮ǰ������ԭ���İ󶨸����
    void showLingdan()
    {
        //���������鵤��������Ų�
        for (int i=0; i < lingdanUIList.Count; i++)
        {
            lingdanUIList[i].transform.localPosition = new Vector3(60 + (i%10) * 110, -(i / 10) * 110 - 70, 0);
        }

        //����鵤UI�ĵ���¼��������°�
        for (int i = 0; i < lingdanUIList.Count; i++)
        {
            int temp = i;
            lingdanUIList[i].GetComponent<Button>().onClick.RemoveAllListeners();
            lingdanUIList[i].GetComponent<Button>().onClick.AddListener(() => clickLingdan(temp));
        }


    }

    //����˵�n���鵤
    public void clickLingdan(int number)
    {
        if(number< lingdanList.Count)
        {

            //�����鵤�����Ч
            AudioManager.instance.PlayAudio(clickLingdanClip);

            Debug.Log("��������е��鵤���Ϊ��" + number);
            Debug.Log("�鵤�б�������" + lingdanList.Count);
            /*
            for(int i = 0; i < lingdanList.Count; i++)
            {
                Debug.Log("�����鵤�����б��ȼ�����" + lingdanList[i].rank);
                Debug.Log("�����鵤�����б���������" + lingdanList[i].quality);
            }
            */

            saleLingdan(lingdanList[number], number, 1);//���۱����е��鵤��λ����number�������ֶ����
            lingdanList.RemoveAt(number);

            GameObject t = lingdanUIList[number];
            lingdanUIList.RemoveAt(number);

            StartCoroutine(AnimLingdan(t));//Э�����������鵤UI


            //ˢ���鵤���ı�
            lingdanNumber--;
            lingdanText.text = NumberController.instance.NumberToChinaString((int)lingdanNumber);
            lingdanSpeedText.text = "+" + NumberController.instance.NumberToChinaString((int)lingdanSpeed);

            //ˢ���鵤��UI
            if (number < lingdanList.Count)
            {
                spawnOneLingdanUI(lingdanList[number].rank);
            }
               
        }

    }

    //Э��ִ�ж���
    IEnumerator AnimLingdan(GameObject lingdan)
    {
        Debug.Log("���Ŷ���������һ����ʯUI");

        lingdan.transform.SetParent(destoryLingdanParent.transform, false);

        //lingdan.transform.localPosition
        lingdan.GetComponent<LingdanFadeAnimation>().isAnimate = true;//���Ŷ���

        

        //�ȴ�2sִ����һ���
        yield return new WaitForSeconds(2.0f);
        Destroy(lingdan);

    }



    //����ĳ���鵤�����ݽ�����Ʒ�����۲�ͬ����ʯ
    /*�鵤����ʯ�Ķһ���ϵ����10���Ĺ�ϵ�㣺
     * һ���鵤��1��ʯ
     * �����鵤��10��ʯ
     * �����鵤��100��ʯ
     * �Ľ��鵤��1000��ʯ
     * ����鵤��10000��ʯ
     * 
     * ��Ʒ��n * 1
     * ��Ʒ��n * 2
     * ��Ʒ��n * 5
     * 
     * type�����۵�����
     * �Զ����ۣ�0
     * �ֶ����ۣ�1
     */
    void saleLingdan(LingDan lingdan, int pos, int type)
    {
        int plus = 0;

        plus = (int)(Mathf.Pow(10, lingdan.rank + 1) * (lingdan.quality + 1) * PriceController.instance.price);

        Transform temp = lingdanUIList[pos].transform.Find("PlusNumber(Clone)");

        if (temp != null)
        {
            if(type == 0)
            {
                plus = (int)(plus / 2);
                temp.GetComponent<TMP_Text>().text = "<color=#F32D2D>��۳���</color>";
            }
            else if(type == 1)
            {
                temp.GetComponent<TMP_Text>().text = "+" + plus.ToString();
            }
            
            
        }
        else
        {
            Debug.Log("��ʯ���۵�����������");
        }

        Debug.Log("�������ۻ����ʯ����" + plus);

        lingshiNumber += plus;

        Debug.Log("������һ���鵤����Ϊ��" + lingdan.rank + "  Ʒ��Ϊ��" + lingdan.quality + "  ����ܹ���ʯΪ��" + lingshiNumber);

    }

    //���ɵȴ���ȡ���鵤����
    public void saleLingdanToWaitGet(int lingdanNumber)
    {
        int lingdanToSale = 0;
        for(int i =0; i<lingdanNumber; i++)
        {
            int probability = Random.Range(1, 100);
            int quality = 0;
            if (probability <= 10)
            {
                quality = 2;
            }
            else if (10 < probability && probability <= 30)
            {
                quality = 1;
            }
            else if (probability > 30)
            {
                quality = 0;
            }

            lingdanToSale += (int)(Mathf.Pow(10, Random.Range(CatController.instance.getCatMaxLevel(), CatController.instance.getCatMaxLevel() + 1)) 
                * (quality + 1) * PriceController.instance.price);

        }

        waitToGetLingshi = lingdanToSale;
    }

    //�������������鵤
    public void getWaitingLingdan()
    {
        lingshiNumber += waitToGetLingshi;
        waitToGetLingshi = 0;
        waitToGetLingDan = 0;
    }

    public void consumeLingShi(float number)
    {
        lingshiNumber -= number;
    }
}

[Serializable]
public class LingDan
{
    public int rank;//����
    public int quality;//Ʒ��
}