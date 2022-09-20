using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoScroll
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            var editor = new ScoreEditor();
            editor.Show();
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            if (scoreListBox.SelectedItems.Count > 0)
            {
                Score score = (Score)scoreListBox.SelectedItems[0];
                var editor = new ScoreEditor(score);
                editor.Show();
            }
        }

        private void ButtonScan_Click(object sender, RoutedEventArgs e)
        {
            if (scoreListBox.SelectedItems.Count > 0)
            {
                new Player((Score)scoreListBox.SelectedItems[0]).Show();
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
