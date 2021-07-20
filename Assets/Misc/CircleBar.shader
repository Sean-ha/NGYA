Shader "CircularBarGradientTexture" {
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}

		_Frac("Progress Bar Value", Range(0,1)) = 1.0
		[NoScaleOffset] _AlphaTex("Alpha", 2D) = "White" {}
		[NoScaleOffset] _GradientTex("Gradient", 2D) = "white" {}
		_FillColor("Fill Color", Color) = (1,1,1,1)
		_BackColor("Back Color", Color) = (0,0,0,1)
	}

		SubShader{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
			}

				Cull Off
				Lighting Off
				ZWrite Off
				Blend One OneMinusSrcAlpha

			Pass{
				 CGPROGRAM
				 #pragma vertex vert
				 #pragma fragment frag

				 #include "UnityCG.cginc"

				 // Direct3D compiled stats:
				 // vertex shader:
				 //   8 math
				 // fragment shader:
				 //   2 math, 2 texture

				 half _Frac;
				 fixed4 _FillColor;
				 fixed4 _BackColor;

				 sampler2D _AlphaTex;
				 sampler2D _GradientTex;

				 struct v2f {
					  float4 pos : SV_POSITION;
					  float2 uv : TEXCOORD0;
				 };

				 v2f vert(appdata_img v)
				 {
					  v2f o;
					  o.pos = UnityObjectToClipPos(v.vertex);
					  o.uv.xy = v.texcoord.xy;
					  return o;
				 }

				 fixed4 frag(v2f i) : SV_Target
				 {
					// sample gradient texture
					fixed gradient = tex2D(_GradientTex, i.uv).r;

					// ternary to pick between fill and background colors
					fixed4 col = (_Frac >= gradient) ? _FillColor : _BackColor;

					return col;
				 }
		 ENDCG
		 }
		}
}