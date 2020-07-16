using mshtml;
using SuperMemoAssistant.Extensions;
using SuperMemoAssistant.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.PopupWiki
{

  public static class ContentUtils
  {
    /// <summary>
    /// Gets the currently selected text in SM without punc / whitespace
    /// </summary>
    /// <returns>
    /// selected text or null.
    /// </returns>
    public static string GetSMSelText()
    {
      var ctrlGroup = Svc.SM.UI.ElementWdw.ControlGroup;
      var htmlCtrl = ctrlGroup?.FocusedControl?.AsHtml();
      var htmlDoc = htmlCtrl?.GetDocument();
      var sel = htmlDoc?.selection;

      if (!(sel?.createRange() is IHTMLTxtRange textSel))
        return null;

      string cleanSelText = null;

      if (!string.IsNullOrEmpty(textSel.text))
      {
        cleanSelText = Regex.Replace(textSel.text, @"[\.|\?|\,|\!]", "");
        cleanSelText = Regex.Replace(cleanSelText, @"[\n|\t|\r]", " ");
        cleanSelText = cleanSelText.Trim();
      }
      return cleanSelText;
    }
  }
}
