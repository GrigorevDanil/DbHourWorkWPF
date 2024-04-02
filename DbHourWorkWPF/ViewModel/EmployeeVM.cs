using DbHourWorkWPF.Items;
using DbHourWorkWPF.Model;
using DbHourWorkWPF.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;

namespace DbHourWorkWPF.ViewModel
{
    class EmployeeVM : ViewModelBase
    {
        string cmdEmp = "SELECT IdEmployee, employee.IdPost, manualpost.Title, NumEmployee, Surname,Name, Lastname , DateEmployment, DateDismissal FROM employee LEFT JOIN manualpost ON manualpost.IdPost = employee.IdPost",
            cmdAddEmp = "INSERT INTO employee VALUES (NULL,@idPost,@numEmp,@surn,@name,@last,@dateEmp,@dateDis);",
            cmdEditEmp = "UPDATE employee SET IdPost = @_idPost, NumEmployee = @numEmp, Surname = @surn, Name = @name, Lastname = @last, DateEmployment = @dateEmp, DateDismissal = @dateDis WHERE IdEmployee = @idEmp",
            cmdAddPost = "INSERT INTO manualpost VALUES (NULL,@titl)",
            cmdEditPost = "UPDATE manualpost SET Title = @titl WHERE IdPost = @_idPost";

        RelayCommand? addCommand;
        RelayCommand? editCommand;
        RelayCommand? deleteCommand;

        RelayCommand? addPostCommand;
        RelayCommand? editPostCommand;
        RelayCommand? deletePostCommand;

        RelayCommand? multiplydeleteCommand;
        RelayCommand? runPostFormCommand;

        ItemPost selectedPost;
        string inputTextFilter;


        private readonly EmployeeModel _empModel;


        public ObservableCollection<ItemEmployee> Employees { get; set; }
        public ObservableCollection<ItemPost> Posts { get; set; }

        public ItemPost SelectedPost
        {
            get { return selectedPost; }
            set
            {
                selectedPost = value;
                if (selectedPost == null && (inputTextFilter == null || inputTextFilter == "")) cmdEmp = "SELECT IdEmployee, employee.IdPost, manualpost.Title, NumEmployee, Surname,Name, Lastname , DateEmployment, DateDismissal FROM employee LEFT JOIN manualpost ON manualpost.IdPost = employee.IdPost";
                else if (selectedPost == null && (inputTextFilter != null || inputTextFilter != "")) cmdEmp = $"SELECT IdEmployee, employee.IdPost, manualpost.Title, NumEmployee, Surname,Name, Lastname , DateEmployment, DateDismissal FROM employee LEFT JOIN manualpost ON manualpost.IdPost = employee.IdPost WHERE Lower(CONCAT(NumEmployee,Surname,Name,Lastname)) LIKE '%{inputTextFilter}%'";
                else if (selectedPost != null && (inputTextFilter == null || inputTextFilter == "")) cmdEmp = $"SELECT IdEmployee, employee.IdPost, manualpost.Title, NumEmployee, Surname,Name, Lastname , DateEmployment, DateDismissal FROM employee LEFT JOIN manualpost ON manualpost.IdPost = employee.IdPost WHERE manualpost.IdPost = {selectedPost.Id}";
                else cmdEmp = $"SELECT IdEmployee, employee.IdPost, manualpost.Title, NumEmployee, Surname,Name, Lastname , DateEmployment, DateDismissal FROM employee LEFT JOIN manualpost ON manualpost.IdPost = employee.IdPost WHERE manualpost.IdPost = {selectedPost.Id} AND Lower(CONCAT(NumEmployee,Surname,Name,Lastname)) LIKE '%{inputTextFilter}%'";
                UpdateListEmployee();
                OnPropertyChanged("SelectedPost");

            }
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
                if (selectedPost == null && inputTextFilter == "") cmdEmp = "SELECT IdEmployee, employee.IdPost, manualpost.Title, NumEmployee, Surname,Name, Lastname , DateEmployment, DateDismissal FROM employee LEFT JOIN manualpost ON manualpost.IdPost = employee.IdPost";
                else if (selectedPost == null && inputTextFilter != "") cmdEmp = $"SELECT IdEmployee, employee.IdPost, manualpost.Title, NumEmployee, Surname,Name, Lastname , DateEmployment, DateDismissal FROM employee LEFT JOIN manualpost ON manualpost.IdPost = employee.IdPost WHERE Lower(CONCAT(NumEmployee,Surname,Name,Lastname)) LIKE '%{inputTextFilter}%'";
                else cmdEmp = $"SELECT IdEmployee, employee.IdPost, manualpost.Title, NumEmployee, Surname,Name, Lastname , DateEmployment, DateDismissal FROM employee LEFT JOIN manualpost ON manualpost.IdPost = employee.IdPost WHERE manualpost.IdPost = {selectedPost.Id} AND Lower(CONCAT(NumEmployee,Surname,Name,Lastname)) LIKE '%{inputTextFilter}%'";
                UpdateListEmployee();
                OnPropertyChanged("InputTextFilter");
            }
        }


        public EmployeeVM()
        {
            _empModel = new EmployeeModel();
            UpdateListEmployee();
            UpdateListPosts();
        }

        void UpdateListPosts()
        {
            Posts = new ObservableCollection<ItemPost>(App.serviceDb.LoadListFromServer("Select * FROM manualpost", reader =>
            {
                return new ItemPost()
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1)
                };
            }));
            OnPropertyChanged(nameof(Posts));
        }

        void UpdateListEmployee()
        {
            Employees = new ObservableCollection<ItemEmployee>(App.serviceDb.LoadListFromServer(cmdEmp, reader =>
            {
                return new ItemEmployee()
                {
                    Id = reader.GetInt32(0),
                    IdPost = reader.GetInt32(1),
                    Post = reader.GetString(2),
                    NumEmployee = reader.GetString(3),
                    Surname = reader.GetString(4),
                    Name = reader.GetString(5),
                    Lastname = reader[6] != DBNull.Value ? reader.GetString(6) : null,
                    DateEmployment = reader.GetDateTime(7).ToString("dd.MM.yyyy"),
                    DateDismissal = reader[8] != DBNull.Value ? reader.GetDateTime(8).ToString("dd.MM.yyyy") : null
                };
            }));
            OnPropertyChanged(nameof(Employees));
        }

        string[] FillParam(ItemPost item)
        {
            string[] param = new string[2];
            param[0] = item.Title;
            return param;
        }

        string[] FillParam(ItemEmployee item)
        {
            string[] param = new string[8];
            param[0] = item.IdPost.ToString();
            param[1] = item.NumEmployee;
            param[2] = item.Surname;
            param[3] = item.Name;
            param[4] = item.Lastname != null ? item.Lastname : null;
            param[5] = item.DateEmployment;
            param[6] = item.DateDismissal != null ? item.DateDismissal : null;
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
                      ContextEmployee contextEmployee = new ContextEmployee(new ItemEmployee());
                      if (contextEmployee.ShowDialog() == true)
                      {
                          App.serviceDb.OperationOnRecord(cmdAddEmp, FillParam(contextEmployee.Employee));
                          UpdateListEmployee();
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
                      ItemEmployee? emp = selectedItem as ItemEmployee;
                      if (emp == null) return;

                      ItemEmployee vm = new ItemEmployee
                      {
                          Id = emp.Id,
                          IdPost = emp.IdPost,
                          Post = emp.Post,
                          NumEmployee = emp.NumEmployee,
                          Surname = emp.Surname,
                          Name = emp.Name,
                          Lastname = emp.Lastname,
                          DateEmployment = emp.DateEmployment,
                          DateDismissal = emp.DateDismissal,
                      };
                      ContextEmployee contextEmployee = new ContextEmployee(vm);


                      if (contextEmployee.ShowDialog() == true)
                      {
                          string[] param = FillParam(contextEmployee.Employee);
                          param[7] = emp.Id.ToString();
                          App.serviceDb.OperationOnRecord(cmdEditEmp, param);
                          UpdateListEmployee();
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
                      ItemEmployee? emp = selectedItem as ItemEmployee;
                      if (emp == null) return;
                      if (MessageBox.Show("Вы уверены что хотите удалить данную запись?", "Удаление сотрудника", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                      {
                          App.serviceDb.DeleteRecord(emp.Id.ToString(), "DELETE FROM employee WHERE employee.IdEmployee = @id");
                          UpdateListEmployee();
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
                  (multiplydeleteCommand = new RelayCommand((obj) =>
                  {
                      if (MessageBox.Show("Вы уверены что хотите удалить выбранные записи?", "Удаление сотрудника", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                      {
                          foreach (var emp in Employees.Where(e => e.IsSelected).ToList()) App.serviceDb.DeleteRecord(emp.Id.ToString(), "DELETE FROM employee WHERE employee.IdEmployee = @id");
                          UpdateListEmployee();
                      }
                  }));
            }
        }

        // справочник должностей
        public RelayCommand RunPostFormCommand
        {
            get
            {
                return runPostFormCommand ??
                  (runPostFormCommand = new RelayCommand((obj) =>
                  {
                      PostForm form = new PostForm();
                      UpdateListPosts();
                      form.ShowDialog();
                      UpdateListPosts();
                  }));
            }
        }

        // команда добавления должности
        public RelayCommand AddPostCommand
        {
            get
            {
                return addPostCommand ??
                  (addPostCommand = new RelayCommand((o) =>
                  {
                      ContextPost contextPost = new ContextPost(new ItemPost());
                      if (contextPost.ShowDialog() == true)
                      {
                          App.serviceDb.OperationOnRecord(cmdAddPost, FillParam(contextPost.Post));
                          UpdateListPosts();
                      }
                  }));
            }
        }

        // команда редактирования должности
        public RelayCommand EditPostCommand
        {
            get
            {
                return editPostCommand ??
                  (editPostCommand = new RelayCommand((selectedItem) =>
                  {
                      // получаем выделенный объект
                      ItemPost? post = selectedItem as ItemPost;
                      if (post == null) return;

                      ItemPost vm = new ItemPost
                      {
                          Id = post.Id,
                          Title = post.Title,
                      };
                      ContextPost contextPost = new ContextPost(vm);


                      if (contextPost.ShowDialog() == true)
                      {
                          string[] param = FillParam(contextPost.Post);
                          param[1] = post.Id.ToString();
                          App.serviceDb.OperationOnRecord(cmdEditPost, param);
                          UpdateListPosts();
                      }
                  }));
            }
        }

        // команда удаления
        public RelayCommand DeletePostCommand
        {
            get
            {
                return deletePostCommand ??
                  (deletePostCommand = new RelayCommand((selectedItem) =>
                  {
                      // получаем выделенный объект
                      ItemPost? post = selectedItem as ItemPost;
                      if (post == null) return;
                      if (MessageBox.Show("Вы уверены что хотите удалить данную запись?", "Удаление сотрудника", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                      {
                          App.serviceDb.DeleteRecord(post.Id.ToString(), "DELETE FROM manualpost WHERE manualpost.IdPost = @id");
                          UpdateListPosts();
                      }
                  }));
            }
        }

    }
}
