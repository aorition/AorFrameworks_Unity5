Shader "Custom/NGUI/NGUI-GaussBlur" {

	Properties{

	}

		SubShader{
		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
	}

		Pass{

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


		sampler2D _GBlurTex;
	float _GBlurLerpValue;

	//计算权重
	float GetGaussianDistribution(float x, float y, float rho) {
		float g = 1.0f / sqrt(2.0f * 3.141592654f * rho * rho);
		return g * exp(-(x * x + y * y) / (2 * rho * rho));
	}


	fixed4 frag(v2f_img i) : COLOR
	{

		//算出一个像素的空间
		float space = 1.0 / 512;

		float rho = 20 * space / 3.0;

		//权重总和
		float weightTotal = 0;
		for (int x = -20; x <= 20; x++)
		{
			for (int y = -20; y <= 20; y++)
			{
				weightTotal += GetGaussianDistribution(x * space, y * space, rho);
			}
		}

		fixed4 final;
		for (int z = -20; z <= 20; z++)
		{
			for (int w = -20; w <= 20; w++)
			{
				fixed weight = GetGaussianDistribution(z * space, w * space, rho) / weightTotal;

				fixed4 col = tex2D(_GBlurTex, i.uv + float2(z * space,w * space));
				col *= weight;
				final += col;
			}
		}
		final = lerp(fixed4(final.r, final.g, final.b,0), final, _GBlurLerpValue);

		return final;
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

		SetTexture[_GBlurTex]
	{
		Combine Texture * Primary
	}

	}

	}

}