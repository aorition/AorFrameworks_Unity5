using System;

/// <summary>
/// KeyframeTangentMode
/// tangentMode在官方API文档中未能查到,自己补一个
/// </summary>
public enum KeyframeTangentMode
{
    FreeBoth = 1,
    LinearFree = 5,
    ConstantFree = 7,
    FreeLinear = 17,
    LinearBoth = 21,
    ConstantLinear = 23,
    FreeConstant = 25,
    LinearConstant = 29,
    ConstantBoth = 31,
}
