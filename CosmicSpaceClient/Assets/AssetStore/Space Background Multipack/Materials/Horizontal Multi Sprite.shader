// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Horizontal" {
Properties
{
		_FarLeft("Far Left", 2D) = "white" {}
		_Left("Left", 2D) = "white" {}
		_Right("Right", 2D) = "white" {}
		_FarRight("Far Right", 2D) = "white" {}
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

			sampler2D _FarLeft;
			sampler2D _FarRight;
			sampler2D _Right;
			sampler2D _Left;
			float4 _FarLeft_ST;
			float overlap;
			float position;

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _FarLeft);
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				i.texcoord.x = fmod(i.texcoord.x, 1);
				i.texcoord.y = fmod(i.texcoord.y, 1);
				
				fixed4 col;
				
				float2 farLeftPos = (i.texcoord * float2(4 / (1 + overlap), 1 / (1 + overlap * 4)) );
				float2 leftPos = ((i.texcoord - float2(0.25f - overlap, 0)) * float2(4 / (1 + overlap * 6), 1 / (1 + overlap * 6)));
				float2 rightPos =  ((i.texcoord - float2(0.5f - overlap  / 2, 0)) * float2(4 / (1 + overlap * 6), 1 / (1 + overlap * 6)));
				float2 farRightPos = ((i.texcoord - float2(0.75f - overlap,0)) * float2(4 / (1 + overlap  * 4), 1 / (1 + overlap * 4)));

				float4 farLeftColour = tex2D(_FarLeft, farLeftPos);
				float4 leftColour = tex2D(_Left, leftPos);
				float4 rightColour = tex2D(_Right, rightPos);
				float4 farRightColour = tex2D(_FarRight, farRightPos);

				if(i.texcoord.x < 0.25)
				{					
					col = farLeftColour;//top left
												
					if(i.texcoord.x > 0.25 -overlap)//right hand overlap
					{
						float ratio = 0.5f + ((0.25 - i.texcoord.x) / overlap) / 2;//combine part of one texture

						col = lerp(leftColour, farLeftColour, ratio);//this way we get a nice slow transition
					}					
				}
				else if(i.texcoord.x < 0.5)
				{
					col = leftColour;

					if(i.texcoord.x < 0.25 + overlap)//left hand overlap
					{
						float ratio = 0.5f + ((i.texcoord.x - 0.25) / overlap) / 2;//combine part of one texture

						col = lerp(farLeftColour, leftColour, ratio);//this way we get a nice slow transition
					}	

					if(i.texcoord.x > 0.5 -overlap)//right hand overlap
					{
						float ratio = 0.5f + ((0.5 - i.texcoord.x) / overlap) / 2;//combine part of one texture

						col = lerp(rightColour, leftColour, ratio);//this way we get a nice slow transition
					}	
				}
				else if(i.texcoord.x < 0.75)
				{
					col = rightColour;
				
					if(i.texcoord.x < 0.5 + overlap)//left hand overlap
					{
						float ratio = 0.5f + ((i.texcoord.x - 0.5) / overlap) / 2;//combine part of one texture

						col = lerp(leftColour, rightColour, ratio);//this way we get a nice slow transition
					}	

					if(i.texcoord.x > 0.75 -overlap)//right hand overlap
					{
						float ratio = 0.5f + ((0.75 - i.texcoord.x) / overlap) / 2;//combine part of one texture

						col = lerp(farRightColour, rightColour, ratio);//this way we get a nice slow transition
					}	
				}
				else
				{
					col = farRightColour;
				
					if(i.texcoord.x < 0.75 + overlap)//left hand overlap
					{
						float ratio = 0.5f + ((i.texcoord.x - 0.75) / overlap) / 2;//combine part of one texture

						col = lerp(rightColour, farRightColour, ratio);//this way we get a nice slow transition
					}	
				}

				return col;
			}
			ENDCG
		}
	}
}