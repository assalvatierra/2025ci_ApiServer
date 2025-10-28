namespace ApiServer.Postgres
{
    public class PostgreSQLSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public int CommandTimeout { get; set; } = 30;
        public int MaxPoolSize { get; set; } = 100;

 
    }
}
