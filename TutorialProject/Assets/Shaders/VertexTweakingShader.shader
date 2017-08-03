// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SampleVertexShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 normal: NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				
				
				// Alter vertex along normal
				
				
				
				v.vertex = v.vertex + (sin(_Time)) * 0.2 * v.normal;
				o.vertex = UnityObjectToClipPos(v.vertex);

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return float4(1,1,1,1);

				// sample the texture
				//fixed4 col = tex2D(_MainTex, i.uv);

				// Oscillates between 0 and 1
				float x = sin(_Time * 20) * 0.5 + 0.5;

				// If vertical texture coordinate < x
				if (i.uv.y < x)
				{
					// Draw green pixel (with x brightness)
					return float4(0, x, 0, 1);
				}
				// If vertical texture coordinate > x
				else
				{
					// Draw purple pixel (with 1-x brightness)
					return float4(1-x, 0, 1-x, 1);
				}
			}
			ENDCG
		}
	}
}
