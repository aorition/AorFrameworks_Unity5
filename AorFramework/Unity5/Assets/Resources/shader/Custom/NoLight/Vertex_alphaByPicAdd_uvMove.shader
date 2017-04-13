Shader "Custom/NoLight/Unlit - AlphaByPicAdd_uvMove" {
    Properties {
        _MainTex ("Texture", 2D) = "white" { }
        _AlphaTex ("Texture", 2D) = "white" { }
        _color("Color", Color)=(1,1,1,0)
        _TimeScale("TimeScale", float) = 3
    }
    

    SubShader
    {
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }

		Blend SrcAlpha One
		
	Tags
		{
			"Queue" = "Transparent+10"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
        pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
       
            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            sampler2D _AlphaTex;
            float4 _AlphaTex_ST;
            
           float4 _color;
           
           float _TimeScale;
           
            struct v2f {
                float4  pos : SV_POSITION;
                float2  uv : TEXCOORD0;
                 float2  uv2 : TEXCOORD1;
            };
           
            //顶点函数没什么特别的，和常规一样
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
                o.uv =    TRANSFORM_TEX(v.texcoord,_MainTex);
                o.uv2 =    TRANSFORM_TEX(v.texcoord,_AlphaTex);
                return o;
            }
 
            float4 frag (v2f i) : COLOR
            {
      			  float4 texCol = tex2D(_MainTex,i.uv+_Time*_TimeScale);
      			   float4 alphaCol = tex2D(_AlphaTex,i.uv2);
      			   
      			  texCol.r*=_color.r+_color.a;
      			  texCol.b*=_color.g+_color.a;
      			  texCol.g*=_color.b+_color.a;
      				 texCol.a=min(alphaCol.r,texCol.a);
                return texCol;
            }
            ENDCG
        }
    }
}