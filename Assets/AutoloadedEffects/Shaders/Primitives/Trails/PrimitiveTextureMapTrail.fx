sampler mapTexture : register(s1);
matrix uWorldViewProjection;

bool flipVertically;
bool flipHorizontally;
float mapTextureSize;
float textureScaleFactor;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
    float4 pos = mul(input.Position, uWorldViewProjection);
    output.Position = pos;
    
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    output.TextureCoordinates.y = (output.TextureCoordinates.y - 0.5) / input.TextureCoordinates.z + 0.5;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 baseColor = input.Color;
    float2 baseCoords = input.TextureCoordinates;
  
    float2 coords = baseCoords * textureScaleFactor / mapTextureSize;
    coords.x = 1 - frac(coords.x);
    coords.y = saturate(baseCoords.y);
    if (flipVertically)
        coords.y = 1 - coords.y;
    if (flipHorizontally)
        coords.x = 1 - coords.x;
    
    float4 textureColor = tex2D(mapTexture, coords);
    return textureColor * baseColor;
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
