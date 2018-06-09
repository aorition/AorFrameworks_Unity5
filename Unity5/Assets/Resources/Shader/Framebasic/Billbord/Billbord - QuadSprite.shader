//@@@DynamicShaderInfoStart
//支持自定义图集的Billbord
//@@@DynamicShaderInfoEnd


Shader "FrameBase/Billbord/Billbord - QuadSprite" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SpritePos ("SpritePos", Vector) = (0,0,1024,1024)
		_SourceSize ("SourceSize", Vector) = (0,0,1024,1024)
		_AtlasSize ("AtlasSize", Vector) = (1024,1024,0,0)
		_Color ("Color", Color) = (1, 1, 1, 1)
	}

	SubShader {
		Pass{
			Tags { "RenderType"="Transparent" }

			Blend SrcAlpha OneMinusSrcAlpha
			LOD 200
			CGPROGRAM
 
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Assets/ObjectBaseShader.cginc"

			struct v2f {
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD2;
				
 
			};

			float4 _SpritePos;
			float4 _SourceSize;
			float4 _AtlasSize;


			v2f vert (appdata_base v)
			{
			    v2f o;

				float4x4 mvMat = UNITY_MATRIX_MV;
				float3 scale =  float3(length(float3(mvMat[0][0], mvMat[0][1], mvMat[0][2])), length(float3(mvMat[1][0], mvMat[1][1], mvMat[1][2])), length(float3(mvMat[2][0], mvMat[0][1], mvMat[0][2])));
				mvMat = float4x4(scale.x, 0, 0, mvMat[0][3],
											 0, scale.y, 0, mvMat[1][3],
											 0, 0, scale.z, mvMat[2][3],
											 0, 0, 0, 1);
				float4x4 mvpMat = mul(UNITY_MATRIX_P, mvMat);

			    o.pos = mul(mvpMat,v.vertex);
			  	o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
 
				return o;
			}
			      
			float4 frag (v2f i) : COLOR
			{
				float2 texSize = _AtlasSize.xy;//float2(1024, 1024);
				float2 uv = i.uv;

				uv.y = 1- uv.y;

				float2 offsetSrc = _SourceSize.xy / _SourceSize.zw;
				float2 scaleSrc = _SpritePos.zw / _SourceSize.zw;
				uv = (uv - offsetSrc) / scaleSrc;
				uv = uv;

				float outRange = min(min(uv.x, uv.y), (-max(uv.x, uv.y) + 1));
				clip(outRange);
				

				float2 offset = _SpritePos.xy / texSize;
				float2 scale = _SpritePos.zw / texSize;
				uv = (uv * scale + offset);

				uv.y = 1- uv.y;
				

				float4 mainCol = tex2D(_MainTex, uv);
				if (mainCol.a < 0.1)
				{
					discard;
				}
 
				return mainCol * _Color;
			}
			ENDCG
		}
	} 
}
