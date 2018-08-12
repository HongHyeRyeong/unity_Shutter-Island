Shader "Custom/SMurderer" {
	Properties{
		_MainTex("Albedo", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_GlossTex("Specular", 2D) = "white" {}
		_RampPow("Ramp Power", Range(0.1, 0.5)) = 0.1
		_SpecCol("Specular Color", Color) = (1,1,1,1)
		_SpecPow("Specular Power", Range(10,200)) = 100
	}

	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Warp noambient

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _GlossTex;
		float _RampPow;
		float4 _SpecCol;
		float _SpecPow;
	
		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv_GlossTex;
			float3 viewDir;
		};
	
		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 m = tex2D(_GlossTex, IN.uv_GlossTex);
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Albedo = c.rgb + m.rgb;
			o.Gloss = m.a;
			o.Alpha = c.a;
		}
	
		float4 LightingWarp(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
		{
			// 하이라이트
			float3 SpecColor;
			float3 H = normalize(lightDir + viewDir);
			float spec = saturate(dot(H, s.Normal));
			spec = pow(spec, _SpecPow);
			SpecColor = spec * _SpecCol.rgb * s.Gloss;
	
			// 외곽선
			float rim = abs(dot(s.Normal, viewDir));

			if (rim > 0.3)
				rim = 1;
			else
				rim = -1;

			// 음영
			float ndot1 = dot(s.Normal, lightDir) * 0.5 + 0.5;
	
			// 합치기
			float4 final;
			final.rgb = (s.Albedo.rgb * rim) + _RampPow + SpecColor;
			final.a = s.Alpha;
			return final;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
