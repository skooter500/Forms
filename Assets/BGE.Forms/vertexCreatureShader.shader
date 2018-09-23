Shader "Custom/CreatureColoursSpecular"
{
	Properties
	{
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Ambient ("Ambient", Range (0, 1)) = 0.25
		_SpecColor ("Specular Material Color", Color) = (1,1,1,1) 
        _Shininess ("Shininess", Float) = 10
		_PositionScale("PositionScale", Range(0, 1000)) = 250
		_Fade("Fade", Range(0, 1)) = 1

	}
	SubShader
	{
		Pass
		{
			Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			
			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertexClip : SV_POSITION;
				float4 vertexWorld : TEXCOORD2;
				float3 worldNormal : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Ambient;
          	float _Shininess;
			
			half _Fade;
			float _PositionScale;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertexClip = UnityObjectToClipPos(v.vertex);
				o.vertexWorld = mul(unity_ObjectToWorld, v.vertex);

				float u1,v1;			
				u1 = abs(o.vertexWorld.x / _PositionScale);
				v1 = abs(o.vertexWorld.z / _PositionScale);
				v.uv = float2(u1, v1);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldNormal = worldNormal;
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
            	float3 normalDirection = normalize(i.worldNormal);
            	float3 viewDirection = normalize(UnityWorldSpaceViewDir(i.vertexWorld));
				float3 lightDirection = normalize(UnityWorldSpaceLightDir(i.vertexWorld));
				
				// sample the texture

				float4 tex = tex2D(_MainTex, i.uv);

				//Diffuse implementation (Lambert)
                float nl = max(_Ambient, dot(normalDirection, lightDirection));

				float4 diffuseTerm = nl * tex * _LightColor0;
				//diff.rbg += ShadeSH9(half4(i.worldNormal,1));
				
				//Specular implementation (Phong)
				float3 reflectionDirection = reflect(-lightDirection, normalDirection);
				float3 specularDot = max(0.0, dot(viewDirection, reflectionDirection));
				float3 specular = pow(specularDot, _Shininess); 
				float4 specularTerm = float4(specular, 1) * _SpecColor * _LightColor0; 

                float4 finalColor = diffuseTerm + specularTerm;
                return finalColor;
			}
			ENDCG
		}
		Pass
		{
			Tags { "LightMode" = "ForwardAdd" }
			Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdadd
			
			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertexClip : SV_POSITION;
				float4 vertexWorld : TEXCOORD2;
				float3 worldNormal : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float _Ambient;
          	float _Shininess;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertexClip = UnityObjectToClipPos(v.vertex);
				o.vertexWorld = mul(unity_ObjectToWorld, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldNormal = worldNormal;
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
            	float3 normalDirection = normalize(i.worldNormal);
            	float3 viewDirection = normalize(UnityWorldSpaceViewDir(i.vertexWorld));
				float3 lightDirection = normalize(UnityWorldSpaceLightDir(i.vertexWorld));
				
				// sample the texture
				float4 tex = tex2D(_MainTex, i.uv);

				//Diffuse implementation (Lambert)
                float nl = max(0.0, dot(normalDirection, lightDirection));
				float4 diffuseTerm = nl * _Color * tex * _LightColor0;
				//diff.rbg += ShadeSH9(half4(i.worldNormal,1));
				
				//Specular implementation (Phong)
				float3 reflectionDirection = reflect(-lightDirection, normalDirection);
				float3 specularDot = max(0.0, dot(viewDirection, reflectionDirection));
				float3 specular = pow(specularDot, _Shininess); 
				float4 specularTerm = float4(specular, 1) * _SpecColor * _LightColor0; 

                float4 finalColor = diffuseTerm + specularTerm;
                return finalColor;
			}
			ENDCG
		}
	}
}