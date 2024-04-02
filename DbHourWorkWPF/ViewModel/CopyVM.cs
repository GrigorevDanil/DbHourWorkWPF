using DbHourWorkWPF.Model;

namespace DbHourWorkWPF.ViewModel
{
    class CopyVM : Utilities.ViewModelBase
    {
        private readonly PageModel _pageModel;

        public CopyVM()
        {
            _pageModel = new PageModel();
        }
    }
}
