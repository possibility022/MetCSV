using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MET.Data.Models;
using MET.Data.Models.Profits;
using Microsoft.EntityFrameworkCore;

namespace MET.Data.Storage
{
    public class StorageService : IDisposable
    {
        private readonly StorageContext context;

        public StorageService(StorageContext context)
        {
            this.context = context;
        }

        public async Task MakeSureDbCreatedAsync()
        {
            //await context.Database.EnsureCreatedAsync();
            await context.Database.MigrateAsync();
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

            if (categoryProfit.Provider == Providers.MET || categoryProfit.Provider == Providers.None)
                throw new Exception("Cannot set category profits for this provider: " + categoryProfit.Provider);

            if (categoryProfits.Count == 1)
            {
                categoryProfits[0].Profit = categoryProfit.Profit;
                context.Update(categoryProfits[0]);
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

        public IQueryable<CustomProfit> GetCustomProfits()
        {
            return context.CustomProfits;
        }

        public IQueryable<CategoryProfit> GetCategoryProfits()
        {
            return context.CategoryProfits;
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
