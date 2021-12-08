Shader "Custom/BlendTestShader"
{
     Properties 
     {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {} 
        _BlendTex ("Blend (RGB)", 2D) = "white"
        _BlendAlpha ("Blend Alpha", Float) = 0.5
    }
     SubShader 
     {
        Tags { "Queue"="Transparent" "RenderType" = "Opaque" }
        Lighting Off
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
  
        CGPROGRAM
        #pragma surface surf Lambert alpha
  
        fixed4 _Color;
        sampler2D _MainTex;
        sampler2D _BlendTex;
        float _BlendAlpha;
  
        struct Input {
          float2 uv_MainTex;
        };
  
        void surf (Input IN, inout SurfaceOutput o) {
          fixed4 c = (tex2D( _MainTex, IN.uv_MainTex ) + _BlendAlpha * tex2D( _BlendTex, IN.uv_MainTex ) ) * _Color;
        //   fixed4 c = ( ( 1 - _BlendAlpha ) * tex2D( _MainTex, IN.uv_MainTex ) + _BlendAlpha * tex2D( _BlendTex, IN.uv_MainTex ) ) * _Color;
          o.Albedo = c.rgb;
          o.Alpha = c.a;
        }
        ENDCG
     }
  
     Fallback "Transparent/VertexLit"
 }
