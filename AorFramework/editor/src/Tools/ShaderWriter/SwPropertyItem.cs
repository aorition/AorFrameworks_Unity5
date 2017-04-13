using UnityEngine;

namespace ShaderWriter
{
    internal enum SwPropertyValueType
    {
        Textrue,
        Color,
        Float,
        Range,
    }

    internal class SwPropertyItem
    {

        public SwPropertyItem(){}

        public bool PerRendererData = false;
        public bool MaterialToggle = false;
        public bool HideInInspector = false;

        public string _name;
        public string _label;
        public SwPropertyValueType _valueType;
        public float _floatValue;
        public Vector2 _range = new Vector2(0, 1f);
        public Color _colorValue = Color.white;

        private string _valueToString()
        {
            switch (_valueType)
            {
                case SwPropertyValueType.Textrue:
                    return "\"white\" {}";
                case SwPropertyValueType.Color:
                    return "(" + _colorValue.r + "," + _colorValue.g + "," + _colorValue.b + "," + _colorValue.a + ")";
                case SwPropertyValueType.Range:
                case SwPropertyValueType.Float:
                    return _floatValue.ToString();
                default:
                    return "";
            }
        }

        private string _valueTypeToString()
        {
            switch (_valueType)
            {
                case SwPropertyValueType.Textrue:
                    return "2D";
                case SwPropertyValueType.Float:
                    return "Float";
                case SwPropertyValueType.Color:
                    return "Color";
                case SwPropertyValueType.Range:
                    return "Range(" + _range.x + "," + _range.y + ")";
                default:
                    return "";
            }
        }

        public string toString()
        {
            return  "\t\t" +
                    (HideInInspector ? "[HideInInspector] " : "") +
                    (PerRendererData ? "[PerRendererData] " : "") + 
                    (MaterialToggle ? "[MaterialToggle]" : "") +
                    " " + _name + "(" + 
                    "\"" + _label + "\", " + _valueTypeToString() + ")" + 
                    " = " + _valueToString();
        }
    }
}