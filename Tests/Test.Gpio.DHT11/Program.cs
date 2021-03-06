﻿#region References

using System;
using Raspberry.IO.Components.Sensors.Temperature.Dht;
using Raspberry.IO.GeneralPurpose;
using Raspberry.Timers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

#endregion

namespace Test.Gpio.DHT11
{
    internal class Program
    {
        private static void Main()
        {
            const ConnectorPin measurePin = ConnectorPin.P1Pin7;

            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();

            var sp = serviceCollection.BuildServiceProvider();
            sp.GetRequiredService<LoggerFactory>().AddConsole().AddDebug();
            var log = sp.GetRequiredService<ILogger<Program>>();

            log.LogInformation("DHT-11/DHT-22 Sample: measure humidity and temperature");
            log.LogInformation("\tMeasure: {0}", measurePin);

            var driver = GpioConnectionSettings.GetBestDriver(GpioConnectionDriverCapabilities.CanChangePinDirectionRapidly);

            using (var pin = driver.InOut(measurePin))
            using (var dhtConnection = new Dht11Connection(sp, pin))
            {
                while (!Console.KeyAvailable)
                {
                    var data = dhtConnection.GetData();
                    if (data != null)
                        Console.WriteLine("{0:0.00}% humidity, {1:0.0}°C, {2} attempts", data.RelativeHumidity.Percent, data.Temperature.DegreesCelsius, data.AttemptCount);
                    else
                        Console.WriteLine("Unable to read data");

                    Timer.Sleep(TimeSpan.FromSeconds(2));
                }
            }
        }
    }
}
