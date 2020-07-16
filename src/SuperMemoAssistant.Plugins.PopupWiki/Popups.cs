using Anotar.Serilog;
using Forge.Forms;
using SuperMemoAssistant.Extensions;
using SuperMemoAssistant.Plugins.PopupWiki.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SuperMemoAssistant.Plugins.PopupWiki
{
  public static class Popups
  {

    /// <summary>
    /// Get the search query from the user.
    /// </summary>
    /// <returns></returns>
    public static string GetSearchQuery(string Title)
    {
      string selText = ContentUtils.GetSMSelText();
      return OpenEditSearchTermDialog(Title, "Search:", selText);
    }

    /// <summary>
    /// Opens a window allowing the user the edit the search term.
    /// Preferred over Forge.Forms solution because this always opens as topmost.
    /// </summary>
    /// <param name="Title"></param>
    /// <param name="Message"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    public static string OpenEditSearchTermDialog(string Title, string Message, string query = "")
    {

      var editedTerm = Application.Current.Dispatcher.Invoke(() =>
      {
        string res = null;
        var wdw = new EditSearchTermWdw(query, Title, Message);
        wdw.ShowDialog();

        if (wdw.Confirmed)
        {
          res = wdw.Value;
        }

        return res;
      });

      return editedTerm;
    }

    /// <summary>
    /// Opens an alert window to inform the user about invalid input.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="title"></param>
    public static void OpenAlertWdw(string message, string title)
    {
      if (!string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(title))
      {
        Application.Current.Dispatcher.Invoke(() =>
        {
          Show.Window().For(new Alert(message, title));
        });
      }
    }
  }
}
