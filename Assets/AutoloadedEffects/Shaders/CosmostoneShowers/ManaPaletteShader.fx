sampler baseTexture : register(s0);

float globalTime;
float timeMultiplier;
float flowCompactness;
float gradientPrecision;
float4 palette[10];
float opacity;

float4 PaletteLerp(float interpolant)
{
    // These 5s should be 2 less than the length of the palette array.
    int startIndex = clamp(interpolant * 8, 0, 8);
    int endIndex = startIndex + 1;
    return lerp(palette[startIndex], palette[endIndex], frac(interpolant * 8));
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 baseColor = tex2D(baseTexture, coords);
    float paletteInterpolant = sin((baseColor.r + globalTime * timeMultiplier) * flowCompactness) * 0.5 + 0.5;
    float4 paletteSample = PaletteLerp(paletteInterpolant);
    
    // Apply posterization.
    //paletteSample = round(paletteSample * gradientPrecision) / gradientPrecision;
    
    return paletteSample * baseColor.a * opacity;
}
technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}