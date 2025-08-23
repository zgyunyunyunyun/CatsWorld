using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AllCatUI : MonoBehaviour
{
    public GameObject catUI;//用来生成小猫的prefab
    public GameObject catListUI;//展示小猫列表的UI

    public GameObject content;//挂在内容组件上

    public GameObject catDetailPanel;//猫咪详情面板，用来点击以后打开详情页

    public GameObject catListGreyPanel;//猫咪列表的灰色背景板

    public TMP_Dropdown dropdown;//下拉选项条

    private List<GameObject> catUIList = new List<GameObject>();//所有产生小猫的UI

    private bool isShowAll = true;//是否打开首次展示全部小猫UI

    // Start is called before the first frame update
    void Start()
    {
        //给下拉条添加选项改变事件
        dropdown.onValueChanged.AddListener((value) => spawnCatUI(value));
    }

    private void Update()//遗留：目的是为了能够让小猫UI反复打开的时候，比较实时地刷新数据
    {

        //打开小猫列表的UI，且是打开的首次展示全部小猫UI
        if (catListUI.activeSelf && isShowAll)
        {
            spawnCatUI(0);//初始化展示所有小猫

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

        Debug.Log("筛选条件为：" + condition);

        //当选项为全部时（0），展示所有小猫
        if(condition == 0)
        {
            cats = tempCats;
        }
        //否则，展示选中的小猫
        else
        {
            for(int i = 0; i < tempCats.Count; i++)
            {
                if(condition == 1 && tempCats[i].big_level == "练气期")
                {
                    cats.Add(tempCats[i]);
                }else if (condition == 2 && tempCats[i].big_level == "筑基期")
                {
                    cats.Add(tempCats[i]);
                }
                else if (condition == 3 && tempCats[i].big_level == "金丹期")
                {
                    cats.Add(tempCats[i]);
                }
                else if (condition == 4 && tempCats[i].big_level == "元婴期")
                {
                    cats.Add(tempCats[i]);
                }
                else if (condition == 5 && tempCats[i].big_level == "化神期")
                {
                    cats.Add(tempCats[i]);
                }
            }
        }

        Debug.Log("筛选结果：" + cats.Count);

        Debug.Log("清理前：" + catUIList.Count);
        //清空小猫的UI，用于刷新选择的结果
        for (int i = catUIList.Count - 1; i >= 0; i--)
        {
            GameObject tempObj = catUIList[i];          
            catUIList.RemoveAt(i);
            Destroy(tempObj);
        }

        Debug.Log("清理结果：" + catUIList.Count);

        //当小猫的数量展示范围超过5行时，扩大可展示的content范围
        if ((cats.Count / 4.0f) > 4)
        {
            Debug.Log("尝试改变UI");
            RectTransform contentRect = content.GetComponent<RectTransform>();
            if ((cats.Count / 4.0f) == (cats.Count / 4))
            {
                //contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, 1279.9f + ((cats.Count / 4) - 5) * 260);
                contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1300 + ((cats.Count / 4) - 4) * 270);
                Debug.Log("尝试改变UI1");
            }
            else
            {
                //contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, 1279.9f + ((cats.Count / 4) - 4) * 260);
                contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1300 + ((cats.Count / 4) - 3) * 270);
                Debug.Log("尝试改变UI2");
            }
        }

        //给小猫进行等级排序
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

        //第一个是最小的，然后挪到第一位去
        var firstObj = cats[0];
        cats.RemoveAt(0);
        cats.Add(firstObj);

        //展示所有小猫并处理对应的位置
        for (int i = 0; i< cats.Count; i++)
        {
            GameObject tempObj = Instantiate(catUI);
            catUIList.Add(tempObj);

            //给小猫添加点击监听事件，用在这里是因为筛选会重新更新UI
            int catID = cats[i].cat_id;
            tempObj.GetComponent<Button>().onClick.AddListener(() => showCatDetail(catID));

            //将小猫属性列表挂着父节点上
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

            //对获得的UI进行赋值
            string path = "Materials/BigCat/cat" + cats[i].cat_icon.ToString();
            //Debug.Log("成功诞生小猫，小猫头像 path:" + path);
            Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
            catIcon.sprite = sprite;

            catName.text = cats[i].cat_name;

        }
        
    }

    //展示第n只小猫的详情，从0算起
    void showCatDetail(int catID)
    {
        Debug.Log("点击了第" + catID + "只猫");
        catListGreyPanel.gameObject.SetActive(false);
        catDetailPanel.gameObject.SetActive(true);

        CatDetailController.instance.showCatUI(catID);
    }
}
