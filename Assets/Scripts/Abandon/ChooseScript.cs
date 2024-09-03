using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChooseScript : MonoBehaviour
{
    //ɸѡ��ť��list
    private Button[] filterButtons;

    //ѡ������ɸѡ��ť�ĸ��ڵ㣨���ݵȼ�ɸѡ��
    public GameObject buttons;
    public Button all;

    //�������ݵĸ��ڵ�
    public Transform contentParent;


    // Start is called before the first frame update
    void Start()
    {
        
        //���ɸѡ��ť����������ֵ��list
        int btnNumber = buttons.transform.childCount;
        filterButtons = new Button[btnNumber];

        for (int i=0; i< btnNumber; i++)
        {
            filterButtons[i] = buttons.transform.GetChild(i).GetComponent<Button>();
        }

        // ��ÿ��ɸѡ��ť��ӵ���¼�
        for (int i = 0; i < filterButtons.Length; i++)
        {          
            //index����n��ɸѡ��ť
            int buttonIndex = i;
            filterButtons[i].onClick.AddListener(() => FilterContent(buttonIndex));
        }
    }

    void Update()
    {
        //�޸ı�ѡ�еİ�ť��ɫ
        if (CatListUI.instance.currChoose != null)
        {
            //Debug.Log("�ı�ѡ����ɫ");
            if (CatListUI.instance.currChoose == "ȫ��")
            {
                filterButtons[0].GetComponent<Image>().color = new Color32(255, 128, 98, 255);
                filterButtons[1].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                filterButtons[2].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                filterButtons[3].GetComponent<Image>().color = new Color32(255, 255, 255, 255);

            }
            else if(CatListUI.instance.currChoose == "������")
            {
                
                filterButtons[1].GetComponent<Image>().color = new Color32(255, 128, 98, 255);
                filterButtons[0].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                filterButtons[2].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                filterButtons[3].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
            else if (CatListUI.instance.currChoose == "̽����")
            {
                filterButtons[2].GetComponent<Image>().color = new Color32(255, 128, 98, 255);
                filterButtons[0].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                filterButtons[1].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                filterButtons[3].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
            else if (CatListUI.instance.currChoose == "����")
            {
                filterButtons[3].GetComponent<Image>().color = new Color32(255, 128, 98, 255);
                filterButtons[0].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                filterButtons[1].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                filterButtons[2].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
        }
    }

    void FilterContent(int filterIndex)
    {
        /*
        // �����������������ɸѡ��ť�������������ǵļ���״̬
        foreach (Transform child in contentParent)
        {
            // ������Ҫɸѡ���߼��������������
            bool shouldActivate = true; 

            // ���磬���ݰ�ť����ɸѡ
            if ((child.GetSiblingIndex() % filterButtons.Length)  !=  filterIndex)
            {
                Debug.Log("ɸѡ�Ľ����" + child.GetSiblingIndex() % filterButtons.Length);
                shouldActivate = false;
            }
            child.gameObject.SetActive(shouldActivate);
        }
        */


        //ɸѡ����������ʲô����״̬����õ����ť���ص�ֵ
        string workstate = filterButtons[filterIndex].GetComponentInChildren<TMP_Text>().text;

        Debug.Log("ɸѡ�Ĺ���״̬�ǣ�" + workstate);

        if(workstate != null)
        {
            //contentParent.GetComponent<CatListUI>().RefreshUI(workstate);
        }
              
    }
}
