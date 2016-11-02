#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0

float Brightness;
float Contrast;

float Width;
float Height;

float4 Color;

float gaussAmount = 0.55;
float gaussIntensity = 0.25;

static const int gaussPasses = 4;
static const int gaussRadius = 11;
static float gaussFilter[gaussRadius] =
{
	0.0402,
	0.0623,
	0.0877,
	0.1120,
	0.1297,
	0.1362,
	0.1297,
	0.1120,
	0.0877,
	0.0623,
	0.0402
};


sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 gaussBlur(float2 Shift, float2 TexCoords)
{
	float2 texCoord = TexCoords - float(int(gaussRadius / 2)) * Shift;
		float3 color = float3(0, 0, 0);

		for (int i = 0; i<gaussRadius; ++i) {
			color += gaussFilter[i] * tex2D(SpriteTextureSampler, texCoord).xyz / gaussPasses;
			texCoord += Shift;
		}

	float Alpha = tex2D(SpriteTextureSampler, texCoord).a * gaussIntensity;

	return (float4(color.x, color.y, color.z, 1) * Alpha);
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 pixel = tex2D(SpriteTextureSampler, input.TextureCoordinates) * Color;

	pixel.rgb = ((pixel.rgb - 0.5f) * max(Contrast, 0)) + 0.5f;

	pixel.rgb += Brightness;

	pixel.rgb *= pixel.a;

	return pixel;
}

float4 BlurHorizontal(VertexShaderOutput input) : COLOR
{
	return gaussBlur(float2((gaussAmount / Width), 0), input.TextureCoordinates);
}
float4 BlurVertical(VertexShaderOutput input) : COLOR
{
	return gaussBlur(float2(0, (gaussAmount / Height)), input.TextureCoordinates);
}
float4 BlurDiag1(VertexShaderOutput input) : COLOR
{
	return gaussBlur(float2((gaussAmount / Width), (gaussAmount / Height)), input.TextureCoordinates);
}
float4 BlurDiag2(VertexShaderOutput input) : COLOR
{
	return gaussBlur(float2(-(gaussAmount / Width), (gaussAmount / Height)), input.TextureCoordinates);
}



technique Default
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
technique Blur
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
	pass P1
	{
		PixelShader = compile PS_SHADERMODEL BlurHorizontal();
	}
	pass P2
	{
		PixelShader = compile PS_SHADERMODEL BlurVertical();
	}
	pass P3
	{
		PixelShader = compile PS_SHADERMODEL BlurDiag1();
	}
	pass P4
	{
		PixelShader = compile PS_SHADERMODEL BlurDiag2();
	}
}