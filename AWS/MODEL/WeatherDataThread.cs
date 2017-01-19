using System;
using System.Collections.Generic;
using System.Text;

using AWS.MODEL;

namespace AWS.CONTROL
{
    class WeatherDataThread
    {
        public KMA2 kma2 = null;
        
        public WeatherDataThread(KMA2 protocol, string protocolType)
        {
            this.kma2 = protocol;
        }

    }
}
