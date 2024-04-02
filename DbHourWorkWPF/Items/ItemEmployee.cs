using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DbHourWorkWPF.Items
{
    public class ItemEmployee : INotifyPropertyChanged
    {
        bool isSelected;
        int id;
        int idPost;
        string post;
        string numEmployee;
        string surname;
        string name;
        string? lastname;
        string dateEmployment;
        string? dateDismissal;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }
        public int IdPost
        {
            get { return idPost; }
            set
            {
                idPost = value;
                OnPropertyChanged("IdPost");
            }
        }

        public string Post
        {
            get { return post; }
            set
            {
                post = value;
                OnPropertyChanged("Post");
            }
        }
        public string NumEmployee
        {
            get { return numEmployee; }
            set
            {
                numEmployee = value;
                OnPropertyChanged("NumEmployee");
            }
        }

        public string Surname
        {
            get { return surname; }
            set
            {
                surname = value;
                OnPropertyChanged("Surname");
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public string? Lastname
        {
            get { return lastname; }
            set
            {
                lastname = value;
                OnPropertyChanged("Lastname");
            }
        }

        public string DateEmployment
        {
            get { return dateEmployment; }
            set
            {
                dateEmployment = value;
                OnPropertyChanged("DateEmployment");
            }
        }

        public string DateDismissal
        {
            get { return dateDismissal; }
            set
            {
                dateDismissal = value;
                OnPropertyChanged("DateDismissal");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
