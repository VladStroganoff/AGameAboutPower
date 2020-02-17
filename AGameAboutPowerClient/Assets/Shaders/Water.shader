// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Water"
{
	Properties
	{
		_WaveSpeed("Wave Speed", Float) = 0.2
		_WaveHeight("Wave Height", Float) = 0.5
		_Smothness("Smothness", Float) = 1
		_WaterColor("Water Color", Color) = (0.1994482,0.2821375,0.509434,1)
		_WaveColor("Wave Color", Color) = (0.2470185,0.3674179,0.5754717,1)
		_EdgeDistance("Edge Distance", Float) = 1
		_EdgePower("Edge Power", Float) = 1
		[Normal]_Texture0("Texture 0", 2D) = "bump" {}
		_NormalTile("Normal Tile", Float) = 1
		_NormalStrength("Normal Strength", Range( 0 , 1)) = 1
		_NormalSpeed("Normal Speed", Float) = 1
		_SeaFoam("Sea Foam", 2D) = "white" {}
		_FoamTile("Foam Tile", Float) = 1
		_RefractionAmount("Refraction Amount", Float) = 0.03
		_Depth("Depth", Float) = -4
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Standard keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float4 screenPos;
		};

		uniform float _WaveHeight;
		uniform float _WaveSpeed;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _EdgeDistance;
		uniform sampler2D _SeaFoam;
		uniform float _FoamTile;
		uniform float _EdgePower;
		uniform sampler2D _Texture0;
		uniform float _NormalStrength;
		uniform float _NormalSpeed;
		uniform float _NormalTile;
		uniform float4 _WaterColor;
		uniform float4 _WaveColor;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _RefractionAmount;
		uniform float _Depth;
		uniform float _Smothness;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float4 appendResult12 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float4 WorldSpaceTile13 = appendResult12;
			float2 panner4 = ( ( _Time.y * _WaveSpeed ) * float2( 1,0 ) + ( ( WorldSpaceTile13 * float4( float2( 0.1,0.001 ), 0.0 , 0.0 ) ) * 1.0 ).xy);
			float simplePerlin2D1 = snoise( panner4 );
			simplePerlin2D1 = simplePerlin2D1*0.5 + 0.5;
			float3 WaveValueAndHeight52 = ( ( float3(0,1,0) * _WaveHeight ) * simplePerlin2D1 );
			float3 temp_cast_2 = (0.0).xxx;
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth43 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_LOD( _CameraDepthTexture, float4( ase_screenPosNorm.xy, 0, 0 ) ));
			float distanceDepth43 = abs( ( screenDepth43 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _EdgeDistance ) );
			float4 clampResult50 = clamp( ( ( ( 1.0 - distanceDepth43 ) + tex2Dlod( _SeaFoam, float4( ( ( WorldSpaceTile13 / 10.0 ) * _FoamTile ).xy, 0, 0.0) ) ) * _EdgePower ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float4 Edge48 = clampResult50;
			float3 lerpResult135 = lerp( WaveValueAndHeight52 , temp_cast_2 , Edge48.rgb);
			v.vertex.xyz += lerpResult135;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float4 appendResult12 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float4 WorldSpaceTile13 = appendResult12;
			float4 temp_output_76_0 = ( WorldSpaceTile13 / 10.0 );
			float2 panner64 = ( 1.0 * _Time.y * ( float2( 1,0 ) * _NormalSpeed ) + ( temp_output_76_0 * _NormalTile ).xy);
			float2 panner65 = ( 1.0 * _Time.y * ( float2( -1,0 ) * ( _NormalSpeed * 3.0 ) ) + ( temp_output_76_0 * ( _NormalTile * 5.0 ) ).xy);
			float3 NormalMaps75 = BlendNormals( UnpackScaleNormal( tex2D( _Texture0, panner64 ), _NormalStrength ) , UnpackScaleNormal( tex2D( _Texture0, panner65 ), _NormalStrength ) );
			o.Normal = NormalMaps75;
			float2 panner4 = ( ( _Time.y * _WaveSpeed ) * float2( 1,0 ) + ( ( WorldSpaceTile13 * float4( float2( 0.1,0.001 ), 0.0 , 0.0 ) ) * 1.0 ).xy);
			float simplePerlin2D1 = snoise( panner4 );
			simplePerlin2D1 = simplePerlin2D1*0.5 + 0.5;
			float WavePattern31 = simplePerlin2D1;
			float clampResult35 = clamp( WavePattern31 , 0.0 , 1.0 );
			float4 lerpResult33 = lerp( _WaterColor , _WaveColor , clampResult35);
			float4 Albedo37 = lerpResult33;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 screenColor114 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( float3( (ase_grabScreenPosNorm).xy ,  0.0 ) + ( _RefractionAmount * NormalMaps75 ) ).xy);
			float4 clampResult115 = clamp( screenColor114 , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float4 Refraction116 = clampResult115;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth119 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth119 = abs( ( screenDepth119 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Depth ) );
			float clampResult121 = clamp( distanceDepth119 , 0.0 , 1.0 );
			float Depth122 = ( 1.0 - clampResult121 );
			float4 lerpResult123 = lerp( Albedo37 , Refraction116 , Depth122);
			o.Albedo = lerpResult123.rgb;
			float screenDepth43 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth43 = abs( ( screenDepth43 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _EdgeDistance ) );
			float4 clampResult50 = clamp( ( ( ( 1.0 - distanceDepth43 ) + tex2D( _SeaFoam, ( ( WorldSpaceTile13 / 10.0 ) * _FoamTile ).xy ) ) * _EdgePower ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float4 Edge48 = clampResult50;
			o.Emission = Edge48.rgb;
			o.Smoothness = _Smothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
0;431;1906;455;2377.915;1111.907;1.887921;True;True
Node;AmplifyShaderEditor.CommentaryNode;27;-3690.292,-32.10231;Inherit;False;824.0005;235;World Space UVs;3;11;12;13;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;11;-3640.292,17.89769;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;12;-3362.292,19.89769;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;-3095.292,44.89767;Inherit;False;WorldSpaceTile;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;78;960.8204,-2144.006;Inherit;False;2474.489;954;Normal Map;21;36;70;77;60;66;71;62;67;76;59;63;69;72;64;65;57;73;29;58;74;75;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;77;1062.055,-1868.405;Inherit;False;Constant;_Divide;Divide;11;0;Create;True;0;0;False;0;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;36;1010.82,-2016.183;Inherit;False;13;WorldSpaceTile;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;70;1541.31,-1807.006;Inherit;False;Property;_NormalSpeed;Normal Speed;10;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;1065.288,-1744.833;Inherit;False;Property;_NormalTile;Normal Tile;8;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;1791.31,-1699.006;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;67;1477.31,-1354.006;Inherit;False;Constant;_PanDirectionMinus;Pan Direction Minus;9;0;Create;True;0;0;False;0;-1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;66;1542.31,-2094.006;Inherit;False;Constant;_PanDirection;Pan Direction;9;0;Create;True;0;0;False;0;1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;1327.288,-1639.833;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;76;1272.055,-1906.405;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;54;-1881.154,-439.4032;Inherit;False;2057.677;879.5825;Wave Value And Height;17;14;16;10;9;15;18;8;6;17;4;1;25;19;24;20;31;52;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;1811.31,-1946.006;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;1565.991,-1937.185;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;1578.284,-1636.885;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;1836.31,-1430.006;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;16;-1817.154,-103.4032;Inherit;False;Constant;_WaveStretch;Wave Stretch;1;0;Create;True;0;0;False;0;0.1,0.001;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexturePropertyNode;57;1987.876,-1823.386;Inherit;True;Property;_Texture0;Texture 0;7;1;[Normal];Create;True;0;0;False;0;dd2fd2df93418444c8e280f1d34deeb5;dd2fd2df93418444c8e280f1d34deeb5;True;bump;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;14;-1833.154,-183.4032;Inherit;False;13;WorldSpaceTile;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;73;2361.31,-1782.006;Inherit;False;Property;_NormalStrength;Normal Strength;9;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;64;2012.31,-2091.006;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;65;2009.31,-1489.006;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-1609.154,-23.40319;Inherit;False;Constant;_WaveTile;Wave Tile;1;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-1609.154,-151.4032;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleTimeNode;10;-1625.154,216.5967;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;58;2362.304,-1587.809;Inherit;True;Property;_TextureSample0;Texture Sample 0;3;1;[Normal];Create;True;0;0;False;0;-1;dd2fd2df93418444c8e280f1d34deeb5;dd2fd2df93418444c8e280f1d34deeb5;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;29;2393.525,-2078.372;Inherit;True;Property;_NormalMap;Normal Map;3;1;[Normal];Create;True;0;0;False;0;-1;dd2fd2df93418444c8e280f1d34deeb5;dd2fd2df93418444c8e280f1d34deeb5;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;51;-4226.673,-1765.522;Inherit;False;2204.954;639.3613;Edge;15;86;79;47;82;85;84;80;81;83;45;48;50;46;43;44;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-1609.154,328.5969;Inherit;False;Property;_WaveSpeed;Wave Speed;0;0;Create;True;0;0;False;0;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-1401.154,-55.40319;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;6;-1401.154,72.59675;Inherit;False;Constant;_WaveDirection;Wave Direction;0;0;Create;True;0;0;False;0;1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-1353.154,216.5967;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;74;2868.31,-1757.006;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;80;-4139.333,-1456.589;Inherit;False;13;WorldSpaceTile;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;85;-4003.336,-1352.589;Inherit;False;Constant;_Float0;Float 0;12;0;Create;True;0;0;False;0;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;117;-1460.35,-2467.415;Inherit;False;1648;427;Refraction;9;116;115;114;109;108;113;110;111;112;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;84;-3769.338,-1463.589;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-3831.902,-1717.154;Inherit;False;Property;_EdgeDistance;Edge Distance;5;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-3783.337,-1303.589;Inherit;False;Property;_FoamTile;Foam Tile;12;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;3211.31,-1767.006;Inherit;False;NormalMaps;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PannerNode;4;-1113.154,56.59678;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;-3621.023,-1474.768;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;110;-1403.35,-2156.415;Inherit;False;75;NormalMaps;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;79;-4106.39,-1658.589;Inherit;True;Property;_SeaFoam;Sea Foam;11;0;Create;True;0;0;False;0;d01457b88b1c5174ea4235d140b5fab8;d01457b88b1c5174ea4235d140b5fab8;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.DepthFade;43;-3596.862,-1715.522;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;1;-905.1542,56.59678;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;108;-1343.35,-2417.415;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;128;-1205.838,-2823.546;Inherit;False;1143.283;210.3528;Depth;5;121;122;127;119;120;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;112;-1410.35,-2241.415;Inherit;False;Property;_RefractionAmount;Refraction Amount;14;0;Create;True;0;0;False;0;0.03;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;120;-1155.838,-2759.604;Inherit;False;Property;_Depth;Depth;15;0;Create;True;0;0;False;0;-4;-4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;45;-3256.584,-1702.936;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;109;-1076.35,-2411.415;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;38;-1371.79,-1336.264;Inherit;False;1512.119;592.4669;Albedo;6;37;33;32;35;30;34;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;111;-1044.35,-2209.415;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;31;-566.5107,86.04218;Inherit;False;WavePattern;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;83;-3480.023,-1590.768;Inherit;True;Property;_TextureSample1;Texture Sample 1;12;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;19;-894.699,-229.5455;Inherit;False;Constant;_WaveUp;Wave Up;1;0;Create;True;0;0;False;0;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;34;-1229.068,-849.704;Inherit;False;31;WavePattern;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;113;-698.3496,-2306.415;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-894.699,-53.5454;Inherit;False;Property;_WaveHeight;Wave Height;1;0;Create;True;0;0;False;0;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;119;-941.0212,-2773.546;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;86;-3019.327,-1612.591;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-3068.578,-1474.288;Inherit;True;Property;_EdgePower;Edge Power;6;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;35;-947.5243,-946.9741;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-654.699,-181.5454;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;30;-1136.712,-1318.315;Inherit;False;Property;_WaterColor;Water Color;3;0;Create;True;0;0;False;0;0.1994482,0.2821375,0.509434,1;0.1994482,0.2821375,0.509434,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;32;-1179.003,-1103.962;Inherit;False;Property;_WaveColor;Wave Color;4;0;Create;True;0;0;False;0;0.2470185,0.3674179,0.5754717,1;0,0.8542309,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenColorNode;114;-501.35,-2325.415;Inherit;False;Global;_GrabScreen0;Grab Screen 0;14;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-2800.793,-1698.684;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;121;-653.5554,-2772.193;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;33;-543.2627,-1123.035;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-345.1541,-71.40319;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClampOpNode;115;-241.35,-2321.415;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;127;-470.7168,-2760.226;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;50;-2629.27,-1654.919;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;37;-83.67103,-1102.366;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;48;-2434.793,-1640.684;Inherit;False;Edge;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;122;-286.5554,-2772.193;Inherit;False;Depth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;116;-36.34998,-2318.415;Inherit;False;Refraction;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;52;-89.15411,-71.40319;Inherit;False;WaveValueAndHeight;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;39;580.8682,-826.4898;Inherit;False;37;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;118;-4878.151,-1101.883;Inherit;False;2523.623;729.4561;Sea Foam;13;92;98;88;91;90;99;97;89;87;100;104;105;93;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;134;428.2871,-143.9707;Inherit;False;Constant;_Float2;Float 2;16;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;130;413.2871,-56.9707;Inherit;True;48;Edge;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;125;580.5861,-765.4029;Inherit;False;116;Refraction;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;126;573.586,-683.4029;Inherit;False;122;Depth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;53;386.0372,-231.7177;Inherit;False;52;WaveValueAndHeight;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;88;-4828.151,-807.6376;Inherit;False;13;WorldSpaceTile;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;135;834.2871,-188.9707;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClampOpNode;105;-2810.497,-776.7001;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;92;-4404.66,-800.3525;Inherit;False;Constant;_Float1;Float 1;12;0;Create;True;0;0;False;0;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;93;-2578.528,-782.3298;Inherit;False;SeaFoam;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;-4031.956,-914.8446;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;41;691.527,-510.2475;Inherit;False;75;NormalMaps;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;104;-3039.051,-775.0599;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;98;-4196.675,-549.6641;Inherit;False;Constant;_FoamMask;Foam Mask;14;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;90;-4180.271,-903.6657;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;28;878.6054,-347.2183;Inherit;False;Property;_Smothness;Smothness;2;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;97;-3732.795,-629.134;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.01,0.001;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;49;576.9608,-435.9758;Inherit;False;48;Edge;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;100;-3502.312,-631.427;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;91;-4207.723,-785.9427;Inherit;False;Property;_SeaFoamTile;Sea Foam Tile;13;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;87;-3829.492,-1051.883;Inherit;True;Property;_TextureSample2;Texture Sample 2;13;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;123;843.5861,-771.4029;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-3947.825,-625.1096;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1153.7,-512.5818;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Water;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;12;0;11;1
WireConnection;12;1;11;3
WireConnection;13;0;12;0
WireConnection;71;0;70;0
WireConnection;62;0;60;0
WireConnection;76;0;36;0
WireConnection;76;1;77;0
WireConnection;69;0;66;0
WireConnection;69;1;70;0
WireConnection;59;0;76;0
WireConnection;59;1;60;0
WireConnection;63;0;76;0
WireConnection;63;1;62;0
WireConnection;72;0;67;0
WireConnection;72;1;71;0
WireConnection;64;0;59;0
WireConnection;64;2;69;0
WireConnection;65;0;63;0
WireConnection;65;2;72;0
WireConnection;15;0;14;0
WireConnection;15;1;16;0
WireConnection;58;0;57;0
WireConnection;58;1;65;0
WireConnection;58;5;73;0
WireConnection;29;0;57;0
WireConnection;29;1;64;0
WireConnection;29;5;73;0
WireConnection;17;0;15;0
WireConnection;17;1;18;0
WireConnection;8;0;10;0
WireConnection;8;1;9;0
WireConnection;74;0;29;0
WireConnection;74;1;58;0
WireConnection;84;0;80;0
WireConnection;84;1;85;0
WireConnection;75;0;74;0
WireConnection;4;0;17;0
WireConnection;4;2;6;0
WireConnection;4;1;8;0
WireConnection;81;0;84;0
WireConnection;81;1;82;0
WireConnection;43;0;44;0
WireConnection;1;0;4;0
WireConnection;45;0;43;0
WireConnection;109;0;108;0
WireConnection;111;0;112;0
WireConnection;111;1;110;0
WireConnection;31;0;1;0
WireConnection;83;0;79;0
WireConnection;83;1;81;0
WireConnection;113;0;109;0
WireConnection;113;1;111;0
WireConnection;119;0;120;0
WireConnection;86;0;45;0
WireConnection;86;1;83;0
WireConnection;35;0;34;0
WireConnection;24;0;19;0
WireConnection;24;1;25;0
WireConnection;114;0;113;0
WireConnection;46;0;86;0
WireConnection;46;1;47;0
WireConnection;121;0;119;0
WireConnection;33;0;30;0
WireConnection;33;1;32;0
WireConnection;33;2;35;0
WireConnection;20;0;24;0
WireConnection;20;1;1;0
WireConnection;115;0;114;0
WireConnection;127;0;121;0
WireConnection;50;0;46;0
WireConnection;37;0;33;0
WireConnection;48;0;50;0
WireConnection;122;0;127;0
WireConnection;116;0;115;0
WireConnection;52;0;20;0
WireConnection;135;0;53;0
WireConnection;135;1;134;0
WireConnection;135;2;130;0
WireConnection;105;0;104;0
WireConnection;93;0;105;0
WireConnection;89;0;90;0
WireConnection;89;1;91;0
WireConnection;104;0;87;0
WireConnection;104;1;100;0
WireConnection;90;0;88;0
WireConnection;90;1;92;0
WireConnection;97;0;99;0
WireConnection;100;0;97;0
WireConnection;100;1;97;0
WireConnection;87;0;79;0
WireConnection;87;1;89;0
WireConnection;123;0;39;0
WireConnection;123;1;125;0
WireConnection;123;2;126;0
WireConnection;99;0;88;0
WireConnection;99;1;98;0
WireConnection;0;0;123;0
WireConnection;0;1;41;0
WireConnection;0;2;49;0
WireConnection;0;4;28;0
WireConnection;0;11;135;0
ASEEND*/
//CHKSM=2BA5A49BCC0B1FF8A850728C3E5A6CB48A5B1066