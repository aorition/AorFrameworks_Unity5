Shader "Hidden/PEOverrideAdd"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_EffectTex("Effect RT", 2D) = "white" {}
		_Power("Power", float) = 1
	}

	SubShader
	{

		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True"}

		// No culling or depth
		Cull Off 
		ZWrite Off 
		ZTest Always

		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			float _Power;
			sampler2D _MainTex;
			sampler2D _EffectTex;
			
			fixed4 frag (v2f i) : SV_Target
			{

				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 over = tex2D(_EffectTex, i.uv) * _Power;
				col += over;
				return col;
			}
			ENDCG
		}//end pass
	}//end SubShader
}//end Shader