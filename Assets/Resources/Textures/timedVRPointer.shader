Shader "Custom/timedVRPointer" {
  Properties {
    _Color  ("Color", Color) = ( 1, 1, 0, 1 )
	_Angle  ("Angle", Range(0, 360)) = 180
    _InnerDiameter ("InnerDiameter", Range(0, 10.0)) = 1.5
    _OuterDiameter ("OuterDiameter", Range(0.00872665, 10.0)) = 2.0
    _DistanceInMeters ("DistanceInMeters", Range(0.0, 100.0)) = 2.0
  }

  SubShader {
    Tags { "Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent" }
    Pass {
      Blend SrcAlpha OneMinusSrcAlpha
      AlphaTest Off
      Cull Back
      Lighting Off
      ZWrite Off
      ZTest Always

      Fog { Mode Off }
      CGPROGRAM

      #pragma vertex vert
      #pragma fragment frag
	  #pragma enable_d3d11_debug_symbols

      #include "UnityCG.cginc"

      uniform float4 _Color;
      uniform float _InnerDiameter;
      uniform float _OuterDiameter;
      uniform float _DistanceInMeters;
	  uniform float _Angle;

      struct vertexInput {
        float4 vertex : POSITION;
      };

      struct fragmentInput{
          float4 position : SV_POSITION;	  
      };

      fragmentInput vert(vertexInput i) {        		
		fragmentInput o;
		if(_DistanceInMeters<20){
			// 180* = 2.9
			// limit = - (a - 180) * 2.9/180
			float limit = - (_Angle-180)*0.0161111111;
			float3 vert_out = float3(0, 0, _DistanceInMeters);
			float a =  - atan2( i.vertex.x , i.vertex.y );
			if(a >= limit){ 
				float scale = lerp(_OuterDiameter, _InnerDiameter, i.vertex.z);
				vert_out = float3(i.vertex.x * scale, i.vertex.y * scale, _DistanceInMeters);
			}
			o.position = UnityObjectToClipPos (vert_out);		
		}
		return o;
      }

      fixed4 frag(fragmentInput i) : SV_Target {       
        return _Color;
      }

      ENDCG
    }
  }
}
