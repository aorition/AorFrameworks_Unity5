// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Custom/NoLight/Unlit - Black - line" {

	Properties{
	    _LineWidth("LineWidth", float) = 14
		_LineColor("LineColor", Color) = (0.5,0.7,1,1)

		_LineValue("LineValue", float) = 0.5
		_LineMin("LineMin", float) = 0
		_LineMax("LineMax", float) = 2
	}

	SubShader{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		    LOD 200

		    Pass {
			    Tags{ "Queue" = "Geometry+1" }

			    Cull Front
			    ZWrite Off

			    CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "Assets/ObjectBaseShader.cginc"	

		        float _LineWidth;
		        float4 _LineColor;

				float _LineValue;
				float _LineMin;
				float _LineMax;

		        struct v2f {
			        float4 pos:SV_POSITION;
					float4 vertexColor : TEXCOORD0;
		        };

		        v2f vert(appdata_base v) {
			        v2f o;
			        v.vertex.xyz += v.normal*_LineWidth*0.001;
			        o.pos = UnityObjectToClipPos(v.vertex);
					o.vertexColor = v.vertex;

			        return o;
		        }

		        float4 frag(v2f i) :SV_Target {

					float lineMask = ((i.vertexColor.x - _LineMin) / (_LineMax - _LineMin) + 1) / 2;
				    lineMask -= _LineValue;
				    lineMask = clamp(lineMask * 16, 0, 1);

					fixed4 lineCol = (_LineColor*0.2 * lineMask + (1 - lineMask)) * _LineColor;

		            return lineCol;
		        }
		        ENDCG
            }


			Pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "Assets/ObjectBaseShader.cginc"	

					struct v2f {
						float4 pos : SV_POSITION;
					};

					v2f vert(appdata_base v) {
						v2f o;
						o.pos = UnityObjectToClipPos(v.vertex);

						return o;
					}

					fixed4 frag(v2f i) : COLOR {
						fixed4 c = (0,0,0,0);

						return c;
					}
					ENDCG
			  }

	}

}
