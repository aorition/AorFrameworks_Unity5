using System;
using System.Collections.Generic;

namespace Framework
{

    public enum HOTransformTypeEnum
    {
        Relative,//相对
        Absolutely,//绝对
        Overlying,//叠加
        AbsOverlying,//绝对叠加
        OverlyingRate,//等比叠加
        AbsOverlyingRate,//绝对等比叠加
        Circle,//圆环叠加
    }

    public enum HORotationTypeEnum
    {
        Relative,//相对
        Absolutely,//绝对
        Overlying,//叠加
        AbsOverlying,//绝对叠加
        OverlyingRate,//等比叠加
        AbsOverlyingRate,//绝对等比叠加
    }

    public enum HOScaleTypeEnum
    {
        Relative,//相对
        Overlying,//叠加
        OverlyingRate,//等比叠加
    }

    public enum HOCircleType
    {
        X,
        Y,
        Z
    }

}
