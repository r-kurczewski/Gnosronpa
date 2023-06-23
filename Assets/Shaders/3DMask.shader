Shader "Custom/3DMask"
{
	SubShader
	{
		Tags{"Queue" = "Transparent+1"}
		

		Pass{
			Blend Zero One
		}
	}
}
