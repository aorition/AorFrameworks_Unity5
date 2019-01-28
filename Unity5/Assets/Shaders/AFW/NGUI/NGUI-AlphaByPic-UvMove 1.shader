Shader "AFW/NGUI/NGUI - AlphaByPic - UvMove 1" 
{

	Properties
	{

		_MainTex("Texture", 2D) = "white" { }
		_AlphaTex("Texture", 2D) = "white" { }

		_TintColor("Color", Color) = (1,1,1,0)
		_TimeScale("TimeScale", float) = 3

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
			ZWrite Off
			ZTest[_zTest]
			Cull[_cull]

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag
			
			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _AlphaTex;
			float4 _AlphaTex_ST;

			float4 _TintColor;
			float _TimeScale;
			//NGUI 1字头 -- 标配参数组
			float4 _ClipRange0 = float4(0.0, 0.0, 1.0, 1.0);
			float2 _ClipArgs0 = float2(1000.0, 1000.0);
			//NGUI 1字头 -- 标配参数组 end
			struct v2f {
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
				float2  uv2 : TEXCOORD1;
				float2 worldPos : TEXCOORD2;
			};

			//顶点函数没什么特别的，和常规一样
			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.uv2 = TRANSFORM_TEX(v.texcoord,_AlphaTex);
				o.worldPos = v.vertex.xy * _ClipRange0.zw + _ClipRange0.xy;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float4 texCol = tex2D(_MainTex,i.uv + _Time*_TimeScale);
				float4 alphaCol = tex2D(_AlphaTex,i.uv2);

				texCol.r *= _TintColor.r + _TintColor.a;
				texCol.g *= _TintColor.g + _TintColor.a;
				texCol.b *= _TintColor.b + _TintColor.a;
				texCol.a = min(alphaCol.r,texCol.a);

				//NGUI 1字头 -- 标配裁切算法
				float2 factor = (float2(1.0, 1.0) - abs(i.worldPos)) * _ClipArgs0;
				texCol.a *= min(max(min(factor.x, factor.y), 0.0), 1.0);
				//NGUI 1字头 -- 标配裁切算法 end

				return texCol;
			}

			ENDCG
		}//end pass
	}//end SubShader
}//end Shader