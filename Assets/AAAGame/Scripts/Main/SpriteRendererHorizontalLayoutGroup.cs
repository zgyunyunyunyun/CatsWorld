using UnityEngine;

/// <summary>
/// SpriteRenderer版 Horizontal Layout Group
/// 支持在固定宽度内根据子元素数量自适应排列
/// </summary>
[ExecuteAlways]
public class SpriteRendererHorizontalLayoutGroup : MonoBehaviour
{
    public enum Alignment
    {
        Left,
        Center,
        Right
    }

    [Header("Layout Settings")]
    public float containerWidth = 10f;  // 容器总宽度
    public float spacing = 0.2f;        // 最小间距
    public Alignment alignment = Alignment.Center;
    public bool autoUpdate = true;

    void Update()
    {
        if (autoUpdate)
        {
            ApplyLayout();
        }
    }

    public void ApplyLayout()
    {
        // 1. 收集所有有效的子物体
        var children = new System.Collections.Generic.List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.GetComponent<SpriteRenderer>() != null)
            {
                children.Add(child);
            }
        }

        if (children.Count == 0) return;

        // 2. 计算所有子物体的原始总宽度
        float totalChildWidth = 0f;
        foreach (var child in children)
        {
            totalChildWidth += child.GetComponent<SpriteRenderer>().bounds.size.x;
        }

        // 3. 计算平均间距
        float availableSpace = containerWidth - totalChildWidth;
        float actualSpacing = spacing;

        if (children.Count > 1)
        {
            actualSpacing = Mathf.Max(spacing, availableSpace / (children.Count - 1));
        }

        // 4. 根据对齐方式确定起始X
        float totalWidth = totalChildWidth + actualSpacing * (children.Count - 1);
        float startX = 0f;
        switch (alignment)
        {
            case Alignment.Left:
                startX = -containerWidth / 2f;
                break;
            case Alignment.Center:
                startX = -totalWidth / 2f;
                break;
            case Alignment.Right:
                startX = containerWidth / 2f - totalWidth;
                break;
        }

        // 5. 按顺序放置子物体
        float currentX = startX;
        foreach (var child in children)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            float w = sr.bounds.size.x;

            child.localPosition = new Vector3(currentX + w / 2f, 0f, 0f);
            currentX += w + actualSpacing;
        }
    }
}
