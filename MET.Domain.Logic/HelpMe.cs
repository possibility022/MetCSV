using System;

namespace MET.Domain.Logic
{
    static class HelpMe
    {
        public static void ThrowIfNull(Product product)
        {
            if (product == null)
                throw new InvalidOperationException(
                    "Product is null.");
        }
    }
}
