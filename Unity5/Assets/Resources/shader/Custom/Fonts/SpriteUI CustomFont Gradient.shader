// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Fonts/SpriteUI CustomFont Gradient" {
	Properties {
		_MainTex ("Font Texture", 2D) = "white" {}
		_ColorT("Text Color Top", Color) = (1,1,1,1)
		_ColorB("Text Color Bottom", Color) = (1,1,1,1)
		_OutLineColorT("OutLine Color Top", Color) = (1,1,1,1)
        _OutLineColorB("OutLine Color Bottom", Color) = (1,1,1,1)
		_Offset("OutLineSize", Float) = 1
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
		_Gray("_Gray", Float) = 0
	}

	SubShader {

		Tags 
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}
		
		Lighting Off 
		Cull Off 
		ZTest [unity_GUIZTestMode]
		ZWrite Off 
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#pragma target 3.0

			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				fixed4 tangent : TANGENT;
				float2 texcoord : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
				fixed4 tangent : TEXCOORD2;
			};

			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform fixed4 _Color;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.vertex;// * _Color;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.texcoord1 = v.texcoord1;
				o.tangent = v.tangent;


#ifdef UNITY_HALF_TEXEL_OFFSET
				o.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
#endif
				return o;
			}


			static const float samplesX[16] =
			{
				-1,
				-0.75,
				-0.5,
				-0.25,
				0,
				0.25,
				0.5,
				0.75,
				1,
				0.75,
				0.5,
				0.25,
				0,
				-0.25,
				-0.5,
				-0.75,
			};

			static const float samplesY[16] =
			{
				0,
				0.25,
				0.5,
				0.75,
				1,
				0.75,
				0.5,
				0.25,
				0, 
				-0.25,
				-0.5,
				-0.75,
				-1,
				-0.75,
				-0.5,
				-0.25,
			};

			static const float samples[16] =
			{
				-1,
				-0.875,
				-0.75,
				-0.625,
				-0.5,
				-0.375,
				-0.25,
				-0.125,
				0.125,
				0.25,
				0.375,
				0.5,
				0.625,
				0.75,
				0.875,
				1,
			};

			static const float weigth[16] =
			{
				0.1,
				0.3,
				0.5,
				0.7,
				1,
				0.7,
				0.4,
				0.2,
				0.4,
				0.5,
				0.8,
				1,
				0.8,
				0.6,
				0.3,
				0.1,

			};


			float _Offset;
		 
			fixed4 _OutLineColorT;
            fixed4 _OutLineColorB;
			fixed4 _size;
			
			float _Gray;
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = i.color;
				float2 texcoord;
				float2 texcoord1;

                fixed finalA = 0;
				fixed4 final;
		 
				for (int j = 0; j < 16; j+= 1) {
                    texcoord1 = i.texcoord1;
					texcoord = i.texcoord;
					texcoord = float2(texcoord.x + samplesX[j]* _Offset*_size.x, texcoord.y + samplesY[j]* _Offset*_size.y);
					float blur = tex2D(_MainTex, texcoord).a;

 
					float blurA = step(i.tangent.x, texcoord.x) - step(i.tangent.z, texcoord.x) - step(texcoord.y, i.tangent.y) - step(i.tangent.w, texcoord.y);
					blur = min(blurA, blur);
					blur = max(blur, 0);
					finalA += blur;
				
				}

                final = _OutLineColorT * texcoord1.y + _OutLineColorB * (1 - texcoord1.y);

		        finalA *= _OutLineColorT.a * texcoord1.y + _OutLineColorB.a * (1 - texcoord1.y);
                finalA = min(finalA, 1);
                
                final.a = finalA;
				clip (finalA - 0.01);

                float grey = dot(final.rgb, fixed3(0.22, 0.707, 0.071));
				final.rgb = lerp(final.rgb, fixed3(grey, grey, grey), _Gray);

				return final;
			}
			ENDCG 
		}

		Pass
		{
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#pragma target 3.0

			struct appdata_t {
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			fixed4 tangent : TANGENT;
			float2 texcoord : TEXCOORD0;
			float2 texcoord1 : TEXCOORD1;
		};

		struct v2f {
			float4 vertex : SV_POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
			float2 texcoord1 : TEXCOORD1;
			fixed4 tangent : TEXCOORD2;
		};

		sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform fixed4 _ColorT;
		uniform fixed4 _ColorB;

		float _Gray;

		v2f vert(appdata_t v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.color = v.color * (_ColorT * v.texcoord1.y + _ColorB * (1 - v.texcoord1.y));
			o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.tangent = v.tangent;


#ifdef UNITY_HALF_TEXEL_OFFSET
			o.vertex.xy += (_ScreenParams.zw - 1.0)*float2(-1,1);
#endif
			return o;
		}

 

		fixed4 frag(v2f i) : SV_Target
		{
			fixed4 col = i.color;
		float2 texcoord = i.texcoord;
		float textA = tex2D(_MainTex, texcoord).a;
 


		texcoord = i.texcoord;
		float rectA = step(i.tangent.x,texcoord.x) - step(i.tangent.z, texcoord.x) - step(texcoord.y, i.tangent.y) - step(i.tangent.w, texcoord.y);
		col.a = min(rectA, textA);
		col.a = max(col.a, 0);

		clip(col.a - 0.01);
		
		float grey = dot(col.rgb, fixed3(0.22, 0.707, 0.071));
		col.rgb = lerp(col.rgb, fixed3(grey, grey, grey), _Gray);
		
		return col;
		}
			ENDCG
		}

	}
}
