// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ChromAb"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		 _Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0

		_OffsetBlueX("Blue Offset X", Range(-.1,.1)) = 0
		_OffsetBlueY("Blue Offset Y", Range(-.1,.1)) = 0

		_OffsetRedX("Red Offset X", Range(-.1,.1)) = 0
		_OffsetRedY("Red Offset Y", Range(-.1,.1)) = 0


		_OffsetGreenX("Green Offset X", Range(-.1,.1)) = 0
		_OffsetGreenY("Green Offset Y", Range(-.1,.1)) = 0
	}

		SubShader
		 {
			 Tags
		 {
			 "Queue" = "Transparent"
			 "IgnoreProjector" = "True"
			 "RenderType" = "Transparent"
			 "PreviewType" = "Plane"
			 "CanUseSpriteAtlas" = "True"
		 }

			 Cull Off
			 Lighting Off
			 ZWrite Off
			 Blend One OneMinusSrcAlpha

			 Pass
		 {
			 CGPROGRAM
	 #pragma vertex vert
	 #pragma fragment frag
	 #pragma multi_compile _ PIXELSNAP_ON
	 #include "UnityCG.cginc"

		 struct appdata_t
		 {
			 float4 vertex   : POSITION;
			 float4 color    : COLOR;
			 float2 texcoord : TEXCOORD0;
		 };

		 struct v2f
		 {
			 float4 vertex   : SV_POSITION;
			 fixed4 color : COLOR;
			 float2 texcoord  : TEXCOORD0;
		 };

		 fixed4 _Color;
		 float _OffsetRedX;
		 float _OffsetGreenX;
		 float _OffsetBlueX;
		 float _OffsetRedY;
		 float _OffsetGreenY;
		 float _OffsetBlueY;

		 v2f vert(appdata_t IN)
		 {
			 v2f OUT;
			 OUT.vertex = UnityObjectToClipPos(IN.vertex);
			 OUT.texcoord = IN.texcoord;


			 OUT.color = IN.color * _Color;
	 #ifdef PIXELSNAP_ON
			 OUT.vertex = UnityPixelSnap(OUT.vertex);
	 #endif

			 return OUT;
		 }

		 sampler2D _MainTex;
		 sampler2D _AlphaTex;
		 float _AlphaSplitEnabled;

		 fixed4 SampleSpriteTexture(float2 uv)
		 {
			 fixed4 color = tex2D(_MainTex, uv);

	 #if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
			 if (_AlphaSplitEnabled)
				 color.a = tex2D(_AlphaTex, uv).r;
	 #endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

			 return color;
		 }

		 fixed4 frag(v2f IN) : SV_Target
		 {
			 fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
			 fixed4 blue = SampleSpriteTexture(IN.texcoord + float2 (_OffsetBlueX, _OffsetBlueY)) * IN.color;
			 fixed4 red = SampleSpriteTexture(IN.texcoord + float2 (_OffsetRedX, _OffsetRedY)) * IN.color;
			 fixed4 green = SampleSpriteTexture(IN.texcoord + float2 (_OffsetGreenX, _OffsetGreenY)) * IN.color;

			 c.a = (green.a + blue.a + red.a) / 3;
			 c.g = green.g * green.a;
			 c.b = blue.b * blue.a;
			 c.r = red.r * red.a;

			 return c;

		 }
			 ENDCG
		 }
		 }
}