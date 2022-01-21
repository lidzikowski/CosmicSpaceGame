Shader "EbalStudios/StarShader"
//Simple shader to give the stars a fresnel effect.
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_FresnelColor("Fresnel  Color", Color) = (1,1,1,1)
		_FallOff("FallOff", Range(0,1)) = 0
		_Power("Fresnel Power", float) = 1
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
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 wNormal : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _FresnelColor;
			float _FallOff;
			float _Power;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.viewDir = WorldSpaceViewDir(v.vertex);
				o.wNormal = UnityObjectToWorldNormal(v.normal);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			float InverseLerp(float min, float max, float t)
			{
				t = clamp(t, min, max);
				return (t - min) / (max - min);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float fernel = InverseLerp ( 0,1-_FallOff, abs(dot(normalize(i.viewDir),normalize(i.wNormal))));
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				col = lerp(_FresnelColor,col, pow(fernel, 1.0/_Power));
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
