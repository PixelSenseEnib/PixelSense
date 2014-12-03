Shader "Custom/Catia/CatiaShader" {
	Properties {
		//_MainTex ("Texture", 2D) = "white" {}
		_Color ("EmissiveColor", Color) = (0.0,0.0,0.0,0.0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull off
		
		CGPROGRAM
		inline fixed4 LightingLambertNocull (SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed diff = abs(dot (s.Normal, lightDir));
			
			fixed4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * 2);
			c.a = s.Alpha;
			return c;
		}
		
		
		inline fixed4 LightingLambertNocull_PrePass (SurfaceOutput s, half4 light)
		{
			fixed4 c;
			c.rgb = s.Albedo * light.rgb;
			c.a = s.Alpha;
			return c;
		}
		
		inline half4 LightingLambertNocull_DirLightmap (SurfaceOutput s, fixed4 color, fixed4 scale, bool surfFuncWritesNormal)
		{
			UNITY_DIRBASIS
			half3 scalePerBasisVector;
			
			half3 lm = DirLightmapDiffuse (unity_DirBasis, color, scale, s.Normal, surfFuncWritesNormal, scalePerBasisVector);
			
			return half4(lm, 0);
		}

		#pragma surface surf LambertNocull

		float4 _Color;

		struct Input {
			float4 color : COLOR;
			//float2 uv_MainTex : TEXCOORD0;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = IN.color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Emission = _Color;
			//o.Albedo.rg = IN.uv_MainTex;
			//o.Albedo.b = 0.0;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
