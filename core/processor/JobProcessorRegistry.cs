using System.Collections.Concurrent;

namespace openjob_sdk_csharp_agent.core.processor
{
    /// <summary>
    /// 任务处理器注册表
    /// </summary>
    public class JobProcessorRegistry
    {
        private readonly ConcurrentDictionary<string, IJobProcessor> _processors =
            new(StringComparer.OrdinalIgnoreCase);

        public JobProcessorRegistry(IEnumerable<IJobProcessor> processors)
        {
            foreach (var processor in processors)
            {
                Register(processor);
            }
        }

        public void Register(IJobProcessor processor)
        {
            ArgumentNullException.ThrowIfNull(processor);
            if (string.IsNullOrWhiteSpace(processor.Name))
            {
                throw new ArgumentException("Processor name cannot be empty", nameof(processor));
            }

            _processors[processor.Name] = processor;
        }

        public bool TryGet(string? name, out IJobProcessor? processor)
        {
            processor = null;
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            return _processors.TryGetValue(name, out processor);
        }

        public IReadOnlyCollection<string> GetRegisteredNames()
        {
            return _processors.Keys.ToArray();
        }
    }
}
