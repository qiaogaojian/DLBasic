Shader "Custom/ShadowCollector" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}

		_AlphaValue("_AlphaValue", Range(0.5,1)) = 0.8
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
        #include "UnityCG.cginc"
		#pragma surface surf Lambert fullforwardshadows vertex:vert
		#pragma target 3.0
        
        sampler2D _ShadowTex;
        float4x4 _ShadowMatrix;
        float _ShadowBias;
		float _AlphaValue;
        sampler2D _MainTex;
        
        struct Input {
			float2 uv_MainTex;
            float4 shadowCoords;
		};

        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            float3 worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)).xyz;
            o.shadowCoords = mul(_ShadowMatrix, float4(worldPos, 1.0));
        }

		void surf(Input IN, inout SurfaceOutput o)
        {
            float lightDepth = 1.0 - tex2Dproj(_ShadowTex, IN.shadowCoords).r;
            float shadow = (IN.shadowCoords.z - _ShadowBias) < lightDepth ? 1.0 : _AlphaValue;


			float4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * shadow;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
