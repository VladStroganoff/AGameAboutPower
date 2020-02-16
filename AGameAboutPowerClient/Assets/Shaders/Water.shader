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
		_SeaFoamTile("Sea Foam Tile", Float) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float4 screenPos;
		};

		uniform float _WaveHeight;
		uniform float _WaveSpeed;
		uniform sampler2D _Texture0;
		uniform float _NormalStrength;
		uniform float _NormalSpeed;
		uniform float _NormalTile;
		uniform float4 _WaterColor;
		uniform float4 _WaveColor;
		uniform sampler2D _SeaFoam;
		uniform float _SeaFoamTile;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _EdgeDistance;
		uniform float _FoamTile;
		uniform float _EdgePower;
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


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float4 appendResult12 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float4 WorldSpaceTile13 = appendResult12;
			float2 panner4 = ( ( _Time.y * _WaveSpeed ) * float2( 1,0 ) + ( ( WorldSpaceTile13 * float4( float2( 1,0.05 ), 0.0 , 0.0 ) ) * 0.5 ).xy);
			float simplePerlin2D1 = snoise( panner4 );
			simplePerlin2D1 = simplePerlin2D1*0.5 + 0.5;
			float3 WaveValueAndHeight52 = ( ( float3(0,1,0) * _WaveHeight ) * simplePerlin2D1 );
			v.vertex.xyz += WaveValueAndHeight52;
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
			float4 SeaFoam93 = tex2D( _SeaFoam, ( ( WorldSpaceTile13 / 10.0 ) * _SeaFoamTile ).xy );
			float2 panner4 = ( ( _Time.y * _WaveSpeed ) * float2( 1,0 ) + ( ( WorldSpaceTile13 * float4( float2( 1,0.05 ), 0.0 , 0.0 ) ) * 0.5 ).xy);
			float simplePerlin2D1 = snoise( panner4 );
			simplePerlin2D1 = simplePerlin2D1*0.5 + 0.5;
			float WavePattern31 = simplePerlin2D1;
			float clampResult35 = clamp( WavePattern31 , 0.0 , 1.0 );
			float4 lerpResult33 = lerp( _WaterColor , ( _WaveColor + SeaFoam93 ) , clampResult35);
			float4 Albedo37 = lerpResult33;
			o.Albedo = Albedo37.rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
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
34;435;1906;510;4217.973;674.5566;1.3;True;True
Node;AmplifyShaderEditor.CommentaryNode;27;-3690.292,-32.10231;Inherit;False;824.0005;235;World Space UVs;3;11;12;13;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;11;-3640.292,17.89769;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;12;-3362.292,19.89769;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;54;-1881.154,-439.4032;Inherit;False;2057.677;879.5825;Wave Value And Height;17;14;16;10;9;15;18;8;6;17;4;1;25;19;24;20;31;52;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;-3095.292,44.89767;Inherit;False;WorldSpaceTile;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;16;-1817.154,-103.4032;Inherit;False;Constant;_Vector0;Vector 0;1;0;Create;True;0;0;False;0;1,0.05;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;14;-1833.154,-183.4032;Inherit;False;13;WorldSpaceTile;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-1609.154,-151.4032;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-1609.154,-23.40319;Inherit;False;Constant;_WaveTile;Wave Tile;1;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;88;-4240.288,-679.2657;Inherit;False;13;WorldSpaceTile;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;92;-4116.686,-474.5585;Inherit;False;Constant;_Float1;Float 0;12;0;Create;True;0;0;False;0;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;51;-4226.673,-1765.522;Inherit;False;2204.954;639.3613;Edge;15;86;79;47;82;85;84;80;81;83;45;48;50;46;43;44;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;10;-1625.154,216.5967;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-1609.154,328.5969;Inherit;False;Property;_WaveSpeed;Wave Speed;0;0;Create;True;0;0;False;0;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;90;-3882.687,-585.5586;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;85;-4003.336,-1352.589;Inherit;False;Constant;_Float0;Float 0;12;0;Create;True;0;0;False;0;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;80;-4139.333,-1456.589;Inherit;False;13;WorldSpaceTile;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;91;-3896.686,-425.5585;Inherit;False;Property;_SeaFoamTile;Sea Foam Tile;13;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-1401.154,-55.40319;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-1353.154,216.5967;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;6;-1401.154,72.59675;Inherit;False;Constant;_WaveDirection;Wave Direction;0;0;Create;True;0;0;False;0;1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;78;960.8204,-2144.006;Inherit;False;2474.489;954;Normal Map;21;36;70;77;60;66;71;62;67;76;59;63;69;72;64;65;57;73;29;58;74;75;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-3783.337,-1303.589;Inherit;False;Property;_FoamTile;Foam Tile;12;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;70;1541.31,-1807.006;Inherit;False;Property;_NormalSpeed;Normal Speed;10;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-3831.902,-1717.154;Inherit;False;Property;_EdgeDistance;Edge Distance;5;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;-3734.372,-596.7375;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;84;-3769.338,-1463.589;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;77;1062.055,-1868.405;Inherit;False;Constant;_Divide;Divide;11;0;Create;True;0;0;False;0;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;1065.288,-1744.833;Inherit;False;Property;_NormalTile;Normal Tile;8;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;36;1010.82,-2016.183;Inherit;False;13;WorldSpaceTile;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PannerNode;4;-1113.154,56.59678;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;79;-4153.57,-1668.638;Inherit;True;Property;_SeaFoam;Sea Foam;11;0;Create;True;0;0;False;0;d01457b88b1c5174ea4235d140b5fab8;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.Vector2Node;66;1542.31,-2094.006;Inherit;False;Constant;_PanDirection;Pan Direction;9;0;Create;True;0;0;False;0;1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;1791.31,-1699.006;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;67;1477.31,-1354.006;Inherit;False;Constant;_PanDirectionMinus;Pan Direction Minus;9;0;Create;True;0;0;False;0;-1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;1327.288,-1639.833;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;1;-905.1542,56.59678;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;87;-3531.907,-733.7761;Inherit;True;Property;_TextureSample2;Texture Sample 2;13;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;76;1272.055,-1906.405;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;-3621.023,-1474.768;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DepthFade;43;-3596.862,-1715.522;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;83;-3480.023,-1590.768;Inherit;True;Property;_TextureSample1;Texture Sample 1;12;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;45;-3256.584,-1702.936;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;1578.284,-1636.885;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;1836.31,-1430.006;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;1811.31,-1946.006;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;93;-3198.75,-712.5334;Inherit;False;SeaFoam;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;1565.991,-1937.185;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;31;-617.1542,136.5967;Inherit;False;WavePattern;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;38;-1371.79,-1336.264;Inherit;False;1512.119;592.4669;Albedo;8;37;33;32;35;30;34;94;95;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;73;2361.31,-1782.006;Inherit;False;Property;_NormalStrength;Normal Strength;9;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-3068.578,-1474.288;Inherit;True;Property;_EdgePower;Edge Power;6;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;86;-3019.327,-1612.591;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;34;-1188.126,-890.7968;Inherit;False;31;WavePattern;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;65;2009.31,-1489.006;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;94;-972.1095,-1007.606;Inherit;False;93;SeaFoam;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.TexturePropertyNode;57;1987.876,-1823.386;Inherit;True;Property;_Texture0;Texture 0;7;1;[Normal];Create;True;0;0;False;0;dd2fd2df93418444c8e280f1d34deeb5;dd2fd2df93418444c8e280f1d34deeb5;True;bump;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.ColorNode;32;-1197.79,-1118.919;Inherit;False;Property;_WaveColor;Wave Color;4;0;Create;True;0;0;False;0;0.2470185,0.3674179,0.5754717,1;0,0.8542309,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;19;-894.699,-229.5455;Inherit;False;Constant;_WaveUp;Wave Up;1;0;Create;True;0;0;False;0;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;25;-894.699,-53.5454;Inherit;False;Property;_WaveHeight;Wave Height;1;0;Create;True;0;0;False;0;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;64;2012.31,-2091.006;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;30;-1061.926,-1286.264;Inherit;False;Property;_WaterColor;Water Color;3;0;Create;True;0;0;False;0;0.1994482,0.2821375,0.509434,1;0.1994482,0.2821375,0.509434,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-2800.793,-1698.684;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;29;2393.525,-2078.372;Inherit;True;Property;_NormalMap;Normal Map;3;1;[Normal];Create;True;0;0;False;0;-1;dd2fd2df93418444c8e280f1d34deeb5;dd2fd2df93418444c8e280f1d34deeb5;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;58;2362.304,-1587.809;Inherit;True;Property;_TextureSample0;Texture Sample 0;3;1;[Normal];Create;True;0;0;False;0;-1;dd2fd2df93418444c8e280f1d34deeb5;dd2fd2df93418444c8e280f1d34deeb5;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;35;-763.9413,-848.8093;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;95;-758.1095,-1098.606;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-654.699,-181.5454;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-345.1541,-71.40319;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClampOpNode;50;-2629.27,-1654.919;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendNormalsNode;74;2868.31,-1757.006;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;33;-543.2627,-1123.035;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;37;-83.67103,-1102.366;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;48;-2434.793,-1640.684;Inherit;False;Edge;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;3211.31,-1767.006;Inherit;False;NormalMaps;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;52;-89.15411,-71.40319;Inherit;False;WaveValueAndHeight;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;100;-2962.173,-453.5567;Inherit;False;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;41;801.527,-492.2475;Inherit;False;75;NormalMaps;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;39;812.4116,-587.4029;Inherit;False;37;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;53;894.0372,-214.7177;Inherit;False;52;WaveValueAndHeight;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;28;878.6054,-347.2183;Inherit;False;Property;_Smothness;Smothness;2;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;49;682.9608,-421.9758;Inherit;False;48;Edge;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-3531.191,-331.9363;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PannerNode;97;-3306.191,-433.9363;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;98;-3816.092,-277.2363;Inherit;False;Property;_FoamMask;Foam Mask;14;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;101;-2631.973,-439.2566;Inherit;False;FoamMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1153.7,-512.5818;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Water;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;12;0;11;1
WireConnection;12;1;11;3
WireConnection;13;0;12;0
WireConnection;15;0;14;0
WireConnection;15;1;16;0
WireConnection;90;0;88;0
WireConnection;90;1;92;0
WireConnection;17;0;15;0
WireConnection;17;1;18;0
WireConnection;8;0;10;0
WireConnection;8;1;9;0
WireConnection;89;0;90;0
WireConnection;89;1;91;0
WireConnection;84;0;80;0
WireConnection;84;1;85;0
WireConnection;4;0;17;0
WireConnection;4;2;6;0
WireConnection;4;1;8;0
WireConnection;71;0;70;0
WireConnection;62;0;60;0
WireConnection;1;0;4;0
WireConnection;87;0;79;0
WireConnection;87;1;89;0
WireConnection;76;0;36;0
WireConnection;76;1;77;0
WireConnection;81;0;84;0
WireConnection;81;1;82;0
WireConnection;43;0;44;0
WireConnection;83;0;79;0
WireConnection;83;1;81;0
WireConnection;45;0;43;0
WireConnection;63;0;76;0
WireConnection;63;1;62;0
WireConnection;72;0;67;0
WireConnection;72;1;71;0
WireConnection;69;0;66;0
WireConnection;69;1;70;0
WireConnection;93;0;87;0
WireConnection;59;0;76;0
WireConnection;59;1;60;0
WireConnection;31;0;1;0
WireConnection;86;0;45;0
WireConnection;86;1;83;0
WireConnection;65;0;63;0
WireConnection;65;2;72;0
WireConnection;64;0;59;0
WireConnection;64;2;69;0
WireConnection;46;0;86;0
WireConnection;46;1;47;0
WireConnection;29;0;57;0
WireConnection;29;1;64;0
WireConnection;29;5;73;0
WireConnection;58;0;57;0
WireConnection;58;1;65;0
WireConnection;58;5;73;0
WireConnection;35;0;34;0
WireConnection;95;0;32;0
WireConnection;95;1;94;0
WireConnection;24;0;19;0
WireConnection;24;1;25;0
WireConnection;20;0;24;0
WireConnection;20;1;1;0
WireConnection;50;0;46;0
WireConnection;74;0;29;0
WireConnection;74;1;58;0
WireConnection;33;0;30;0
WireConnection;33;1;95;0
WireConnection;33;2;35;0
WireConnection;37;0;33;0
WireConnection;48;0;50;0
WireConnection;75;0;74;0
WireConnection;52;0;20;0
WireConnection;100;1;97;0
WireConnection;99;0;88;0
WireConnection;99;1;98;0
WireConnection;97;0;99;0
WireConnection;101;0;100;0
WireConnection;0;0;39;0
WireConnection;0;1;41;0
WireConnection;0;2;49;0
WireConnection;0;4;28;0
WireConnection;0;11;53;0
ASEEND*/
//CHKSM=5037A50DA6521C9D12EB0BACE122CC8417EF4DBA