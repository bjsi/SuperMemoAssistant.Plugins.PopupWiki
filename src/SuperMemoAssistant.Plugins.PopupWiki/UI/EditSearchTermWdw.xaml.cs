using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SuperMemoAssistant.Plugins.PopupWiki.UI
{
  /// <summary>
  /// Interaction logic for EditSearchTermWdw.xaml
  /// </summary>
  public partial class EditSearchTermWdw
  {
    public bool Confirmed = false;
    public string Value;

    public EditSearchTermWdw(string searchTerm, string wdwTitle, string msg)
    {

      InitializeComponent();
      InputTextbox.Focus();
      InputTextbox.Text = searchTerm ?? string.Empty;
      this.Title = wdwTitle;
      this.TextboxDescriptionLabel.Content = msg;
      this.InputTextbox.SelectAll();

    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {

      Confirmed = true;
      Value = InputTextbox.Text;
      Close();

    }

    private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        Button_Click(sender, e);
        e.Handled = true;
      }
      else if (e.Key == Key.Escape)
      {
        Close();
      }
    }
  }
}
