# openjob-sdk-csharp

OpenJob C# Worker SDK（.NET 8）。作为 HTTP Worker 接入 OpenJob Server：自动注册/心跳/下线，接收任务回调并上报执行状态。

## 能力一览

| 能力 | 说明 |
| --- | --- |
| Worker 生命周期 | 启动 `RegisterOn`（失败重试）、定时心跳、停机 `RegisterOff` |
| Server 回调 | `/job-instance/submit`、`/stop`、`/check` |
| 处理器 SPI | 实现 `IJobProcessor` 并注册到 DI |
| 状态上报 | 运行中 / 成功 / 失败 / 超时 / 停止 |
| 配置 | `IOptions<OpenJobConfig>`，自动补齐 Host/WorkerKey/端口 |

## 快速开始

### 1. 配置

`appsettings.json` / `appsettings.Development.json`：

```json
{
  "OpenJob": {
    "ClusterHost": "http://localhost:8080",
    "HeartBeatInterval": 5,
    "Host": "127.0.0.1",
    "Port": 8081,
    "AppName": "test-openjob",
    "Token": "your-token",
    "Version": "1.0.0",
    "HttpTimeoutSeconds": 10
  }
}
```

说明：

- `ClusterHost` / `AppName` / `Token` 必填
- `Host` 不填时自动探测本机 IPv4
- Worker 上报地址格式为 `Host:Port`（对齐 OpenJob 协议）
- `AppId` 由注册成功响应回填，无需手工配置

### 2. 自定义处理器

```csharp
using openjob_sdk_csharp_agent.core.processor;

public class OrderCleanupProcessor : IJobProcessor
{
    public string Name => "orderCleanupProcessor"; // 必须与控制台 processorInfo 一致

    public async Task<ProcessResult> ProcessAsync(JobContext context)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
        // 业务逻辑...
        await Task.CompletedTask;
        return ProcessResult.Ok("cleanup done");
    }
}
```

在 `Program.cs` 注册：

```csharp
builder.Services.AddSingleton<IJobProcessor, OrderCleanupProcessor>();
```

内置示例处理器：`defaultEchoProcessor`。

### 3. 启动

```bash
dotnet run --project openjob-sdk-csharp-agent.csproj
```

启动后日志应出现：

- `Worker register success`
- 周期性心跳（Debug/Information）

## Server 回调接口

| Method | Path | 作用 |
| --- | --- | --- |
| POST | `/job-instance/submit` | 接收并异步执行任务 |
| POST | `/job-instance/stop` | 取消正在执行的任务 |
| POST | `/job-instance/check` | 检查实例是否仍在运行 |

`submit` 请求体关键字段：

```json
{
  "jobId": 1,
  "jobInstanceId": 1001,
  "processorInfo": "defaultEchoProcessor",
  "jobParams": "{\"key\":\"value\"}",
  "executeTimeout": 60
}
```

成功响应示例：

```json
{
  "status": 200,
  "code": 0,
  "message": "OK",
  "data": "accepted",
  "serverTime": 1730000000000
}
```

## 执行流程

1. Worker 启动 → 调用 Server `/openapi/worker/start`
2. 后台按 `HeartBeatInterval` 调用 `/openapi/worker/heartbeat`，携带运行中 `jobInstanceId`
3. Server 回调 `/job-instance/submit` → 查找处理器 → 异步执行
4. 执行过程上报 `/openapi/job-instance/handle-status`
5. 进程退出时调用 `/openapi/worker/stop`

## 验证用例

```bash
dotnet test openjob-sdk-csharp-agent.sln -c Release
```

当前覆盖：

- 配置规范化与校验（`OpenJobConfigTests`）
- 处理器注册与默认 Echo 执行（`JobProcessorRegistryTests`）
- 成功响应构造（`ApiResponseHelperTests`）
- 任务提交/拒绝/停止/检查（`JobInstanceServiceTests`）
- 本机 IP 探测不抛异常（`NetHelperTests`）

本地联调建议：

1. 启动 OpenJob Server，确认应用名与 Token
2. 启动本 Worker
3. 在控制台创建 HTTP 任务，`processorInfo=defaultEchoProcessor`
4. 触发一次执行，观察 Worker 日志与实例状态

## 项目结构

```
Controllers/          # Server 回调入口
common/helper/        # Http 客户端、网络工具、响应构造
common/request|response|constant/
config/               # OpenJobConfig
core/                 # JobInstanceService / HeartBeatService
core/processor/       # IJobProcessor SPI
tests/                # xUnit 用例
```
