struct VS_IN
{
    float4 pos : POSITION;
    float4 col : COLOR;
};

struct PS_IN
{
    float4 pos : SV_POSITION;
    float4 col : COLOR;
};

float4x4 WorldViewProj;

PS_IN main(VS_IN input)
{
    PS_IN output = (PS_IN) 0;

    output.pos = mul(input.pos, WorldViewProj);
    output.col = input.col;

    return output;
}