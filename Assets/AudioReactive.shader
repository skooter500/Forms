Shader "Custom/AudioReactive"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_ParticleTexture("Particle Texture(RGB)", 2D) = "white" {}
		_PositionScale("PositionScale", Range(0, 1000)) = 250
		_Offset("Offset", Float) = 0
	}
		SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 100
		Blend One One
		Zwrite Off
		Cull Back
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
		float2 uv : TEXCOORD0;
		float4 color : COLOR;
	};
	struct v2f
	{
		float2 uv : TEXCOORD0;
		float2 uv1 : TEXCOORD0;
		UNITY_FOG_COORDS(1)
			float4 vertex : SV_POSITION;
		float4 color : COLOR;
	};
	sampler2D _ParticleTexture;
	sampler2D _MainTex;
	half _Offset;

	float _PositionScale;
	
	
	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		float uu, vv;
		uu = abs((v.vertex.x + _Offset) / _PositionScale);
		vv = abs((v.vertex.z + _Offset) / _PositionScale);
		o.uv = float2(uu, vv);
		o.uv1 = v.uv;
		UNITY_TRANSFER_FOG(o,o.vertex);
		o.color = v.color;
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		// sample the texture
		fixed4 col = tex2D(_MainTex, i.uv1); // tex2D(_ParticleTexture, i.uv1);
	// apply fog
	UNITY_APPLY_FOG(i.fogCoord, col);
	return col;
	}
		ENDCG
	}
	}
}

/*

Shader "Custom/AudioReactive" {
	Properties{
		_ParticleTexture("Particle Texture(RGB)", 2D) = "white" {}
		_MainTex("Main Texture(RGB)", 2D) = "white" {}
	_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_PositionScale("PositionScale", Range(0, 1000)) = 250
		_Fade("Fade", Range(0, 1)) = 1
		_Offset("Offset", Float) = 0
	}
		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		sampler2D _MainTex;
		sampler2D __ParticleTexture;

	struct Input {		
		float3 worldPos;
		float2 uv_MainTex;
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

		void surf(Input IN, inout SurfaceOutputStandard o) {
		// Albedo comes from a texture tinted by color
		float u,v;
		u = abs((IN.worldPos.x + _Offset) / _PositionScale);
		//u -= (int)u;
		v = abs((IN.worldPos.z + _Offset) / _PositionScale);
		//v -= (int)v;
		
		fixed4 c = tex2D(__ParticleTexture, float2(u, v)); // tex2D(_MainTex, float2(u, v));
		o.Albedo = c.rgb;
		o.Alpha = c.a;
		// Metallic and smoothness come from slider variables
		o.Metallic = _Metallic;
		o.Smoothness = _Glossiness;
	}
	ENDCG
	}
		FallBack "Diffuse"
}

*/