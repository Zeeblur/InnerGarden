Shader "Custom/Waves"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}
       	_Color("Color", Color) = (1, 1, 1, 1)

        // Waves
        _Direction("Direction", Vector) = (1.0,0.0,0.0,1.0)
        _Steepness("Steepness", Range(0.001, 1.0)) = 0.5
        _Freq("Frequency", Range (1.0, 10.0)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off
        //LOD 200
        
        CGPROGRAM
        #pragma surface surf Standard vertex:vert
        
        sampler2D _MainTex;
        sampler2D _NormalMap;

        // Wave props
        float _Steepness, _Freq;
        float4 _Direction;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float2 uv : TEXCOORD0;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert(inout appdata_full v)
        {



            // Wave Displacement
            float3 pos = v.vertex.xyz;
            float4 dir = normalize(_Direction);
            float defaultWavelength = 2 * UNITY_PI;

            // Amplitude * Sin ( wavelenght + phase offset)
            float wL = defaultWavelength / _Freq;
            float phase = sqrt(9.8 / wL);
            float disp = wL * (dot(dir, pos) - (phase * _Time.y));
            float peak = _Steepness / wL;



            if (v.vertex.y >= 0.1)
            {
                pos.x += dir.x * (peak * cos(disp));
                pos.z += dir.y * (peak * cos(disp));
            }

            v.vertex.xyz = pos;

            float d = tex2Dlod(_NormalMap, float4(v.texcoord.xy, 0, 0)).r * _Steepness;
            v.vertex.xyz -= v.normal * d;

        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
