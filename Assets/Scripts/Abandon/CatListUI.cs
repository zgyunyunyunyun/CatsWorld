using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CatListUI : MonoBehaviour
{
    public GameObject workUI;
    public GameObject chooseWork;
    public GameObject greyBG;
    public Button disLiandanWork;
    public Button disTansuoWork;

    private List<Cat> cats = new List<Cat>();

    //private int disCatId;//�����乤��Сè��id����ʼ��δ-1
    public string currChoose = "ȫ��";//��ǰ��ѡ���Сè���࣬Ĭ����ȫ��

    public static CatListUI instance;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //disCatId = -1;
        //RefreshUI("ȫ��");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    //��յ�ǰ�ӽڵ�����³�ʼ���б�
    public void RefreshUI(string work_state)
    {
        currChoose = work_state;
        //����ӽڵ�
        if (transform.childCount > 0)
        {
            for (int j = 0; j < transform.childCount; j++)
            {
                GameObject.Destroy(transform.GetChild(j).gameObject);

                Debug.Log("�����ԭ���ڵ�������ӽڵ�");
            }
        }
        

        if (cats != null)
        {
            cats.Clear();
        }


        //���Сèlist����������
        List<Cat> tempcats = CatController.instance.cats;

        Debug.Log("Сè��������" + tempcats.Count);

        for (int j = 0; j < tempcats.Count; j++)
        {
            if(tempcats[j].work == "����")
            {
                cats.Insert(0, tempcats[j]);
            }
            else
            {
                cats.Add(tempcats[j]);
            }
        }


        int i;//����Сè
        int t = 0;//ʵ��չʾСè��λ��

        for (i = 0; i < cats.Count; i++)
        {
            //Сè������չʾ��������������������һ��ѭ��
            if(work_state != "ȫ��" && cats[i].work != work_state)
            {
                Debug.Log("������������Сè��ţ�" + i);
                continue;             
            }

            Debug.Log("��ǰСè������" + cats.Count);

            //���ʵ��������������ݣ����ں��渳ֵ
            GameObject temp = Instantiate(workUI);

            //��Сè�����б���Ÿ��ڵ���
            temp.transform.SetParent(this.transform, false);

            temp.transform.localPosition = new Vector3(410, -60 - t * 236, 0);

            Image catIcon = temp.transform.Find("catIcon").GetComponent<Image>();
            TMP_Text catName = temp.transform.Find("catName").GetComponent<TMP_Text>();
            TMP_Text workState = temp.transform.Find("workState").GetComponent<TMP_Text>();
            TMP_Text getThing = temp.transform.Find("getThing").GetComponent<TMP_Text>();
            Button button = temp.transform.Find("Button").GetComponent<Button>();
            TMP_Text contentBtn = button.transform.Find("Text (TMP)").GetComponent<TMP_Text>();


            //�Ի�õ�UI���и�ֵ
            string path = "Materials/" + cats[i].cat_icon;
            Debug.Log("�ɹ�����Сè��Сèͷ�� path:" + path);
            Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
            catIcon.sprite = sprite;

            catName.text = cats[i].cat_name;
            workState.text = cats[i].work;
            if (cats[i].work == "������")
            {
                getThing.text = "�ջ��鵤�������㣩";
                contentBtn.text = "ֹͣ";

                //�����ťʱ���ѵ�ǰ��Ҫ���乤����Сè��id��¼����
                int catid = cats[i].cat_id;
                button.onClick.AddListener(() => clickWorkBtn(catid));
                button.onClick.AddListener(() => this.DistributeWork("����"));
            }
            else if (cats[i].work == "̽����")
            {
                getThing.text = "�ջ���ݣ������㣩";
                contentBtn.text = "ֹͣ";

                //�����ťʱ���ѵ�ǰ��Ҫ���乤����Сè��id��¼����
                int catid = cats[i].cat_id;
                button.onClick.AddListener(() => clickWorkBtn(catid));
                button.onClick.AddListener(() => this.DistributeWork("����"));
            }
            else
            {
                getThing.text = "�ջ���";
                contentBtn.text = "����";

                button.onClick.AddListener(() => chooseWork.gameObject.SetActive(true));
                button.onClick.AddListener(() => greyBG.gameObject.SetActive(false));

                //�����ťʱ���ѵ�ǰ��Ҫ���乤����Сè��id��¼����
                int catid = cats[i].cat_id;
                button.onClick.AddListener(() => clickWorkBtn(catid));

                //�󶨰�ť������乤��
                disLiandanWork.onClick.AddListener(() => this.DistributeWork("������"));
                disTansuoWork.onClick.AddListener(() => this.DistributeWork("̽����"));
                disLiandanWork.onClick.AddListener(() => chooseWork.gameObject.SetActive(false));
                disLiandanWork.onClick.AddListener(() => greyBG.gameObject.SetActive(true));
                disTansuoWork.onClick.AddListener(() => chooseWork.gameObject.SetActive(false));
                disTansuoWork.onClick.AddListener(() => greyBG.gameObject.SetActive(true));
            }

            t++;//չʾ��1ֻСè�����ۼ�

        }
    }
    */
    /*
    void clickWorkBtn(int catID)
    {
        if (disCatId == -1)
        {
            disCatId = catID;
        }
        else
        {
            Debug.LogError("Сè�����������δ��ȷ��ʼ��");
        }    
    }*/

    /*
    void DistributeWork(string workState)
    {
        List<Cat> cats = CatController.instance.cats;
        //Debug.Log("׼�����乤��:");
        if (cats != null && cats.Count > 0)
        {
            for(int i =0; i < cats.Count; i++)
            {
                //�ж��Ƿ���Ҫ��ʼ���乤����ֹͣ����
                if (disCatId != -1 && cats[i].cat_id == disCatId)
                {
                    CatController.instance.cats[i].work = workState;

                    Debug.Log("Сè��ʼ�����ˣ���СèidΪ��" + cats[i].cat_id);

                    disCatId = -1;
                    RefreshUI(currChoose);
                }else
                {
                    Debug.LogError("���乤��ʧ��");
                }
            }
        }
        
    }
    */

}
