Shader "Custom/Biomes"
{
    Properties
    {
        _RockColour("Rock Colour", Color) = (1,1,1,1)
        _Color("Colour", Color) = (1,1,1,1)
        _SlopeThreshold("Slope Threshold", Range(0,1)) = .5
        _SlopeBlendAmount("Slope Blend Amount", Range(0,1)) = .5

        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float4 color : COLOR;
            float3 worldPos;
            float3 worldNormal;
        };

        half _MaxHeight;
        half _SlopeThreshold;
        half _SlopeBlendAmount;
        half _Glossiness;
        half _Metallic;
        fixed4 _RockColour;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float slope = 1 - IN.worldNormal.y;
            float blendHeight = _SlopeThreshold * (1 - _SlopeBlendAmount);
            float biomeColorWeight = 1 - saturate((slope - blendHeight) / (_SlopeThreshold - blendHeight));
            o.Albedo = _Color * biomeColorWeight + _RockColour * (1 - blendHeight);
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
