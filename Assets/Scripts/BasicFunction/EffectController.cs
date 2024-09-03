using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    public float shrinkDuration = 0.3f; // ��������ʱ������λ��
    public float shrinkInterval = 0.005f; // ÿ�εļ��ʱ�䣬��λ��
    public bool isAnimating = false;
    public bool animationFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        isAnimating = false;
        animationFinished = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool AnimationFinished()
    {
        return animationFinished;
    }

    public bool AnimationBeginning()
    {
        return isAnimating;
    }

    //�Ŵ������С���壨1���Ŵ�0����С��
    public void scaleObject(bool isScale)
    {
        isAnimating = true;

        // ��ʼ��С����
        StartCoroutine(ShrinkToHidden(isScale));
    }

    IEnumerator ShrinkToHidden(bool isScale)
    {
        // ��ȡ��ʼ��С
        Vector3 initialScale = transform.localScale;

        // ͨ��ָ��ʱ��������С������С����������
        float elapsedTime = 0;
        while (elapsedTime < shrinkDuration)
        {
            // ������С�ı���
            float scaleRatio = 0;
            if (isScale)
            {
                scaleRatio = 1 + elapsedTime / shrinkDuration;
            }
            else
            {
                scaleRatio = 1 - elapsedTime / shrinkDuration;
            }
            
            transform.localScale = new Vector3(initialScale.x * scaleRatio, initialScale.y * scaleRatio, initialScale.z * scaleRatio);

            // �ȴ����ʱ��
            yield return new WaitForSeconds(shrinkInterval);
            elapsedTime += shrinkInterval;
            //Debug.Log("�������Ž��������ö�Ӧ��ֵ1" + animationFinished + "��ֵ2" + isAnimating);
        }

        // ��������С����Ϊ0������
        if (!isScale)
        {
            transform.localScale = Vector3.zero;
        }

        //��������
        animationFinished = true;
        isAnimating = false;
        Debug.Log("�������Ž��������ö�Ӧ��ֵ1" + animationFinished + "��ֵ2" + isAnimating);
    }
}
