using System;
using System.Collections.Generic;
using AorBaseUtility;
public class RoleViewConfig : TConfig
{

    [ConfigComment("角色名称")]
    public readonly string Name;

    [ConfigComment("模型资源")]
    public readonly string ModelPath;

    [ConfigComment("缩放倍率")]
    public readonly float Scale = 1f;

    [ConfigComment("模型旋转修正")]
    public readonly float[] Rotation = { 0, 0, 0 };

    [ConfigComment("头像名称")]
    public readonly string HeadImageName = "";

    [ConfigComment("特写名称")]
    public readonly string CutInName = "";

    [ConfigComment("待机动画")]
    public readonly string IdleAnim = "stand1";

    [ConfigComment("移动动画")]
    public readonly string MoveAnim = "rush";

    [ConfigComment("行进动画")]
    public readonly string AdvanceAnim = "move1";

    [ConfigComment("浮空动画")]
    public readonly string FloatingAnim = "hurtfly";

    [ConfigComment("起身动画")]
    public readonly string GetUpAnim = "getup";

    [ConfigComment("失控动画")]
    public readonly string UncontrollableAnim = "stun";

    [ConfigComment("硬直动画")]
    public readonly string StiffAnim = "hurt";

    [ConfigComment("死亡动画")]
    public readonly string DeadAnim = "die";
}
