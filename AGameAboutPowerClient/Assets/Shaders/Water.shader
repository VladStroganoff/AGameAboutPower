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
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _EdgeDistance;
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
			float2 panner4 = ( ( _Time.y * _WaveSpeed ) * float2( 1,0 ) + ( ( WorldSpaceTile13 * float4( float2( 1,0.05 ), 0.0 , 0.0 ) ) * 0.5 ).xy);
			float simplePerlin2D1 = snoise( panner4 );
			simplePerlin2D1 = simplePerlin2D1*0.5 + 0.5;
			float WavePattern31 = simplePerlin2D1;
			float clampResult35 = clamp( WavePattern31 , 0.0 , 1.0 );
			float4 lerpResult33 = lerp( _WaterColor , _WaveColor , clampResult35);
			float4 Albedo37 = lerpResult33;
			o.Albedo = Albedo37.rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth43 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth43 = abs( ( screenDepth43 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _EdgeDistance ) );
			float clampResult50 = clamp( ( ( 1.0 - distanceDepth43 ) * _EdgePower ) , 0.0 , 1.0 );
			float Edge48 = clampResult50;
			float3 temp_cast_5 = (Edge48).xxx;
			o.Emission = temp_cast_5;
			o.Smoothness = _Smothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
-590;526;1906;516;825.5679;2145.984;2.15048;True;True
Node;AmplifyShaderEditor.CommentaryNode;27;-3690.292,-32.10231;Inherit;False;824.0005;235;World Space UVs;3;11;12;13;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;11;-3640.292,17.89769;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;12;-3362.292,19.89769;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;54;-1931.629,-344.5641;Inherit;False;2057.677;879.5825;Wave Value And Height;17;14;16;10;9;15;18;8;6;17;4;1;25;19;24;20;31;52;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;-3095.292,44.89767;Inherit;False;WorldSpaceTile;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;16;-1857.672,-1.175507;Inherit;False;Constant;_Vector0;Vector 0;1;0;Create;True;0;0;False;0;1,0.05;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;14;-1881.629,-90.93009;Inherit;False;13;WorldSpaceTile;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-1658.414,-57.5981;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-1656.227,67.19557;Inherit;False;Constant;_WaveTile;Wave Tile;1;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;10;-1674.565,309.9902;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-1653.574,419.0186;Inherit;False;Property;_WaveSpeed;Wave Speed;0;0;Create;True;0;0;False;0;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-1399.242,306.6243;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;6;-1448.386,171.5134;Inherit;False;Constant;_WaveDirection;Wave Direction;0;0;Create;True;0;0;False;0;1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;78;960.8204,-2144.006;Inherit;False;2474.489;954;Normal Map;21;36;70;77;60;66;71;62;67;76;59;63;69;72;64;65;57;73;29;58;74;75;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-1442.508,47.56972;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;77;1062.055,-1868.405;Inherit;False;Constant;_Divide;Divide;11;0;Create;True;0;0;False;0;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;70;1541.31,-1807.006;Inherit;False;Property;_NormalSpeed;Normal Speed;10;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;4;-1165.209,157.4256;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;51;-1353.961,-1707.576;Inherit;False;1494;310.3678;Edge;7;44;43;45;47;46;50;48;;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;36;1010.82,-2016.183;Inherit;False;13;WorldSpaceTile;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;60;1065.288,-1744.833;Inherit;False;Property;_NormalTile;Normal Tile;8;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-1303.961,-1645.208;Inherit;False;Property;_EdgeDistance;Edge Distance;5;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;1327.288,-1639.833;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;76;1272.055,-1906.405;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;67;1477.31,-1354.006;Inherit;False;Constant;_PanDirectionMinus;Pan Direction Minus;9;0;Create;True;0;0;False;0;-1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;1791.31,-1699.006;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;66;1542.31,-2094.006;Inherit;False;Constant;_PanDirection;Pan Direction;9;0;Create;True;0;0;False;0;1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.NoiseGeneratorNode;1;-942.7029,158.6994;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;1565.991,-1937.185;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;1836.31,-1430.006;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;1811.31,-1946.006;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;1578.284,-1636.885;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DepthFade;43;-1057.922,-1657.576;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;31;-659.9772,226.2325;Inherit;False;WavePattern;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;38;-1115.79,-1336.264;Inherit;False;1256.119;513.4669;Albedo;6;34;30;32;35;33;37;;1,1,1,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;45;-637.9612,-1638.208;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;34;-1040.126,-938.7968;Inherit;False;31;WavePattern;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;73;2361.31,-1782.006;Inherit;False;Property;_NormalStrength;Normal Strength;9;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;57;1987.876,-1823.386;Inherit;True;Property;_Texture0;Texture 0;7;1;[Normal];Create;True;0;0;False;0;dd2fd2df93418444c8e280f1d34deeb5;dd2fd2df93418444c8e280f1d34deeb5;True;bump;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.PannerNode;65;2009.31,-1489.006;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;64;2012.31,-2091.006;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-633.9612,-1513.208;Inherit;False;Property;_EdgePower;Edge Power;6;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;19;-1012.273,-294.5641;Inherit;False;Constant;_WaveUp;Wave Up;1;0;Create;True;0;0;False;0;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;25;-1015.659,-123.5987;Inherit;False;Property;_WaveHeight;Wave Height;1;0;Create;True;0;0;False;0;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;58;2362.304,-1587.809;Inherit;True;Property;_TextureSample0;Texture Sample 0;3;1;[Normal];Create;True;0;0;False;0;-1;dd2fd2df93418444c8e280f1d34deeb5;dd2fd2df93418444c8e280f1d34deeb5;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;29;2393.525,-2078.372;Inherit;True;Property;_NormalMap;Normal Map;3;1;[Normal];Create;True;0;0;False;0;-1;dd2fd2df93418444c8e280f1d34deeb5;dd2fd2df93418444c8e280f1d34deeb5;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-449.9616,-1655.208;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;35;-771.9413,-993.8093;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;32;-1065.79,-1108.919;Inherit;False;Property;_WaveColor;Wave Color;4;0;Create;True;0;0;False;0;0.2470185,0.3674179,0.5754717,1;0,0.8542309,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-778.6594,-239.5986;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;30;-1061.926,-1286.264;Inherit;False;Property;_WaterColor;Water Color;3;0;Create;True;0;0;False;0;0.1994482,0.2821375,0.509434,1;0.1994482,0.2821375,0.509434,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;50;-278.4391,-1611.443;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-383.5729,30.13033;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BlendNormalsNode;74;2868.31,-1757.006;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;33;-543.2627,-1123.035;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;3211.31,-1767.006;Inherit;False;NormalMaps;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;37;-83.67103,-1102.366;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;48;-83.96158,-1597.208;Inherit;False;Edge;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;52;-136.9516,33.26687;Inherit;False;WaveValueAndHeight;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;53;894.0372,-214.7177;Inherit;False;52;WaveValueAndHeight;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;28;878.6054,-347.2183;Inherit;False;Property;_Smothness;Smothness;2;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;39;881.4116,-551.4029;Inherit;False;37;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;41;854.527,-481.2475;Inherit;False;75;NormalMaps;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;49;709.9608,-376.9758;Inherit;False;48;Edge;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1153.7,-512.5818;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Water;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;12;0;11;1
WireConnection;12;1;11;3
WireConnection;13;0;12;0
WireConnection;15;0;14;0
WireConnection;15;1;16;0
WireConnection;8;0;10;0
WireConnection;8;1;9;0
WireConnection;17;0;15;0
WireConnection;17;1;18;0
WireConnection;4;0;17;0
WireConnection;4;2;6;0
WireConnection;4;1;8;0
WireConnection;62;0;60;0
WireConnection;76;0;36;0
WireConnection;76;1;77;0
WireConnection;71;0;70;0
WireConnection;1;0;4;0
WireConnection;59;0;76;0
WireConnection;59;1;60;0
WireConnection;72;0;67;0
WireConnection;72;1;71;0
WireConnection;69;0;66;0
WireConnection;69;1;70;0
WireConnection;63;0;76;0
WireConnection;63;1;62;0
WireConnection;43;0;44;0
WireConnection;31;0;1;0
WireConnection;45;0;43;0
WireConnection;65;0;63;0
WireConnection;65;2;72;0
WireConnection;64;0;59;0
WireConnection;64;2;69;0
WireConnection;58;0;57;0
WireConnection;58;1;65;0
WireConnection;58;5;73;0
WireConnection;29;0;57;0
WireConnection;29;1;64;0
WireConnection;29;5;73;0
WireConnection;46;0;45;0
WireConnection;46;1;47;0
WireConnection;35;0;34;0
WireConnection;24;0;19;0
WireConnection;24;1;25;0
WireConnection;50;0;46;0
WireConnection;20;0;24;0
WireConnection;20;1;1;0
WireConnection;74;0;29;0
WireConnection;74;1;58;0
WireConnection;33;0;30;0
WireConnection;33;1;32;0
WireConnection;33;2;35;0
WireConnection;75;0;74;0
WireConnection;37;0;33;0
WireConnection;48;0;50;0
WireConnection;52;0;20;0
WireConnection;0;0;39;0
WireConnection;0;1;41;0
WireConnection;0;2;49;0
WireConnection;0;4;28;0
WireConnection;0;11;53;0
ASEEND*/
//CHKSM=D19E8BA5646A80B0EE2FB08399222875989691D4