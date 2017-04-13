// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable

#include "UnityCG.cginc"
 

//这里加multi_compile 没用
sampler2D _MainTex;
float4 _MainTex_ST;
#ifdef CLIP_ON
fixed _CutOut;
#endif

//anim
#ifdef ANIM_ON
fixed _power;
fixed _wind;
#endif

#ifdef LIGHTMAP_ON
// sampler2D unity_Lightmap;
// float4 unity_LightmapST;
#endif

float4 _Color;
fixed _Lighting;
float _time;

#ifdef FOG_ON
float _fogDestiy;
float _fogDestance;
float _volumeFogDestiy;
float _volumeFogOffset;
#endif


float _HdrIntensity;//hdr增幅
float4 _AmbientColor;//环境光

float3 _CustomLightColor0;
float3 _CustomLightColor1;
float3 _CustomLightColor2;
float3 _CustomLightColor3;

float4 _CustomLightPosX;
float4 _CustomLightPosY;
float4 _CustomLightPosZ;

float4 _CustomLightAtten;//4个光的Atten

float4 _DirectionalLightDir;//平行光源方向 w强度
float4 _DirectionalLightColor;//平行光源 颜色

inline float3 Shade4PointLights(half4 posWorld, float3 normal)
{
	// to light vectors
	float4 toLightX = _CustomLightPosX - posWorld.x;
	float4 toLightY = _CustomLightPosY - posWorld.y;
	float4 toLightZ = _CustomLightPosZ - posWorld.z;
	// squared lengths
	float4 lengthSq = 0;
	lengthSq += toLightX * toLightX;
	lengthSq += toLightY * toLightY;
	lengthSq += toLightZ * toLightZ;
	// NdotL
	float4 ndotl = 0;
	ndotl += toLightX * normal.x;
	ndotl += toLightY * normal.y;
	ndotl += toLightZ * normal.z;
	// correct NdotL
	float4 corr = rsqrt(lengthSq);
	ndotl = max(float4(0, 0, 0, 0), ndotl * corr);
	// attenuation
	float4 atten = _CustomLightAtten / lengthSq;

	//float4 atten = 1.0 / (1.0 + lengthSq * _CustomLightAtten);

	float4 diff = ndotl * atten;
	// final color
	float3 col = 0;
	col += _CustomLightColor0 * diff.x;
	col += _CustomLightColor1 * diff.y;
	col += _CustomLightColor2 * diff.z;
	col += _CustomLightColor3* diff.w;
	return col;
}

float CustomClamp(float src, float _min, float _max) {

	src = min(src, _max);
	src = max(src, _min);

	return src;


}

fixed3 DecodeLogLuv(in fixed4 vLogLuv)
{
	return DecodeLightmap(vLogLuv);

	fixed3x3 InverseM = fixed3x3(6.0014, -2.7008, -1.7996, -1.3320, 3.1029, -5.7721, 0.3008, -1.0882, 5.6268);
	fixed Le = vLogLuv.z * 255 + vLogLuv.w;
	fixed3 Xp_Y_XYZp;
	Xp_Y_XYZp.y = exp2((Le - 127) / 2);
	Xp_Y_XYZp.z = Xp_Y_XYZp.y / vLogLuv.y;
	Xp_Y_XYZp.x = vLogLuv.x * Xp_Y_XYZp.z;
	fixed3 vRGB = mul(Xp_Y_XYZp, InverseM);

	return max(vRGB, 0);
}


