using Anotar.Serilog;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.PopupWiki
{
  public static class HtmlEx
  {
    public static List<string> ParseImageUrls(string html, string baseUrl)
    {
      List<string> imageUrls = new List<string>();

      if (!string.IsNullOrEmpty(html))
      {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var imageNodes = doc.DocumentNode.SelectNodes("//img[@src]");
        if (imageNodes != null)
        {
          foreach (var imageNode in imageNodes)
          {
            string url = imageNode.GetAttributeValue("src", null);
            if (!string.IsNullOrEmpty(url))
            {
              // Convert relative links to absolute
              url = UrlUtils.ConvRelToAbsLink(baseUrl, url);
              if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
              {
                imageUrls.Add(url);
              }
            }
          }
        }
      }
      return imageUrls;
    }



    public static string FilterMobileWikipediaArticle(string html, string language)
    {

      if (string.IsNullOrEmpty(html))
      {
        LogTo.Error("Attempted to call FilterDesktopWikipediaArticle with a null or empty html string.");
        return null;
      }

      if (string.IsNullOrEmpty(language))
      {
        LogTo.Error("Attempted to call FilterMobileWiktionaryArticle with a null or empty language.");
        return null;
      }

      var doc = new HtmlDocument();
      doc.LoadHtml(html);

      string[] nodesToRemove = new[]
      {
        "//script",
        "//noscript",
        "//comment()"
      };

      // Apply HTML Filters
      doc = RemoveHtmlNodes(doc, nodesToRemove);
      doc = RemoveOnclickAttr(doc);
      doc = ConvRelToAbsLinks(doc, $"https://{language}.wikipedia.org");
      doc = WiktionaryDesktopToMobileLinks(doc);
      doc = UpdateBaseHref(doc, $"https://{language}.wikipedia.org/wiki");
      doc = ConvertImagePlaceholders(doc);
      doc = RemoveImageParentLink(doc);
      doc = OpenCollapsedDivs(doc);
      doc = UpdateMetaIE(doc);
      doc = ShowHiddenSections(doc);
      doc = ConvMathImgToPng(doc);

      return doc.DocumentNode.OuterHtml;
    }


    public static string FilterMobileWiktionaryArticle(string html, string language)
    {

      string stylesheet = $"https://{language}.m.wiktionary.org/w/load.php?lang={language}&modules=ext.wikimediaBadges%7Cmediawiki.hlist%7Cmediawiki.ui.button%2Cicon%7Cmobile.init.styles%7Cskins.minerva.base.styles%7Cskins.minerva.content.styles%7Cskins.minerva.content.styles.images%7Cskins.minerva.icons.images%2Cwikimedia%7Cskins.minerva.mainMenu.icons%2Cstyles&only=styles&skin=minerva";

      if (string.IsNullOrEmpty(html))
      {
        // Should have been caught earlier.
        LogTo.Error("Attempted to call FilterMobileWiktionaryArticle with a null or empty html string.");
        return null;
      }

      if (string.IsNullOrEmpty(language))
      {
        // Should have been caught earlier.
        LogTo.Error("Attempted to call FilterMobileWiktionaryArticle with a null or empty language.");
        return null;
      }

      var doc = new HtmlDocument();
      doc.LoadHtml(html);

      string[] nodesToRemove = new[]
      {
        "//script",
        "//header",
        "//comment()",
        "//span[@class='mw-editsection']"
      };

      // Apply HTML Filters
      doc = ConvRelToAbsLinks(doc, $"https://{language}.m.wiktionary.org");
      doc = WiktionaryDesktopToMobileLinks(doc);
      doc = RemoveHtmlNodes(doc, nodesToRemove);
      doc = RemoveNoscriptNodes(doc);
      doc = RemoveImageParentLink(doc);
      doc = UpdateBaseHref(doc, $"https://{language}.m.wiktionary.org/wiki");
      doc = RemoveOnclickAttr(doc);
      doc = UpdateMetaIE(doc);
      doc = UpdateWiktionaryStylesheet(doc, stylesheet);

      return doc.DocumentNode.OuterHtml;
    }


    public static HtmlDocument ConvMathImgToPng(HtmlDocument doc)
    {
      if (doc != null)
      {
        var linkNodes = doc.DocumentNode.SelectNodes("//img[@src]");
        if (linkNodes != null)
        {
          foreach (var linkNode in linkNodes)
          {
            string url = linkNode.Attributes["src"].Value;
            linkNode.Attributes["src"].Value = UrlUtils.ConvMathImgSvgToPng(url);
          }
        }
      }

      return doc;
    }

    /// <summary>
    /// Removes Noscript tags and preserves the children nodes (in wiktionary articles the children are images)
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public static HtmlDocument RemoveNoscriptNodes(HtmlDocument doc)
    {
      if (doc != null)
      {
        var noscriptNodes = doc.DocumentNode.SelectNodes("//noscript");
        if (noscriptNodes != null)
        {
          foreach (var noscriptNode in noscriptNodes)
          {
            var parentNode = noscriptNode.ParentNode;
            parentNode.RemoveChild(noscriptNode, true);
          }
        }
      }
      return doc;
    }

    public static HtmlDocument WiktionaryDesktopToMobileLinks(HtmlDocument doc)
    {
      if (doc != null)
      {
        var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]")?
                                        .Where(a => UrlUtils.IsDesktopWiktionaryUrl(a.GetAttributeValue("href", null)));
        if (linkNodes != null)
        {
          foreach (var linkNode in linkNodes)
          {
            linkNode.Attributes["href"].Value = UrlUtils.ConvDesktopWiktionaryToMob(linkNode.GetAttributeValue("href", null));
          }
        }
      }
      return doc;
    }

    public static HtmlDocument WiktionaryMobileToDesktopLinks(HtmlDocument doc)
    {
      if (doc != null)
      {
        var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]")?
                                        .Where(a => UrlUtils.IsMobileWiktionaryUrl(a.GetAttributeValue("href", null)));
        if (linkNodes != null)
        {
          foreach (var linkNode in linkNodes)
          {
            linkNode.Attributes["href"].Value = UrlUtils.ConvMobWiktionaryToDesktop(linkNode.GetAttributeValue("href", null));
          }
        }
      }
      return doc;
    }

    public static HtmlDocument UpdateMetaIE(HtmlDocument doc)
    {
      if (doc != null)
      {
        string meta = "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=10\">";
        HtmlNode _meta = HtmlNode.CreateNode(meta);
        HtmlNode head = doc.DocumentNode.SelectSingleNode("//head");
        head.ChildNodes.Add(_meta);
      }
      return doc;
    }

    public static HtmlDocument UpdateBaseHref(HtmlDocument doc, string href)
    {
      if (doc != null && !string.IsNullOrEmpty(href))
      {
        HtmlNode _base = doc.DocumentNode.SelectSingleNode("//base");
        if (_base != null)
        {
          _base.SetAttributeValue("href", href);
        }
        else
        {
          // add base node
          var headNode = doc.DocumentNode.SelectSingleNode("//head");
          HtmlNode baseNode = HtmlNode.CreateNode($"<base href=\"{href}\"/>");
          headNode.ChildNodes.Add(baseNode);
        }
      }
      return doc;
    }

    public static HtmlDocument RemoveEditSectionBtns(HtmlDocument doc)
    {
      if (doc != null)
      {
        var editSectionBtns = doc.DocumentNode.SelectNodes("//span[@class='mw-editsection']");
        if (editSectionBtns != null)
        {
          foreach (var editSectionBtn in editSectionBtns)
          {
            editSectionBtn.ParentNode.RemoveChild(editSectionBtn);
          }
        }
      }
      return doc;
    }

    public static HtmlDocument ConvRelToAbsLinks(HtmlDocument doc, string baseUrl)
    {
      if (doc != null)
      {
        // Can't directly check if relative, fails if rel url contains #
        var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]")?
                                        .Where(a => !Uri.IsWellFormedUriString(a.GetAttributeValue("href", null),
                                                                               UriKind.Absolute));
        if (linkNodes != null)
        {
          foreach (var linkNode in linkNodes)
          {
            string absHref = UrlUtils.ConvRelToAbsLink(baseUrl, linkNode.GetAttributeValue("href", null));
            linkNode.Attributes["href"].Value = absHref;
          }
        }
      }
      return doc;
    }

    // Pass a list of xpaths to remove nodes that match any of them
    // Doesn't save the children
    public static HtmlDocument RemoveHtmlNodes(HtmlDocument doc, string[] xpaths)
    {
      if (doc != null && xpaths.Length > 0)
      {
        string selectExpression = string.Join(" | ", xpaths);
        var removeNodes = doc.DocumentNode.SelectNodes(selectExpression);
        if (removeNodes != null)
        {
          foreach (var node in removeNodes)
          {
            node.ParentNode.RemoveChild(node);
          }
        }
      }
      return doc;
    }

    public static HtmlDocument UpdateWiktionaryStylesheet(HtmlDocument doc, string href)
    {
      if (doc != null)
      {
        var stylesheetNode = doc.DocumentNode.SelectSingleNode("//link[@rel='stylesheet']");
        if (stylesheetNode != null)
        {
          stylesheetNode.Attributes["href"].Value = href;
        }
        // TODO: else create one??
      }
      return doc;
    }


    public static HtmlDocument RemoveImageParentLink(HtmlDocument doc)
    {
      if (doc != null)
      {
        // Remove the <a> parent element of images
        var imageNodes = doc.DocumentNode.SelectNodes("//img");
        if (imageNodes != null)
        {
          foreach (HtmlNode imageNode in imageNodes)
          {
            if (imageNode.ParentNode.Name == "a")
            {
              var grandParentNode = imageNode.ParentNode.ParentNode;
              grandParentNode.RemoveChild(imageNode.ParentNode, true);
            }
          }
        }
      }
      return doc;
    }

    public static HtmlDocument ConvertImagePlaceholders(HtmlDocument doc)
    {
      if (doc != null)
      {
        HtmlNodeCollection figureNodes = doc.DocumentNode.SelectNodes("//figure | //figure-inline");
        if (figureNodes != null)
        {
          foreach (HtmlNode figureNode in figureNodes)
          {
            bool hasImg = false;
            foreach (HtmlNode child in figureNode.ChildNodes)
            {
              if (figureNode.Name == "img")
              {
                hasImg = true;
                break;
              }
            }
            if (!hasImg)
            {
              // Image Placeholders all contain the word 'lazy'
              var imgPlaceholder = figureNode.SelectSingleNode("//span[contains(@class, 'lazy')]");
              if (imgPlaceholder != null)
              {
                // Replace the placeholder with an img element
                string dataHeight = imgPlaceholder.GetAttributeValue("data-height", "");
                string dataWidth = imgPlaceholder.GetAttributeValue("data-width", "");
                string dataSrc = imgPlaceholder.GetAttributeValue("data-src", "");

                if (!string.IsNullOrEmpty(dataHeight) && !string.IsNullOrEmpty(dataWidth) && !string.IsNullOrEmpty(dataSrc))
                {
                  HtmlNode imgNode = HtmlNode.CreateNode($"<img src=\"{dataSrc}\" height=\"{dataHeight}\" width=\"{dataWidth}\" />");
                  imgPlaceholder.ParentNode.ChildNodes.Add(imgNode);
                  imgPlaceholder.Remove();
                }
              }
            }
          }
        }
      }
      return doc;
    }

    public static HtmlDocument RemoveOnclickAttr(HtmlDocument doc)
    {
      var onclickNodes = doc.DocumentNode.SelectNodes("//*[@onclick]");
      if (onclickNodes != null)
      {
        foreach (var onclickNode in onclickNodes)
        {
          onclickNode.Attributes.Remove("onclick");
        }
      }
      return doc;
    }

    public static HtmlDocument OpenCollapsedDivs(HtmlDocument doc)
    {
      if (doc != null)
      {
        HtmlNodeCollection collapseNodes = doc.DocumentNode.SelectNodes("//*[contains(@class, 'pcs-collapse')]");
        if (collapseNodes != null)
        {
          foreach (HtmlNode collapseNode in collapseNodes)
          {
            string style = collapseNode.GetAttributeValue("style", null);
            if (style != null && style.Contains("display: none;"))
            {
              style = style.Replace("display: none;", "display: block;");
              collapseNode.SetAttributeValue("style", style);
            }
          }
        }
      }
      return doc;
    }

    public static HtmlDocument ShowHiddenSections(HtmlDocument doc)
    {
      if (doc != null)
      {
        HtmlNodeCollection sectionNodes = doc.DocumentNode.SelectNodes("//section[@style]");
        if (sectionNodes != null)
        {
          foreach (HtmlNode sectionNode in sectionNodes)
          {
            sectionNode.Attributes["style"].Value = "display: block;";
          }
        }
      }
      return doc;
    }
  }
}
