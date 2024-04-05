using DbHourWorkWPF.Items;
using DbHourWorkWPF.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbHourWorkWPF.Model
{
    class EmployeeModel
    {
        public string cmdEmp = "SELECT IdEmployee, employee.IdPost, manualpost.Title, NumEmployee, Surname,Name, Lastname , DateEmployment, DateDismissal FROM employee LEFT JOIN manualpost ON manualpost.IdPost = employee.IdPost",
            cmdAddEmp = "INSERT INTO employee VALUES (NULL,@idPost,@numEmp,@surn,@name,@last,@dateEmp,@dateDis);",
            cmdEditEmp = "UPDATE employee SET IdPost = @_idPost, NumEmployee = @numEmp, Surname = @surn, Name = @name, Lastname = @last, DateEmployment = @dateEmp, DateDismissal = @dateDis WHERE IdEmployee = @idEmp",
            cmdAddPost = "INSERT INTO manualpost VALUES (NULL,@titl)",
            cmdEditPost = "UPDATE manualpost SET Title = @titl WHERE IdPost = @_idPost";

        public RelayCommand? addCommand { get; set; }
        public RelayCommand? editCommand { get; set; }
        public RelayCommand? deleteCommand { get; set; }

        public RelayCommand? addPostCommand { get; set; }
        public RelayCommand? editPostCommand { get; set; }
        public RelayCommand? deletePostCommand { get; set; }

        public RelayCommand? multiplydeleteCommand { get; set; }
        public RelayCommand? runPostFormCommand { get; set; }

        public ItemPost selectedPost { get; set; }
        public string inputTextFilter { get; set; }
    }
}
