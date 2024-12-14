sampler baseTexture : register(s0);
sampler noiseTexture : register(s1);
sampler distortionTexture : register(s2);

float time;
float manaCapacity;
float pixelationFactor;
float4 palette[8];

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    coords = round(coords / pixelationFactor) * pixelationFactor;
    
    // Distort the noise map using another noise texture.
    float2 distortionUV = float2(coords.x + time * 0.34, coords.y - time * 0.21);
    distortionUV *= 0.3;
    float noiseDistortion = tex2D(distortionTexture, distortionUV).r;
    float2 noiseUV = coords + noiseDistortion * 0.67;
    
    // Scale up the noise map.
    noiseUV *= 0.6;
    // Move the noise map gradually.
    noiseUV.x += time * 0.03;
    float4 noiseColor = tex2D(noiseTexture, noiseUV);
    
    // Saturate the colors depending on the amount of mana available.
    if (noiseColor.a > 0 && coords.y >= manaCapacity)
        noiseColor.rgb = (noiseColor.r + noiseColor.g + noiseColor.b) / 3.0;
    
    float4 tankColor = tex2D(baseTexture, coords);

    float minColorDiff = 10000.0;	
	float4 closestColor = float4(0.0,0.0,0.0,1.0);	
	
	for(int i = 0; i < 8; ++i) {
		float colorDist = distance(palette[i], tankColor * noiseColor * 4);		
		if(colorDist < minColorDiff) {
			minColorDiff = colorDist;	
			closestColor = palette[i];
		}		
	}

    if (coords.y < manaCapacity)
        closestColor.rgba = 0;

    //float4 tankColor = tex2D(baseTexture, coords);
    return closestColor;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}