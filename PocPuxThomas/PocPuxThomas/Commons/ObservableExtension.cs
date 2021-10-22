using PageUpX.Core.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace PocPuxThomas.Commons
{
    public static class ObservableExtenstions
    {
        public static IDisposable SubscribeSafe<t>(
            this IObservable<t> @this,
            IPuxLogger logger,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {


            return @this
                .Subscribe(
                    _ => { },
                    ex =>
                    {
                        logger.Error($"An exception went unhandled on the observable {@this}", ex, callerMemberName);

                        Debugger.Break();
                    });
        }

        public static IDisposable SubscribeSafe<t>(
            this IObservable<t> @this, Action<t> action,
            IPuxLogger logger,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {


            return @this
                .Subscribe(
                    onNext: action,
                    ex =>
                    {
                        logger.Error($"An exception went unhandled on the observable {@this}", ex, callerMemberName);

                        Debugger.Break();
                    });
        }
    }
}
