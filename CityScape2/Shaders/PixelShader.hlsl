Texture2D txt : register(t0);
SamplerState smpl : register(s0);

struct PS_IN
{
    float4 pos : SV_POSITION;
    float3 norm : NORMAL;
    float2 tex: TEXCOORD0;
    float fog : TEXCOORD1;
    float3 mod : TEXCOORD2;
};

float4 main(PS_IN input) : SV_Target
{
    float3 lightDirection = normalize(float3(1, -1, 0));
    float4 texel = txt.Sample(smpl, input.tex);
    float lightMag = 0.5 + 0.5f * saturate(dot(input.norm, -lightDirection));

    float4 color = (texel * lightMag) * float4(input.mod, 0.0f);
    float4 fogColor = float4(0.0f, 0.0f, 0.1f, 0.0f);


//        return float4(input.fog, input.fog, input.fog, 1.0);
    return lerp(fogColor, color, input.fog);
    
};