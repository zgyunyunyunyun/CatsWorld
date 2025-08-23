using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ExpendTerritory : MonoBehaviour
{
    public GameObject content;//存储物体的父节点（scrow的内容节点）
    public GameObject enemyUI;//敌人的数据UI

    public GameObject victoryUI;//胜利UI
    public GameObject defeatedUI;//失败UI

    public int enemyListCount = 12;//敌人列表数=5
    private List<EnemyData> enemyDataList = new List<EnemyData>();
    private List<GameObject> enemyUIList = new List<GameObject>();

    private string[] enemyNameList0 = {"野狼", "黑鹰", "巨虎", "猎豹", "独角犀牛"};
    private string[] enemyNameList1 = {"沙漠土狼", "风豹", "赤金熊","烈风马","三目神猴","幻影狐","九色鹿","烈焰鼠","幼年麒麟"};
    private string[] enemyNameList2 = {"烈焰天麟", "雷兽", "应龙","噬金飞蚁","噬灵虫","巨岩怪","幼年鲲鹏", "幼年青龙"};
    private string[] enemyNameList3 = {"蛟龙", "五彩凤凰", "沙漠巨兽", "独角兽","噬灵虫","鹏鹰","北域金雕", "黑磷巨蟒"};
    private string[] enemyNameList4 = {"穷奇", "纯血青龙", "纯血凤凰", "百兽帝君", "吞月噬魂蜈蚣", "吞天蛤蟆", "龙女", "鲲鹏"};


    private bool hasAttackEnemy = false;//当前是否有来袭的敌人
    private float attackDetectTimer = 5;//检测是否有敌人的计时器
    public float attackTimeGap = 5;

    private EnemyData enemyAttack;//来袭的敌人数据

    public GameObject enemyComingUI;//敌人来袭的提示条
    //public Image enemyImage;//提示条上敌人的头像
    public TMP_Text timeTipText;//倒计时提示
    private float expendRestTimer = 900;//剩余时间——单位：秒。（需要重置）
    private float expendRestGap = 900;

    public GameObject enemyDetailUI;//敌人来袭的提示条
    //public Image enemyPanelImgae;//面板上的敌人提示
    public TMP_Text enemyNameText;//敌人的名称
    public TMP_Text enemyNumberText;//敌人的数量
    public TMP_Text enemyMaxLevelText;//敌人最高境界
    public TMP_Text rewardText;//奖励文本
    public Button attackBtn;//攻击按钮
    public TMP_Text attackBtnText;//攻击按钮文本

    private void Start()
    {
        //生成敌人
        StartCoroutine(RandomSpawnEnemy());

    }

    /// <summary>
    /// 倒计时的计算放在的跨场景的对象里
    /// </summary>

    private void Update()
    {
        //第二次及以后，从其他场景回来以后。如果处于倒计时状态，则刷新数据
        if (SceneTransferData.instance.hasAttackEnemy)
        {
            hasAttackEnemy = SceneTransferData.instance.hasAttackEnemy;
            expendRestTimer = SceneTransferData.instance.expendRestTimer;
        }

        //每个一段时间检测是否生成来袭的敌人
        attackDetectTimer -= Time.deltaTime;
        
        /*
        if(false)
        //if (attackDetectTimer < 0)
        {
            //生成的概率为20%，且当前没有来袭的敌人——生成1个敌人并展示提示条UI
            int x = Random.Range(0, 100);
            if (x > 90 && !hasAttackEnemy)
            {
                hasAttackEnemy = true;
            }

            attackDetectTimer = attackTimeGap;
        }

        //当攻击提示展示时，倒计时开始。倒计时结束，则开始攻击
        //if (hasAttackEnemy && enemyComingUI != null && expendRestTimer >= 0)
        if(false)
        {
            //Debug.Log("来袭的敌人，准备生成敌人");
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
                //攻击——遗留
            }

            //初始化时，把剩余时间传递给场景记录data
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

    //展示开拓领土的任务
    public void ShowTerritoryTask()
    {
        //清空content
        if (content.transform.childCount > 0)
        {
            foreach (Transform child in content.transform)
            {
                Destroy(child.gameObject);
            }

            enemyUIList.Clear();
        }

        Debug.Log("开始展示敌人列表UI，数量为：" + enemyDataList.Count);

        //根据data生成UI
        for (int i=0; i < enemyDataList.Count; i++)
        {

            enemyUIList.Add(SpawnOneEnemyUI(i));

        }

    }

    private GameObject SpawnOneEnemyUI(int i)
    {
        GameObject tempUI = Instantiate(enemyUI);

        //将敌人UI挂着父节点上
        tempUI.transform.SetParent(content.transform, false);

        //改变UI位置
        tempUI.transform.localPosition = new Vector3(500, -160 - 300 * i, 0);
        Debug.Log("新增敌人的position：" + tempUI.transform.localPosition);
        Debug.Log("新增敌人的序号i：" + i);

        //对文字进行赋值
        TMP_Text title = tempUI.transform.Find("Title").GetComponent<TMP_Text>();
        TMP_Text number = tempUI.transform.Find("Number").GetComponent<TMP_Text>();
        TMP_Text maxLevel = tempUI.transform.Find("MaxLevel").GetComponent<TMP_Text>();
        TMP_Text area = tempUI.transform.Find("TerritoryArea").GetComponent<TMP_Text>();

        title.text = enemyDataList[i].name;
        number.text = "敌人：" + enemyDataList[i].number.ToString();
        maxLevel.text = "最大等级：" + CatController.instance.numberToCatLevelString(enemyDataList[i].big_level) + " " + enemyDataList[i].small_level + "层";
        area.text = "领土：" + NumberController.instance.NumberToChinaString((int)enemyDataList[i].area) + " 平方米";

        //对获得的UI进行赋值
        /*Image icon = tempUI.transform.Find("Icon").GetComponent<Image>();
        string path = "Materials/BigCat/cat" + enemyDataList[i].enemyType.ToString();//遗留：待改动
        Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
        icon.sprite = sprite;
        */

        //给按钮绑定事件
        int index = i;
        tempUI.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => ClickSubdueButton(index));

        return tempUI;
    }

    //初始化敌人列表
    IEnumerator RandomSpawnEnemy()
    {
        yield return new WaitForSeconds(0.3f);

        //清空原本的数组
        if (enemyDataList.Count > 0)
        {
            enemyDataList.Clear();
        }

        //随机生成新敌人
        for (int i = 0; i < enemyListCount; i++)
        {
            Debug.Log("随机生成敌人：" + enemyDataList.Count);
            enemyDataList.Add(RandomSpawnOneEnemy());
        }
    }

    //随机生成一个敌人
    private EnemyData RandomSpawnOneEnemy()
    {
        EnemyData enemy = new EnemyData();

        //敌人的最高等级
        int pro = Random.Range(1, 100);
        if (pro >= 90)
        {
            enemy.big_level = CatController.instance.getCatMaxLevel() + 1;
        } else
        {
            enemy.big_level = CatController.instance.getCatMaxLevel();
        }

        //小境界（纯随机）
        enemy.small_level = Random.Range(1, 9);

        //敌人的数量
        int playerCatCount = CatController.instance.cats.Count;
        enemy.number = (Random.Range((int)(playerCatCount * 0.3) + 1, (int)(playerCatCount * 1.2) + 1));

        //敌人的名称
        if (enemy.big_level == 0)
        {
            int eType = Random.Range(0, enemyNameList0.Length - 1);
            enemy.enemyType = eType;
            enemy.name = enemyNameList0[eType];//随机生成敌人名称
        }else if(enemy.big_level == 1)
        {
            int eType = Random.Range(0, enemyNameList1.Length - 1);
            enemy.enemyType = eType;
            enemy.name = enemyNameList1[eType];//随机生成敌人名称
        }
        else if (enemy.big_level == 2)
        {
            int eType = Random.Range(0, enemyNameList2.Length - 1);
            enemy.enemyType = eType;
            enemy.name = enemyNameList2[eType];//随机生成敌人名称
        }
        else if (enemy.big_level == 3)
        {
            int eType = Random.Range(0, enemyNameList3.Length - 1);
            enemy.enemyType = eType;
            enemy.name = enemyNameList3[eType];//随机生成敌人名称
        }
        else if (enemy.big_level == 4)
        {
            int eType = Random.Range(0, enemyNameList4.Length - 1);
            enemy.enemyType = eType;
            enemy.name = enemyNameList4[eType];//随机生成敌人名称
        }


        //奖励：领土面积。与怪物等级、数量有关（主要是为了契合晋级需要的领土面积和小猫的数量）
        float area = Random.Range(enemy.number * enemy.big_level + 3, enemy.number * (enemy.big_level + 1) + 3 );
        enemy.area = area;

        return enemy;
    }


    //点击了收服按钮（收服的位置），触发收服的结果
    private void ClickSubdueButton(int pos)
    {
        Debug.Log("点击收服的按钮顺序为：" + pos);

        StartCoroutine(Battle(enemyDataList[pos], 0, pos));//播放战斗画面

    }

    //播放战斗动画后出结果（敌人，类型）
    //类型：0，列表敌人；1，来袭敌人
    IEnumerator Battle(EnemyData enemy, int type, int pos = -1)
    {
        //播放战斗画面
        if(type == 0)
        {
            enemyUIList[pos].transform.Find("Button").transform.Find("Text (TMP)").GetComponent<TMP_Text>().text = "战斗中";
        }
        else if(type == 1)
        {
            if(attackBtnText != null)
            {
                attackBtnText.text = "战斗中";
            }

        }



        //等待3s执行下一语句
        yield return new WaitForSeconds(0.5f);
        Debug.Log("战斗结束");
        if (attackBtnText != null)
        {
            attackBtnText.text = "攻击";
        }
        //enemyUIList[pos].transform.Find("Button").transform.Find("Text (TMP)").GetComponent<TMP_Text>().text = "收服";


        //触发战斗并返回是否胜利
        bool battleResult = BattleWithEnemy(enemy);

        //胜利 or 失败
        /*胜利：
         * 1、弹出胜利UI，获得奖励
         * 2、隐藏敌人列表UI
         * 3、生成新的敌人数据，添加到列表上；同时展示列表UI
         * 
         * 失败：
         * 1、弹出失败UI，获得惩罚
         * 2、其他不变
         * 
         */

        if (battleResult)
        {
            //展示胜利的UI
            victoryUI.gameObject.SetActive(true);

            float reward = enemy.area;
            victoryUI.transform.Find("Reward").GetComponent<TMP_Text>().text = "领土：+" + NumberController.instance.NumberToChinaString((int)reward) + "平方米";
            PropertyController.instance.territoryArea += reward;

            int hurt = 0;
            for(int i=0;i< CatController.instance.cats.Count; i++)
            {
                //受伤的概率：0.05
                if(Random.Range(0,100) > 95)
                {
                    hurt++;
                    if (CatController.instance.cats[i].small_level > 0)
                    {
                        CatController.instance.cats[i].small_level--;
                    }
                }
            }
            victoryUI.transform.Find("HurtPunishmen").GetComponent<TMP_Text>().text = "受伤小猫：" + hurt.ToString();

            victoryUI.transform.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();

            //绑定按钮事件
            if (type == 0)
            {
                victoryUI.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => ClickConfirmButton(pos));
            }else if(type == 1)
            {
                Debug.Log("来袭小猫，点击了攻击按钮，类型为1的胜利");
                victoryUI.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => ClickConfirmButtonByAttack());
                enemyComingUI.gameObject.SetActive(false);
                enemyDetailUI.gameObject.SetActive(false);
                expendRestTimer = expendRestGap;
                hasAttackEnemy = false;
            }
            
        }
        else
        {
            //展示失败的UI
            defeatedUI.gameObject.SetActive(true);

            int hurt = 0;
            for (int i = 0; i < CatController.instance.cats.Count; i++)
            {
                //受伤的概率：0.5
                if (Random.Range(0, 100) > 50)
                {
                    hurt++;
                    if (CatController.instance.cats[i].small_level > 0)
                    {
                        CatController.instance.cats[i].small_level--;
                    }
                }
            }
            defeatedUI.transform.Find("HurtPunishmen").GetComponent<TMP_Text>().text = "受伤小猫数量：" + hurt.ToString();

        }

    }

    public void ClickConfirmButtonByAttack()
    {
        Debug.Log("来袭小猫，点击胜利的确认按钮");
        victoryUI.gameObject.SetActive(false);
    }

    //点击确认按钮，然后才播放动画
    public void ClickConfirmButton(int pos)
    {
        StartCoroutine(RefreshEnemyData(pos));//播放战斗画面
        victoryUI.transform.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
        victoryUI.gameObject.SetActive(false);
    }

    //协程延时播放动画
    IEnumerator RefreshEnemyData(int pos)
    {
        /*pos输出两遍的原因是，重复绑定了点击事件
         * 
         */
        Debug.Log("输出pos：" + pos);
        yield return new WaitForSeconds(0.08f);
        //清除原来的UI和数据
        Destroy(enemyUIList[pos]);
        enemyUIList.RemoveAt(pos);
        enemyDataList.RemoveAt(pos);

        yield return new WaitForSeconds(0.2f);

        //生成新的UI和数据
        enemyDataList.Insert(pos, RandomSpawnOneEnemy());
        enemyUIList.Insert(pos, SpawnOneEnemyUI(pos));
    }


    //小猫和需要收服的敌人战斗（返回是否胜利）
    private bool BattleWithEnemy(EnemyData enemy)
    {
        int enemyPoints = 0;//敌人的点数
        int playerPoints = 0;//玩家的点数

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

        Debug.Log("敌人点数：" + enemyPoints);
        Debug.Log("玩家点数：" + playerPoints);

        if (enemyPoints >= playerPoints)
        {
            return false;
        }
        else
        {
            return true;
        }       
    }


    //展示即将来袭敌人的详情页UI
    public void ShowAttackEnemyDetail()
    {
        enemyNameText.text = enemyAttack.name;
        enemyNumberText.text = "敌人：" + enemyAttack.number.ToString();
        enemyMaxLevelText.text = "最大等级：" + CatController.instance.numberToCatLevelString(enemyAttack.big_level) + " " + enemyAttack.small_level + "层";
        rewardText.text = "领土：" + NumberController.instance.NumberToChinaString((int)enemyAttack.area) + " 平方米";

    }

    //回击即将来袭的敌人——遗留
    public void AttackComingEnemy()
    {
        if(enemyAttack != null)
        {
            StartCoroutine(Battle(enemyAttack, 1));//播放战斗画面
        }
    }
}

[Serializable]
public class EnemyData
{
    public string name;//名称
    public int enemyType;//敌人类型 
    public int number;//数量
    public int big_level;//大境界
    public int small_level;//小境界
    public float area;//领土面积
}