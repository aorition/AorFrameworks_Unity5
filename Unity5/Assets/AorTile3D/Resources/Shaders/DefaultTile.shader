Shader "Hidden/AorTile3D/DefaultTile"
{
	Properties
	{
		_MainColor("Main Color", Color) = (1,1,1,1)
		_InnerColor("Inner Color", Color) = (1,1,1,0.25)
		_BorderColor("Border Color", Color) = (0,0.5,1,0.75)
		_BorderValue("Border Value", Range(0,1)) = 0.01
	}

	SubShader
	{
		Tags{"Queue" = "Transparent" "RenderType" = "Transparent"}

		LOD 100

		Pass
		{
			
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"
			
			float4 _MainColor;
			float4 _InnerColor;
			float4 _BorderColor;
			float _BorderValue;
			
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{

				float ib = 1 - _BorderValue;

				fixed l = i.uv.x >= _BorderValue && i.uv.x <= ib && i.uv.y >= _BorderValue && i.uv.y <= ib ? 0 : 1;

				fixed4 col = lerp(_InnerColor, _BorderColor, l);
				col *= _MainColor;

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
