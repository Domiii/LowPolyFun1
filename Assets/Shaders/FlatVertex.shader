Shader "FlatVertex" {
	Properties {
		_Color ("Tint", Color) = (1,1,1,1)
		_MainTex ("Color Scheme", 2D) = "white" {}
	}
	SubShader {
		//Tags { "RenderType"="Transparent" }
		Tags { "RenderType"="Opaque" }
		//Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

		LOD 200
//      Cull Off
//     	ZWrite Off
 		//Blend SrcAlpha OneMinusSrcAlpha 
 		//Blend One OneMinusSrcAlpha
 		//Blend SrcAlpha OneMinusSrcAlpha

        //Tags {"LightMode"="ForwardBase"}

        //SetTexture [_MainTex] { combine texture }
		
		CGPROGRAM
		#pragma target 4.0
		//#pragma vertex vert
		//#pragma geometry FlatGeom
		//#pragma fragment frag	
		#pragma surface surf Standard //alpha

		sampler2D _MainTex;
		fixed4 _Color;

		#include "FlatShader.cginc"

		struct Input {
            float2 uv_MainTex;
            nointerpolation float4 color: Color; // Vertex color
        };
 
        void surf (Input IN, inout SurfaceOutputStandard o) {
            half4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb * IN.color.rgb * _Color.rgb; // vertex RGB
            o.Alpha = c.a * IN.color.a * _Color.a; // vertex Alpha
        }

		void vert (inout appdata_full v) {
			//v2f o;
			//appdata_full o = v;
			//v.pos= UnityObjectToClipPos(v.pos);
			//v.color = v.color * _Color;


//                // get vertex normal in world space
//                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
//                // dot product between normal and light direction for
//                // standard diffuse (Lambert) lighting
//                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
//                // factor in the light color
//                o.diff = nl * _LightColor0;

            //o.diff = fixed4(1,1,1,1);

            // add ambient light
            //o.diff.rgb += ShadeSH9(half4(worldNormal,1));

            //return o;
        }
        
//        float4 frag (v2f v) : SV_Target
//        {
//            // sample the texture
//            //fixed4 col = ;
//
//            //float4 col = tex2D(_MainTex, v.uv) * v.color * v.diff;
//            float4 col = tex2D(_MainTex, v.uv) * v.color;
//            return col;
//            //return float4(1,0,0,1);
//        }

		ENDCG
	}
	Fallback " VertexLit", 1
}
