﻿using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using NetTopologySuite.Algorithm;
using System.Reflection;
using System.Linq.Dynamic.Core;
using CodeUI.Service.Attributes;
using System.Collections;
using CodeUI.Data.Entity;
using System.ComponentModel;

namespace CodeUI.Service.Utilities
{
    public static class Utils
    {
        public static string GenerateRandomCode(int length)
        {
            var randomCode = new Random();

            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[randomCode.Next(s.Length)]).ToArray());
        }

        public static string ToSnakeCase(this string o) => Regex.Replace(o, @"(\w)([A-Z])", "$1-$2").ToLower();

        public static DateTime GetCurrentDatetime()
        {
            return DateTime.UtcNow.AddHours(7);
        }

        public static bool CheckVNPhone(string phoneNumber)
        {
            string strRegex = @"(^(0)(3[2-9]|5[6|8|9]|7[0|6-9]|8[0-6|8|9]|9[0-4|6-9])[0-9]{7}$)";
            Regex re = new Regex(strRegex);
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            if (re.IsMatch(phoneNumber))
            {
                return true;
            }
            else
                return false;
        }

        public static (DateTime, DateTime) GetLastAndFirstDateInCurrentMonth()
        {
            var now = DateTime.Now;
            var first = new DateTime(now.Year, now.Month, 1);
            var last = first.AddMonths(1).AddDays(-1);
            return (first, last);
        }

        public static DateTime GetStartOfDate(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 0, 0, 0);
        }

        public static DateTime GetEndOfDate(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 23, 59, 59);
        }

        public static IQueryable<TEntity> DynamicFilter<TEntity>(this IQueryable<TEntity> source, TEntity entity)
        {
            var properties = entity.GetType().GetProperties();
            foreach (var item in properties)
            {
                if (entity.GetType().GetProperty(item.Name) == null) continue;
                var propertyVal = entity.GetType().GetProperty(item.Name).GetValue(entity, null);
                if (propertyVal == null) continue;
                if (item.CustomAttributes.Any(a => a.AttributeType == typeof(SkipAttribute))) continue;
                bool isDateTime = item.PropertyType == typeof(DateTime?);
                if (isDateTime)
                {
                    DateTime dt = (DateTime)propertyVal;
                    source = source.Where($"{item.Name} >= @0 && {item.Name} < @1", dt.Date, dt.Date.AddDays(1));
                }
                else if (item.PropertyType == typeof(int?) || item.PropertyType == typeof(int))
                {
                    source = source.Where($"{item.Name} == @{item.Name}", propertyVal);
                }
                else if (item.PropertyType == typeof(Guid?) || item.PropertyType == typeof(Guid))
                {
                    source = source.Where($"{item.Name} == @{item.Name}", propertyVal);
                }
                else if (item.CustomAttributes.Any(a => a.AttributeType == typeof(ContainAttribute)))
                {
                    var array = (IList)propertyVal;
                    source = source.Where($"{item.Name}.Any(a=> @0.Contains(a))", array);
                    //source = source.Where($"{item.Name}.Intersect({array}).Any()",);
                }
                else if (item.CustomAttributes.Any(a => a.AttributeType == typeof(ChildAttribute)))
                {
                    var childProperties = item.PropertyType.GetProperties();
                    foreach (var childProperty in childProperties)
                    {
                        var childPropertyVal = propertyVal.GetType().GetProperty(childProperty.Name)
                            .GetValue(propertyVal, null);
                        if (childPropertyVal != null && childProperty.PropertyType.Name.ToLower() == "string")
                        {
                            source = source.Where($"{item.Name}.{childProperty.Name} = \"{childPropertyVal}\"");
                        }
                        else if(childPropertyVal != null && (childProperty.PropertyType.Name.ToLower() == "boolean" || childProperty.PropertyType.Name.ToLower() == "nullable`1"))
                        {
                            source = source.Where($"{item.Name}.{childProperty.Name} = \"{childPropertyVal}\"");
                        }
                    }
                }
                else if (item.CustomAttributes.Any(a => a.AttributeType == typeof(ExcludeAttribute)))
                {
                    var childProperties = item.PropertyType.GetProperties();
                    var field = item.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(ExcludeAttribute))
                        .NamedArguments.FirstOrDefault().TypedValue.Value;
                    var array = ((List<int>)propertyVal).Cast<int?>();
                    source = source.Where($"!@0.Contains(it.{field})", array);

                }
                else if (item.CustomAttributes.Any(a => a.AttributeType == typeof(SortAttribute)))
                {
                    string[] sort = propertyVal.ToString().Split(", ");
                    if (sort.Length == 2)
                    {
                        if (sort[1].Equals("asc"))
                        {
                            source = source.OrderBy(sort[0]);
                        }

                        if (sort[1].Equals("desc"))
                        {
                            source = source.OrderBy(sort[0] + " descending");
                        }
                    }
                    else
                    {
                        source = source.OrderBy(sort[0]);
                    }
                }
                else if (item.PropertyType == typeof(string) || item.CustomAttributes.Any(a => a.AttributeType == typeof(StringAttribute)))
                {
                    source = source.Where($"{item.Name}.ToLower().Contains(@0)", propertyVal.ToString().ToLower());
                }
                else if (item.PropertyType == typeof(string))
                {
                    source = source.Where($"{item.Name} = \"{((string)propertyVal).Trim()}\"");
                }
                else
                {
                    source = source.Where($"{item.Name} = {propertyVal}");
                }
            }
            return source;
        }

        public static IQueryable<TEntity> DynamicSort<TEntity>(this IQueryable<TEntity> source, TEntity entity)
        {
            if (entity.GetType().GetProperties()
                    .Any(x => x.CustomAttributes.Any(a => a.AttributeType == typeof(SortDirectionAttribute))) &&
                entity.GetType().GetProperties()
                    .Any(x => x.CustomAttributes.Any(a => a.AttributeType == typeof(SortPropertyAttribute))))
            {
                var sortDirection = entity.GetType().GetProperties().SingleOrDefault(x =>
                        x.CustomAttributes.Any(a => a.AttributeType == typeof(SortDirectionAttribute)))?
                    .GetValue(entity, null);
                var sortBy = entity.GetType().GetProperties().SingleOrDefault(x =>
                        x.CustomAttributes.Any(a => a.AttributeType == typeof(SortPropertyAttribute)))?
                    .GetValue(entity, null);

                if (sortDirection != null && sortBy != null)
                {
                    if ((string)sortDirection == "asc")
                    {
                        source = source.OrderBy((string)sortBy);
                    }
                    else if ((string)sortDirection == "desc")
                    {
                        source = source.OrderBy((string)sortBy + " descending");
                    }
                }
            }

            return source;
        }

        public static IQueryable<T> Shuffle<T>(this IList<T> list, string? seed = "")
        {
            Random rng = new Random(seed.GetHashCode());
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list.AsQueryable();
        }

        public static (int, IQueryable<TResult>) PagingQueryable<TResult>(this IQueryable<TResult> source, int page,
           int size, int limitPaging = 50, int defaultPaging = 1)
        {
            if (size > limitPaging)
            {
                size = limitPaging;
            }

            if (size < 1)
            {
                size = defaultPaging;
            }

            if (page < 1)
            {
                page = 1;
            }

            int total = source.Count();
            IQueryable<TResult> results = source.Skip((page - 1) * size).Take(size);
            return (total, results);
        }
    }
}
