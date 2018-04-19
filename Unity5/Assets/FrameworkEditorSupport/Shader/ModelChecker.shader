//@@@DynamicShaderInfoStart
//<Readonly>模型检查Shader, 方便快捷查看模型的 VertexColor, Normal, WorldNormal, Tangent, UV, Lightmap 的情况
//@@@DynamicShaderInfoEnd

Shader "FrameworkTools/ModelChecker"
{
	Properties
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		[Enum(MainTex, 0, VertexColor, 1, Normal, 2, WorldNormal, 3, Tangent, 4, UV, 5, Lightmap, 6)] _checkType ("Check Type", Float) = 0
		[Enum(UnityEngine.Rendering.CullMode)] _cull("Cull Mode", Float) = 0
	}

	SubShader
	{

		Tags{ "Queue" = "Geometry" "RenderType" = "Geometry" }

		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			Lighting On
			
			Cull[_cull]
			
			CGPROGRAM

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile LIGHTMAP_OFF  LIGHTMAP_ON 
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
			#pragma exclude_renderers xbox360 ps3
			
			sampler2D _MainTex;
			//UV映射必要声明的参数, TRANSFORM_TEX(v.uv, _MainTex)会用到
			float4 _MainTex_ST;
			float _checkType;
			
			struct v2f
			{
				float4 pos : SV_POSITION;
				float2  uv : TEXCOORD0;
				
				#ifdef LIGHTMAP_ON
				float2  lightmapUV : TEXCOORD1;
				#endif
				
				float3  normal : TEXCOORD2;
				float4  tangent : TEXCOORD3;
				float3 worldPos : TEXCOORD4;
				
				float3	color : TEXCOORD5;
				
				LIGHTING_COORDS(6, 7)
				UNITY_FOG_COORDS(8)
			};
			
			v2f vert(appdata_full v)
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				
				#ifdef LIGHTMAP_ON
				o.lightmapUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				
				//o.normal = UnityObjectToWorldNormal(v.normal); // 模型坐标法线转换世界坐标法线
				o.normal = v.normal;
				
				o.tangent = v.tangent;
				
				o.worldPos = mul(unity_ObjectToWorld, v.vertex); // 模型坐标顶点转换世界坐标顶点
				
				o.color = v.color;

				UNITY_TRANSFER_FOG(o, o.pos);

				TRANSFER_VERTEX_TO_FRAGMENT(o);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				if(_checkType == 1) {
					return fixed4(i.color,1);
				}
				if(_checkType == 2){
					return fixed4(i.normal,1);
				}
				if(_checkType == 3){
					fixed3 worldNormal = normalize(UnityObjectToWorldNormal(i.normal)); // 法线方向
					return fixed4(worldNormal,1);
				}
				if(_checkType == 4){
					return i.tangent;
				}
				if(_checkType == 5){
					return fixed4(i.uv,0,1);
				} 
				if(_checkType == 6){
					#ifdef LIGHTMAP_ON
					fixed3 lightMapColor = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lightmapUV)).rgb;
					return fixed4(lightMapColor,1);
					#endif
					return 0;
				} 
				return tex2D(_MainTex, i.uv);
			}
			ENDCG
		}

	}
}