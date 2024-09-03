using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpLianDanController : MonoBehaviour
{
    //��Ҫ��ʼ��������
    private int purityPercentage;//��ɵĴ��ȣ�Ӱ���������������Ч��
    private float schedule;//��ɵĽ��ȣ������˵�����Ϸ�Ƿ����������������������Һ�ı���

    private float purityPoint;//��Һ����
    private float impurityPoint;//���ʷ���

    public float point = 1f;//ÿ����Һ�������ʵķ������ݶ�1��

    public int level = 2;//��Ϸ�Ĺؿ�����������Ϸ���Ѷ�
    private int purityNumber;//���ݹؿ�����Ҫ���ɵ�����Һ����
    //private int impurityNumber;//���ݹؿ�����Ҫ���ɵ�����������

    public float upLianDan;//�����ӳ�
    public float duration;//����ʱ��

    //public bool isFinish = false;//��ǰ��Ϸ�Ƿ����


    //��Ҫ��ֵ�Ķ���
    public GameObject purity;//��ʵ��������Һ
    public GameObject impurity;//��ʵ����������

    public GameObject purities;//��Һ������
    public GameObject impurities;//���ʿ�����

    public Slider slider;//��Ϸ������
    public TMP_Text purityPerText;//չʾ��ɴ��ȵİٷֱȣ���100%��ʼ
    public GameObject gameResult;//չʾ��Ϸ��������
    public TMP_Text purityReward;//�����Ϸ�ļ��ٽ���
    public TMP_Text durationTime;//��ɽ������ٵĳ���ʱ��



    public int gameHard = 2;//��Ϸ�Ѷ�



    public static UpLianDanController instance;
    private void Awake()
    {
        instance = this;        
    }

    // Start is called before the first frame update
    void Start()
    {
        startGame();
    }

    // Update is called once per frame
    void Update()
    {
        //ˢ�µ�ǰ��ҩ�Ĵ���
        if (impurityPoint == 0 && purityPoint > 0)
        {
            purityPercentage = 100;
            purityPerText.text = purityPercentage.ToString() + "%";
        }
        else if(purityPoint > 0)
        {
            purityPercentage = ((int)(purityPoint / (purityPoint + impurityPoint)*100));
            purityPerText.text = purityPercentage.ToString()+ "%";
        }

        //�����ɵĽ��ȴﵽ100%������Ϸ����
        if (schedule >= 1)
        {
            //��ͣ��Ϸ��չʾ������
            stopGame();
            gameResult.SetActive(true);


            //���㲢չʾ����
            upLianDan = purityPercentage * (level + 1) * 2;
            duration = (level + 1) * 10;

            purityReward.text = upLianDan.ToString() + "%";
            durationTime.text = duration.ToString() + "����";

            ResultData.instance.upLianDan = upLianDan;
            ResultData.instance.duration = duration;
        }

    }

    public void stopGame()
    {
        Time.timeScale = 0; // ��ͣ��Ϸ
    }

    public void keepGame()
    {
        Time.timeScale = 1; // ������Ϸ
    }

    //��ʼ��Ϸʱ����ʼ�����ݣ�ͬʱ�������½����µĹؿ�
    public void startGame()
    {
        //������ʺ���Һ
        clearThings();

        //��ʼ����������
        purityPoint = 0;
        impurityPoint = 0;
        purityPercentage = 0;
        schedule = 0;
        changeSchedule(schedule);

        upLianDan = 0;
        duration = 0;

        //�������ʺ���Һ
        spawnPurities(level);

        purityNumber = gameHard * (level + 1);

        Time.timeScale = 1; // ������Ϸ(���½�����Ϸʱ��
    }

    //������Ϸ����һ��
    public void nextLevelGame()
    {
        level++;

        startGame();
    }

    public void finishGame()
    {
        ResultData.instance.isFinish = true;

        //TaskManager.instance.ldResult.upSpeed = upLianDan;
        //TaskManager.instance.ldResult.duration =  duration;
        //TaskManager.instance.rewardChanged = true;
        //Debug.Log("������ǰ������Һ��Ϸ������������: " + TaskManager.instance.rewardChanged);
    }

    //��յ�ǰ��Ϸ�����ʺ���Һ
    public void clearThings()
    {
        Debug.Log("��սڵ㣺"+ purity.transform.childCount);
        if (purities.transform.childCount > 0)
        {
            Debug.Log("��սڵ�");
            for (int j = 0; j < purities.transform.childCount; j++)
            {
                Destroy(purities.transform.GetChild(j).gameObject);

                Debug.Log("���ԭ���ڵ���ӽڵ�");
            }
        }

        if (impurities.transform.childCount > 0)
        {
            for (int j = 0; j < impurities.transform.childCount; j++)
            {
                Destroy(impurities.transform.GetChild(j).gameObject);

                Debug.Log("���ԭ���ڵ���ӽڵ�");
            }
        }
    }

    //���ݹؿ�����������Һ�����ʵ�����
    void spawnPurities(int level)
    {
        //һ�������ɵ����� = �ؿ��Ѷ� * �ؿ���
        for(int i=0; i< gameHard*(level+1); i++)
        {
            //���ڸ��ڵ���
            GameObject tempP = Instantiate(purity);
            tempP.transform.SetParent(purities.transform, false);
            tempP.transform.localPosition = new Vector3(Random.Range(-2.8f, 2.8f), Random.Range(-5, 5), 0);

            GameObject tempImP = Instantiate(impurity);
            tempImP.transform.SetParent(impurities.transform, false);
            tempImP.transform.localPosition = new Vector3(Random.Range(-2.8f, 2.8f), Random.Range(-5, 5), 0);
        }
    }

    //��Һ���ӷ���
    public void addPurityPoint(float point)
    {
        purityPoint += point;

        //������ɵĽ���
        schedule = (purityPoint / point) / purityNumber;
        changeSchedule(schedule);
    }

    //���ʼӷ�
    public void addImpurityPoint(float point)
    {
        impurityPoint += point;
    }

    //�ı����
    public void changeSchedule(float schedule)
    {
        slider.value = schedule;
    }



}
