//@@@DynamicShaderInfoStart
//<readonly> NGUI ��������һ����ͼ��RGBŤ������Ч�� (ע�⣺Shader�����UITexttureʹ��)
//@@@DynamicShaderInfoEnd
Shader "AFW/NGUI/NGUI - TextureNoise" 
{
	
	Properties
	{
		[PerRendererData]_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		_noiseTex("Noise(RGB)",2D) = "white"{}
		_size("noise Size",range(0,1)) = 0.1
		_speed("water speed",range(-1,1)) = 1
		_Scale("wave Scale",range(0.01,2)) = 1
		_alpha("wave  alpha", float) = 1
	}
	
	SubShader
	{
		LOD 200

		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		
		Pass
		{
			Lighting Off
			Fog{ Mode Off }
			Offset -1, -1
			Cull Off
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _noiseTex;

			float  _size;
			float  _speed;
			float  _Scale;
			float  _alpha;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
	
			struct v2f
			{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 noiseUV = i.uv;
				noiseUV += _Time*_speed;
				//ֻҪRG
				fixed2 noiseCol = tex2D(_noiseTex,noiseUV);
				fixed2 mainUV = i.uv;

				mainUV += noiseCol*0.2*_size*_Scale;

				fixed4 col = tex2D(_MainTex,mainUV);

				col.a = max(0, col.a);

				return col*i.color*fixed4(1,1,1,_alpha);
			}
			ENDCG
		}//end pass
	}//end SubShader

	SubShader
	{
		LOD 100

		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			//ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex]
			{
				Combine Texture * Primary
			}
		}//end pass
	}//end SubShader
}//end Shader