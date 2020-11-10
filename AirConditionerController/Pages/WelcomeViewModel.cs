using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NLECloudSDK;
using Stylet;
using StyletIoC;
using Utilities;

namespace AirConditionerController.Pages
{
    public class WelcomeViewModel : Screen
    {
        private readonly IWindowManager _windowManager;
        private readonly IContainer _container;

        public WelcomeViewModel(IWindowManager windowManager, IContainer container)
        {
            _windowManager = windowManager;
            _container = container;

        }

    

        protected override void OnViewLoaded()
        {
        
        }

        public void Launch()
        {
            View.Visibility = Visibility.Hidden;
            _windowManager.ShowWindow(_container.Get<ShellViewModel>());
            RequestClose();

        }
    }
}
