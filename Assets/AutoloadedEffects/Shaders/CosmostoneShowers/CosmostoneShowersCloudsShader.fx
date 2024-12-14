sampler maskTexture : register(s0);
sampler noiseTetxure : register(s1);
sampler distortionTexture : register(s2);
sampler erosionTexture : register(s3);

float globalTime;
float2 textureSize;
float cloudOpacity;
float erosionStrength;
float fadeOutMarginTop;
float fadeOutMarginBottom;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Pixelation.
    float2 pixelSize = 1.5 / textureSize;
    coords = round(coords / pixelSize) * pixelSize;
    
    // Squish everything so that it looks more like a condensed cloud.
    coords *= float2(0.5, 1);
    
    // Calculate distortion for the main noise texture.
    float2 distortionCoords = float2(coords.x - globalTime * 0.02, coords.y + globalTime * -0.007);
    distortionCoords *= 0.2;
    float distortion = tex2D(distortionTexture, distortionCoords).r;
    
    // Create a noise map with the distortion applied.
    float2 cloudCoords = coords + distortion * 0.28;
    cloudCoords.x -= globalTime * 0.008;
    float4 clouds = tex2D(noiseTetxure, cloudCoords);
    
    // Mask the clouds over a primary texture.
    float2 maskCoords = float2(coords.x - globalTime * 0.01, coords.y + globalTime * 0.02);
    float4 mask = tex2D(maskTexture, maskCoords + distortion * 0.18);
    
    // Fade out the coordinates at specific screen margins.
    float fadeOutRegion = pow(coords.y, 3) * smoothstep(fadeOutMarginTop, fadeOutMarginBottom, coords.y);
    
    // Apply erosion to the final color.
    float2 erosionCoords = float2(coords.x - globalTime * 0.025, coords.y - globalTime * 0.003);
    float4 erosion = tex2D(erosionTexture, erosionCoords + distortion * 0.32);
    float erosionLevel = smoothstep(0, erosionStrength, erosion.r);
    
    float4 finalColor = ((mask * 12 * clouds * fadeOutRegion) * cloudOpacity);
    finalColor.a *= erosionLevel;
   
    return finalColor;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
