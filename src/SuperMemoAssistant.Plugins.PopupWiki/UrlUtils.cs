using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.PopupWiki
{
  public static class UrlUtils
  {
    public const string DesktopWikiUrlRegex = @"^https?\:\/\/([\w\.]+)wikipedia.org\/wiki\/([\w]+)+";
    public const string MobileWikiUrlRegex = @"^https?\:\/\/([\w\.]+)\.m\.wikipedia.org\/wiki\/([\w]+)+";

    // Example url: https://en.m.wiktionary.org/wiki/Hello_World
    public static bool IsMobileWiktionaryUrl(string url)
    {

      if (string.IsNullOrEmpty(url))
        return false;

      if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
        return false;

      Uri uri = new Uri(url);
      string[] split = uri.Host.Split('.');
      if (split.Length != 4)
        return false;

      if (split[1] == "m" && split[2] == "wiktionary")
        return true;

      return false;
    }


    // eg. "https://wikimedia.org/api/rest_v1/media/math/render/svg/786849c765da7a84dbc3cce43e96aad58a5868dc"
    public static bool IsMathImgUrl(this string url)
    {

      const string mathImgUrl = "https://wikimedia.org/api/rest_v1/media/math/render";
      return string.IsNullOrEmpty(url)
        ? false
        : url.Contains(mathImgUrl);

    }

    public static string ConvMathImgSvgToPng(string url) => url?.Replace("svg", "png");

    // Example url: https://en.wikipedia.org/wiki/Hello_World
    public static bool IsDesktopWikipediaUrl(string url)
    {
      if (!string.IsNullOrEmpty(url))
      {
        if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
          Uri uri = new Uri(url);
          // Host == en.wikipedia.org
          string[] splitUri = uri.Host.Split('.');

          if (splitUri.Length == 3)
          {
            if (splitUri[1] == "wikipedia")
            {
              return true;
            }
          }
        }
      }
      return false;
    }

    // Example url: https://en.wiktionary.org/wiki/Hello_World
    public static bool IsDesktopWiktionaryUrl(string url)
    {
      if (!string.IsNullOrEmpty(url))
      {
        // Should fail for relative urls like /wiki/Hello_World
        if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
          Uri uri = new Uri(url);
          // Host == en.wiktionary.org
          string[] splitUri = uri.Host.Split('.');

          // Should fail for mobile wiktionary (length 4)
          if (splitUri.Length == 3)
          {
            // Should fail for wikipedia links
            if (splitUri[1] == "wiktionary")
            {
              return true;
            }
          }
        }
      }
      return false;
    }

    // Example url: https://en.m.wikipedia.org/wiki/Hello_World
    public static bool IsMobileWikipediaUrl(string url)
    {
      if (!string.IsNullOrEmpty(url))
      {
        // Should fail for relative links like /wiki/Hello_World
        if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
          Uri uri = new Uri(url);
          // Host == en.m.wikipedia.org
          string[] splitUri = uri.Host.Split('.');

          // Should fail for destop links (length 3)
          if (splitUri.Length == 4)
          {
            // Should fail for desktop links
            if (splitUri[1] == "m")
            {
              // Should fail for wiktionary links
              if (splitUri[2] == "wikipedia")
              {
                return true;
              }
            }
          }
        }
      }
      return false;
    }

    public static string ParseTitle(string articleTitle)
    {
      string ret = null;
      if (!string.IsNullOrEmpty(articleTitle))
      {
        ret = articleTitle.Trim().Replace(" ", "_");
      }
      return ret;
    }

    public static string ConvDesktopWiktionaryToMob(string url)
    {
      if (IsDesktopWiktionaryUrl(url))
      {
        var mobile = url.Split('.').ToList();
        mobile.Insert(1, "m");
        url = string.Join(".", mobile);
      }
      return url;
    }

    public static string ConvMobWiktionaryToDesktop(string url)
    {
      if (IsMobileWiktionaryUrl(url))
      {
        var desktop = url.Split('.').ToList();
        desktop.RemoveAt(1);
        url = string.Join(".", desktop);
      }
      return url;
    }

    public static string ConvRelToAbsLink(string baseUrl, string relUrl)
    {
      if (!string.IsNullOrEmpty(baseUrl) && !string.IsNullOrEmpty(relUrl))
      {
        // UriKind.Relative will be false for rel urls containing #
        if (Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute))
        {
          if (baseUrl.EndsWith("/"))
          {
            baseUrl = baseUrl.TrimEnd('/');
          }

          if (relUrl.StartsWith("/") && !relUrl.StartsWith("//"))
          {
            if (relUrl.StartsWith("/wiki") || relUrl.StartsWith("/w/"))
            {
              return $"{baseUrl}{relUrl}";
            }
            return $"{baseUrl}/wiki{relUrl}";
          }
          else if (relUrl.StartsWith("./"))
          {
            if (relUrl.StartsWith("./wiki") || relUrl.StartsWith("./w/"))
            {
              return $"{baseUrl}{relUrl.Substring(1)}";
            }
            return $"{baseUrl}/wiki{relUrl.Substring(1)}";
          }
          else if (relUrl.StartsWith("#"))
          {
            return $"{baseUrl}/wiki/{relUrl}";
          }
          else if (relUrl.StartsWith("//"))
          {
            return $"https:{relUrl}";
          }
        }
      }
      return relUrl;
    }
  }
}
