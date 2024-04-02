using DbHourWorkWPF.Model;

namespace DbHourWorkWPF.ViewModel
{
    class TimeVM : Utilities.ViewModelBase
    {
        private readonly PageModel _pageModel;

        public TimeVM()
        {
            _pageModel = new PageModel();
        }
    }
}
