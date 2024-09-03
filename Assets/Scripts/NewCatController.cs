using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using WeChatWASM;
using Random = UnityEngine.Random;

public class NewCatController : MonoBehaviour//��Ȼ����д����Сè�����������̽����Ѱ��
{
    public TMP_Text consumeText;//������ʾ�İ�
    public TMP_Text freeTimeText;//ʱ�䵹��ʱ
    public TMP_Text freeTipsText;//�״δ���Ϸʱ����ѻ��Сè�ĵ���ʱ
    public TMP_Text outCatNumberText;//��ǰ����Сè����������

    private DateTime currentTime;//��¼��ǰʱ��
    private string hour = "00";
    private string minute = "00";
    private string second = "00";

    public DateTime lastNewTime;//��¼��һ����ļ��ʱ��

    private bool isFree = false;

    public int consumeStone = 100;//��һ�����ĵ���ʯ����

    public int time = 0;//��ǰ�ڼ��ε���Сè

    public int freeNewTime = 3;//��ѵ���Сè����
    public int shareTime = 3;//�ɷ���Ĵ���

    public Button newBtn;//����Сè��ť
    public Button shareBtn;//����ť
    public GameObject chooseCatUI;//ѡ��Сè��UI
    public GameObject newCatUI;//Ѱ��Сè��UI
    public GameObject toast;//��ʯ����toast

    public Image redPoint;//����Ƿ�չʾ

    public int probabilityToNewCat;//����Сè�ĸ���

    public int outCatNumber = 0;//����Сè������

    private float checkShowTipsTime = 10.0f;//ÿns���һ���Ƿ���Ҫ�ж�չʾtips
    private float checkShowTipsGap = 10.0f;//�м���ns

    string[] Name = { "��ˮ����", "���ĳ�����", "�����ȵ����� ", "Ѹ��", "������",
        "����С��Ů", "��Ϊ����ʫ", "Ѹ��", "ǳͫ", "��ˮ����", "ëë", "ǳɫ��ĩ", "���ߵ�",
        "����", "�ú�", "�»�֮��", "��Ѿ", "����", "�仨", "����֮��", "��ҹѡ�ֵ�һ��", "����ү",
        "ů����Ů", "�ǿ�Ů��", "С����", "����С��Ů", "ө��֮��", "����", "��ѩ��Ů", "������", "����", "ѩ����", "������", "�����껪",
        "��Ů", "Ů��", "����˯����", "С��Ů", "���������", "��һ��", "��ѩŮ", "��", "����", "��", "����", "�껪",
        "һֱ��������", "ƴ��Ů��", "С�ùԹ�", "������ɰ���", "ͽ��ժ��", "С������", "����Ӱ��", "�����崿", "ů��", "С��", "��ˮ����", "��è��Ů",
        "��һ�ڴ�����", "��Ұ����", "�򲻵���С����", " ���̸ǵ�С��Ů", "��סħ�ɱ�", "������������", "��Ե���", "��Ȼ��", "���� ", "ѩ��", "����", "�껪"};

    string[] level = { "����", "����", "����", "����", "����", "ԪӤ��", "ԪӤ��", "ԪӤ��", "������" };
    public GameObject outCatTips;//tips����

    public static NewCatController instance;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //���»ص�������������Ӱ��ĵط���Ҫ���ڣ�ԭ���и��ۼƵ����ݣ���Ҫͨ��update�����£������Ǳ�start��ʼ������

        //lastNewTime = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        //ÿ�ε����𲽵���������ʯ
        consumeStone = int.Parse((Mathf.Pow(2, time)).ToString() + "00");

        //��������������ʱ�����Сè�����仯�������ֵ������������ݵ���ֵ
        if (outCatNumber > 0)
        {
            SceneTransferData.instance.outCatNumber = outCatNumber;
        }

        checkShowTipsTime -= Time.deltaTime;
        if(checkShowTipsTime <= 0)
        {
            int pro = Random.Range(0, 110);

            if(pro >= 90 && pro < 110)
            {
                outCatTips.SetActive(true);
                string n = Name[Random.Range(0, Name.Length - 1)];
                string l = level[Random.Range(0, level.Length - 1)];
                outCatNumberText.text = "��ϲ�����"+n+ "Ѱ�ҵ���<color=#9800FF>" + l + "</color>�����Сè";
            }
            else if (pro >= 80 && pro < 90)
            {
                outCatTips.SetActive(true);
                outCatNumberText.text = "ע�⣺������������<color=#9800FF>" + outCatNumber.ToString() + "</color>ֻСè�������ˣ���ȥѰСè";
            }
            else if (pro >= 70 && pro < 80)
            {
                outCatTips.SetActive(true);
                outCatNumberText.text = "��ʾ��Сè���Զ�������͵͵���鵤�ղص��ص���";
            }
            else if (pro >= 60 && pro < 70)
            {
                outCatTips.SetActive(true);
                outCatNumberText.text = "��ʾ������鵤���ɳ��ۣ��ص�������Сè��͵͵��۳���";
            }
            else if (pro >= 50 && pro < 60)
            {
                outCatTips.SetActive(true);
                outCatNumberText.text = "��ʾ��������Ͻ�Сèͷ�����������ȼ������ɸ���Сè";
            }
            else if (pro >= 40 && pro < 50)
            {
                outCatTips.SetActive(true);
                outCatNumberText.text = "��ʾ������ʯ����ʱ���������Ƶ�ҩ���ٻ�ô�����ʯ";
            }
            else if (pro >= 30 && pro < 40)
            {
                outCatTips.SetActive(true);
                outCatNumberText.text = "��ʾ������������Ҫ��ʯ��������ͬʱ�ǵ�����СèŶ";
            }
            else if (pro >= 20 && pro < 30)
            {
                outCatTips.SetActive(true);
                outCatNumberText.text = "��ʾ��Сè������Ҫ������ʯ�����ͣ���";
            }
            else if (pro >= 10 && pro < 20)
            {
                outCatTips.SetActive(true);
                outCatNumberText.text = "��ʾ��Сèͨ�������ɻ����Ϊ���Ӷ�����";
            }
            else if (pro >= 0 && pro < 10)
            {
                outCatTips.SetActive(true);
                outCatNumberText.text = "��ʾ��Сè�����󣬿��Դ�ܸ��ߵȼ��ĵ���";
            }
            else
            {
                outCatTips.SetActive(false);
                Debug.Log("����СètipsΪ��");
            }

            checkShowTipsTime = checkShowTipsGap;
        }

        

        //����ǵ�һ�δ���Ϸ�����3�Σ�չʾ�İ��������
        if (MainController.instance.isFirstTimeGaming)
        {
            freeTipsText.gameObject.SetActive(true);
            isFree = true;
            freeTipsText.text = "�״�����Ϸ�������Ѱ��" + freeNewTime.ToString() + "��Сè";

            if (freeNewTime <= 0)
            {
                MainController.instance.isFirstTimeGaming = false;
                isFree = false;
                time = 0;
            }
        }


        //���������İ�����
        if (isFree)
        {
            consumeText.text = "���";
            freeTimeText.text = " ";
            time = 0;

            //���º������
            redPoint.gameObject.SetActive(true);

            shareBtn.gameObject.SetActive(false);
            newBtn.transform.localPosition = new Vector3(0, -500, 0);
        }
        else
        {
            //���º������
            redPoint.gameObject.SetActive(false);

            //��������İ�
            freeTipsText.gameObject.SetActive(false);

            consumeText.text = "���ģ�<b><color=#2D5AFD>" + consumeStone.ToString() + "</color></b> ��ʯ\n������ʯԽ�࣬Ѱ�ҵ�Сè����Խ��"; 

            //����ʱ�䵹��ʱ
            currentTime = DateTime.Now;

            TimeSpan difference = currentTime - lastNewTime; // ����ʱ���

            float secondsDifference = (float)difference.TotalSeconds; // ����������

            //���ʱ�䶼��0������ʱ����������չʾ��Ѱ�ť����������չʾʱ�䵹��ʱ
            if (secondsDifference >= 3600)
            {
                isFree = true;
                freeNewTime = 1;
                Debug.Log("�������");
            }else
            {
                //����ʱʣ��ʱ��
                float rest = 3600 - secondsDifference;
                int m = ((int)rest) / 60;
                int s = ((int)rest) - m * 60;

                //Debug.Log("M" + m);
                //Debug.Log("S" + s);

                if (m < 10)
                {
                    minute = "0" + m.ToString();
                }
                else
                {
                    minute = m.ToString();
                }

                if (s < 10)
                {
                    second = "0" + s.ToString();
                }
                else
                {
                    second = s.ToString();
                }

                if(shareTime > 0)
                {
                    freeTimeText.text = hour + ":" + minute + ":" + second + " ����ѣ�ÿ�տ�<color=#F32D2D>����</color>3���������";

                    if (!shareBtn.gameObject.activeSelf)
                    {
                        newBtn.transform.localPosition = newBtn.transform.localPosition + new Vector3(170, 0, 0);
                    }
                    shareBtn.gameObject.SetActive(true);
                }
                else
                {
                    freeTimeText.text = hour + ":" + minute + ":" + second + " �����";
                }

            }

        }


    }

    //��Ѵ�������1�������Ѵ���Ϊ0���򵹼�ʱ����
    public void minusOneFreeTime()
    {
        freeNewTime--;
        if (freeNewTime <= 0)
        {
            isFree = false;
            lastNewTime = DateTime.Now;
        }
        else
        {
            isFree = true;
        }     
    }

    //��Сè����ҳ
    public void newCatAndOpenCatDetail()
    {


        if (UpRaceController.instance.MaxCatNumber(UpRaceController.instance.raceLevel) <= CatController.instance.cats.Count)
        {
            //������ʾСè�����ﵽ��ֵ
            toast.SetActive(true);
            toast.GetComponent<Toast>().setText("Сè�����Ѵﵽ���ֵ������������ȼ�");
            Debug.Log("Сè�����ﵽ��ֵ");
        }
        //������㵮��Сè����������ѻ������㹻��ʯ�����ݶ�100��ʯ����1�Σ���������Сè��������ʾ��ʯ����
        else if (isFree || PropertyController.instance.lingshiNumber >= consumeStone && UpRaceController.instance.MaxCatNumber(UpRaceController.instance.raceLevel) > CatController.instance.cats.Count)
        {
            //����������ʯ
            if (!isFree)
            {
                PropertyController.instance.consumeLingShi(consumeStone);
            }

            //����ǵ�һ������Ϸ���ص�3ֻСè��������ʯ������ȥ���Сè
            if (MainController.instance.isFirstTimeGaming)
            {
                ChooseCatUI.instance.newCatAndCatUI(0);//����Сè
                chooseCatUI.gameObject.SetActive(true);

                minusOneFreeTime();
            }
            else
            {
                //����list�����׻�ã���̫���ܻ�ã��м�ֵ
                int[] proList =
                    { 90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 99, 95, 95, 95,
                    90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 99, 95, 95, 95,
                    90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 90, 80, 70, 99, 95, 95, 95,
                     70, 60, 50, 50,70, 70, 60, 50, 70, 90, 80, 70, 70, 60, 50, 50,70, 70, 60, 50, 50, 20, 20, 10,
                    70, 60, 50, 50,70, 70,70, 60, 50, 50,70, 70,70, 60, 50, 50,70, 70,70, 60, 50, 50,70, 70,70, 60, 50, 50,
                    50, 50,70, 70, 60, 50, 50, 20, 20, 10,30, 30, 10, 10, 10, 15, 10, 30, 40, 50, 10 };

                int r = Random.Range(0, 100);
                probabilityToNewCat = proList[Random.Range(0, proList.Length - 1)];//���ѡ������list�ĸ���

                Debug.Log("����Сè�Ĵ�����" + time);
                //����ɹ�����Сè����������Сè�����������򵯳���ʾ����ʧ��
                if (r >= probabilityToNewCat)
                {
                    ChooseCatUI.instance.newCatAndCatUI(0);//����Сè
                    //newCatUI.gameObject.SetActive(false);
                    chooseCatUI.gameObject.SetActive(true);
                }
                else
                {
                    //��������ʧ��
                    toast.SetActive(true);
                    toast.GetComponent<Toast>().setText("����̽��δ��Ѱ�ҵ�Сè");
                    Debug.Log("����̽��δ��Ѱ�ҵ�Сè");

                }

                //�������Сè����ʱ
                if (isFree)
                {
                    minusOneFreeTime();
                    Debug.Log("������ѵ���ʱ");
                }

                //��ǰ�ִ������Сè����+1
                time++;
                
            }
                   
            
        }
        else if(PropertyController.instance.lingshiNumber < consumeStone)
        {
             //������ʾ��ʯ����
             toast.SetActive(true);
             toast.GetComponent<Toast>().setText("��ʯ���㣬���ø�����ʯ");
             Debug.Log("������ʯ����toast");
        }
        
        

    }


    //������
    public void Share()
    {
        WX.ShareAppMessage(new ShareAppMessageOption
        {
            //imageUrl = imageUrl, // ͼƬ��URL��Ҳ���Բ���Զ�������
            title = "�ٺ٣������Ͼ����ҵ�" + outCatNumber.ToString() + "ֻèè��", // ��ʾ�ı�
            //query = query, // ��������������2k����
        });


        isFree = true;
        freeNewTime = 1;
        shareTime--;
        Debug.Log("�������");
    }
}
