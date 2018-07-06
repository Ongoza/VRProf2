Shader "Custom/yuoTubeShaderTBB_counterClock" {

Properties {
    _Tint ("Tint Color", Color) = (1,1,1,1)
    [Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
    _Rotation ("Rotation", Range(0, 360)) = 0
    [NoScaleOffset] _MainTex ("Spherical  (HDR)", 2D) = "grey" {}
    [KeywordEnum(6 Frames Layout)] _Mapping("Mapping", Float) = 1
}

SubShader {
    Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
    Cull Off ZWrite Off

    Pass {

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        #pragma multi_compile __ _MAPPING_6_FRAMES_LAYOUT

        #include "UnityCG.cginc"

        sampler2D _MainTex;
        float4 _MainTex_TexelSize;
        half4 _MainTex_HDR;
        half4 _Tint;
        half _Exposure;
        float _Rotation;
        bool _MirrorOnBack;
        int _ImageType;
        int _Layout;

        float3 RotateAroundYInDegrees (float3 vertex, float degrees) { 
        	float alpha = degrees * UNITY_PI / 180.0; float sina, cosa; sincos(alpha, sina, cosa);
            float2x2 m = float2x2(cosa, -sina, sina, cosa);
            return float3(mul(m, vertex.xz), vertex.y).xzy;
        }

        struct appdata_t {  float4 vertex : POSITION;   UNITY_VERTEX_INPUT_INSTANCE_ID   };

        struct v2f {
            float4 vertex : SV_POSITION;
            float3 texcoord : TEXCOORD0;
            float3 layout : TEXCOORD1;
            float4 edgeSize : TEXCOORD2;
            float4 faceXCoordLayouts : TEXCOORD3;
            float4 faceYCoordLayouts : TEXCOORD4;
            float4 faceZCoordLayouts : TEXCOORD5;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        v2f vert (appdata_t v) { v2f o;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
          //  float3 rotated = RotateAroundYInDegrees(v.vertex, _Rotation);
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.texcoord = v.vertex.xyz;
            o.faceXCoordLayouts = float4(0.5,1.5,2.5,1.5); //  1 -3 // left rigth
            o.faceYCoordLayouts = float4(2.5,0.5,0.5,0.5); //  4 - 6 -- turn 4 on -90 and 6 -90 // top bottom
            o.faceZCoordLayouts = float4(1.5,0.5,1.5,1.5); //  5 -2 -- 5 turn on +90  //  back forward
            o.layout = float3(0,1.0/3.0,1.0/2.0);
//          // edgeSize specifies the minimum (xy) and maximum (zw) normalized face texture coordinates that will be used for
//          // sampling in the texture. Setting these to the effective size of a half pixel horizontally and vertically
//          // effectively enforces clamp mode texture wrapping for each individual face.
            o.edgeSize.xy = _MainTex_TexelSize.xy * 0.5 / o.layout.yz - 0.5;
            o.edgeSize.zw = -o.edgeSize.xy;
            return o;
        }

        fixed4 frag (v2f i) : SV_Target {
            float3 coords = i.texcoord;
            float3 absn = abs(coords);
            float3 absdir =  absn > float3(max(absn.y,absn.z), max(absn.x,absn.z), max(absn.x,absn.y)) ? 1 : 0;
            float3 tcAndLen = mul(absdir, float3x3(coords.zyx, coords.xzy, float3(-coords.xy,coords.z)));
            tcAndLen.xy /= tcAndLen.z;
            // Flip-flop faces for proper orientation and normalize to [-0.5,+0.5]
            bool2 positiveAndVCross = float2(tcAndLen.z, i.layout.x) > 0;
            tcAndLen.xy *= (positiveAndVCross[0] ? absdir.yx : 	(positiveAndVCross[1] ? float2(absdir[2],0) : float2(0,absdir[2]))) - 0.5;
//            float2x2 rotMtrxPos = float2x2( 0, 1, -1, 0); // +90
//            float2x2 rotMtrxNeg = float2x2( 0, -1, 1, 0); // -90
            if (positiveAndVCross[0]){ // 1,4,5    && absdir.y>0=4 absdir.z>0==5
            	if((absdir.z>0) || (absdir.y>0)){
            		tcAndLen.xy =  mul(tcAndLen.xy,float2x2( 0, -1, 1, 0)); 
            	}
            }else{  // 6
             	if(absdir.y>0){ tcAndLen.xy =  mul(tcAndLen.xy,float2x2( 0, -1, 1, 0)); }}
            // Clamp values which are close to the face edges to avoid bleeding/seams (ie. enforce clamp texture wrap mode)
          //  tcAndLen.xy = clamp(tcAndLen.xy, i.edgeSize.xy, i.edgeSize.zw);
            // Scale and offset texture coord to match the proper square in the texture based on layout.
            float4 coordLayout = mul(float4(absdir,0), float4x4(i.faceXCoordLayouts, i.faceYCoordLayouts, i.faceZCoordLayouts, i.faceZCoordLayouts));
            tcAndLen.xy = (tcAndLen.xy + (positiveAndVCross[0] ? coordLayout.xy : coordLayout.zw)) * i.layout.yz;
            half4 tex = tex2D (_MainTex, tcAndLen.xy);
            half3 c = DecodeHDR (tex, _MainTex_HDR);
//            c = c * _Tint.rgb * unity_ColorSpaceDouble.rgb;
//            c *= _Exposure;
            return half4(c, 1);
        }
        ENDCG
    }
}


CustomEditor "SkyboxPanoramicShaderGUI"
Fallback Off

} 