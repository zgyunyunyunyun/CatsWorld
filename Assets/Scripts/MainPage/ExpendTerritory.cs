using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ExpendTerritory : MonoBehaviour
{
    public GameObject content;//�洢����ĸ��ڵ㣨scrow�����ݽڵ㣩
    public GameObject enemyUI;//���˵�����UI

    public GameObject victoryUI;//ʤ��UI
    public GameObject defeatedUI;//ʧ��UI

    public int enemyListCount = 12;//�����б���=5
    private List<EnemyData> enemyDataList = new List<EnemyData>();
    private List<GameObject> enemyUIList = new List<GameObject>();

    private string[] enemyNameList0 = {"Ұ��", "��ӥ", "�޻�", "�Ա�", "����Ϭţ"};
    private string[] enemyNameList1 = {"ɳĮ����", "�籪", "�����","�ҷ���","��Ŀ���","��Ӱ��","��ɫ¹","������","��������"};
    private string[] enemyNameList2 = {"��������", "����", "Ӧ��","�ɽ����","�����","���ҹ�","��������", "��������"};
    private string[] enemyNameList3 = {"����", "��ʷ��", "ɳĮ����", "������","�����","��ӥ","������", "���׾���"};
    private string[] enemyNameList4 = {"����", "��Ѫ����", "��Ѫ���", "���޵۾�", "�����ɻ����", "������", "��Ů", "����"};


    private bool hasAttackEnemy = false;//��ǰ�Ƿ�����Ϯ�ĵ���
    private float attackDetectTimer = 5;//����Ƿ��е��˵ļ�ʱ��
    public float attackTimeGap = 5;

    private EnemyData enemyAttack;//��Ϯ�ĵ�������

    public GameObject enemyComingUI;//������Ϯ����ʾ��
    //public Image enemyImage;//��ʾ���ϵ��˵�ͷ��
    public TMP_Text timeTipText;//����ʱ��ʾ
    private float expendRestTimer = 900;//ʣ��ʱ�䡪����λ���롣����Ҫ���ã�
    private float expendRestGap = 900;

    public GameObject enemyDetailUI;//������Ϯ����ʾ��
    //public Image enemyPanelImgae;//����ϵĵ�����ʾ
    public TMP_Text enemyNameText;//���˵�����
    public TMP_Text enemyNumberText;//���˵�����
    public TMP_Text enemyMaxLevelText;//������߾���
    public TMP_Text rewardText;//�����ı�
    public Button attackBtn;//������ť
    public TMP_Text attackBtnText;//������ť�ı�

    private void Start()
    {
        //���ɵ���
        StartCoroutine(RandomSpawnEnemy());

    }

    /// <summary>
    /// ����ʱ�ļ�����ڵĿ糡���Ķ�����
    /// </summary>

    private void Update()
    {
        //�ڶ��μ��Ժ󣬴��������������Ժ�������ڵ���ʱ״̬����ˢ������
        if (SceneTransferData.instance.hasAttackEnemy)
        {
            hasAttackEnemy = SceneTransferData.instance.hasAttackEnemy;
            expendRestTimer = SceneTransferData.instance.expendRestTimer;
        }

        //ÿ��һ��ʱ�����Ƿ�������Ϯ�ĵ���
        attackDetectTimer -= Time.deltaTime;
        
        /*
        if(false)
        //if (attackDetectTimer < 0)
        {
            //���ɵĸ���Ϊ20%���ҵ�ǰû����Ϯ�ĵ��ˡ�������1�����˲�չʾ��ʾ��UI
            int x = Random.Range(0, 100);
            if (x > 90 && !hasAttackEnemy)
            {
                hasAttackEnemy = true;
            }

            attackDetectTimer = attackTimeGap;
        }

        //��������ʾչʾʱ������ʱ��ʼ������ʱ��������ʼ����
        //if (hasAttackEnemy && enemyComingUI != null && expendRestTimer >= 0)
        if(false)
        {
            //Debug.Log("��Ϯ�ĵ��ˣ�׼�����ɵ���");
            enemyAttack = RandomSpawnOneEnemy();
            enemyComingUI.gameObject.SetActive(true);

            attackBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            attackBtn.GetComponent<Button>().onClick.AddListener(() => AttackComingEnemy());

            expendRestTimer -= Time.deltaTime;
            timeTipText.text = ((int)expendRestTimer / 60).ToString() + " : " + ((int)expendRestTimer % 60).ToString();
            if (expendRestTimer <= 0)
            {
                AttackComingEnemy();
                expendRestTimer = attackTimeGap;
                hasAttackEnemy = false;
                //������������
            }

            //��ʼ��ʱ����ʣ��ʱ�䴫�ݸ�������¼data
            if (SceneTransferData.instance.expendRestTimer < 0 || SceneTransferData.instance.expendRestTimer < expendRestTimer)
            {
                SceneTransferData.instance.expendRestTimer = expendRestTimer;
                SceneTransferData.instance.hasAttackEnemy = hasAttackEnemy;
            }
        }
        else
        {
            enemyComingUI.gameObject.SetActive(false);
        }
        */
    }

    //չʾ��������������
    public void ShowTerritoryTask()
    {
        //���content
        if (content.transform.childCount > 0)
        {
            foreach (Transform child in content.transform)
            {
                Destroy(child.gameObject);
            }

            enemyUIList.Clear();
        }

        Debug.Log("��ʼչʾ�����б�UI������Ϊ��" + enemyDataList.Count);

        //����data����UI
        for (int i=0; i < enemyDataList.Count; i++)
        {

            enemyUIList.Add(SpawnOneEnemyUI(i));

        }

    }

    private GameObject SpawnOneEnemyUI(int i)
    {
        GameObject tempUI = Instantiate(enemyUI);

        //������UI���Ÿ��ڵ���
        tempUI.transform.SetParent(content.transform, false);

        //�ı�UIλ��
        tempUI.transform.localPosition = new Vector3(500, -160 - 300 * i, 0);
        Debug.Log("�������˵�position��" + tempUI.transform.localPosition);
        Debug.Log("�������˵����i��" + i);

        //�����ֽ��и�ֵ
        TMP_Text title = tempUI.transform.Find("Title").GetComponent<TMP_Text>();
        TMP_Text number = tempUI.transform.Find("Number").GetComponent<TMP_Text>();
        TMP_Text maxLevel = tempUI.transform.Find("MaxLevel").GetComponent<TMP_Text>();
        TMP_Text area = tempUI.transform.Find("TerritoryArea").GetComponent<TMP_Text>();

        title.text = enemyDataList[i].name;
        number.text = "���ˣ�" + enemyDataList[i].number.ToString();
        maxLevel.text = "���ȼ���" + CatController.instance.numberToCatLevelString(enemyDataList[i].big_level) + " " + enemyDataList[i].small_level + "��";
        area.text = "������" + NumberController.instance.NumberToChinaString((int)enemyDataList[i].area) + " ƽ����";

        //�Ի�õ�UI���и�ֵ
        /*Image icon = tempUI.transform.Find("Icon").GetComponent<Image>();
        string path = "Materials/BigCat/cat" + enemyDataList[i].enemyType.ToString();//���������Ķ�
        Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
        icon.sprite = sprite;
        */

        //����ť���¼�
        int index = i;
        tempUI.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => ClickSubdueButton(index));

        return tempUI;
    }

    //��ʼ�������б�
    IEnumerator RandomSpawnEnemy()
    {
        yield return new WaitForSeconds(0.3f);

        //���ԭ��������
        if (enemyDataList.Count > 0)
        {
            enemyDataList.Clear();
        }

        //��������µ���
        for (int i = 0; i < enemyListCount; i++)
        {
            Debug.Log("������ɵ��ˣ�" + enemyDataList.Count);
            enemyDataList.Add(RandomSpawnOneEnemy());
        }
    }

    //�������һ������
    private EnemyData RandomSpawnOneEnemy()
    {
        EnemyData enemy = new EnemyData();

        //���˵���ߵȼ�
        int pro = Random.Range(1, 100);
        if (pro >= 90)
        {
            enemy.big_level = CatController.instance.getCatMaxLevel() + 1;
        } else
        {
            enemy.big_level = CatController.instance.getCatMaxLevel();
        }

        //С���磨�������
        enemy.small_level = Random.Range(1, 9);

        //���˵�����
        int playerCatCount = CatController.instance.cats.Count;
        enemy.number = (Random.Range((int)(playerCatCount * 0.3) + 1, (int)(playerCatCount * 1.2) + 1));

        //���˵�����
        if (enemy.big_level == 0)
        {
            int eType = Random.Range(0, enemyNameList0.Length - 1);
            enemy.enemyType = eType;
            enemy.name = enemyNameList0[eType];//������ɵ�������
        }else if(enemy.big_level == 1)
        {
            int eType = Random.Range(0, enemyNameList1.Length - 1);
            enemy.enemyType = eType;
            enemy.name = enemyNameList1[eType];//������ɵ�������
        }
        else if (enemy.big_level == 2)
        {
            int eType = Random.Range(0, enemyNameList2.Length - 1);
            enemy.enemyType = eType;
            enemy.name = enemyNameList2[eType];//������ɵ�������
        }
        else if (enemy.big_level == 3)
        {
            int eType = Random.Range(0, enemyNameList3.Length - 1);
            enemy.enemyType = eType;
            enemy.name = enemyNameList3[eType];//������ɵ�������
        }
        else if (enemy.big_level == 4)
        {
            int eType = Random.Range(0, enemyNameList4.Length - 1);
            enemy.enemyType = eType;
            enemy.name = enemyNameList4[eType];//������ɵ�������
        }


        //��������������������ȼ��������йأ���Ҫ��Ϊ�����Ͻ�����Ҫ�����������Сè��������
        float area = Random.Range(enemy.number * enemy.big_level + 3, enemy.number * (enemy.big_level + 1) + 3 );
        enemy.area = area;

        return enemy;
    }


    //������շ���ť���շ���λ�ã��������շ��Ľ��
    private void ClickSubdueButton(int pos)
    {
        Debug.Log("����շ��İ�ť˳��Ϊ��" + pos);

        StartCoroutine(Battle(enemyDataList[pos], 0, pos));//����ս������

    }

    //����ս�����������������ˣ����ͣ�
    //���ͣ�0���б���ˣ�1����Ϯ����
    IEnumerator Battle(EnemyData enemy, int type, int pos = -1)
    {
        //����ս������
        if(type == 0)
        {
            enemyUIList[pos].transform.Find("Button").transform.Find("Text (TMP)").GetComponent<TMP_Text>().text = "ս����";
        }
        else if(type == 1)
        {
            if(attackBtnText != null)
            {
                attackBtnText.text = "ս����";
            }

        }



        //�ȴ�3sִ����һ���
        yield return new WaitForSeconds(0.5f);
        Debug.Log("ս������");
        if (attackBtnText != null)
        {
            attackBtnText.text = "����";
        }
        //enemyUIList[pos].transform.Find("Button").transform.Find("Text (TMP)").GetComponent<TMP_Text>().text = "�շ�";


        //����ս���������Ƿ�ʤ��
        bool battleResult = BattleWithEnemy(enemy);

        //ʤ�� or ʧ��
        /*ʤ����
         * 1������ʤ��UI����ý���
         * 2�����ص����б�UI
         * 3�������µĵ������ݣ���ӵ��б��ϣ�ͬʱչʾ�б�UI
         * 
         * ʧ�ܣ�
         * 1������ʧ��UI����óͷ�
         * 2����������
         * 
         */

        if (battleResult)
        {
            //չʾʤ����UI
            victoryUI.gameObject.SetActive(true);

            float reward = enemy.area;
            victoryUI.transform.Find("Reward").GetComponent<TMP_Text>().text = "������+" + NumberController.instance.NumberToChinaString((int)reward) + "ƽ����";
            PropertyController.instance.territoryArea += reward;

            int hurt = 0;
            for(int i=0;i< CatController.instance.cats.Count; i++)
            {
                //���˵ĸ��ʣ�0.05
                if(Random.Range(0,100) > 95)
                {
                    hurt++;
                    if (CatController.instance.cats[i].small_level > 0)
                    {
                        CatController.instance.cats[i].small_level--;
                    }
                }
            }
            victoryUI.transform.Find("HurtPunishmen").GetComponent<TMP_Text>().text = "����Сè��" + hurt.ToString();

            victoryUI.transform.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();

            //�󶨰�ť�¼�
            if (type == 0)
            {
                victoryUI.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => ClickConfirmButton(pos));
            }else if(type == 1)
            {
                Debug.Log("��ϮСè������˹�����ť������Ϊ1��ʤ��");
                victoryUI.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => ClickConfirmButtonByAttack());
                enemyComingUI.gameObject.SetActive(false);
                enemyDetailUI.gameObject.SetActive(false);
                expendRestTimer = expendRestGap;
                hasAttackEnemy = false;
            }
            
        }
        else
        {
            //չʾʧ�ܵ�UI
            defeatedUI.gameObject.SetActive(true);

            int hurt = 0;
            for (int i = 0; i < CatController.instance.cats.Count; i++)
            {
                //���˵ĸ��ʣ�0.5
                if (Random.Range(0, 100) > 50)
                {
                    hurt++;
                    if (CatController.instance.cats[i].small_level > 0)
                    {
                        CatController.instance.cats[i].small_level--;
                    }
                }
            }
            defeatedUI.transform.Find("HurtPunishmen").GetComponent<TMP_Text>().text = "����Сè������" + hurt.ToString();

        }

    }

    public void ClickConfirmButtonByAttack()
    {
        Debug.Log("��ϮСè�����ʤ����ȷ�ϰ�ť");
        victoryUI.gameObject.SetActive(false);
    }

    //���ȷ�ϰ�ť��Ȼ��Ų��Ŷ���
    public void ClickConfirmButton(int pos)
    {
        StartCoroutine(RefreshEnemyData(pos));//����ս������
        victoryUI.transform.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
        victoryUI.gameObject.SetActive(false);
    }

    //Э����ʱ���Ŷ���
    IEnumerator RefreshEnemyData(int pos)
    {
        /*pos��������ԭ���ǣ��ظ����˵���¼�
         * 
         */
        Debug.Log("���pos��" + pos);
        yield return new WaitForSeconds(0.08f);
        //���ԭ����UI������
        Destroy(enemyUIList[pos]);
        enemyUIList.RemoveAt(pos);
        enemyDataList.RemoveAt(pos);

        yield return new WaitForSeconds(0.2f);

        //�����µ�UI������
        enemyDataList.Insert(pos, RandomSpawnOneEnemy());
        enemyUIList.Insert(pos, SpawnOneEnemyUI(pos));
    }


    //Сè����Ҫ�շ��ĵ���ս���������Ƿ�ʤ����
    private bool BattleWithEnemy(EnemyData enemy)
    {
        int enemyPoints = 0;//���˵ĵ���
        int playerPoints = 0;//��ҵĵ���

        for(int i = 0; i < CatController.instance.cats.Count; i++)
        {
            playerPoints += (int)(Mathf.Pow(10, 1 + (int)(CatController.instance.levelStringToNumber(CatController.instance.cats[i].big_level))) 
                + CatController.instance.cats[i].small_level * 5);
        }

        for(int i=0; i < enemy.number - 1; i++)
        {
            enemyPoints += (int)(Mathf.Pow(10, Random.Range(enemy.big_level/2, enemy.big_level)) + Random.Range(0,10) * 3);
        }

        enemyPoints += (int)(Mathf.Pow(10, 1 + enemy.big_level) + enemy.small_level * 5);

        Debug.Log("���˵�����" + enemyPoints);
        Debug.Log("��ҵ�����" + playerPoints);

        if (enemyPoints >= playerPoints)
        {
            return false;
        }
        else
        {
            return true;
        }       
    }


    //չʾ������Ϯ���˵�����ҳUI
    public void ShowAttackEnemyDetail()
    {
        enemyNameText.text = enemyAttack.name;
        enemyNumberText.text = "���ˣ�" + enemyAttack.number.ToString();
        enemyMaxLevelText.text = "���ȼ���" + CatController.instance.numberToCatLevelString(enemyAttack.big_level) + " " + enemyAttack.small_level + "��";
        rewardText.text = "������" + NumberController.instance.NumberToChinaString((int)enemyAttack.area) + " ƽ����";

    }

    //�ػ�������Ϯ�ĵ��ˡ�������
    public void AttackComingEnemy()
    {
        if(enemyAttack != null)
        {
            StartCoroutine(Battle(enemyAttack, 1));//����ս������
        }
    }
}

[Serializable]
public class EnemyData
{
    public string name;//����
    public int enemyType;//�������� 
    public int number;//����
    public int big_level;//�󾳽�
    public int small_level;//С����
    public float area;//�������
}