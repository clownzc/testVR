Shader "VR/TreeLeave" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (0,0,0,0)
		_AOInten("AO Intensity", Range(0,1)) = 1.0
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		_Wavespeed("Wave Speed", Range(1,15)) = 1
		_WaveVector("Wave Vector(X:MainBlend X Y:MainBlend Y Z:Up Down  W:Global)", Vector) = (0,0,0,0)
		_MblendScale("WaveWM", Range(-30,30)) = 0.01
	}
	SubShader {
		Tags{
		"Queue" = "AlphaTest"
		"RenderType" = "TransparentCutout"
		}
		LOD 150

		Cull Off
	Pass {
		Tags { "LightMode" = "ForwardBase" }

		CGPROGRAM
		#pragma vertex vert_surf
		#pragma fragment frag_surf
		#pragma multi_compile_instancing
		#pragma multi_compile_fog
		#pragma multi_compile_fwdbase
		#include "UnityShaderVariables.cginc"
		// -------- variant for: <when no other keywords are defined>
		#if !defined(INSTANCING_ON)
		#define UNITY_PASS_FORWARDBASE
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "AutoLight.cginc"

		#define SIDE_TO_SIDE_FREQ1 1.975
		#define SIDE_TO_SIDE_FREQ2 0.793
		#define UP_AND_DOWN_FREQ1 0.375
		#define UP_AND_DOWN_FREQ2 0.193

		sampler2D _MainTex;		half4 _MainTex_ST;
		sampler2D _Emissive;	half4 _Emissive_ST;
		fixed _AOInten;
		fixed4 _Color;
		fixed _Cutoff;
		fixed _Wavesize;
		fixed _Wavespeed;
		fixed4 _WaveVector;
		fixed _MblendScale;
		
		// This bends the entire plant in the direction of the wind.
		// vPos:		The world position of the plant *relative* to the base of the plant.
		//			(That means we assume the base is at (0, 0, 0). Ensure this before calling this function).
		// vWind:		The current direction and strength of the wind.
		// fBendScale:	How much this plant is affected by the wind.
		half3 ApplyMainBending(half3 vPos, half2 vWind, half fBendScale)
		{
			// Calculate the length from the ground, since we'll need it.
			half fLength = length(vPos);
			// Bend factor - Wind variation is done on the CPU.
			half fBF = vPos.y * fBendScale;
			// Smooth bending factor and increase its nearby height limit.
			fBF += 1.0;
			fBF *= fBF;
			fBF = fBF * fBF - fBF;
			// Displace position
			half3 vNewPos = vPos;
			vNewPos.xz += vWind.xy * fBF;
			// Rescale - this keeps the plant parts from "stretching" by shortening the y (height) while
			// they move about the xz.
			vPos.xyz = normalize(vNewPos.xyz)* fLength;
			return vPos;
		}

		half4 SmoothCurve(half4 x) {
			return x * x *(3.0 - 2.0 * x);
		}

		half4 TriangleWave(half4 x) {
			return abs(frac(x + 0.5) * 2.0 - 1.0);
		}

		half4 SmoothTriangleWave(half4 x) {
			return SmoothCurve(TriangleWave(x));
		}


		// This provides "chaotic" motion for leaves and branches (the entire plant, really)
		void ApplyDetailBending(
			inout half3 vPos,		// The final world position of the vertex being modified
			half3 vNormal,			// The world normal for this vertex
			half3 objectPosition,	        // The world position of the plant instance (same for all vertices)
			half fDetailPhase,		// Optional phase for side-to-side. This is used to vary the phase for side-to-side motion
			half fBranchPhase,		// The green vertex channel per Crytek's convention
			half fTime,			// Ever-increasing time value (e.g. seconds ellapsed)
			half fEdgeAtten,		// "Leaf stiffness", red vertex channel per Crytek's convention
			half fBranchAtten,		// "Overall stiffness", *inverse* of blue channel per Crytek's convention
			half fBranchAmp,		// Controls how much up and down
			half fSpeed,			// Controls how quickly the leaf oscillates
			half fDetailFreq,		// Same thing as fSpeed (they could really be combined, but I suspect
									// this could be used to let you additionally control the speed per vertex).
			half fDetailAmp)		// Controls how much back and forth

		{
			// Phases (object, vertex, branch)
			// fObjPhase: This ensures phase is different for different plant instances, but it should be
			// the same value for all vertices of the same plant.
			half fObjPhase = dot(objectPosition.xyz, 1);

			// In this sample fBranchPhase is always zero, but if you want you could somehow supply a
			// different phase for each branch.
			fBranchPhase += fObjPhase;

			// Detail phase is (in this sample) controlled by the GREEN vertex color. In your modelling program,
			// assign the same "random" phase color to each vertex in a single leaf/branch so that the whole leaf/branch
			// moves together.
			half fVtxPhase = dot(vPos.xyz, fDetailPhase + fBranchPhase);

			half2 vWavesIn = fTime + half2(fVtxPhase, fBranchPhase);
			half4 vWaves = (frac(vWavesIn.xxyy *
				half4(SIDE_TO_SIDE_FREQ1, SIDE_TO_SIDE_FREQ2, UP_AND_DOWN_FREQ1, UP_AND_DOWN_FREQ2)) *
				2.0 - 1.0) * fSpeed * fDetailFreq;
			vWaves = SmoothTriangleWave(vWaves);
			half2 vWavesSum = vWaves.xz + vWaves.yw;

			// -fBranchAtten is how restricted this vertex of the leaf/branch is. e.g. close to the stem
			//  it should be 0 (maximum stiffness). At the far outer edge it might be 1.
			//  In this sample, this is controlled by the blue vertex color.
			// -fEdgeAtten controls movement in the plane of the leaf/branch. It is controlled by the
			//  red vertex color in this sample. It is supposed to represent "leaf stiffness". Generally, it
			//  should be 0 in the middle of the leaf (maximum stiffness), and 1 on the outer edges.
			// -Note that this is different from the Crytek code, in that we use vPos.xzy instead of vPos.xyz,
			//  because I treat y as the up-and-down direction.

			vPos.xyz += vWavesSum.x * half3(fEdgeAtten * fDetailAmp * vNormal.xyz);
			vPos.y += vWavesSum.y * fBranchAtten * fBranchAmp;
		}

		half3 MainBlend(half3 vPos, half fBendScale, half2 vWind, half fLength)
		{
			// Bend factor - Wind variation is done on the CPU.
			half fBF = vPos.z * fBendScale;
			// Smooth bending factor and increase its nearby height limit.
			fBF += 1.0;
			fBF *= fBF;
			fBF = fBF * fBF - fBF;
			// Displace position
			half3 vNewPos = vPos;
			vNewPos.xy += vWind.xy * fBF;
			// Rescale
			vPos.xyz = normalize(vNewPos.xyz)* fLength;
			return vPos;
		}

		half3 SimpleWave(half3 WorldPos, half3 ObjPos, half WaveSize, half WaveSpeed, half VertexColor) {
			half2 a = ((WorldPos - ObjPos) / WaveSize).rb;
			half b = (a.r * a.g + WaveSpeed * (1 - _Time.x * _Wavespeed) + VertexColor) * 6.28318548;
			b = sin(b);
			return b;
		}

	// vertex-to-fragment interpolation data
		struct v2f_surf {
			float4 pos : SV_POSITION;
			float2 uv1 : TEXCOORD0;
			fixed4 vertexColor : COLOR;
			#ifndef LIGHTMAP_ON
			half3 sh : TEXCOORD1;
			#else
			fixed4 lmap : TEXCOORD1;
			#endif
			half3 normalDir : TEXCOORD2;
			UNITY_FOG_COORDS(3)
			LIGHTING_COORDS(4, 5)
			half4 fogValue : TEXCOORD6;
			UNITY_VERTEX_INPUT_INSTANCE_ID
			UNITY_VERTEX_OUTPUT_STEREO
		};
		// vertex shader
		v2f_surf vert_surf (appdata_full v) {
			UNITY_SETUP_INSTANCE_ID(v);
			v2f_surf o;
			UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
			UNITY_TRANSFER_INSTANCE_ID(v,o);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
			
			fixed3 lightColor = _LightColor0.rgb;
			o.uv1.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
			half4 worldPos = mul(unity_ObjectToWorld, v.vertex);
			o.normalDir = UnityObjectToWorldNormal(v.normal);
			o.vertexColor = v.color;
			
			half3 WorldNormal = normalize(o.normalDir);

			//Vertex Animation
			//v.vertex += mul(unity_WorldToObject, SimpleWave(worldPos, v.vertex.xyz, _Wavesize, _Wavespeed, v.color.g)) * _WaveVector * (1 - v.color.b);

			//v.vertex.rgb += SimpleWave(worldPos, v.vertex.xyz, _Wavesize, _Wavespeed, v.color.g) * _WaveVector * (1 - v.color.b);

			// Grab the object position from the translation part of the world matrix.
			// We need the object position because we want to temporarily translate the vertex
			// back to the position it would be if the plant were at (0, 0, 0).
			// This is necessary for the main bending to work.
			_WaveVector.xy = sin(_WaveVector.xy * _Time.x * _Wavespeed);

			worldPos.xyz -= v.vertex.rgb;

			half3 vpos = ApplyMainBending(worldPos, _WaveVector.xy, _MblendScale);
			vpos += v.vertex.rgb;

			fixed windStrength = length(_WaveVector.xy);

			ApplyDetailBending(vpos, o.normalDir, v.vertex.xyz, 0, v.color.g, _Time.x * _Wavespeed, v.color.r, 1 - v.color.b, windStrength * _WaveVector.z, 2, 1, _WaveVector.w * windStrength);

			v.vertex.xyzw = mul(unity_WorldToObject, half4(vpos, worldPos.w));

			o.pos = UnityObjectToClipPos(v.vertex.xyzw);

			#if defined(LIGHTMAP_ON)
				o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
			#elif defined(LIGHTMAP_OFF)
				#ifdef VERTEXLIGHT_ON
					o.sh = Shade4PointLights(
						unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
						unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
						unity_4LightAtten0, worldPos, o.normalDir);
			#endif
			o.sh = ShadeSHPerVertex (o.normalDir, o.sh);
			#endif

			UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
			return o;
		}

		// fragment shader Input IN
		fixed4 frag_surf (v2f_surf IN) : SV_Target {
			UNITY_SETUP_INSTANCE_ID(IN);

			half3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
			half3 normalWorldVertex = normalize(IN.normalDir);
			fixed3 lightColor = _LightColor0.rgb;

			fixed attenuation = LIGHT_ATTENUATION(IN);
			fixed3 attenColor = attenuation * lightColor;

			fixed4 c = 0;
			fixed4 basetex = tex2D(_MainTex, TRANSFORM_TEX(IN.uv1.xy, _MainTex)) * _Color;

			#if defined(LIGHTMAP_ON)
				fixed3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap)).rgb;
				c.rgb += lm * basetex * lerp(IN.vertexColor.a,1, _AOInten);
			
			#elif defined(LIGHTMAP_OFF)
				fixed NdotL = saturate(dot(normalWorldVertex, lightDirection));
				fixed3 directDiffuse = NdotL * attenColor;
				c.rgb += (directDiffuse + IN.sh) * basetex * lerp(IN.vertexColor.a, 1, _AOInten);
			#endif

			clip(basetex.a - _Cutoff);
			UNITY_APPLY_FOG(IN.fogCoord, c);
			UNITY_OPAQUE_ALPHA(c.a);
			return c;
		}
		#endif

		ENDCG
	}
}

Fallback "Mobile/VertexLit"
}
