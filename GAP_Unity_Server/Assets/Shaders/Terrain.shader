// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Terrain"
{
	Properties
	{
		_MainTexture("Main Texture", 2D) = "white" {}
		_BigMainTex("Big Main Tex", 2D) = "white" {}
		_EdgeLength ( "Edge length", Range( 2, 50 ) ) = 15
		_AmbientOcclusion("Ambient Occlusion", 2D) = "white" {}
		_Smooth("Smooth", Range( 0 , 1)) = 0
		_BigUVTile("Big UV Tile", Float) = 2
		[Normal]_NormalMap("Normal Map", 2D) = "white" {}
		_Tile("Tile", Float) = 0
		_NormalPower("Normal Power", Float) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float3 worldPos;
		};

		uniform sampler2D _NormalMap;
		uniform float _Tile;
		uniform float _NormalPower;
		uniform sampler2D _BigMainTex;
		uniform float _BigUVTile;
		uniform sampler2D _MainTexture;
		uniform float _Smooth;
		uniform sampler2D _AmbientOcclusion;
		uniform float _EdgeLength;

		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
		}

		void vertexDataFunc( inout appdata_full v )
		{
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float4 appendResult4 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float4 WorldUV5 = ( appendResult4 * _Tile );
			o.Normal = UnpackScaleNormal( tex2D( _NormalMap, WorldUV5.xy ), _NormalPower );
			float4 BigWorldUV31 = ( WorldUV5 / _BigUVTile );
			float4 tex2DNode2 = tex2D( _MainTexture, WorldUV5.xy );
			o.Albedo = ( tex2D( _BigMainTex, BigWorldUV31.xy ) * tex2DNode2 ).rgb;
			o.Smoothness = ( tex2DNode2.a * _Smooth );
			o.Occlusion = tex2D( _AmbientOcclusion, WorldUV5.xy ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
137;271;1906;861;2427.373;-214.3854;1.601717;True;True
Node;AmplifyShaderEditor.CommentaryNode;6;-3149.39,72.65177;Inherit;False;732.5344;413.3714;UVs;5;5;21;22;4;3;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;3;-3099.389,125.6869;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;22;-3005.086,335.9212;Inherit;False;Property;_Tile;Tile;11;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;4;-2896.283,132.6518;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-2731.702,190.4675;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;5;-2607.855,214.7537;Inherit;False;WorldUV;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;28;-3101.469,-206.7667;Inherit;False;5;WorldUV;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-3092.087,-124.205;Inherit;False;Property;_BigUVTile;Big UV Tile;9;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;36;-2770.699,-203.9492;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;31;-2525.413,-201.1375;Inherit;False;BigWorldUV;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TexturePropertyNode;10;-1408.585,254.2879;Inherit;True;Property;_NormalMap;Normal Map;10;1;[Normal];Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;1;-1397.124,-241.5999;Inherit;True;Property;_MainTexture;Main Texture;0;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;33;-1149.737,-450.9998;Inherit;False;31;BigWorldUV;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;24;-1135.251,-155.1302;Inherit;False;5;WorldUV;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TexturePropertyNode;32;-1411.61,-537.4695;Inherit;True;Property;_BigMainTex;Big Main Tex;1;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;23;-1131.147,367.3986;Inherit;False;5;WorldUV;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-447.993,440.8548;Inherit;False;Property;_NormalPower;Normal Power;12;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;7;-1141.54,95.90805;Inherit;False;5;WorldUV;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;34;-863.7861,-503.2884;Inherit;True;Property;_TextureSample3;Texture Sample 0;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;14;-1405.867,15.15119;Inherit;True;Property;_AmbientOcclusion;Ambient Occlusion;7;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;11;-871.8395,301.4659;Inherit;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-863.1135,-215.7076;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;26;-554.0402,-16.0781;Inherit;False;Property;_Smooth;Smooth;8;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-222.2643,-48.61286;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.UnpackScaleNormalNode;27;-258.6347,317.1573;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-133.616,-359.3007;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;15;-859.6331,33.1928;Inherit;True;Property;_TextureSample2;Texture Sample 2;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;374.1128,53.38984;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;Terrain;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;2;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;3;1
WireConnection;4;1;3;3
WireConnection;21;0;4;0
WireConnection;21;1;22;0
WireConnection;5;0;21;0
WireConnection;36;0;28;0
WireConnection;36;1;30;0
WireConnection;31;0;36;0
WireConnection;34;0;32;0
WireConnection;34;1;33;0
WireConnection;11;0;10;0
WireConnection;11;1;23;0
WireConnection;2;0;1;0
WireConnection;2;1;24;0
WireConnection;25;0;2;4
WireConnection;25;1;26;0
WireConnection;27;0;11;0
WireConnection;27;1;13;0
WireConnection;35;0;34;0
WireConnection;35;1;2;0
WireConnection;15;0;14;0
WireConnection;15;1;7;0
WireConnection;0;0;35;0
WireConnection;0;1;27;0
WireConnection;0;4;25;0
WireConnection;0;5;15;0
ASEEND*/
//CHKSM=0976DE837A0C005845F5710B78FB8CAFEBF03EDF