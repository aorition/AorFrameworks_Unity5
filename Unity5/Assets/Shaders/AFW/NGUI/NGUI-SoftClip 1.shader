Shader "AFW/NGUI/NGUI - SoftClip 1"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		_CenterOffset ("Center Offset", float) = 1
		_Softness("Softness", float) = 120
		_width("Width", float) =  0
	}

	SubShader
	{
		LOD 200

		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Offset -1, -1
			Fog { Mode Off }
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			half _CenterOffset;
			half _Softness;
			half _width;

			//NGUI 1字头 -- 标配参数组
			float4 _ClipRange0 = float4(0.0, 0.0, 1.0, 1.0);
			float2 _ClipArgs0 = float2(1000.0, 1000.0);
			//NGUI 1字头 -- 标配参数组 end

			struct appdata_t
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : POSITION;
				half4 color : COLOR;
				float2 uv : TEXCOORD0;
				float2 worldPos : TEXCOORD1;
				fixed gray : TEXCOORD2;
			};

			v2f o;

			v2f vert (appdata_t v)
			{
				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.uv = v.texcoord;
				o.worldPos = v.vertex.xy * _ClipRange0.zw + _ClipRange0.xy;
				o.gray =step( dot(v.color, fixed4(1,1,1,0)),0); 
				return o;
			}

			half4 frag(v2f i) : SV_Target
			{
				// Sample the texture
				half4 col = tex2D(_MainTex, i.uv) * i.color;

				//渐变
				float2 uv = i.uv.xy;
				col.a = min(1 - pow((abs(uv.y - _CenterOffset) *_width), _Softness), col.a);

				fixed4 col2 = dot(col.rgb, float3(0.299, 0.587, 0.114));
				col2.a = col.a* i.color.a;
				col = lerp(col* i.color, col2, i.gray);

				col.a = max(0, col.a);

				//NGUI 1字头 -- 标配裁切算法
				float2 factor = (float2(1.0, 1.0) - abs(i.worldPos)) * _ClipArgs0;
				col.a *= min(max(min(factor.x, factor.y), 0.0), 1.0);
				//NGUI 1字头 -- 标配裁切算法 end

				return col;
			}
			ENDCG
		}//end pass
	}//end SubShader
	
	SubShader
	{
		LOD 100

		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex]
			{
				Combine Texture * Primary
			}
		}//end pass
	}//end SubShader
}//end Shader