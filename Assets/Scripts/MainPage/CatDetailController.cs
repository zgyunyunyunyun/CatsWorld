using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public void showCatUI(int num)
    {
        cat = CatController.instance.cats[num];

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
            rank.text = cat.big_level + " " + cat.small_level + "��";
        }
        else if(cat.big_level == "������")
        {
            capacity.text = "���������ƶ��׵�ҩ";
            cultivation.text = "��Ϊ��" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
            consumeStone.text = "���ģ�" + cat.lingshi_consume.ToString() + "��ʯ/��";
            rank.text = "<color=#19932D>" + cat.big_level + "</color> " + cat.small_level + "��";
        }
        else if (cat.big_level == "����")
        {
            capacity.text = "�������������׵�ҩ";
            cultivation.text = "��Ϊ��" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
            consumeStone.text = "���ģ�" + cat.lingshi_consume.ToString() + "��ʯ/��";
            rank.text = "<color=#805C00>" + cat.big_level + "</color> " + cat.small_level + "��";
        }
        else if (cat.big_level == "ԪӤ��")
        {
            capacity.text = "�����������Ľ׵�ҩ";
            cultivation.text = "��Ϊ��" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
            consumeStone.text = "���ģ�" + cat.lingshi_consume.ToString() + "��ʯ/��";
            rank.text = "<color=#F21BEA>" + cat.big_level + "</color> " + cat.small_level + "��";
        }
        else if (cat.big_level == "������")
        {
            capacity.text = "������������׵�ҩ";
            cultivation.text = "��Ϊ��" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
            consumeStone.text = "���ģ�" + cat.lingshi_consume.ToString() + "��ʯ/��";
            rank.text = "<color=#FF1010>" + cat.big_level + "</color> " + cat.small_level + "��";
        }

        
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
            }

            /*������ˢ������
             * 
             */

            //�ı���ʯ��������
            cat.lingshi_consume = (int)(cat.small_level * Mathf.Pow(10, CatController.instance.levelStringToNumber(cat.big_level)));
            consumeStone.text = "���ģ�" + cat.lingshi_consume.ToString() + "��ʯ/��";

            //�ı侳����İ�
            rank.text = cat.big_level + " " + cat.small_level + "��";

            //��Ϊ��գ����ı��������Ϊ��������Ϊ�İ�
            cat.cultivation = 0;
            if (cat.big_level == "������")
            {
                capacity.text = "����������һ�׵�ҩ";
                cultivation.text = "��Ϊ��" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
                consumeStone.text = "���ģ�" + cat.lingshi_consume.ToString() + "��ʯ/��";
            }
            else if (cat.big_level == "������")
            {
                capacity.text = "���������ƶ��׵�ҩ";
                cultivation.text = "��Ϊ��" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
                consumeStone.text = "���ģ�" + cat.lingshi_consume.ToString() + "��ʯ/��";
            }
            else if (cat.big_level == "����")
            {
                capacity.text = "�������������׵�ҩ";
                cultivation.text = "��Ϊ��" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
                consumeStone.text = "���ģ�" + cat.lingshi_consume.ToString() + "��ʯ/��";
            }
            else if (cat.big_level == "ԪӤ��")
            {
                capacity.text = "�����������Ľ׵�ҩ";
                cultivation.text = "��Ϊ��" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
                consumeStone.text = "���ģ�" + cat.lingshi_consume.ToString() + "��ʯ/��";
            }
            else if (cat.big_level == "������")
            {
                capacity.text = "������������׵�ҩ";
                cultivation.text = "��Ϊ��" + cat.cultivation + "/" + CatController.instance.levelNeedCul(cat.small_level, cat.big_level).ToString();
                consumeStone.text = "���ģ�" + cat.lingshi_consume.ToString() + "��ʯ/��";
            }

            //���غ��
            //upRankBtn.gameObject.SetActive(false);
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
                cat.had_stone += (int)PropertyController.instance.lingshiNumber;
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
