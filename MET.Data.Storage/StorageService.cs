using System;
using System.Collections.Generic;
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
            await context.Database.MigrateAsync();
        }

        public void MakeSureDbCreated()
        {
            context.Database.Migrate();
        }

        public void SetProfit(CategoryProfit categoryProfit)
        {
            SetProfit<CategoryProfit>(categoryProfit, context.CategoryProfits, (newProfit, oldProfit) => newProfit.Category == oldProfit.Category && newProfit.Provider == oldProfit.Provider);
        }

        public void SetProfit(CustomProfit customProfit)
        {
            SetProfit<CustomProfit>(customProfit, context.CustomProfits, (newProfit, oldProfit) => newProfit.PartNumber == oldProfit.PartNumber);
        }

        public void SetProfit(ManufacturerProfit manufacturerProfit)
        {
            SetProfit<ManufacturerProfit>(manufacturerProfit, context.ManufacturerProfits, (newProfit, oldProfit) => newProfit.Manufacturer == oldProfit.Manufacturer);
        }

        public void SetProfit<TProfit>(TProfit profit, IQueryable<TProfit> dbSet, Func<TProfit, TProfit, bool> where)
            where TProfit : IProfit

        {
            var customProfits = dbSet
                .AsEnumerable() // I know!
                .Where(r => where(profit, r))
                .Take(2)
                .ToList();

            if (customProfits.Count >= 2)
            {
                throw new DataException("We should not have two records.");
            }

            if (customProfits.Count == 1)
            {
                customProfits[0].Profit = profit.Profit;
                context.Update(customProfits[0]);
            }
            else
            {
                context.Add(profit);
            }

            context.SaveChanges();
        }

        public void RemoveCustomDefaultProfits(double profitDefaultValue)
        {
            RemoveDefaultProfits(context.CustomProfits, profitDefaultValue);
        }

        public void RemoveCategoryDefaultProfits(double profitDefaultValue)
        {
            RemoveDefaultProfits(context.CategoryProfits, profitDefaultValue);
        }

        public void RemoveManufacturersDefaultProfits(double profitDefaultValue)
        {
            RemoveDefaultProfits(context.ManufacturerProfits, profitDefaultValue);
        }

        private void RemoveDefaultProfits<T>(DbSet<T> dbSet, double profitDefaultValue) where T : class, IProfit
        {
            var profitsToRemove = dbSet
                .Where(r => r.Profit == profitDefaultValue)
                .ToList();

            context.RemoveRange(profitsToRemove);
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

        public void OverrideRenameManufacturerDictionary(IReadOnlyDictionary<string, string> newDictionary)
        {
            var dict = context.RenameManufacturer.ToDictionary(r => r.From);

            foreach (var from in newDictionary.Keys)
            {
                if (dict.ContainsKey(from))
                {
                    var toUpdate = dict[from];
                    toUpdate.To = newDictionary[from];
                    context.Update(toUpdate);
                }
                else
                {
                    context.RenameManufacturer.Add(new RenameManufacturerModel()
                    {
                        From = from,
                        To = newDictionary[from]
                    });
                }
            }

            foreach (var oldMapping in dict.Keys)
            {
                if (!newDictionary.ContainsKey(oldMapping))
                    context.Remove(dict[oldMapping]);
            }

            context.SaveChanges();
        }

        public IReadOnlyDictionary<string, string> GetRenameManufacturerDictionary()
        {
            var list = context.RenameManufacturer.ToDictionary(r => r.From, v => v.To);
            return list;
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
