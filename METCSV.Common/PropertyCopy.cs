using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METCSV.Common
{
    public static class PropertyCopy
    {

        public static bool AnyChanges<T>(T source, T target) where T : class
        {
            if (source == target)
                return false;
            
            var properties = source.GetType().GetProperties();

            foreach(var prop in properties)
            {
                var targetVal = prop.GetValue(target);
                var sourceVal = prop.GetValue(source);

                if (!sourceVal.Equals(targetVal))
                {
                    return true;
                }
            }

             return false;
        }

    }
}
