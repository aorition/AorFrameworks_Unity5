// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable

#include "UnityCG.cginc"

#define vec2 float2  
#define vec3 float3  
#define vec4 float4  
#define mat2 float2x2  
#define iGlobalTime _Time.y  
#define mod fmod  
#define mix lerp  
#define atan atan2  
#define fract frac   
#define texture2D tex2D 

// 屏幕的尺寸  
#define iResolution _ScreenParams  
// 屏幕中的坐标，以pixel为单位  
#define gl_FragCoord ((_iParam.srcPos.xy/_iParam.srcPos.w)*_ScreenParams.xy)   

#define PI2 6.28318530718  
#define pi 3.14159265358979  
#define halfpi (pi * 0.5)  
#define oneoverpi (1.0 / pi)  

vec4 circle(vec2 pos, vec2 center, float radius, float4 color) {
	if (length(pos - center) < radius) {
		// In the circle  
		return vec4(1, 1, 1, 1) * color;
	}
	else {
		return vec4(0, 0, 0, 1);
	}
}