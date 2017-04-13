Shader "Custom/Fonts/SpriteUI CustomFont" {
	Properties {
		_MainTex ("Font Texture", 2D) = "white" {}
		_Color("Text Color", Color) = (1,1,1,1)
		_OutLineColor("OutLine Color", Color) = (1,1,1,1)
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
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				fixed4 tangent : TEXCOORD1;
			};

			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform fixed4 _Color;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color * _Color;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
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
		 
			fixed4 _OutLineColor;
			fixed4 _size;
			
			float _Gray;
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = i.color;
				float2 texcoord;
				fixed final = 0;
 
		 
				for (int j = 0; j < 16; j+= 1) {
					texcoord = i.texcoord;
					texcoord = float2(texcoord.x + samplesX[j]* _Offset*_size.x, texcoord.y + samplesY[j]* _Offset*_size.y);
					float blur = tex2D(_MainTex, texcoord).a;

 
					float blurA = step(i.tangent.x, texcoord.x) - step(i.tangent.z, texcoord.x) - step(texcoord.y, i.tangent.y) - step(i.tangent.w, texcoord.y);
					blur = min(blurA, blur);
					blur = max(blur, 0);
					final += blur;
				
				}
		
				final *= _OutLineColor.a;
			 	final = min(final, 1);
				_OutLineColor.a= final;

				float grey = dot(_OutLineColor.rgb, fixed3(0.22, 0.707, 0.071));
				_OutLineColor.rgb = lerp(_OutLineColor.rgb, fixed3(grey, grey, grey), _Gray);

					clip (final - 0.01);
					return _OutLineColor;
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
		};

		struct v2f {
			float4 vertex : SV_POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
			fixed4 tangent : TEXCOORD1;
		};

		sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform fixed4 _Color;

		v2f vert(appdata_t v)
		{
			v2f o;
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			o.color = v.color * _Color;
			o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.tangent = v.tangent;


#ifdef UNITY_HALF_TEXEL_OFFSET
			o.vertex.xy += (_ScreenParams.zw - 1.0)*float2(-1,1);
#endif
			return o;
		}

		float _Gray;

		fixed4 frag(v2f i) : SV_Target
		{
			fixed4 col = i.color;
		float2 texcoord = i.texcoord;
		float textA = tex2D(_MainTex, texcoord).a;
		
		texcoord = i.texcoord;
		float rectA = step(i.tangent.x,texcoord.x) - step(i.tangent.z, texcoord.x) - step(texcoord.y, i.tangent.y) - step(i.tangent.w, texcoord.y);
		col.a = min(rectA, textA);
		col.a = max(col.a, 0);

		float grey = dot(col.rgb, fixed3(0.22, 0.707, 0.071));
		col.rgb = lerp(col.rgb, fixed3(grey, grey, grey), _Gray);

		clip(col.a - 0.01);
		return col;
		}
			ENDCG
		}




	}
}
