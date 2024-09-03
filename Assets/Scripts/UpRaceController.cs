using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpRaceController : MonoBehaviour
{
    public int raceLevel = 0;//��ǰ����ȼ�����0��ʼ����һ��

    public TMP_Text levelMain;//������չʾ����ȼ�
    public TMP_Text level;//չʾ����ȼ�
    public TMP_Text requestLevel;//չʾҪ��ﵽ��Сè�ȼ�
    public TMP_Text requestStone;//չʾҪ��ӵ�е���ʯ����
    public TMP_Text requestArea;//չʾҪ��ӵ�е��������

    public Button upBtn;//������ť
    public TMP_Text upBtnText;//������ť���İ�
    public TMP_Text maxCatNumber;//���Сè����
    public TMP_Text maxLingDanNumber;//����鵤����
    public TMP_Text catNumber;//Сè�������������

    public Image btn1;//��һ�������Ƿ���
    public Image btn2;//�ڶ��������Ƿ���
    public Image btn3;//�����������Ƿ���

    public GameObject upPanel;//�������

    private List<Cat> cats;//����Сè�ĵȼ�

    //��������
    private bool levelCondition = false;
    private bool stoneCondition = false;
    private bool areaCondition = false;

    public TMP_Text nextRankText;//������ǰ�ȼ�
    public TMP_Text newtMaxLDNumberText;//������ǰ�鵤����
    public TMP_Text nextMaxCNumberText;//������ǰСè����

    private float tipsTimer = 1.0f;//�ж����ѵļ�ʱ��

    public Image redPoint;//��������ĺ����ʾ

    public static UpRaceController instance;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //����������չʾ�ˣ��򲻶�ˢ��UI
        if (upPanel.gameObject.activeSelf)
        {
            
            level.text = changeLevel(raceLevel) + "����";
            requestLevel.text = RequestLevel();//Ҫ�����һ�ȼ�
            requestStone.text = RequestStone().ToString() + "��ʯ";
            requestArea.text = RequestTerritoryArea().ToString() + "ƽ����";

            maxCatNumber.text = "Сè���������" + MaxCatNumber(raceLevel).ToString();
            maxLingDanNumber.text = "�鵤���������" + MaxLingDanNumber(raceLevel).ToString();

            if(raceLevel >= 4)
            {
                upBtnText.text = "�Ѵ���ߵȼ�";
                upBtn.interactable = false;
            }
            
        }

        levelMain.text = changeLevel(raceLevel) + "����";
        catNumber.text = CatController.instance.cats.Count + " / " + MaxCatNumber(raceLevel).ToString();

        //���Ϳ�����������Ϣ
        tipsTimer -= Time.deltaTime;
        if (tipsTimer < 0)
        {
            cats = CatController.instance.cats;

            //ˢ�µȼ�
            bool ifLevelCondition = false; 
            //����Сè���Ա�Сè�ȼ��Ƿ��������
            for (int i = 0; i < cats.Count; i++)
            {
                //Сè�ȼ�������һ�ȼ�������
                if (CatController.instance.levelStringToNumber(cats[i].big_level) > raceLevel)
                {                    
                    ifLevelCondition = true;
                    break;

                }
                else
                {
                    ifLevelCondition = false;
                }
            }

            if (ifLevelCondition)
            {
                levelCondition = true;
            }
            else
            {
                levelCondition = false;
            }

            //�Ƿ�������ʯ����
            if (PropertyController.instance.lingshiNumber >= RequestStone())
            {
                //�ı�ɫ��
                stoneCondition = true;
            }
            else
            {
                stoneCondition = false;
            }

            //�Ƿ�������������
            if (PropertyController.instance.territoryArea >= RequestTerritoryArea())
            {
                areaCondition = true;
            }
            else
            {
                areaCondition = false;
            }

            //Сè�����ﵽ���
            if (CatController.instance.cats.Count >= MaxCatNumber(raceLevel))
            {
                Debug.Log("Сè�����ﵽ���");
                //Tips.instance.setTip(8);
            }

            //�ﵽ�����������
            if (areaCondition && levelCondition && stoneCondition)
            {
                Debug.Log("�ﵽ�������������");
                //Tips.instance.setTip(9);

                redPoint.gameObject.SetActive(true);
            }
            else
            {
                redPoint.gameObject.SetActive(false);
            }

            tipsTimer = 1.0f;
        }


        //�Ƿ���������
        if (upPanel.gameObject.activeSelf)
        {

            //Сè�ȼ�������һ�ȼ�������
            if (levelCondition)
            {
                //�ı�ɫ��
                btn1.GetComponent<Image>().color = Color.green;
            }
            else
            {
                btn1.GetComponent<Image>().color = Color.grey;
            }

            //�Ƿ�������ʯ����
            if (stoneCondition)
            {
                //�ı�ɫ��
                btn2.GetComponent<Image>().color = Color.green;
            }
            else
            {
                btn2.GetComponent<Image>().color = Color.grey;
            }

            //�Ƿ�������������
            if (areaCondition)
            {
                //�ı�ɫ��
                btn3.GetComponent<Image>().color = Color.green;
            }
            else
            {
                btn3.GetComponent<Image>().color = Color.grey;
            }

            //3�����������㣬��ť�ı�
            if (stoneCondition && levelCondition && areaCondition)
            {
                upBtn.interactable = true;
            }
            else
            {
                upBtn.interactable = false;
            }
        }


    }



    //����Сè
    public void upLevel()
    {
        //������ʯ
        PropertyController.instance.lingshiNumber -= RequestStone();

        //�����ɹ����ȼ���һ��ͬʱ�ı����Сè����������鵤����
        raceLevel++;
        PropertyController.instance.maxLingdanNumber = MaxLingDanNumber(raceLevel);
        PropertyController.instance.maxLingdanUINumber = MaxLingDanNumber(raceLevel);

        //�ı�ɹ�����UI

        nextRankText.text = "��ǰ�ȼ���" + changeLevel(raceLevel) + "����";
        newtMaxLDNumberText.text = "����鵤������+" + (MaxLingDanNumber(raceLevel) - MaxLingDanNumber(raceLevel-1)).ToString() ;
        nextMaxCNumberText.text = "���Сè����:+" + (MaxCatNumber(raceLevel) - MaxCatNumber(raceLevel - 1)).ToString();

        
    }

    //���ȼ�תΪ����
    string changeLevel(int rLevel)
    {
        if(rLevel == 0)
        {
            return "һ��";
        }else if(rLevel == 1)
        {
            return "����";
        }
        else if (rLevel == 2)
        {
            return "����";
        }
        else if (rLevel == 3)
        {
            return "�Ľ�";
        }
        else if (rLevel == 4)
        {
            return "���";
        }

        return " ";
    }

    //�����һ�ȼ���Ҫ��1
    string RequestLevel()
    {
        if (raceLevel == 0)
        {
            return "������";
        }
        else if (raceLevel == 1)
        {
            return "����";
        }
        else if (raceLevel == 2)
        {
            return "ԪӤ��";
        }
        else if (raceLevel == 3)
        {
            return "������";
        }

        return " ";
    }

    //�����һ�ȼ�����ʯҪ��2
    int RequestStone()
    {
        return 5000 * (int)Mathf.Pow(10, raceLevel);
    }

    //�����һ�ȼ����������Ҫ��
    int RequestTerritoryArea()
    {
        return 10 * (int)Mathf.Pow(5, raceLevel + 1);
    }

    //�����ǰ�ȼ����Сè����
    public int MaxCatNumber(int rLevel)
    {
        return 10 * (rLevel + 1);
    }

    //�����ǰ�ȼ����Lingdan����
    int MaxLingDanNumber(int rLevel)
    {
        return 10 * (rLevel + 2);
    }
}
