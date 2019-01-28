//@@@DynamicShaderInfoStart
//<readonly>空气扭曲,请配合DistortionSupport脚本使用,用了此shader的物体层级设置为postEffect
//@@@DynamicShaderInfoEnd
Shader "AFW/Unlit/Unlit - Distortion - RT" 
{

	Properties 
	{
		_MainTex ("MainRT", 2D) = "white" {}
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_MaskTex ("Mask Texture", 2D) = "white" {}  
		_Scale("Distortion Scale", Float) =1
		_SpeedX("Distortion SpeedX", Float) = 1
		_SpeedY("Distortion SpeedY", Float) = 1

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
		
		Tags { "Queue" = "Geometry" "IgnoreProjector" = "True" "RenderType" = "Geometry" }

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
			sampler2D _MaskTex;
			sampler2D _NoiseTex;
			float4 _MaskTex_ST;
			float4 _NoiseTex_ST;

			float4 _TintColor;
			float _mask;
			float _lineSize;	
			float4 _lineColor;

			float _Scale;
			float _SpeedX;
			float _SpeedY;

			struct a2v 
			{
				float4 vertex : POSITION;
				float2 texcoord:TEXCOORD0;
			};

			struct v2f 
			{
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
				float2  uvMask : TEXCOORD1;
				float2  uvNoise : TEXCOORD2;
			};
			
			//顶点函数没什么特别的，和常规一样
			v2f vert (a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uvMask = TRANSFORM_TEX(v.texcoord, _MaskTex);
				o.uvNoise = TRANSFORM_TEX(v.texcoord, _NoiseTex);

				float4 screenUV = ComputeGrabScreenPos(o.pos);//计算该模型顶点在屏幕坐标的纹理信息
				o.uv = screenUV.xy/screenUV.w;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				
				half4 maskCol = tex2D(_MaskTex, i.uvMask);
				half coordOffsetX = _SpeedX * _Time;
				half coordOffsetY = _SpeedY * _Time;
				half4 noiseCol = tex2D(_NoiseTex, i.uvNoise+ half2(coordOffsetX, coordOffsetY));

				noiseCol = noiseCol * 2 - 1;
				
				half offsetX = noiseCol.r * maskCol.r*_Scale;
				half offsetY = noiseCol.g * maskCol.r*_Scale;

				half4 texCol = tex2D(_MainTex, i.uv + half2(offsetX, offsetY));
				
				return texCol;
			}
			ENDCG
		}//end pass
	}//end SubShader
}//end Shader