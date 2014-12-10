Texture2D txt : register(t0);
SamplerState smpl : register(s0);

struct PS_IN
{
    float4 pos : SV_POSITION;
    float3 norm : NORMAL;
    float2 tex: TEXCOORD0;
};

float4 main(PS_IN input) : SV_Target
{
    float3 lightDirection = normalize(float3(1, -1, 0));
    float4 texel = txt.Sample(smpl, input.tex);
    float lightMag = 0.2 + 0.8f * saturate(dot(input.norm, -lightDirection));

    return texel * lightMag;
    
};