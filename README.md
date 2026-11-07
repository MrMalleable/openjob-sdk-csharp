# openjob-sdk-csharp

OpenJob C# Worker SDK（.NET 8）。

## 能力

- Worker 注册 / 心跳 / 优雅下线
- Server 回调：`/job-instance/submit|stop|check`
- 处理器 SPI（`IJobProcessor`）与运行中实例跟踪
- 任务状态上报（成功 / 失败 / 超时 / 停止）

## 配置

`appsettings.json`：

```json
{
  "OpenJob": {
    "ClusterHost": "http://localhost:8080",
    "HeartBeatInterval": 5,
    "Host": "127.0.0.1",
    "Port": 8081,
    "AppName": "test-openjob",
    "Token": "your-token",
    "Version": "1.0.0"
  }
}
```

## 自定义处理器

实现 `IJobProcessor` 并注册到 DI：

```csharp
builder.Services.AddSingleton<IJobProcessor, YourProcessor>();
```

控制台任务的 `processorInfo` 需与 `IJobProcessor.Name` 一致。

## 运行

```bash
dotnet run --project openjob-sdk-csharp-agent.csproj
dotnet test
```
