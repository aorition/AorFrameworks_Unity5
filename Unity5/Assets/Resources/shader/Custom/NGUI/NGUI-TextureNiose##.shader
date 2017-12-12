// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//注意：Shader须配合UITextture使用
//@@@DynamicShaderInfoStart
//根据另外一张贴图的RGB扭曲最终效果
//@@@DynamicShaderInfoEnd

//@@@DynamicShaderTitleRepaceStart
Shader "Custom/NGUI/NGUI-TextureNoise##" {
//@@@DynamicShaderTitleRepaceEnd

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

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Pass
		{
			Lighting Off
			Fog{ Mode Off }
			Offset -1, -1
			//@@@DynamicShaderBlendRepaceStart
			Cull Off
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			//@@@DynamicShaderBlendRepaceEnd
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
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
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
	
			v2f o;

			v2f vert (appdata_t v)
			{
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.color = v.color;
				return o;
			}
			
			float _Rotation;
			fixed4 frag (v2f IN) : SV_Target
			{
				float2 noiseUV = IN.texcoord;
				noiseUV += _Time*_speed;
				//只要RG
				fixed2 noiseCol = tex2D(_noiseTex,noiseUV);
				fixed2 mainUV = IN.texcoord;

				mainUV += noiseCol*0.2*_size*_Scale;

				fixed4 col = tex2D(_MainTex,mainUV);

				col.a = max(0, col.a);

				return col*IN.color*fixed4(1,1,1,_alpha);

			}
			ENDCG
		}
	}

	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
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
		}
	}
}
