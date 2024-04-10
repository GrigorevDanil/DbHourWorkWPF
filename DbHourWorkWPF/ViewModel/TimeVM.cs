using DbHourWorkWPF.Items;
using DbHourWorkWPF.Model;
using DbHourWorkWPF.Utilities;
using DbHourWorkWPF.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;
using MessageBox = System.Windows.MessageBox;

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

        DataView dataTableTimeView;
        RelayCommand? addCommand;
        RelayCommand? editCommand;
        RelayCommand? deleteCommand;

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

        void UpdateListCards()
        {
            if (DataTableTime == null) return;
            Days.Clear();
            DataTableTime.Rows.Clear();
            while (DataTableTime.Columns.Count != 8) DataTableTime.Columns.RemoveAt(8);
            for (int i = 1; i <= DateTime.DaysInMonth(SelectedYear, SelectedMonth + 1); i++)
            {
                DataTableTime.Columns.Add(new DateTime(SelectedYear, SelectedMonth + 1, i).ToString("dd-MM-yyyy ddd", new System.Globalization.CultureInfo("ru-RU")), typeof(string));
                Days.Add(new DateTime(SelectedYear, SelectedMonth + 1, i).ToString("dd-MM-yyyy ddd", new System.Globalization.CultureInfo("ru-RU")));
            }

            Cards = new List<ItemCard>(App.serviceDb.LoadListFromServer($"SELECT `IdCard`, `IdEmployee` FROM `card` WHERE MONTH(card.DateWork) = {SelectedMonth + 1} AND YEAR(card.DateWork) = {SelectedYear} GROUP BY IdEmployee", reader =>
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
            DataTableTime = new DataTable();
            DataTableTime.Columns.Add("Фамилия, инициалы", typeof(string));
            DataTableTime.Columns.Add("Должность", typeof(string));
            DataTableTime.Columns.Add("Табельный номер", typeof(string));
            DataTableTime.Columns.Add("Отработано дней", typeof(int));
            DataTableTime.Columns.Add("Отработано часов", typeof(int));
            DataTableTime.Columns.Add("Неявочных дней", typeof(int));
            DataTableTime.Columns.Add("Коды", typeof(string));
            DataTableTime.Columns.Add("Пропущенные часы", typeof(int));
            SelectedMonth = 0;
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
                          ContextCard contextCard = new ContextCard(Cards[indexRow], Days, SelectedCellIndex - 8, false);
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
                          int indexCell = Cards[indexRow].WorkTimes.FindIndex(item => item.DateWork == Days[SelectedCellIndex - 8]);
                          if (indexRow == -1 || indexCell == -1) return;
                          ContextCard contextCard = new ContextCard(Cards[indexRow], Days, indexCell, true);
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
                          if (MessageBox.Show("Вы уверены что хотите удалить данную запись?", "Удаление дня", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                          {
                              App.serviceDb.DeleteRecord(Cards[indexRow].Id.ToString(), "DELETE FROM card WHERE IdCard = @id");
                              UpdateListCards();
                          }
                      }
                  }));
            }
        }
    }
}
