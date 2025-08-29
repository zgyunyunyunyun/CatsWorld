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

    //将数字转化为字符串（万、亿），保留千位一下的数字，如101000转化为10万1000
    public string NumberToChinaString(int number)
    {
        string str;
        if (number >= 100000000)
        {
            int yi = number / 100000000;
            int wan = number % 100000000 / 10000;
            int qian = number % 10000;

            if (wan == 0 && qian == 0)
            {
                str = $"{yi}亿";
            }
            else if (wan != 0 && qian == 0)
            {
                str = $"{yi}亿{wan}万";
            }
            else if (wan == 0 && qian != 0)
            {
                str = $"{yi}亿 {qian}";
            }
            else
            {
                str = $"{yi}亿{wan}万 {qian}";
            }
        }
        else if (number >= 10000)
        {
            int wan = number / 10000;
            int qian = number % 10000;

            if (qian == 0)
            {
                str = $"{wan}万";
            }
            else
            {
                str = $"{wan}万 {qian}";
            }
        }
        else
        {
            str = $"{number}";
        }

        return str;
    }
}
