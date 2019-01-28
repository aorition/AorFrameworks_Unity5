//@@@DynamicShaderInfoStart
//<readonly> NGUI 自发光贴图材质 支持上色和亮度 (1字头带裁切)
//@@@DynamicShaderInfoEnd
Shader "AFW/NGUI/NGUI - Color 1" 
{

	Properties 
	{
		_MainTex ("Texture", 2D) = "white" { }
		_TintColor ("Color", Color) = (1,1,1,1)
		_Lighting ("Lighting",  float) = 1
 
		_CutOut("CutOut", float) = 0

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
			ZWrite Off
			ZTest[_zTest]
			Cull[_cull]

            CGPROGRAM

			#include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag
            
			#pragma multi_compile CLIP_OFF CLIP_ON
			#pragma shader_feature _FOG_ON
			#pragma multi_compile_fog

            sampler2D _MainTex;
            float4 _MainTex_ST;

			float _Lighting;
			fixed4 _TintColor;

			fixed _CutOut;

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

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex,i.uv);
                col = col * _TintColor * i.color;
				col.rgb *= _Lighting;
	 
                //先clip，再fog 不然会出错	
 				clip(col.a - _CutOut);
			
				//NGUI 1字头 -- 标配裁切算法
				float2 factor = (float2(1.0, 1.0) - abs(i.worldPos)) * _ClipArgs0;
				col.a *= min(max(min(factor.x, factor.y), 0.0), 1.0);
				//NGUI 1字头 -- 标配裁切算法 end
			
				return col;
            }

            ENDCG
        }//end pass
	}//end SubShader
}//end Shader