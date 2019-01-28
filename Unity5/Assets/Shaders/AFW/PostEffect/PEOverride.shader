Shader "Hidden/PEOverride"
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
		
		Cull Off 
		ZWrite On 
		ZTest Always

		//Blend SrcAlpha OneMinusSrcAlpha

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
				//col.a = 1; //强制写主RT的Alpha为1;
				fixed4 over = tex2D(_EffectTex, i.uv) * _Power;

				fixed4 final;
				final.rgb = lerp(col.rgb, over.rgb, over.a);
				final.a = min(col.a + over.a, 1);
				return final;
			}
			ENDCG
		}//end pass
	}//end SubShader
}//end Shader
