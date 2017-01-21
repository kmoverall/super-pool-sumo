Shader "Hidden/Water"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_Ambient ("Ambient Light", Color) = (0,0,0,1)
		_ToonRamp ("Toon Ramp", 2D) = "white" {}

		_LightDir ("Light Direction", Vector) = (-0.5, -1, 0.5, 1)

		_SpringK ("Spring Constant", Float) = 0.025
		_Dampening ("Dampening", Float) = 0.025
		_Spread ("Spread", Float) = 0.5

		_Decal ("Decal Texture", 2D) = "black" {}
		_SplashPow ("Splash Power", Float) = 1
		_DecalPos ("Decal Position", Vector) = (0,0,0,0)
		_DecalRot ("Decal Rotation", Float) = 0
		_DecalScale ("Decal Scale", Float) = 1
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		//Water Height Pass
		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float2 uv  : TEXCOORD0;
			};
			

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;

				return o;
			}

			sampler2D _MainTex;
			uniform float4 _MainTex_TexelSize;

			float _SpringK;
			float _Dampening;
			float _Spread;

			float4 frag(v2f i) : SV_Target
			{
				float4 c = tex2D(_MainTex, i.uv);

				float accel = 0;

				float2 uvDist = _MainTex_TexelSize.xy;
				float4 samples[4];

				samples[0] = tex2D(_MainTex, i.uv + uvDist*float2(0, 1)); // T
				samples[1] = tex2D(_MainTex, i.uv - uvDist*float2(1, 0)); // L
				samples[2] = tex2D(_MainTex, i.uv + uvDist*float2(1, 0)); // R
				samples[3] = tex2D(_MainTex, i.uv - uvDist*float2(0, 1)); // B
				
				for (int ii = 0; ii < 4; ii++)
				{
					if (samples[ii].a > 0)
						accel += _Spread * (samples[ii].r - c.r);
				}

				accel += -_SpringK * c.r;
				c.g = c.g * _Dampening + accel * unity_DeltaTime * 0.5;
				c.r += c.g * unity_DeltaTime * 20;

				c.rgb *= c.a;

				return c;
			}
		ENDCG
		}


		//Water Visual Pass
		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float2 uv  : TEXCOORD0;
			};
			

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				return o;
			}

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float3 _LightDir;
			float3 _Ambient;
			sampler2D _ToonRamp;

			fixed4 _Color;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv);

				float3 norm;
				float3 lightDir = normalize(_LightDir);

				float2 uvDist = _MainTex_TexelSize.xy;
				float4 samples[4];

				samples[0] = tex2D(_MainTex, i.uv + uvDist*float2(0, -1)); // B
				samples[1] = tex2D(_MainTex, i.uv + uvDist*float2(-1, 0)); // L
				samples[2] = tex2D(_MainTex, i.uv + uvDist*float2(1, 0)); // R
				samples[3] = tex2D(_MainTex, i.uv + uvDist*float2(0, 1)); // T

				norm.y = samples[0].r - samples[3].r;
				norm.x = samples[1].r - samples[2].r;
				norm.z = -2;

				norm = normalize(norm);

				float NdotL = dot(norm, lightDir);
				float NdotV = dot(norm, float3(0,0,-1));
				float3 R = reflect(lightDir, norm);
				float spec = 0.5 * pow(dot(R, float3(0,0,-1)), 6);

				float light = NdotL * 0.5 + 0.5;

				float3 color = _Color;
				float foam = pow(abs(c.g), 5);
				foam = smoothstep(0.25, 0.5, foam);
				color = lerp(color, float3(1,1,1), foam);

				float2 rampUV = float2(light, 0);
				light = tex2D(_ToonRamp, rampUV);

				color = color * c.a;
				color = lerp(_Ambient * color, color, light);
				color += saturate(spec);
				
				c.rgb = color;

				return c;
			}
		ENDCG
		}

		//Splash Pass
		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float2 texcoord  : TEXCOORD0;
			};
			

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;

				return o;
			}

			sampler2D _MainTex;

			sampler2D _Decal;
			float _SplashPow;
			float4 _DecalPos;
			float _DecalRot;
			float _DecalScale;

			float4 frag(v2f i) : SV_Target
			{
				float4 c = tex2D(_MainTex, i.texcoord);

				float2 decalUV = i.texcoord;
				//TODO: ROTATION
				decalUV -= _DecalPos - 0.5;
				decalUV -= 0.5;
				decalUV /= _DecalScale;
				decalUV += 0.5;

				c.g -= tex2D(_Decal, decalUV) * _SplashPow;

				return c;
			}
		ENDCG
		}
	}
}
