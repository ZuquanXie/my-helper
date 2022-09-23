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
using System.Xml.Serialization;

namespace AutoScroll
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string saveDataFileName = "savedata.xml";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            LoadFromFile();
        }

        private void SaveToFile(ItemCollection list, string directory)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            SaveData saveData = new SaveData();
            TextWriter writer = new StreamWriter(saveDataFileName);

            saveData.ScoreCount = list.Count;
            saveData.ScoreResourceDirectory = directory;
            foreach(var item in list)
            {
                saveData.ScoreList.Add((Score)item);
            }
            serializer.Serialize(writer, saveData);
            writer.Close();
        }

        private void LoadFromFile()
        {
            if (!File.Exists(saveDataFileName))
            {
                return;
            }
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            serializer.UnknownNode += SerializerUnknownNode;
            serializer.UnknownAttribute += SerializerUnknownAttribute;

            FileStream fs = new FileStream(saveDataFileName, FileMode.Open);

            SaveData saveData = (SaveData)serializer.Deserialize(fs);
            Scores scores = (Scores)(Application.Current.Resources["ScoresData"] as ObjectDataProvider)?.Data;
            foreach(var item in saveData.ScoreList)
            {
                scores.Add(item);
            }
            textBoxDirectory.Text = saveData.ScoreResourceDirectory;
        }

        protected void SerializerUnknownNode(object sender, XmlNodeEventArgs e)
        {
            Console.WriteLine("UnknownXMLNode: " + e.Name + " - " + e.Text);
        }
        
        protected void SerializerUnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            Console.WriteLine("UnknownXMLAttribute: " + e.Attr.Name + " - " + e.Attr.Value);
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            var editor = new ScoreEditor(textBoxDirectory.Text);
            editor.Show();
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (scoreListBox.SelectedItems.Count > 0)
            {
                Score score = (Score)scoreListBox.SelectedItems[0];
                var editor = new ScoreEditor(textBoxDirectory.Text, score);
                editor.Show();
            }
            */
            new ScoreCutter().Show();
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
            if (scoreListBox.SelectedItems.Count > 0)
            {
                Scores scores = (Scores)(Application.Current.Resources["ScoresData"] as ObjectDataProvider)?.Data;
                scores.Remove((Score)scoreListBox.SelectedItem);
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveToFile(scoreListBox.Items, textBoxDirectory.Text);
        }

        private void ButtonDirectory_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                textBoxDirectory.Text = dialog.SelectedPath;
            }
        }
    }
}
