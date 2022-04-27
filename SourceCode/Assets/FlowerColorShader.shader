Shader "Unlit/NewUnlitShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}

	}
		SubShader
	{
		Tags { "RenderType" = "Transparent" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
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
			int inputColor;
			int inputIntensity;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
			if (col.r == 1.0 && col.b == 1.0 && col.g == 0.0) {
				col.r = 1.0;
				col.g = 1.0;
				col.b = 1.0;
				col.a = 0.0;
				}
			else if ((col.r == col.g) && (col.g == col.b)) {
				switch (inputColor) {
					case 0: // red
						col.r = col.r * 1.0;
						col.g = col.g * 0.0;
						col.b = col.b * 0.0;
						break;
					case 1:
						col.r = col.r * 1.0;
						col.g = col.g * 0.3;
						col.b = col.b * 0.0;
						break;
					case 2:  //orange
						col.r = col.r * 1.0;
						col.g = col.g * 0.6;
						col.b = col.b * 0.0;
						break;
					case 3:
						col.r = col.r * 1.0;
						col.g = col.g * 0.8;
						col.b = col.b * 0.0;
						break;
					case 4: //yellow
						col.r = col.r * 1.0;
						col.g = col.g * 1.0;
						col.b = col.b * 0.0;
						break;
					case 5:
						col.r = col.r * 0.5;
						col.g = col.g * 1.0;
						col.b = col.b * 0.0;
						break;
					case 6: // green
						col.r = col.r * 0.0;

						col.g = col.g * 0.66;
						col.b = col.b * 0.0;
						break;
					case 7:
						col.r = col.r * 0.0;

						col.g = col.g * 0.33;
						col.b = col.b * 0.5;
						break;
					case 8: //blue
						col.r = col.r * 0.0;
						col.g = col.g * 0.0;
						col.b = col.b * 1.0;
						break;
					case 9:

						col.r = col.r * 0.4;
						col.g = col.g * 0.0;
						col.b = col.b * 0.8;
						break;
					case 10: //purple
						col.r = col.r * 0.6;
						col.g = col.g * 0.0;
						col.b = col.b * 0.6;
						break;
					case 11:
						col.r = col.r * 0.8;
						col.g = col.g * 0.0;
						col.b = col.b * 0.6;
						break;
					default:
						break;
					}
				switch (inputIntensity) {
					case 0:
						col.r = col.r * 0.75;
						col.g = col.g * 0.75;
						col.b = col.b * 0.75;
						break;
					case 1:
						break;
					case 2:
						col.r = col.r + ((1.0 - col.r) / 3.0);
						col.g = col.g + ((1.0 - col.g) / 3.0);
						col.b = col.b + ((1.0 - col.b) / 3.0);
						break;
					default:
						break;
					}
				}
				return col;
			}
			ENDCG
		}
	}
}
