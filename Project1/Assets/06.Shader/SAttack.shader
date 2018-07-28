Shader "Custom/SAttack" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_TimeX ("Time", Range(0.0, 1.0)) = 1.0
		_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
		_Value ("_Value", Range(0.0, 20.0)) = 6.0
	}

	SubShader {
		Pass {
			Cull Off ZWrite Off ZTest Always
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			#pragma glsl
			#include "UnityCG.cginc"			
			
			uniform sampler2D _MainTex;
			uniform float _TimeX;
			uniform float4 _ScreenResolution;
			uniform float _Value;
			uniform float _Speed;
			uniform float _Wavy;
			uniform float _Wave;
			uniform float _Fade;
			
			struct appdata_t {
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float2 texcoord  : TEXCOORD0;
				float4 vertex   : SV_POSITION;
				float4 color    : COLOR;
			};   
			
			v2f vert(appdata_t IN) {
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color;
				
				return OUT;
			}
			
			half4 _MainTex_ST;
			float4 frag(v2f i) : COLOR
			{
				float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
				float t = _TimeX*_Speed;
				float drunk 	 = (sin(t*2.0));
				float unitDrunk1 = (sin(t*1.2)+1.0)/2.0;
				float unitDrunk2 = (sin(t*1.8)+1.0)/2.0;
				
				float2 normalizedCoord = fmod((uvst.xy + (float2(0, drunk) / _ScreenResolution.x)), 1.0);
				normalizedCoord.x = pow(normalizedCoord.x, lerp(1.25, 0.85, unitDrunk1));
				normalizedCoord.y = pow(normalizedCoord.y, lerp(0.85, 1.25, unitDrunk2));
				
				float2 normalizedCoord2 = fmod((uvst.xy + (float2(drunk, 0.) / _ScreenResolution.x)), 1.0);	
				normalizedCoord2.x = pow(normalizedCoord2.x, lerp(0.95, 1.1, unitDrunk2));
				normalizedCoord2.y = pow(normalizedCoord2.y, lerp(1.1, 0.95, unitDrunk1));
				
				float2 normalizedCoord3 = uvst.xy;
				
				normalizedCoord = lerp(normalizedCoord3, normalizedCoord, _Wavy);
				normalizedCoord2 = lerp(normalizedCoord3, normalizedCoord2, _Wavy);
				float4 color  = tex2D(_MainTex, normalizedCoord);	
				float4 color2 = tex2D(_MainTex, normalizedCoord2);
				
				float y = 0.7*sin((normalizedCoord3.y + _TimeX) * 4.0) * 0.038 +
							0.3*sin((normalizedCoord3.y + _TimeX) * 8.0) * 0.010 +
							0.05*sin((normalizedCoord3.y + _TimeX) * 40.0) * 0.05;
				
				float x = 0.5*sin((normalizedCoord3.y + _TimeX) * 5.0) * 0.1 +
							0.2*sin((normalizedCoord3.x + _TimeX) * 10.0) * 0.05 +
							0.2*sin((normalizedCoord3.x + _TimeX) * 30.0) * 0.02;
				
				_Wave *= _Fade;
				normalizedCoord3.x += _Wave*x;
				normalizedCoord3.y += _Wave*y;
				
				float4 color3 = tex2D(_MainTex, normalizedCoord3 + float2(0, 0));
				float4 finalColor = lerp( lerp(color, color2, unitDrunk1), color3, 0.6);
				finalColor = lerp(color3,finalColor,_Fade);

				return finalColor;
			}
			ENDCG
		}
	}
}