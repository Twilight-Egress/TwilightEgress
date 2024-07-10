float globalTime;
matrix uWorldViewProjection;

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

// GLSL function ports.
// mod function.
float mod(float x, float y)
{
    return x - y * floor(x / y);
}

// fract function.
float fract(float x)
{
    return x - floor(x);
}

float rand(float x)
{
    return fract(sin(x) * 43758.5453);
}

float Triangle(float x)
{
    return abs(1.0 - mod(abs(x), 2.0)) * 2.0 - 1.0;
}

// Entirely based off of this shader on Shadertoy; https://www.shadertoy.com/view/MsSGD1
float2 CalculateDistortedTextureCoordinates(float2 baseCoords)
{
    float time = floor(globalTime * 4) / 0.1;
    
    float2 pixel = baseCoords;
    pixel += float2(Triangle(pixel.y * rand(time) * 4) * rand(time * 1.9) * 0.015,
                    Triangle(pixel.x * rand(time * 3.4) * 4) * rand(time * 2.1) * 0.015);
    pixel += float2(rand(pixel.x * 3.1 + pixel.y * 8.7) * 0.01, rand(pixel.x * 1.1 + pixel.y * 6.7) * 0.01);
    return pixel;
}

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
        
    // Distort the x and y coordinates of the vertex shader's position to achieve the 
    // messy, moving sketch effect on the primitive shape directly.
    float4 pos = mul(input.Position, uWorldViewProjection);
    pos.xy = CalculateDistortedTextureCoordinates(pos.xy);
    output.Position = pos;
    
    output.Color = input.Color;
    
    output.TextureCoordinates = input.TextureCoordinates;
    output.TextureCoordinates.y = (output.TextureCoordinates.y - 0.5) / input.TextureCoordinates.z + 0.5;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return input.Color;
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
