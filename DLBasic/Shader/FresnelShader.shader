Shader "White_Shader/FresnelShader"
{
	Properties
	{
		_Alpha("中心透明度",Range(0,1)) = 0.8
		_RimPower("发光系数",Range(0.1,8)) = 2
		_SpecColor("菲涅尔颜色",COLOR) = (1,1,1,1)
		_SpecPower("Specular Power", Range(0,1)) = 0.5
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" "Queue" = "Transparent"}
			
			LOD 200
			Blend SrcAlpha OneMinusSrcAlpha Lighting Off
			ZWrite Off

			CGPROGRAM
			#pragma surface surf Unlit noambient keepalpha
			#pragma target 3.0

			half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
			{
			   return fixed4(s.Albedo, s.Alpha);
			}

			struct Input
			{
				float2 uv_BumpMap;
				float3 viewDir;
				float2 uv_DetailTex;
			};

			sampler2D _MainTex;  //定义纹理对象
			float _Alpha;
			float _RimPower;
			float _SpecPower;

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf(Input IN, inout SurfaceOutput o)
			{
				float rim = 1 - saturate(dot(o.Normal, normalize(IN.viewDir)));
				rim = pow(rim, _RimPower);

				o.Alpha = _Alpha * rim;
				o.Albedo += _SpecColor.rgb* rim;
				o.Specular = _SpecPower;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
