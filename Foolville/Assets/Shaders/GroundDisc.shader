Shader "Custom/GroundDisc" 
{
	Properties 
	{
		[Header(Main Attributes)]
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}

		[Header(Disc Clipping)]
		_Center("Center of Disc", Vector) = (0, 0, 0, 0)
		_Radius("Disc Radius", Range(0.0, 1.0)) = 1.0
		_ScaleFactor("Scale Factor", Float) = 3.0
	}
	SubShader 
	{
		//if performance is bad it's probably because of  "DisableBatching"="True" !!! 
		Tags { "RenderType"="Transparent" "Queue"="Geometry" "DisableBatching"="True" }
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
		
		fixed4 _Center;
		float _Radius;
		float _ScaleFactor;

		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			
			fixed4 worldPos = mul(transpose(unity_ObjectToWorld), float4(0, 1, 0, 1));
			v.normal = worldPos;
		}

		void surf (Input IN, inout SurfaceOutput o) 
		{
			float2 world2 = float2(IN.worldPos.x, IN.worldPos.z);
			float2 center2 = float2(_Center.x, _Center.z);
			float len = length(world2 - center2);
			float rad = _Radius * (2.5 * _ScaleFactor);
			if (len > rad)
			{
				discard;
			}

			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}