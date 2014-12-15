cbuffer constantBuffer : register(b0)
{
    matrix model;
    matrix view;
    matrix projection;
};

struct VS_IN
{
    float3 pos : POSITION;
    float3 norm : NORMAL;
    float2 tex: TEXCOORD0;
    float3 mod: TEXCOORD1;
};

struct PS_IN
{
    float4 pos : SV_POSITION;
    float3 norm : NORMAL;
    float2 tex: TEXCOORD0;
    float fog : TEXCOORD1;
    float3 mod : TEXCOORD2;
};

PS_IN main(VS_IN input)
{
    PS_IN output;

    // Transform position by model-view-projection
    float4 pos = float4(input.pos, 1.0f);
    float4 modelPos = mul(pos, model);
    float4 viewPos = mul(modelPos, view);
    float4 projPos = mul(viewPos, projection);
    output.pos = projPos;

    // Passthrough texture coords
    output.tex = input.tex;

    // Transform normal by model
    float4 norm = float4(normalize(input.norm), 0.0f);
    norm = mul(norm, model);

    // and normalise
    output.norm = normalize(norm.xyz);

    // Fogging
    float distance = length(viewPos);
    output.fog = saturate(1 / (0.1f * distance));

    // Mod
    output.mod = input.mod;

    return output;
}