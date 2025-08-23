using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AllCatUI : MonoBehaviour
{
    public GameObject catUI;//��������Сè��prefab
    public GameObject catListUI;//չʾСè�б��UI

    public GameObject content;//�������������

    public GameObject catDetailPanel;//è��������壬��������Ժ������ҳ

    public GameObject catListGreyPanel;//è���б�Ļ�ɫ������

    public TMP_Dropdown dropdown;//����ѡ����

    private List<GameObject> catUIList = new List<GameObject>();//���в���Сè��UI

    private bool isShowAll = true;//�Ƿ���״�չʾȫ��СèUI

    // Start is called before the first frame update
    void Start()
    {
        //�����������ѡ��ı��¼�
        dropdown.onValueChanged.AddListener((value) => spawnCatUI(value));
    }

    private void Update()//������Ŀ����Ϊ���ܹ���СèUI�����򿪵�ʱ�򣬱Ƚ�ʵʱ��ˢ������
    {

        //��Сè�б��UI�����Ǵ򿪵��״�չʾȫ��СèUI
        if (catListUI.activeSelf && isShowAll)
        {
            spawnCatUI(0);//��ʼ��չʾ����Сè

            isShowAll = false;

        }

        if (!catListUI.activeSelf)
        {
            isShowAll = true;
        }
    }


    void spawnCatUI(int condition)
    {
        List<Cat> tempCats = CatController.instance.cats;
        List<Cat> cats = new List<Cat>();

        Debug.Log("ɸѡ����Ϊ��" + condition);

        //��ѡ��Ϊȫ��ʱ��0����չʾ����Сè
        if(condition == 0)
        {
            cats = tempCats;
        }
        //����չʾѡ�е�Сè
        else
        {
            for(int i = 0; i < tempCats.Count; i++)
            {
                if(condition == 1 && tempCats[i].big_level == "������")
                {
                    cats.Add(tempCats[i]);
                }else if (condition == 2 && tempCats[i].big_level == "������")
                {
                    cats.Add(tempCats[i]);
                }
                else if (condition == 3 && tempCats[i].big_level == "����")
                {
                    cats.Add(tempCats[i]);
                }
                else if (condition == 4 && tempCats[i].big_level == "ԪӤ��")
                {
                    cats.Add(tempCats[i]);
                }
                else if (condition == 5 && tempCats[i].big_level == "������")
                {
                    cats.Add(tempCats[i]);
                }
            }
        }

        Debug.Log("ɸѡ�����" + cats.Count);

        Debug.Log("����ǰ��" + catUIList.Count);
        //���Сè��UI������ˢ��ѡ��Ľ��
        for (int i = catUIList.Count - 1; i >= 0; i--)
        {
            GameObject tempObj = catUIList[i];          
            catUIList.RemoveAt(i);
            Destroy(tempObj);
        }

        Debug.Log("��������" + catUIList.Count);

        //��Сè������չʾ��Χ����5��ʱ�������չʾ��content��Χ
        if ((cats.Count / 4.0f) > 4)
        {
            Debug.Log("���Ըı�UI");
            RectTransform contentRect = content.GetComponent<RectTransform>();
            if ((cats.Count / 4.0f) == (cats.Count / 4))
            {
                //contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, 1279.9f + ((cats.Count / 4) - 5) * 260);
                contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1300 + ((cats.Count / 4) - 4) * 270);
                Debug.Log("���Ըı�UI1");
            }
            else
            {
                //contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, 1279.9f + ((cats.Count / 4) - 4) * 260);
                contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1300 + ((cats.Count / 4) - 3) * 270);
                Debug.Log("���Ըı�UI2");
            }
        }

        //��Сè���еȼ�����
        for(int i = cats.Count-1; i > 0 ; i--)
        {
            int maxIndex = i;
            for(int j = i-1; j >= 0; j--)
            {
                if(CatController.instance.levelStringToNumber(cats[j].big_level) > CatController.instance.levelStringToNumber(cats[maxIndex].big_level) 
                    || (CatController.instance.levelStringToNumber(cats[j].big_level) == CatController.instance.levelStringToNumber(cats[maxIndex].big_level) && cats[j].small_level > cats[maxIndex].small_level))
                {
                    maxIndex = j;
                }
            }

            var temp = cats[maxIndex];
            cats.RemoveAt(maxIndex);
            cats.Add(temp);
        }

        //��һ������С�ģ�Ȼ��Ų����һλȥ
        var firstObj = cats[0];
        cats.RemoveAt(0);
        cats.Add(firstObj);

        //չʾ����Сè�������Ӧ��λ��
        for (int i = 0; i< cats.Count; i++)
        {
            GameObject tempObj = Instantiate(catUI);
            catUIList.Add(tempObj);

            //��Сè��ӵ�������¼���������������Ϊɸѡ�����¸���UI
            int catID = cats[i].cat_id;
            tempObj.GetComponent<Button>().onClick.AddListener(() => showCatDetail(catID));

            //��Сè�����б���Ÿ��ڵ���
            tempObj.transform.SetParent(content.transform, false);
            //Debug.Log("%" + i % 4);
            //Debug.Log("/" + i / 4);
            //tempObj.transform.localPosition = new Vector3(95 + (i%4)*220, -100 - 290*(i/4), 0);
            //temp.transform.localPosition = new Vector3(0, 0, 0);

            Image catIcon = tempObj.transform.Find("Image").GetComponent<Image>();
            TMP_Text catName = tempObj.transform.Find("Name").GetComponent<TMP_Text>();
            GameObject redPoint = tempObj.transform.Find("RedPoint").gameObject;

            if (cats[i].canUp)
            {
                redPoint.gameObject.SetActive(true);
            }

            if (cats[i].had_stone <= 0)
            {
                redPoint.gameObject.SetActive(true);
            }

            //�Ի�õ�UI���и�ֵ
            string path = "Materials/BigCat/cat" + cats[i].cat_icon.ToString();
            //Debug.Log("�ɹ�����Сè��Сèͷ�� path:" + path);
            Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
            catIcon.sprite = sprite;

            catName.text = cats[i].cat_name;

        }
        
    }

    //չʾ��nֻСè�����飬��0����
    void showCatDetail(int catID)
    {
        Debug.Log("����˵�" + catID + "ֻè");
        catListGreyPanel.gameObject.SetActive(false);
        catDetailPanel.gameObject.SetActive(true);

        CatDetailController.instance.showCatUI(catID);
    }
}
