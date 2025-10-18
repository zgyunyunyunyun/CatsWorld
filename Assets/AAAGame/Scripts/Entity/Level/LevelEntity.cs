using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 通过控制每层的卡片种类来实现难度控制的关卡（卡片堆最多为5*5）
/// </summary>
public class LevelEntity : LevelEntityBase<SlotEntityBase, FishPoolEntityBase>
{
    private List<int> layerCount = new(); // 每层的卡片数量
    private List<int> layerCatTypeCount = new(); // 每层的卡片种类数量

    private Dictionary<int, int> layerRemainCatTypeCount = new(); // 记录上一层需要补充的卡片种类及对应数量 
    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
    }
    protected override void OnShow(object userData)
    {
        base.OnShow(userData);
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);
    }
    protected override void OnHide(bool isShutdown, object userData)
    {
        base.OnHide(isShutdown, userData);
    }

    protected override bool InitLevelData()
    {
        m_SlotCount = levelTable.SlotCount;

        var layerIds = levelTable.Layers;
        // 获取层配置
        var layers = GF.DataTable.GetDataTable<LayerTable>();
        for (int i = 0; i < layerIds.Length; i++)
        {
            layerConfigs.Add(layers.GetDataRow(layerIds[i]));
        }

        layerCount.Clear();
        layerCatTypeCount.Clear();
        for (int i = 0; i < layerConfigs.Count; i++)
        {
            layerCount.Add(layerConfigs[i].PosArr.Length);
        }

        layerCatTypeCount = levelTable.CatTypes.ToList();

        return true;
    }

    // 按照层配置构建随机猫咪牌组
    protected override List<int> BuildShuffledDeck(List<CatData> catTypes)
    {
        List<int> deck = new();

        for (int i = 0; i < layerCount.Count; i++)
        {
            int count = layerCount[i];
            int typeCount = layerCatTypeCount[i];

            // 在可用的猫种类中，随机选择typeCount种类，确保不重复
            List<CatData> layerCats = new();
            HashSet<int> selectedIndices = new();
            HashSet<int> lastLayerSelectedIndices = new(); // 记录上一层遗留下来被下一层选中的卡片种类
            List<CatData> currentLayerRandomAddCats = new(); // 除去上一层遗留下来的卡片种类后，当前层随机添加的卡片种类
            // 先把上一层剩余的卡片种类添加进来
            foreach (var kvp in layerRemainCatTypeCount)
            {
                if (selectedIndices.Count <= typeCount)
                {
                    CatData cat = catTypes.First(c => c.id == kvp.Key);
                    layerCats.Add(cat);
                    lastLayerSelectedIndices.Add(cat.id);
                    selectedIndices.Add(cat.id);
                }
            }
            // 再随机选择剩余的卡片种类
            while (selectedIndices.Count < typeCount)
            {
                int randomIndex = Random.Range(0, catTypes.Count);
                if (!selectedIndices.Contains(catTypes[randomIndex].id))
                {
                    selectedIndices.Add(catTypes[randomIndex].id);
                    currentLayerRandomAddCats.Add(catTypes[randomIndex]);
                }
            }
            layerCats.AddRange(currentLayerRandomAddCats);

            // 根据每层的卡片数量和种类数量，生成该层的卡片列表
            List<int> layerDeck = new();

            // 如果上一层有剩余的卡片种类，则优先补充这些种类数量达到3，但layerDeck数量不能超过count
            foreach (var kvp in layerRemainCatTypeCount)
            {
                if (lastLayerSelectedIndices.Contains(kvp.Key))
                {
                    // 如果上一层剩余的卡片种类在当前层被选中了，则优先补充这些种类数量
                    for (int j = 0; j < kvp.Value; j++)
                    {
                        layerDeck.Add(kvp.Key);
                    }
                }
            }

            // 清空上一层剩余的卡片种类记录
            foreach (var kvp in lastLayerSelectedIndices)
            {
                if (layerRemainCatTypeCount.ContainsKey(kvp))
                {
                    layerRemainCatTypeCount.Remove(kvp);
                }
            }

            int layerRemainingCount = count - layerDeck.Count;

            // 如果剩余卡片数量小于等于0，说明当前层因为补充上一层剩余卡片种类数量已经满足要求
            if (layerRemainingCount <= 0)
            {
                // 将超出的卡片数量移出，记录在layerRemainCatTypeCount中
                int excessCount = -layerRemainingCount;
                for (int j = layerDeck.Count - 1; j >= 0 && excessCount > 0; j--)
                {
                    int catId = layerDeck[j];
                    layerDeck.RemoveAt(j);

                    if (layerRemainCatTypeCount.ContainsKey(catId))
                    {
                        layerRemainCatTypeCount[catId]++;
                    }
                    else
                    {
                        layerRemainCatTypeCount[catId] = 1;
                    }
                    excessCount--;
                }

                // 需要在当前层展示，但因为卡片数量关系没有展示的种类，也需要记录在layerRemainCatTypeCount中
                foreach (var cat in layerCats)
                {
                    if (!layerDeck.Contains(cat.id))
                    {
                        if (layerRemainCatTypeCount.ContainsKey(cat.id))
                        {
                            layerRemainCatTypeCount[cat.id] += 3;
                        }
                        else
                        {
                            layerRemainCatTypeCount[cat.id] = 3;
                        }
                    }
                }
            }
            // 如果卡片数量大于等于种类数量*3且卡片数量为3的倍数，则该层满足种类分布，进行下一层的计算
            else if (layerRemainingCount >= typeCount * 3 && layerRemainingCount % 3 == 0)
            {
                List<int> additionalDeck = base.BuildCatsDeckBy3(layerRemainingCount, layerCats);
                layerDeck.AddRange(additionalDeck);
            }
            // 如果卡片数量大于种类数量*3但不为3的倍数，则先添加完整的种类分布，再随机添加剩余的卡片
            else if (layerRemainingCount > typeCount * 3)
            {
                int remainder = layerRemainingCount % (typeCount * 3);
                int fullSets = layerRemainingCount - remainder;
                layerDeck.AddRange(base.BuildCatsDeckBy3(fullSets, layerCats));

                // 添加剩余的卡片，尽量保持种类分布
                List<int> remainderCats = new();
                for (int j = 0; j < remainder; j++)
                {
                    int randomCatIndex = Random.Range(0, layerCats.Count);
                    remainderCats.Add(layerCats[randomCatIndex].id);
                }
                // 根据当前层剩余的卡片种类数量，计算并记录用于下一层补充对应的卡片形成3的倍数
                Dictionary<int, int> tempRemainCount = new();
                foreach (var catId in remainderCats)
                {
                    if (tempRemainCount.ContainsKey(catId))
                    {
                        tempRemainCount[catId]++;
                    }
                    else
                    {
                        tempRemainCount[catId] = 1;
                    }
                }
                foreach (var kvp in tempRemainCount)
                {
                    if (layerRemainCatTypeCount.ContainsKey(kvp.Key))
                    {
                        layerRemainCatTypeCount[kvp.Key] += 3 - (kvp.Value % 3);
                    }
                    else
                    {
                        layerRemainCatTypeCount[kvp.Key] = 3 - (kvp.Value % 3);
                    }
                }

                layerDeck.AddRange(remainderCats);
            }
            // 如果卡片数量小于种类数量*3
            else
            {
                // 方案一，随机添加卡片，尽量保持种类分布

                // // 计算每种卡片的基础数量
                // int baseCount = count / typeCount;
                // int extraCards = count % typeCount;

                // // 添加基础数量的卡片
                // foreach (var cat in layerCats)
                // {
                //     for (int j = 0; j < baseCount; j++)
                //     {
                //         layerDeck.Add(cat.id);
                //     }
                // }

                // // 随机分配剩余的卡片
                // List<int> extraCatIds = new();
                // for (int j = 0; j < extraCards; j++)
                // {
                //     int randomCatIndex = Random.Range(0, layerCats.Count);
                //     extraCatIds.Add(layerCats[randomCatIndex].id);
                // }
                // layerDeck.AddRange(extraCatIds);

                // // 剔除掉当前层已经是3的倍数的卡片种类，记录当前层剩余的卡片种类数量，用于下一层补充对应的卡片形成3的倍数
                // foreach (var cat in layerCats)
                // {
                //     int catTypeCount = layerDeck.Count(x => x == cat.id);

                //     if (catTypeCount == 0)
                //     {
                //         layerRemainCatTypeCount[cat.id] = 0;
                //     }
                //     if (catTypeCount % 3 != 0)
                //     {
                //         layerRemainCatTypeCount[cat.id] = catTypeCount % 3;
                //     }
                // }

                // 方案二，按照种类顺序，每种卡片生成3个，不足3个的先记录下来，在下一层补充
                int full3TypeCount = layerRemainingCount / 3; // 满足3个的种类数量
                int notFull3TypeCount = typeCount - full3TypeCount; // 不足3个的种类数量
                int remainder = layerRemainingCount % 3; // 剩余数量
                for (int j = 0; j < full3TypeCount; j++)
                {
                    layerDeck.AddRange(Enumerable.Repeat(currentLayerRandomAddCats[j].id, 3));
                }
                if (remainder > 0)
                {
                    layerDeck.AddRange(Enumerable.Repeat(layerCats[full3TypeCount].id, remainder));

                    if (layerRemainCatTypeCount.ContainsKey(layerCats[full3TypeCount].id))
                    {
                        layerRemainCatTypeCount[layerCats[full3TypeCount].id] += 3 - remainder;
                    }
                    else
                    {
                        layerRemainCatTypeCount[layerCats[full3TypeCount].id] = 3 - remainder;
                    }
                }
                for (int j = full3TypeCount + 1; j < typeCount; j++)
                {
                    // 记录当前层剩余的卡片种类数量，用于下一层补充对应的卡片形成3的倍数
                    if (layerRemainCatTypeCount.ContainsKey(layerCats[j].id))
                    {
                        layerRemainCatTypeCount[layerCats[j].id] += 3;
                    }
                    else
                    {
                        layerRemainCatTypeCount[layerCats[j].id] = 3;
                    }
                }
            }

            // 打乱当前层的卡片顺序
            Shuffle(layerDeck);

            // 如果当前层为最后一层且layerRemainCatTypeCount不为空，则需要把剩余的卡片种类补齐3的倍数
            if (i == layerCount.Count - 1 && layerRemainCatTypeCount.Count > 0)
            {
                List<int> addCards = new();
                foreach (var kvp in layerRemainCatTypeCount)
                {
                    int needCount = kvp.Value;
                    for (int j = 0; j < needCount; j++)
                    {
                        addCards.Add(kvp.Key);
                    }
                }
                // 打乱当前层的卡片顺序
                Shuffle(addCards);

                layerDeck.AddRange(addCards);
            }

            deck.AddRange(layerDeck);
        }
        return deck;
    }

    // 按层摆放猫咪
    protected override async Task<int> PlaceCatsByLayer(List<CatData> catTypes, List<int> deck)
    {
        int deckIndex = await base.PlaceCatsByLayer(catTypes, deck);

        // 如果还有剩余的卡片没有摆放，说明构建牌组时补充了卡片，需要再添加n层摆放卡片，添加的层使用倒序配置
        if (deckIndex < deck.Count)
        {
            List<LayerTable> reversedLayerConfigs = layerConfigs.Reverse<LayerTable>().ToList();
            for (int layerIdx = 0; layerIdx < reversedLayerConfigs.Count; layerIdx++)
            {
                LayerTable layerConfig = reversedLayerConfigs[layerIdx];
                Vector3[] layerCatPosArr = layerConfig.PosArr; // 每层猫咪的位置

                for (int rowIdx = 0; rowIdx < layerCatPosArr.Length; rowIdx++)
                {
                    if (deckIndex >= deck.Count) break;
                    await CreateCatEntity(catTypes, deck[deckIndex], layerConfigs.Count + layerIdx, layerCatPosArr[rowIdx]);
                    deckIndex += 1; // 更新索引
                }

                await Task.Yield(); // 创建一层后稍微等待，避免卡顿
            }
        }

        return deckIndex;
    }

}
