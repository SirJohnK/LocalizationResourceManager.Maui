using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationResourceManager.Maui
{
    public interface ILocalizationResourceManager
    {
        CultureInfo DefaultCulture { get; }

        CultureInfo CurrentCulture { get; set; }

        string GetValue(string text);
    }
}