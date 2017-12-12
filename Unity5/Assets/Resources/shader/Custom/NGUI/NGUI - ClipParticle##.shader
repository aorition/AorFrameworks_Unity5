// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// NGUI裁切粒子
Shader "Custom/NGUI/NGUI - ClipParticle##" {
	Properties {
 _MainTex ("Texture", 2D) = "white" { }
_TintColor("Color", Color) = (1,1,1,1)
_Lighting ("Lighting",  float) = 1
 
[HideInInspector]_CutOut("CutOut", float) = 0.1
[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4
[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend Mode", Float) = 5
[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend Mode", Float) = 10
[Enum(UnityEngine.Rendering.BlendMode)] _SrcAlphaBlend("Src Alpha Blend Mode", Float) = 1
[Enum(UnityEngine.Rendering.BlendMode)] _DstAlphaBlend("Dst Alpha Blend Mode", Float) = 10

}


	SubShader {
 
  
	Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
 
	
	Pass
    {


	Blend[_SrcBlend][_DstBlend],[_SrcAlphaBlend][_DstAlphaBlend]
	ZWrite Off
	ZTest[_ZTest]
	Cull[_Cull]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
			

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float _Lighting;



			float4 _ClipArea;    // ( min x ,  max x ,  min y ,  max y )
					

			struct appdata {
				float4 vertex : POSITION;
				float2 texcoord:TEXCOORD0;
				float4 color : COLOR;
			};

            struct v2f {
                float4  pos : SV_POSITION;
                float2  uv : TEXCOORD0;
                float4 color : COLOR;

				float2 worldPos : TEXCOORD1;

            };



            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
                o.color=v.color;

			    o.worldPos = v.vertex.xy;
 
                return o;
            }


            
             fixed4 _TintColor;

            float4 frag (v2f i) : SV_Target
            {
                float4 c= tex2D(_MainTex,i.uv);
                c = c * _TintColor * i.color;
				c.rgb*=_Lighting;

				bool inArea = i.worldPos.x > _ClipArea.x && i.worldPos.x < _ClipArea.y && 
					i.worldPos.y > _ClipArea.z && i.worldPos.y < _ClipArea.w;
				return inArea ? c : fixed4(0, 0, 0, 0);

            }
            ENDCG
		}
		
	}


}
