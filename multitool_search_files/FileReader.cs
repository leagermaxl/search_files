using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static multitool_search_files.Page1;

namespace multitool_search_files
{
    public class FileReader
    {
        Page1 page1;
        public FileReader() { }
        public FileReader(Page1 page1) 
        { 
            this.page1 = page1;
        }

        private List<FileInfo> fileInfoList = new List<FileInfo>();
        private List<FileItem> fileItemList = new List<FileItem>();


        public List<FileInfo> FileInfoList
        {
            get { return fileInfoList; }
        }
        public List<FileItem> FileItemList
        {
            get { return fileItemList; }
        }

        static int CompareBySize(FileInfo a, FileInfo b)
        {
            return a.Length.CompareTo(b.Length);
        }

        public void SearchIdentical(List<string> directory, List<string> extensions, long minSize, long maxSize)
        {
            foreach (string dir in directory)
            {
                ProcessDirectory(dir, extensions, minSize, maxSize);
            }
            fileInfoList.Sort(CompareBySize);
            FileHandling();
        }

        /// <summary>
        /// Зчитування файлів з директорії
        /// </summary>
        /// <param name="targetDirectory">Директорія</param>
        /// <param name="extensions">Список розширень, за якими треба шукати</param>
        /// <param name="minSize">Мінімальний розмір файлу</param>
        /// <param name="maxSize">Максимальний розмір файлу</param>
        void ProcessDirectory(string targetDirectory, List<string> extensions, long minSize, long maxSize)
        {
            //Отримуємо список файлів
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                //Обробляємо файл
                ProcessFile(fileName, extensions, minSize, maxSize);
            }
            //Отримуємо список піддиректорій
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                //Викликаємо рекурсивно оброблення директорій
                ProcessDirectory(subdirectory, extensions, minSize, maxSize);
        }


        /// <summary>
        /// Обробка файлу
        /// </summary>
        /// <param name="path">Шлях до файлу</param>
        /// <param name="extensions">Список розширень, за якими треба шукати</param>
        /// <param name="minSize">Мінімальний розмір файлу</param>
        /// <param name="maxSize">Максимальний розмір файлу</param>
        void ProcessFile(string path, List<string> extensions, long minSize, long maxSize)
        {
            //Створюємо екземляр представлення нашого файлу
            FileInfo fileInfo = new FileInfo(path);
            //Перевіряємо на вхождення у заданий діапазон розмірів
            if (fileInfo.Length > minSize && fileInfo.Length < maxSize)
            {
                if (extensions.Count != 0)
                {
                    foreach (string extension in extensions)
                    {
                        //Перевіряємо на ідентичність заданих розширень
                        if (fileInfo.Extension == extension)
                        {
                            //Додаємо у список файлів
                            fileInfoList.Add(fileInfo);
                            break;
                        }
                    }
                }
                else
                    fileInfoList.Add(fileInfo);
            }
        }

        /// <summary>
        /// Пошук однакових файлів
        /// </summary>
        public void FileHandling()
        {
            //Створення екземпляру делегату, для оновлення ProgressBar на Page1
            Page1.UpdateProgressBarDelegate updateProgressBarDelegate = new UpdateProgressBarDelegate(page1.progressBar.SetValue);
            double value = 0;

            int i = 0;
            //Цикл перебора файлів
            while (i < fileInfoList.Count - 1)
            {
                //Обчислення value для ProgressBar на Page1
                value = ((double)i + 1) / fileInfoList.Count * 100;

                //Оновлення значення value для ProgressBar на Page1
                Dispatcher dispatcher = page1.Dispatcher;
                dispatcher.Invoke(updateProgressBarDelegate, new object[] { ProgressBar.ValueProperty, value });

                string[] fileInfoHashSum = null;
                bool isExist = false;

                //Цикл перевірки, чи існує ланцюг таких самих файлів як наш поточний
                for (int j = 0; j < fileItemList.Count; j++)
                {
                    if (fileInfoList[i].Length == fileItemList[j].FileInfoListItem[0].Length)
                    {
                        //Перевіряється, чи існу вже такий файл у ланцюзі
                        for (int k = 0; k < fileItemList[j].FileInfoListItem.Count; k++)
                        {
                            if (fileInfoList[i].FullName == fileItemList[j].FileInfoListItem[k].FullName)
                            {
                                isExist = true;
                                break;
                            }
                        }
                        if (!isExist)
                        {
                            fileInfoHashSum = CalcHashSum(fileInfoList[i].FullName);
                            if (fileInfoHashSum.SequenceEqual(fileItemList[j].HashSum))
                            {
                                fileItemList[j].FileInfoListItemAdd(fileInfoList[i]);
                                isExist = true;
                                break;
                            }
                        }
                    }
                }
                //Якщо ланцюга не існує
                if (!isExist)
                {
                    int j = i + 1;
                    //Якщо розмір файлів не однаковий
                    if (fileInfoList[i].Length < fileInfoList[j].Length)
                    {
                        i++;
                        continue;
                    }
                    //Цикл перевірки, чи існують такі ж файли
                    while (j < fileInfoList.Count && fileInfoList[i].Length == fileInfoList[j].Length)
                    {
                        if (j < fileInfoList.Count)
                        {
                            //Обрахунок хеш-суми
                            fileInfoHashSum = fileInfoHashSum == null ? CalcHashSum(fileInfoList[i].FullName) : fileInfoHashSum;
                            string[] fileInfoListHashSum = CalcHashSum(fileInfoList[j].FullName);
                            //Порівняння хеш-сум
                            if (fileInfoHashSum.SequenceEqual(fileInfoListHashSum))
                            {
                                if (fileItemList.Count > 0)
                                {
                                    //В переборі файлів з однаковим розміром вже
                                    //було створено ланцюг, то додаємо в нього
                                    if (fileInfoList[i].FullName == fileItemList[fileItemList.Count - 1].FileInfoListItem[0].FullName)
                                    {
                                        fileItemList[fileItemList.Count - 1].FileInfoListItemAdd(fileInfoList[j]);
                                    }
                                    //Якщо ні
                                    else
                                    {
                                        fileItemList.Add(new FileItem(fileInfoList[i], fileInfoList[j], fileInfoHashSum));
                                    }
                                }
                                //Якщо ланцюгів не існує, створюємо новий
                                else
                                {
                                    fileItemList.Add(new FileItem(fileInfoList[i], fileInfoList[j], fileInfoHashSum));
                                }
                            }
                            j++;
                        }
                        else
                            break;
                    }
                }
                i++;
            }
        }

        /// <summary>
        /// Підрахунок хеш-суми файлу
        /// </summary>
        /// <param name="filePath">Місце розташуавання файлу</param>
        /// <returns>Хеш-суму файлу</returns>
        public string[] CalcHashSum(string filePath)
        {
            //Створюємо екземпляр хеш-суми та алгоритму хешування MD5
            string[] hashSum = new string[3];
            using (MD5 md5 = MD5.Create())
            {
                //Отримуємо потік нашого файлу
                using (FileStream stream = File.OpenRead(filePath))
                {
                    //Задаємо поточні зміщення, коефіцієнт та розмір фрагменту
                    long offset = stream.Length / 6;
                    long count = 0;
                    int coef = 1;
                    if (stream.Length < 20480)
                        count = offset;
                    else
                        count = 4096;
                    //Задаємо початок
                    stream.Seek(offset, SeekOrigin.Begin);

                    byte[] buffer = new byte[count];

                    int bytesRead, i = 0;
                    //Зчитуємо потрібну кількість байтів
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        if (i < 3)
                        {
                            //Підраховуємо хеш-суму фрагменту
                            byte[] hash = md5.ComputeHash(buffer, 0, bytesRead);
                            //Перетворюємо в потрібний вигляд
                            hashSum[i] = BitConverter.ToString(hash).Replace("-", "").ToLower();

                            //Задаємо новий коефіцієнт та зміщення
                            i++;
                            coef += 2;
                            offset = stream.Length / 6 * coef;


                            if (offset >= stream.Length)
                            {
                                return hashSum;
                            }
                            //Задаємо новий початок для наступного фрагменту
                            stream.Seek(offset, SeekOrigin.Begin);
                        }
                        else { break; } 
                    }
                    return hashSum;
                }
            }
        }
    }
}
