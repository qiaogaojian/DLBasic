// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "AnimationInstancing/Unlit" {
Properties {
	_Color("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_ColorDensity ("AlbedoDensity", Range(0,5)) = 1
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200

CGPROGRAM
#include "AnimationInstancingBase.cginc"


#pragma surface surf UnlitShader vertex:vert addshadow noambient 
#pragma multi_compile_instancing

sampler2D _MainTex;
	fixed4 _Color;
half _ColorDensity;
struct Input {
	float2 uv_MainTex;
};

half4 LightingUnlitShader(SurfaceOutput s, half3 lightDir,half viewDir, float atten) {
	half4 c;
	c.rgb = s.Albedo;
	c.a = 1;
    return c;
}
void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = c.rgb*_ColorDensity;
	o.Alpha = c.a;
}
ENDCG
}

//Fallback "Legacy Shaders/VertexLit"
}
