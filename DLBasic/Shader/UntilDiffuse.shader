
Shader "White_Shader/UntilDiffuse"
{
	Properties{
		_Diffuse("Diffuse Color",Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_EColor("Envir Color",Color) = (1,1,1,1)
	}
		SubShader
	{
		Pass{
		Tags{ "LightMode" = "ForwardBase" }
		CGPROGRAM
#include "Lighting.cginc"
#pragma vertex vert		
#pragma fragment frag
#pragma multi_compile_fog

		fixed4 _Diffuse;
		fixed4 _EColor;
		struct a2v {
		float4 vertex:POSITION;
		float3 normal:NORMAL;
		float2 uv : TEXCOORD0;
	};
		sampler2D _MainTex;
		float4 _MainTex_ST;

	struct v2f {
		float4 position:SV_POSITION;
		fixed3 color : COLOR;
		float2 uv : TEXCOORD0;
		UNITY_FOG_COORDS(1)
	};

	v2f vert(a2v v) {
		v2f f;
		f.position = UnityObjectToClipPos(v.vertex);
		//环境光
		//fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;

		f.uv = TRANSFORM_TEX(v.uv, _MainTex);
		UNITY_TRANSFER_FOG(f, f.position);

		fixed3 ambient = _EColor.rgb;
		fixed3 normalDir = normalize(mul(v.normal, (float3x3) unity_WorldToObject));
		fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
		fixed3 diffuse = _LightColor0.rgb * max(dot(normalDir,lightDir),0)*_Diffuse.rgb;
		f.color = diffuse + ambient;	//光是综合作用所以叠加，叠加+会越来越亮，融合*
		return f;
	}


	fixed4 frag(v2f f) :SV_Target{

		// sample the texture
	fixed4 col = tex2D(_MainTex, f.uv);
	fixed4 col1 = fixed4(f.color, 1)* col;

	UNITY_APPLY_FOG(f.fogCoord, col1);
//	return col;
		return col1;
	}
		ENDCG
	}
	}
		Fallback "VertexLit"
}

