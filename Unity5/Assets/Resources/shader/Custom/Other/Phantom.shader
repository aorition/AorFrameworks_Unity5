// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Other/Phantom" {
    Properties {
		_edgeVar("EdgeVar", float) = 0.46
		_lumFront("LumFront", float) = 4
        _lumBack("LumBack", float) = 0.2
		_color("Color", color) = (0.19, 0.48, 1, 1)

 
    }
    

    SubShader
    {

		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

 
       		Blend One OneMinusSrcAlpha
       		Lighting Off ZWrite Off
        pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

			float _edgeVar;
			float _lumFront;
			float _lumBack;
			float4 _color;
            struct v2f {

            float4  pos : SV_POSITION;
            float3 normal : TEXCOORD0;
            float3 worldvertpos : TEXCOORD1;

        };
           
            //顶点函数没什么特别的，和常规一样
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
     
				o.normal=v.normal;
                o.worldvertpos = ObjSpaceViewDir  (v.vertex).xyz;
                return o;
            }
 
 
            float4 frag (v2f i) : COLOR
            {
            
                i.normal = normalize(i.normal);
				float3 viewdir = normalize( i.worldvertpos);
                   
				float nDotV = 1 - saturate(dot(viewdir, i.normal));
				float stepVar = step(_edgeVar, nDotV);
				float lum = _lumFront * stepVar + _lumBack * (1 - stepVar);
				
				//float lum = min(pow(nDotV, 3), 1);
				float4 color;
				color.a = lum;
				color.rgb = lum * _color.xyz;// *float3(0.2, 0.5, 1);
				color.a /= 4;

				//return float4(lum, lum, lum, 1);// color*_light;
				return color;
            }
            ENDCG
        }
    }
}