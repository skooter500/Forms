// AlanZucconi.com: http://www.alanzucconi.com/?p=4539
Shader "Custom/CreatureTextureMaker"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_ColourScale("Colour Scale", Range(0, 1)) = 1
	}
	
	SubShader
	{
		// Required to work
		ZTest Always Cull Off ZWrite Off
		Fog{ Mode off }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;

			float _ColourScale;

			float3 hsvToRgb(float3 HSV)
			{
				float3 RGB = HSV.z;
		   
				float var_h = HSV.x * 6;
				float var_i = floor(var_h);   // Or ... var_i = floor( var_h )
				float var_1 = HSV.z * (1.0 - HSV.y);
				float var_2 = HSV.z * (1.0 - HSV.y * (var_h-var_i));
				float var_3 = HSV.z * (1.0 - HSV.y * (1-(var_h-var_i)));
				if      (var_i == 0) { RGB = float3(HSV.z, var_3, var_1); }
				else if (var_i == 1) { RGB = float3(var_2, HSV.z, var_1); }
				else if (var_i == 2) { RGB = float3(var_1, HSV.z, var_3); }
				else if (var_i == 3) { RGB = float3(var_1, var_2, HSV.z); }
				else if (var_i == 4) { RGB = float3(var_3, var_1, HSV.z); }
				else                 { RGB = float3(HSV.z, var_1, var_2); }
		   
				return (RGB);
			}

			float4 frag(v2f_img i) : COLOR
			{
				float h = ((i.uv.x + i.uv.y) / 2.0f) * _ColourScale;
				float3 c = hsvToRgb(float3(h, 1.0f, 0.9f));
				return float4(c, 1);
			}
			ENDCG
		}
	}
}