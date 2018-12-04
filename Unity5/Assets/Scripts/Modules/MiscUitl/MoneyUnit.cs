using System;
using System.Collections.Generic;
using UnityEngine;

public enum MoneyUnitEnum
{
    none = 1,
    K,
    M,
    G,
    T,
    P,
    E,
    Z,
    Y
}

public class MoneyUnit
{

    /// <summary>
    /// 显示数字转换
    /// 达到6位进行转换
    /// </summary>
    /// <param name="money"></param>
    /// <returns></returns>
    public static string MoneyToStr(long money)
    {

        if ( money <= Math.Pow(1000, 2))
        {
            return permil(money, MoneyUnitEnum.none);
        }
        else if (money > Math.Pow(1000, 2) && money <= Math.Pow(1000, 3))
        {
            money /= (long)Math.Pow(1000, 1); 
            return permil(money, MoneyUnitEnum.K);
        }
        else if (money > Math.Pow(1000, 3) && money <= Math.Pow(1000, 4))
        {
            money /= (long)Math.Pow(1000, 2);
            return permil(money, MoneyUnitEnum.M);
        }
        else if (money > Math.Pow(1000, 4) && money <= Math.Pow(1000, 5))
        {
            money /= (long)Math.Pow(1000, 3);
            return permil(money, MoneyUnitEnum.G);
        }
        else if (money > Math.Pow(1000, 5) && money <= Math.Pow(1000, 6))
        {
            money /= (long)Math.Pow(1000, 4);
            return permil(money, MoneyUnitEnum.T);
        }
        else if (money > Math.Pow(1000, 6) && money <= Math.Pow(1000, 7))
        {
            money /= (long)Math.Pow(1000, 5);
            return permil(money, MoneyUnitEnum.P);
        }
        else if (money > Math.Pow(1000, 7) && money <= Math.Pow(1000, 8))
        {
            money /= (long)Math.Pow(1000, 6);
            return permil(money, MoneyUnitEnum.E);
        }

        return "null";
    }


    /// <summary>
    /// 显示数字转换
    /// 达到3位进行转换
    /// </summary>
    /// <param name="money"></param>
    /// <returns></returns>
    public static string MoneyToStrThree(long money)
    {

        if (money < 1000)
        {
            return permilThree(money, MoneyUnitEnum.none);
        }
        else if (money >= Math.Pow(1000, 1) && money < Math.Pow(1000, 2))
        {
            money /= (long)Math.Pow(1000, 1);
            return permilThree(money, MoneyUnitEnum.K);
        }
        else if (money >= Math.Pow(1000, 2) && money <= Math.Pow(1000, 3))
        {
            money /= (long)Math.Pow(1000, 2);
            return permilThree(money, MoneyUnitEnum.M);
        }
        else if (money >= Math.Pow(1000, 3) && money < Math.Pow(1000, 4))
        {
            money /= (long)Math.Pow(1000, 3);
            return permilThree(money, MoneyUnitEnum.G);
        }
        else if (money >= Math.Pow(1000, 4) && money < Math.Pow(1000, 5))
        {
            money /= (long)Math.Pow(1000, 4);
            return permilThree(money, MoneyUnitEnum.T);
        }
        else if (money > Math.Pow(1000, 5) && money <= Math.Pow(1000, 6))
        {
            money /= (long)Math.Pow(1000, 5);
            return permilThree(money, MoneyUnitEnum.P);
        }
        else if (money > Math.Pow(1000, 6) && money <= Math.Pow(1000, 7))
        {
            money /= (long)Math.Pow(1000, 6);
            return permilThree(money, MoneyUnitEnum.E);
        }

        return "null";
    }


    private static string permil(long num, MoneyUnitEnum moneyUnit)
    {

        string MoneyNum = num.ToString();

        if (MoneyNum.Length <= 3)
        {
            return MoneyNum;
        }
        else
        {
            if ((int)moneyUnit == 1)
            {
                return MoneyNum.Substring(0, MoneyNum.Length - 3) + "," + MoneyNum.Substring(MoneyNum.Length - 3, 3);
            }
            else
            {
                return MoneyNum.Substring(0, MoneyNum.Length - 3) + "," + MoneyNum.Substring(MoneyNum.Length - 3, 3) + moneyUnit.ToString();

            }
        }

    }

    private static string permilThree(long num, MoneyUnitEnum moneyUnit)
    {

        string MoneyNum = num.ToString();

        if ((int)moneyUnit == 1)
        {
            return MoneyNum.ToString();
        }
        else
        {
            return MoneyNum.ToString() + moneyUnit.ToString();

        }
    }

}
