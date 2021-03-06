Shader "White_Shader/UVAnimator"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_XSpeed("xspeed",Range(0.1,10.0)) = 1.0
		_YSpeed("yspeed",Range(0.1,10.0)) = 1.0
		_XOffset("xoffset",Range(0,1.0)) = 1.0
		_YOffset("yoffset",Range(0,1.0)) = 1.0
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent" }
			LOD 100

			Pass
			{
				ZWrite Off
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

				float _XSpeed;      //x偏移速度
				float _YSpeed;      //y偏移速度
				float _XOffset;     //x偏移幅度
				float _YOffset;     //y偏移幅度

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					//x进行sin周期运动，y直接前向运动
					//float2 uv = float2(cos(_Time.y * _XSpeed) * _XOffset, _Time.y * _YSpeed * _YOffset);
					float2 uv = float2(-_Time.y* _XSpeed, 0);
					i.uv += uv;
					fixed4 col = tex2D(_MainTex, i.uv);
					return col;
				}
				ENDCG
			}
		}
}