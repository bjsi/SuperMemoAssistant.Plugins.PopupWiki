﻿#region License & Metadata

// The MIT License (MIT)
// 
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// 
// 
// Created On:   6/24/2020 2:59:25 AM
// Modified By:  james

#endregion




namespace SuperMemoAssistant.Plugins.PopupWiki
{
  using System.Collections.Generic;
  using System.Diagnostics.CodeAnalysis;
  using System.Runtime.Remoting;
  using System.Windows.Input;
  using Anotar.Serilog;
  using SuperMemoAssistant.Plugins.PopupWindow.Interop;
  using SuperMemoAssistant.Services;
  using SuperMemoAssistant.Services.IO.HotKeys;
  using SuperMemoAssistant.Services.IO.Keyboard;
  using SuperMemoAssistant.Services.Sentry;
  using SuperMemoAssistant.Services.UI.Configuration;
  using SuperMemoAssistant.Sys.IO.Devices;

  // ReSharper disable once UnusedMember.Global
  // ReSharper disable once ClassNeverInstantiated.Global
  [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
  public class PopupWikiPlugin : SentrySMAPluginBase<PopupWikiPlugin>
  {
    #region Constructors

    /// <inheritdoc />
    public PopupWikiPlugin() : base("https://6d08a2c936c642c690dd4edad996e782@sentry.io/5171113") { }

    #endregion

    #region Properties Impl - Public

    /// <inheritdoc />
    public override string Name => "PopupWiki";

    /// <inheritdoc />
    public override bool HasSettings => true;
    public PopupWikiCfg Config { get; set; }
    private PopupWikiProvider _popupWikiProvider { get; } = new PopupWikiProvider();
    private IPopupWindowSvc popupWindowSvc { get; set; }

    #endregion

    private void LoadConfig()
    {
      Config = Svc.Configuration.Load<PopupWikiCfg>() ?? new PopupWikiCfg();
    }

    #region Methods Impl

    /// <inheritdoc />
    protected override void PluginInit()
    {

      LoadConfig();

      this.RegisterPopupWindowProvider("Wikipedia", new List<string> { UrlUtils.DesktopWikiUrlRegex, UrlUtils.MobileWikiUrlRegex }, _popupWikiProvider);

      popupWindowSvc = GetService<IPopupWindowSvc>();

      Svc.HotKeyManager
         .RegisterGlobal(
           "SearchWikipedia",
           "Search Wikipedia for the selected term",
           HotKeyScopes.SM,
           new HotKey(Key.W, KeyModifiers.CtrlAlt),
           WikipediaSearch
      );

    }

    // /// <inheritdoc />
    public override void ShowSettings()
    {
      ConfigurationWindow.ShowAndActivate(HotKeyManager.Instance, Config);
    }

    [LogToErrorOnException]
    public async void WikipediaSearch()
    {
      try
      {

        string query = Popups.GetSearchQuery("Search Wikipedia");
        if (query.IsNullOrEmpty())
          return;

        // build the search url
        string url = "";

        if (await popupWindowSvc?.Open(url))
        {
          LogTo.Debug("");
        }
        else
        {
          LogTo.Error("");
        }

      }
      catch (RemotingException) { }

    }

    #endregion

      #region Methods
      #endregion
    }
  }