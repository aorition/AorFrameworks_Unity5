// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

 Shader "Fast Shadow Projector/Multiply"
 {
     Properties
     {
         _ShadowTex ("Cookie", 2D) = "gray"
     }
      
     Subshader
     {
         Tags { "RenderType"="Transparent" "Queue"="Transparent-2" }
         Pass
         {
             ZWrite Off
             ColorMask RGB
             Blend DstColor Zero
			 Offset -1, -1
			 Fog { Color (1, 1, 1) }
                          
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
   
             #include "UnityCG.cginc"
              
             struct v2f
             {
                 float4 pos : SV_POSITION;
                 float4 uv_Main : TEXCOORD0;
             };
             
              
             sampler2D _ShadowTex;
             float4x4 _GlobalProjector;
              
             v2f vert(appdata_tan v)
             {
                 v2f o;
                 o.pos = UnityObjectToClipPos (v.vertex);
                 o.uv_Main = mul (_GlobalProjector, v.vertex);
                 return o;
             }
              
             half4 frag (v2f i) : COLOR
             {
                 half4 tex = half4(i.uv_Main.w, i.uv_Main.w, i.uv_Main.w, 1);
                 
				 if (max(abs(i.uv_Main.x - 0.5), abs(i.uv_Main.y - 0.5)) < 0.5f)
				 {
                 	tex = tex2D(_ShadowTex, i.uv_Main.xy);
                 }

          
				 return tex;
             }
             ENDCG
      
         }
     }
 }