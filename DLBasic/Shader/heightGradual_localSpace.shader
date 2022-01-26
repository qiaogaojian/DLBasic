
Shader "White_Shader/heightGradual_localSpace" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Excessive("ExcessiveValue", Range(0,1)) = 0.05
		_Intensity("Intensity", Range(0,1)) = 0.8
		_ButtonY("ButtonY", Range(-10,10)) = 0.0
		_GradualScale("GradualScale",Range(0.1,10)) = 1.0
	}
		SubShader{
			Tags { "RenderType" = "Opaque" "DisableBatching" = "True" }//DisableBatching tag,ref: http://docs.unity3d.com/Manual/SL-SubShaderTags.html
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows vertex:vert

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;

			struct Input {
				float2 uv_MainTex;
				float3 my_vertPos;
			};

			half _Excessive;
			half _Intensity;
			fixed4 _Color;
			float _ButtonY;
			float _GradualScale;
			void vert(inout appdata_full v, out Input o) {

				UNITY_INITIALIZE_OUTPUT(Input,o);//ref: http://forum.unity3d.com/threads/what-does-unity_initialize_output-do.186109/
				o.my_vertPos = v.vertex;
			 }
			void surf(Input IN, inout SurfaceOutputStandard o) {
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = (c.rgb * ((IN.my_vertPos.z + IN.my_vertPos.x * 0.5f) * _Excessive * _GradualScale - _ButtonY)) * _Intensity;

				o.Alpha = c.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}