using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WeChatWASM;

public class ChooseCatUI : MonoBehaviour
{
    public Image catIcon;//Сèͷ��
    public TMP_Text catName;//Сè����
    public TMP_Text catIntro;//Сè���
    public TMP_Text catCapacity;//Сè����
    public TMP_Text catLevel;//Сè����
    public TMP_Text stoneNumber;//Я����ʯ������
    public TMP_Text freshBtnText;//ˢ�°�ť�İ�
    public Button freshBtn;//ˢ�°�ť�İ�
    public Button shareBtn;//����ť

    public TMP_Text freshTips;//ˢ��������ʾ

    private Cat currCat;//��ǰ�����СèUI����ֻ��

    private int currFreshTime = 3;//��ǰ��ˢ�µĴ���

    public static ChooseCatUI instance;
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
        if(currFreshTime <= 0)
        {

            shareBtn.gameObject.SetActive(true);
            freshTips.gameObject.SetActive(true);
            freshBtn.gameObject.SetActive(false);

            //freshBtn.interactable = false;
        }
        else
        {
            shareBtn.gameObject.SetActive(false);
            freshTips.gameObject.SetActive(false);
            freshBtn.gameObject.SetActive(true);
        }

        freshBtnText.text = "ˢ��(" + currFreshTime.ToString() + ")";
    }

    public void shareCat()
    {
        WX.ShareAppMessage(new ShareAppMessageOption
        {
            //imageUrl = imageUrl, // ͼƬ��URL��Ҳ���Բ���Զ�������
            title = "�ٺ٣���������1ֻèè", // ��ʾ�ı�
            //query = query, // ��������������2k����
        });

        currFreshTime = 5;
    }


    //�ڵ���Сè������ϣ�չʾСè������Ϣ�����������0������Сè��1��ˢ��Сè��
    public void newCatAndCatUI(int type)
    {
        if(type == 0)
        {
            //����Сè
            currCat = CatController.instance.newCat(NewCatController.instance.time);

            currFreshTime = 3;
            freshBtn.interactable = true;
        }
        else if(type == 1)
        {
            //ˢ��Сè
            currCat = CatController.instance.newCat(NewCatController.instance.time);

            currFreshTime--;
        }
        else
        {
            Debug.Log("����Сè����!");
        }
        

        //�������Сè����Ϣ
        string path = "Materials/BigCat/cat" + currCat.cat_icon;
        Debug.Log("�ɹ�����Сè��Сèͷ�� path:" + path);
        Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
        catIcon.sprite = sprite;

        catName.text = currCat.cat_name;
        catIntro.text = "�Ը�" + currCat.introuction;

        string capa = "";
        if (currCat.big_level == "������")
        {
            capa = "����һ�׵�ҩ";
            catLevel.text = "���磺<color=#000000>" + currCat.big_level + "</color> " + currCat.small_level.ToString() + " ��";
        }
        else if (currCat.big_level == "������")
        {
            capa = "���ƶ��׵�ҩ";
            catLevel.text = "���磺<color=#19932D>" + currCat.big_level + "</color> " + currCat.small_level.ToString() + " ��";
        }
        else if (currCat.big_level == "����")
        {
            capa = "�������׵�ҩ";
            catLevel.text = "���磺<color=#805C00>" + currCat.big_level + "</color> " + currCat.small_level.ToString() + " ��";
        }
        else if (currCat.big_level == "ԪӤ��")
        {
            capa = "�����Ľ׵�ҩ";
            catLevel.text = "���磺<color=#F21BEA>" + currCat.big_level + "</color> " + currCat.small_level.ToString() + " ��";
        }
        else if (currCat.big_level == "������")
        {
            capa = "������׵�ҩ";
            catLevel.text = "���磺<color=#FF1010>" + currCat.big_level + "</color> " + currCat.small_level.ToString() + " ��";
        }
        catCapacity.text = "������" + capa;
        
        stoneNumber.text = "��ʯ��<color=#2D5AFD>" + currCat.had_stone.ToString() + "</color>"; 

    }
    public void chooseCat()
    {
        CatController.instance.chooseCat(currCat);

        NewCatController.instance.outCatNumber--;
    }

}
