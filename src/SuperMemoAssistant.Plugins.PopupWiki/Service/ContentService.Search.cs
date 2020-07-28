using Anotar.Serilog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuperMemoAssistant.Plugins.PopupWindow.Interop;
using SuperMemoAssistant.Services;
using SuperMemoAssistant.Sys.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SuperMemoAssistant.Plugins.PopupWiki.Service
{

  //
  // SEARCH METHODS

  public partial class ContentService 
  {


    [LogToErrorOnException]
    public RemoteTask<BrowserContent> Search(string searchTerm)
    {

      if (searchTerm.IsNullOrEmpty())
        return null;

      return GetSearchResultsAsync(searchTerm);

    }

    public async Task<BrowserContent> GetSearchResultsAsync(string searchTerm)
    {
      string searchResults = null;

      try
      {

        foreach (string language in Languages)
        {

          string baseurl = string.Format(searchUrl, language);

          // TODO: Experiment with the different search types eg. fuzzy
          // possibly allow the user to change in the config.
          string options = $"&search={HttpUtility.UrlEncode(searchTerm)}" +
                           $"&limit={NumSearchResults}" +
                           $"&namespace=0" +
                           $"&format=json";

          string fullurl = baseurl + options;

          try
          {
            string response = await GetAsync(fullurl);
            if (response.IsNullOrEmpty())
              continue;
            dynamic search = JsonConvert.DeserializeObject(response);
            JArray searchResultTitles = search[1];
            JArray searchResultUrls = search[3];

            // Returns a block of results for a language
            // TODO: Use a mustache template?
            if (searchResultTitles.Count > 0 && searchResultUrls.Count > 0)
            {
              searchResults += $"<h3>{language} search results</h3>";
              searchResults += "<ul>";

              List<string> htmlLinkNodes = new List<string>();

              // TODO: Can these be written more clearly?
              htmlLinkNodes = searchResultTitles
                                         .Zip(searchResultUrls,
                                              (title, link) =>
                                              $"<a href=\"{UrlUtils.ConvDesktopWiktionaryToMob(link.ToString())}\">{title}</a>")
                                         .ToList();

              // Zip and iterate over the searchTitles and searchUrls arrays
              // to pair them and add to the searchResults.
              if (htmlLinkNodes != null && htmlLinkNodes.Count > 0)
              {
                foreach (var linkNode in htmlLinkNodes)
                {
                  searchResults += $"<li>{linkNode}</li>";
                }
                searchResults += "</ul>";
              }
            }
          }
          catch (UriFormatException e)
          {
            LogTo.Error($"UriFormatException on {fullurl}");
          }
        }

        return new BrowserContent(searchResults, null, false, ContentType.Search);

      }
      catch (RemotingException) { }

      return null;
    }

  }

}
