
Shader "Hologram/HologramV1" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _MainColor ("MainColor", Color) = (0.4558824,1,0.9774847,1)
        _DisortWaves ("DisortWaves", Float ) = 2
        _Speedofbigwaves ("Speed  of big  waves", Float ) = 1
        _Timeofbigwaves ("Time of big waves", Range(0, 1)) = 0.6324792
        _Strongofbigwaves_copy ("Strong  of big  waves_copy", Range(0, 10)) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _MainColor;
            uniform float _DisortWaves;
            uniform float _Speedofbigwaves;
            uniform float _Timeofbigwaves;
            uniform float _Strongofbigwaves_copy;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 node_6583 = _Time;
                float2 node_4167 = (_DisortWaves*i.uv0);
                float node_7338 = frac((node_4167+node_6583.g*float2(1,-1)).g);
                float node_8729 = 1.0;
                float4 node_7243 = _Time;
                float node_2887_if_leA = step(sin((node_7243.g*_Speedofbigwaves)),(1.0 - _Timeofbigwaves));
                float node_2887_if_leB = step((1.0 - _Timeofbigwaves),sin((node_7243.g*_Speedofbigwaves)));
                float node_9903 = 4.0;
                float node_9716 = 0.2;
                float2 node_1479_tc_rcp = float2(1.0,1.0)/float2( node_9903, 1.0 );
                float node_1479_ty = floor(node_9716 * node_1479_tc_rcp.x);
                float node_1479_tx = node_9716 - node_9903 * node_1479_ty;
                float2 node_1479 = (i.uv0 + float2(node_1479_tx, node_1479_ty)) * node_1479_tc_rcp;
                float node_5865 = (saturate((1.0 - (abs((clamp(frac((node_1479+node_6583.g*float2(0,-0.5)).g),0,0.5)+(-0.5)))*10.0)))*(0.05*_Strongofbigwaves_copy));
                float node_9875 = 45.0;
                float node_7895 = (floor(lerp((node_2887_if_leA*0.0)+(node_2887_if_leB*node_5865),node_5865,node_2887_if_leA*node_2887_if_leB) * node_9875) / (node_9875 - 1)+floor((pow(abs((frac((node_4167+node_6583.g*float2(1,-1)).g)-0.5)),10.0)*50.0) * node_9875) / (node_9875 - 1));
                float2 node_6781_tc_rcp = float2(1.0,1.0)/float2( node_8729, node_8729 );
                float node_6781_ty = floor(node_7895 * node_6781_tc_rcp.x);
                float node_6781_tx = node_7895 - node_8729 * node_6781_ty;
                float2 node_6781 = (i.uv0 + float2(node_6781_tx, node_6781_ty)) * node_6781_tc_rcp;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_6781, _MainTex));
                float3 node_6657 = (_MainTex_var.rgb*float3(1,1,1)*(_MainColor.rgb*1.0));
                float3 emissive = (((_MainColor.rgb*4.0)*(pow(node_7338,10.0)*node_6657))+node_6657);
                float3 finalColor = emissive;
                return fixed4(finalColor,saturate(((0.6*_MainTex_var.a)+(_MainTex_var.a*node_7338*(abs((frac(((i.uv0*256.0)+node_6583.g*float2(1,-4)).g)-0.5))*2.0)))));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
