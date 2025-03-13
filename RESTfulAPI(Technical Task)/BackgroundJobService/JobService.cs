namespace RESTfulAPI_Technical_Task_.BackgroundJobService
{
    public class JobService
    {
        public void RunTask()
        {
            Console.WriteLine($"Фоновая задача выполняется в {DateTime.Now}");
        }
    }
}
