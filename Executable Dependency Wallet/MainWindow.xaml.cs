using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

/*
 *  Application is design in MVVM base model
 */

namespace Executable_Dependency_Wallet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel vm;
        private string selectedType = "process";
        public MainWindow()
        {
            InitializeComponent();
            vm = new MainViewModel();
            this.Loaded += (s, e) => DataContext = vm;
        }
        

        //To enable and display 'Get Info' button
        private void txtFilePath_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtName.Text.Length == 0)
                vm.EnableGetInfoBtn = false;
            else
                vm.EnableGetInfoBtn = true;
        }
        
        //Get Infomation depending upon selected type: {process , dll }
        private void GetInformation(object sender, RoutedEventArgs e)
        {
            vm.ProcessInformation(selectedType);
        }

        private void NameTypeIsChanged(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (sender as RadioButton);
            selectedType = rb.Name;
        }
    }
        
    // all logic of window
    public class MainViewModel : INotifyPropertyChanged
    {
        //Important
        private void Notify(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        // process/dll name
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                Notify("Name");
            }
        }
 
        // Information to display
        private List<string> _listOfInformation;
        public List<string> ListOfInformation
        {
            get { return _listOfInformation; }
            set
            {
                _listOfInformation = value;
                Notify("ListOfInformation");
            }
        }

        private string _searchStatus;
        public string SearchStatus
        {
            get { return _searchStatus; }
            set
            {
                _searchStatus = value;
                Notify("SearchStatus");
            }
        }

        private bool _enableGetInfoBtn;
        public bool EnableGetInfoBtn
        {
            get { return _enableGetInfoBtn; }
            set
            {
                _enableGetInfoBtn = value;
                Notify("EnableGetInfoBtn");
            }
        }
        
        public Helper hObj;

        // Constructor
        public MainViewModel()
        {
            // Setting default value
            hObj = new Helper();
            ListOfInformation = new List<string>();
            
            Name = "";
            SearchStatus = "";
            EnableGetInfoBtn = false;
        }

        // Fetch the information depending upon selectedType{process,dll}
        public void ProcessInformation( string selectedType)
        {
            if(selectedType.Equals("dll"))
            {
                ListOfInformation.Clear();
                ListOfInformation = hObj.GetDLLInformation(Name);
            }
            else
            {
                ListOfInformation.Clear();
                ListOfInformation = hObj.GetProcessInformation(Name);
            }
        }
        
    }

    // Helper class which provide functionality
    public class Helper
    {
        public List<string> GetProcessInformation(string name)
        {
            List<string> info = new List<string>();
            Process[] processes =  Process.GetProcessesByName(name);
            string data;

            if(processes.Length == 0)
            {
                // If process is not running
                info.Add("Process is not running");
            }
            else
            {
                Process process = processes[0];

                // Process Id
                data = string.Format("Process Id: {0}", process.Id);
                info.Add(data);
                
                // Executable Name
                data = string.Format("Executable Name: {0}", process.StartInfo.FileName);
                info.Add(data);

                // Base Priority
                data = string.Format("Base Priority: {0}", process.BasePriority);
                info.Add(data);

                // Start Time
                data = string.Format("Start Time: {0}", process.StartTime);
                info.Add(data);

                // Responding status
                data = string.Format("Is Process Responding: {0}", process.Responding);
                info.Add(data);

                // Thread Count
                data = string.Format("Thread Count: {0}", process.Threads.Count);
                info.Add(data);

                // Process size in byte
                data = string.Format("Process Size in byte: {0}", process.WorkingSet64);
                info.Add(data);


                // A module is an executable file or a dynamic link library (DLL).
                // Get all modules associate with process
                ProcessModuleCollection processModuleCollection = process.Modules;
                data = string.Format("Module use by Process: {0}", processModuleCollection.Count);
                info.Add(data);
                foreach (ProcessModule module in processModuleCollection)
                {
                    data = string.Format("--- {0} ", module.ModuleName);
                    info.Add(data);
                }
                
            }
            return info;
        }

        public List<string> GetDLLInformation(string dllName)
        {
            List<string> info = new List<string>();
            string data;


            // To Store count of reference of dll
            int iRefernceCnt = 0;

            // Get all the running process refernce
            Process[] processes = Process.GetProcesses();


            data = string.Format("Process using {0} ",dllName);
            info.Add(data);
            foreach (Process process in processes)
            {
                try
                {
                    // get all modules of each process
                    foreach (ProcessModule module in process.Modules)
                    {
                        if ( dllName.Equals(module.ModuleName))
                        {
                            iRefernceCnt++;
                            data = string.Format("--- {0} ", process.ProcessName);
                            info.Add(data);
                            break;
                        }
                    }
                }
                catch (Exception)
                { }
            }

            data = string.Format("Reference Count {0} ", iRefernceCnt);
            info.Insert(0,data);
            return info;
        }
    }

}
