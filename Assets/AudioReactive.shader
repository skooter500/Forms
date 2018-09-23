Shader "Custom/AudioReactive" {
	Properties{
		_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Particle Texture", 2D) = "white" {}
		_ColorTex("Color Texture", 2D) = "white" {}
		_InvFade("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_PositionScale("PositionScale", Range(0, 1000)) = 250
		_Offset("Offset", Float) = 0

	}

		Category{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }
		Blend SrcAlpha One
		ColorMask RGB
		Cull Off Lighting Off ZWrite Off

		SubShader{
		Pass{

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0
#pragma multi_compile_particles
#pragma multi_compile_fog

#include "UnityCG.cginc"

		sampler2D _MainTex;
		sampler2D _ColorTex;
		fixed4 _TintColor;

	struct appdata_t {
		float4 vertex : POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f {
		float4 vertex : SV_POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		UNITY_FOG_COORDS(1)
#ifdef SOFTPARTICLES_ON
			float4 projPos : TEXCOORD2;
#endif
		UNITY_VERTEX_OUTPUT_STEREO
	};

	float4 _MainTex_ST;

	float _PositionScale;
	half _Offset;

	v2f vert(appdata_t v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.vertex = UnityObjectToClipPos(v.vertex);
#ifdef SOFTPARTICLES_ON
		o.projPos = ComputeScreenPos(o.vertex);
		COMPUTE_EYEDEPTH(o.projPos.z);
#endif
		o.color = v.color;
		o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
		UNITY_TRANSFER_FOG(o,o.vertex);
		return o;
	}

	UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
	float _InvFade;

	fixed4 frag(v2f i) : SV_Target
	{
#ifdef SOFTPARTICLES_ON
		float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
	float partZ = i.projPos.z;
	float fade = saturate(_InvFade * (sceneZ - partZ));
	i.color.a *= fade;
#endif

	float uu,vv;
	uu = abs((i.vertex.x + _Offset) / _PositionScale);
	//u -= (int)u;
	vv = abs((i.vertex.z + _Offset) / _PositionScale);
	//v -= (int)v;
	fixed4 c = tex2D(_ColorTex, float2(uu,vv));
	

	fixed4 col = 50.0f * tex2D(_MainTex, i.texcoord)* c;
	col.a = saturate(col.a); // alpha should not have double-brightness applied to it, but we can't fix that legacy behaior without breaking everyone's effects, so instead clamp the output to get sensible HDR behavior (case 967476)

	UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0)); // fog towards black due to our blend mode
	return col;
	}
		ENDCG
	}
	}
	}
}
