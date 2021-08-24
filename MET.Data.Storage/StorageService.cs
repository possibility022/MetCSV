using System;
using System.Data;
using System.Linq;
using MET.Data.Models.Profits;

namespace MET.Data.Storage
{
    public class StorageService : IDisposable
    {
        private readonly StorageContext context;

        public StorageService(StorageContext context)
        {
            this.context = context;
        }

        public void SetProfit(CategoryProfit categoryProfit)
        {
            var categoryProfits = context
                .CategoryProfits
                .Where(r => r.Category == categoryProfit.Category && r.Provider == categoryProfit.Provider)
                .Take(2)
                .ToList();

            if (categoryProfits.Count >= 2)
            {
                throw new DataException("We should not have two records.");
            }

            if (categoryProfits.Count == 1)
            {
                categoryProfits[0].Profit = categoryProfit.Profit;
                context.Update(categoryProfit);
            }
            else
            {
                context.Add(categoryProfit);
            }

            context.SaveChanges();
        }

        public void SetProfit(CustomProfit customProfit)
        {
            var customProfits = context
                .CustomProfits
                .Where(r => r.PartNumber == customProfit.PartNumber)
                .Take(2)
                .ToList();

            if (customProfits.Count >= 2)
            {
                throw new DataException("We should not have two records.");
            }

            if (customProfits.Count == 1)
            {
                customProfits[0].Profit = customProfit.Profit;
                context.Update(customProfit);
            }
            else
            {
                context.Add(customProfit);
            }

            context.SaveChanges();
        }

        private void ReleaseUnmanagedResources()
        {
            // TODO release unmanaged resources here
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                context?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~StorageService()
        {
            Dispose(false);
        }
    }
}
