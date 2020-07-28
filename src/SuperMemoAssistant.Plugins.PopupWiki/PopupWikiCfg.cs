using Forge.Forms.Annotations;
using Newtonsoft.Json;
using SuperMemoAssistant.Interop.SuperMemo.Content.Models;
using SuperMemoAssistant.Plugins.PopupWiki.Models;
using SuperMemoAssistant.Services.UI.Configuration;
using SuperMemoAssistant.Sys.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.PopupWiki
{
  [Form(Mode = DefaultFields.None)]
  [Title("Dictionary Settings",
   IsVisible = "{Env DialogHostContext}")]
  [DialogAction("cancel",
          "Cancel",
          IsCancel = true)]
  [DialogAction("save",
          "Save",
          IsDefault = true,
          Validates = true)]
  public class PopupWikiCfg : CfgBase<PopupWikiCfg>, INotifyPropertyChangedEx
  {

    [Title("Popup Wiki Plugin")]

    [Heading("By Jamesb | Experimental Learning")]

    [Heading("Features:")]
    [Text(@"- IContentProvider for Wikipedia integration with PopupBrowser.")]

    [Heading("Language Settings")]
    [Field(Name = "Wikipedia Languages (comma-separated, first is the main language eg. en,fi,fr)")]
    public string WikiLanguages { get; set; } = "en";
    public string MainLanguage => WikiLanguages.Split(',')[0];

    [Field(Name = "")]
    public int MaxSearchResults { get; set; } = 15

    // Could add a PopupWiki lite version for slower internet speeds
    //[Field(Name = "Load minimalist HTML for speed?")]
    //public bool MinimalistHtml { get; set; } = false;

    [Heading("Search Settings")]
    [Field(Name = "Search results per language?")]
    public int NumSearchResults { get; set; } = 15;

    [Heading("Extract Settings")]
    [Field(Name = "Default SM Extract Priority (%)")]
    [Value(Must.BeGreaterThanOrEqualTo,
           0,
           StrictValidation = true)]
    [Value(Must.BeLessThanOrEqualTo,
           100,
           StrictValidation = true)]
    public double SMExtractPriority { get; set; } = 15;

    [Field(Name = "Default Image Stretch Type")]
    [SelectFrom(typeof(ImageStretchMode),
                SelectionType = SelectionType.RadioButtonsInline)]
    public ImageStretchMode ImageStretchType { get; set; } = ImageStretchMode.Proportional;

    // Similar to the SMA PDF option for extracts containing only images.
    [Field(Name = "Add HTML component to extracts containing only images?")]
    public bool ImageExtractAddHtml { get; set; } = false;

    [Field(Name = "Extract as child of current element or into concept hook?")]
    [SelectFrom(typeof(ExtractMode),
                SelectionType = SelectionType.RadioButtonsInline)]
    public ExtractMode ExtractType { get; set; } = ExtractMode.Child;

    [JsonIgnore]
    public bool IsChanged { get; set; }

    public override string ToString()
    {
      return "PopupWiki";
    }

    public event PropertyChangedEventHandler PropertyChanged;
  }
}
