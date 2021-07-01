// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "VR/procedural_skybox" {
    Properties {
		_Skymap("SkyMap", 2D) = "white" {}
		_MountainMap("MountainMap", 2D) = "grey" {}
		_CloudsNormal("CloudNormal", 2D) = "white" {}
		_CloudsLutMap("CloudLutMap", 2D) = "white" {}
		_CloudCover("CloudCover", Range(0,1)) = 0.5
		
		_SkyboxTintColor("SkyboxTint", Color) = (1,1,1,1)
		_Fogcolor("Fogcolor", Color) = (1,1,1,1)
		_CloudTint("CloudTint", Color) = (1,1,1,1)
		_CloudSoftness("CloudSoftness", Range(0,0.5)) = 0.5
		_CloudHemisphereRadius("CloudHemisphereRadius", Range(1,1024)) = 64
		_CloudSize("CloudSize", Range(1,20)) = 2
		_CloudDirection("CloudDirection", Range(0,180)) = 0 //Cloud Direction
		_CloudVelocity("CloudVelocity",Range(0,50)) = 1
		_NormalIntensity("NormalIntensity", Range(0,1)) = 1
		_FogTransparencyScale("FogTransparencyScale",Range(0,1)) = 1
		_FogTransparencyPower("FogTransparencyPower",Range(0,16)) = 4

		_Horizon_Offset("Horizon Offset", Range(0,1000)) = 0
		_Vertical_Offset("Verteical Offset", Range(0,1000)) = 0
		_Cloud_Tiling("Cloud Tiling", Range(0.1,20)) = 1
		_CloudFarTiling("Cloud FarTiling", Range(0.001,1)) = 0.125

		_DetailScale("Detail Scale", Range(0,1)) = 0.5

		_SunSize("Sun Size", Range(0,1)) = 0.04
		_SunColor("Sun Color", Color) = (1,1,1,1)
		_SunLight("Sun Light", Range(0,10)) = 0.8

		//Mountains
		_MountainsUScale("MountainsUScale",Range(1,16)) = 4
		_MountainsVScale("MountainsUScale",Range(1,16)) = 8
		_MountainsUOffset("MountainsUOffset",Range(0,1)) = 0
		_MountainsVOffset("MountainsUOffset",Range(0,1)) = 0.5
		_MountainsFogIntensity("MountainsFogIntensity", Range(0,1)) = 0.5
    }
    SubShader {
        Tags { "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
		Cull Off ZWrite Off
        LOD 200
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform float4 _LightColor0;
			half _SunSize;
			half3 _SunColor;
			half _SunLight;
			sampler2D _CloudsNormal;
			sampler2D _CloudsLutMap;
			sampler2D _Skymap;
			sampler2D _MountainMap;
			half _CloudHemisphereRadius;
			half _CloudSize;
			half _CloudDirection;
			half _CloudVelocity;
			half _CloudCover;
			half _CloudSoftness;

			//Mountains
			half _MountainsUScale;
			half _MountainsVScale;
			half _MountainsUOffset;
			half _MountainsVOffset;
			half _MountainsFogIntensity;

			half _Horizon_Offset;
			half _Vertical_Offset;

			half _DetailScale;
			half _NormalIntensity;
			half _FogTransparencyPower;
			half _FogTransparencyScale;

			half4 _SkyboxTintColor;
			half4 _Fogcolor;
			half4 _CloudTint;

			half _Cloud_Tiling;
			half _CloudFarTiling;

            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
				float2 texcoord0 : TEXCOORD1;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
				float2 uv0 : TEXCOORD1;
				float4 uv1 : TEXCOORD2;//Cloud UV
				float4 uv2 : TEXCOORD3;
				float2 uv3 : TEXCOORD4;//Mountain uv
				float3 worlduv0 : TEXCOORD5;//Lut
                float3 vertexpos : TEXCOORD6;
				float3 normalDir : TEXCOORD7;
                LIGHTING_COORDS(8,9)
//              UNITY_FOG_COORDS(6)
				float2 testuv : TEXCOORD11;

            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex);

				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				float3 VertexObjectPos = normalize(v.vertex.xyz);//ObjectPos

				//Unity Shader Skybox CloudUV
				//(_CloudFarTiling + o.posWorld.y *(1 - _CloudFarTiling))
				float2 local_001 = float2(_Horizon_Offset, _Vertical_Offset) + o.posWorld.xz/(_CloudFarTiling + abs(o.posWorld.y) * ( 1 - _CloudFarTiling));

				//Vertexpos.xz same as local_151
				float2 local_151 = -VertexObjectPos.xz;
				//vertexpos.y * vertex.xz * other the same as local_002.xz * local_003 * ( 1 - _CloudFarTiling)
				float2 local_152 = (_CloudHemisphereRadius - 1) * abs(VertexObjectPos.y) * -VertexObjectPos.xz;

				float local_155 = 1 - 2 * _CloudHemisphereRadius;
				//float local_155a = (_CloudHemisphereRadius - 1) * (_CloudHemisphereRadius - 1) - _CloudHemisphereRadius * _CloudHemisphereRadius;

				//Vertexpos.xz * Vertexpos.xz * 
				float2 local_157 = float2(local_155, local_155) * VertexObjectPos.xz * VertexObjectPos.xz;//float2(e, e) = local_156  float2 e2 = local_157

				//Returns - 1 if x is less than zero; 0 if x equals zero; and 1 if x is greater than zero.
				float2 local_159 = sign(local_151);

				float2 local_160 = local_152 * local_152;
				//float2 local_160a = ((1 - _CloudHemisphereRadius) * abs(local_140.y) * -local_10.xz) * ((1 - _CloudHemisphereRadius) * abs(local_140.y) * -local_10.xz);
				float2 local_161 = local_160 - local_157;
				//float2 local_161a = ((1 - _CloudHemisphereRadius) * abs(local_140.y) * -local_10.xz) * ((1 - _CloudHemisphereRadius) * abs(local_140.y) * -local_10.xz) - float2((1 - 2 * _CloudHemisphereRadius),1 - 2 * _CloudHemisphereRadius) * -local_10.xz * -local_10.xz;

				float2 local_162 = sqrt(local_161);//when _CloudHemisphereRadius = 1 local_162 = vertexPos.xz
				float2 local_163 = local_159 * local_162;//Cloud uv1
				//UV Important
				//float2 local_164a = (1 - _CloudHemisphereRadius) * abs(local_140.y) * -local_10.xz - sign(local_151) * ((1 - _CloudHemisphereRadius) * abs(local_140.y) * -local_10.xz) * ((1 - _CloudHemisphereRadius) * abs(local_140.y) * -local_10.xz)
				
				float2 local_164 = local_152 - local_163;//when _CloudHemisphereRadius = 1 local_164 = -vertexPos.xz

				float local_166 = 0.1 /_CloudSize;

				//Cloud UV speed
				float local_168 = _CloudDirection * 0.0174f;
				float2 local_175 = _CloudVelocity * 0.125f * _Time.r * float2(cos(local_168), sin(local_168));
				
				//Cloud UV1 add Animation
				float2 uv_01 = local_166 * (local_164 + local_175);//Cloud uv1

				//Cloud UV2
				float2 uv_02 = local_166 * 4.0f * (local_164 + 1.5f * local_175);//Cloud uv2

				//Cloud Cover Softness Feature
				float local_184 = 1.0f - _CloudCover;
				float2 local_185 = saturate(float2(local_184 - _CloudSoftness, local_184 + _CloudSoftness));

				//Sun Feature
				float local_189 = dot(-lightDirection, -VertexObjectPos);
				float local_190 = saturate(local_189);
				float local_191 = local_190 * local_190;
				float local_192 = local_191 * local_191;

				float local_195 = _MountainsUScale * v.texcoord0.x;
				float local_196 = local_195 + _MountainsUOffset;

				float local_198 = v.texcoord0.y - 0.5f;
				float local_199 = _MountainsVScale * local_198;
				float local_200 = local_199 + _MountainsVOffset;

				o.worlduv0 = -VertexObjectPos;//v_texture5
				o.uv0 = v.texcoord0;
				o.uv1 = float4(uv_01, uv_02);//Cloud Normal UV!!!
				o.uv2 = float4(local_185, local_191, local_192);//local_77;
				o.uv3 = float2(local_196, local_200);
				//o.vertexpos = normalize(v.vertex.xyz);
				o.testuv = local_001;
//              UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }

			// Calculates the Mie phase function
			#define MIE_G (-0.990)
			#define MIE_G2 0.9801
			half getMiePhase(half eyeCos, half eyeCos2)
			{
				half temp = 1.0 + MIE_G2 - 2.0 * MIE_G * eyeCos;
				temp = pow(temp, pow(_SunSize, 0.65) * 10);
				temp = max(temp, 1.0e-4); // prevent division by zero, esp. in half precision
				temp = 1.5 * ((1.0 - MIE_G2) / (2.0 + MIE_G2)) * (1.0 + eyeCos2) / temp;
#if defined(UNITY_COLORSPACE_GAMMA)
				temp = pow(temp, .454545);
#endif
				return temp;
			}

			#define kSUN_BRIGHTNESS 20.0 	// Sun brightness
			static const half kSunScale = 400.0 * kSUN_BRIGHTNESS;
			half calcSunSpot(half3 vec1, half3 vec2)
			{
				half3 delta = vec1 - vec2;
				half dist = length(delta);
				half spot = 1.0 - smoothstep(0.0, _SunSize, dist);
				return kSunScale * spot * spot;
			}

            float4 frag(VertexOutput i) : SV_Target {
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				//World Vecter
				float local_12 = sign(i.worlduv0.y);

				//Cloud Noise Normal_01
				float4 Normal_01 = tex2D(_CloudsNormal, i.uv1.xy);//Important!!!!!!problem
				float4 local_32 = tex2D(_CloudsNormal, i.testuv);//Important!!!!!!problem
				float4 Normal_02 = tex2D(_CloudsNormal, i.uv1.zw);//Important!!!!!!

				//-------------------------------Normal------------------------------
				float2 local_45 = Normal_01.xy * 2.0 - 1;//2 * local_32
				float local_47 = local_12 * Normal_01.z;//normal.z * sign
				float3 F_Normal_01 = float3(local_45.x, local_47, local_45.y);

				float2 local_56 = Normal_02.xy * 2 - 1;
				float local_58 = local_12 * Normal_02.z;
				float3 F_Normal_02 = float3(local_56.x, local_58, local_56.y);
				//--------------------------------------------------------------------

				//------------------------------Normal Combine-----------------------------
				//add normaluv1 normaluv2
				float3 local_63 = F_Normal_01 + F_Normal_02 * _DetailScale;

				//lerp Y- direction Normal1 and Normal2 _NormalIntensity = (0,1)
				float3 NormalFinal = normalize(lerp(float3(0, local_12, 0), local_63, _NormalIntensity));
				//-------------------------------------------------------------------------

				//------------------------------Mask Combine-------------------------------
				//normaluv2.w * mask + mask
				float local_73 = saturate(Normal_01.w + Normal_02.w * _DetailScale);
				//Cloud Mask i.uv2.x = MinValue i.uv2.y = MaxValue
				float local_78 = smoothstep(i.uv2.x, i.uv2.y, local_73);
				//-------------------------------------------------------------------------

				//Directional LightDir
				//NdotL
				float NdotL = dot(-lightDirection, NormalFinal);
				// (local_85 + 1) * 0.5 remap to 0 - 1 like saturate
				float local_86 = (NdotL + 1) * 0.5;

				//-----------------------------Lut Texture---------------------------------
				//SunLight Dot VertexPos
				float Lightflare = i.uv2.z;
				//SunLight Dir Dot VertexPos 2 Power  light ring
				float LightflarePow = i.uv2.w;

				//reverse Mask
				float local_82 = 1.0- local_78 * local_78;
				//Cloud Mask * DotMask
				float local_83 = local_82 * Lightflare;

				//CloudLut U
				float local_24 = atan2(i.worlduv0.z, i.worlduv0.x) * 0.159f + 0.5;
				float local_13 = 1.0 - local_24;
				//CloudLut V
				float local_87 = lerp(local_86, 1, local_83);

				//CloudLut UV
				float2 CloudLut_UV = float2(local_13, local_87);
				//LutMap
				float4 Cloud_Lut = tex2D(_CloudsLutMap, CloudLut_UV);
				//-----------------------------------------------------------------------

				float3 local_90 = Cloud_Lut.xyz * _CloudTint;
				float2 local_94 = float2(local_13, i.uv0.y);

				//SkyMap
				float4 SkyMap = tex2D(_Skymap, local_94);
				//LightIntensity

				//Sun
				float3 ray = normalize(i.posWorld.xyz);
				half eyeCos = dot(_WorldSpaceLightPos0.xyz, -ray);
				half eyeCos2 = eyeCos * eyeCos;
				half mie = getMiePhase(eyeCos, eyeCos2);
				//half mie = calcSunSpot(_WorldSpaceLightPos0.xyz, ray);
				float3 sun = _SunColor * mie;

				//add skymap and LightParam  
				float3 local_100 = SkyMap.xyz + _SunColor * _SunLight * LightflarePow + sun;

				//lerp skymap and Cloudlutmap 
				float3 local_102 = lerp(local_100, local_90, local_78);//local_100 is Skymap and sun,local_90 is CloudMap local_78 is CloudMask

				//float3(0,1,0) Ydot
				float3 local_103 = float3(0, 1, 0);
				float local_104 = dot(local_103, i.worlduv0.xyz);
				//-------------------

				//---------------------------------Fog------------------------------------
				float local_105 = abs(local_104);
				float local_106 = 1.0f - local_105;
				//Power Fog Range
				float local_107 = pow(local_106, _FogTransparencyPower);
				//Modify Fog Transparent
				float local_108 = lerp(1, local_107, _FogTransparencyScale);
				//------------------------------------------------------------------------

				//lerp (lerp skymap , CloudMap and CloudMask ),fogcolor,FogMask)
				float3 local_115 = lerp(local_102, _Fogcolor, local_108);
				float4 local_116 = float4(local_115.x, local_115.y, local_115.z, local_108);

				//MountainMap
				float4 MountainMap = tex2D(_MountainMap, i.uv3);
				//-------------------
				float local_149 = MountainMap.w;

				float local_152 = _MountainsFogIntensity * local_108;
				float local_153 = 1.0f - local_152;
				float local_154 = local_149 * local_153;

				float3 local_155 = float3(local_154, local_154, local_154);
				//local_155 is 0.5 - 1 so local_156 is not true color
				float3 local_156 = lerp(local_116.xyz, MountainMap.xyz, local_155);//local_150 is Skymap  local_148 is MountainMap local_155 is problem
				//-------------------
				//Skybox Tint
				float3 local_159 = local_156 * _SkyboxTintColor.xyz;

				//Final Color
				float4 final_color = float4(local_159.xyz, 1);;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/// Final Color:
//              float3 finalColor = fixed3(0,0,0);
//				fixed4 finalRGBA = saturate(fixed4(local_32.xyz, 1));
//				fixed4 finalRGBA = saturate(fixed4(i.testuv.xy,0, 1));
//				fixed4 finalRGBA = fixed4(i.uv3, 0, 1);

//				fixed4 finalRGBA = fixed4(Normal_02.xyz, 1);
				fixed4 finalRGBA = fixed4(final_color.xyz, 1);
//              UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
