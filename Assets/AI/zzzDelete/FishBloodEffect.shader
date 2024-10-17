Shader "Unlit/FishBloodEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} 
        _BloodAccumulation ("Blood Accumulation", 2D) = "white" {} 
        _Radius ("Radius", Float) = 0.1 
        _TintStrength ("Tint Strength", Float) = 0.5 
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" } 
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1; 
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _BloodAccumulation;
            float _Radius;
            float _TintStrength;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenPos = ComputeScreenPos(o.pos); 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv); 
                fixed4 bloodTint = tex2D(_BloodAccumulation, i.uv); 

                
                col.rgb = lerp(col.rgb, col.rgb + bloodTint.rgb, bloodTint.a); 

                return col; 
            }
            ENDCG
        }
    }
}