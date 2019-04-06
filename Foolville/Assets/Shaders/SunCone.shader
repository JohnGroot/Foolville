Shader "Custom/SunCone" 
{
	Properties 
	{
		[Header(Main Attributes)]
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}

		[Header(UV Scrolling)]
		_UVScrollSpeed ("UV Scroll Speed", Float) = 1.0

		[Header(Vertex Noise)]
		_NoiseScale ("Noise Scale", Float) = 1.0
		_NoiseSpeed ("Noise Speed", Float) = 1.0
		_NoiseAmplitude ("Noise Amplitude", Float) = 1.0
	}
	SubShader 
	{
		//if performance is bad it's probably because of  "DisableBatching"="True" !!! 
		Tags { "RenderType"="Transparent" "Queue"="AlphaTest" "DisableBatching"="True" }
		LOD 200

		Cull Off
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf NoLighting vertex:vert alpha:fade addshadow

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

		sampler2D _MainTex;

		struct Input 
		{
			float2 uv_MainTex;
			float3 worldPos;
		};

		fixed4 _Color;
		float _UVScrollSpeed;

		float _NoiseAmplitude;
		float _NoiseScale;
		float _NoiseSpeed;

		float eznoise(float3 co)
		{
			return frac(sin( dot(co.xyz ,float3(12.9898,78.233,45.5432) )) * 43758.5453);
		}

		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			
			//fixed4 worldPos = mul(transpose(unity_ObjectToWorld), float4(0, 1, 0, 1));
			//v.normal = worldPos;

			fixed4 worldPos = mul(unity_ObjectToWorld, v.vertex);
			
			fixed3 noise = eznoise(v.vertex.xyz * _NoiseScale);
			worldPos.xyz += sin(v.normal * noise.x * _NoiseSpeed * _Time.y) * _NoiseAmplitude;

			v.vertex = mul(unity_WorldToObject, worldPos);
		}

		void surf (Input IN, inout SurfaceOutput o) 
		{
			// uv scroll
			IN.uv_MainTex.x += _Time + _UVScrollSpeed;

			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}