//@@@DynamicShaderInfoStart
//<readonly> NGUI 裁切粒子
//@@@DynamicShaderInfoEnd
Shader "AFW/NGUI/NGUI - ClipParticle" 
{
	Properties 
	{
		_MainTex ("Texture", 2D) = "white" { }
		_TintColor("Color", Color) = (1,1,1,1)
		_Lighting ("Lighting",  float) = 1
		
		[HideInInspector]_CutOut("CutOut", float) = 0.1

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

            sampler2D _MainTex;
            float4 _MainTex_ST;

			float _Lighting;
            fixed4 _TintColor;

			float4 _ClipArea;    // ( min x ,  max x ,  min y ,  max y )
			
			struct a2v {
				float4 vertex : POSITION;
				float2 texcoord:TEXCOORD0;
				float4 color : COLOR;
			};

            struct v2f {
                float4  pos : SV_POSITION;
                float2  uv : TEXCOORD0;
                float4 color : COLOR;
				float2 worldPos : TEXCOORD1;
            };

            v2f vert (a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
                o.color=v.color;

			    o.worldPos = v.vertex.xy;
 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c= tex2D(_MainTex,i.uv);
                c = c * _TintColor * i.color;
				c.rgb*=_Lighting;

				bool inArea = i.worldPos.x > _ClipArea.x && i.worldPos.x < _ClipArea.y && 
					i.worldPos.y > _ClipArea.z && i.worldPos.y < _ClipArea.w;
				return inArea ? c : fixed4(0, 0, 0, 0);

            }

            ENDCG

		}//end pass
		
	}//end SubShader

}//end Shader
