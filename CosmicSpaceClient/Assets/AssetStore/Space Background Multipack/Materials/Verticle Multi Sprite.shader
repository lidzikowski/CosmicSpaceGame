// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Verticle" {
Properties
{
		_Bottom("Bottom", 2D) = "white" {}
		_Second("Second", 2D) = "white" {}
		_Third("Third", 2D) = "white" {}
		_Top("Top", 2D) = "white" {}
		overlap("Overlap", Range(0,0.2)) = 0.01
		position("Position", Float) = 0.2
}

	SubShader{
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
		LOD 100

		ZWrite Off
		Fog { Mode Off }
		Blend One OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
			};

			sampler2D _Bottom;
			sampler2D _Second;
			sampler2D _Third;
			sampler2D _Top;
			float4 _Bottom_ST;

			float overlap;
			float position;

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _Bottom);
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				i.texcoord.x = fmod(i.texcoord.x, 1);
				i.texcoord.y = fmod(i.texcoord.y, 1);
				
				fixed4 col;
				
				float2 bottomPos = (i.texcoord * float2(1 / (1 + overlap), 4 / (1 + overlap * 4)));
				float2 secondPos = ((i.texcoord - float2(0, 0.25f - overlap)) * float2(1 / (1 + overlap * 6), 4 / (1 + overlap * 6)));
				float2 thirdPos =  ((i.texcoord - float2(0, 0.5f - overlap  / 2)) * float2(1 / (1 + overlap * 6), 4 / (1 + overlap * 6)));
				float2 topPos = ((i.texcoord - float2(0, 0.75f - overlap)) * float2(1 / (1 + overlap * 4), 4 / (1 + overlap  * 4)));

				float4 bottomColour = tex2D(_Bottom, bottomPos);
				float4 secondColour = tex2D(_Second, secondPos);
				float4 thirdColour = tex2D(_Third, thirdPos);
				float4 topColour = tex2D(_Top, topPos);

				if(i.texcoord.y < 0.25)
				{					
					col = bottomColour;//top left
												
					if(i.texcoord.y > 0.25 -overlap)//right hand overlap
					{
						float ratio = 0.5f + ((0.25 - i.texcoord.y) / overlap) / 2;//combine part of one texture

						col = lerp(secondColour, bottomColour, ratio);//this way we get a nice slow transition
					}					
				}
				else if(i.texcoord.y < 0.5)
				{
					col = secondColour;

					if(i.texcoord.y < 0.25 + overlap)//left hand overlap
					{
						float ratio = 0.5f + ((i.texcoord.y - 0.25) / overlap) / 2;//combine part of one texture

						col = lerp(bottomColour, secondColour, ratio);//this way we get a nice slow transition
					}	

					if(i.texcoord.y > 0.5 -overlap)//right hand overlap
					{
						float ratio = 0.5f + ((0.5 - i.texcoord.y) / overlap) / 2;//combine part of one texture

						col = lerp(thirdColour, secondColour, ratio);//this way we get a nice slow transition
					}	
				}
				else if(i.texcoord.y < 0.75)
				{
					col = thirdColour;
				
					if(i.texcoord.y < 0.5 + overlap)//left hand overlap
					{
						float ratio = 0.5f + ((i.texcoord.y - 0.5) / overlap) / 2;//combine part of one texture

						col = lerp(secondColour, thirdColour, ratio);//this way we get a nice slow transition
					}	

					if(i.texcoord.y > 0.75 -overlap)//right hand overlap
					{
						float ratio = 0.5f + ((0.75 - i.texcoord.y) / overlap) / 2;//combine part of one texture

						col = lerp(topColour, thirdColour, ratio);//this way we get a nice slow transition
					}	
				}
				else
				{
					col = topColour;
				
					if(i.texcoord.y < 0.75 + overlap)//left hand overlap
					{
						float ratio = 0.5f + ((i.texcoord.y - 0.75) / overlap) / 2;//combine part of one texture

						col = lerp(thirdColour, topColour, ratio);//this way we get a nice slow transition
					}	
				}

				return col;
			}
			ENDCG
		}
	}
}