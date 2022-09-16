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
using System.Windows.Shapes;

namespace AutoScroll
{
    /// <summary>
    /// ScoreEditor.xaml 的交互逻辑
    /// </summary>
    public partial class ScoreEditor : Window
    {
        private Score scoreData = new Score();
        private ArrayList localFiles = new ArrayList();
        private ObservableCollection<FileInfo> selectedFiles = new ObservableCollection<FileInfo>();
        public ScoreEditor()
        {
            InitializeComponent();
        }

        public ScoreEditor(Score data)
        {
            scoreData.Name = data.Name;
            scoreData.BPM = data.BPM;
            scoreData.Description = data.Description;
            foreach(var item in data.FileRefrences)
            {
                selectedFiles.Add(new FileInfo(item));
            }
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            GetScoreFiles();
            formGrid.DataContext = scoreData;
            fileListBox.DataContext = localFiles;
            selectedFileListBox.DataContext = selectedFiles;
        }

        private void GetScoreFiles()
        {
            localFiles = new ArrayList();
            var files = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\scoreFiles");
            foreach (var path in files)
            {
                localFiles.Add(new FileInfo(path));
            }
        }

        private Collection<string> GetFileRefrences()
        {
            var result = new Collection<string>();

            foreach(var item in selectedFiles)
            {
                result.Add(item.FullName);
            }

            return result;
        }


        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(scoreData.Name))
            {
                MessageBox.Show("Name can't be null!");
                return;
            }
            Scores scores = (Scores)(Application.Current.Resources["ScoresData"] as ObjectDataProvider)?.Data;
            foreach(var score in scores)
            {
                if (score.Name != scoreData.Name)
                {
                    continue;
                }
                if (MessageBox.Show("Scope " + score.Name + " already exists, confirm to cover?", "Save Scope", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    score.Name = scoreData.Name;
                    score.BPM = scoreData.BPM;
                    score.Description = scoreData.Description;
                    score.FileRefrences = GetFileRefrences();
                }
                Close();
                return;
            }
            scores.Add(new Score(scoreData.Name, scoreData.BPM, scoreData.Description, GetFileRefrences()));
            Close();
        }

        private void FileSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            if (listBox.SelectedItem != null)
            {
                previewGrid.DataContext = (FileInfo)listBox.SelectedItem;
            }
        }

        private void FileListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ListBox listBox = (ListBox)sender;
                var selected = (FileInfo)listBox.SelectedItem;
                if (selected != null)
                {
                    foreach(var item in selectedFiles)
                    {
                        if (item.FullName == selected.FullName)
                        {
                            return;
                        }
                    }
                    selectedFiles.Add((FileInfo)listBox.SelectedItem);
                }
            }
        }

        private void SelectedFileListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                ListBox listBox = (ListBox)sender;
                var selected = (FileInfo)listBox.SelectedItem;
                if (selected != null)
                {
                    selectedFiles.Remove((FileInfo)listBox.SelectedItem);
                }
            }
        }
    }
}
