using MessageBroker;
using System.Text;
using tNavigatorLauncher;
using tNavigatorModels;
using tNavigatorModels.Project;
using tNavigatorModels.Result;

namespace tNavigatorComputingNode;

internal class Program
{
    public static object LockObject { get; set; } = new();

    static void Main(string[] args)
    {
        Console.Title = nameof(tNavigatorComputingNode);
        Log("Start");

        var config = NodeConfig.LoadConfig("config.json");
        var brokerForConsumeTask = config.GetBroker(BrokerQueue.ModelReadyCalculation);

        brokerForConsumeTask.ConsumeMessageAsync(Calculate);
        Console.ReadKey();
        return;

        async void Calculate(byte[] message)
        {
            var resultMessage = Encoding.UTF8.GetString(message);
            var project = JsonUtil.Deserialize<Project>(resultMessage)!;
            var result = new ModelResult()
            {
                TeamName = project.Team.Name
            };


            try
            {
                Log("Calculate");

                var launcher = new Launcher(new LauncherConfig(config.TNavPath, config.ProjectDirPath), project);
                result = launcher.Start();

                Log("Calculate complete");
            }
            catch (Exception e)
            {
                Log(result.Report = e.Message + e);
            }

            await SendResult(result, project.ResultAddress);
        }

        async Task SendResult(ModelResult result, string url)
        {
            string jsonPayload = JsonUtil.Serialize(result);
            using var client = new HttpClient();
            StringContent content = new(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                //var response = await client.PatchAsync(url, content);
                var response = client.PatchAsync(url, content).Result;
                response.EnsureSuccessStatusCode();
                var res = response.Content.ReadAsStringAsync().Result;

                Log($"{await response.Content.ReadAsStringAsync()} {response.StatusCode}");
            }
            catch (Exception e)
            {
                Log("Exception: " + e.Message + e);
            }
        }
    }

    public static void Log(string message)
    {
        lock (LockObject)
        {
            File.AppendAllLines("log.txt", [$"---{DateTime.Now}", message, "\n"]);
        }

        Console.WriteLine(message);
    }
}