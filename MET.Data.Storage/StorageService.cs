﻿using System;
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
                context.Update(customProfits[0]);
            }
            else
            {
                context.Add(customProfit);
            }

            context.SaveChanges();
        }

        public void RemoveCustomDefaultProfits(double profitDefaultValue)
        {
            var customProfits = context
                .CustomProfits
                .Where(r => r.Profit == profitDefaultValue)
                .ToList();

            context.RemoveRange(customProfits);
            context.SaveChanges();
        }

        public void RemoveCategoryDefaultProfits(double profitDefaultValue)
        {
            var categoryProfits = context
                .CategoryProfits
                .Where(r => r.Profit == profitDefaultValue)
                .ToList();

            context.RemoveRange(categoryProfits);
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
