using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectMove : MonoBehaviour
{
    public float speed = 0.1f;//�ƶ��ٶ�
    private Rigidbody2D rb;
    private Vector3 movement;//�ƶ�����

    public float timer = 0f;//ÿ��x��ı��ƶ�����
    public float activity = 5f;//�ƶ�����ľ��룬�������ƶ��ķ���

    private RectTransform rectTransform;

    public int objectType = -1;//0����Һ��1������

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //GetComponent<Button>().onClick.AddListener(() => addPointAndDestroy());


        //rectTransform = GetComponent<RectTransform>();//ʵ�ִ���Ч��

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("Touch");
            //����������Ƿ���
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var onScreenPosition = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(onScreenPosition);

            var hit = Physics2D.Raycast(new Vector2(ray.origin.x, ray.origin.y), Vector2.zero, Mathf.Infinity);
            if (hit.collider != null && gameObject == hit.collider.gameObject)
            {               
                Debug.Log("Touch");
                Debug.Log(hit.collider.gameObject.name);
                addPointAndDestroy();
                
                //hit.collider.gameObject.transform.position = ray.origin;
            }
        }

        //ʵ�ִ���Ч������ʱ����
        /*
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Debug.Log("Touch");
            Touch touch = Input.GetTouch(0);
            Vector3 worldPosition;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, touch.position, Camera.main, out worldPosition))
            {
                Vector2 localPoint;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, touch.position, Camera.main, out localPoint))
                {
                    Vector2 localMousePos = new Vector2(localPoint.x, localPoint.y);
                    if (rectTransform.rect.Contains(localMousePos))
                    {
                        Debug.Log("Clicked on 2D Object!");
                    }
                }
            }
        }*/
    }

    private void FixedUpdate()
    {
        
        //��ʱ����ÿ��xs���ı�һ���ƶ�����
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            movement = new Vector3(Random.Range(-0.7f * activity, 0.7f * activity), Random.Range(-1 * activity, 1 * activity), 0);
            timer = 5f;
        }
       
        rb.AddForce(movement * speed);
    }

    void addPointAndDestroy()
    {
        if(objectType == 0)
        {
            Debug.Log("�ռ���Һ1��!");
            UpLianDanController.instance.addPurityPoint(1f);
            Destroy(gameObject);
        }else if(objectType ==1)
        {
            Debug.Log("�ռ�����1��!");
            UpLianDanController.instance.addImpurityPoint(1f);
            Destroy(gameObject);
        }
        
    }
}
