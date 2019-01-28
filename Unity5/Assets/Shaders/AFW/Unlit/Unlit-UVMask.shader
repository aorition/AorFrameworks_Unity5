//@@@DynamicShaderInfoStart
//按UV Y方向做半透明过度 可反向
//@@@DynamicShaderInfoEnd
Shader "AFW/Unlit/Unlit - UVMask" 
{

	Properties 
	{
		_MainTex ("Texture", 2D) = "white" {}
		_mask("Mask", Range(0,1)) = 0
		_range("Range", Range(0.01,1)) = 0
		_TintColor("Color", Color) = (1,1,1,1)
		_Lighting("Lighting",  float) = 1

		[MaterialToggle] _reverse("MaskReverse", Float) = 0

		[Enum(Off, 0, On, 1)] _zWrite("ZWrite", Float) = 1
		[Enum(UnityEngine.Rendering.CompareFunction)] _zTest("ZTest", Float) = 4
		[Enum(UnityEngine.Rendering.BlendMode)] _srcBlend("Src Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _dstBlend("Dst Blend Mode", Float) = 10
		[Enum(UnityEngine.Rendering.BlendMode)] _srcAlphaBlend("Src Alpha Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _dstAlphaBlend("Dst Alpha Blend Mode", Float) = 10
		[Enum(UnityEngine.Rendering.CullMode)] _cull("Cull Mode", Float) = 2
	}

	SubShader
	{
		
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
		{
		
			Blend[_srcBlend][_dstBlend],[_srcAlphaBlend][_dstAlphaBlend]
			ZWrite[_zWrite]
			ZTest[_zTest]
			Cull[_cull]

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag
			
			float4 _TintColor;
			float _Lighting;
			float _range;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _mask;
			float _reverse;

			struct v2f 
			{
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
			};
           
           
			struct appdata 
			{
				float4 vertex : POSITION;
				float2 texcoord:TEXCOORD0;
			};

			//顶点函数没什么特别的，和常规一样
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				_mask = _mask * (1.0 +_range) - _range;  		
				float4 col= tex2D(_MainTex,i.uv)*_TintColor*_Lighting;
				col.a=min(col.a, saturate((i.uv.y * (1.0 - _reverse) + (1.0- i.uv.y) * _reverse - _mask)/_range));
				clip(col.a);
				return col;
				return  float4(_reverse,_reverse,_reverse,1);
			}
			ENDCG
		}//end pass
	}//end SubShader
}//end Shader