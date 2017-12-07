// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//@@@DynamicShaderInfoStart
//自发光的边缘高光材质 混合模式可以改叠加 不可写深度，不可选无Alpha的叠加模式
//@@@DynamicShaderInfoEnd


//@@@DynamicShaderTitleRepaceStart
Shader "Custom/NoLight/Unlit - Tornado##" {
//@@@DynamicShaderTitleRepaceEnd

    Properties {
        _MainTex ("Texture", 2D) = "white" { }
		_TintColor("TintColor", color) = (1,1,1,1)
        _alpha("Alpha", range(-2,2)) = 1
        _light("Light", float) = 1

        _RimColor("Rim Color", Color) = (1,1,1,1)
		_RimWidth ("Rim Width", range(0, 1.3)) = 1
		_RimPower ("Rim Power", range(-4, 8)) = 1

			[Enum(Off, 0, On, 1)] _ZWrite("ZWrite", Float) = 0
			[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4
			[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend Mode", Float) = 5
			[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend Mode", Float) = 10
			[Enum(UnityEngine.Rendering.BlendMode)] _SrcAlphaBlend("Src Alpha Blend Mode", Float) = 1
			[Enum(UnityEngine.Rendering.BlendMode)] _DstAlphaBlend("Dst Alpha Blend Mode", Float) = 10
			[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2

			[Toggle] _HasNight("HasNight?", Float) = 0    //夜晚效果开关
    }
    

    SubShader
    {
			Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
       		
        pass
        {
		

				Blend[_SrcBlend][_DstBlend],[_SrcAlphaBlend][_DstAlphaBlend]
				ZWrite[_ZWrite]
				ZTest[_ZTest]
				Cull[_Cull]
				Lighting Off
			
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _HASNIGHT_ON
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            half4 _MainTex_ST;
			fixed4 _TintColor;
           	fixed _alpha;
           	fixed _light;
           	fixed4 _RimColor;
            fixed _RimWidth;
            fixed _RimPower;

            //夜晚系统全局变量
            float _HdrIntensity;
            float3 _DirectionalLightColor;
            float4 _DirectionalLightDir;


            struct v2f {

                float4  pos : SV_POSITION;
                float3 normal : TEXCOORD0;
                float2  uv : TEXCOORD1;
                float3 worldvertpos : TEXCOORD2;
                float4 vertColor: COLOR;
                float4 rimColor : COLOR1;

            };
           

            v2f vert (appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
				o.normal=v.normal;
				o.uv =TRANSFORM_TEX(v.texcoord,_MainTex);
				o.worldvertpos = ObjSpaceViewDir(v.vertex).xyz;

				float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
                float dotProduct = 1 - dot(v.normal, viewDir);
                o.rimColor = smoothstep(1 - _RimWidth, 1, dotProduct);
                o.rimColor *= _RimColor * _RimPower;

            #ifdef _HASNIGHT_ON
                o.rimColor.rgb *= _HdrIntensity+_DirectionalLightColor*_DirectionalLightDir.w+ o.rimColor.rgb*UNITY_LIGHTMODEL_AMBIENT.xyz;    //夜晚效果
            #endif

                o.vertColor = v.color;

                return o;
            }
 
 
            float4 frag (v2f i) : COLOR
            {
            
				i.normal = normalize(i.normal);
				fixed3 viewdir = normalize( i.worldvertpos);
				fixed4 texCol = tex2D(_MainTex,i.uv)*_TintColor;

				texCol.rgb += i.rimColor * _light;

				//texCol.a =pow(saturate(dot(viewdir, i.normal)),_alpha) * 1.5f;
				texCol.a = smoothstep(1 - _alpha, 1, dot(viewdir, i.normal));

				texCol *= i.vertColor;

				return texCol;
 
            }
            ENDCG
        }
    }
}