Shader "Hidden/PostEffect/BloomDrawShader" 
{

	Properties 
	{
		_MainTex ("MainRt", 2D) = "white" {}
		_CurveTex ("CurveTex", 2D) = "Black" {}
	}

	SubShader 
	{

	     Tags
		 { 
			 "Queue"="Transparent" 
			 "IgnoreProjector"="True" 
			 "RenderType"="Transparent" 
			 "PreviewType"="Plane"
		     //"CanUseSpriteAtlas"="True"
		 }

		 Pass 
		 {

			//Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			//#pragma multi_compile DUMMY PIXELSNAP_ON
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _CurveTex;
			
			float _Intensity;
			float _Saturation;


			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f 
			{
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD2;
				float4 color    : COLOR;
			};

			//顶点函数没什么特别的，和常规一样
			v2f vert (appdata_t v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv =TRANSFORM_TEX(v.texcoord,_MainTex);
				//	o.pos = UnityPixelSnap ( o.pos);
				o.color=v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target 
			{
				
				float4 mainCol = tex2D(_MainTex, i.uv);
				
				fixed4 curveCol = tex2D(_CurveTex, i.uv);

				// 饱和度
				// 特定系数
				fixed luminance = 0.2125 * curveCol.r + 0.7154 * curveCol.g + 0.0721 * curveCol.b;
				fixed3 luminanceColor = fixed3(luminance, luminance, luminance);
				curveCol.rgb = lerp(luminanceColor, curveCol, _Saturation);
				
				float3 rgbBloom = mainCol.rgb*curveCol.rgb + pow(curveCol.rgb, 2)*(1 + _Intensity);
				//  return rgbBloom.xyzz;
				mainCol.rgb = mainCol.rgb + rgbBloom;
				mainCol.a += rgbBloom * 4;
				return  mainCol;
			}
			ENDCG
		 }//end pass
	}//end SubShader
}//end Shader