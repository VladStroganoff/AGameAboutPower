Shader "TycoonTile/TerrainGridShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_GridColor ("Grid Color", Color) = (0,0,0,1)
		_GridThickness ("Grid Thickness", Range(0,1)) = 0.1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _GridColor;
		half __GridThickness;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

			//fixed ud = (abs((IN.uv_MainTex.x) * 2 - 1));
			//fixed vd = (abs((IN.uv_MainTex.y) * 2 - 1));

			//half t = max(ud, vd);// (ud + vd) * 0.5;


			//t = clamp(0, pow(t, 100), 1);

			//t *= _GridColor.a;

			float2 coord = IN.uv_MainTex;
			half2 grid = abs(frac(coord - float2(0.5, 0.5)) - 0.5) / fwidth(coord);
			float l = min(grid.x, grid.y);

			////t = 1 - smoothstep((0.5 + _GridThickness), (fwidth(t) - _GridThickness), t);

			half m = 1.0 - min(l, 1.0);

			half t = m * _GridColor.a;
			//o.Albedo = half4(m,m,m, 1.0);
			o.Albedo = lerp(c.rgb, _GridColor.rgb, t);
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
