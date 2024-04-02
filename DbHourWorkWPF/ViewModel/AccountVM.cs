using DbHourWorkWPF.Items;
using DbHourWorkWPF.Model;
using DbHourWorkWPF.Utilities;

namespace DbHourWorkWPF.ViewModel
{
    class AccountVM : Utilities.ViewModelBase
    {

        string cmdEdit = "UPDATE user SET Image = @img, Name = @nam, Surname = @surn, Login = @log, Password = @pass WHERE IdUser = @_idUser";

        private readonly AccountModel _accountModel;

        public ItemUser Account { get; set; }

        RelayCommand? editCommand;
        RelayCommand? deleteCommand;



        public AccountVM()
        {
            _accountModel = new AccountModel();

            Account = App.Account;
            /*
            if (MainForm.UserName != null) CurrentName = MainForm.UserName;
            if (MainForm.UserLogin != null) CurrentLogin = MainForm.UserLogin;
            if (MainForm.UserRole != null) CurrentRole = MainForm.UserRole;
            if (MainForm.IconUser != null) CurrentIcon = MainForm.IconUser;
            */
        }

        /*
        string[] FillParam(ItemAccount item)
        {
            string[] param = new string[6];
            param[1] = item.Name;
            param[2] = item.Surname;
            param[3] = item.Login;
            param[4] = item.Password;
            
            return param;
        }


        // команда редактирования
        public RelayCommand EditCommand
        {
            get
            {
                return editCommand ??
                  (editCommand = new RelayCommand((o) =>
                  {
                      ItemAccount vm = new ItemAccount
                      {
                          Image = Account.Image,
                          Surname = Account.Surname,
                          Name = Account.Name,
                          Login = Account.Login,
                          Password = Account.Password
                      };
                      ContextAccount contextAccount = new ContextAccount(vm);


                      if (contextAccount.ShowDialog() == true)
                      {
                          string[] param = FillParam(contextAccount.Account);
                          param[5] = Account.Id.ToString();
                          App.serviceDb.OperationOnRecord(cmdEdit, param);
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
        */
    }
}
