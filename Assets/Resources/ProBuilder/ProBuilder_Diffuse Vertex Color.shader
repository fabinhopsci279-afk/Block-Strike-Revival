// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_ShadowFadeCenterAndType', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "ProBuilder/Diffuse Vertex Color"
{
  Properties
  {
    _MainTex ("Texture", 2D) = "white" {}
  }
  SubShader
  {
    Tags
    { 
      "RenderType" = "Opaque"
    }
    Pass // ind: 1, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "LIGHTMODE" = "FORWARDBASE"
        "RenderType" = "Opaque"
        "SHADOWSUPPORT" = "true"
      }
      ZClip Off
      ColorMask RGB
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      #define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)
      
      
      #define CODE_BLOCK_VERTEX
      uniform float4 _MainTex_ST;
      //uniform float4 _ProjectionParams;
      //uniform float4 unity_4LightPosX0;
      //uniform float4 unity_4LightPosY0;
      //uniform float4 unity_4LightPosZ0;
      //uniform float4 unity_4LightAtten0;
      //uniform float4 unity_LightColor;
      //uniform float4 unity_SHBr;
      //uniform float4 unity_SHBg;
      //uniform float4 unity_SHBb;
      //uniform float4 unity_SHC;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4x4 unity_MatrixVP;
      //uniform float4 unity_Lightmap_HDR;
      uniform float4 _LightColor0;
      //uniform float3 _WorldSpaceCameraPos;
      //uniform float4 _WorldSpaceLightPos0;
      //uniform float4 _LightShadowData;
      // uniform float4 unity_ShadowFadeCenterAndType;
      //uniform float4x4 unity_MatrixV;
      uniform sampler2D _MainTex;
      uniform sampler2D _ShadowMapTexture;
      // uniform sampler2D unity_Lightmap;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float4 tangent :TANGENT;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
          float4 color :COLOR;
      };
      
      struct OUT_Data_Vert
      {
          float4 vertex :SV_POSITION;
          float2 texcoord :TEXCOORD0;
          float3 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
          float4 color :COLOR;
          float3 texcoord3 :TEXCOORD3;
          float4 texcoord4 :TEXCOORD4;
          float4 texcoord6 :TEXCOORD6;
      };
      
      struct v2f
      {
          float4 vertex :SV_POSITION;
          float2 texcoord :TEXCOORD0;
          float3 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
          float4 color :COLOR;
          float4 texcoord3 :TEXCOORD3;
          float4 texcoord4 :TEXCOORD4;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target;
      };
      
      float4 u_xlat0;
      float4 u_xlat1;
      float4 u_xlat2;
      float4 u_xlat3;
      float4 u_xlat4;
      float4 u_xlat5;
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          u_xlat0 = (in_v.vertex.yyyy * conv_mxt4x4_-2(unity_WorldToObject));
          u_xlat0 = ((conv_mxt4x4_-3(unity_WorldToObject) * in_v.vertex.xxxx) + u_xlat0);
          u_xlat0 = ((conv_mxt4x4_-1(unity_WorldToObject) * in_v.vertex.zzzz) + u_xlat0);
          u_xlat1 = (u_xlat0 + conv_mxt4x4_0(unity_WorldToObject));
          u_xlat0.xyz = ((conv_mxt4x4_0(unity_WorldToObject).xyz * in_v.vertex.www) + u_xlat0.xyz);
          u_xlat1 = mul(unity_MatrixVP, u_xlat1);
          out_v.vertex = u_xlat1;
          out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          u_xlat2.x = dot(in_v.normal.xyzx, conv_mxt4x4_1(unity_WorldToObject).xyzx);
          u_xlat2.y = dot(in_v.normal.xyzx, conv_mxt4x4_2(unity_WorldToObject).xyzx);
          u_xlat2.z = dot(in_v.normal.xyzx, conv_mxt4x4_3(unity_WorldToObject).xyzx);
          u_xlat0.w = dot(u_xlat2.xyzx, u_xlat2.xyzx);
          u_xlat0.w = rsqrt(u_xlat0.w);
          u_xlat2.xyz = (u_xlat0.www * u_xlat2.xyz);
          out_v.texcoord1.xyz = u_xlat2.xyz;
          out_v.texcoord2.xyz = u_xlat0.xyz;
          out_v.color = in_v.color;
          u_xlat3 = ((-u_xlat0.xxxx) + unity_SHC);
          u_xlat4 = ((-u_xlat0.yyyy) + unity_SHC);
          u_xlat0 = ((-u_xlat0.zzzz) + unity_SHC);
          u_xlat5 = (u_xlat2.yyyy * u_xlat4);
          u_xlat4 = (u_xlat4 * u_xlat4);
          u_xlat4 = ((u_xlat3 * u_xlat3) + u_xlat4);
          u_xlat3 = ((u_xlat3 * u_xlat2.xxxx) + u_xlat5);
          u_xlat3 = ((u_xlat0 * u_xlat2.zzzz) + u_xlat3);
          u_xlat0 = ((u_xlat0 * u_xlat0) + u_xlat4);
          u_xlat0 = max(u_xlat0, float4(1E-06, 1E-06, 1E-06, 1E-06));
          u_xlat4 = rsqrt(u_xlat0);
          u_xlat0 = ((u_xlat0 * unity_SHC) + float4(1, 1, 1, 1));
          u_xlat0 = (float4(1, 1, 1, 1) / u_xlat0);
          u_xlat3 = (u_xlat3 * u_xlat4);
          u_xlat3 = max(u_xlat3, float4(0, 0, 0, 0));
          u_xlat0 = (u_xlat0 * u_xlat3);
          u_xlat3.xyz = (u_xlat0.yyy * unity_SHC.xyz);
          u_xlat3.xyz = ((unity_SHC.xyz * u_xlat0.xxx) + u_xlat3.xyz);
          u_xlat0.xyz = ((unity_SHC.xyz * u_xlat0.zzz) + u_xlat3.xyz);
          u_xlat0.xyz = ((unity_SHC.xyz * u_xlat0.www) + u_xlat0.xyz);
          u_xlat3.xyz = ((u_xlat0.xyz * float3(0.305306, 0.305306, 0.305306)) + float3(0.682171, 0.682171, 0.682171));
          u_xlat3.xyz = ((u_xlat0.xyz * u_xlat3.xyz) + float3(0.012523, 0.012523, 0.012523));
          u_xlat0.w = (u_xlat2.y * u_xlat2.y);
          u_xlat0.w = ((u_xlat2.x * u_xlat2.x) - u_xlat0.w);
          u_xlat2 = (u_xlat2.yzzx * u_xlat2.xyzz);
          u_xlat4.x = dot(unity_SHC, u_xlat2);
          u_xlat4.y = dot(unity_SHC, u_xlat2);
          u_xlat4.z = dot(unity_SHC, u_xlat2);
          u_xlat2.xyz = ((unity_SHC.xyz * u_xlat0.www) + u_xlat4.xyz);
          out_v.texcoord3.xyz = ((u_xlat0.xyz * u_xlat3.xyz) + u_xlat2.xyz);
          u_xlat0.x = (u_xlat1.y * _ProjectionParams.x);
          u_xlat0.w = (u_xlat0.x * 0.5);
          u_xlat0.xz = (u_xlat1.xw * float2(0.5, 0.5));
          out_v.texcoord4.zw = u_xlat1.zw;
          out_v.texcoord4.xy = (u_xlat0.zz + u_xlat0.xw);
          out_v.texcoord6 = float4(0, 0, 0, 0);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float4 u_xlat0_d;
      float4 u_xlat1_d;
      float4 u_xlat2_d;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xyz = (in_f.texcoord2.xyz - unity_ShadowFadeCenterAndType.xyz);
          u_xlat0_d.x = length(u_xlat0_d.xyzx);
          u_xlat0_d.yzw = ((-in_f.texcoord2.xyz) + _WorldSpaceCameraPos.xyz);
          u_xlat1_d.x = conv_mxt4x4_1(unity_MatrixV).z;
          u_xlat1_d.y = conv_mxt4x4_2(unity_MatrixV).z;
          u_xlat1_d.z = conv_mxt4x4_3(unity_MatrixV).z;
          u_xlat0_d.y = dot(u_xlat0_d.yzwy, u_xlat1_d.xyzx);
          u_xlat0_d.x = ((-u_xlat0_d.y) + u_xlat0_d.x);
          u_xlat0_d.x = ((unity_ShadowFadeCenterAndType.w * u_xlat0_d.x) + u_xlat0_d.y);
          u_xlat0_d.x = saturate(((u_xlat0_d.x * unity_ShadowFadeCenterAndType.z) + unity_ShadowFadeCenterAndType.w));
          u_xlat0_d.yz = (in_f.texcoord4.xy / in_f.texcoord4.ww);
          u_xlat1_d = tex2D(_ShadowMapTexture, u_xlat0_d.yz);
          u_xlat0_d.x = saturate((u_xlat0_d.x + u_xlat1_d.x));
          u_xlat0_d.xyz = (u_xlat0_d.xxx * _LightColor0.xyz);
          u_xlat1_d = tex2D(_MainTex, in_f.texcoord.xy);
          u_xlat1_d.xyz = (u_xlat1_d.xyz * in_f.color.xyz);
          u_xlat0_d.xyz = (u_xlat0_d.xyz * u_xlat1_d.xyz);
          u_xlat2_d = UNITY_SAMPLE_TEX2D(unity_Lightmap, in_f.texcoord3.xy);
          u_xlat0_d.w = (u_xlat2_d.w * _LightColor0.x);
          u_xlat2_d.xyz = (u_xlat2_d.xyz * u_xlat0_d.www);
          u_xlat1_d.xyz = (u_xlat1_d.xyz * u_xlat2_d.xyz);
          u_xlat0_d.w = dot(in_f.texcoord1.xyzx, _WorldSpaceLightPos0.xyzx);
          u_xlat0_d.w = max(u_xlat0_d.w, 0);
          out_f.color.xyz = ((u_xlat0_d.xyz * u_xlat0_d.www) + u_xlat1_d.xyz);
          out_f.color.w = 1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 2, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "LIGHTMODE" = "FORWARDADD"
        "RenderType" = "Opaque"
      }
      ZClip Off
      ZWrite Off
      Blend One One
      ColorMask RGB
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      #define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)
      
      
      #define CODE_BLOCK_VERTEX
      uniform float4 _MainTex_ST;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4x4 unity_MatrixVP;
      uniform float4x4 unity_WorldToLight;
      uniform float4 _LightColor0;
      //uniform float4 _WorldSpaceLightPos0;
      //uniform float4 unity_OcclusionMaskSelector;
      //uniform float4x4 unity_ProbeVolumeWorldToObject;
      //uniform float4 unity_ProbeVolumeParams;
      //uniform float3 unity_ProbeVolumeSizeInv;
      //uniform float3 unity_ProbeVolumeMin;
      uniform sampler2D _MainTex;
      uniform sampler2D _LightTexture0;
      //uniform sampler3D unity_ProbeVolumeSH;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float4 tangent :TANGENT;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
          float4 color :COLOR;
      };
      
      struct OUT_Data_Vert
      {
          float4 vertex :SV_POSITION;
          float2 texcoord :TEXCOORD0;
          float2 texcoord3 :TEXCOORD3;
          float3 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
          float4 color :COLOR;
      };
      
      struct v2f
      {
          float4 vertex :SV_POSITION;
          float2 texcoord :TEXCOORD0;
          float2 texcoord3 :TEXCOORD3;
          float3 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
          float4 color :COLOR;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target;
      };
      
      float4 u_xlat0;
      float4 u_xlat1;
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          u_xlat0 = (in_v.vertex.yyyy * conv_mxt4x4_-2(unity_WorldToObject));
          u_xlat0 = ((conv_mxt4x4_-3(unity_WorldToObject) * in_v.vertex.xxxx) + u_xlat0);
          u_xlat0 = ((conv_mxt4x4_-1(unity_WorldToObject) * in_v.vertex.zzzz) + u_xlat0);
          u_xlat1 = (u_xlat0 + conv_mxt4x4_0(unity_WorldToObject));
          out_v.texcoord2.xyz = ((conv_mxt4x4_0(unity_WorldToObject).xyz * in_v.vertex.www) + u_xlat0.xyz);
          out_v.vertex = mul(unity_MatrixVP, u_xlat1);
          out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          out_v.texcoord3.xy = float2(0, 0);
          u_xlat0.x = dot(in_v.normal.xyzx, conv_mxt4x4_1(unity_WorldToObject).xyzx);
          u_xlat0.y = dot(in_v.normal.xyzx, conv_mxt4x4_2(unity_WorldToObject).xyzx);
          u_xlat0.z = dot(in_v.normal.xyzx, conv_mxt4x4_3(unity_WorldToObject).xyzx);
          u_xlat0.w = dot(u_xlat0.xyzx, u_xlat0.xyzx);
          u_xlat0.w = rsqrt(u_xlat0.w);
          out_v.texcoord1.xyz = (u_xlat0.www * u_xlat0.xyz);
          out_v.color = in_v.color;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float4 u_xlat0_d;
      float4 u_xlat1_d;
      float4 u_xlat2;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d = tex2D(_MainTex, in_f.texcoord.xy);
          u_xlat0_d.xyz = (u_xlat0_d.xyz * in_f.color.xyz);
          u_xlat1_d.xy = (in_f.texcoord2.yy * _LightColor0.xy);
          u_xlat1_d.xy = ((_LightColor0.xy * in_f.texcoord2.xx) + u_xlat1_d.xy);
          u_xlat1_d.xy = ((_LightColor0.xy * in_f.texcoord2.zz) + u_xlat1_d.xy);
          u_xlat1_d.xy = (u_xlat1_d.xy + _LightColor0.xy);
          u_xlat0_d.w = (unity_ProbeVolumeMin.x==1);
          if((u_xlat0_d.w!=0))
          {
              u_xlat0_d.w = (unity_ProbeVolumeMin.y==1);
              u_xlat2.xyz = (in_f.texcoord2.yyy * unity_ProbeVolumeMin.xyz);
              u_xlat2.xyz = ((unity_ProbeVolumeMin.xyz * in_f.texcoord2.xxx) + u_xlat2.xyz);
              u_xlat2.xyz = ((unity_ProbeVolumeMin.xyz * in_f.texcoord2.zzz) + u_xlat2.xyz);
              u_xlat2.xyz = (u_xlat2.xyz + unity_ProbeVolumeMin.xyz);
              u_xlat2.xyz = (u_xlat0_d.www)?(u_xlat2.xyz):(in_f.texcoord2.xyz);
              u_xlat2.xyz = (u_xlat2.xyz - unity_ProbeVolumeMin.xyz);
              u_xlat2.yzw = (u_xlat2.xyz * unity_ProbeVolumeMin.xyz);
              u_xlat0_d.w = ((u_xlat2.y * 0.25) + 0.75);
              u_xlat1_d.z = ((unity_ProbeVolumeMin.z * 0.5) + 0.75);
              u_xlat2.x = max(u_xlat0_d.w, u_xlat1_d.z);
              u_xlat2 = tex3D(unity_ProbeVolumeSH, u_xlat2.xz);
          }
          else
          {
              u_xlat2 = float4(1, 1, 1, 1);
          }
          u_xlat0_d.w = saturate(dot(u_xlat2, unity_OcclusionMaskSelector));
          u_xlat1_d = tex2D(_LightTexture0, u_xlat1_d.xy);
          u_xlat0_d.w = (u_xlat0_d.w * u_xlat1_d.w);
          u_xlat1_d.xyz = (u_xlat0_d.www * _LightColor0.xyz);
          u_xlat0_d.w = dot(in_f.texcoord1.xyzx, unity_OcclusionMaskSelector.xyzx);
          u_xlat0_d.w = max(u_xlat0_d.w, 0);
          u_xlat0_d.xyz = (u_xlat0_d.xyz * u_xlat1_d.xyz);
          out_f.color.xyz = (u_xlat0_d.www * u_xlat0_d.xyz);
          out_f.color.w = 1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 3, name: PREPASS
    {
      Name "PREPASS"
      Tags
      { 
        "LIGHTMODE" = "PREPASSBASE"
        "RenderType" = "Opaque"
      }
      ZClip Off
      ColorMask RGB
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      #define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4x4 unity_MatrixVP;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float4 tangent :TANGENT;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
          float4 color :COLOR;
      };
      
      struct OUT_Data_Vert
      {
          float4 vertex :SV_POSITION;
          float3 texcoord :TEXCOORD0;
          float3 texcoord1 :TEXCOORD1;
      };
      
      struct v2f
      {
          float4 vertex :SV_POSITION;
          float3 texcoord :TEXCOORD0;
          float3 texcoord1 :TEXCOORD1;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target;
      };
      
      float4 u_xlat0;
      float4 u_xlat1;
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          u_xlat0 = (in_v.vertex.yyyy * conv_mxt4x4_-2(unity_WorldToObject));
          u_xlat0 = ((conv_mxt4x4_-3(unity_WorldToObject) * in_v.vertex.xxxx) + u_xlat0);
          u_xlat0 = ((conv_mxt4x4_-1(unity_WorldToObject) * in_v.vertex.zzzz) + u_xlat0);
          u_xlat1 = (u_xlat0 + conv_mxt4x4_0(unity_WorldToObject));
          out_v.texcoord1.xyz = ((conv_mxt4x4_0(unity_WorldToObject).xyz * in_v.vertex.www) + u_xlat0.xyz);
          out_v.vertex = mul(unity_MatrixVP, u_xlat1);
          u_xlat0.x = dot(in_v.normal.xyzx, conv_mxt4x4_1(unity_WorldToObject).xyzx);
          u_xlat0.y = dot(in_v.normal.xyzx, conv_mxt4x4_2(unity_WorldToObject).xyzx);
          u_xlat0.z = dot(in_v.normal.xyzx, conv_mxt4x4_3(unity_WorldToObject).xyzx);
          u_xlat0.w = dot(u_xlat0.xyzx, u_xlat0.xyzx);
          u_xlat0.w = rsqrt(u_xlat0.w);
          out_v.texcoord.xyz = (u_xlat0.www * u_xlat0.xyz);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          out_f.color.xyz = ((in_f.texcoord.xyz * float3(0.5, 0.5, 0.5)) + float3(0.5, 0.5, 0.5));
          out_f.color.w = 0;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 4, name: PREPASS
    {
      Name "PREPASS"
      Tags
      { 
        "LIGHTMODE" = "PREPASSFINAL"
        "RenderType" = "Opaque"
      }
      ZClip Off
      ZWrite Off
      ColorMask RGB
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      #define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)
      
      
      #define CODE_BLOCK_VERTEX
      uniform float4 _MainTex_ST;
      //uniform float4 _ProjectionParams;
      // uniform float4 unity_ShadowFadeCenterAndType;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixV;
      //uniform float4x4 unity_MatrixVP;
      // uniform float4 unity_LightmapST;
      //uniform float4 unity_Lightmap_HDR;
      uniform sampler2D _MainTex;
      uniform sampler2D _LightBuffer;
      // uniform sampler2D unity_Lightmap;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float4 tangent :TANGENT;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
          float4 color :COLOR;
      };
      
      struct OUT_Data_Vert
      {
          float4 vertex :SV_POSITION;
          float2 texcoord :TEXCOORD0;
          float3 texcoord1 :TEXCOORD1;
          float4 color :COLOR;
          float4 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
          float4 texcoord4 :TEXCOORD4;
      };
      
      struct v2f
      {
          float4 vertex :SV_POSITION;
          float2 texcoord :TEXCOORD0;
          float3 texcoord1 :TEXCOORD1;
          float4 color :COLOR;
          float4 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
          float4 texcoord4 :TEXCOORD4;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target;
      };
      
      float4 u_xlat0;
      float4 u_xlat1;
      float4 u_xlat2;
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          u_xlat0 = (in_v.vertex.yyyy * conv_mxt4x4_1(unity_ObjectToWorld));
          u_xlat0 = ((conv_mxt4x4_0(unity_ObjectToWorld) * in_v.vertex.xxxx) + u_xlat0);
          u_xlat0 = ((conv_mxt4x4_2(unity_ObjectToWorld) * in_v.vertex.zzzz) + u_xlat0);
          u_xlat1 = (u_xlat0 + conv_mxt4x4_3(unity_ObjectToWorld));
          u_xlat0.xyz = ((conv_mxt4x4_3(unity_ObjectToWorld).xyz * in_v.vertex.www) + u_xlat0.xyz);
          u_xlat2 = mul(unity_MatrixVP, u_xlat1);
          out_v.vertex = u_xlat2;
          out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          out_v.texcoord1.xyz = u_xlat0.xyz;
          u_xlat0.xyz = (u_xlat0.xyz - unity_ShadowFadeCenterAndType.xyz);
          out_v.texcoord4.xyz = (u_xlat0.xyz * unity_ShadowFadeCenterAndType.www);
          out_v.color = in_v.color;
          u_xlat0.x = (u_xlat2.y * _ProjectionParams.x);
          u_xlat0.w = (u_xlat0.x * 0.5);
          u_xlat0.xz = (u_xlat2.xw * float2(0.5, 0.5));
          out_v.texcoord2.zw = u_xlat2.zw;
          out_v.texcoord2.xy = (u_xlat0.zz + u_xlat0.xw);
          out_v.texcoord3.xy = ((in_v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
          out_v.texcoord3.zw = float2(0, 0);
          u_xlat0.x = (u_xlat1.y * conv_mxt4x4_-7(unity_MatrixVP).z);
          u_xlat0.x = ((conv_mxt4x4_-8(unity_MatrixVP).z * u_xlat1.x) + u_xlat0.x);
          u_xlat0.x = ((conv_mxt4x4_-6(unity_MatrixVP).z * u_xlat1.z) + u_xlat0.x);
          u_xlat0.x = ((conv_mxt4x4_-5(unity_MatrixVP).z * u_xlat1.w) + u_xlat0.x);
          u_xlat0.y = ((-unity_ShadowFadeCenterAndType.w) + 1);
          out_v.texcoord4.w = (u_xlat0.y * (-u_xlat0.x));
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float4 u_xlat0_d;
      float4 u_xlat1_d;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xy = (in_f.texcoord2.xy / in_f.texcoord2.ww);
          u_xlat0_d = tex2D(_LightBuffer, u_xlat0_d.xy);
          u_xlat1_d = UNITY_SAMPLE_TEX2D(unity_Lightmap, in_f.texcoord3.xy);
          u_xlat0_d.w = (u_xlat1_d.w * unity_Lightmap_HDR.x);
          u_xlat0_d.xyz = ((u_xlat0_d.www * u_xlat1_d.xyz) + u_xlat0_d.xyz);
          u_xlat1_d = tex2D(_MainTex, in_f.texcoord.xy);
          u_xlat1_d.xyz = (u_xlat1_d.xyz * in_f.color.xyz);
          out_f.color.xyz = (u_xlat0_d.xyz * u_xlat1_d.xyz);
          out_f.color.w = 1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 5, name: DEFERRED
    {
      Name "DEFERRED"
      Tags
      { 
        "LIGHTMODE" = "DEFERRED"
        "RenderType" = "Opaque"
      }
      ZClip Off
      ColorMask RGB
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      #define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)
      
      
      #define CODE_BLOCK_VERTEX
      uniform float4 _MainTex_ST;
      // uniform float4 unity_ShadowFadeCenterAndType;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4x4 unity_MatrixV;
      //uniform float4x4 unity_MatrixVP;
      // uniform float4 unity_LightmapST;
      //uniform float4 unity_Lightmap_HDR;
      uniform sampler2D _MainTex;
      // uniform sampler2D unity_Lightmap;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float4 tangent :TANGENT;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
          float4 color :COLOR;
      };
      
      struct OUT_Data_Vert
      {
          float4 vertex :SV_POSITION;
          float2 texcoord :TEXCOORD0;
          float3 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
          float4 color :COLOR;
          float4 texcoord3 :TEXCOORD3;
          float4 texcoord4 :TEXCOORD4;
      };
      
      struct v2f
      {
          float4 vertex :SV_POSITION;
          float2 texcoord :TEXCOORD0;
          float3 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
          float4 color :COLOR;
          float4 texcoord3 :TEXCOORD3;
          float4 texcoord4 :TEXCOORD4;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target;
          float4 color1 :SV_Target1;
          float4 color2 :SV_Target2;
          float4 color3 :SV_Target3;
      };
      
      float4 u_xlat0;
      float4 u_xlat1;
      float4 u_xlat2;
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          u_xlat0 = (in_v.vertex.yyyy * conv_mxt4x4_-2(unity_WorldToObject));
          u_xlat0 = ((conv_mxt4x4_-3(unity_WorldToObject) * in_v.vertex.xxxx) + u_xlat0);
          u_xlat0 = ((conv_mxt4x4_-1(unity_WorldToObject) * in_v.vertex.zzzz) + u_xlat0);
          u_xlat1 = (u_xlat0 + conv_mxt4x4_0(unity_WorldToObject));
          u_xlat0.xyz = ((conv_mxt4x4_0(unity_WorldToObject).xyz * in_v.vertex.www) + u_xlat0.xyz);
          out_v.vertex = mul(unity_MatrixVP, u_xlat1);
          out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          u_xlat2.x = dot(in_v.normal.xyzx, conv_mxt4x4_1(unity_WorldToObject).xyzx);
          u_xlat2.y = dot(in_v.normal.xyzx, conv_mxt4x4_2(unity_WorldToObject).xyzx);
          u_xlat2.z = dot(in_v.normal.xyzx, conv_mxt4x4_3(unity_WorldToObject).xyzx);
          u_xlat0.w = dot(u_xlat2.xyzx, u_xlat2.xyzx);
          u_xlat0.w = rsqrt(u_xlat0.w);
          out_v.texcoord1.xyz = (u_xlat0.www * u_xlat2.xyz);
          out_v.texcoord2.xyz = u_xlat0.xyz;
          u_xlat0.xyz = (u_xlat0.xyz - unity_ShadowFadeCenterAndType.xyz);
          out_v.texcoord4.xyz = (u_xlat0.xyz * unity_ShadowFadeCenterAndType.www);
          out_v.color = in_v.color;
          out_v.texcoord3.xy = ((in_v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
          out_v.texcoord3.zw = float2(0, 0);
          u_xlat0.x = (u_xlat1.y * conv_mxt4x4_-7(unity_MatrixVP).z);
          u_xlat0.x = ((conv_mxt4x4_-8(unity_MatrixVP).z * u_xlat1.x) + u_xlat0.x);
          u_xlat0.x = ((conv_mxt4x4_-6(unity_MatrixVP).z * u_xlat1.z) + u_xlat0.x);
          u_xlat0.x = ((conv_mxt4x4_-5(unity_MatrixVP).z * u_xlat1.w) + u_xlat0.x);
          u_xlat0.y = ((-unity_ShadowFadeCenterAndType.w) + 1);
          out_v.texcoord4.w = (u_xlat0.y * (-u_xlat0.x));
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float4 u_xlat0_d;
      float4 u_xlat1_d;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d = tex2D(_MainTex, in_f.texcoord.xy);
          u_xlat0_d.xyz = (u_xlat0_d.xyz * in_f.color.xyz);
          out_f.color.xyz = u_xlat0_d.xyz;
          out_f.color.w = 1;
          out_f.color1 = float4(0, 0, 0, 0);
          out_f.color2.xyz = ((in_f.texcoord1.xyz * float3(0.5, 0.5, 0.5)) + float3(0.5, 0.5, 0.5));
          out_f.color2.w = 1;
          u_xlat1_d = UNITY_SAMPLE_TEX2D(unity_Lightmap, in_f.texcoord3.xy);
          u_xlat0_d.w = (u_xlat1_d.w * unity_Lightmap_HDR.x);
          u_xlat1_d.xyz = (u_xlat1_d.xyz * u_xlat0_d.www);
          out_f.color3.xyz = (u_xlat0_d.xyz * u_xlat1_d.xyz);
          out_f.color3.w = 1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 6, name: META
    {
      Name "META"
      Tags
      { 
        "LIGHTMODE" = "META"
        "RenderType" = "Opaque"
      }
      ZClip Off
      Cull Off
      ColorMask RGB
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      #define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)
      
      
      #define CODE_BLOCK_VERTEX
      uniform float4 _MainTex_ST;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixVP;
      // uniform float4 unity_LightmapST;
      // uniform float4 unity_DynamicLightmapST;
      uniform float4 unity_MetaVertexControl;
      uniform float unity_OneOverOutputBoost;
      uniform float unity_MaxOutputValue;
      uniform float4 unity_MetaFragmentControl;
      uniform sampler2D _MainTex;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float4 tangent :TANGENT;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
          float4 color :COLOR;
      };
      
      struct OUT_Data_Vert
      {
          float4 vertex :SV_POSITION;
          float2 texcoord :TEXCOORD0;
          float3 texcoord1 :TEXCOORD1;
          float4 color :COLOR;
      };
      
      struct v2f
      {
          float4 vertex :SV_POSITION;
          float2 texcoord :TEXCOORD0;
          float3 texcoord1 :TEXCOORD1;
          float4 color :COLOR;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target;
      };
      
      float4 u_xlat0;
      float4 u_xlat1;
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          u_xlat0.x = (0<in_v.vertex.z);
          u_xlat0.z = (uint(u_xlat0.x) & uint(953267991));
          u_xlat0.xy = ((in_v.texcoord1.xy * unity_DynamicLightmapST.xy) + unity_DynamicLightmapST.zw);
          u_xlat0.xyz = (unity_MetaVertexControl.xxx)?(u_xlat0.xyz):(in_v.vertex.xyz);
          u_xlat0.w = (0<u_xlat0.z);
          u_xlat1.z = (uint(u_xlat0.w) & uint(953267991));
          u_xlat1.xy = ((in_v.texcoord2.xy * unity_DynamicLightmapST.xy) + unity_DynamicLightmapST.zw);
          u_xlat0.xyz = (unity_MetaVertexControl.yyy)?(u_xlat1.xyz):(u_xlat0.xyz);
          out_v.vertex = UnityObjectToClipPos(u_xlat0);
          out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          u_xlat0.xyz = (in_v.vertex.yyy * conv_mxt4x4_1(unity_ObjectToWorld).xyz);
          u_xlat0.xyz = ((conv_mxt4x4_0(unity_ObjectToWorld).xyz * in_v.vertex.xxx) + u_xlat0.xyz);
          u_xlat0.xyz = ((conv_mxt4x4_2(unity_ObjectToWorld).xyz * in_v.vertex.zzz) + u_xlat0.xyz);
          out_v.texcoord1.xyz = ((conv_mxt4x4_3(unity_ObjectToWorld).xyz * in_v.vertex.www) + u_xlat0.xyz);
          out_v.color = in_v.color;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float4 u_xlat0_d;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d = tex2D(_MainTex, in_f.texcoord.xy);
          u_xlat0_d.xyz = (u_xlat0_d.xyz * in_f.color.xyz);
          u_xlat0_d.xyz = log(u_xlat0_d.xyz);
          u_xlat0_d.w = saturate(unity_MaxOutputValue.x);
          u_xlat0_d.xyz = (u_xlat0_d.xyz * u_xlat0_d.www);
          u_xlat0_d.xyz = exp(u_xlat0_d.xyzx);
          u_xlat0_d.xyz = min(u_xlat0_d.xyz, unity_MaxOutputValue.yyy);
          u_xlat0_d.w = 1;
          u_xlat0_d = (unity_MetaFragmentControl.xxxx)?(u_xlat0_d):(float4(0, 0, 0, 0));
          out_f.color = (unity_MetaFragmentControl.yyyy)?(float4(0, 0, 0, 0.023529)):(u_xlat0_d);
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Mobile/VertexLit"
}
