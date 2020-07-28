using SuperMemoAssistant.Plugins.PopupWindow.Interop;
using SuperMemoAssistant.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.PopupWiki.Service
{
  public partial class ContentService : ContentServiceBase, IBrowserContentProvider
  {

    /// <summary>
    /// Base URL for search. {0} is a placeholder for the language.
    /// </summary>
    private const string searchUrl = @"https://{0}.wiktionary.org/w/api.php?action=opensearch";
    private PopupWikiCfg Config => Svc<PopupWikiPlugin>.Plugin.Config;
    private int NumSearchResults => Config.NumSearchResults;
    private string[] Languages => Config.WikiLanguages?.Split(',');

  }
}
