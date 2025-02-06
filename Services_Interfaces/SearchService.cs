using Inventory_System_API.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

public class SearchService
{
    private readonly DataContex _contex;

    public SearchService(DataContex contex)
    {
        _contex = contex;
    }

    public async Task<Dictionary<string, object>> DynamicSearchAsync(string query)
    {
        var results = new Dictionary<string, object>();

        // Get all DbSet properties
        var dbSets = _contex.GetType().GetProperties()
            .Where(p => p.PropertyType.IsGenericType &&
                       p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

        Console.WriteLine($"Found {dbSets.Count()} potential tables to search");

        foreach (var dbSet in dbSets)
        {
            try
            {
                var entityType = dbSet.PropertyType.GetGenericArguments()[0];
                Console.WriteLine($"Processing table: {entityType.Name}");

                var dbSetInstance = dbSet.GetValue(_contex) as IQueryable;
                if (dbSetInstance != null)
                {
                    // Get total count before filtering
                    var totalCount = await dbSetInstance.Cast<object>().CountAsync();
                    Console.WriteLine($"Total records in {entityType.Name}: {totalCount}");

                    var queryResults = await FilterDbSetAsync(dbSetInstance, entityType, query);
                    if (queryResults != null && queryResults.Any())
                    {
                        results[entityType.Name] = queryResults;
                        Console.WriteLine($"Found {queryResults.Count} matches in {entityType.Name}");
                    }
                    else
                    {
                        Console.WriteLine($"No matches found in {entityType.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {dbSet.Name}: {ex.Message}");
                continue;
            }
        }

        return results;
    }

    private async Task<List<object>> FilterDbSetAsync(IQueryable dbSet, Type entityType, string query)
    {
        try
        {
            var parameter = Expression.Parameter(entityType, "x");
            var predicates = new List<Expression>();

            // Get all string properties
            var stringProperties = entityType.GetProperties()
                .Where(p => p.PropertyType == typeof(string));

            Console.WriteLine($"Found {stringProperties.Count()} string properties to search in {entityType.Name}");

            foreach (var property in stringProperties)
            {
                try
                {
                    // Create case-insensitive contains expression
                    var propertyAccess = Expression.Property(parameter, property);

                    // Handle null properties
                    var nullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(string)));

                    // Convert both strings to uppercase for comparison
                    var toUpperMethod = typeof(string).GetMethod("ToUpper", Type.EmptyTypes);
                    var propertyUpper = Expression.Call(propertyAccess, toUpperMethod);
                    var queryUpper = Expression.Call(Expression.Constant(query), toUpperMethod);

                    // Create contains expression
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var containsExpression = Expression.Call(propertyUpper, containsMethod, queryUpper);

                    // Combine null check with contains
                    var safePredicate = Expression.AndAlso(nullCheck, containsExpression);

                    predicates.Add(safePredicate);
                    Console.WriteLine($"Added search predicate for property: {property.Name}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating predicate for {property.Name}: {ex.Message}");
                    continue;
                }
            }

            if (!predicates.Any())
            {
                Console.WriteLine($"No valid predicates created for {entityType.Name}");
                return null;
            }

            var combinedPredicate = predicates.Aggregate(Expression.OrElse);
            var lambda = Expression.Lambda(combinedPredicate, parameter);

            var whereCall = Expression.Call(
                typeof(Queryable),
                "Where",
                new[] { entityType },
                dbSet.Expression,
                lambda);

            var filteredQuery = dbSet.Provider.CreateQuery(whereCall);

            // Execute query and get results
            var results = await Task.Run(() => filteredQuery.Cast<object>().ToList());
            Console.WriteLine($"Query executed for {entityType.Name}, found {results.Count} results");
            return results;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error filtering {entityType.Name}: {ex.Message}");
            return null;
        }
    }
}