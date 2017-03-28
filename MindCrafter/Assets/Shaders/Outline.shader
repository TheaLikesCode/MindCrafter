
Shader "Custom/Outline" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BlinkInterval("BlinkInterval", float) = 0.5
		_BlinkIntensity("BlinkIntensity", float) = 0.5
		_BlinkColor("BlinkColor",Color) = (0.3, 2.5, 0.5)
	}
	SubShader {
	//	Tags { "RenderType"="Opaque" }
	//	LOD 200
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		float _BlinkInterval,_BlinkIntensity;
		fixed4 _BlinkColor;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Alpha = 0.3;//0.5+c.a;
			o.Albedo = c;
				// _BlinkColor*(0.5 + c.rgb*cos(_Time.x*140.0/_BlinkInterval))*((0.2 + IN.uv_MainTex.x ))*_BlinkIntensity;
			
			//bool isVisible = true;
			//if(max(cos(_Time.x*140.0/_BlinkInterval), 0.2)<0.1) o.Albedo = 0.3 + c.rgb-half3(1, .1, 0);
			
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
