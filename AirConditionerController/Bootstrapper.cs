using System;
using Stylet;
using StyletIoC;
using AirConditionerController.Pages;
using Utilities;

namespace AirConditionerController
{
    public class Bootstrapper : Bootstrapper<WelcomeViewModel>
    {
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            // Configure the IoC container in here
        }

        protected override void Configure()
        {
            // Perform any other configuration before the application starts
            UDG_.Initialize();
        }
    }
}
