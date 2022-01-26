Shader "White_Shader/Outline_Usual"
{
     Properties
    {
        _OutlineCol("OutlineCol",Color) = (1,0,0,1)
        _OutlineFactor("OutlineFactor",Range(0,1)) = 0.1
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            //�ü�ģ�͵�����
            Cull Front
            Offset 1,1  
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            fixed4 _OutlineCol;
            float _OutlineFactor;

            struct v2f
            {
                float4 pos : SV_POSITION;
            };


            v2f vert (appdata_base v)
            {
                v2f o;


                o.pos = UnityObjectToClipPos(v.vertex);

                


                //����һ��������ת����ͶӰ�ռ䣬��ͶӰ�׶ν���ƫ��
                //�ж���
                float3 viewNormal = mul((float3x3)UNITY_MATRIX_IT_MV,v.normal);
                float3 offset = TransformViewToProjection(viewNormal);
				//��������z���꣬��ֹ�ڵ�������Ⱦ
				offset.z = -0.5f;
				o.pos.xyz+=offset * _OutlineFactor;
                
                //��������������������Ϊ����ʸ����ת����ͶӰ�ռ䣬�Դ�Ϊ����ƫ��
                //û�ж���
                 float3 dir = normalize(v.vertex.xyz);
				 float4 newPos = v.vertex;
				 newPos.xyz += dir * _OutlineFactor;
				 o.pos = UnityObjectToClipPos(newPos);
                


                
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineCol;
            }
            ENDCG
        }


        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD1;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i):SV_TARGET
            {
                return tex2D(_MainTex,i.uv);
            }


            ENDCG
        }
    }
}