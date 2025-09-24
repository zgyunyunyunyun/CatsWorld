using UnityEngine;

public class ClickManager : MonoBehaviour
{
    public LayerMask catLayerMask;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // 获取所有被点击到的对象
            RaycastHit[] hits = Physics.RaycastAll(ray, 100f, catLayerMask);

            if (hits.Length > 0)
            {
                CatEntity topCat = null;
                int maxOrder = int.MinValue;

                foreach (var hit in hits)
                {
                    SpriteRenderer sr = hit.collider.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        int order = sr.sortingOrder;
                        if (order > maxOrder)
                        {
                            maxOrder = order;
                            topCat = hit.collider.GetComponent<CatEntity>();
                        }
                    }
                }

                if (topCat != null)
                {
                    Debug.Log("点中了最上层的 CatEntity: " + topCat.name);
                    topCat.OnClick();
                }
                else
                {
                    Debug.Log("点中了其他物体，但不是 CatEntity");
                }
            }
            else
            {
                Debug.Log("没点中任何 CatEntity");
            }
        }
    }
}
