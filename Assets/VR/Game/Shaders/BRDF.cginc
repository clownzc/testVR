//---------------------------------UE4 Formula-------------------------------------

half PhongApprox( half Roughness, half RoL )
{
    half a = Roughness * Roughness;            // 1 mul
    half a2 = a * a;                        // 1 mul
    float rcp_a2 = 1/a2;                    // 1 rcp
    //half rcp_a2 = exp2( -6.88886882 * Roughness + 6.88886882 );

    // Spherical Gaussian approximation: pow( x, n ) ~= exp( (n + 0.775) * (x - 1) )
    // Phong: n = 0.5 / a2 - 0.5
    // 0.5 / ln(2), 0.275 / ln(2)
    half c = 0.72134752 * rcp_a2 + 0.39674113;    // 1 mad
    return rcp_a2 * exp2( c * RoL - c );        // 2 mad, 1 exp2, 1 mul
    // Total 7 instr
}

half3 EnvBRDFApprox(half3 SpecularColor, half Roughness, half NoV)
{
	// [ Lazarov 2013, "Getting More Physical in Call of Duty: Black Ops II" ]
	// Adaptation to fit our G term.
	const half4 c0 = { -1, -0.0275, -0.572, 0.022 };
	const half4 c1 = { 1, 0.0425, 1.04, -0.04 };
	half4 r = Roughness * c0 + c1;
	half a004 = min(r.x * r.x, exp2(-9.28 * NoV)) * r.x + r.y;
	half2 AB = half2(-1.04, 1.04) * a004 + r.zw;
	return SpecularColor * AB.x + AB.y;
}