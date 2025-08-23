using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WeChatWASM;

public class CatDetailController : MonoBehaviour
{
    public Image catPicture;
    public TMP_Text catName;
    public TMP_Text state;
    public TMP_Text rank;
    public TMP_Text introduciton;
    public TMP_Text capacity;
    public TMP_Text cultivation;
    public TMP_Text consumeStone;
    public TMP_Text hadStone;

    public Image rankPicture;//

    public Button upRankBtn;//������ť

    public Image upRedPoint;//�����ĺ��
    public Image stoneRedPoint;//��ʯ����ĺ��

    public GameObject toast;

    private Cat cat;


    public static CatDetailController instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {


    }

    private void Update()
    {
        //����Сèӵ�е���ʯ����
        if(cat != null)
        {
            hadStone.text = "��ʯ��" + cat.had_stone;
            cultivation.text = "��Ϊ��" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();

            //�ж�Сè�Ƿ��չʾ�������Ͱ�ť
            if (cat.cultivation >= CatController.instance.levelNeedCul(cat.small_level, cat.big_level))
            {
                upRankBtn.gameObject.SetActive(true);
                upRedPoint.gameObject.SetActive(true);
            }
            else
            {
                upRedPoint.gameObject.SetActive(false);
            }
        }


    }

    //��Сè����ҳ��չʾСè����ʽ
    public void showCatUI(int catID)
    {
        for(int i=0;i< CatController.instance.cats.Count; i++)
        {
            if(catID == CatController.instance.cats[i].cat_id)
            {
                cat = CatController.instance.cats[i];
            }
        }
        

        //�Ի�õ�UI���и�ֵ
        string path = "Materials/BigCat/cat" + cat.cat_icon.ToString();
        Debug.Log("�ɹ�����Сè��Сèͷ�� path:" + path);
        Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
        catPicture.sprite = sprite;

        catName.text = cat.cat_name;
        //state.text = cat.work;
        
        hadStone.text = "��ʯ��" + cat.had_stone;

        introduciton.text = "�Ը�" + cat.introuction;

        //�ж��Ƿ�չʾ���
        if (cat.had_stone <= 0)
        {
            stoneRedPoint.gameObject.SetActive(true);
        }
        else
        {
            stoneRedPoint.gameObject.SetActive(false);
        }

        if (cat.big_level == "������")
        {
            capacity.text = "����������һ�׵�ҩ";
            cultivation.text = "��Ϊ��" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
            consumeStone.text = "���ģ�" + cat.lingshi_consume.ToString() +"��ʯ/��";
            rank.text = "<color=#000000>" + cat.big_level + "</color> " + cat.small_level + "��";

            Color color = ParseHexColor("#000000");
            rankPicture.color = color;
        }
        else if(cat.big_level == "������")
        {
            capacity.text = "���������ƶ��׵�ҩ";
            cultivation.text = "��Ϊ��" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
            consumeStone.text = "���ģ�" + cat.lingshi_consume.ToString() + "��ʯ/��";
            rank.text = "<color=#19932D>" + cat.big_level + "</color> " + cat.small_level + "��";

            Color color = ParseHexColor("#19932D");
            rankPicture.color = color;
        }
        else if (cat.big_level == "����")
        {
            capacity.text = "�������������׵�ҩ";
            cultivation.text = "��Ϊ��" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
            consumeStone.text = "���ģ�" + cat.lingshi_consume.ToString() + "��ʯ/��";
            rank.text = "<color=#C3A010>" + cat.big_level + "</color> " + cat.small_level + "��";

            Color color = ParseHexColor("#C3A010");
            rankPicture.color = color;
        }
        else if (cat.big_level == "ԪӤ��")
        {
            capacity.text = "�����������Ľ׵�ҩ";
            cultivation.text = "��Ϊ��" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
            consumeStone.text = "���ģ�" + cat.lingshi_consume.ToString() + "��ʯ/��";
            rank.text = "<color=#A72EB0>" + cat.big_level + "</color> " + cat.small_level + "��";

            Color color = ParseHexColor("#A72EB0");
            rankPicture.color = color;
        }
        else if (cat.big_level == "������")
        {
            capacity.text = "������������׵�ҩ";
            cultivation.text = "��Ϊ��" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
            consumeStone.text = "���ģ�" + cat.lingshi_consume.ToString() + "��ʯ/��";
            rank.text = "<color=#FF1010>" + cat.big_level + "</color> " + cat.small_level + "��";

            Color color = ParseHexColor("#FF1010");
            rankPicture.color = color;
        }

    }

    //��ɫת��
    Color ParseHexColor(string hexColor)
    {
        hexColor = hexColor.TrimStart('#');
        byte r = byte.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, 255); // �����Ҫ͸���ȣ��������������alphaֵ
    }

    //����Сè�ȼ�����
    /*Сè���磺
     * �󾳽磺����������
     * С���磺10��
     * 
     */
    public void upCatRank()
    {
        if (cat != null && cat.cultivation >= CatController.instance.levelNeedCul(cat.small_level, cat.big_level))
        {
            //���С���粻��10�������С���磬��������󾳽�
            if (cat.small_level < 10)
            {
                cat.small_level++;
            }
            else
            {
                cat.big_level = CatController.instance.numberToCatLevelString(CatController.instance.levelStringToNumber(cat.big_level) + 1);
                cat.small_level = 1;
            }

            /*������ˢ������
             * 
             */

            //�ı���ʯ��������
            cat.lingshi_consume = (int)(cat.small_level * Mathf.Pow(4, 1 + CatController.instance.levelStringToNumber(cat.big_level)));
            consumeStone.text = "���ģ�" + cat.lingshi_consume.ToString() + "��ʯ/��";


            //��Ϊ��գ����ı��������Ϊ��������Ϊ�İ�
            cat.cultivation = 0;
            if (cat.big_level == "������")
            {
                capacity.text = "����������һ�׵�ҩ";
                cultivation.text = "��Ϊ��" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
                consumeStone.text = "���ģ�" + cat.lingshi_consume.ToString() + "��ʯ/��";
                rank.text = "<color=#000000>" + cat.big_level + "</color> " + cat.small_level + "��";

                Color color = ParseHexColor("#000000");
                rankPicture.color = color;
            }
            else if (cat.big_level == "������")
            {
                capacity.text = "���������ƶ��׵�ҩ";
                cultivation.text = "��Ϊ��" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
                consumeStone.text = "���ģ�" + cat.lingshi_consume.ToString() + "��ʯ/��";
                rank.text = "<color=#19932D>" + cat.big_level + "</color> " + cat.small_level + "��";

                Color color = ParseHexColor("#19932D");
                rankPicture.color = color;
            }
            else if (cat.big_level == "����")
            {
                capacity.text = "�������������׵�ҩ";
                cultivation.text = "��Ϊ��" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
                consumeStone.text = "���ģ�" + cat.lingshi_consume.ToString() + "��ʯ/��";
                rank.text = "<color=#C3A010>" + cat.big_level + "</color> " + cat.small_level + "��";

                Color color = ParseHexColor("#C3A010");
                rankPicture.color = color;
            }
            else if (cat.big_level == "ԪӤ��")
            {
                capacity.text = "�����������Ľ׵�ҩ";
                cultivation.text = "��Ϊ��" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
                consumeStone.text = "���ģ�" + cat.lingshi_consume.ToString() + "��ʯ/��";
                rank.text = "<color=#A72EB0>" + cat.big_level + "</color> " + cat.small_level + "��";

                Color color = ParseHexColor("#A72EB0");
                rankPicture.color = color;
            }
            else if (cat.big_level == "������")
            {
                capacity.text = "������������׵�ҩ";
                cultivation.text = "��Ϊ��" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
                consumeStone.text = "���ģ�" + cat.lingshi_consume.ToString() + "��ʯ/��";
                rank.text = "<color=#FF1010>" + cat.big_level + "</color> " + cat.small_level + "��";

                Color color = ParseHexColor("#FF1010");
                rankPicture.color = color;
            }

            //���غ��
            upRedPoint.gameObject.SetActive(false);


            Debug.Log("Сè��ɽ���");
        }
        else
        {
            //������ʯ����ʧ��
            toast.SetActive(true);
            toast.GetComponent<Toast>().setText("��Ϊ���㣬�޷�����");
            Debug.Log("��ʯ���㣬����ʧ��");
        }
    }

    //������ʯ����
    public void giveStone(int stoneNumber)
    {
        //�жϴ�������ҳ��Сè�ǿ�
        if(cat != null)
        {
            //�ж��Ƿ��ṩȫ����ʯ��stoneNUmber==-1
            if (stoneNumber < 0)
            {
                cat.had_stone += PropertyController.instance.lingshiNumber;
                hadStone.text = "��ʯ��" + cat.had_stone;

                PropertyController.instance.lingshiNumber = 0;
            }
            else
            {   
                //�ж��Ƿ����㹻��ʯ����
                if(stoneNumber > PropertyController.instance.lingshiNumber)
                {
                    //������ʯ����ʧ��
                    toast.SetActive(true);
                    toast.GetComponent<Toast>().setText("��ʯ����");
                    Debug.Log("��ʯ���㣬����ʧ��");
                }
                else//���ɹ���������
                {
                    cat.had_stone += stoneNumber;
                    hadStone.text = "��ʯ��" + cat.had_stone;

                    PropertyController.instance.lingshiNumber -= stoneNumber;
                }

            }

        }
        else
        {
            Debug.Log("������ʯ��СèΪ��");
        }

        //�����Ժ��ж���ʯ�㲻��
        if (cat.had_stone <= 0)
        {
            stoneRedPoint.gameObject.SetActive(true);
        }
        else
        {
            stoneRedPoint.gameObject.SetActive(false);

        }
    }


}
