using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameFramework.Event;
using UnityGameFramework.Runtime;

/// <summary>
/// 将所有子对象视为一个整体居中，不改变子对象之间的相对位置
/// 注意：此脚本会重新调整子对象位置，请勿与需要特定布局的对象同时使用
/// </summary>
[ExecuteAlways]
public class ItemLayoutFormat : MonoBehaviour
{
    [Header("Layout Settings")]
    [Tooltip("警告：启用自动更新可能会干扰其他脚本的布局")]
    public bool autoUpdate = false; // 默认关闭自动更新，避免干扰其他脚本的布局
    public bool includeInactive = false;  // 是否包含未激活的子对象
    [Tooltip("是否在启用脚本时执行一次居中")]
    public bool centerOnEnable = true;  // 是否在对象启用时执行一次居中
    [Tooltip("设置为true时，脚本将在完成居中后禁用自身")]
    public bool disableAfterCentering = true;  // 居中后是否禁用此脚本
    [Tooltip("等待子对象创建完成的时间（秒）")]
    public float waitForChildrenTime = 1.0f;  // 等待子对象创建完成的时间

    [Header("GameFramework Integration")]
    [Tooltip("是否监听GameFramework的实体创建事件")]
    public bool listenToEntityEvents = true;  // 是否监听GameFramework的实体创建事件
    [Tooltip("等待多少个子实体创建完成后执行居中")]
    public int expectedChildCount = 5;  // 预期的子对象数量
    [Tooltip("超时时间（秒），超过此时间无论如何都会执行居中")]
    public float timeout = 5.0f;  // 超时时间，单位秒

    [Header("Exclusion Settings")]
    [Tooltip("是否排除包含特定名称的子对象")]
    public bool excludeByName = true;  // 是否根据名称排除对象
    [Tooltip("要排除的对象名称，如果子对象名称包含这些字符串，则不会被居中")]
    public string[] excludeNameContains = new string[] { };  // 要排除的对象名称

    private bool hasPerformedInitialCentering = false;
    private List<int> loadedEntityIds = new List<int>();
    private float centeringStartTime = 0f;

    void Awake()
    {
        // 设置默认排除项，确保不会干扰猫咪布局
        if (excludeNameContains == null || excludeNameContains.Length == 0)
        {
            excludeNameContains = new string[] { };
        }

        Debug.Log($"ItemLayoutFormat Awake: {gameObject.name}，路径: {GetFullPath(transform)}");
    }

    // 获取对象的完整路径，便于调试
    private string GetFullPath(Transform obj)
    {
        string path = obj.name;
        while (obj.parent != null)
        {
            obj = obj.parent;
            path = obj.name + "/" + path;
        }
        return path;
    }

    void OnEnable()
    {
        Debug.Log($"ItemLayoutFormat OnEnable: {gameObject.name}, 已执行过居中={hasPerformedInitialCentering}");

        // 监听GameFramework的实体创建事件
        if (listenToEntityEvents && Application.isPlaying)
        {
            GF.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            Debug.Log($"ItemLayoutFormat: {gameObject.name} 已注册GameFramework实体创建事件监听");
        }

        if (centerOnEnable && !hasPerformedInitialCentering)
        {
            // 启动等待子对象创建的协程
            StartCoroutine(WaitForChildrenAndCenter());
        }
    }

    void OnDisable()
    {
        // 取消事件监听
        if (listenToEntityEvents && Application.isPlaying)
        {
            GF.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            Debug.Log($"ItemLayoutFormat: {gameObject.name} 已取消GameFramework实体创建事件监听");
        }
    }

    // 处理GameFramework的实体创建成功事件
    private void OnShowEntitySuccess(object sender, GameEventArgs e)
    {
        var args = e as ShowEntitySuccessEventArgs;
        if (args == null) return;

        // 检查创建的实体是否是我们感兴趣的子对象
        if (args.Entity.transform.parent == transform)
        {
            int entityId = args.Entity.Id;
            loadedEntityIds.Add(entityId);
            Debug.Log($"ItemLayoutFormat: {gameObject.name} 检测到子实体创建 ID={entityId}, 名称={args.Entity.gameObject.name}, 当前已加载 {loadedEntityIds.Count}/{expectedChildCount}");

            // 如果已经达到预期的子对象数量，执行居中
            if (loadedEntityIds.Count >= expectedChildCount && !hasPerformedInitialCentering)
            {
                Debug.Log($"ItemLayoutFormat: {gameObject.name} 已达到预期子实体数量，执行居中");
                CenterAllChildren();
                hasPerformedInitialCentering = true;

                if (disableAfterCentering)
                {
                    enabled = false;
                    Debug.Log($"ItemLayoutFormat: {gameObject.name} 已完成居中操作，脚本已禁用");
                }
            }
        }
    }

    // 等待子对象创建完成后执行居中
    private IEnumerator WaitForChildrenAndCenter()
    {
        Debug.Log($"ItemLayoutFormat: {gameObject.name} 开始等待子对象创建完成...");
        centeringStartTime = Time.time;
        loadedEntityIds.Clear();

        // 先等待指定时间，让子对象有时间创建
        yield return new WaitForSeconds(waitForChildrenTime);

        // 如果通过监听事件已经执行过居中，则不再执行
        if (hasPerformedInitialCentering)
        {
            Debug.Log($"ItemLayoutFormat: {gameObject.name} 通过事件监听已执行过居中，不再重复执行");
            yield break;
        }

        // 检查是否达到超时时间
        while (Time.time - centeringStartTime < timeout && loadedEntityIds.Count < expectedChildCount)
        {
            yield return new WaitForSeconds(0.5f);
            Debug.Log($"ItemLayoutFormat: {gameObject.name} 等待子对象创建中... 已加载{loadedEntityIds.Count}/{expectedChildCount}, 已等待{Time.time - centeringStartTime}/{timeout}秒");
        }

        // 无论如何，超时后都执行居中
        if (!hasPerformedInitialCentering)
        {
            Debug.Log($"ItemLayoutFormat: {gameObject.name} 等待完成或超时，执行居中。已加载子实体:{loadedEntityIds.Count}/{expectedChildCount}");
            CenterAllChildren();
            hasPerformedInitialCentering = true;

            if (disableAfterCentering)
            {
                enabled = false;
                Debug.Log($"ItemLayoutFormat: {gameObject.name} 已完成居中操作，脚本已禁用");
            }
        }
    }

    void Start()
    {
        // 记录开始时布局的情况
        Debug.Log($"ItemLayoutFormat Start: {gameObject.name}, autoUpdate={autoUpdate}, centerOnEnable={centerOnEnable}, excludeByName={excludeByName}, 排除项数量={excludeNameContains.Length}");
    }
    void Update()
    {
        if (autoUpdate)
        {
            CenterAllChildren();
        }
    }

    [ContextMenu("Center All Children Now")]
    public void CenterAllChildrenFromMenu()
    {
        Debug.Log($"ItemLayoutFormat: {gameObject.name} 通过菜单手动执行居中操作");
        CenterAllChildren();
    }

    [ContextMenu("Force Center (Ignore Exclusions)")]
    public void ForceCenterIgnoreExclusions()
    {
        Debug.Log($"ItemLayoutFormat: {gameObject.name} 强制执行居中，忽略所有排除规则");
        CenterAllChildrenForce();
    }

    // 强制居中所有子对象，忽略排除规则
    public void CenterAllChildrenForce()
    {
        // 无子对象时不处理
        if (transform.childCount == 0)
        {
            Debug.Log($"ItemLayoutFormat: {gameObject.name} 没有子对象，不进行强制居中");
            return;
        }

        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (includeInactive || child.gameObject.activeInHierarchy)
            {
                children.Add(child);
            }
        }

        if (children.Count == 0)
        {
            Debug.Log($"ItemLayoutFormat: {gameObject.name} 没有活动的子对象，不进行强制居中");
            return;
        }

        // 计算所有子对象的边界
        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        foreach (Transform child in children)
        {
            // 如果子对象有渲染器组件，使用渲染器的边界
            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null)
            {
                Bounds bounds = renderer.bounds;
                min = Vector3.Min(min, bounds.min);
                max = Vector3.Max(max, bounds.max);
            }
            else
            {
                // 否则使用子对象的位置
                min = Vector3.Min(min, child.position);
                max = Vector3.Max(max, child.position);
            }
        }

        // 计算整体边界的中心点
        Vector3 center = (min + max) / 2;

        // 计算需要移动的偏移量（从当前中心到父对象原点）
        Vector3 offset = transform.position - center;

        // 移动所有子对象，使整体居中
        foreach (Transform child in children)
        {
            child.position += offset;
        }

        Debug.Log($"ItemLayoutFormat: {gameObject.name} 已强制居中所有 {children.Count} 个子对象");
    }

    public void CenterAllChildren()
    {
        // 无子对象时不处理
        if (transform.childCount == 0)
        {
            Debug.Log($"ItemLayoutFormat: {gameObject.name} 没有子对象，不进行居中操作");
            return;
        }

        // 收集有效的子对象
        List<Transform> children = new List<Transform>();
        List<string> excludedNames = new List<string>(); // 用于调试，记录被排除的对象

        foreach (Transform child in transform)
        {
            bool excluded = false;

            // 跳过有LevelEntity脚本的对象，避免干扰猫咪布局
            if (child.GetComponent<LevelEntity>() != null)
            {
                excludedNames.Add($"{child.name}(LevelEntity)");
                excluded = true;
                continue;
            }

            // // 检查是否有CatEntity子组件
            // if (child.GetComponentInChildren<CatEntity>(true) != null)
            // {
            //     excludedNames.Add($"{child.name}(含CatEntity子对象)");
            //     excluded = true;
            //     continue;
            // }

            // 根据名称排除对象
            if (excludeByName && ShouldExcludeByName(child.name))
            {
                excludedNames.Add($"{child.name}(名称排除)");
                excluded = true;
                continue;
            }

            if (!excluded && (includeInactive || child.gameObject.activeInHierarchy))
            {
                children.Add(child);
            }
            else if (!excluded)
            {
                excludedNames.Add($"{child.name}(未激活)");
            }
        }

        if (children.Count == 0)
        {
            Debug.Log($"ItemLayoutFormat: {gameObject.name} 没有符合条件的子对象，不进行居中操作。被排除的对象: {string.Join(", ", excludedNames)}");
            return;
        }        // 计算所有子对象的边界
        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        foreach (Transform child in children)
        {
            // 如果子对象有渲染器组件，使用渲染器的边界
            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null)
            {
                Bounds bounds = renderer.bounds;
                min = Vector3.Min(min, bounds.min);
                max = Vector3.Max(max, bounds.max);
            }
            else
            {
                // 否则使用子对象的位置
                min = Vector3.Min(min, child.position);
                max = Vector3.Max(max, child.position);
            }
        }

        // 计算整体边界的中心点
        Vector3 center = (min + max) / 2;

        // 计算需要移动的偏移量（从当前中心到父对象原点）
        Vector3 offset = transform.position - center;

        // 移动所有子对象，使整体居中
        foreach (Transform child in children)
        {
            child.position += offset;
        }

        // 完成居中后在控制台输出信息
        Debug.Log($"ItemLayoutFormat: 已完成对象居中操作，处理了 {children.Count} 个子对象，排除了 {excludedNames.Count} 个对象");

        // 输出每个被处理的子对象名称，帮助调试
        if (children.Count > 0)
        {
            Debug.Log($"处理的对象: {string.Join(", ", children.Select(c => c.name))}");
        }
    }    // 检查对象名称是否应该被排除
    private bool ShouldExcludeByName(string name)
    {
        foreach (string excludeName in excludeNameContains)
        {
            if (!string.IsNullOrEmpty(excludeName) && name.Contains(excludeName))
            {
                return true;
            }
        }
        return false;
    }
}
