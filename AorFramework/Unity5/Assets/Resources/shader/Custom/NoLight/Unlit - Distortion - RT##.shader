// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//@@@DynamicShaderInfoStart
//空气扭曲,请配合DistortionSupport脚本使用,用了此shader的物体层级设置为postEffect
//@@@DynamicShaderInfoEnd

//@@@DynamicShaderTitleRepaceStart
Shader "Custom/NoLight/Unlit - Distortion - RT##" {
//@@@DynamicShaderTitleRepaceEnd
Properties {
	_MainTex ("MainRT", 2D) = "white" {}
	_NoiseTex("Noise Texture", 2D) = "white" {}
	_MaskTex ("Mask Texture", 2D) = "white" {}  
	_Scale("Distortion Scale", Float) =1
		_SpeedX("Distortion SpeedX", Float) = 1
		_SpeedY("Distortion SpeedY", Float) = 1
}
	SubShader {
//@@@DynamicShaderTagsRepaceStart
	Tags {
	"Queue"="Geometry"
	 "IgnoreProjector"="True" 
	 "RenderType"="Geometry"
	 }
//@@@DynamicShaderTagsRepaceEnd



  Pass {

	Lighting Off
  	 //@@@DynamicShaderBlendRepaceStart
	//Blend SrcAlpha OneMinusSrcAlpha
	ZWrite Off
	 //@@@DynamicShaderBlendRepaceEnd

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            sampler2D _MaskTex;
			sampler2D _NoiseTex;
           float4 _MaskTex_ST;
			float4 _NoiseTex_ST;
			float	_Scale;
			float	_SpeedX;
			float	_SpeedY;
            struct v2f {
                float4  pos : SV_POSITION;
                float2  uv : TEXCOORD0;
				float2  uvMask : TEXCOORD1;
				float2  uvNoise : TEXCOORD2;
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
				o.uvMask = TRANSFORM_TEX(v.texcoord, _MaskTex);
                 o.uvNoise = TRANSFORM_TEX(v.texcoord, _NoiseTex);

				float4 screenUV = ComputeGrabScreenPos(o.pos);//计算该模型顶点在屏幕坐标的纹理信息
				o.uv = screenUV.xy/screenUV.w;
                return o;
            }
 
		 float4 _TintColor;
		  float _mask;
		  float _lineSize;	
		  float4 _lineColor;
		  
            float4 frag (v2f i) : COLOR
            {
			half4 maskCol = tex2D(_MaskTex, i.uvMask);
			half coordOffsetX = _SpeedX * _Time;
			half coordOffsetY = _SpeedY * _Time;
			half4 noiseCol = tex2D(_NoiseTex, i.uvNoise+ half2(coordOffsetX, coordOffsetY));

			noiseCol = noiseCol * 2 - 1;
			 
		half offsetX = noiseCol.r * maskCol.r*_Scale;
		half offsetY = noiseCol.g * maskCol.r*_Scale;

		half4 texCol = tex2D(_MainTex, i.uv + half2(offsetX, offsetY));
		 
	 
		//	texCol.r = 1;

			return texCol;

  
             
            }
            ENDCG
        }
 
}



}