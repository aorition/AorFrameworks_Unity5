// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//@@@DynamicShaderInfoStart
//不写深度的双面的叠加的shader,层:Transparent
//@@@DynamicShaderInfoEnd


//@@@DynamicShaderTitleRepaceStart
Shader "Hidden/Custom/NoLight/Unlit - FallOff 3000_0_False_2_0_5_1_1_10"  {  
//@@@DynamicShaderTitleRepaceEnd

    Properties {
        _MainTex ("Texture", 2D) = "white" { }
		_TintColor("TintColor", color) = (1,1,1,1)
        _alpha("Alpha", float) = 1
        _light("Light", float) = 1
    }
    

    SubShader
    {
	   //@@@DynamicShaderTagsRepaceStart
Tags {   "Queue"="Transparent0" } 
//@@@DynamicShaderTagsRepaceEnd
       	
       		Lighting Off

       		
        pass
        {
		   	 //@@@DynamicShaderBlendRepaceStart
Blend SrcAlpha One,One OneMinusSrcAlpha
ZTest Less
 ZWrite Off
 Cull Off
//@@@DynamicShaderBlendRepaceEnd
		
			
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            half4 _MainTex_ST;
			fixed4 _TintColor;
           	fixed _alpha;
           	fixed _light;
            struct v2f {

                float4  pos : SV_POSITION;
                float3 normal : TEXCOORD0;
                float2  uv : TEXCOORD2;
                float3 worldvertpos : TEXCOORD1;

            };
           
            //顶点函数没什么特别的，和常规一样
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
				o.normal=v.normal;
				o.uv =TRANSFORM_TEX(v.texcoord,_MainTex);
				o.worldvertpos = ObjSpaceViewDir(v.vertex).xyz;
                return o;
            }
 
 
            float4 frag (v2f i) : COLOR
            {
            
				i.normal = normalize(i.normal);
				fixed3 viewdir = normalize( i.worldvertpos);
				fixed4 texCol = tex2D(_MainTex,i.uv)*_TintColor;
				texCol.a =pow((1- saturate(dot(viewdir, i.normal))),_alpha);
				return texCol*_light;
 
            }
            ENDCG
        }
    }
}