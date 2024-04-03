using DbHourWorkWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbHourWorkWPF.ViewModel
{
    class AdminVM : Utilities.ViewModelBase
    {
        private readonly PageModel _pageModel;

        public AdminVM()
        {
            _pageModel = new PageModel();
        }
    }
}
