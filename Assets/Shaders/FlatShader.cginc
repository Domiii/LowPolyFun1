#if !defined(FLAT_WIREFRAME_INCLUDED)
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members texcoord1,texcoord2)
#pragma exclude_renderers d3d11
#define FLAT_WIREFRAME_INCLUDED

//#include "My Lighting.cginc"

#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"


//
//struct InterpolatorsVertex {
//	float4 pos : POSITION;
//	nointerpolation float4 color: COLOR;
//	float4 uv : TEXCOORD0;
//	float3 normal : NORMAL;
//};

struct v2f
{
    float4 vertex : POSITION;
    float3 normal : NORMAL;
	//nointerpolation float4 color: COLOR;
	float4 color: COLOR;
	float2 texcoord1;
	float2 texcoord2;
    //float2 texcoord1 : TEXCOORD0;
    //fixed4 diff : COLOR1; // diffuse lighting color
};

//
//[maxvertexcount(3)]
//void FlatGeom (
//	triangle v2f i[3],
//	inout TriangleStream<v2f> stream
//) {
//	float3 p0 = i[0].pos.xyz;
//	float3 p1 = i[1].pos.xyz;
//	float3 p2 = i[2].pos.xyz;
//
//	float3 triangleNormal = normalize(cross(p1 - p0, p2 - p0));
//	i[0].normal = triangleNormal;
//	i[1].normal = triangleNormal;
//	i[2].normal = triangleNormal;
//
//	stream.Append(i[0]);
//	stream.Append(i[1]);
//	stream.Append(i[2]);
//}

#endif