using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для Page2.xaml
    /// </summary>
    public partial class Page2 : Page
    {
        FileReader fileReader;

        ObservableCollection<FileInfo> itemSourceTable1 = new ObservableCollection<FileInfo>();
        ObservableCollection<ItemSource> itemSourceTable2 = new ObservableCollection<ItemSource>();
        public Page2()
        {
            InitializeComponent();

            if(fileReader.FileItemList.Count > 0)
                textBlockEmpty.Visibility = Visibility.Visible;
            else
                textBlockEmpty.Visibility = Visibility.Collapsed;

            dataGrid1.ItemsSource = fileReader.FileItemList;
        }

        public Page2(FileReader fileReader)
        {
            InitializeComponent();

            this.fileReader = fileReader;

            if (fileReader.FileItemList.Count <= 0)

                textBlockEmpty.Visibility = Visibility.Visible;
            else
                textBlockEmpty.Visibility = Visibility.Collapsed;

            RefreshItemTable1();

            dataGrid1.ItemsSource = itemSourceTable1;
        }

        private void RefreshItemTable1()
        {
            itemSourceTable1.Clear();
            for(int i = 0; i < fileReader.FileItemList.Count; i++)
            {
                itemSourceTable1.Add(fileReader.FileItemList[i].FileInfoListItem[0]);
            }
        }

        private void DataGrid1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FileInfo selectedInfo = (FileInfo)dataGrid1.SelectedItem;

            itemSourceTable2.Clear();

            for (int i = 0; i < fileReader.FileItemList.Count; i++)
            {
                if (selectedInfo.FullName == fileReader.FileItemList[i].FileInfoListItem[0].FullName)
                {
                    //selectedItem = fileReader.FileItemList[i]; 
                    for (int j = 0; j < fileReader.FileItemList[i].FileInfoListItem.Count; j++)
                    {
                        itemSourceTable2.Add(new ItemSource(fileReader.FileItemList[i].FileInfoListItem[j], false));
                    }
                    break;
                }
            }

            dataGrid2.ItemsSource = itemSourceTable2;
        }
        

        private void dataGrid2_MouseOpenDirClick(object sender, RoutedEventArgs e)
        {
            ItemSource selectedInfo = (ItemSource)dataGrid2.SelectedItem;
            Process.Start("explorer.exe", $"/select,\"{selectedInfo.Directory}\\{selectedInfo.Name}\"");
            //Process.Start("explorer.exe", selectedInfo.Directory);
        }

        private void dataGrid2_MouseDelClick(object sender, RoutedEventArgs e)
        {
            ItemSource removeItem = (ItemSource)dataGrid2.SelectedItem;
            itemSourceTable2.Remove(removeItem);
            Delete(removeItem);
            RefreshItemTable1();
            dataGrid1.Items.Refresh();
            dataGrid2.Items.Refresh();
        }

        private void Delete(ItemSource itemSource)
        {
            bool isBreak = false;
            for (int i = 0; i < fileReader.FileItemList.Count; i++)
            {
                if (fileReader.FileItemList[i].FileInfoListItem[0].Length == itemSource.Length)
                {
                    for (int j = 0; j < fileReader.FileItemList[i].FileInfoListItem.Count; j++)
                    {
                        if (fileReader.FileItemList[i].FileInfoListItem[j].FullName == itemSource.FullName)
                        {
                            File.Delete(fileReader.FileItemList[i].FileInfoListItem[j].FullName);
                            fileReader.FileItemList[i].FileInfoListItem.RemoveAt(j);
                            if (fileReader.FileItemList[i].FileInfoListItem.Count == 0)
                                fileReader.FileItemList.RemoveAt(i);
                            isBreak = true;
                            break;
                        }
                    }
                }
                if (isBreak)
                    break;
            }
        }

        private void buttonDelSelect_Click(object sender, RoutedEventArgs e)
        {
            bool isSelect = false;
            for (int i = 0; i < itemSourceTable2.Count; i++)
            {
                if (itemSourceTable2[i].Select)
                {
                    isSelect = true; 
                    break;
                }
            }
            if (isSelect) 
            {
                for (int i = 0; i < itemSourceTable2.Count; i++)
                {
                    if (itemSourceTable2[i].Select)
                    {
                        Delete(itemSourceTable2[i]);
                    }
                }

                for (int i = 0; i < itemSourceTable2.Count; i++)
                {
                    if (itemSourceTable2[i].Select)
                    {
                        itemSourceTable2.Remove(itemSourceTable2[i]);
                        i--;
                    }
                }

                RefreshItemTable1();
                dataGrid1.Items.Refresh();
                dataGrid2.Items.Refresh();
            }
            else
                MessageBox.Show("Не одна з копій ще не вибрана. Оберіть копії, які хочите видалити.", "Помилка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void buttonDelAll_Click(object sender, RoutedEventArgs e)
        {
            bool isSelect = false;
            for (int i = 0; i < itemSourceTable2.Count; i++)
            {
                if (itemSourceTable2[i].Select)
                {
                    isSelect = true;
                    break;
                }
            }
            if (isSelect)
            {
                for (int i = 0; i < itemSourceTable2.Count; i++)
                {
                    if (!itemSourceTable2[i].Select)
                    {
                        Delete(itemSourceTable2[i]);
                    }
                }

                for (int i = 0; i < itemSourceTable2.Count; i++)
                {
                    if (!itemSourceTable2[i].Select)
                    {
                        itemSourceTable2.Remove(itemSourceTable2[i]);
                        i--;
                    }
                }

                RefreshItemTable1();
                dataGrid1.Items.Refresh();
                dataGrid2.Items.Refresh();
            }
            else
                MessageBox.Show("Оригінал ще не вибраний. Оберіть той файл, який хочите зберегти.", "Помилка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
