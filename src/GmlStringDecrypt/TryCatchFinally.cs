using System;
using System.Collections.Generic;

namespace GmlStringDecrypt
{
    internal sealed class TryCatchFinally
    {
        internal delegate bool CatchException(Exception e);

        internal delegate bool CatchException<in TException>(TException e)
            where TException : Exception;

        private Action? TryAction;
        private readonly Dictionary<Type, CatchException> CatchAction = new();
        private Action<Exception>? CatchAllAction;
        private Action? FinallyAction;

        public TryCatchFinally() { }

        internal TryCatchFinally Try(Action tryAction) {
            TryAction = tryAction;
            return this;
        }

        internal TryCatchFinally Catch<TException>(CatchException<TException> catchAction)
            where TException : Exception {
            CatchAction[typeof(TException)] = (exception) => catchAction((TException) exception);
            return this;
        }

        internal TryCatchFinally CatchAll(Action<Exception> catchAllAction) {
            CatchAllAction = catchAllAction;
            return this;
        }

        internal TryCatchFinally Finally(Action finallyAction) {
            FinallyAction = finallyAction;
            return this;
        }

        internal void Execute() {
            try { 
                TryAction?.Invoke();
            }
            catch (Exception e) {
                if (CatchAllAction is not null) {
                    CatchAllAction(e);
                }
                else if (CatchAction.TryGetValue(e.GetType(), out CatchException? catchAction)) {
                    catchAction(e);
                }
                else {
                    throw;
                }
            }
            finally {
                FinallyAction?.Invoke();
            }
        }
    }
}