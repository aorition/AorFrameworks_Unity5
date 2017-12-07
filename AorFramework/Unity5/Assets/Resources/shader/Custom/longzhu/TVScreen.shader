// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Custom/longzhu/TVScreen" {

	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Lighting("Lighting",  float) = 1
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_SizeX ("Size X", float) = 8
		_SizeY ("Size Y", float) = 16
		_Speed ("Speed", float) = 100

		_TvFlickSpeed("Flicker Speed", range(0,50)) = 5
		_TvFlickLevel("Flicker Level", range(0,1)) = 0.1
		_TvScrollSpeed("Scrolling Speed", range(0,2)) = 0.04
		_TvScrollTiling("Scrolling Tiling", range(0,25)) = 8
		_TvScrollTex("Scrolling Texture", 2D) = "white" {}
		_TvMask("Mask", 2D) = "white" {}
	}

	SubShader {
		Tags{ "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass {
			ZTest Always

			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Assets/ObjectBaseShader.cginc"

			float _SizeX;
		    float _SizeY;
			float _Speed;

			float _TvFlickSpeed;
			float _TvFlickLevel;
			float _TvScrollSpeed;
			float _TvScrollTiling;
			sampler2D _TvScrollTex;
			sampler2D _TvMask;


			struct v2f {
			    float4  pos : SV_POSITION;
			    float2  uv : TEXCOORD0;
            };

		    v2f vert(appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

				return o;
		    }

			fixed4 frag(v2f i): COLOR {
				int index = floor(_Time.x * _Speed);
				int indexY = floor(_SizeY - (index+1)/_SizeX);
				int indexX = index - indexY * _SizeX;
				float2 mUv = float2(i.uv.x/_SizeX, i.uv.y/_SizeY);
				mUv.x += indexX / _SizeX;
				mUv.y += indexY / _SizeY;

				fixed4 TvCol = tex2D(_TvScrollTex, float2(i.uv.x, i.uv.y*_TvScrollTiling - _TvScrollSpeed * _Time.y));
				float flicker = lerp(1, sin(_Time.y* _TvFlickSpeed), _TvFlickLevel);

				fixed4 TvMask = tex2D(_TvMask, i.uv);

				fixed4 c = tex2D(_MainTex, mUv) * _Color * _Lighting;
				c *= TvCol * TvMask;
				c *= flicker;

			    return c;
			}
			ENDCG

		}
		
	}

}
