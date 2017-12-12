Shader "Hidden/Alpha" {
	SubShader {
		Pass {
			ZTest Always 
			Cull Off 
			ZWrite Off
			Blend One Zero
			Color (0,0,0,1)
		}
	}
}