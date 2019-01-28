Shader "AFW/NGUI/NGUI - ScreenChange 1" 
{
	
	Properties{}
	
	SubShader
	{

	    Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
		{
			
			ZWrite Off
			ZWrite Off
			Cull Off
			Lighting Off
			ZWrite Off
			Fog{ Mode Off }
			Offset -1, -1
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _Color;

			float _Tiling;
			float _Scale1;
			float _Scale2;

			//NGUI 1字头 -- 标配参数组
			float4 _ClipRange0 = float4(0.0, 0.0, 1.0, 1.0);
			float2 _ClipArgs0 = float2(1000.0, 1000.0);
			//NGUI 1字头 -- 标配参数组 end

			struct a2v {
                float4 vertex : POSITION;
                float2 texcoord:TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f {
                float4  pos : SV_POSITION;
                float2  uv : TEXCOORD0;
				float2 worldPos : TEXCOORD2;
                float4 color : COLOR;
            };

			//顶点函数没什么特别的，和常规一样 
            v2f vert (a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
                o.color=v.color;
				o.worldPos = v.vertex.xy * _ClipRange0.zw + _ClipRange0.xy;
                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv) * _Color;

				float2 uvdir = float2(1 - i.uv.x, i.uv.y);
				float2 mUv = uvdir - floor(uvdir * _Tiling) / _Tiling * _Scale2;
				float blockWidth = _Scale1 / _Tiling * _Scale2;
				fixed m = step(mUv.x, blockWidth) * step(mUv.y, blockWidth);
				c = lerp(fixed4(0,0,0,0), c, m);

				//NGUI 1字头 -- 标配裁切算法
				float2 factor = (float2(1.0, 1.0) - abs(i.worldPos)) * _ClipArgs0;
				c.a *= min(max(min(factor.x, factor.y), 0.0), 1.0);
				//NGUI 1字头 -- 标配裁切算法 end

				return c;
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
			Fog{ Mode Off }
			Offset -1, -1
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse

			SetTexture[_MainTex]
		    {
				Combine Texture * Primary
			}
		}//end pass
	}//end SubShader
}//end Shader