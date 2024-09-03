using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LingdanCat : MonoBehaviour
{
    public bool moveToFur = false;//�Ƿ��ƶ�����¯
    public bool moveToStay = false;//�Ƿ��ƶ�����¯
    public Vector3 target;//�ƶ���Ŀ��
    public float speed;//�ƶ����ٶ�

    private int cType;//Сè����
    private int fSite;//Ŀ�ĵ�¯λ��
    private GameObject furnace;//Ŀ�ĵ�¯λ��
    private int cSite;//Ŀ��Сè��λ
    private int catNumber;//Сè�������б��е�λ��

    private int sNumber;//Сè��stayBar��λ��

    private Vector3 lastPosition;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;

        speed = 85.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //��������������ƶ�Сè
        if (moveToStay || moveToFur)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);


            //transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);

            //transform.Find("Particle").position = transform.position;
            
            if (lastPosition == transform.position && moveToFur)
            {
                //Debug.Log("Сè��ֹ�����ˣ��ұ��������Ϊ��" + catNumber);

                //�滻��¯�ϵ�Сèui
                
                if (TaskController.instance.taskList.Count >= 1)
                {
                    if (TaskController.instance.taskList.Count == 1)
                    {
                        fSite = 0;
                    }
                    Debug.Log("�����¯�����Ϊ��" + fSite);
                    GameObject tempTask = furnace;//TaskController.instance.taskList[fSite];
                    Image catIcon = tempTask.transform.Find("MainObject").Find("CatPos" + cSite.ToString()).GetComponent<Image>();
                    string path = "Materials/Cat/cat" + cType.ToString();
                    Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
                    catIcon.sprite = sprite;

                    Color color = catIcon.color; // ��ȡ��ǰ��ɫ
                    color.a = 1.0f; // ����͸����
                    catIcon.color = color; // Ӧ������ɫ

                    gameObject.SetActive(false);
                }


            }

            if (lastPosition == transform.position && moveToStay)
            {
                //Debug.Log("Сè��ֹ�����ˣ���ͣ����stayBar");

            }

            lastPosition = transform.position;

        }
    }

    public void moveCat(Vector3 t, int type, int furnaceSite, GameObject furnaceObj, int catSite, int cNumber)
    {
        Debug.Log("Сè���ƶ�����¯�ϣ�Сè���Ϊ��" + cNumber);
        Debug.Log("Сè���ƶ�����¯�ϣ�Сè����Ϊ��" + type);
        //Debug.Log("Сè���ƶ�����¯�ϣ���¯���Ϊ��" + furnaceSite);
       // //Debug.Log("Сè���ƶ����ж��������ĵ�¯�ϣ���¯���Ϊ��" + furnaceSite);

        target = t;
        moveToFur = true;
        cType = type;
        fSite = furnaceSite;
        furnace = furnaceObj;
        cSite = catSite;
        catNumber = cNumber;

        //transform.Find("Particle").gameObject.SetActive(true);
        //TaskController.instance.taskDataList[furnaceSite].cats.Add(LingDanCatController.instance.catList[cNumber]);
        TaskController.instance.taskDataList[furnaceSite].cats.Add(gameObject);
        //furnace.cats.Add(gameObject);

        //TaskController.instance.addTaskCat(furnaceSite, catNumber, cSite);
    }

    public void moveCatToStay(Vector3 t, int type, int stayNumber, int cNumber)
    {
        Debug.Log("Сè���ƶ���stayBar�ϣ�Сè���Ϊ��" + cNumber);
        Debug.Log("Сè���ƶ���stayBar�ϣ�Сè����Ϊ��" + type);
        target = t;
        moveToStay = true;
        cType = type;
        sNumber = stayNumber;
        catNumber = cNumber;

        //transform.Find("Particle").gameObject.SetActive(true);

        LingDanCatController.instance.addStayCat(cType, sNumber, catNumber);
    }

}
