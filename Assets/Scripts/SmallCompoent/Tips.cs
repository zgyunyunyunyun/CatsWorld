using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.IO;
using System.Text;

public class Tips : MonoBehaviour
{
    public GameObject tipsUIObj;//tipsUI�Ķ���
    public TMP_Text text;//�ı�
    public Image catIcon;//Сèͷ��

    public GameObject catDetailPanel;//Сè����ҳ

    public float detectTimeGap = 1.0f;//��ns���һ��tips��״̬��������Ϣ�ļ����
    private float detectTimer = 1.0f;//��ns���һ��tips��״̬

    public float clearTimeGap = 30.0f;//��30s����һ�α������������
    private float clearTimer = 30.0f;//��30s����һ�α������������

    private List<TipsData> tipsList = new List<TipsData>();

    private bool isShowed = false;//�Ƿ�չʾ��һ�ε��������

    public static Tips instance;
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
        if(MainController.instance.isFirstTimeGaming && !isShowed)
        {
            /*ÿ�ο��֣��Զ���ʾ������Ϣ��
            * 0��Сè�����鵤˵������
            * 1��Сè�����鵤����
            * 4��ʣ������Сè����
            */

            setTip(0);
            setTip(1);
            //setTip(4);

            isShowed = true;
        }

        //����tips�б�չʾ���ŵ�����
        detectTimer -= Time.deltaTime;
        if (MainController.instance.isFirstTimeGaming && detectTimer < 0)
        {
            Debug.Log("����tip��ʱ�����ڣ�" + detectTimer);
            Debug.Log("��ǰtip���б�������" + tipsList.Count);
            for (int i = tipsList.Count - 1; i >= 0; i--)
            {
                Debug.Log("��ǰtip��"+i+"���Ƿ񱻵������" + tipsList[i].clicked);
            }

            TipsData tipToShow = null;//��¼��Ҫչʾ��tip����
            int tipPos = -1;//��¼tip��λ�ã����������ɾ������
            int unclicked = -1;//��¼��ߵ�һ��û�б������������

            if (tipsList.Count > 0)
            {
                //�Ӻ�߱����б��ҵ���һ��û�б��������tip
                for(int i = tipsList.Count - 1; i >= 0; i--)
                {
                    if (!tipsList[i].clicked)
                    {
                        tipToShow = tipsList[i];
                        tipPos = i;
                        tipsUIObj.gameObject.SetActive(true);
                        
                        unclicked = i;
                        Debug.Log("��ǰ��tip��ʾ���¼������������Ϊ��" + i);
                        break;
                    }
                }
                

            }else
            {
                tipsUIObj.gameObject.SetActive(false);
                Debug.Log("��ǰû��tip��ʾ���¼�����");
            }
                      
            //��û�б��������������ǰ��
            for(int i = unclicked - 1; i >= 0; i--)
            {
                if(!tipsList[i].clicked && tipsList[i].priority > tipToShow.priority)
                {
                    tipToShow = tipsList[i];
                    tipPos = i;
                }               
            }

            if (tipToShow != null)
            {
                //�����ı�
                setText(tipToShow.content);

                //����ͷ��
                if(tipToShow.catNumber >= 0)
                {
                    setIcon(tipToShow.catNumber);
                }
                

                //��յ���¼��İ�
                tipsUIObj.transform.GetComponent<Button>().onClick.RemoveAllListeners();

                //���õ���¼�
                if(tipToShow.catNumber >= 0)
                {
                    //��������ҳ���ر�toast
                    tipsUIObj.transform.GetComponent<Button>().onClick.AddListener(() => showCatDetail(tipToShow.catNumber));
                    tipsUIObj.transform.GetComponent<Button>().onClick.AddListener(() => closeTips(tipPos));
                }
                else
                {
                    //�ر�toast
                    tipsUIObj.transform.GetComponent<Button>().onClick.AddListener(() => closeTips(tipPos));
                    
                }

            }

            detectTimer = detectTimeGap;
        }

        //�����������tips
        clearTimer -= Time.deltaTime;
        if(clearTimer < 0)
        {
            for(int i = tipsList.Count - 1; i >= 0; i--)
            {
                //�������
                if (tipsList[i].clicked)
                {
                    tipsList.RemoveAt(i);
                }
            }

            clearTimer = clearTimeGap;
        }
    }

    //����tips�����ݼ���ת
    public void setTip(int type, int catNumber = -1)
    {

        bool ifSet = true;
        for(int i = 0; i < tipsList.Count; i++)
        {
            //�жϸ���Ϣ�Ƿ�����list��
            if(type == tipsList[i].type && catNumber == tipsList[i].catNumber)
            {
                //Debug.Log("����Ϣ�Ѵ����б��У���������");

                ifSet = false;
                break;
            }
            
        }

        if(ifSet)
        {
            spawnTipData(type, catNumber);
        }

    }

    public void spawnTipData(int type, int catNumber)
    {
        Debug.Log("������Ϣ����Ϊ��" + type);
        Debug.Log("������Ϣ��Сè���Ϊ��" + catNumber);

        //��ʼ���м����
        string content = "";
        int priority = -1;

        /*��ʾ���ͣ�
        * 0��Сè�����鵤˵������.��
        * 1��Сè�����鵤����.��
        * 2���鵤�����ﵽ���ޣ����۳������ѡ�
        * 3��Сè��ʯ����.��
        * 4��ʣ������Сè����.��
        * 5�������ʯ�������ѡ�
        * 6���鵤�۸���������
        * 7��Сè�������ѡ�
        * 8��Сè�����ﵽ�������ѡ�
        * 9����Ⱥ�ﵽ�����������ѡ�
        */
        //�ж���Ϣ����
        if (type == 0)
        {
            content = "Сè������Ƶ��鵤�ŵ��ص���";
            priority = 100;
        }
        else if (type == 1)
        {
            content = "����鵤���ɰ���Сè <b><color=#F32D2D>ԭ��</color></b> �����鵤";
            priority = 99;

        }
        else if (type == 2)
        {
            content = "�鵤�����������ޣ�Сè���� <b><color=#F32D2D>����</color></b> ����";
            priority = 1;

        }
        else if (type == 3)
        {
            content = "Сè����ʯ���㣬�޷����������Ϊ";
            priority = 2;

        }
        /*else if (type == 4)//ÿ�δ���Ϸ������1��
        {
            
            content = "������ <b><color=#F32D2D>" + NewCatController.instance.outCatNumber.ToString() + "</color></b> ֻСè��������";
            priority = 1;
        }*/
        else if (type == 5)
        {
            content = "��ʯ�������٣�<b><color=#F32D2D>��ʼ����</color></b>���԰���Сè���������ٻ����ʯ";
            priority = 1;

        }
        else if (type == 6)
        {
            content = "<b><color=#F32D2D>15������</color></b>�鵤�۸�����������Сè�����鵤";
            priority = 2;

        }
        else if (type == 7)
        {
            content = "������Сè��Ϊ����������������";
            priority = 1;
        }
        else if (type == 8)
        {
            content = "Сè�����ﵽ���ޣ����èȺ�ȼ�������������Сè";
            priority = 1;
        }
        else if (type == 9)
        {
            content = "������èȺ�����������������ȥ����";
            priority = 1;
        }

        //����tip���ݶ���
        TipsData tip = new TipsData();
        tip.type = type;
        tip.catNumber = catNumber;
        tip.content = content;
        tip.priority = priority;
        tip.clicked = false;

        //��ӵ������б���
        tipsList.Add(tip);
    }

        //����tips���ı�
    public void setText(string content)
    {
        text.text = content;
    }

    //����ı�Сèicon�����ͣ�������Сè���ͣ���ʾ���ͣ�Сè˳��
    public void setIcon(int catNumber)
    {
        Debug.Log("����ı���Ϣ��Сèicon�ı�ţ�" + catNumber);
        string path = "Materials/BigCat/cat" + CatController.instance.cats[catNumber].cat_icon;
        Debug.Log("Сèͷ�� path:" + path);
        Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
        catIcon.sprite = sprite;
        
    }

    //�󶨵���¼����ɴ�Сè����ҳ
    public void showCatDetail(int catNumber)
    {
        Debug.Log("�����Сè��ţ�" + catNumber);
        catDetailPanel.gameObject.SetActive(true);
        CatDetailController.instance.showCatUI(catNumber);
    }

    public void closeTips(int tipPos)
    {
        Debug.Log("�������tip˳��Ϊ��" + tipPos);
        tipsList[tipPos].clicked = true;
        tipsUIObj.gameObject.SetActive(false);
    }

}

[Serializable]
public class TipsData
{
    /*��ʾ���ͣ�
     * 0��Сè�����鵤˵������
     * 1��Сè�����鵤����
     * 2���鵤�����ﵽ���ޣ����۳�������
     * 3��Сè��ʯ����
     * 4��ʣ������Сè����
     * 5�������ʯ��������
     * 6���鵤�۸���������
     * 7��Сè��������
     * 8��Сè�����ﵽ��������
     * 9����Ⱥ�ﵽ������������
     */
    public int type;//��ʾ����
    public int catNumber;//Сè˳��
    public string content;//��ʾ����

    /*��ֵԽ�����ȼ�Խ��
     * 
     * ����������ʾ����100��ʼ
     * 
     * �������ѣ���0��ʼ
     * 0������Ҫ����
     * 1���ٽ������Ϊ����
     * 2����Ҫ����������
     */
    public int priority;//չʾ�����ȼ�

    public bool clicked;//�Ƿ��Ѿ��������
}