//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Text;
using System.Linq;

namespace JBooth.MicroSplat
{
   public class UnityHDRenderLoopAdapter : IRenderLoopAdapter
   {
      public string GetDisplayName()
      {
         return "HDRP";
      }

      public string GetShaderExtension() { return "shader"; }

      public string GetRenderLoopKeyword()
      {
         return "_MSRENDERLOOP_UNITYHD";
      }

      public string GetVersion()
      {
         return "3.7";
      }

      MicroSplatGenerator gen;

      TextAsset templateHDRP;
      TextAsset templateInclude;
      TextAsset templatePassDepthOnly;
      TextAsset templatePassForward;
      TextAsset templatePassGBuffer;
      TextAsset templatePassMeta;
      TextAsset templatePassSceneSelection;
      TextAsset templatePassShadow;
      TextAsset templateShared;
      TextAsset templateHDRPShared;
      TextAsset templateVert;
      TextAsset templateChain;
      TextAsset templateShaderDesc;
      TextAsset templateTess;


      public void Init(string[] paths)
      {
         if (gen == null)
         {
            gen = new MicroSplatGenerator();
         }
         gen.Init(paths);

         for (int i = 0; i < paths.Length; ++i)
         {
            string p = paths[i];
            if (p.EndsWith("microsplat_template_hdrp2019_template.txt"))
            {
               templateHDRP = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            else if (p.EndsWith("microsplat_func_tess2.txt"))
            {
               templateTess = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            else if (p.EndsWith("microsplat_template_shaderdesc.txt"))
            {
               templateShaderDesc = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            else if (p.EndsWith("microsplat_template_chain.txt"))
            {
               templateChain = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            else if (p.EndsWith("microsplat_template_hdrp2019_passforward.txt"))
            {
               templatePassForward = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            else if (p.EndsWith("microsplat_template_hdrp2019_passgbuffer.txt"))
            {
               templatePassGBuffer = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            else if (p.EndsWith("microsplat_template_hdrp2019_passdepthonly.txt"))
            {
               templatePassDepthOnly = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            else if (p.EndsWith("microsplat_template_hdrp2019_passmeta.txt"))
            {
               templatePassMeta = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            else if (p.EndsWith("microsplat_template_hdrp2019_passsceneelection.txt"))
            {
               templatePassSceneSelection = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            else if (p.EndsWith("microsplat_template_hdrp2019_passshadow.txt"))
            {
               templatePassShadow = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            else if (p.EndsWith("microsplat_template_hdrp2019_shared.txt"))
            {
               templateHDRPShared = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            else if (p.EndsWith("microsplat_template_shared.txt"))
            {
               templateShared = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            else if (p.EndsWith("microsplat_template_hdrp2019_vert.txt"))
            {
               templateVert = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            else if (p.EndsWith("microsplat_template_hdrp2019_include.txt"))
            {
               templateInclude = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }

         }
         if (templateHDRP == null || templateInclude == null || templateShared == null || templateVert == null
            || templateHDRPShared == null || templatePassGBuffer == null)
         {
            Debug.LogError ("HDRP Template files not found, will not be able to compile shaders");
         }
      }

      StringBuilder BuildTemplate(Blocks blocks, string fallbackOverride = null)
      {

         var template = new StringBuilder(100000);
         template.Append(templateHDRP.text);

         var passforward = new StringBuilder(templatePassForward.text);
         var passGBuffer = new StringBuilder(templatePassGBuffer.text);
         var passShadow = new StringBuilder(templatePassShadow.text);
         var passMeta = new StringBuilder(templatePassMeta.text);
         var passSelection = new StringBuilder(templatePassSceneSelection.text);
         var passDepthOnly = new StringBuilder(templatePassDepthOnly.text);
         var vert = new StringBuilder(templateVert.text);
         var hdrpShared = new StringBuilder(templateHDRPShared.text);
         var hdrpInclude = new StringBuilder(templateInclude.text);

         if (blocks.options.Contains("Unlit"))
         {
            passGBuffer.Length = 0;
         }

         if (blocks.options.Contains("Alpha \"Blend\""))
         {
            //passGBuffer.Length = 0;
            passDepthOnly.Length = 0;
            passShadow.Length = 0;
            passforward = passforward.Replace("%FORWARDBASEBLEND%", "Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha\nCull Back\n ZTest LEqual\nZWrite Off");
            blocks.defines += "\n#define _BLENDMODE_ALPHA 1\n#define _SURFACE_TYPE_TRANSPARENT 1\n";
         }
         else
         {
            passforward = passforward.Replace("%FORWARDBASEBLEND%", "");
         }

         template = template.Replace("%PASSGBUFFER%", passGBuffer.ToString());
         template = template.Replace("%PASSMETA%", passMeta.ToString());
         template = template.Replace("%PASSFORWARD%", passforward.ToString());
         template = template.Replace("%PASSSCENESELECT%", passSelection.ToString());
         template = template.Replace("%PASSDEPTHONLY%", passDepthOnly.ToString());
         template = template.Replace("%PASSSHADOW%", passShadow.ToString());
         template = template.Replace("%HDRPSHARED%", hdrpShared.ToString());
         template = template.Replace("%HDRPINCLUDE%", hdrpInclude.ToString());
         template = template.Replace("%VERT%", vert.ToString());
         template = template.Replace("%SHADERDESC%", templateShaderDesc.text + templateChain.text);



         StringBuilder header = new StringBuilder();
         header.AppendLine("////////////////////////////////////////");
         header.AppendLine("// MicroSplat");
         header.AppendLine("// Copyright (c) Jason Booth");
         header.AppendLine("//");
         header.AppendLine("// Auto-generated shader code, don't hand edit!");
         header.AppendLine("//");
         header.AppendLine("//   Unity Version: " + Application.unityVersion);
         header.AppendLine("//   Render Pipeline: HDRP2019");
         header.AppendLine("//   Platform: " + Application.platform);
         header.AppendLine("////////////////////////////////////////\n\n");

         template = template.Insert(0, header);

         string tags = SurfaceShaderRenderLoopAdapter.GetTags(blocks.options);
         if (tags != null)
         {
            tags = tags.Replace("\"RenderType\"", "\"RenderPipeline\" = \"HDRenderPipeline\" \"RenderType\"");
            tags = tags.Replace("Opaque", "HDLitShader");
            tags = tags.Replace("Geometry+100", "Geometry+255");
         }
         else
         {
            tags = "\"RenderPipeline\" = \"HDRenderPipeline\" \"RenderType\" = \"HDLitShader\" \"Queue\" = \"Geometry+255\"";
         }
         if (blocks.options.Contains("Alpha"))
         {
            tags = tags.Replace("Geometry+255", "Transparent");
         }

         template = template.Replace("%TAGS%", tags);



         template = template.Replace("%TEMPLATE_SHARED%", templateShared.text);
         template = SurfaceShaderRenderLoopAdapter.ReplaceOrRemove(template, "%CUSTOMEDITOR%", "CustomEditor", SurfaceShaderRenderLoopAdapter.GetOption(blocks.options, "CustomEditor"));
         if (fallbackOverride != null)
         {
            template = template.Replace("%FALLBACK%", "Fallback \"" + fallbackOverride + "\"");
            template = SurfaceShaderRenderLoopAdapter.ReplaceOrRemove(template, "%DEPENDENCY%", "Dependency \"BaseMapShader\" = ", fallbackOverride);
         }
         else
         {
            template = SurfaceShaderRenderLoopAdapter.ReplaceOrRemove(template, "%FALLBACK%", "Fallback", "");
            template = SurfaceShaderRenderLoopAdapter.ReplaceOrRemove(template, "%DEPENDENCY%", "Dependency", "");
         }
         return template;
      }

      public StringBuilder WriteShader(string[] features,
            MicroSplatShaderGUI.MicroSplatCompiler compiler,
            MicroSplatShaderGUI.MicroSplatCompiler.AuxShader auxShader,
            string name,
            string baseName)
      {
         StringBuilder defines = new StringBuilder();
         var blocks = gen.GetShaderBlocks(features, compiler, auxShader);

         var shader = BuildTemplate(blocks, baseName);
         defines.AppendLine(blocks.defines);
         defines.AppendLine("\n   #define _HDRP 1");


         string shaderTarget = "4.6";
         if (features.Contains("_FORCEMODEL50"))
         {
            shaderTarget = "5.0";
         }

         if (features.Contains("_TESSDISTANCE"))
         {
            defines.AppendLine("\n      #define _TESSELLATION_ON 1");
            shader = shader.Replace("%TESSELLATION%", templateTess.text);
            shader = shader.Replace("%PRAGMAS%", "   #pragma hull Hull\n   #pragma domain Domain\n   #pragma vertex TessVert\n   #pragma fragment Frag\n   #pragma require tesshw\n");
         }
         else
         {
            shader = shader.Replace("%PRAGMAS%", "   #pragma vertex Vert\n   #pragma fragment Frag");
            shader = shader.Replace("%TESSELLATION%", "");
         }
         shader = shader.Replace("%SHADERTARGET%", shaderTarget);
         if (features.Contains("_USESPECULARWORKFLOW"))
         {
            defines.AppendLine("\n#define _USESPECULAR 1");
            defines.AppendLine("#define _MATERIAL_FEATURE_SPECULAR_COLOR 1");
         }

         defines.AppendLine();

         shader = shader.Replace("%SHADERNAME%", name);
         shader = shader.Replace("%PROPERTIES%", blocks.properties);
         shader = shader.Replace("%CODE%", blocks.code);
         shader = shader.Replace("%DEFINES%", defines.ToString());
         shader = shader.Replace("%CBUFFER%", blocks.cbuffer);
         string codeNoComments = blocks.code.StripComments();

         shader = SurfaceShaderRenderLoopAdapter.Strip(codeNoComments, shader);

         // standard pipeline stuff
         shader = shader.Replace("fixed", "half");
         shader = shader.Replace("unity_ObjectToWorld", "GetObjectToWorldMatrix()");
         shader = shader.Replace("unity_WorldToObject", "GetWorldToObjectMatrix()");

         //shader = shader.Replace("UNITY_MATRIX_M", "GetObjectToWorldMatrix()");
         //shader = shader.Replace("UNITY_MATRIX_I_M", "GetWorldToObjectMatrix()");
         //shader = shader.Replace("UNITY_MATRIX_VP", "GetWorldToHClipMatrix()");
         //shader = shader.Replace("UNITY_MATRIX_V", "GetWorldToViewMatrix()");
         //shader = shader.Replace("UNITY_MATRIX_P", "GetViewToHClipMatrix()");

         return shader;
      }

      
   }
}
