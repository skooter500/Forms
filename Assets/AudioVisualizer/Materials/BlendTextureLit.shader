Shader "Custom/BlendTextureLit" {
	Properties {
	_Color ("Color", Color) = (1,1,1)
	_Blend ("Blend", Range (0,1)) = 0.5 
	_MainTex ("Texture 1", 2D) = "" 
	_Texture2 ("Texture 2", 2D) = ""
}
 
Category {
	Material {
		Ambient[_Color]
		Diffuse[_Color]
	}
 
	// iPhone 3GS and later
	SubShader {Pass {
		Lighting On
		SetTexture[_MainTex]
		SetTexture[_Texture2] { 
			ConstantColor (0,0,0, [_Blend]) 
			Combine texture Lerp(constant) previous
		}
		SetTexture[_] {Combine previous * primary Double}
	}}
 
	// pre-3GS devices, including the September 2009 8GB iPod touch
	SubShader {
		Pass {
			SetTexture[_MainTex]
			SetTexture[_Texture2] {
				ConstantColor (0,0,0, [_Blend])
				Combine texture Lerp(constant) previous
			}
		}
		Pass {
			Lighting On
			Blend DstColor SrcColor
		}
	}
}
	FallBack "Diffuse"
}
