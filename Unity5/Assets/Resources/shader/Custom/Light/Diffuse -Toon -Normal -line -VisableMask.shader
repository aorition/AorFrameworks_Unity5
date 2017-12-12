// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Light/Diffuse -Toon -Normal -line -VisableMask" {

	Properties{

		_MainTex("MainTex", 2D) = "white" {}
		_MaskTex("Mask(Must No mipMap)", 2D) = "black" {}
		_NormalTex("NormalTex", 2D) = "white" {}
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		//_LowOutlineWidth("LowOutlineWidth", float) = 4
		 _HightOutlineWidth("HightOutlineWidth", float) = 4
		_ToonShade("ToonShader", 2D) = "white" {}
		_HideFace("_HideFace", int) = 0
		[HideInInspector] _Color("Color", Color) = (1,1,1,1)
		[HideInInspector] _Lighting("Lighting", float) = 1
		[HideInInspector] _CutOut("CutOut", float) = 0.1

	}

	SubShader{

		Tags {
			"Queue" = "Geometry+20"
			"RenderType" = "Transparent"
		}

		LOD 600

		Pass{

			Name "VISABLEMASKPASS"
			Blend SrcAlpha One
			ZTest Greater
			ZWrite Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			float4 _notVisibleColor;

			struct VsOutput {
				float4 vertex: SV_POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			struct v2f {
				float4 pos : SV_POSITION;
				float4 color : COLOR;
				float2 uv0 : TEXCOORD0;
			};
			v2f vert(VsOutput v) {
				v2f o;
				o.uv0 = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				return o;
			}
			fixed4 frag(v2f i) : COLOR{
				//clip(-1);
				fixed4 col = tex2D(_MainTex,i.uv0);
				//  col.rgb *= _notVisibleColor.rgb;
				return fixed4(1,1,1,0.5);
			}
			ENDCG
		}

		UsePass "Custom/Light/Diffuse - Toon - Normal -line/LINEPASS"
		UsePass "Custom/Light/Diffuse - Toon - Normal -line/COLORPASS"

	}
	


	SubShader{

		Tags{
			"Queue" = "Geometry+20"
			"RenderType" = "Transparent"
		}

		LOD 200

		Pass{

			Name "VISABLEMASKPASS"
			Blend SrcAlpha One
			ZTest Greater
			ZWrite Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			float4 _notVisibleColor;

			struct VsOutput {
				float4 vertex: SV_POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			struct v2f {
				float4 pos : SV_POSITION;
				float4 color : COLOR;
				float2 uv0 : TEXCOORD0;
			};
			v2f vert(VsOutput v) {
				v2f o;
				o.uv0 = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				return o;
			}
			fixed4 frag(v2f i) : COLOR{
				//clip(-1);
				fixed4 col = tex2D(_MainTex,i.uv0);
				//  col.rgb *= _notVisibleColor.rgb;
				return fixed4(1,1,1,0.5);
			}
			ENDCG
		}

		UsePass "Custom/Light/Diffuse - Toon/BASETOON"

	}
}
