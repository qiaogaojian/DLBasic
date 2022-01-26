// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/UnlitShader-Transparent"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Color("Color",Color) = (1,1,1,1)
		_TransVal("Transparency", Range(0,1)) = 1  //透明度的值
	}
		SubShader{
			Tags{ "RenderType" = "Queue" "Queue" = "Transparent" } // "Queue"="Transparent"将其设置为透明，不然无法看见后面的东西（即使透明）
			Blend SrcAlpha OneMinusSrcAlpha //实现Alpha的核心，使用语句进行Alpha混合
			Pass{

			CGPROGRAM
#pragma vertex vert  
#pragma fragment frag  

#include "UnityCG.cginc"  

			sampler _MainTex;
		//float4 _MainTex_ST;  使用TRANSFORM_TEX必备
		float4 _Color;
		float	_TransVal;
		struct v2f {
			float4 pos : POSITION;
			float2 uv : TEXCOORD0;
			float3 color : TEXCOORD1;
		};

		v2f vert(appdata_base v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			//TRANSFORM_TEX是在_MainTex_ST中的宏
			//原始方法o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			//将uv贴图的坐标取出来
			o.uv = v.texcoord;
			//o.color = ShadeVertexLights(v.vertex, v.normal);取得法线贴图的光照  
			return o;
		}

		float4 frag(v2f i) : COLOR{
			half4 c = tex2D(_MainTex, i.uv);
			//在这个位置接收i.color的话可以接收光照  
			c.rgb = c.rgb * _Color;
			c = c*_Color;
			c.a = _TransVal;
			return c;
		}
			ENDCG
		}
		}
			FallBack "Diffuse"
}
