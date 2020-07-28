using SuperMemoAssistant.Interop.Plugins;
using SuperMemoAssistant.Plugins.PopupWindow.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.PopupWiki
{
  public static class PluginBaseEx
  {
    public static bool RegisterPopupWindowProvider<T>(this SMAPluginBase<T> plugin, string name, string[] urlRegexes, IBrowserContentProvider provider) where T : SMAPluginBase<T>
    {
      var svc = plugin.GetService<IPopupWindowSvc>();
      return svc.IsNull()
        ? false
        : svc.RegisterPopupWindowProvider(name, urlRegexes, provider);
    }
  }
}
