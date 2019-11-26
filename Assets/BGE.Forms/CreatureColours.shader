// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/CreatureColours" {
	Properties {
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_PositionScale("PositionScale", Range(0, 1000)) = 250
		_Fade("Fade", Range(0, 1)) = 1
		_Offset("Offset", Float) = 0
	}
	SubShader {
		Tags {"Queue" = "Transparent" "RenderType"="Transparent" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alpha:fade
         
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		
		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		half _Fade;
		half _Offset;

		float _PositionScale;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			float u,v;			
			u = abs((IN.worldPos.x + _Offset) / _PositionScale);
			//u -= (int)u;
            v = abs((IN.worldPos.z + _Offset) / _PositionScale);
			//v -= (int)v;
			fixed4 c = tex2D (_MainTex, float2(u,v));
			o.Albedo = c.rgb;

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = _Fade;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

/*
Shader "Custom/CreatureColours" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_PositionScale("PositionScale", Range(0, 1000)) = 250
		_Fade("Fade", Range(0, 1)) = 1
	}
	SubShader {
		Tags {"Queue" = "Transparent" "RenderType"="Transparent" }
		LOD 200

		ZWrite On
		Cull Back
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		half _Fade;

		float _PositionScale;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			float u,v;			
			u = abs(IN.worldPos.x + _Offset / _PositionScale);
			//u -= (int)u;
            v = abs(IN.worldPos.z + _Offset / _PositionScale);
			//v -= (int)v;
			fixed4 c = tex2D (_MainTex, float2(u,v));
			o.Albedo = c.rgb;

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = _Fade;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
*/