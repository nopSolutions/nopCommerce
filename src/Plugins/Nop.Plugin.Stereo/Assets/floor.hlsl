#include <stereokit.hlsli>

//--name = app/floor

//--color:color = 0,0,0,1
float4 color;
//--radius      = 5,10,0,0
float4 radius;

struct vsIn {
	float4 pos : SV_POSITION;
};
struct psIn {
	float4 pos   : SV_POSITION;
	float4 world : TEXCOORD0;
	uint view_id : SV_RenderTargetArrayIndex;
};

psIn vs(vsIn input, uint id : SV_InstanceID) {
	psIn o;
	o.view_id = id % sk_view_count;
	id        = id / sk_view_count;

	o.world = mul(input.pos, sk_inst[id].world);
	o.pos   = mul(o.world,   sk_viewproj[o.view_id]);

	return o;
}
float4 ps(psIn input) : SV_TARGET{
	// This line algorithm is inspired by :
	// http://madebyevan.com/shaders/grid/

	// Minor grid lines
	float2 step = 1 / fwidth(input.world.xz);
	float2 grid = abs(frac(input.world.xz - 0.5) - 0.5) * step; // minor grid lines
	float  dist = sqrt(dot(input.world.xz, input.world.xz)); // transparency falloff
	float  size = min(grid.x, grid.y);
	float  val = 1.0 - min(size, 1.0);
	val *= saturate(1 - ((dist - radius.x)/radius.y));

	// Major axis lines
	const float extraThickness = 0.5;
	float2 axes = (abs(input.world.xz)) * step - extraThickness;
	size = min(axes.x, axes.y);
	float  axisVal = 1.0 - min(size, 1.0);
	axisVal *= saturate(1 - ((dist - radius.x*1.5)/(radius.y*1.5)));

	// combine, and drop transparent pixels
	val = max(val*0.6, axisVal);
	if (val <= 0) discard;

	return float4(color.rgb, val);
}