using Microsoft.AspNetCore.Mvc;

namespace UniqChat.Controllers
{
    public class ErrorTextLogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Method to log error messages to a text file
        private void LogErrorToFile(string errorMessage)
        {
            string logFilePath = @"C:\Users\Private\Documents\GitHub\UniqChat\UniqChat\UniqChat\RuntimeExceptions.txt";
            try
            {
                // Create or append to the log file
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"[Timestamp: {DateTime.Now}] - Error Message: {errorMessage}");
                    writer.WriteLine(new string('-', 50)); // Separator
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during logging
                Console.WriteLine("An error occurred while logging: " + ex.Message);
            }
        }
        // Example usage
        public IActionResult LogError(string errorMessage)
        {
            LogErrorToFile($"Error Code: {errorMessage}");
            return Content("Error logged successfully.");
        }
    }
}
