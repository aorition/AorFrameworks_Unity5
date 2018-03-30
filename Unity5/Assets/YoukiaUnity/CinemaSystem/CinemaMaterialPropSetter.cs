using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Cinema 动态对象 材质修改器 子组件
/// 
/// 须配合CinemaMaterialHandler一起使用
/// 
/// </summary>
public class CinemaMaterialPropSetter : MonoBehaviour
{
    public CinemaMaterialPropSetterType type = CinemaMaterialPropSetterType.Float;
    public string ShaderPropName;

    public Color color;
    public float Value;
    

}


public enum CinemaMaterialPropSetterType
{
    Color,
    Float
}