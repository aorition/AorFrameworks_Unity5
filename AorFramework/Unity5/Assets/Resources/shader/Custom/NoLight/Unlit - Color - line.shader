// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//@@@DynamicShaderInfoStart
//自发光贴图材质 支持上色和亮度
//@@@DynamicShaderInfoEnd


//@@@DynamicShaderTitleRepaceStart
Shader "Custom/NoLight/Unlit - Color - line" {
//@@@DynamicShaderTitleRepaceEnd

Properties {
 _MainTex ("Texture", 2D) = "white" { }
_TintColor("Color", Color) = (1,1,1,1)
_Lighting("Lighting",  float) = 1
[Toggle] _Fog("Fog?", Float) = 0

_OutlineWidth("OutlineWidth", float) = 1
_OutlineColor("OutLineColor", Color) = (0,0,0,1)

[Toggle] _HasNight("HasNight?", Float) = 0    //夜晚效果开关

[HideInInspector]_CutOut("CutOut", float) = 0.1
[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4
[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend Mode", Float) = 5
[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend Mode", Float) = 10
[Enum(UnityEngine.Rendering.BlendMode)] _SrcAlphaBlend("Src Alpha Blend Mode", Float) = 1
[Enum(UnityEngine.Rendering.BlendMode)] _DstAlphaBlend("Dst Alpha Blend Mode", Float) = 10
[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2
}


	SubShader {
 
  
	Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
	
	pass {
		Tags{ "LightMode" = "Always" }
		Cull Front
		ZWrite On
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "Assets/ObjectBaseShader.cginc"	


		float _HideFace;
	float _OutlineWidth;
	float _Factor = 0.5;

	fixed4 _OutlineColor;

	struct v2f {
		float4 pos:POSITION;
		float2 uv0 : TEXCOORD0;
		half4 color : COLOR;
	};

	v2f vert(appdata_full v) {
		v2f o;
		o.color = v.color;
		float far = UnityObjectToClipPos(v.vertex).w;
		float3 dir = normalize(v.vertex.xyz);
		float3 dir2 = v.normal;
		float D = dot(dir,dir2);
		dir = dir*sign(D);
		dir = dir*_Factor + dir2*(1 - _Factor);
		v.vertex.xyz += dir*_OutlineWidth*0.0004* min(3,far);

		o.pos = UnityObjectToClipPos(v.vertex);

		o.uv0 = TRANSFORM_TEX(v.texcoord, _MainTex);
		//	o.pos /= o.pos.w;
		//   o.uv0.x = o.pos.w;
		return o;
	}
	float4 frag(v2f i) :COLOR
	{
		return _OutlineColor;
	}
		ENDCG
}//end of pass

	
	Pass
    {


	Blend[_SrcBlend][_DstBlend],[_SrcAlphaBlend][_DstAlphaBlend]
	ZWrite Off
	ZTest[_ZTest]
	Cull[_Cull]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#include "Assets/ObjectBaseShader.cginc"
       		#pragma multi_compile CLIP_OFF CLIP_ON
       		#pragma shader_feature _HASNIGHT_ON
			#pragma shader_feature _FOG_ON
			#pragma multi_compile_fog

 

            struct v2f {
                float4  pos : SV_POSITION;
                float2  uv : TEXCOORD0;
                float4 color : COLOR;
				#if _FOG_ON
				UNITY_FOG_COORDS(2)
				#endif

            }

;
            struct appdata {
                float4 vertex : POSITION;
                float2 texcoord:TEXCOORD0;
                float4 color : COLOR;
            }

;
            //顶点函数没什么特别的，和常规一样 
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
                o.color=v.color;

				#if _FOG_ON
				UNITY_TRANSFER_FOG(o, o.pos);
				#endif
                return o;
            }


            
             fixed4 _TintColor;
            float4 frag (v2f i) : COLOR
            {
                float4 col= tex2D(_MainTex,i.uv);
                
				//col = col* _TintColor*i.color;
				col.rgb *= _Lighting;

				float isGray = step(dot(_TintColor.rgb, fixed4(1, 1, 1, 0)), 0);
				
				float3 grayCol = dot(col.rgb, float3(0.299, 0.587, 0.114));
			
				col.rgb = lerp(col.rgb* _TintColor*i.color, grayCol.rgb, isGray);

				#ifdef _HASNIGHT_ON
					col.rgb*= _HdrIntensity+_DirectionalLightColor*_DirectionalLightDir.w+ col.rgb*UNITY_LIGHTMODEL_AMBIENT.xyz;    //夜晚效果
				#endif

	 
					//先clip，再fog 不然会出错	
 				#ifdef CLIP_ON
					clip(  col.a-_CutOut);
				#endif
			


				#if _FOG_ON
					UNITY_APPLY_FOG(i.fogCoord, col);
				#endif
				col.a*=_TintColor.a*i.color.a;
				return col;
            }


            ENDCG
        }
        

}
}