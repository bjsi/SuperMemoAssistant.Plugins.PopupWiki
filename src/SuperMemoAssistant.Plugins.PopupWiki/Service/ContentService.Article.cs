using Anotar.Serilog;
using SuperMemoAssistant.Interop.SuperMemo.Elements.Builders;
using SuperMemoAssistant.Plugins.PopupWindow.Interop;
using SuperMemoAssistant.Sys.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.PopupWiki.Service
{
  
  //
  // ARTICLE FETCHING METHODS

  public partial class ContentService
  {

    [LogToErrorOnException]
    public RemoteTask<BrowserContent> FetchArticleHtml(string url)
    {

      return UrlUtils.IsDesktopWikipediaUrl(url)
        ? GetArticleHtmlAsync(url) 
        : null;

    }

    public async Task<BrowserContent> GetArticleHtmlAsync(string url)
    {

      var uri = new Uri(url);
      var language = uri.Host.Split('.')[0];
      var articleTitle = uri.Segments.Last();

      try
      {

        string html = null;

        if (articleTitle.IsNullOrEmpty())
          return null;

        if (language.IsNullOrEmpty())
          return null;

        string articleUrlTitle = UrlUtils.ParseTitle(articleTitle);
        string articleUrl = $"https://{language}.wikipedia.org/api/rest_v1/page/mobile-html/{articleUrlTitle}";
        string response = await GetAsync(articleTitle);

        if (response.IsNullOrEmpty())
          return null;

        html = HtmlEx.FilterMobileWikipediaArticle(response, language);

        var refs = new References();
        refs.Title = articleTitle;
        refs.Link = url;
        refs.Source = "Wikipedia";

        return new BrowserContent(html, refs, true, ContentType.Article);
      }
      catch (RemotingException) { }

      return null;

    }
  }
}
