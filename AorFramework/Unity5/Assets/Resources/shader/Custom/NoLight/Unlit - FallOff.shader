// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/NoLight/FallOff" {
	Properties{
	_TintColor("TintColor", color) = (0.2, 0.5, 1,1)
	 _Alpha("Alpha", float) = 1
	 _Lighting("Light", float) = 1
	}


		SubShader
	{
		// AlphaTest Greater .2
			 Blend One OneMinusSrcAlpha
			 //BlendOp Max
			 Lighting Off ZWrite Off
		 pass
		 {
			 CGPROGRAM
			 #pragma vertex vert
			 #pragma fragment frag
			 #include "UnityCG.cginc"

			float4 _TintColor;
			float _Alpha;
			float _Lighting;
			 struct v2f {

				 float4  pos : SV_POSITION;
				 float3 normal : TEXCOORD0;
				 float2  uv : TEXCOORD2;
				  float3 worldvertpos : TEXCOORD1;

			 };

			 //顶点函数没什么特别的，和常规一样
			 v2f vert(appdata_base v)
			 {
				 v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);

					o.normal = v.normal;
					//  o.uv =    TRANSFORM_TEX(v.texcoord,_MainTex);
					   o.uv = float2(0, 0);
						o.worldvertpos = ObjSpaceViewDir(v.vertex).xyz;
					return o;
				}


				float4 frag(v2f i) : COLOR
				{

					i.normal = normalize(i.normal);
					   float3 viewdir = normalize(i.worldvertpos);

						float4 texCol = 1;

						texCol.a = min(pow((1 - saturate(dot(viewdir, i.normal))), 3), 1);
						 texCol.rgb = texCol.a * 3 * _TintColor.xyz;
						 texCol.a /= 4 * _Alpha;

						   return texCol*_Lighting;

				}
				ENDCG
			}
	}
}