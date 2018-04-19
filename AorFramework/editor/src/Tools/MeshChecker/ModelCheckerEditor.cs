using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Framework.Tools;
using UnityEditor;
using UnityEngine;

namespace Framework.editor.tools
{


    public class ShaderDefine_ModelChecker : IShaderDefine
    {
        public string ShaderName { get; } = "ModelChecker";
        public string ShaderLabel { get; } = "FrameworkTools/ModelChecker";
        public string SavePath { get; } = FrameworkAssetsPathDefine.EditorShader;
        public string ShdaerCode { get; } = "//@@@DynamicShaderInfoStart\n//<Readonly>模型检查Shader, 方便快捷查看模型的 VertexColor, Normal, WorldNormal, Tangent, UV, Lightmap 的情况\n//@@@DynamicShaderInfoEnd\n\nShader \"FrameworkTools/ModelChecker\"\n{\n\tProperties\n\t{\n\t\t_MainTex (\"Base (RGB) Trans (A)\", 2D) = \"white\" {}\n\t\t[Enum(MainTex, 0, VertexColor, 1, Normal, 2, WorldNormal, 3, Tangent, 4, UV, 5, Lightmap, 6)] _checkType (\"Check Type\", Float) = 0\n\t\t[Enum(UnityEngine.Rendering.CullMode)] _cull(\"Cull Mode\", Float) = 0\n\t}\n\n\tSubShader\n\t{\n\n\t\tTags{ \"Queue\" = \"Geometry\" \"RenderType\" = \"Geometry\" }\n\n\t\tPass\n\t\t{\n\t\t\tTags{ \"LightMode\" = \"ForwardBase\" }\n\n\t\t\tLighting On\n\t\t\t\n\t\t\tCull[_cull]\n\t\t\t\n\t\t\tCGPROGRAM\n\n\t\t\t#include \"UnityCG.cginc\"\n\t\t\t#include \"Lighting.cginc\"\n\t\t\t#include \"AutoLight.cginc\"\n\t\t\t\n\t\t\t#pragma vertex vert\n\t\t\t#pragma fragment frag\n\t\t\t#pragma multi_compile LIGHTMAP_OFF  LIGHTMAP_ON \n\t\t\t#pragma multi_compile_fwdbase\n\t\t\t#pragma multi_compile_fog\n\t\t\t#pragma exclude_renderers xbox360 ps3\n\t\t\t\n\t\t\tsampler2D _MainTex;\n\t\t\t//UV映射必要声明的参数, TRANSFORM_TEX(v.uv, _MainTex)会用到\n\t\t\tfloat4 _MainTex_ST;\n\t\t\tfloat _checkType;\n\t\t\t\n\t\t\tstruct v2f\n\t\t\t{\n\t\t\t\tfloat4 pos : SV_POSITION;\n\t\t\t\tfloat2  uv : TEXCOORD0;\n\t\t\t\t\n\t\t\t\t#ifdef LIGHTMAP_ON\n\t\t\t\tfloat2  lightmapUV : TEXCOORD1;\n\t\t\t\t#endif\n\t\t\t\t\n\t\t\t\tfloat3  normal : TEXCOORD2;\n\t\t\t\tfloat4  tangent : TEXCOORD3;\n\t\t\t\tfloat3 worldPos : TEXCOORD4;\n\t\t\t\t\n\t\t\t\tfloat3\tcolor : TEXCOORD5;\n\t\t\t\t\n\t\t\t\tLIGHTING_COORDS(6, 7)\n\t\t\t\tUNITY_FOG_COORDS(8)\n\t\t\t};\n\t\t\t\n\t\t\tv2f vert(appdata_full v)\n\t\t\t{\n\t\t\t\tv2f o;\n\n\t\t\t\to.pos = UnityObjectToClipPos(v.vertex);\n\t\t\t\to.uv = TRANSFORM_TEX(v.texcoord, _MainTex);\n\t\t\t\t\n\t\t\t\t#ifdef LIGHTMAP_ON\n\t\t\t\to.lightmapUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;\n\t\t\t\t#endif\n\t\t\t\t\n\t\t\t\t//o.normal = UnityObjectToWorldNormal(v.normal); // 模型坐标法线转换世界坐标法线\n\t\t\t\to.normal = v.normal;\n\t\t\t\t\n\t\t\t\to.tangent = v.tangent;\n\t\t\t\t\n\t\t\t\to.worldPos = mul(unity_ObjectToWorld, v.vertex); // 模型坐标顶点转换世界坐标顶点\n\t\t\t\t\n\t\t\t\to.color = v.color;\n\n\t\t\t\tUNITY_TRANSFER_FOG(o, o.pos);\n\n\t\t\t\tTRANSFER_VERTEX_TO_FRAGMENT(o);\n\n\t\t\t\treturn o;\n\t\t\t}\n\n\t\t\tfixed4 frag(v2f i) : SV_Target\n\t\t\t{\n\t\t\t\tif(_checkType == 1) {\n\t\t\t\t\treturn fixed4(i.color,1);\n\t\t\t\t}\n\t\t\t\tif(_checkType == 2){\n\t\t\t\t\treturn fixed4(i.normal,1);\n\t\t\t\t}\n\t\t\t\tif(_checkType == 3){\n\t\t\t\t\tfixed3 worldNormal = normalize(UnityObjectToWorldNormal(i.normal)); // 法线方向\n\t\t\t\t\treturn fixed4(worldNormal,1);\n\t\t\t\t}\n\t\t\t\tif(_checkType == 4){\n\t\t\t\t\treturn i.tangent;\n\t\t\t\t}\n\t\t\t\tif(_checkType == 5){\n\t\t\t\t\treturn fixed4(i.uv,0,1);\n\t\t\t\t} \n\t\t\t\tif(_checkType == 6){\n\t\t\t\t\t#ifdef LIGHTMAP_ON\n\t\t\t\t\tfixed3 lightMapColor = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lightmapUV)).rgb;\n\t\t\t\t\treturn fixed4(lightMapColor,1);\n\t\t\t\t\t#endif\n\t\t\t\t\treturn 0;\n\t\t\t\t} \n\t\t\t\treturn tex2D(_MainTex, i.uv);\n\t\t\t}\n\t\t\tENDCG\n\t\t}\n\n\t}\n}";
    }
    
    [CustomEditor(typeof(MeshChecker))]
    public class MeshCheckerEditor : UnityEditor.Editor
    {
        private void Awake()
        {
            ShaderDefine_ModelChecker shaderDefine = new ShaderDefine_ModelChecker();

            Shader ckShader = Shader.Find(shaderDefine.ShaderLabel);
            if (!ckShader)
            {
                FrameworkBaseShaderCreater.BuildingShaderFile(shaderDefine);
            }

        }
    }

}

