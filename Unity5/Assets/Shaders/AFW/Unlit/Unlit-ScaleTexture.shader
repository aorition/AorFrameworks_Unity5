//@@@DynamicShaderInfoStart
//<readonly> ScaleTexture
//@@@DynamicShaderInfoEnd
Shader "AFW/Unlit/Unlit - ScaleTexture" 
{

	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		
		_Main_Rotation("Main Rotation Angle", Float) = 0
		_Main_Scale("Main Scale",Range(0.001,10)) = 1

		[Enum(Off, 0, On, 1)] _zWrite("ZWrite", Float) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)] _zTest("ZTest", Float) = 4
		[Enum(UnityEngine.Rendering.BlendMode)] _srcBlend("Src Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _dstBlend("Dst Blend Mode", Float) = 10
		[Enum(UnityEngine.Rendering.BlendMode)] _srcAlphaBlend("Src Alpha Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _dstAlphaBlend("Dst Alpha Blend Mode", Float) = 10
		[Enum(UnityEngine.Rendering.CullMode)] _cull("Cull Mode", Float) = 2
	}

	SubShader
	{
		
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

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
			
			sampler2D _MainTex;
			float4 _MainTex_ST;

			float _Main_Rotation;
			float _Main_Scale;

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord  : TEXCOORD0;
			};

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = TRANSFORM_TEX(IN.texcoord,_MainTex);
				OUT.color = IN.color;

				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				float2 uv = IN.texcoord.xy - float2(0.5,0.5);
				float mag = radians(_Main_Rotation);

				uv = float2(uv.x/_Main_Scale*cos(mag) - uv.y/_Main_Scale*sin(mag),
					uv.x/_Main_Scale*sin(mag) + uv.y/_Main_Scale*cos(mag));
				uv += float2(0.5,0.5);

				float4 col = tex2D(_MainTex,uv) * IN.color;

				return col;
			}
			ENDCG
		}//end pass
	}//end SubShader
}//end SubShader