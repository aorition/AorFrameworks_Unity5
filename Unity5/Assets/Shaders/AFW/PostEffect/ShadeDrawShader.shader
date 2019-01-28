Shader "Hidden/PostEffect/ShadeDrawShader"
{  
    Properties   
    {  
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		[NoScaleOffset] _ShadeTex ("ShadeTex (RGB)", 2D) = "white" {}
        _fade      ("Opacity",   float) = 1 
        _sharpness ("Sharpness", float) = 1  
        _offsetX   ("OffsetX",   float) = 0  
        _offsetY   ("OffsetY",   float) = 0  
        _width     ("Width",     float) = 1  
        _height    ("Height",    float) = 1  
        _ellipse   ("Ellipse",   float) = 4  
        _brightness     ("Brightness",     float) = 1  
		_saturation     ("Saturation",     float) = 0.2  
		[HideInInspector] _isShadeByTex ("isShadeByTex", int) = 0
    }  
      
    SubShader   
    {   
        Tags   
        {   
          "QUEUE"="Transparent"   
          "RenderType"="Transparent"   
        } 
		
        Pass   
        {  
            ZWrite Off  
            //Blend SrcAlpha OneMinusSrcAlpha  

            CGPROGRAM  

			#include "UnityCG.cginc"

            #pragma vertex vert  
            #pragma fragment frag 
			
            struct v2f   
            {  
                float4 vertex : POSITION;  
				float2  texcoord : TEXCOORD0;  
                float2  texcoord1 : TEXCOORD1;  
            };  
  
            struct appdata_t   
            {  
                float4 vertex : POSITION;  
				float2 texcoord : TEXCOORD0;
            };  
  
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _ShadeTex;
            float _offsetX;  
            float _offsetY;  
            float _fade;  
            float _sharpness;  
            float _width;  
            float _height;  
            float _ellipse;  
            float _brightness;  
			float _saturation;
			int _isShadeByTex;
              
            v2f vert(in appdata_t v)   
            {  
                float4 tmpvar_2 = UnityObjectToClipPos(v.vertex); 
                float2 tmpvar_3 = (tmpvar_2.xy / tmpvar_2.w);  
                  
                float2 tmpvar_1;  
                tmpvar_1.x = (tmpvar_3.x + _offsetX);  
                tmpvar_1.y = (tmpvar_3.y + _offsetY) * 2.5;  
                  
                v2f o;            
				o.texcoord =TRANSFORM_TEX(v.texcoord,_MainTex);
				o.vertex   = tmpvar_2;  
                o.texcoord1 = tmpvar_1;  
                  
                return o;  
            }  
  
            fixed4 frag(v2f IN) : SV_Target  
            {  
                half4 col;  
				half4 renderTex = tex2D(_MainTex, IN.texcoord);
				if (_isShadeByTex == 0)
				{
					col.xyz   = half3(0.0, 0.0, 0.0);  
                  
					col.w = clamp  
							(  
								pow(abs(IN.texcoord1.x / 0.5) / _width,  _ellipse) +   
								pow(abs(IN.texcoord1.y / 0.5) / _height, _ellipse),  
								0.0,   
								1.0  
							);  			
				}
				else
				{
					IN.texcoord1.x = (IN.texcoord.x + _offsetX) / _width;
					IN.texcoord1.y = (IN.texcoord.y + _offsetY) / _height;
					col = tex2D(_ShadeTex, IN.texcoord1);
				}

				if (col.w < 1.0)
				{
					half mul = pow( 1 / col.w,  1 / _ellipse);
					col.w = 1 / mul;
					col.w = pow(col.w, 1 / _sharpness);
				}
				col.w *= _fade;

				half gray = 0.2125 * renderTex.r + 0.7154 * renderTex.g + 0.0721 * renderTex.b;
				half3 grayColor = half3(gray, gray, gray);
				grayColor = lerp(grayColor, renderTex.xyz, _saturation * (1 - col.w / _fade));
				renderTex.xyz = grayColor * _brightness;
				col = col * col.w + renderTex  * (1 - col.w);
				                        
                return  col;  
            }
            ENDCG  
        }//end pass
    }//end SubShader  
      
    FallBack "Diffuse"  
}//end Shader