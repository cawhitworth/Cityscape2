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
};

struct PS_IN
{
    float4 pos : SV_POSITION;
    float3 norm : NORMAL;
    float2 tex: TEXCOORD0;
};

PS_IN main(VS_IN input)
{
    PS_IN output;

    float4 pos = float4(input.pos, 1.0f);
    pos = mul(pos, model);
    pos = mul(pos, view);
    pos = mul(pos, projection);
    output.pos = pos;

    output.tex = input.tex;

    float4 norm = float4(normalize(input.norm), 0.0f);
    norm = mul(norm, model);

    output.norm = normalize(norm.xyz);

    return output;
}