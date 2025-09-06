using System.Collections.Generic;
using System;

namespace Kontecg.Configuration
{
    public class MassTransitOptions
    {
        public MassTransitOptions()
        {
            Host = "localhost";
            Port = 5672;
            Username = "guest";
            Password = "guest";
            VirtualHost = "/";
            Heartbeat = TimeSpan.FromSeconds(30);
            QueueName = "kontecg";
            ClusterNodes = new List<string>();
        }

        public string Host { get; set; }

        public int Port { get; set; }

        public string VirtualHost { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public TimeSpan Heartbeat { get; set; }

        public string QueueName { get; set; }

        public List<string> ClusterNodes { get; }
    }
}
