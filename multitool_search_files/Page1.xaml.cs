using multitool_search_files;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageBox = System.Windows.Forms.MessageBox;

namespace multitool_search_files
{
    /// <summary>
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        public delegate void UpdateProgressBarDelegate(DependencyProperty dp, object value);
        BackgroundWorker worker;

        
        List<string> extensions = new List<string>();
        List<string> paths = new List<string>();
        FileReader fileReader;

        long minValue, maxValue;
        public Page1()
        {
            InitializeComponent();

            progressBar.Visibility = Visibility.Collapsed;
            textBlockProgBar.Visibility = Visibility.Collapsed;

            fileReader = new FileReader(this);

            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            fileReader.SearchIdentical(paths, extensions, minValue, maxValue);
        }
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            buttonSearch.IsEnabled = true;
            progressBar.Visibility = Visibility.Collapsed;
            textBlockProgBar.Visibility = Visibility.Collapsed;
            NavigationService.Navigate(new Page2(fileReader));
        }

        private void buttonAddDir_Click(object sender, RoutedEventArgs e)
        {
            bool next = true;
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Оберіть папку"; 
            folderBrowserDialog.SelectedPath = @"C:\"; 
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!paths.Contains(folderBrowserDialog.SelectedPath))
                {
                    int size = 0;
                    for (int i = 0; i < paths.Count; i++)
                    {
                        if (paths[i].Length > folderBrowserDialog.SelectedPath.Length)
                        {
                            if (paths[i].IndexOf(folderBrowserDialog.SelectedPath) >= 0)
                            {
                                next = false;
                                size = paths[i].Length;
                                paths.RemoveAt(i);
                                i--;
                            }

                        }
                        else
                        {
                            if (folderBrowserDialog.SelectedPath.IndexOf(paths[i]) >= 0)
                            {
                                next = false;
                            }
                        }
                    }
                    if (!next)
                    {
                        if (size > folderBrowserDialog.SelectedPath.Length)
                        {
                            paths.Add(folderBrowserDialog.SelectedPath);
                        }
                    }
                    if (next)
                    {
                        paths.Add(folderBrowserDialog.SelectedPath);
                    }
                }

            }

            comboBox.Items.Clear();

            foreach (string folder in paths)
            {
                comboBox.Items.Add(folder);
            }
            comboBox.SelectedIndex = paths.Count - 1;
        }

        private void buttonDelDir_Click(object sender, RoutedEventArgs e)
        {
            if (paths.Count != 0)
            {
                paths.RemoveAt(comboBox.SelectedIndex);
                comboBox.Items.Clear();
                foreach (string folder in paths)
                {
                    comboBox.Items.Add(folder);
                }
                comboBox.SelectedIndex = paths.Count - 1;
            }
            else
                MessageBox.Show("Нічого видаляти, директорії ще не задано.", "Помилка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            if (paths.Count != 0)
            {
                progressBar.Visibility = Visibility.Visible;
                textBlockProgBar.Visibility = Visibility.Visible;
                buttonSearch.IsEnabled = false;

                fileReader = new FileReader(this);

                progressBar.Value = 0;

                worker.RunWorkerAsync();
            }
            else
                MessageBox.Show("Оберіть потрібну директорію для пошуку однакових файлів.", "Помилка!", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
        
        private void buttonAddExt_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxExt.Text != "")
            {
                string strExt = textBoxExt.Text.Replace(" ", "");
                char[] chStrExtExt = strExt.ToCharArray();
                
                if (chStrExtExt[0] != '.')
                    strExt = "." + strExt;

                extensions.Add(strExt);
                textBoxExt.Text = string.Empty;

                comboBoxExt.Items.Clear();

                foreach (string extension in extensions)
                {
                    comboBoxExt.Items.Add(extension);
                }
                comboBoxExt.SelectedIndex = extensions.Count - 1;
            }
            else
                MessageBox.Show("Введіть будь-яке розширення.", "Помилка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void buttonDelExt_Click(object sender, RoutedEventArgs e)
        {
            if (extensions.Count != 0)
            {
                extensions.RemoveAt(comboBoxExt.SelectedIndex);
                comboBoxExt.Items.Clear();
                foreach (string extension in extensions)
                {
                    comboBoxExt.Items.Add(extension);
                }
                comboBoxExt.SelectedIndex = extensions.Count - 1;
            }
            else
                MessageBox.Show("Нічого видаляти, розширення ще не задано.", "Помилка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        bool lockChanges = false;
        void slidersValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (lockChanges)
                return;
            lockChanges = true;
            if (sliderMin != null && sliderMax != null)
            {
                if (sender == sliderMin)
                    if(sliderMin.Value > sliderMax.Value)
                        sliderMax.Value = sliderMin.Value + 1;

                if (sender == sliderMax)
                    if (sliderMax.Value < sliderMin.Value)
                        sliderMin.Value = sliderMax.Value - 1;

                minValue = (long)sliderMin.Value * 1024 * 1024;
                maxValue = (long)sliderMax.Value * 1024 * 1024;
            }
            lockChanges = false;
        }


    }
}
