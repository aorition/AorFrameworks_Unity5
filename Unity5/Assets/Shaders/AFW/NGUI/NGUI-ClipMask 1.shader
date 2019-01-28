//@@@DynamicShaderInfoStart
//<readonly> NGUI 定义一张黑白图片作为Mask  (1字头带裁切) (注意：Shader须配合UITextture使用)
//@@@DynamicShaderInfoEnd
Shader "AFW/NGUI/NGUI - ClipMask 1" {

	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		_MaskTex("Mask Texture", 2D) = "white" {}
	}
	
	SubShader
	{
		LOD 200

		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		
		Pass
		{

			Lighting Off
			Fog{ Mode Off }
			Offset -1, -1
			Cull Off
			ZWrite Off
			//ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag		
			
			sampler2D _MainTex;
			sampler2D _MaskTex;

			//NGUI 1字头 -- 标配参数组
			float4 _ClipRange0 = float4(0.0, 0.0, 1.0, 1.0);
			float2 _ClipArgs0 = float2(1000.0, 1000.0);
			//NGUI 1字头 -- 标配参数组 end

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
				float2 worldPos : TEXCOORD2;
				fixed4 color : COLOR;
				fixed gray : TEXCOORD1;
			};
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.color = v.color;
				o.gray =step( dot(v.color, fixed4(1,1,1,0)),0); 
				o.worldPos = v.vertex.xy * _ClipRange0.zw + _ClipRange0.xy;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{

				float4 mask = tex2D(_MaskTex,i.uv);
			
				float4 col = tex2D(_MainTex,i.uv) * i.color;
				fixed4 col2 =  dot(col.rgb, float3(0.299, 0.587, 0.114));
				col2.a=col.a* i.color.a;
				col=lerp(col* i.color,col2, i.gray);

				col.a *= mask.r;

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
