Shader "SLG/TextureSticker"
{   
    SubShader
    {
        Cull Off 
        ZWrite Off 
        ZTest Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _StickerTex;
            float _StickerTex_ST;
            
            float3 _StickerPosition;
            float _Size;

            struct appdata
            {
                float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
            };

            float mask(float3 position, float3 center, float size, float hardness)
            {
                float m = distance(center, position);
                return 1 - smoothstep(size * hardness, size, m);
            }

            v2f vert (appdata v)
            {
                v2f o;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = v.uv;
				float4 uv = float4(0, 0, 0, 1);
                uv.xy = float2(1, _ProjectionParams.x) * (v.uv.xy * float2( 2, 2) - float2(1, 1));
				o.vertex = uv; 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                float4 stickerCol = tex2D(_StickerTex, i.uv);
                float f = mask(i.worldPos, _StickerPosition, _Size, 1.0f/*_Hardness*/);
                float edge = f * 1.0f/*_Strength*/;
                return lerp(stickerCol, col, edge) * stickerCol.a;
            }
            ENDCG
        }
    }
}