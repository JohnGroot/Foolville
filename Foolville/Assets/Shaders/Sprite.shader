// Unlit alpha-cutout shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/Sprite (Transparent Cutout)" 
{
Properties 
{
	_Color ("Main Color", Color) = (1, 1, 1, 1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	[MaterialToggle] _ToggleBillboard("Toggle Billboard Effect", Float) = 0

	[Header(Dissolve)]
	_DissolveTex ("Dissolution texture", 2D) = "gray" {}
	_Threshold ("Threshold", Range(0.0, 1.01)) = 0.0
}
SubShader 
{
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 100

	Lighting Off

	Pass 
	{  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _Cutoff;
			float _ToggleBillboard;

			sampler2D _DissolveTex;
			float _Threshold;

			v2f vert (appdata_t v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				if (_ToggleBillboard == 1) {
					//code via https://gist.github.com/renaudbedard/7a90ec4a5a7359712202

					// get the camera basis vectors
					float3 forward = -normalize(UNITY_MATRIX_V._m20_m21_m22);
					float3 up = normalize(float3(0, 1, 0)); //normalize(UNITY_MATRIX_V._m10_m11_m12); //rotate on all axes
					//float3 up = UNITY_MATRIX_IT_MV[1].xyz;
					float3 right = normalize(UNITY_MATRIX_V._m00_m01_m02);

					// rotate to face camera
					float4x4 rotationMatrix = float4x4(right, 0,
						up, 0,
						forward, 0,
						0, 0, 0, 1);

					//float offset = _Object2World._m22 / 2;
					float offset = 0;
					v.vertex = mul(v.vertex + float4(0, offset, 0, 0), rotationMatrix) + float4(0, -offset, 0, 0);
				}

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
				float val = tex2D(_DissolveTex, i.texcoord).r;

				col.a *= step(_Threshold, val);
				clip(col.a - _Cutoff);

				col *= _Color;

				UNITY_APPLY_FOG(i.fogCoord, col);

				return col;
			}
		ENDCG
	}
}

}