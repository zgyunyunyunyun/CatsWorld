using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CatController : MonoBehaviour
{
    //��������Сè��list��Сè�����ԣ�������ѡ�����Ϊ

    public List<Cat> cats = new List<Cat>(); //Ŀǰ��Ҿ߱�������Сè
    List<string[]> catNameList = new List<string[]>();

    private float timer = 1.0f;//���ʱ��


    public GameObject tips;//Сè��ʯ���������ʾ
    public int catTypeNumber = 5;//Сè��������

    public Image catListRedPoint;//Сè�б�ĺ��


    public static CatController instance;
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

    private void Start()
    {

        /*
        Debug.Log("��ʼ��ȡСè�����ļ�");

        string csvUrl = "https://catgame-v0001-4g0nvm54abfb04ae-1258905158.tcloudbaseapp.com/Cat/Cat_0005/Resources/DataFile/catName.csv"; // �滻Ϊ���CSV�ļ�·��
        //string[] lines = File.ReadAllLines(path, Encoding.GetEncoding(936));

        using (UnityWebRequest www = UnityWebRequest.Get(csvUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error downloading CSV: " + www.error);
            }
            else
            {
                string[] lines = www.downloadHandler.text.Split('\n');
                foreach (string line in lines)
                {
                    string[] columns = line.Split(',');

                    catNameList.Add(columns);
                    foreach (string column in columns)
                    {
                        Debug.Log(column);
                    }
                }
            }
        }

        foreach (string line in lines)
        {
            

            string[] values = line.Split(',');
            catNameList.Add(values);

            //Debug.Log("�����ȡ�ļ�ÿ�еĽ����" + values);
        }

        // �����ȡ������
        for (int i = 0; i < catNameList.Count; i++)
        {
            for (int j = 0; j < catNameList[i].Length; j++)
            {
                //Debug.Log("�����ȡСè�����ļ���" + i + "�еĽ��Ϊ��" + catNameList[i][j]);
            }
        }
        */

        string[] lines0 = { "��è", "����", "����", "С��", "����", "����", "����", "����", "����", "��", "ëë", "�в�", "Ǯ���", "�ϰ�", "����", "����", "��Ѿ", "����", "�仨", "��ţ", "��ү", "����", "��׳", "��", "С����", "����", "С��", "����", "����", "��", "����", "��Բ", "����", "����" };
        catNameList.Add(lines0);
        string[] lines1 = { "��è", "С��", "����", "����", "����", "����", "����", "��", "ëë", "�в�", "Ǯ���", "�ϰ�", "����", "����", "��Ѿ", "����", "�仨", "��ţ", "��ү", "����", "��׳", "��", "С����", "����", "С��", "����", "����", "��", "����", "��Բ", "����", "����", "����", "����" };
        catNameList.Add(lines1);
        string[] lines2 = { "��è", "����", "С��", "�ֻ�", "С��", "â��", "����", "С̫��", "���", "����", "����", "����", "����", "��", "ëë", "�в�", "Ǯ���", "�ϰ�", "����", "����", "��Ѿ", "����", "�仨", "��ţ", "��ү", "����", "��׳", "��", "С����", "����", "С��", "����", "����", "��" };
        catNameList.Add(lines2);
        string[] lines3 = { "��è", "����", "С��", "С��", "����", "����", "����", "��", "ëë", "�в�", "Ǯ���", "�ϰ�", "����", "����", "��Ѿ", "����", "�仨", "��ţ", "��ү", "����", "��׳", "��", "С����", "����", "С��", "����", "����", "��", "����", "��Բ", "����", "����", "����", "����" };
        catNameList.Add(lines3);
        string[] lines4 = { "����è", "����", "С��", "����", "����", "����", "����", "����", "��", "ëë", "�в�", "Ǯ���", "�ϰ�", "����", "����", "��Ѿ", "����", "�仨", "��ţ", "��ү", "����", "��׳", "��", "С����", "����", "С��", "����", "����", "��", "����", "��Բ", "����", "����", "����" };
        catNameList.Add(lines4);
        string[] lines5 = { "��è", "С��", "����", "����", "����", "����", "����", "��", "ëë", "�в�", "Ǯ���", "�ϰ�", "����", "����", "��Ѿ", "����", "�仨", "��ţ", "��ү", "����", "��׳", "��", "С����", "����", "С��", "����", "����", "��", "����", "��Բ", "����", "����", "����", "����" };
        catNameList.Add(lines5);
        string[] lines6 = { "�ٺ�è", "����", "С��", "����", "����", "����", "����", "��", "ëë", "�в�", "Ǯ���", "�ϰ�", "����", "����", "��Ѿ", "����", "�仨", "��ţ", "��ү", "����", "��׳", "��", "С����", "����", "С��", "����", "����", "��", "����", "��Բ", "����", "����", "����", "����" };
        catNameList.Add(lines6);

        catTypeNumber = catNameList.Count;

    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            autoConsumeStone();

            bool redPointShowed = false;//����Ƿ�չʾ��?
            //����������ʾ
            for (int i = 0; i < cats.Count; i++)
            {
                //��ʯ��������
                if (cats[i].had_stone <= 0)
                {
                    Debug.Log("������ʯ���������Ϣ��չʾ���");

                    //������Ϣ
                    //Tips.instance.setTip(3, i);

                    redPointShowed = true;
                }

                //Сè����������
                bool canUp = cats[i].cultivation + cats[i].lingshi_consume > levelNeedCul(cats[i].small_level, cats[i].big_level);
                if (canUp)
                {
                    Debug.Log("���Ϳ���������Ϣ��չʾ���");

                    //������Ϣ
                    Tips.instance.setTip(7, i);
                    cats[i].canUp = true;

                    redPointShowed = true;
                }
                else
                {
                    cats[i].canUp = false;
                }
            }

            //���������Ժ󣬻���չʾ��㣬����������
            catListRedPoint.gameObject.SetActive(redPointShowed);
            

            timer = 1.0f;
        }

    }


    //����������Ե�Сè����δ����Сè���б��У�
    //����:��ǰ����Сè�Ĵ����������ж��Ƿ�Ҫ����Сè�ĵȼ���
    public Cat newCat(int newCatTime)
    {
        Cat newCat = new Cat();

        //��������Сè�����ֵ
        newCat.cat_id = cats.Count;//Сèid��0��ʼ
                                  
        newCat.cat_icon = Random.Range(0, catTypeNumber - 1);//icon��Сè����
        int rName = Random.Range(1, catNameList[0].Length - 1);//�������������

        Debug.Log("Ѱ�ҵ�Сè���ͣ�" + newCat.cat_icon);
        Debug.Log("Ѱ�ҵ�Сè������ţ�" + rName);

        newCat.cat_name = catNameList[newCat.cat_icon][rName];

        string[] intro = { "����", "ų��", "��Ƥ", "����", "����", "�߰�", "����", "��Į" };

        newCat.introuction = intro[Random.Range(0, intro.Length - 1)];

        //��������Сè�Ĵ��������Сè�ĵȼ�
        newCat.big_level = "������";

        if(newCatTime < 8 && newCatTime >= 5 && Random.Range(0, 100) >= 80)
        {
            newCat.big_level = "������";
            Debug.Log("Ѱ�ҵ�Сè�ȼ�??��" + newCat.big_level);
        } 

        if (newCatTime < 12 && newCatTime >= 8 && Random.Range(0, 100) >= 80)
        {
            newCat.big_level = "����";
        }

        if (newCatTime >= 12 && Random.Range(0, 100) >= 80)
        {
            newCat.big_level = "ԪӤ��";
        }
        Debug.Log("Ѱ�ҵ�Сè������" + newCatTime);
        Debug.Log("Ѱ�ҵ�Сè�ȼ���" + newCat.big_level);

        newCat.small_level = Random.Range(1, 5);
        newCat.cultivation = 0;
        newCat.canUp = false;
        newCat.lingshi_consume = (int)(newCat.small_level * Mathf.Pow(10, levelStringToNumber(newCat.big_level) + 1));
        newCat.had_stone = Random.Range(1,10)* (int)(Mathf.Pow(10, levelStringToNumber(newCat.big_level) + 2));

        return newCat;
    }

    //��Сè�����list
    public void chooseCat(Cat cat)
    {
        
        cats.Add(cat);

        Debug.Log("���ѡ����Сè��СèidΪ��" + cat.cat_id.ToString());
    }

    //���ݵ�ǰ����Сè��ÿ��һ��ʱ�������ʯ
    public List<int> spawnLingdan()
    {
        //������ʯ������
        List<int> lingdan = new List<int>() { 0, 0, 0, 0, 0 };
        
        for (int i = 0; i < cats.Count; i++)
        {
            if (cats[i].big_level == "������")
            {
                lingdan[0] += 1;
            }
            else if (cats[i].big_level == "������")
            {
                lingdan[1] += 1;
            }
            else if (cats[i].big_level == "����")
            {
                lingdan[2] += 1;
            }
            else if (cats[i].big_level == "ԪӤ��")
            {
                lingdan[3] += 1;
            }
            else if (cats[i].big_level == "������")
            {
                lingdan[4] += 1;
            }
        }

        return lingdan;
    }
    
    //���ȼ��ַ����Զ�ת��Ϊ����
    public int levelStringToNumber(string level)
    {
        if (level == "������")
        {
            return 0;
        }
        else if (level == "������")
        {
            return 1;
        }
        else if (level == "����")
        {
            return 2;
        }
        else if (level == "ԪӤ��")
        {
            return 3;
        }
        else if (level == "������")
        {
            return 4;
        }

        return 0;
    }

    public string numberToCatLevelString(int level)
    {
        if (level == 0)
        {
            return "������";
        }
        else if (level == 1)
        {
            return "������";
        }
        else if (level == 2)
        {
            return "����";
        }
        else if (level == 3)
        {
            return "ԪӤ��";
        }
        else if (level == 4)
        {
            return "������";
        }

        return "";
    }

    //���ȼ�ת��Ϊ��Ϊ������
    public int levelNeedCul(int small_level, string big_level)
    {
        return (int)(small_level * Mathf.Pow(10, levelStringToNumber(big_level) + 3));
    }


    //�Զ�������ʯ��������Сè��Ϊ�ﵽ����
    public void autoConsumeStone()
    {           
        int secondsDifference = 1; // ����������

        if (!SceneTransferData.instance.isConsumeStone)
        {
            //����ʱ�䵹��ʱ
            DateTime currentTime = DateTime.Now;
            TimeSpan difference = currentTime - StorageController.instance.endTime; // ����ʱ���
            secondsDifference = (int)difference.TotalSeconds; // ����������

            Debug.Log("�����ϴδ򿪵��������Ϊ��" + secondsDifference.ToString());

            SceneTransferData.instance.isConsumeStone = true;
        }

        //����������ʯ
        for (int i = 0; i < cats.Count; i++)
        {
            cats[i].lingshi_consume = (int)(cats[i].small_level * Mathf.Pow(10, levelStringToNumber(cats[i].big_level)));

            //��ʯ�Ƿ��㹻����
            bool enoughStone = cats[i].had_stone >= cats[i].lingshi_consume * secondsDifference;
            
            //�ж�Сè��ǰ��ʯ�����Ƿ��㹻���ģ�������㣬�������Ŀǰ�е�
            if (enoughStone)
            {
                //���ĵ���ʯ�Ƿ񳬹�Сè����Ϊ����
                bool canUp = cats[i].cultivation + cats[i].lingshi_consume * secondsDifference > levelNeedCul(cats[i].small_level, cats[i].big_level);
                //������ʯ�󳬹���Ϊ���ޣ��Ҹ�Сè���Ͻ�������
                if (canUp)
                {
                    cats[i].had_stone -= (int)(levelNeedCul(cats[i].small_level, cats[i].big_level) - cats[i].cultivation);
                    cats[i].cultivation = levelNeedCul(cats[i].small_level, cats[i].big_level);

                    Debug.Log("Сè�ɽ��������Ϊ��" + i);
                }
                else
                {
                    
                    cats[i].had_stone -= cats[i].lingshi_consume * secondsDifference;
                    cats[i].cultivation += cats[i].lingshi_consume * secondsDifference;

                }
                
            }
            else
            {
                //���ĵ���ʯ�Ƿ񳬹�Сè����Ϊ����
                bool canUp = cats[i].cultivation + cats[i].had_stone > levelNeedCul(cats[i].small_level, cats[i].big_level);
                //������ʯ�󳬹���Ϊ���ޣ��Ҹ�Сè���Ͻ�������
                if (canUp)
                {
                    cats[i].had_stone -= (int)(levelNeedCul(cats[i].small_level, cats[i].big_level) - cats[i].cultivation);
                    cats[i].cultivation = levelNeedCul(cats[i].small_level, cats[i].big_level);

                    Debug.Log("Сè�ɽ��������Ϊ��" + i);
                }
                else
                {
                    cats[i].cultivation += cats[i].had_stone;
                    cats[i].had_stone = 0;
                    
                }
                
            }

        }
    }

    //���Сè��ǰ��ߵȼ�
    public int getCatMaxLevel()
    {
        int level = 0;

        for(int i = 0; i < cats.Count; i++)
        {

            if (levelStringToNumber(cats[i].big_level) > level)
            {

                level = levelStringToNumber(cats[i].big_level);
            }
        }

        return level;
    }

}

[Serializable]
public class Cat
{
    public int cat_id;//Сè��id����0��ʼ

    public int cat_icon;//Сèͷ��

    public string cat_name;//Сè����

    public string introuction;//Сè���

    //public string work;//����״̬�������С�̽���С�������

    public string big_level;//�󾳽�
    public int small_level;//С����

    public float cultivation;//��Ϊ

    public bool canUp;//Сè�Ƿ�������

    public int lingshi_consume;//��ʯ�����ٶ�x/s

    public int had_stone;//ӵ�е���ʯ����

    /*
    public static implicit operator GameObject(Cat v)
    {
        throw new NotImplementedException();
    }
    */
}