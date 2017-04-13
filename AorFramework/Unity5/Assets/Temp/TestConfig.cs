using System;
using System.Collections.Generic;
using AorBaseUtility;

public enum TestConfigEnum
{
    one,
    two,
    three,
    four
}

public class TestConfig : Config
{

    public readonly int intValue;
    public readonly string stringValue;
    public readonly bool boolValue;
    public readonly TestConfigEnum enumValue;
    public readonly string[] strArrayValue;
    public readonly string[][] strArray2Value;
    public readonly List<string> listValue;
    public readonly Dictionary<string, int> DicValue;

}
