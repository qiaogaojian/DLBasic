Shader "Unlit/ScrollX"
{
	Properties
	{
		_MainTex("Base Layer(RGB)", 2D) = "white" {}    // ����    
		_ScrollX("Base layer Scroll Speed",Float) = 0.1   // �����ٶ�
		_Offect("YOffect",Float)=0.2
		_Mutiplier("Layer Mutiplier", Float) = 1         //��������
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry" }
		LOD 100

		Pass
	{
		Tags{ "LightMode" = "ForwardBase" }
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag


#include "UnityCG.cginc"

		struct a2v
	{
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;
	float _ScrollX;
	float _Mutiplier;
	float _Offect;

	v2f vert(a2v v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);

		o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex) + frac(float2 (0.0,_ScrollX) * _Time.y)+ _Offect;//�˴����Ը��� ���� ������

		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{

		fixed4 c = tex2D(_MainTex, i.uv.xy);
		c.rgb *= _Mutiplier;

	return c;
	}
		ENDCG
	}
	}
		FallBack "VertexLit"
}
