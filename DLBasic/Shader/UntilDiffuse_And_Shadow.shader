
Shader "White_Shader/UntilDiffuse_And_Shadow"
{
	Properties{
		_Diffuse("Diffuse Color",Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_EColor("EnvirColor",Color) = (1,1,1,1)
	}
		SubShader
		{
			Pass{
			Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
			#include "UnityCG.cginc"  
			#include "Lighting.cginc"
			 #include "AutoLight.cginc"
			#pragma vertex vert		
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase

			fixed4 _Diffuse;
			fixed4 _EColor;
			struct a2v {
				float4 vertex:POSITION;
				float3 normal:NORMAL;// 切线空间的确定是通过(存储模型里的)法线和(存储模型里的)切线确定的，所以需要模型的法线
				float2 uv : TEXCOORD0;
				float3 lightDir : TEXCOORD1;
		};
			sampler2D _MainTex;
			float4 _MainTex_ST;

		struct v2f {
			float4 position:SV_POSITION;
			fixed3 color : COLOR;
			float2 uv : TEXCOORD0;
			float3 lightDir : TEXCOORD1;
			float3 normal : TEXCOORD2;
			UNITY_FOG_COORDS(5)
			LIGHTING_COORDS(3, 4)
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
			fixed3 diffuse = _LightColor0.rgb * max(dot(normalDir,lightDir),0) * _Diffuse.rgb;
			f.color = diffuse + ambient;	//光是综合作用所以叠加，叠加+会越来越亮，融合*

			f.lightDir = normalize(ObjSpaceLightDir(v.vertex));
			f.normal = normalize(v.normal).xyz;

			TRANSFER_VERTEX_TO_FRAGMENT(f);
			return f;
		}


		fixed4 frag(v2f f) :SV_Target{

		float3 L = normalize(f.lightDir);
		float3 N = normalize(f.normal);
		float attenuation = LIGHT_ATTENUATION(f) * 2;
		float NdotL = saturate(dot(N, L));
		float4 diffuseTerm = NdotL * _LightColor0 * attenuation;
		// sample the texture
	fixed4 col = tex2D(_MainTex, f.uv);
	fixed4 col1 = (fixed4(f.color, 1) + diffuseTerm) * col;

	UNITY_APPLY_FOG(f.fogCoord, col1);
	//	return col;
			return col1;
		}
			ENDCG
		}
		}
			Fallback "VertexLit"
}

