using DbHourWorkWPF.Model;

namespace DbHourWorkWPF.ViewModel
{
    class ConnectionVM : Utilities.ViewModelBase
    {
        private readonly PageModel _pageModel;

        public ConnectionVM()
        {
            _pageModel = new PageModel();
        }
    }
}
