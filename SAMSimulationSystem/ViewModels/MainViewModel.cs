using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SAMSimulationSystem.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged 
    {
        SAMSimulationWrapper.SAMSimulationWrapper _SAMSimulationWrapper;

        public MainViewModel()
        {
            _SAMSimulationWrapper = new SAMSimulationWrapper.SAMSimulationWrapper();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
