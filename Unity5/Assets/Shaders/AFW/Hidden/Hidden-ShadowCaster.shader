Shader "Hidden/Hidden - ShadowCaster"
{

	Properties{}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
        {
            Name "ShadowCaster"

            Tags { "LightMode" = "ShadowCaster" }
           
            Fog { Mode Off }
            ZWrite On ZTest Less Cull Off
            Offset 1, 1
             
            CGPROGRAM
			 
			#include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #pragma fragmentoption ARB_precision_hint_fastest
            
            struct v2f
            { 
                V2F_SHADOW_CASTER;
            };
           
            v2f vert(appdata_base v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
 
            ENDCG
        }//end pass
	}//end SubShader
}//end Shader
