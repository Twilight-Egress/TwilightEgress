sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float4 uShaderSpecificData;
float uNoiseScale;

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 color = tex2D(uImage0, input.TextureCoordinates);
    
    float time = uTime * 0.75;
    float4 scrollColor1 = tex2D(uImage1, input.TextureCoordinates * 2 + float2(sin(time * 0.12) * 0.5, time * 0.05));
    float4 scrollColor2 = tex2D(uImage1, input.TextureCoordinates * 3 + float2(sin(time * -0.07) * -0.3, time * -0.02));
    float4 scrollColor3 = tex2D(uImage1, input.TextureCoordinates * 4 + float2(sin(time * -0.02) * -0.1, time * -0.09));
    
    float result = (scrollColor1 + scrollColor2 + scrollColor3) * 0.4;
    color.rgb *= uColor.rgb * result;
    return color * input.Color.a;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
