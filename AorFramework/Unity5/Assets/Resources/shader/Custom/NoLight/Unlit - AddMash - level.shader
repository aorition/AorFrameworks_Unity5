// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/NoLight/Unlit - AddMash - level"
{
	Properties
	{
		_MainTex ("_MainTex (RGB), Alpha (A)", 2D) = "black" {}
		_moveTex ("_moveTex (RGB), Alpha (A)", 2D) = "black" {}
	//	_moveTex2 ("_moveTex2 (RGB), Alpha (A)", 2D) = "black" {}
		_noiseTex ("_noiseTex (RGB), Alpha (A)", 2D) = "black" {}
		_noiseScale("_noise", float) =1
			_level("_level", float) =1
	}
	
	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent+10"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
	//		Blend SrcAlpha OneMinusSrcAlpha
	Blend SrcAlpha One
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			sampler2D _moveTex;
			float4 _moveTex_ST;
			
				sampler2D _noiseTex;
			float4 _noiseTex_ST;
		float	_noiseScale;
			float	_level;
			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
	
			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed gray : TEXCOORD1; 
						half2 uv2 : TEXCOORD2;
	half2 uv3 : TEXCOORD3;
				fixed4 color : COLOR;
			};
	
			v2f o;

			v2f vert (appdata_t v)
			{
				o.vertex = UnityObjectToClipPos(v.vertex);
			//	o.texcoord = v.texcoord;
				    o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
						    o.uv2 = TRANSFORM_TEX(v.texcoord,_moveTex);	   
						     						    o.uv3 = TRANSFORM_TEX(v.texcoord,_noiseTex);	   
				o.color = v.color;
				o.gray =step( dot(v.color, fixed4(1,1,1,0)),0); 
				return o;
			}
				
			fixed4 frag (v2f IN) : COLOR
			{	  
						fixed4 col=tex2D(_MainTex,IN.texcoord);

 			  fixed4 noiseCol=tex2D(_noiseTex, IN.uv3);
 			  float2 noiseUV=IN.uv2+noiseCol.rgb*_noiseScale;
 			  fixed4 movecol=tex2D(_moveTex, noiseUV);
 		
 			  	//	col	+=movecol;
 			  		
movecol=lerp(fixed4(0,0,0,0),movecol,col.r);


 			  		clip(_level-IN.texcoord.y);
 			  return  movecol;
			}
			ENDCG
		}
	}

	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex]
			{
				Combine Texture * Primary
			}
		}
	}
}
