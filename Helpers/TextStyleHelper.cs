using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Fluent;

namespace ZC_Informes.Helpers
{
    internal class TextStyleHelper
    {
        public static readonly Dictionary<string, Action<TextSpanDescriptor>> StyleMap = new()
        {
            { "Regular", span => { /* No action needed for regular style */ } },
            { "SemiBold", span => span.SemiBold() },
            { "Bold", span => span.Bold() },
            { "Italic", span => span.Italic() },
        };
    }
}
