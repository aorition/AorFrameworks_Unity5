// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Sprites/SpriteUI Blur"
{
 Properties
 {
     [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
     [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_Distance("Distance", float) = 0
		 _OutLineColor("OutLineColor", color) = (1,1,1,1)
 }
Category {
     Tags
     { 
         "Queue"="Transparent" 
         "IgnoreProjector"="True" 
         "RenderType"="Transparent" 
         "PreviewType"="Plane"
         "CanUseSpriteAtlas"="True"
     }

  	 Cull Off
     Lighting Off
     ZWrite Off
     Fog { Mode Off }
	ZTest [unity_GUIZTestMode]
 Blend SrcAlpha OneMinusSrcAlpha
 SubShader
 {

  Pass
     {
     Name "SPRITE_BASE"
     CGPROGRAM
         #pragma vertex vert
         #pragma fragment frag
         #pragma multi_compile DUMMY PIXELSNAP_ON
         #include "UnityCG.cginc"
		 #pragma target 3.0

     
         struct appdata_t
         {
             float4 vertex   : POSITION;
             float4 color    : COLOR;
             float2 texcoord : TEXCOORD0;
         };

         struct v2f
         {
             float4 vertex   : SV_POSITION;
             fixed4 color    : COLOR;
             half2 texcoord  : TEXCOORD0;
         };



         v2f vert(appdata_t IN)
         {
             v2f OUT;
             OUT.vertex = UnityObjectToClipPos(IN.vertex);
             OUT.texcoord = IN.texcoord;
             OUT.color = IN.color ;
             #ifdef PIXELSNAP_ON
             OUT.vertex = UnityPixelSnap (OUT.vertex);
             #endif

             return OUT;
         }

         sampler2D _MainTex;
		 float _Gray;
		 float  _Distance;
		 float4 _OutLineColor;
         fixed4 frag(v2f IN) : COLOR
         {
		 
			 fixed4 computedColor = tex2D(_MainTex, IN.texcoord);
			 fixed4 orgColor = computedColor;
	 
			 computedColor += tex2Dbias(_MainTex, half4(IN.texcoord.x , IN.texcoord.y,0 , _Distance)) * IN.color;

			 computedColor = computedColor / 4;
			 computedColor *= _OutLineColor*( _OutLineColor.a*10);
			 computedColor = lerp(computedColor,orgColor, orgColor.a);
	 
			 return computedColor;

 
         }
     ENDCG
     }
 
 }
 }
}
