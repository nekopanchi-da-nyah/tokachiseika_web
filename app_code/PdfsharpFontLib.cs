using System;
using System.IO;
using PdfSharp;
using PdfSharp.Fonts;

namespace PdfFontLib
{
   public class Resolver : IFontResolver
   {
      public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
      {
         try
         {
            var path = @"D:\WEB開発\とかち製菓DEMO\web\font\GenShinGothic-Medium.ttf";
            return new FontResolverInfo( path );
         }
         catch
         {
            return null;
         }
      }
      
      public byte[] GetFont(string faceName)
      {
         using( var ms = new MemoryStream() )
         {
            using( var fs = File.Open(faceName, FileMode.Open) )
            {
               fs.CopyTo(ms);
               ms.Position = 0;
               return ms.ToArray();
            }
         }
      }
   }
}