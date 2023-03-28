using System.Diagnostics;
using System.Reflection;
using Castle.DynamicProxy;
using EthExplorer.Domain.Common;
using EthExplorer.Domain.Common.Extensions;

namespace EthExplorer.Infrastructure.Common.Interceptors.Diagnostic
{
    public class DiagnosticInterceptor : AsyncTimingInterceptor
    {
        private readonly ILogService _logService;
        
        public DiagnosticInterceptor(ILogService logService)
        {
            _logService = logService;
        }

        protected override void StartingTiming(IInvocation invocation)
        {
            
        }

        protected override void CompletedTiming(IInvocation invocation, Stopwatch stopwatch)
        {
            if (GetAttribute(invocation) is null) return;

            var elapsedSeconds = (decimal)stopwatch.Elapsed.TotalMilliseconds / 1000;

            var msg = GetInfoMessage(invocation, elapsedSeconds);
            
            if (elapsedSeconds > CommonInfraConst.DIAGNOSTIC_METHOD_ELAPSED_SECONDS_WARN_THRESHOLD)
            {
                _logService.Warn(msg);
            }
            else
            {
                _logService.Info(msg);
            }
        }

        private static string GetInfoMessage(IInvocation invocation, decimal elapsedSeconds)
        {
            var methodName = $"{invocation.MethodInvocationTarget.DeclaringType?.FullName}. {invocation.MethodInvocationTarget.Name}";
            var arguments = invocation.Arguments.ToJson();

            return $"[DiagnosticInfo] {methodName} ({arguments}) executed in {elapsedSeconds:0.00} seconds.";
        }

        private DiagnosticAttribute? GetAttribute(IInvocation invocation)
            => invocation.MethodInvocationTarget.GetCustomAttribute<DiagnosticAttribute>();
    }
}
