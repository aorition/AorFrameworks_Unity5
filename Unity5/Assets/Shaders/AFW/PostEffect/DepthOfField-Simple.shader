Shader "Hidden/PostEffect/DepthOfField - Simple"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		//_HighQuality_ON
		[Toggle] _HighQuality("High Quality?", Float) = 0
	}

	SubShader 
	{
		
		Pass 
		{
			
			ZTest Always 
			Cull Off 
			ZWrite Off
			//Fog { Mode off }
              
			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert_img
			#pragma fragment frag
			//#pragma fragmentoption ARB_precision_hint_fastest 
			
			#pragma shader_feature _HIGHQUALITY_ON

			uniform sampler2D _MainTex;
			uniform sampler2D _CameraDepthTexture; //接收Unity系统生成的深度贴图
			uniform half _OffsetDistance;
			uniform half focalDistance01;

			fixed4 frag (v2f_img i) : SV_Target
			{
				half blurFactor;
				half4 I = tex2D(_MainTex, i.uv);
				//fixed4 original = tex2D(_MainTex, i.uv);
				half4 C = tex2D(_MainTex, i.uv);
				C =tex2D(_MainTex,half2(i.uv.x+_OffsetDistance,i.uv.y+_OffsetDistance));
				C+=tex2D(_MainTex,half2(i.uv.x+_OffsetDistance,i.uv.y-_OffsetDistance));
				C+=tex2D(_MainTex,half2(i.uv.x-_OffsetDistance,i.uv.y+_OffsetDistance));
				C+=tex2D(_MainTex,half2(i.uv.x-_OffsetDistance,i.uv.y-_OffsetDistance));
            
				C+=tex2D(_MainTex,half2(i.uv.x+_OffsetDistance,i.uv.y));
				C+=tex2D(_MainTex,half2(i.uv.x,i.uv.y+_OffsetDistance));
				C+=tex2D(_MainTex,half2(i.uv.x-_OffsetDistance,i.uv.y));
				C+=tex2D(_MainTex,half2(i.uv.x,i.uv.y-_OffsetDistance));
				
				#ifdef _HIGHQUALITY_ON

					C+=tex2D(_MainTex,half2(i.uv.x+_OffsetDistance * 0.5,i.uv.y+_OffsetDistance* 0.5));
					C+=tex2D(_MainTex,half2(i.uv.x+_OffsetDistance* 0.5,i.uv.y-_OffsetDistance* 0.5));
					C+=tex2D(_MainTex,half2(i.uv.x-_OffsetDistance* 0.5,i.uv.y+_OffsetDistance* 0.5));
					C+=tex2D(_MainTex,half2(i.uv.x-_OffsetDistance* 0.5,i.uv.y-_OffsetDistance* 0.5));
            
					C+=tex2D(_MainTex,half2(i.uv.x+_OffsetDistance* 0.5,i.uv.y));
					C+=tex2D(_MainTex,half2(i.uv.x,i.uv.y+_OffsetDistance* 0.5));
					C+=tex2D(_MainTex,half2(i.uv.x-_OffsetDistance* 0.5,i.uv.y));
					C+=tex2D(_MainTex,half2(i.uv.x,i.uv.y-_OffsetDistance* 0.5));

					C *= 0.0625;

				#else

					C*=0.125;
				
				#endif

				//return C;
				float d = UNITY_SAMPLE_DEPTH ( tex2D (_CameraDepthTexture, i.uv) ); // i.uv.xy??
				d = Linear01Depth (d);

				//return d;

				blurFactor=saturate(abs(d-focalDistance01));
					return lerp(I,C,blurFactor);
			}
			ENDCG
		}//end pass
	}//end SubShader
	Fallback off
}//end Shader
