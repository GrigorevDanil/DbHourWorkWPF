using DbHourWorkWPF.Items;
using DbHourWorkWPF.Model;
using DbHourWorkWPF.Utilities;
using DbHourWorkWPF.View;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using static System.Net.WebRequestMethods;

namespace DbHourWorkWPF.ViewModel
{
    class CopyVM : Utilities.ViewModelBase
    {
        private readonly PageModel _pageModel;
        string pathCopy, creationTime, lastWriteTime;
        FileInfo curFile;

        public RelayCommand? createCommand { get; set; }
        public RelayCommand? setCommand { get; set; }
        public RelayCommand? deleteCommand { get; set; }
        public RelayCommand? setPathCommand { get; set; }

        public ObservableCollection<FileInfo> Files { get; set; }

        public FileInfo CurFile
        {
            get { return curFile; }
            set 
            { 
                curFile = value;
                CreationTime = CurFile.CreationTime.ToString("dd.MM.yyyy HH:mm");
                LastWriteTime = CurFile.LastWriteTime.ToString("dd.MM.yyyy HH:mm");
                OnPropertyChanged(nameof(CurFile)); 
            }
        }


        public string PathCopy
        {
            get { return pathCopy; }
            set { pathCopy = value; OnPropertyChanged(nameof(PathCopy)); }
        }

        public string CreationTime
        {
            get { return creationTime; }
            set { creationTime = value; OnPropertyChanged(nameof(CreationTime)); }
        }

        public string LastWriteTime
        {
            get { return lastWriteTime; }
            set { lastWriteTime = value; OnPropertyChanged(nameof(LastWriteTime)); }
        }

        void UpdateListFiles()
        {
            Files.Clear();
            foreach (FileInfo file in new DirectoryInfo(PathCopy).GetFiles("*.sql")) Files.Add(file);
        }

        public CopyVM()
        {
            _pageModel = new PageModel();
            PathCopy = App.manager.GetPrivateString("main", "PathCopy");
            Files = new ObservableCollection<FileInfo>();
            UpdateListFiles();
        }

        // команда создания
        public RelayCommand CreateCommand
        {
            get
            {
                return createCommand ??
                  (createCommand = new RelayCommand((o) =>
                  {
                      ProcessStartInfo startInfo = new ProcessStartInfo
                      {
                          FileName = "cmd.exe",
                          RedirectStandardInput = true,
                          UseShellExecute = false
                      };

                      string pathXampp = App.manager.GetPrivateString("main", "PathXampp");

                      if (!Directory.Exists(pathXampp))
                      {
                          pathXampp = App.FindXamppMysqlBin();
                          App.manager.WritePrivateString("main", "PathXampp", pathXampp);
                      }

                      Process process = new Process { StartInfo = startInfo };

                      process.Start();
                      process.StandardInput.WriteLine($"CreateBackup.bat {pathXampp} {PathCopy} Copy_{DateTime.Now.ToString("dd-MM-yyyy_HH-mm")}.sql");
                      process.WaitForExit();

                      UpdateListFiles();
                  }));
            }
        }

        // команда редактирования
        public RelayCommand SetCommand
        {
            get
            {
                return setCommand ??
                  (setCommand = new RelayCommand((seslectedItem) =>
                  {
                      if (new View.MessageWindow("Установка резервной копии", "Вы уверены что хотите установить резервную копию? При установке резервной копии текущая БД будет заменена").ShowDialog() == true)
                      {
                          ProcessStartInfo startInfo = new ProcessStartInfo
                          {
                              FileName = "cmd.exe",
                              RedirectStandardInput = true,
                              UseShellExecute = false
                          };

                          string pathXampp = App.manager.GetPrivateString("main", "PathXampp");

                          if (!Directory.Exists(pathXampp))
                          {
                              pathXampp = App.FindXamppMysqlBin();
                              App.manager.WritePrivateString("main", "PathXampp", pathXampp);
                          }

                          Process process = new Process { StartInfo = startInfo };

                          process.Start();
                          process.StandardInput.WriteLine($"SetBackup.bat {pathXampp} {Directory.GetCurrentDirectory()} {CurFile.Name}");
                          process.WaitForExit();

                          // Создаем новый процесс (новый экземпляр приложения)
                          ProcessStartInfo info = new ProcessStartInfo(Process.GetCurrentProcess().MainModule.FileName);

                          // Запускаем новый процесс
                          Process.Start(info);

                          // Закрываем текущее приложение
                          Application.Current.Shutdown();
                      }
                      



                  }));
            }
        }

        // команда удаления
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand((selectedItem) =>
                  {

                      if (new View.MessageWindow( "Удаление копии", "Вы уверены что хотите удалить данную копию?").ShowDialog() == true)
                      {
                          System.IO.File.Delete(CurFile.FullName);
                          UpdateListFiles();
                      }
                  }));
            }
        }

        // команда установка пути
        public RelayCommand SetPathCommand
        {
            get
            {
                return setPathCommand ??
                  (setPathCommand = new RelayCommand((selectedItem) =>
                  {
                      System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
                      if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                      {
                          PathCopy = dlg.SelectedPath;
                          App.manager.WritePrivateString("main", "PathCopy", PathCopy);
                          UpdateListFiles();
                      }
                  }));
            }
        }



    }
}

