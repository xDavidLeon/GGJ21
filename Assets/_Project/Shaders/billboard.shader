// Source: https://en.wikibooks.org/wiki/Cg_Programming/Unity/Billboards
// Source: http://www.unity3d-france.com/unity/phpBB3/viewtopic.php?t=12304
Shader "Custom/Billboard Y Rotation Only"
{
    Properties
    {
        _MainTex ("Texture Image", 2D) = "white" {}
        _ScaleX ("Scale X", Float) = 1.0
        _ScaleY ("Scale Y", Float) = 1.0
    }
 
    SubShader
    {  
        Cull Front
		ZWrite on
 
        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"
 
            #pragma shader_feature IGNORE_ROTATION_AND_SCALE
            #pragma vertex vert
            #pragma fragment frag
 
            // User-specified uniforms          
            uniform sampler2D _MainTex;      
            uniform float _ScaleX;
            uniform float _ScaleY;
           
            float4 _MainTex_ST;
 
            struct vertexInput
            {
                float4 vertex : POSITION;
                float4 tex : TEXCOORD0;
            };
 
            struct vertexOutput
            {
                float4 pos : SV_POSITION;
                float2 tex : TEXCOORD0;
            };
            vertexOutput vert(vertexInput input)
            {
                // The world position of the center of the object
                float3 worldPos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
 
                // Distance between the camera and the center
                float3 dist = _WorldSpaceCameraPos - worldPos;
 
                // atan2(dist.x, dist.z) = atan (dist.x / dist.z)
                // With atan the tree inverts when the camera has the same z position
                float angle = atan2(dist.x, dist.z);
 
                float3x3 rotMatrix;
                float cosinus = cos(angle);
                float sinus = sin(angle);
       
                // Rotation matrix in Y
                rotMatrix[0].xyz = float3(cosinus, 0, sinus);
                rotMatrix[1].xyz = float3(0, 1, 0);
                rotMatrix[2].xyz = float3(- sinus, 0, cosinus);
 
                // The position of the vertex after the rotation
                float4 newPos = float4(mul(rotMatrix, input.vertex * float4(_ScaleX, _ScaleY, 0, 0)), 1);
 
                // The model matrix without the rotation and scale
                float4x4 matrix_M_noRot = unity_ObjectToWorld;
                matrix_M_noRot[0][0] = 1;
                matrix_M_noRot[0][1] = 0;
                matrix_M_noRot[0][2] = 0;
 
                matrix_M_noRot[1][0] = 0;
                matrix_M_noRot[1][1] = 1;
                matrix_M_noRot[1][2] = 0;
 
                matrix_M_noRot[2][0] = 0;
                matrix_M_noRot[2][1] = 0;
                matrix_M_noRot[2][2] = 1;
 
                vertexOutput output;
 
                // The position of the vertex in clip space ignoring the rotation and scale of the object
                #if IGNORE_ROTATION_AND_SCALE
                output.pos = mul(UNITY_MATRIX_VP, mul(matrix_M_noRot, newPos));
                #else
                output.pos = mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, newPos));
                #endif
 
                output.tex = TRANSFORM_TEX(input.tex, _MainTex);
 
                return output;
            }
            float4 frag(vertexOutput input) : COLOR
            {
                float4 col = tex2D(_MainTex, input.tex.xy);  
				if (col.w < 0.1)
					discard;
				return col;
            }
            ENDCG
        }
    }
}

/*
Shader "Unlit/Billboard"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "DisableBatching" = "True" }

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 pos : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv.xy;

				// billboard mesh towards camera
				float3 vpos = mul((float3x3)unity_ObjectToWorld, v.vertex.xyz);
				float4 worldCoord = float4(unity_ObjectToWorld._m03, unity_ObjectToWorld._m13, unity_ObjectToWorld._m23, 1);
				float4 viewPos = mul(UNITY_MATRIX_V, worldCoord) + float4(vpos, 0);
				float4 outPos = mul(UNITY_MATRIX_P, viewPos);

				o.pos = outPos;

				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
*/