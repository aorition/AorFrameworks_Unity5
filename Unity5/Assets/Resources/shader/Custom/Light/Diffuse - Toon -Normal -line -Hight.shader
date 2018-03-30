// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'



Shader "Custom/Light/Diffuse - Toon -Normal -line -Hight" {
	Properties{
	_MainTex("MainTex", 2D) = "white" {}
	_MaskTex("Mask Tex", 2D) = "black" {}
	_NormalTex("NormalTex", 2D) = "white" {}
//	_OutlineColor("Outline Color", Color) = (0,0,0,1)

	//[HideInInspector]_LowOutlineWidth("LowOutlineWidth", float) = 4
	_HightOutlineWidth("HightOutlineWidth", float) = 2

	//_ToonShade("ToonShader", 2D) = "white" {}
	_HideFace("_HideFace", int) = 0
	[Toggle] _Fog("Fog?", Float) = 0
	_Color("Color", Color) = (1,1,1,1)
	[HideInInspector]_Lighting("Lighting", float) = 1
	[HideInInspector] _CutOut("CutOut", float) = 0.1
	}
		SubShader{
			Tags {
			"RenderType" = "Transparent"
			}


		LOD 600
		pass {
		Name "LINEPASS"
		Tags{ "LightMode" = "Always" }
			Cull Front
			ZWrite On
			CGPROGRAM

#pragma vertex vert
#pragma fragment frag
#include "Assets/ObjectBaseShader.cginc"	

 
			float _HideFace;
			float _HightOutlineWidth;
			float _Factor=0.5;
			

		struct v2f {
			float4 pos:POSITION;
			//float2 uv0 : TEXCOORD0;
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
			v.vertex.xyz += dir*_HightOutlineWidth*0.001* min(3,far);

			o.pos = UnityObjectToClipPos(v.vertex) ;
		
			//o.uv0 = TRANSFORM_TEX(v.texcoord, _MainTex);
		 //	o.pos /= o.pos.w;
			//   o.uv0.x = o.pos.w;
			return o;
		}
		float4 frag(v2f i) :COLOR
		{ 
		 //	 return   i.uv0.x;
		//	fixed4 c = tex2D(_MainTex, i.uv0);
			//c.rgb = lerp(fixed3(0,0,0),i.color*i.color*_Color,0.8);
			//c.rgb *= _HdrIntensity + _DirectionalLightColor*_DirectionalLightDir.w + c.rgb*UNITY_LIGHTMODEL_AMBIENT.xyz;
			clip(i.color.a + 0.2 - _HideFace);
			//c.a = 1;
			return fixed4(0,0,0,1);
		}
			ENDCG
	}//end of pass

			Pass {
				Name "COLORPASS"
				Tags { "LightMode" = "ForwardBase" }
				CGPROGRAM
				#pragma multi_compile CLIP_OFF CLIP_ON
				#pragma shader_feature _FOG_ON
	 
				#pragma multi_compile_fog

				#pragma vertex vert
				#pragma fragment frag
				#include "Assets/ObjectBaseShader.cginc"	
 

					float _UILight;
					float _HideFace;
					//sampler2D _ToonShade;
					sampler2D _MaskTex;
					sampler2D _NormalTex;

					struct v2f {
						float4 pos : SV_POSITION;
						float2 uv0 : TEXCOORD0;
						half3 normal   : TEXCOORD1;
						half3 eyeDir   : TEXCOORD3;
						fixed3 lightColor : TEXCOORD4;
						half4 Tangent : TEXCOORD5;
						half3 BiNormal : TEXCOORD6;
						half4 color : COLOR;
					#if _FOG_ON
						UNITY_FOG_COORDS(2)
					#endif
					};

					v2f vert(appdata_full v) {
						v2f o;
						o.uv0 = TRANSFORM_TEX(v.texcoord,_MainTex);
						o.pos = UnityObjectToClipPos(v.vertex);
						o.color = v.color;
						o.normal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
						half4 worldPos = mul(unity_ObjectToWorld, v.vertex);
					 	o.eyeDir = normalize(worldPos  - _WorldSpaceCameraPos.xyz);
					 
						o.lightColor = Shade4PointLights(worldPos, o.normal);
						o.Tangent = normalize(float4(mul(v.tangent.xyz, (float3x3)unity_WorldToObject), v.tangent.w));
						o.BiNormal = cross(o.normal, o.Tangent) * o.Tangent.w;
					//	o.worldvertpos = ObjSpaceViewDir(v.vertex).xyz;
					#if _FOG_ON
						UNITY_TRANSFER_FOG(o, o.pos);
					#endif

						return o;

						}
					fixed4 frag(v2f i) : COLOR {

						//控制贴图
						half4 mask = tex2D(_MaskTex, i.uv0);
					 //	_UILight =1;
						//主光方向
						half3	lightDirection = lerp(_RoleDirectionalLightDir.xyz, half3(0.6, 0.3, -1), _UILight);

						//法线
						half3 normal = i.normal;
						fixed3 normalMap = UnpackNormal(tex2D(_NormalTex, i.uv0));
						normalMap = normalize(normalMap);
						half3x3 tbnMatrix = half3x3(i.Tangent.xyz, i.BiNormal, normal);
						normal = normalize(mul(normalMap.xyz, tbnMatrix));

						//卡通分界线
						half d = max(0, dot(normal, normalize( lightDirection))-(1-mask.r));
					 d=lerp(d,1,mask.b);


						half diff = pow(max(0, dot(normal, normalize(lightDirection))),2) * 0.15;
						 
						//高光
						 half splight = max(pow(saturate(dot(normal , normalize(lightDirection.xyz - i.eyeDir))),  i.color.b * 16 + 2),0);


						 //顶光
						 half topD = max(0, dot(normal, normalize(half3(0,1,0))));
						
						  topD = pow(topD, 3) ;
						  topD = step( 0.6,topD);
						 
						 //背光
						 half backlight =saturate(-dot(normal, normalize(lightDirection.xyz-half3(-0.3,0,0.5)))) ;
						  backlight = step( 0.2, backlight);
						 fixed3 backCol = fixed3(0.2, 0.1, 0.3)*backlight;
				
							  
						//固有色
					//	fixed4 col = tex2D(_MainTex, i.uv0);
						 
						//环境光
						fixed3	ambientLight = UNITY_LIGHTMODEL_AMBIENT.xyz;


						half3 ramp = tex2D(_MainTex, float2(  d,1- mask.g)).rgb;
						//return mask.r;
						 
						//最终合成
						half4 final;
						final.a =1;
					 
						//UI界面没辉光,用高光补强 
						// ambientLight.rgb 
					 
						half3 mainlight = lerp((ramp*_DirectionalLightDir.w*_DirectionalLightColor.rgb ), ramp, _UILight) + i.lightColor.rgb + ramp* diff*_DirectionalLightColor;


						half3 topCol = tex2D(_MainTex, float2(1, 1 - mask.g)).rgb;
						///topCol=lerp(topCol,fixed3(0,0,0),);
						//return fixed4(topCol*topD, 1);
						mainlight =lerp(mainlight, topCol*topD+0.3, topD*i.color.r);
						
						final.rgb = mainlight*_Color.rgb;
						
						//return mask.a;
						clip(i.color.a + 0.2 - _HideFace);
						#ifdef CLIP_ON
						clip(final.a - _CutOut);
						#endif
						#if _FOG_ON
						  UNITY_APPLY_FOG(i.fogCoord, final);
						#endif
						topCol=topCol*0.2;
					 
						final.rgb =lerp(topCol,final.rgb, mask.a);
						return  final;

							}
						ENDCG
						}

 

	}
	SubShader{
		Tags{
		"RenderType" = "Transparent"
	}

		LOD 200

		UsePass "Custom/Light/Diffuse - Toon/BASETOON"

	}
}