// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "White_Shader/Outline_1"
{
	//����  
	Properties{
		_Diffuse("Diffuse", Color) = (1,1,1,1)
		_OutlineColor("Outline Color", Color) = (0,0,0,0)
		_OutlineSize("OutlineSize", Range(1.0,1.5)) = 1.054
		_MainTex("Base 2D", 2D) = "white"{}
	}

		//����ɫ��    
		SubShader
	{
			Pass
			{
			Cull Front
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"	

				fixed4 _OutlineColor;
				float _OutlineSize;
				struct appdata
				{
					float4 vertex:POSITION;
				};
				struct v2f
				{
					float4 clipPos:SV_POSITION;
				};
				v2f vert(appdata v)
				{
					v2f o;
					o.clipPos = UnityObjectToClipPos(v.vertex * _OutlineSize);
					return o;
				}
				fixed4 frag(v2f i) : SV_Target
				{
					return _OutlineColor;
				}
				ENDCG
			}

		//������ɫ��Pass  
		Pass
		{
			CGPROGRAM

			//����ͷ�ļ�  
			#include "Lighting.cginc"  
			//����Properties�еı���  
			fixed4 _Diffuse;
			sampler2D _MainTex;
			//ʹ����TRANSFROM_TEX�����Ҫ����XXX_ST  
			float4 _MainTex_ST;

			//����ṹ�壺vertex shader�׶����������  
			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float2 uv : TEXCOORD1;
			};

			//���嶥��shader,����ֱ��ʹ��appdata_base������position, noramal, texcoord��  
			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				//ͨ��TRANSFORM_TEX��ת���������꣬��Ҫ������Offset��Tiling�ĸı�,Ĭ��ʱ��ͬ��o.uv = v.texcoord.xy;  
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
				return o;
			}

			//����ƬԪshader  
			fixed4 frag(v2f i) : SV_Target
			{
				//unity�����diffuseҲ�Ǵ��˻����⣬��������Ҳ����һ�»�����  
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * _Diffuse.xyz;
			//��һ�����ߣ���ʹ��vert��һ��Ҳ���У���vert��frag�׶��в�ֵ��������ķ��߷��򲢲���vertex shaderֱ�Ӵ�����  
			fixed3 worldNormal = normalize(i.worldNormal);
			//�ѹ��շ����һ��  
			fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
			//���ݰ�������ģ�ͼ������صĹ�����Ϣ  
			fixed3 lambert = 0.5 * dot(worldNormal, worldLightDir) + 0.5;
			//���������ɫΪlambert��ǿ*����diffuse��ɫ*����ɫ  
			fixed3 diffuse = lambert * _Diffuse.xyz * _LightColor0.xyz + ambient;
			//�����������  
			fixed4 color = tex2D(_MainTex, i.uv);
			color.rgb = color.rgb * diffuse;
			return fixed4(color);
		}

				//ʹ��vert������frag����  
				#pragma vertex vert  
				#pragma fragment frag     

				ENDCG
			}
	}
		//ǰ���ShaderʧЧ�Ļ���ʹ��Ĭ�ϵ�Diffuse  
			FallBack "Diffuse"
}