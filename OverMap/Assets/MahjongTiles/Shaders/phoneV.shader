// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "My/VFSpeculer"  
{  
    Properties  
    {  
        _MainTex ("Texture", 2D) = "white" {}  
        _Color ("Color Tint", Color) = (1, 1, 1, 1)  
        _Specular ("Specular", Color) = (1, 1, 1, 1)  
        _SpeInstantys("SpeculerInstanty",range(0,2)) = 1
        _Gloss ("Gloss", Range(8.0, 256)) = 20  
    }  
    SubShader  
    {  
        Pass  
        {  
            Tags  {"LightMode" = "ForwardBase"}
            CGPROGRAM  
            #pragma vertex vert  
            #pragma fragment frag  
              
            #include "Lighting.cginc"  
  
            struct appdata  
            {  
                float4 vertex : POSITION;  
                float2 uv : TEXCOORD0;  
                float3 normal : NORMAL;  
            };  
  
            struct v2f  
            {       
            	float4 vertex : SV_POSITION;           
                float3 worldNormal : TEXCOORD1;  
                float3 worldPos : TEXCOORD2; 
                float2 uv : TEXCOORD0;                   
            };  
  
            sampler2D _MainTex;  
            float4 _MainTex_ST;  
            fixed4 _Color;  
            fixed4 _Specular;  
            float _Gloss;  
            fixed _SpeInstantys;
  
            v2f vert (appdata v)  
            {  
                v2f o;  
                o.vertex = UnityObjectToClipPos(v.vertex);  
//              o.uv = v.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;  
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);  
  
                o.worldNormal = UnityObjectToWorldNormal(v.normal);  
  
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;  
  
                return o;  
            }  
  
            fixed4 frag (v2f i) : SV_Target  
            {  
                fixed3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb;  
  
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;  
  
                fixed3 worldNormal = normalize(i.worldNormal);  
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));  
  
                fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldNormal, worldLightDir));  
  
                fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));  
                fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(worldNormal, normalize(viewDir + worldLightDir))) * _SpeInstantys, _Gloss);  
  
                fixed3 color = ambient + diffuse + specular;  
                return fixed4(color, 1.0);  
            }  
            ENDCG  
        }  
    }  
  
    FallBack "Diffuse"  
}  