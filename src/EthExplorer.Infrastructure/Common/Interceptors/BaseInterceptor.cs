using System.Reflection;
using Castle.DynamicProxy;
using EthExplorer.Infrastructure.Common.Extensions;

namespace EthExplorer.Infrastructure.Common.Interceptors
{
    public abstract class BaseInterceptor : IAsyncInterceptor
    {
        public void InterceptSynchronous(IInvocation invocation)
        {
            OnBeforeIntercept(invocation);
            OnInterceptSync(invocation);
        }

        public void InterceptAsynchronous(IInvocation invocation)
        {
            OnBeforeIntercept(invocation);
            OnInterceptAsync(invocation);
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            OnBeforeIntercept(invocation);
            OnInterceptAsync<TResult>(invocation);
        }


        protected virtual void OnBeforeIntercept(IInvocation invocation) { }
        protected virtual void OnInterceptSync(IInvocation invocation) { }
        protected virtual void OnInterceptAsync(IInvocation invocation) { }
        protected virtual void OnInterceptAsync<TResult>(IInvocation invocation) { }
    }

    public abstract class BaseInterceptor<TAttribute> : IAsyncInterceptor where TAttribute : BaseMethodInterceptionAttribute
    {
        public void InterceptSynchronous(IInvocation invocation)
        {
            OnBeforeCheckAttributeIntercept(invocation);

            if (GetAttribute(invocation) is null)
            {
                invocation.Proceed();
                return;
            }

            OnBeforeIntercept(invocation);
            OnBeforeInterceptSync(invocation);
            OnInterceptSync(invocation);

            OnAfterIntercept(invocation).Wait();
        }

        public void InterceptAsynchronous(IInvocation invocation)
        {
            OnBeforeCheckAttributeIntercept(invocation);

            if (GetAttribute(invocation) is null)
            {
                invocation.Proceed();
                return;
            }

            OnBeforeIntercept(invocation);
            OnBeforeInterceptAsync(invocation);
            OnInterceptAsync(invocation);
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            OnBeforeCheckAttributeIntercept(invocation);

            if (GetAttribute(invocation) is null)
            {
                invocation.Proceed();
                return;
            }

            OnBeforeIntercept(invocation);
            OnBeforeInterceptAsync<TResult>(invocation);
            OnInterceptAsync<TResult>(invocation);
        }

        protected virtual void OnBeforeCheckAttributeIntercept(IInvocation invocation) { }
        protected virtual void OnBeforeInterceptAsync<TResult>(IInvocation invocation) { }
        protected virtual void OnBeforeInterceptAsync(IInvocation invocation) { }
        protected virtual void OnBeforeInterceptSync(IInvocation invocation) { }
        protected virtual void OnBeforeIntercept(IInvocation invocation) { }
        protected virtual void OnInterceptSync(IInvocation invocation) { }

        protected virtual void OnInterceptAsync(IInvocation invocation)
        {
            invocation.ReturnValue = InternalInterceptAsync(invocation);
        }

        protected virtual void OnInterceptAsync<TResult>(IInvocation invocation)
        {
            invocation.ReturnValue = InternalInterceptAsync<TResult>(invocation);
        }

        protected virtual Task OnAfterInternalInterceptAsync<TResult>(IInvocation invocation, TResult result) => Task.CompletedTask;
        protected virtual Task OnAfterInternalInterceptAsync(IInvocation invocation) => Task.CompletedTask;

        protected virtual Task OnAfterIntercept(IInvocation invocation) => Task.CompletedTask;

        protected async Task InternalInterceptAsync(IInvocation invocation)
        {
            invocation.Proceed();
            var task = (Task)invocation.ReturnValue;
            await task;

            await OnAfterInternalInterceptAsync(invocation);

            await OnAfterIntercept(invocation);
        }

        protected async Task<TResult> InternalInterceptAsync<TResult>(IInvocation invocation)
        {
            invocation.Proceed();

            var task = (Task<TResult>)invocation.ReturnValue;
            var result = await task;

            await OnAfterInternalInterceptAsync(invocation, result);

            await OnAfterIntercept(invocation);

            return result;
        }

        protected TAttribute? GetAttribute(IInvocation invocation)
        {
            return invocation.MethodInvocationTarget.GetCustomAttribute<TAttribute>();
        }

        protected string GetInvocationMethodFullName(IInvocation invocation)
        {
            var methodFullName = invocation.Method.Name;
            if (invocation.Method.DeclaringType != null) methodFullName = $"{invocation.Method.DeclaringType.FullName}.{invocation.Method.Name}";

            return methodFullName;
        }

        protected List<MethodParamInfo> GetArguments(IInvocation invocation)
        {
            var items = new List<MethodParamInfo>();

            for (var i = 0; i < invocation.Arguments.Length; i++)
            {
                var item = invocation.Method.GetParamInfo(i);
                item.Value = invocation.Arguments[i];

                items.Add(item);
            }

            return items;
        }
    }
}
