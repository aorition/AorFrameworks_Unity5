Shader "AFW/Toon/Toon - Simple"
{
	Properties
	{
		//TOONY COLORS
		_Color("Color", Color) = (1.0,1.0,1.0,1.0)
		_HColor("Highlight Color", Color) = (0.98,0.98,0.98,1.0)
		_SColor("Shadow Color", Color) = (0.5,0.5,0.5,1.0)
		_DarkColor("DarkColor",Color) = (0,0,0,1.0)
		_SkinShadowColor("SkinShadowColor",Color) = (0,0,0,1)
		//DIFFUSE
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
		_MaskTex ("Mask (Specular)", 2D) = "white" {}
		//TOONY COLORS RAMP
		_RampThreshold ("#RAMPF# Ramp Threshold", Range(0,1)) = 0.5
		_RampSmooth ("#RAMPF# Ramp Smoothing", Range(0.001,1)) = 0.1
		
		//SPECULAR
		_SpecColor ("#SPEC# Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("#SPEC# Shininess", Range(0.0,2)) = 0.1
		_SpecSmooth ("#SPECT# Smoothness", Range(0,1)) = 0.05

		//OUTLINE
		_OutlineWidth("OutlineWidth", Float) = 1
		_OutLineColor("OutLineColor", Color) = (0,0,0,1)
		//灰度
		_is_gray("是否使用灰度",		Float) = 0
		_gray_factor("使用灰度时的亮度",	Float) = 0.5
	}

	SubShader
	{

		Tags{ "Queue" = "Geometry" "RenderType" = "Opaque" "IgnoreProjector" = "False" "LightMode" = "ForwardBase" }
	
		//Lighting Off
		Fog{ Mode Off }
		Blend Off
		ZTest Lequal

		Pass
		{
			Name "Base"

			Cull Back
			ZWrite On
			
			CGPROGRAM
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			
			fixed4 _Color;
			sampler2D _MainTex;
			fixed4 _MainTex_ST;															// 主贴图UV信息
			sampler2D _MaskTex;

			fixed _Shininess;
			fixed4 _HColor;
			fixed4 _SColor;
			fixed4 _SkinShadowColor;
			float _RampThreshold;
			float _RampSmooth;
			fixed _SpecSmooth;
			fixed4 _DarkColor;
			float _is_gray;																// 是否使用灰度
			float _gray_factor;															// 使用灰度时的亮度

			struct a2v 
			{
				float4	vertex : POSITION;															// 位置
				float4	normal : NORMAL;																// 法线
				float4	texcoord : TEXCOORD0;															// 纹理坐标0
			};

			struct v2f 
			{
				float4 pos:SV_POSITION;
				float2 uv:TEXCOORD0;
				float3 Normal:TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 viewDir : TEXCOORD3;
			};

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);								// 主贴图UV
				o.Normal = mul((float3x3)unity_ObjectToWorld,v.normal);															// 对象空间法线
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				fixed3 worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
				o.viewDir = normalize(_WorldSpaceCameraPos.xyz - o.worldPos.xyz);
				//fixed3 worldNormal = mul((float3x3)_Object2World, i.normal);											// 对象世界法线
				return o;
			}

			fixed4 frag(v2f i) :SV_Target
			{
				i.Normal = normalize(i.Normal);
				fixed4 mainTex = tex2D(_MainTex, i.uv);
				fixed4 mask1 = tex2D(_MaskTex, i.uv);
				fixed skinShadowAmount = (mask1.r - 0.5) * 2;
				fixed3 Albedo = mainTex.rgb * _Color.rgb;
				fixed Alpha = mainTex.a * _Color.a;
				fixed Gloss = mask1.b;
				half Specular = _Shininess;
				//TEXTURED THRESHOLD
				fixed TexThreshold = mask1.g - 0.5;

				float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

				i.Normal = normalize(i.Normal);
				fixed ndl = max(0, dot(i.Normal, lightDir)*0.5 + 0.5);
				ndl += TexThreshold;
				fixed3 ramp = smoothstep(_RampThreshold - _RampSmooth*0.5, _RampThreshold + _RampSmooth*0.5, ndl);
				fixed addShadowRamp = 0;
				addShadowRamp = max(1 - ramp.x, 0);
				_SColor = lerp(_HColor, _SColor, _SColor.a);	//Shadows intensity through alpha
				ramp = lerp(_SColor.rgb, _HColor.rgb, ramp);

				//Specular
				half3 h = normalize(lightDir + i.viewDir);
				float ndh = max(0, dot(i.Normal, h));
				float spec = pow(ndh, Specular*128.0) * Gloss * 2.0;
				spec = smoothstep(0.5 - _SpecSmooth*0.5, 0.5 + _SpecSmooth*0.5, spec);
				fixed4 c;
				c.rgb = Albedo * ramp + addShadowRamp * skinShadowAmount * _SkinShadowColor;
				c.rgb += _SpecColor.rgb * spec + mainTex.rgb * _DarkColor.rgb;

				//return _BloomFactor;

				c.a = Alpha;						// 泛光值;// s.Alpha + _LightColor0.a * _SpecColor.a * spec;
				if (_is_gray > 0)
				{
					float grey = dot(c.rgb, float3(0.299*_gray_factor, 0.587*_gray_factor, 0.114*_gray_factor));
					c.rgb = float3(grey, grey, grey);
				}
				//c.rgb = ramp;
				return c;
			}
			ENDCG
		}//end pass
		
		//LINEPASS
		UsePass "AFW/Toon/Toon - OutLine/LINEPASS"
	}//end SubShader

	FallBack "Mobile/Diffuse"

}//end Shader