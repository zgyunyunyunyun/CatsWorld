using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberController : MonoBehaviour
{
    public static NumberController instance;
    private void Awake()
    {
        instance = this;
    }

    //������ת��Ϊ�ַ��������ڣ�
    public string NumberToChinaString(int number)
    {
        if(number >= 100000000)
        {
            return (number / 100000000).ToString()+"��";
        }else if (number >= 10000)
        {
            return (number / 10000).ToString()+"��";
        }
        else
        {
            return number.ToString();
        }
    }

}
