using Anotar.Serilog;
using PluginManager.Interop.Sys;
using SuperMemoAssistant.Plugins.PopupWindow.Interop;
using SuperMemoAssistant.Sys.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.PopupWiki
{

  //
  // MAIN ARTICLE RETRIEVAL METHODS
  public partial class PopupWikiProvider : PerpetualMarshalByRefObject, IContentProvider
  {

    [LogToErrorOnException]
    public RemoteTask<BrowserContent> FetchHtml(string url)
    {

      try
      {

      }
      catch (RemotingException) { }

    }


  }

  //
  // SEARCH METHODS
  public partial class PopupWikiProvider
  {

    [LogToErrorOnException]
    public RemoteTask<BrowserContent> Search(string url)
    {

      try
      {

      }
      catch (RemotingException) { }

    }

  }

}
