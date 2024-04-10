using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DbHourWorkWPF.Items
{
    public class ItemCard : INotifyPropertyChanged
    {
        int id;
        ItemEmployee employee = new ItemEmployee();

        List<WorkTime> workTimes = new List<WorkTime>();

        public int Id 
        {
            get { return id; }
            set { id = value; OnPropertyChanged(nameof(Id)); }
        }
       
        public ItemEmployee Employee
        {
            get { return employee; }
            set { employee = value; OnPropertyChanged(nameof(Employee)); }
        }

        public List<WorkTime> WorkTimes
        {
            get { return workTimes; }
            set { workTimes = value; OnPropertyChanged(nameof(WorkTimes));}
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class WorkTime : INotifyPropertyChanged
    {
        string dateWork;
        int hourWork;
        ItemDay day = new ItemDay();

        public ItemDay Day
        {
            get { return day; }
            set { day = value; OnPropertyChanged(nameof(Day)); }
        }


        public string DateWork
        {
            get { return dateWork; }
            set { dateWork = value; OnPropertyChanged(nameof(DateWork)); }
        }

        public int HourWork
        {
            get { return hourWork; }
            set { hourWork = value; OnPropertyChanged(nameof(HourWork)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
