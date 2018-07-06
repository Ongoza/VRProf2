Shader "Custom/BackSide" {
Properties {
     _Color ("Main Color", Color) = (1,1,1,0)
     _MainTex ("Base (RGB)", 2D) = "white" {}
     _BumpMap ("Normalmap", 2D) = "bump" {}
 }
 
 SubShader {
     Tags {"RenderType"="Transparent" "Queue"="Transparent"}
 	 Pass {
                 ColorMask 0
             }
             // Render normally
     
     ZWrite Off
     Blend SrcAlpha OneMinusSrcAlpha
     ColorMask RGB
 
 
 CGPROGRAM
 #pragma surface surf Lambert
 
 sampler2D _MainTex;
 sampler2D _BumpMap;
 fixed4 _Color;
 
 struct Input {
     float2 uv_MainTex;
     float2 uv_BumpMap;
 };
 
 void surf (Input IN, inout SurfaceOutput o) {
     //fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
    
     o.Albedo = _Color;
     //o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * IN.color;
     o.Alpha = _Color.a;
      o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
 }
 ENDCG  
 }
 
 FallBack "Diffuse"
 }
