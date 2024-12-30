sampler maskTexture : register(s0);
sampler cloudLayerOneTexture : register(s1);
sampler distortionTexture : register(s2);

float globalTime;
float2 textureSize;
float galaxyOpacity;
float fadeOutMargin;

float2 ApplyRotationToUV(float angle, float2 coords)
{
    float pivot = float2(0.5, 0.5) - coords;   
    float2 preRotatedCoordinates = float2(dot(coords, float2(sin(angle), cos(angle))), dot(coords, float2(cos(angle), -sin(angle))));
    
    coords = preRotatedCoordinates * (coords - pivot) + pivot;
    return coords;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Pixelation.
    float2 pixelSize = 1.1 / textureSize;
    coords = round(coords / pixelSize) * pixelSize;
    
    coords *= 1.25;
    
    // Calculate distortion for the main noise texture.
    float2 distortionCoords = float2(coords.x + globalTime * 0.007, coords.y + globalTime * -0.002);
    distortionCoords *= 0.25;
    float distortion = tex2D(distortionTexture, distortionCoords).r;
    
    // Create two noise maps with the distortion applied.
    float2 layerOneCoords = coords + distortion * 0.28;
    layerOneCoords.x -= globalTime * 0.006;
    layerOneCoords *= 0.4;
    float4 layerOneNoise = tex2D(cloudLayerOneTexture, layerOneCoords);
    
    float2 maskCoords = float2(coords.x - cos(globalTime * 0.04), coords.y + sin(globalTime * 0.06));
    maskCoords *= 0.75;
    float4 mask = tex2D(maskTexture, maskCoords);
    
    // Make everything fade out at a certain margin.
    float4 finalColor = mask * 2.65 * (layerOneNoise * 0.5) * galaxyOpacity;
    finalColor.rgba = finalColor.rgba * (1 - (coords.y * 2 - fadeOutMargin));
    return finalColor;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

