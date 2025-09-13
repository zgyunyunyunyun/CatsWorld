using Cysharp.Threading.Tasks;
using GameFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

public class CatHeap : MonoBehaviour
{
    [SerializeField] Vector2Int m_RowCol = new Vector2Int(10, 5);
    [SerializeField] Vector2 m_PosPadding = Vector2.one;
    [SerializeField] int m_CombatUnitId = 1;
    /// <summary>
    /// 玩家进入区域内开始刷兵
    /// </summary>
    [SerializeField] Vector2 m_TriggerBounds = Vector2.one;
    [SerializeField] int m_MaxSpawnCountPerFrame = 10;
    int m_SpawnCount;
    Bounds m_SpawnBounds;

    CombatUnitTable m_CombatUnitRow;
    private void Start()
    {

    }

}
