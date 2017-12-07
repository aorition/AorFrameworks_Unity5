Shader "Custom/NGUI/NGUI-ScreenChange 1" {


	SubShader
	{

	    Tags
	    {
		    "Queue" = "Transparent"
		    "IgnoreProjector" = "True"
		    "RenderType" = "Transparent"
	    }

	Pass
	{
		ZWrite Off
		ZWrite Off
		Cull Off
		Lighting Off
		ZWrite Off
		Fog{ Mode Off }
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
        #pragma vertex vert_img
        #pragma fragment frag
        #include "UnityCG.cginc"



		sampler2D _MainTex;
	    float4 _MainTex_ST;
	    fixed4 _Color;

	    float _Tiling;
	    float _Scale1;
	    float _Scale2;


	    fixed4 frag(v2f_img i) : COLOR
	    {
		    fixed4 c = tex2D(_MainTex, i.uv) * _Color;

	        float2 uvdir = float2(1 - i.uv.x, i.uv.y);
	        float2 mUv = uvdir - floor(uvdir * _Tiling) / _Tiling * _Scale2;
	        float blockWidth = _Scale1 / _Tiling * _Scale2;
	        fixed m = step(mUv.x, blockWidth) * step(mUv.y, blockWidth);
	        c = lerp(fixed4(0,0,0,0), c, m);

	        return c;
	    }
	    ENDCG

	  }

	}
	

	SubShader
	{
			LOD 100

			Tags
		    {
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		    }

		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog{ Mode Off }
			Offset -1, -1
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse

			SetTexture[_MainTex]
		    {
			Combine Texture * Primary
		}

	  }

	}
	

}
