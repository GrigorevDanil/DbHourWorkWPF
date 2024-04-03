using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace DbHourWorkWPF.Items
{
    public class ItemUser : INotifyPropertyChanged
    {
        ImageSource image;
        int id;
        string name, surname, login, role, fullname;
        bool isLock, isSelected;
        DateTime dateLock;


        public ImageSource Image
        {
            get { return image; }
            set
            {
                image = value;
                OnPropertyChanged(nameof(Image));
            }
        }
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public bool IsLock
        {
            get { return isLock; }
            set
            {
                isLock = value;
                OnPropertyChanged(nameof(IsLock));
            }
        }

        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string FullName
        {
            get { return fullname; }
            set
            {
                fullname = value;
                OnPropertyChanged(nameof(FullName));
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                fullname = surname + " " + name;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(FullName));
            }
        }

        public string Surname
        {
            get { return surname; }
            set
            {
                surname = value;
                fullname = surname + " " + name;
                OnPropertyChanged(nameof(Surname));
                OnPropertyChanged(nameof(FullName));
            }
        }

        public string Login
        {
            get { return login; }
            set
            {
                login = value;
                OnPropertyChanged(nameof(Login));
            }
        }


        public string Role
        {
            get { return role; }
            set
            {
                role = value;
                OnPropertyChanged(nameof(Role));
            }
        }

        public DateTime DateLock
        {
            get { return dateLock; }
            set
            {
                dateLock = value;
                OnPropertyChanged(nameof(DateLock));
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
