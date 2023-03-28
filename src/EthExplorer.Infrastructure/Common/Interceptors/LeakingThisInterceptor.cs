using Castle.DynamicProxy;

namespace EthExplorer.Infrastructure.Common.Interceptors
{
    public class LeakingThisInterceptor : BaseInterceptor
    {
        protected override void OnBeforeIntercept(IInvocation invocation)
        {
            invocation.Proceed();

            if (invocation.ReturnValue == invocation.InvocationTarget)
            {
                invocation.ReturnValue = invocation.Proxy;
            }
        }
    }
}
