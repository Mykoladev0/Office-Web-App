using System;
using System.IO;
using PdfSharpCore.Fonts;

namespace BullITPDF
{
    //This implementation is obviously not very good --> Though it should be enough for everyone to implement their own.
    public class FontResolver : IFontResolver
    {
        public string DefaultFontName => "Calibri";

        public byte[] GetFont(string faceName)
        {
            using (var stream = EmbeddedResource.GetResource("Fonts.Calibri.ttf"))
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    ms.Position = 0;
                    return ms.ToArray();
                }
            }
            // using (var ms = new MemoryStream())
            // {
            //     using (var fs = File.Open(faceName, FileMode.Open))
            //     {
            //         fs.CopyTo(ms);
            //         ms.Position = 0;
            //         return ms.ToArray();
            //     }
            // }
        }
        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (familyName == "Calibri")
            {
                return new FontResolverInfo("Calibri");
            }
            // if (familyName.Equals("OpenSans", StringComparison.CurrentCultureIgnoreCase))
            // {
            //     if (isBold && isItalic)
            //     {
            //         return new FontResolverInfo("OpenSans-BoldItalic.ttf");
            //     }
            //     else if (isBold)
            //     {
            //         return new FontResolverInfo("OpenSans-Bold.ttf");
            //     }
            //     else if (isItalic)
            //     {
            //         return new FontResolverInfo("OpenSans-Italic.ttf");
            //     }
            //     else
            //     {
            //         return new FontResolverInfo("OpenSans-Regular.ttf");
            //     }
            // }
            return null;
        }
    }
}