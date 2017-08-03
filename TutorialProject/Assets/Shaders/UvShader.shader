Shader "Custom/UvShader"
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
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
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
