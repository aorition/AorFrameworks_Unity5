// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//@@@DynamicShaderInfoStart
//按UV Y方向做半透明过度 可反向
//@@@DynamicShaderInfoEnd

//@@@DynamicShaderTitleRepaceStart
Shader "Custom/NoLight/Unlit - UVMask##" {
//@@@DynamicShaderTitleRepaceEnd
Properties {
	_MainTex ("Texture", 2D) = "white" {}
    _color("Color", Color)=(1,1,1,1)
    _mask("Mask", Range(0,1))=0
     _range("Range", Range(0.01,1))=0
    [MaterialToggle] _reverse ("MaskReverse", Float) = 0
          
}


	SubShader {

		 //@@@DynamicShaderTagsRepaceStart
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
			 //@@@DynamicShaderTagsRepaceEnd
	 Lighting Off 

	       Pass
        {
		
			Blend SrcAlpha OneMinusSrcAlpha 

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
       
            sampler2D _MainTex;
            float4 _MainTex_ST;
           float _range;
            struct v2f {
                float4  pos : SV_POSITION;
                float2  uv : TEXCOORD0;
            };
           
           
           struct appdata {
    float4 vertex : POSITION;
    float2 texcoord:TEXCOORD0;
};

            //顶点函数没什么特别的，和常规一样
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
                return o;
            }
 
		 float4 _color;
		  float _mask;
		  float _reverse;	 
            float4 frag (v2f i) : COLOR
            {


                 _mask = _mask * (1.0 +_range) - _range;  		
                float4 col= tex2D(_MainTex,i.uv)*_color;
                col.a=min(col.a, saturate((i.uv.y * (1.0 - _reverse) + (1.0- i.uv.y) * _reverse - _mask)/_range));
   
                   clip(col.a);
                   return col;
                return  float4(_reverse,_reverse,_reverse,1);
             
            }
            ENDCG
        }

}
}