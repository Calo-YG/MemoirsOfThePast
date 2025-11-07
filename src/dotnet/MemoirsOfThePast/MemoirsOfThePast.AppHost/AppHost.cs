var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MemoirsOfThePast_HoST>("MemoirsOfThePast");

builder.Build().Run();
