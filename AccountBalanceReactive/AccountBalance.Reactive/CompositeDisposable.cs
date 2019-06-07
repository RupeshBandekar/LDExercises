namespace AccountBalance.Reactive
{
    using System;
    using System.Collections.Generic;

    sealed class CompositeDisposable : List<IDisposable>, IDisposable
    {
        public void Dispose()
        {
            foreach (var disp in this)
            {
                try
                {
                    disp?.Dispose();
                }
                catch
                {
                    // Ignore
                }
            }
        }
    }
}