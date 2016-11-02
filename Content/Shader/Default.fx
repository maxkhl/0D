#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0

float Brightness;

float Width;
float Height;

static const float gaussAmount = 0.15;
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

float Pixels[13] =
{
	-6,
	-5,
	-4,
	-3,
	-2,
	-1,
	0,
	1,
	2,
	3,
	4,
	5,
	6,
};

float BlurWeights[13] =
{
	0.002216,
	0.008764,
	0.026995,
	0.064759,
	0.120985,
	0.176033,
	0.199471,
	0.176033,
	0.120985,
	0.064759,
	0.026995,
	0.008764,
	0.002216,
};


sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

float4 GaussianPixelShader(float2 TextureCoordinate)
{
	// Pixel width
	float pixelWidth = 1.0 / (float)Width;

	float4 color = { 0, 0, 0, 1 };

		float2 blur;
	blur.y = TextureCoordinate.y;

	for (int i = 0; i < 13; i++)
	{
		blur.x = TextureCoordinate.x + Pixels[i] * pixelWidth;
		color += tex2D(SpriteTextureSampler, blur.xy) * BlurWeights[i];
	}

	return color;
}

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
			color += gaussFilter[i] * tex2D(SpriteTextureSampler, texCoord).xyz;
			texCoord += Shift;
		}

	return (float4(color.x, color.y, color.z, tex2D(SpriteTextureSampler, texCoord).a));
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 pixel = tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;
	//pixel.rgb *= Brightness;
	return pixel; // GaussianPixelShader(input.TextureCoordinates);
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



technique Blur
{
	/*pass P0
	{
	PixelShader = compile PS_SHADERMODEL BlurHorizontal();
	}*/
	pass End
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};