using DbHourWorkWPF.Items;
using DbHourWorkWPF.Model;
using DbHourWorkWPF.Utilities;
using DbHourWorkWPF.View;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;



namespace DbHourWorkWPF.ViewModel
{
    class TimeVM : Utilities.ViewModelBase
    {
        string 
           cmdAdd = "INSERT INTO card VALUES (NULL,@idEmp,@_idDay,@date, @hour)",
           cmdEdit = "UPDATE card SET IdEmployee = @idEmp, IdDay = @_idDay, DateWork = @date, HourWork = @hour WHERE IdCard = @_idCard";

        private readonly PageModel _pageModel;
        int selectedMonth, selectedYear, selectedCellIndex;
        List<string> Days = new List<string>();
        ItemEmployee disEmp;
        bool flagDis;

        DataView dataTableTimeView;
        RelayCommand? addCommand;
        RelayCommand? editCommand;
        RelayCommand? deleteCommand;
        RelayCommand? printHtmlCommand;

        public ObservableCollection<ItemEmployee> DisEmployees { get; set; }




        public bool FlagDis
        {
            get { return flagDis; }
            set { flagDis = value; OnPropertyChanged(nameof(flagDis)); }
        }
        public ItemEmployee DisEmp
        {
            get { return disEmp; }
            set { disEmp = value; OnPropertyChanged(nameof(DisEmp)); UpdateListCards(); }
        }

        public DataView DataTableTimeView
        {
            get { return dataTableTimeView; }
            set { dataTableTimeView = value; OnPropertyChanged(nameof(DataTableTimeView)); }
        }

        public DataTable DataTableTime { get; set; }
        public List<ItemCard> Cards { get; set; }

        public int SelectedMonth
        {
            get { return selectedMonth; }
            set { selectedMonth = value; OnPropertyChanged(nameof(SelectedMonth)); UpdateListCards(); } 
        }

        public int SelectedYear
        {
            get { return selectedYear; }
            set { selectedYear = value; OnPropertyChanged(nameof(SelectedYear)); UpdateListCards(); }
        }

        public int SelectedCellIndex
        {
            get { return selectedCellIndex; }
            set { selectedCellIndex = value; OnPropertyChanged(nameof(SelectedCellIndex)); }
        }

        void UpdateListEmployee()
        {
            DisEmployees = new ObservableCollection<ItemEmployee>(App.serviceDb.LoadListFromServer("SELECT *, manualpost.Title as 'Post' FROM employee LEFT JOIN manualpost ON manualpost.IdPost = employee.IdPost WHERE DateDismissal IS NOT NULL", reader =>
            {
                return new ItemEmployee()
                {
                    Id = reader.GetInt32("IdEmployee"),
                    IdPost = reader.GetInt32("IdPost"),
                    Post = reader.GetString("Post"),
                    NumEmployee = reader.GetString("NumEmployee"),
                    Surname = reader.GetString("Surname"),
                    Name = reader.GetString("Name"),
                    Lastname = reader["Lastname"] != DBNull.Value ? reader.GetString("Lastname") : null,
                    DateEmployment = reader.GetDateTime("DateEmployment").ToString("dd.MM.yyyy"),
                    DateDismissal = reader["DateDismissal"] != DBNull.Value ? reader.GetDateTime("DateDismissal").ToString("dd.MM.yyyy") : null
                };
            }));
            OnPropertyChanged(nameof(DisEmployees));
        }

        void UpdateListCards()
        {
            if (DataTableTime == null) return;
            Days.Clear();
            DataTableTime = new DataTable();
            DataTableTime.Columns.Add("Фамилия, инициалы", typeof(string));
            DataTableTime.Columns.Add("Должность", typeof(string));
            DataTableTime.Columns.Add("Табельный номер", typeof(string));
            DataTableTime.Columns.Add("Отработано дней", typeof(int));
            DataTableTime.Columns.Add("Отработано часов", typeof(int));
            DataTableTime.Columns.Add("Неявочных дней", typeof(int));
            DataTableTime.Columns.Add("Коды", typeof(string));
            DataTableTime.Columns.Add("Пропущенные часы", typeof(int));
            for (int i = 1; i <= DateTime.DaysInMonth(SelectedYear, SelectedMonth + 1); i++)
            {
                DataTableTime.Columns.Add(new DateTime(SelectedYear, SelectedMonth + 1, i).ToString("dd-MM-yyyy ddd", new System.Globalization.CultureInfo("ru-RU")), typeof(string));
                Days.Add(new DateTime(SelectedYear, SelectedMonth + 1, i).ToString("dd-MM-yyyy ddd", new System.Globalization.CultureInfo("ru-RU")));
            }

            string cmdSelect = "";
            if (FlagDis) cmdSelect = $"SELECT `IdCard`, `IdEmployee` FROM `card` WHERE MONTH(card.DateWork) = {SelectedMonth + 1} AND YEAR(card.DateWork) = {SelectedYear} AND IdEmployee = {DisEmp.Id} GROUP BY IdEmployee";
            else cmdSelect = $"SELECT `IdCard`, `IdEmployee` FROM `card` WHERE MONTH(card.DateWork) = {SelectedMonth + 1} AND YEAR(card.DateWork) = {SelectedYear} GROUP BY IdEmployee";

            Cards = new List<ItemCard>(App.serviceDb.LoadListFromServer(cmdSelect, reader =>
            {
                ItemCard item = new ItemCard();
                item.Id = reader.GetInt32("IdCard");
                item.Employee.Id = reader.GetInt32("IdEmployee");
                return item;
            }));

            for(int i = 0; i < Cards.Count;i++)
            {
                Cards[i].Employee = (App.serviceDb.LoadRecordFromServer($"SELECT *, manualpost.Title as 'Post' FROM employee LEFT JOIN manualpost ON manualpost.IdPost = employee.IdPost WHERE IdEmployee = {Cards[i].Employee.Id}", reader =>
                {
                    return new ItemEmployee()
                    {
                        Id = reader.GetInt32("IdEmployee"),
                        IdPost = reader.GetInt32("IdPost"),
                        Post = reader.GetString("Post"),
                        NumEmployee = reader.GetString("NumEmployee"),
                        Surname = reader.GetString("Surname"),
                        Name = reader.GetString("Name"),
                        Lastname = reader["Lastname"] != DBNull.Value ? reader.GetString("Lastname") : null,
                        DateEmployment = reader.GetDateTime("DateEmployment").ToString("dd.MM.yyyy"),
                        DateDismissal = reader["DateDismissal"] != DBNull.Value ? reader.GetDateTime("DateDismissal").ToString("dd.MM.yyyy") : null
                    };
                }));

                Cards[i].WorkTimes = new List<WorkTime>(App.serviceDb.LoadListFromServer($"SELECT IdDay, DateWork, HourWork FROM card WHERE IdEmployee = {Cards[i].Employee.Id} AND MONTH(card.DateWork) = {SelectedMonth + 1} AND YEAR(card.DateWork) = {SelectedYear}", reader =>
                {
                    WorkTime item = new WorkTime();
                    item.Day.Id = reader.GetInt32("IdDay");
                    item.DateWork = reader.GetDateTime("DateWork").ToString("dd-MM-yyyy ddd", new System.Globalization.CultureInfo("ru-RU"));
                    item.HourWork = reader.GetInt32("HourWork");
                    return item;
                }));

                for (int j =0; j < Cards[i].WorkTimes.Count;j++)
                {

                    Cards[i].WorkTimes[j].Day = App.serviceDb.LoadRecordFromServer($"SELECT * FROM manualday WHERE IdDay = {Cards[i].WorkTimes[j].Day.Id}", reader =>
                    {
                        return new ItemDay()
                        {
                            Id = reader.GetInt32("IdDay"),
                            ShortName = reader.GetString("ShortName"),
                            Title = reader.GetString("Title")
                        };
                    });


                }
            }

            if (Cards.Count != 0)
            {
                string codesReason;
                int countHour, workDay, notWorkDay, notWorkHour;
                for (int i=0; i<Cards.Count;i++)
                {
                    if (Cards[i].Employee.DateDismissal != null)
                    {
                        if (DateTime.Parse(Cards[i].Employee.DateDismissal).Month <= SelectedMonth + 1) Cards[i].Employee.IsSelected = true;
                        else continue;
                    }
                    codesReason = "";
                    countHour = 0;
                    workDay = 0;
                    notWorkDay = 0;
                    notWorkHour = 0;
                    DataTableTime.Rows.Add();
                    
                    DataTableTime.Rows[i][0] = Cards[i].Employee.Surname + " " + Cards[i].Employee.Name[0] + "." + Cards[i].Employee.Lastname[0] + ".";
                    DataTableTime.Rows[i][1] = Cards[i].Employee.Post;
                    DataTableTime.Rows[i][2] = Cards[i].Employee.NumEmployee;


                    for (int j = 0, r = 8; j < Cards[i].WorkTimes.Count; j++, r++)
                    {
                        if (Cards[i].WorkTimes[j].Day.ShortName == "Я")
                        {
                            DataTableTime.Rows[i][SearchIndexDataTable(DataTableTime, Cards[i].WorkTimes[j].DateWork)] = Cards[i].WorkTimes[j].HourWork;
                            countHour += Cards[i].WorkTimes[j].HourWork;
                            workDay++;
                        }
                        else if (Cards[i].WorkTimes[j].Day.ShortName == "В")
                        {
                            DataTableTime.Rows[i][SearchIndexDataTable(DataTableTime, Cards[i].WorkTimes[j].DateWork)] = Cards[i].WorkTimes[j].Day.ShortName;
                        }
                        else
                        {
                            DataTableTime.Rows[i][SearchIndexDataTable(DataTableTime, Cards[i].WorkTimes[j].DateWork)] = Cards[i].WorkTimes[j].Day.ShortName;
                            notWorkDay++;
                            if (codesReason == "") codesReason += Cards[i].WorkTimes[j].Day.ShortName;
                            else codesReason += ", " + Cards[i].WorkTimes[j].Day.ShortName;
                            notWorkHour = Cards[i].WorkTimes[j].HourWork;
                        }
                      
                        
                    }

                    DataTableTime.Rows[i][3] = workDay;
                    DataTableTime.Rows[i][4] = countHour;
                    DataTableTime.Rows[i][5] = notWorkDay;
                    DataTableTime.Rows[i][6] = codesReason;
                    DataTableTime.Rows[i][7] = notWorkHour;
                }
            }
            
            DataTableTimeView = null; 
            DataTableTimeView = DataTableTime.DefaultView;
        }

        int SearchIndexDataTable(DataTable dataTable, string headerToFind)
        {
            foreach (DataColumn column in dataTable.Columns) if (column.ColumnName == headerToFind) return column.Ordinal;
            return -1;
        }

        public TimeVM()
        {
            _pageModel = new PageModel();
            SelectedYear = 2024;
            Cards = new List<ItemCard>();
            SelectedMonth = 0;
            DataTableTime = new DataTable();
            UpdateListEmployee();

        }


        string[] FillParam(ItemCard item)
        {
            string[] param = new string[5];
            param[0] = item.Employee.Id.ToString();
            param[1] = item.WorkTimes[0].Day.Id.ToString();
            param[2] = DateTime.Parse(item.WorkTimes[0].DateWork.Substring(0)).ToString("yyyy-MM-dd");
            param[3] = item.WorkTimes[0].HourWork.ToString();
            return param;
        }





        // команда добавления
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand((selectedItem) =>
                  {
                      if (selectedItem is DataRowView)
                      {
                          if (SelectedCellIndex - 8 < 0) return;
                          int indexRow = ((DataRowView)selectedItem).DataView.Table.Rows.IndexOf(((DataRowView)selectedItem).Row);
                          if (Cards[indexRow].Employee.IsSelected) return;
                          int indexCell = Cards[indexRow].WorkTimes.FindIndex(item => item.DateWork == Days[SelectedCellIndex - 8]);
                          if (indexRow == -1) return;
                          if (indexCell != -1)
                          {
                              editCommand.Execute(selectedItem);
                              return;
                          }
                          ContextCard contextCard = new ContextCard(Cards[indexRow], Days, SelectedCellIndex - 8);
                          if (contextCard.ShowDialog() == true)
                          {
                              string[] param = FillParam(contextCard.Card);
                              App.serviceDb.OperationOnRecord(cmdAdd, param);
                              UpdateListCards();
                          }
                      }
                      else
                      {
                          ContextCard contextCard = new ContextCard(new ItemCard(), Days);
                          if (contextCard.ShowDialog() == true)
                          {
                              string[] param = FillParam(contextCard.Card);
                              App.serviceDb.OperationOnRecord(cmdAdd, param);
                              UpdateListCards();
                          }
                      }
                  }));
            }
        }

        // команда редактирования
        public RelayCommand EditCommand
        {
            get
            {
                return editCommand ??
                  (editCommand = new RelayCommand((selectedItem) =>
                  {
                      if (selectedItem is DataRowView)
                      {
                          if (SelectedCellIndex - 8 < 0) return;
                          int indexRow = ((DataRowView)selectedItem).DataView.Table.Rows.IndexOf(((DataRowView)selectedItem).Row);
                          if (Cards[indexRow].Employee.IsSelected) return;
                          int indexCell = Cards[indexRow].WorkTimes.FindIndex(item => item.DateWork == Days[SelectedCellIndex - 8]);
                          if (indexRow == -1) return;
                          if (indexCell == -1)
                          {
                              addCommand.Execute(selectedItem);
                              return;
                          }
                          ContextCard contextCard = new ContextCard(Cards[indexRow], Days, SelectedCellIndex - 8, indexCell, true);
                          if (contextCard.ShowDialog() == true)
                          {
                              string[] param = FillParam(contextCard.Card);
                              param[4] = contextCard.Card.Id.ToString();
                              App.serviceDb.OperationOnRecord(cmdEdit, param);
                              UpdateListCards();
                          }
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
                      if (selectedItem is DataRowView)
                      {
                          if (SelectedCellIndex - 8 < 0) return;
                          int indexRow = ((DataRowView)selectedItem).DataView.Table.Rows.IndexOf(((DataRowView)selectedItem).Row);
                          int indexCell = Cards[indexRow].WorkTimes.FindIndex(item => item.DateWork == Days[SelectedCellIndex - 8]);
                          if (indexRow == -1 || indexCell == -1) return;
                          if (new View.MessageWindow( "Удаление дня", "Вы уверены что хотите удалить данную запись?").ShowDialog() == true)
                          {
                              App.serviceDb.DeleteRecord(Cards[indexCell].Id.ToString(), "DELETE FROM card WHERE IdCard = @id");
                              UpdateListCards();
                          }
                      }
                  }));

            }
           
        }




        // команда печати табеля html
        public RelayCommand PrintHtmlCommand
        {
            get
            {
                return printHtmlCommand ??
                  (printHtmlCommand = new RelayCommand((o) =>
                  {
                      if (Cards.Count == 0)
                      {
                          new MessageWindow("Не найдено записей", "По заданной фильтрации не найдено ни одной записи!", MessageBoxButton.OK, MessageBoxImage.Error).ShowDialog();
                          return;
                      }

                      HtmlDocument htmldoc = new HtmlDocument();

                      

                      htmldoc.Load(Directory.GetCurrentDirectory() + "/PrintPattern/tableHtml.html", Encoding.GetEncoding("UTF-8"));

                      HtmlNode h2 = htmldoc.DocumentNode.SelectSingleNode("//h2");

                      h2.InnerHtml = $"Табель учета рабочего времени за {DateTime.Parse(Cards[0].WorkTimes[0].DateWork.Substring(1)).ToString("MMMM yyyy год")}";

                      var tbody = htmldoc.DocumentNode.SelectSingleNode("//tbody");

                      string row, codesReason;
                      int 
                          hoursWork,
                          workDay,
                          notWorkDay,
                          notWorkHour,
                          partOneWorkDay,
                          partTwoWorkDay,
                          upHour,
                          feastHour;

                      for (int i=0;i<Cards.Count;i++)
                      {
                          row = "";
                          hoursWork = 0;
                          notWorkDay = 0;
                          notWorkHour = 0;
                          workDay = 0;
                          upHour = 0;
                          feastHour = 0;
                          partOneWorkDay = 0;
                          partTwoWorkDay = 0;
                          codesReason = "";
                          row += $"<tr>  " +
                          $"<td>{i+1}</td>" +
                          $"<td>{Cards[i].Employee.Surname + " " + Cards[i].Employee.Name[0] + "." + Cards[i].Employee.Lastname[0] + "."}</td>" +
                          $"<td>{Cards[i].Employee.Post}</td>" +
                          $"<td>{Cards[i].Employee.NumEmployee}</td>";




                          Cards[i].WorkTimes = Cards[i].WorkTimes.OrderBy(item => int.Parse(item.DateWork.Substring(0, 2))).ToList();





                          int it = 0;
                          for (int j = 0, k =1 ; k <= 31;k++)
                          {
                              if (j < Cards[i].WorkTimes.Count) it = int.Parse(Cards[i].WorkTimes[j].DateWork.Substring(0, 2));
                              if (it == k)
                              {
                                  if (Cards[i].WorkTimes[j].Day.ShortName == "Я")
                                  {
                                      row += $"<td>{Cards[i].WorkTimes[j].HourWork}</td>";
                                      hoursWork += Cards[i].WorkTimes[j].HourWork;
                                      workDay++;

                                      if (Cards[i].WorkTimes[j].HourWork > 8) upHour += Cards[i].WorkTimes[j].HourWork - 8;


                                      if (k < 15) partOneWorkDay++;
                                      else partTwoWorkDay++;
                                  }
                                  else if (Cards[i].WorkTimes[j].Day.ShortName == "В")
                                  {
                                      row += $"<td>{Cards[i].WorkTimes[j].Day.ShortName}</td>";
                                      feastHour += Cards[i].WorkTimes[j].HourWork;
                                  }
                                  else
                                  {
                                      row += $"<td>{Cards[i].WorkTimes[j].Day.ShortName}</td>";
                                      notWorkDay++;
                                      if (codesReason == "") codesReason += Cards[i].WorkTimes[j].Day.ShortName;
                                      else codesReason += ", " + Cards[i].WorkTimes[j].Day.ShortName;
                                      notWorkHour = Cards[i].WorkTimes[j].HourWork;
                                  }
                                  j++;
                              }
                              else row += "<td> </td>";

                              if (k == 15)
                                  row += $"<td>{partOneWorkDay}</td>";
                              if (k == 31)
                                  row += $"<td>{partTwoWorkDay}</td>";
                              

                          }

                          row += $"<td>{workDay}</td>" +
                            $"<td>{hoursWork}</td>" +
                            $"<td>{upHour}</td>" +
                            $"<td> </td>" +
                            $"<td>{feastHour}</td>" +
                            $"<td>{notWorkDay}</td>" +
                            $"<td>{codesReason}</td>" +
                            $"<td>{notWorkDay}</td>" +
                            $"</tr>";

                          tbody.AppendChild(HtmlNode.CreateNode(row));
                      }

                      string path = Directory.GetCurrentDirectory() + $@"/Documents/Табель {DateTime.Now.ToString("dd_MM_yyyy HH-mm")}.html";

                      htmldoc.Save(path);

                      var p = new Process();
                      p.StartInfo = new ProcessStartInfo(path)
                      {
                          UseShellExecute = true
                      };
                      p.Start();

                      
                  }));

            }
           
        }

        
        
    }
}
