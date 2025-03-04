namespace csharp_choreography_saga.OrderMicroservice.Configurations
{
    public class AppSetting
    {
        public Connectionstrings ConnectionStrings { get; set; }
        public Logging Logging { get; set; }
        public Rabbitmq RabbitMQ { get; set; }
    }

    public class Connectionstrings
    {
        public string DbConnection { get; set; }
    }

    public class Logging
    {
        public Loglevel LogLevel { get; set; }
    }

    public class Loglevel
    {
        public string Default { get; set; }
        public string MicrosoftAspNetCore { get; set; }
    }

    public class Rabbitmq
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

}
