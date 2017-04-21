// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LCX/ImageEffcetOne"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_RADIUSBUCE("_RADIUSBUCE", Range(0,0.5)) = 0.2
	}
		SubShader
		{

			Pass
			{
				CGPROGRAM
				#pragma exclude _renderers gles
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				float _RADIUSBUCE;
				sampler2D _MainTex;

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
				float2 RadiusBuceVU : TEXCOORD1;
			};

			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.RadiusBuceVU = v.texcoord - float2(0.5,0.5);   //将uv中心点归零。
				return o;
			}
			
	

			fixed4 frag (v2f i) : COLOR
			{
				fixed4 col ;
				col = (0,1,1,0);
		
				if (abs(i.RadiusBuceVU.x) < 0.5 - _RADIUSBUCE || abs(i.RadiusBuceVU.y) < 0.5 - _RADIUSBUCE)        //计算四条边界减去半径后的区域
				{
					col = tex2D(_MainTex, i.uv);
				}
				else
				{
					if (length(abs(i.RadiusBuceVU) - float2(0.5 - _RADIUSBUCE, 0.5 - _RADIUSBUCE)) < _RADIUSBUCE)    //取uv图的四个边界点，选取圆形的中心，渲染在圆形区域内部的图像
					{
						col = tex2D(_MainTex, i.uv);
					}
					else
					{
						discard;
					}
				}
				return col;
			}
			ENDCG
		}
	}
}
