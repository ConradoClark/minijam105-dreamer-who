// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Frosty/Unlit"
{
	Properties
	{
		[Header(Texture)]
		_MainTex ("Main Texture", 2D) = "white" {}

		[Header(Color)]
		_Color("Tint", Color) = (1,1,1,1)
		_Saturation("Saturation", Range(-1,1)) = 0
		_Hue("Hue Shifting", Range(0,255)) = 0
		_Luminance("Luminance", Range(-1,1)) = 0
		_Opacity("Opacity", Range(0,1)) = 1 
		_Colorize("Colorize",Color) = (1,1,1,0)
		[MaterialToggle] _InvertInput("Invert Colors Before Processing",float) = 0
		[MaterialToggle] _InvertOutput("Invert Colors After Processing",float) = 0

		[Header(Color Replacement)]
		_ColorReplSource1("Replacement Source 1", Color) = (1,1,1,1)
		_ColorReplTarget1("Replacement Target 1", Color) = (1,1,1,1)
		_ColorReplTolerance1("Replacement Tolerance 1", Range(0,255)) = 0

		_ColorReplSource2("Replacement Source 2", Color) = (1,1,1,1)
		_ColorReplTarget2("Replacement Target 2", Color) = (1,1,1,1)
		_ColorReplTolerance2("Replacement Tolerance 2", Range(0,255)) = 0

		_ColorReplSource3("Replacement Source 3", Color) = (1,1,1,1)
		_ColorReplTarget3("Replacement Target 3", Color) = (1,1,1,1)
		_ColorReplTolerance3("Replacement Tolerance 3", Range(0,255)) = 0

		_ColorReplSource4("Replacement Source 4", Color) = (1,1,1,1)
		_ColorReplTarget4("Replacement Target 4", Color) = (1,1,1,1)
		_ColorReplTolerance4("Replacement Tolerance 4", Range(0,255)) = 0

		_ColorReplSource5("Replacement Source 5", Color) = (1,1,1,1)
		_ColorReplTarget5("Replacement Target 5", Color) = (1,1,1,1)
		_ColorReplTolerance5("Replacement Tolerance 5", Range(0,255)) = 0

		[Header(Color Levels)]
		_LevelsMinInput("RGB - Min Input",Range(0,255)) = 0
		_LevelsGamma("RGB - Gamma",Range(0.01,10)) = 1
		_LevelsMaxInput("RGB - Max Input",Range(0,255)) = 255

		_LevelsRMinInput("Red - Min Input",Range(0,255)) = 0
		_LevelsRGamma("Red - Gamma",Range(0.01,10)) = 1
		_LevelsRMaxInput("Red - Max Input",Range(0,255)) = 255

		_LevelsGMinInput("Green - Min Input",Range(0,255)) = 0
		_LevelsGGamma("Green - Gamma",Range(0.01,10)) = 1
		_LevelsGMaxInput("Green - Max Input",Range(0,255)) = 255

		_LevelsBMinInput("Blue - Min Input",Range(0,255)) = 0
		_LevelsBGamma("Blue - Gamma",Range(0.01,10)) = 1
		_LevelsBMaxInput("Blue - Max Input",Range(0,255)) = 255

		_LevelsCustom1Color("Custom Color 1",Color) = (1,1,1,0)
		_LevelsCustom1MinInput("Custom Color 1 - Min Input",Range(0,255)) = 0
		_LevelsCustom1Gamma("Custom Color 1 - Gamma",Range(0.01,10)) = 1
		_LevelsCustom1MaxInput("Custom Color 1 - Max Input",Range(0,255)) = 255

		_LevelsCustom2Color("Custom Color 2",Color) = (1,1,1,0)
		_LevelsCustom2MinInput("Custom Color 2 - Min Input",Range(0,255)) = 0
		_LevelsCustom2Gamma("Custom Color 2 - Gamma",Range(0.01,10)) = 1
		_LevelsCustom2MaxInput("Custom Color 2 - Max Input",Range(0,255)) = 255

		_LevelsCustom3Color("Custom Color 3",Color) = (1,1,1,0)
		_LevelsCustom3MinInput("Custom Color 3 - Min Input",Range(0,255)) = 0
		_LevelsCustom3Gamma("Custom Color 3 - Gamma",Range(0.01,10)) = 1
		_LevelsCustom3MaxInput("Custom Color 3 - Max Input",Range(0,255)) = 255

		[Header(Color to Opacity)]
		_LevelsOpacitySource("Replacement Source 1",Color) = (1,1,1,1)
		_LevelsOpacityValue("Replacement 1 - Opacity Value",Range(0,1)) = 0
		_LevelsOpacityGradient("Replacement 1 - Gradient by Range",Range(0,1)) = 0
		_LevelsOpacityTolerance("Replacement 1 - Tolerance",Range(0,255)) = 0

		_LevelsOpacitySource2("Replacement Source 2",Color) = (1,1,1,1)
		_LevelsOpacityValue2("Replacement 2 - Opacity Value",Range(0,1)) = 0
		_LevelsOpacityGradient2("Replacement 2 - Gradient by Range",Range(0,1)) = 0
		_LevelsOpacityTolerance2("Replacement 2 - Tolerance",Range(0,255)) = 0

		_LevelsOpacitySource3("Replacement Source 3",Color) = (1,1,1,1)
		_LevelsOpacityValue3("Replacement 3 - Opacity Value",Range(0,1)) = 0
		_LevelsOpacityGradient3("Replacement 3 - Gradient by Range",Range(0,1)) = 0
		_LevelsOpacityTolerance3("Replacement 3 - Tolerance",Range(0,255)) = 0

		[Header(UV Scrolling)]
		_HAutoScroll ("Scroll - Horizontal Scrolling Speed", float) = 0
		_VAutoScroll("Scroll - Vertical Scrolling Speed", float) = 0

		[Header(Cutoff)]
		_CutoffTexture("Cutoff Texture", 2D) = "white" {}
		[MaterialToggle] _EnableCutoff("Enable Cutoff",float) = 0
		_CutoffFactor("Cutoff Factor", Range(0,1)) = 1
		[MaterialToggle] _EnableCutoffColorization("Enable Cutoff Colorization",float) = 0
		_CutoffColor50("Cutoff Color at 50", Color) = (0,0,0,0)
		_CutoffColor15("Cutoff Color at 15", Color) = (0,0,0,0)

		[Header(FadeIn Effects)]
		[MaterialToggle] _FadeInStripsHorizontal("FadeIn - HorizontalStrips", float) = 0
		_FadeInStripsHorizontalStrips("FadeIn - HorizontalStrips - Number of Strips per UV", float) = 1
		[MaterialToggle] _FadeInStripsHorizontalReverse("FadeIn - HorizontalStrips - Reverse", float) = 0
		_FadeInStripsHorizontalProgress("FadeIn - HorizontalStrips - Progress",Range(0,1)) = 0
		[MaterialToggle] _FadeInStripsVertical("FadeIn - VerticalStrips", float) = 0
		_FadeInStripsVerticalStrips("FadeIn - VerticalStrips - Number of Strips per UV", float) = 1
		[MaterialToggle] _FadeInStripsVerticalReverse("FadeIn - VerticalStrips - Reverse", float) = 0
		_FadeInStripsVerticalProgress("FadeIn - VerticalStrips - Progress",Range(0,1)) = 0
	}
	SubShader
	{

		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha 
		Cull Off
		
		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag			
			
			#include "UnityCG.cginc"

			#define GammaCorrection(color, gamma)  pow(color, 1.0 / gamma)
			#define LevelsControlInputRange(color, minInput, maxInput) min(max(color - half4(minInput,minInput,minInput,0), 0.0) / ( half4(maxInput,maxInput,maxInput,1) - half4(minInput,minInput,minInput,0)), 1.0)
			#define LevelsControlInput(color, minInput, gamma, maxInput) GammaCorrection(LevelsControlInputRange(color, minInput/255, maxInput/255), gamma)

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 screenPos:TEXCOORD2;
				UNITY_FOG_COORDS(1)
				float4 vertex : POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			fixed4 _Color;
			half _Saturation;
			half _Hue;
			half _Luminance;
			half _Opacity;
			fixed4 _Colorize;
			half _InvertInput;
			half _InvertOutput;
			
			float _HAutoScroll;
			float _VAutoScroll;

			half _FadeInStripsHorizontal;
			half _FadeInStripsHorizontalReverse;
			float _FadeInStripsHorizontalStrips;
			float _FadeInStripsHorizontalProgress;

			half _FadeInStripsVertical;
			half _FadeInStripsVerticalReverse;
			float _FadeInStripsVerticalStrips;
			float _FadeInStripsVerticalProgress;

			float _LevelsMinInput;
			float _LevelsGamma;
			float _LevelsMaxInput;

			float _LevelsRMinInput;
			float _LevelsRGamma;
			float _LevelsRMaxInput;

			float _LevelsGMinInput;
			float _LevelsGGamma;
			float _LevelsGMaxInput;

			float _LevelsBMinInput;
			float _LevelsBGamma;
			float _LevelsBMaxInput;

			fixed4 _LevelsCustom1Color;
			float _LevelsCustom1MinInput;
			float _LevelsCustom1Gamma;
			float _LevelsCustom1MaxInput;

			fixed4 _LevelsCustom2Color;
			float _LevelsCustom2MinInput;
			float _LevelsCustom2Gamma;
			float _LevelsCustom2MaxInput;

			fixed4 _LevelsCustom3Color;
			float _LevelsCustom3MinInput;
			float _LevelsCustom3Gamma;
			float _LevelsCustom3MaxInput;

			fixed4 _ColorReplSource1;
			fixed4 _ColorReplTarget1;
			float _ColorReplTolerance1;

			fixed4 _ColorReplSource2;
			fixed4 _ColorReplTarget2;
			float _ColorReplTolerance2;

			fixed4 _ColorReplSource3;
			fixed4 _ColorReplTarget3;
			float _ColorReplTolerance3;

			fixed4 _ColorReplSource4;
			fixed4 _ColorReplTarget4;
			float _ColorReplTolerance4;

			fixed4 _ColorReplSource5;
			fixed4 _ColorReplTarget5;
			float _ColorReplTolerance5;

			fixed4 _LevelsOpacitySource;
			float _LevelsOpacityValue;
			float _LevelsOpacityGradient;
			float _LevelsOpacityTolerance;

			fixed4 _LevelsOpacitySource2;
			float _LevelsOpacityValue2;
			float _LevelsOpacityGradient2;
			float _LevelsOpacityTolerance2;

			fixed4 _LevelsOpacitySource3;
			float _LevelsOpacityValue3;
			float _LevelsOpacityGradient3;
			float _LevelsOpacityTolerance3;

			sampler2D _CutoffTexture;
			float _EnableCutoff;
			float _CutoffFactor;
			float _EnableCutoffColorization;
			fixed4 _CutoffColor50;
			fixed4 _CutoffColor15;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex = UnityPixelSnap(o.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);

				// UV Scrolling
				o.uv = TRANSFORM_TEX(v.uv, _MainTex) - float2(_Time[1] * _HAutoScroll, _Time[1] * _VAutoScroll);
				return o;
			}

			float getHue(fixed4 color) {
				float Epsilon = 1e-10;
				float4 P = (color.g < color.b) ? float4(color.bg, -1.0, 2.0 / 3.0) : float4(color.gb, 0.0, -1.0 / 3.0);
				float4 Q = (color.r < P.x) ? float4(P.xyw, color.r) : float4(color.r, P.yzx);
				float C = Q.x - min(Q.w, Q.y);
				float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
				return H;
			}

			fixed4 shiftHue(fixed4 originalColor, float hue) {
				fixed4 col = originalColor;
				float pi = 3.14159265358979323846;
				float U = cos(hue*pi / 180);
				float W = sin(hue*pi / 180);

				fixed4 colCopy = col;
				col.r = (.299 + .701*U + .168*W)*colCopy.r
					+ (.587 - .587*U + .330*W)*colCopy.g
					+ (.114 - .114*U - .497*W)*colCopy.b;
				col.g = (.299 - .299*U - .328*W)*colCopy.r
					+ (.587 + .413*U + .035*W)*colCopy.g
					+ (.114 - .114*U + .292*W)*colCopy.b;
				col.b = (.299 - .3*U + 1.25*W)*colCopy.r
					+ (.587 - .588*U - 1.05*W)*colCopy.g
					+ (.114 + .886*U - .203*W)*colCopy.b;
				return col;
			}
			
			fixed4 replaceColor(fixed4 originalColor, fixed4 source, fixed4 target, float tolerance) {
				fixed4 col = originalColor;
				float colorDist = distance(getHue(col), getHue(source));
				
				if (colorDist < tolerance / 255) {
					col = shiftHue(col, getHue(col) * 360 - getHue(target) * 360);
				}
				return col;
			}

			float replaceOpacity(fixed4 originalColor, fixed4 source, float target, float mix, float tolerance) {
				fixed4 col = originalColor;
				float colorDist = distance(col, source);

				if (colorDist < tolerance / 255 && col.a>0.1) {
					col.a = ((1 - colorDist) * target * mix) + (1 - mix)*target;
				}
				return col.a;
			}

			void processFadeInEffects(v2f i) {
				float uvx = (_FadeInStripsHorizontalReverse ? _MainTex_ST.x* i.vertex.x * 2 : 0) + (i.uv.x) * (_FadeInStripsHorizontalReverse ? -1 : 1);
				float uvy = (_FadeInStripsVerticalReverse ? _MainTex_ST.y* i.vertex.y * 2 : 0) + i.uv.y * (_FadeInStripsVerticalReverse ? -1 : 1);

				float checker = !_FadeInStripsHorizontal ? 1 : (fmod(uvx, 1/ _FadeInStripsHorizontalStrips) > _FadeInStripsHorizontalProgress / _FadeInStripsHorizontalStrips ? -1 : 1);
				checker = (!_FadeInStripsVertical || checker == -1) ? checker : fmod(uvy, 1/_FadeInStripsVerticalStrips) > _FadeInStripsVerticalProgress / _FadeInStripsVerticalStrips ? -1 : 1;

				clip(checker);
			}

			fixed4 frag(v2f i) : SV_Target
			{				
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				col.rgb = !_InvertInput ? col.rgb : 1 - col.rgb;

				_CutoffFactor -= lerp(0.15, -0.15, _CutoffFactor) +0.1;

				if (_EnableCutoffColorization == 1) {
					_Colorize = fixed4(_CutoffColor50.r, _CutoffColor50.g, _CutoffColor50.b, 0.95-_CutoffFactor/2);
					_Colorize *= fixed4(_CutoffColor15.r, _CutoffColor15.g, _CutoffColor15.b, 0.95-_CutoffFactor);
				}
				
				// Cutoff
				if (_EnableCutoff != 0 && col.a > 0.1) {
					fixed4 cutoff = tex2D(_CutoffTexture, i.uv);

					if (_CutoffFactor >= 1.0001 - cutoff.r) {
						col.a = 1;
					}
					else
					{
						col.a = lerp(1.5 - 0.4 * _CutoffFactor, -0.7 * _CutoffFactor, 1 - pow(_CutoffFactor + cutoff.r, 10) + 0.15);
					}
				}

				col = shiftHue(col, _Hue);
				half sat = saturate(Luminance(col.rgb));

				// Color Replacement
				col = replaceColor(col, _ColorReplSource1, _ColorReplTarget1, _ColorReplTolerance1);
				col = replaceColor(col, _ColorReplSource2, _ColorReplTarget2, _ColorReplTolerance2);
				col = replaceColor(col, _ColorReplSource3, _ColorReplTarget3, _ColorReplTolerance3);
				col = replaceColor(col, _ColorReplSource4, _ColorReplTarget4, _ColorReplTolerance4);
				col = replaceColor(col, _ColorReplSource5, _ColorReplTarget5, _ColorReplTolerance5);

				// Colorize, Luminance, Saturation
				col = fixed4(sat * _Colorize.r* _Colorize.a + col.r*(1-_Colorize.a),
							 sat * _Colorize.g * _Colorize.a + col.g*(1 - _Colorize.a),
							 sat * _Colorize.b * _Colorize.a + col.b*(1 - _Colorize.a), col.a);

				col.rgb = lerp(col.rgb, fixed3(sat, sat, sat), -_Saturation);

				col.rgb = _Luminance >= 0 ? lerp(col.rgb, fixed3(1, 1, 1), _Luminance)
					: lerp(col.rgb, fixed3(0, 0, 0), -_Luminance);

				// Opacity
				col.a *= _Opacity;
				
				// Color Levels
				col = LevelsControlInput(col, _LevelsMinInput, _LevelsGamma, _LevelsMaxInput);
				col.r = LevelsControlInput(col, _LevelsRMinInput, _LevelsRGamma, _LevelsRMaxInput).r;
				col.g = LevelsControlInput(col, _LevelsGMinInput, _LevelsGGamma, _LevelsGMaxInput).g;
				col.b = LevelsControlInput(col, _LevelsBMinInput, _LevelsBGamma, _LevelsBMaxInput).b;
				
				// Custom Color Levels
				col.rgb = col.rgb * (1-_LevelsCustom1Color.rgb *_LevelsCustom1Color.a) + LevelsControlInput(col, _LevelsCustom1MinInput, _LevelsCustom1Gamma, _LevelsCustom1MaxInput).rgb * _LevelsCustom1Color.rgb *_LevelsCustom1Color.a;
				col.rgb = col.rgb * (1 - _LevelsCustom2Color.rgb *_LevelsCustom2Color.a) + LevelsControlInput(col, _LevelsCustom2MinInput, _LevelsCustom2Gamma, _LevelsCustom2MaxInput).rgb * _LevelsCustom2Color.rgb *_LevelsCustom2Color.a;
				col.rgb = col.rgb * (1 - _LevelsCustom3Color.rgb *_LevelsCustom3Color.a) + LevelsControlInput(col, _LevelsCustom3MinInput, _LevelsCustom3Gamma, _LevelsCustom3MaxInput).rgb * _LevelsCustom3Color.rgb *_LevelsCustom3Color.a;

				// Convert Color to Opacity
				col.a = replaceOpacity(col, _LevelsOpacitySource, _LevelsOpacityValue, _LevelsOpacityGradient, _LevelsOpacityTolerance);
				col.a = replaceOpacity(col, _LevelsOpacitySource2, _LevelsOpacityValue2, _LevelsOpacityGradient2, _LevelsOpacityTolerance2);
				col.a = replaceOpacity(col, _LevelsOpacitySource3, _LevelsOpacityValue3, _LevelsOpacityGradient3, _LevelsOpacityTolerance3);

				// Invert Output				
				col.rgb = !_InvertOutput ? col.rgb : 1 - col.rgb;
				
				// Fade In Effects
				processFadeInEffects(i);

				return col;
			}

			ENDCG
		}
	}
}