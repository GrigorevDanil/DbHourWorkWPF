using DbHourWorkWPF.Items;
using DbHourWorkWPF.Model;
using DbHourWorkWPF.Utilities;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace DbHourWorkWPF.ViewModel
{
    class DayVM : Utilities.ViewModelBase
    {
        string cmdDay = "SELECT * FROM manualday",
            cmdAddDay = "INSERT INTO manualday VALUES (NULL,@_shortname,@_title)",
            cmdEditDay = "UPDATE manualday SET ShortName = @short, Title = @titl WHERE IdDay = @_idDay";
        private readonly PageModel _pageModel;

        public ObservableCollection<ItemDay> Days { get; set; }

        RelayCommand? addCommand;
        RelayCommand? editCommand;
        RelayCommand? deleteCommand;
        RelayCommand? multiplydeleteCommand;

        string inputTextFilter;

        public DayVM()
        {
            _pageModel = new PageModel();
            UpdateListDays();
        }

        public string InputTextFilter
        {
            get
            {
                return inputTextFilter;
            }
            set
            {
                inputTextFilter = value;
                if (inputTextFilter == "") cmdDay = "SELECT IdDay, ShortName AS 'Короткое имя', Title AS 'Обозначение' FROM manualday";
                else cmdDay = $"SELECT IdDay, ShortName AS 'Короткое имя', Title AS 'Обозначение' FROM manualday WHERE Lower(ShortName) LIKE '%{inputTextFilter}%'";
                UpdateListDays();
                OnPropertyChanged("InputTextFilter");
            }
        }

        void UpdateListDays()
        {
            Days = new ObservableCollection<ItemDay>(App.serviceDb.LoadListFromServer(cmdDay, reader =>
            {
                return new ItemDay()
                {
                    Id = reader.GetInt32("IdDay"),
                    ShortName = reader.GetString("ShortName"),
                    Title = reader.GetString("Title")
                };
            }));
            OnPropertyChanged(nameof(Days));
        }

        string[] FillParam(ItemDay item)
        {
            string[] param = new string[3];
            param[0] = item.ShortName.ToString();
            param[1] = item.Title;
            return param;
        }

        // команда добавления
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand((o) =>
                  {
                      ContextDay contextDay = new ContextDay(new ItemDay());
                      if (contextDay.ShowDialog() == true)
                      {
                          App.serviceDb.OperationOnRecord(cmdAddDay, FillParam(contextDay.Day));
                          UpdateListDays();
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
                      // получаем выделенный объект
                      ItemDay? day = selectedItem as ItemDay;
                      if (day == null) return;

                      ItemDay vm = new ItemDay
                      {
                          Id = day.Id,
                          ShortName = day.ShortName,
                          Title = day.Title
                      };
                      ContextDay contextDay = new ContextDay(vm, true);


                      if (contextDay.ShowDialog() == true)
                      {
                          string[] param = FillParam(contextDay.Day);
                          param[2] = day.Id.ToString();
                          App.serviceDb.OperationOnRecord(cmdEditDay, param);
                          UpdateListDays();
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
                      // получаем выделенный объект
                      ItemDay? day = selectedItem as ItemDay;
                      if (day == null) return;
                      if (new View.MessageWindow( "Удаление дня", "Вы уверены что хотите удалить данную запись?").ShowDialog() == true)
                      {
                          App.serviceDb.DeleteRecord(day.Id.ToString(), "DELETE FROM manualday WHERE IdDay = @id");
                          UpdateListDays();
                      }
                  }));
            }
        }

        // команда множественного удаления
        public RelayCommand MultiplyDeleteCommand
        {
            get
            {
                return multiplydeleteCommand ??
                  (multiplydeleteCommand = new RelayCommand((o) =>
                  {
                      if (new View.MessageWindow( "Удаление дня", "Вы уверены что хотите удалить выбранные записи?").ShowDialog() == true)
                      {
                          foreach (var emp in Days.Where(e => e.IsSelected).ToList()) App.serviceDb.DeleteRecord(emp.Id.ToString(), "DELETE FROM manualday WHERE IdDay = @id");
                          UpdateListDays();
                      }
                  }));
            }
        }

    }
}
