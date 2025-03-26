Shader "Custom/DriftTrackShader" {
	Properties {
		_Colour2 ("Main Colour", COLOR) = (1,1,1,1)
		_MainTex ("Original Texture", 2D) = "white" {}
		_Colour1 ("Highlight Colour", COLOR) = (1,1,1,1)
		_HighlightTex ("Highlight Texture", 2D) = "white" {}
		[PerRendererData]_HighlightZ ("Current Z position of the effect", float) = 0
		_TransitionSize ("Size of transition", float) = 2
		_StartingZ("Starting point of the effect", float) = -10
	}
    SubShader {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Back
        LOD 200
 
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0
 
        sampler2D _MainTex;
 
        struct Input {
            float2 uv_MainTex;
        };
 
        fixed4 _Colour1;
        fixed4 _Colour2;
 
        void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Colour2;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
 
 
		CGPROGRAM
		#pragma surface surf Standard alpha:fade vertex:vert
 
		struct Input
		{
			float2 uv_MainTex;
			float3 localPos;
		};
 
		sampler2D _MainTex;
		fixed4 _Colour1;
 
		sampler2D _HighlightTex;
		fixed4 _Colour2;
		float _HighlightZ;
		float _TransitionSize;
		float _StartingZ;
 
		void vert (inout appdata_base v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT (Input, o);
			o.localPos = v.vertex.xyz;
		}
 
		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 col = tex2D (_MainTex, IN.uv_MainTex) * _Colour1;
 
			float transition = _HighlightZ - IN.localPos.z;
			clip (_StartingZ + (transition + (tex2D (_HighlightTex, IN.uv_MainTex) * _Colour2) * _TransitionSize));
 
			o.Albedo = col.rgb;
			o.Alpha = col.a;
		}
		ENDCG
       }
    }
