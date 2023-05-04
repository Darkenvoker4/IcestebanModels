#ifndef RALIV_PENETRATION
    #define RALIV_PENETRATION
    float _PenetratorEnabled;
    float _squeeze;
    float _SqueezeDist;
    float _BulgeOffset;
    float _BulgePower;
    float _Length;
    float _EntranceStiffness;
    float _Curvature;
    float _ReCurvature;
    float _WriggleSpeed;
    float _Wriggle;
    float _OrificeChannel;
    float __dirty;
    float _OrifaceEnabled;
    sampler2D _OrificeData;
    float _EntryOpenDuration;
    float _Shape1Depth;
    float _Shape1Duration;
    float _Shape2Depth;
    float _Shape2Duration;
    float _Shape3Depth;
    float _Shape3Duration;
    float _BlendshapePower;
    float _BlendshapeBadScaleFix;
    void GetBestLights(float Channel, inout int orificeType, inout float3 orificePositionTracker, inout float3 orificeNormalTracker, inout float3 penetratorPositionTracker, inout float penetratorLength)
    {
        float ID = step(0.5, Channel);
        float baseID = (ID * 0.02);
        float holeID = (baseID + 0.01);
        float ringID = (baseID + 0.02);
        float normalID = (0.05 + (ID * 0.01));
        float penetratorID = (0.09 + (ID * - 0.01));
        float4 orificeWorld;
        float4 orificeNormalWorld;
        float4 penetratorWorld;
        float penetratorDist = 100;
        for (int i = 0; i < 4; i ++)
        {
            float range = (0.005 * sqrt(1000000 - unity_4LightAtten0[i])) / sqrt(unity_4LightAtten0[i]);
            if (length(unity_LightColor[i].rgb) < 0.01)
            {
                if(abs(fmod(range, 0.1) - holeID) < 0.005)
                {
                    orificeType = 0;
                    orificeWorld = float4(unity_4LightPosX0[i], unity_4LightPosY0[i], unity_4LightPosZ0[i], 1);
                    orificePositionTracker = mul(unity_WorldToObject, orificeWorld).xyz;
                }
                if(abs(fmod(range, 0.1) - ringID) < 0.005)
                {
                    orificeType = 1;
                    orificeWorld = float4(unity_4LightPosX0[i], unity_4LightPosY0[i], unity_4LightPosZ0[i], 1);
                    orificePositionTracker = mul(unity_WorldToObject, orificeWorld).xyz;
                }
                if(abs(fmod(range, 0.1) - normalID) < 0.005)
                {
                    orificeNormalWorld = float4(unity_4LightPosX0[i], unity_4LightPosY0[i], unity_4LightPosZ0[i], 1);
                    orificeNormalTracker = mul(unity_WorldToObject, orificeNormalWorld).xyz;
                }
                if(abs(fmod(range, 0.1) - penetratorID) < 0.005)
                {
                    float3 tempPenetratorPositionTracker = penetratorPositionTracker;
                    penetratorWorld = float4(unity_4LightPosX0[i], unity_4LightPosY0[i], unity_4LightPosZ0[i], 1);
                    penetratorPositionTracker = mul(unity_WorldToObject, penetratorWorld).xyz;
                    if(length(penetratorPositionTracker) > length(tempPenetratorPositionTracker))
                    {
                        penetratorPositionTracker = tempPenetratorPositionTracker;
                    }
                    else
                    {
                        penetratorLength = unity_LightColor[i].a;
                    }
                }
            }
        }
    }
    #ifdef POI_SHADOW
        void applyRalivDynamicPenetrationSystem(inout float3 VertexPosition, inout float3 VertexNormal, inout VertexInputShadow v)
    #else
        void applyRalivDynamicPenetrationSystem(inout float3 VertexPosition, inout float3 VertexNormal, inout appdata v)
    #endif
    {
        
        if((1.0 /*_PenetratorEnabled*/))
        {
			float orificeChannel=0;
			float orificeType = 0;
			float3 orificePositionTracker = float3(0,0,100);
			float3 orificeNormalTracker = float3(0,0,99);
			float3 penetratorPositionTracker = float3(0,0,1);
			float3 penetratorNormalTracker = float3(0,0,1);
			float pl=0;
			GetBestLights(orificeChannel, orificeType, orificePositionTracker, orificeNormalTracker, penetratorNormalTracker, pl);
			float3 orificeNormal = normalize( lerp( ( orificePositionTracker - orificeNormalTracker ) , orificePositionTracker , max( (0.01 /*_EntranceStiffness*/) , 0.01 )) );
			float behind = smoothstep(-(0.1624694 /*_Length*/)*0.5, (0.1624694 /*_Length*/)*0.2, orificePositionTracker.z);
			orificePositionTracker.z=(abs(orificePositionTracker.z+((0.1624694 /*_Length*/)*0.2))-((0.1624694 /*_Length*/)*0.2))*(1+step(orificePositionTracker.z,0)*2);
			orificePositionTracker.z=smoothstep(-(0.1624694 /*_Length*/)*0.2, (0.1624694 /*_Length*/)*0.2, orificePositionTracker.z) * orificePositionTracker.z;
			float distanceToOrifice = length( orificePositionTracker );
			float3 PhysicsNormal = normalize(penetratorNormalTracker.xyz);
			float enterFactor = smoothstep( (0.1624694 /*_Length*/) , (0.1624694 /*_Length*/)+0.05 , distanceToOrifice);
			float wriggleTimeY = _Time.y * (0.1 /*_WriggleSpeed*/);
			float curvatureMod = ( (0.1624694 /*_Length*/) * ( ( cos( wriggleTimeY ) * (0.0 /*_Wriggle*/) ) + (0.0 /*_Curvature*/) ) );
			float wriggleTimeX = _Time.y * ( (0.1 /*_WriggleSpeed*/) * 0.79 );
			float3 finalOrificeNormal = normalize( lerp( orificeNormal , ( PhysicsNormal + ( ( float3(0,1,0) * ( curvatureMod + ( (0.1624694 /*_Length*/) * ( (0.0 /*_ReCurvature*/) + ( ( sin( wriggleTimeY ) * 0.3 ) * (0.0 /*_Wriggle*/) ) ) * 2.0 ) ) ) + ( float3(0.5,0,0) * ( cos( wriggleTimeX ) * (0.0 /*_Wriggle*/) ) ) ) ) , enterFactor) );
			float3 finalOrificePosition = lerp( orificePositionTracker , ( ( normalize(penetratorNormalTracker) * (0.1624694 /*_Length*/) ) + ( float3(0,0.2,0) * ( sin( ( wriggleTimeY + UNITY_PI ) ) * (0.0 /*_Wriggle*/) ) * (0.1624694 /*_Length*/) ) + ( float3(0.2,0,0) * (0.1624694 /*_Length*/) * ( sin( ( wriggleTimeX + UNITY_PI ) ) * (0.0 /*_Wriggle*/) ) ) ) , enterFactor);
			float finalOrificeDistance = length( finalOrificePosition );
			float3 bezierBasePosition = float3(0,0,0);
			float bezierDistanceThird = ( finalOrificeDistance / 3.0 );
			float3 curvatureOffset = lerp( float3( 0,0,0 ) , ( float3(0,1,0) * ( curvatureMod * -0.2 ) ) , saturate( ( distanceToOrifice / (0.1624694 /*_Length*/) ) ));
			float3 bezierBaseNormal = ( ( bezierDistanceThird * float3(0,0,1) ) + curvatureOffset );
			float3 bezierOrificeNormal = ( finalOrificePosition - ( bezierDistanceThird * finalOrificeNormal ) );
			float3 bezierOrificePosition = finalOrificePosition;
			float vertexBaseTipPosition = ( VertexPosition.z / finalOrificeDistance );
			float3 sphereifyDistance = ( VertexPosition.xyz - float3(0,0, distanceToOrifice) );
			float3 sphereifyNormal = normalize( sphereifyDistance );
			float sphereifyFactor = smoothstep( 0.01 , -0.01 , distanceToOrifice - VertexPosition.z);
			sphereifyFactor *= 1-orificeType;
			VertexPosition.xyz = lerp( VertexPosition.xyz , ( float3(0,0, distanceToOrifice) + ( min( length( sphereifyDistance ) , (0.0283 /*_squeeze*/) ) * sphereifyNormal ) ) , sphereifyFactor);
			float squeezeFactor = smoothstep( 0.0 , (0.0 /*_SqueezeDist*/) , VertexPosition.z - distanceToOrifice);
			squeezeFactor = max( squeezeFactor , smoothstep( 0.0 , (0.0 /*_SqueezeDist*/) , distanceToOrifice - VertexPosition.z));
			squeezeFactor = 1- (1-squeezeFactor) * smoothstep(0,0.01,VertexPosition.z) * behind * (1-enterFactor);
			VertexPosition.xy = lerp( ( normalize(VertexPosition.xy) * min( length( VertexPosition.xy ) , (0.0283 /*_squeeze*/) ) ) , VertexPosition.xy , squeezeFactor);
			float bulgeFactor = 1-smoothstep( 0.0 , (0.3 /*_BulgeOffset*/) , abs( ( finalOrificeDistance - VertexPosition.z ) ));
			float bulgeFactorBaseClip = smoothstep( 0.0 , 0.05 , VertexPosition.z);
			VertexPosition.xy *= lerp( 1.0 , ( 1.0 + (0.01 /*_BulgePower*/) ) , ( bulgeFactor * bulgeFactorBaseClip * behind * (1-enterFactor)));
			float t = saturate(vertexBaseTipPosition);
			float oneMinusT = 1 - t;
			float3 bezierPoint = oneMinusT * oneMinusT * oneMinusT * bezierBasePosition + 3 * oneMinusT * oneMinusT * t * bezierBaseNormal + 3 * oneMinusT * t * t * bezierOrificeNormal + t * t * t * bezierOrificePosition;
			float3 straightLine = (float3(0.0 , 0.0 , VertexPosition.z));
			float baseFactor = smoothstep( 0.05 , -0.05 , VertexPosition.z);
			bezierPoint = lerp( bezierPoint , straightLine , baseFactor);
			bezierPoint = lerp( ( ( finalOrificeNormal * ( VertexPosition.z - finalOrificeDistance ) ) + finalOrificePosition ) , bezierPoint , step( vertexBaseTipPosition , 1.0 ));
			float3 bezierDerivitive = 3 * oneMinusT * oneMinusT * (bezierBaseNormal - bezierBasePosition) + 6 * oneMinusT * t * (bezierOrificeNormal - bezierBaseNormal) + 3 * t * t * (bezierOrificePosition - bezierOrificeNormal);
			bezierDerivitive = normalize( lerp( bezierDerivitive , float3(0,0,1) , baseFactor) );
			float bezierUpness = dot( bezierDerivitive , float3( 0,1,0 ) );
			float3 bezierUp = lerp( float3(0,1,0) , float3( 0,0,-1 ) , saturate( bezierUpness ));
			float bezierDownness = dot( bezierDerivitive , float3( 0,-1,0 ) );
			bezierUp = normalize( lerp( bezierUp , float3( 0,0,1 ) , saturate( bezierDownness )) );
			float3 bezierSpaceX = normalize( cross( bezierDerivitive , bezierUp ) );
			float3 bezierSpaceY = normalize( cross( bezierDerivitive , -bezierSpaceX ) );
			float3 bezierSpaceVertexOffset = ( ( VertexPosition.y * bezierSpaceY ) + ( VertexPosition.x * -bezierSpaceX ) );
			float3 bezierSpaceVertexOffsetNormal = normalize( bezierSpaceVertexOffset );
			float distanceFromTip = ( finalOrificeDistance - VertexPosition.z );
			float3 bezierSpaceVertexOffsetFinal = lerp( bezierSpaceVertexOffset , bezierSpaceVertexOffset , enterFactor);
			float3 bezierConstructedVertex = ( bezierPoint + bezierSpaceVertexOffsetFinal );
			VertexNormal = normalize( ( ( -bezierSpaceX * VertexNormal.x ) + ( bezierSpaceY * VertexNormal.y ) + ( bezierDerivitive * VertexNormal.z ) ) );
			VertexPosition.xyz = bezierConstructedVertex;
        }
    }
    float3 getBlendOffset(float blendSampleIndex, float activationDepth, float activationSmooth, int vertexID, float penetrationDepth, float3 normal, float3 tangent, float3 binormal)
    {
        float blendTextureSize = 1024;
        float2 blendSampleUV = (float2(((fmod((float)vertexID, blendTextureSize) + 0.5) / (blendTextureSize)), (((floor((vertexID / (blendTextureSize))) + 0.5) / (blendTextureSize)) + blendSampleIndex / 8)));
        float3 sampledBlend = tex2Dlod(_OrificeData, float4(blendSampleUV, 0, 0.0)).rgb;
        float blendActivation = smoothstep((activationDepth), (activationDepth + activationSmooth), penetrationDepth);
        blendActivation = -cos(blendActivation * 3.1416) * 0.5 + 0.5;
        float3 blendOffset = ((sampledBlend - float3(1, 1, 1)) * (blendActivation) * (1.0 /*_BlendshapePower*/) * (1.0 /*_BlendshapeBadScaleFix*/));
        return((blendOffset.x * normal) + (blendOffset.y * tangent) + (blendOffset.z * binormal));
    }
    #ifdef POI_SHADOW
        void applyRalivDynamicOrifaceSystem(inout VertexInputShadow v)
    #else
        void applyRalivDynamicOrifaceSystem(inout appdata v)
    #endif
    {
        
        if ((0.0 /*_OrifaceEnabled*/))
        {
            float penetratorLength = 0.1;
            float penetratorDistance;
            float3 orificePositionTracker = float3(0, 0, -100);
            float3 orificeNormalTracker = float3(0, 0, -99);
            float3 penetratorPositionTracker = float3(0, 0, 100);
            float orificeType = 0;
            GetBestLights((0.0 /*_OrificeChannel*/), orificeType, orificePositionTracker, orificeNormalTracker, penetratorPositionTracker, penetratorLength);
            penetratorDistance = distance(orificePositionTracker, penetratorPositionTracker);
            float penetrationDepth = (penetratorLength - penetratorDistance);
            float3 normal = normalize(v.normal);
            float3 tangent = normalize(v.tangent.xyz);
            float3 binormal = normalize(cross(normal, tangent));
            v.vertex.xyz += getBlendOffset(0, 0, (0.1 /*_EntryOpenDuration*/), v.vertexId, penetrationDepth, normal, tangent, binormal);
            v.vertex.xyz += getBlendOffset(2, (0.1 /*_Shape1Depth*/), (0.1 /*_Shape1Duration*/), v.vertexId, penetrationDepth, normal, tangent, binormal);
            v.vertex.xyz += getBlendOffset(4, (0.2 /*_Shape2Depth*/), (0.1 /*_Shape2Duration*/), v.vertexId, penetrationDepth, normal, tangent, binormal);
            v.vertex.xyz += getBlendOffset(6, (0.3 /*_Shape3Depth*/), (0.1 /*_Shape3Duration*/), v.vertexId, penetrationDepth, normal, tangent, binormal);
            v.vertex.w = 1;
            v.normal += getBlendOffset(1, 0, (0.1 /*_EntryOpenDuration*/), v.vertexId, penetrationDepth, normal, tangent, binormal);
            v.normal += getBlendOffset(3, (0.1 /*_Shape1Depth*/), (0.1 /*_Shape1Duration*/), v.vertexId, penetrationDepth, normal, tangent, binormal);
            v.normal += getBlendOffset(5, (0.2 /*_Shape2Depth*/), (0.1 /*_Shape2Duration*/), v.vertexId, penetrationDepth, normal, tangent, binormal);
            v.normal += getBlendOffset(7, (0.3 /*_Shape3Depth*/), (0.1 /*_Shape3Duration*/), v.vertexId, penetrationDepth, normal, tangent, binormal);
            v.normal = normalize(v.normal);
        }
    }
#endif
