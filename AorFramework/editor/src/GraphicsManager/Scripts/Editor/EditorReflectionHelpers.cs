using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class EditorReflectionHelpers
{

    /// <summary>Search the assembly for all types that match a predicate</summary>
    /// <param name="assembly">The assembly to search</param>
    /// <param name="predicate">The type to look for</param>
    /// <returns>A list of types found in the assembly that inherit from the predicate</returns>
    public static IEnumerable<Type> GetTypesInAssembly(
        Assembly assembly, Predicate<Type> predicate)
    {
        if (assembly == null)
            return null;

        Type[] types = new Type[0];
        try
        {
            types = assembly.GetTypes();
        }
        catch (Exception)
        {
            // Can't load the types in this assembly
        }
        types = (from t in types
                 where t != null && predicate(t)
                 select t).ToArray();
        return types;
    }

    /// <summary>Get a type from a name</summary>
    /// <param name="typeName">The name of the type to search for</param>
    /// <returns>The type matching the name, or null if not found</returns>
    public static Type GetTypeInAllLoadedAssemblies(string typeName)
    {
        foreach (Type type in GetTypesInAllLoadedAssemblies(t => t.Name == typeName))
            return type;
        return null;
    }

    /// <summary>Search all assemblies for all types that match a predicate</summary>
    /// <param name="predicate">The type to look for</param>
    /// <returns>A list of types found in the assembly that inherit from the predicate</returns>
    public static IEnumerable<Type> GetTypesInAllLoadedAssemblies(Predicate<Type> predicate)
    {
        Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        List<Type> foundTypes = new List<Type>(100);
        foreach (Assembly assembly in assemblies)
        {
            foreach (Type foundType in GetTypesInAssembly(assembly, predicate))
                foundTypes.Add(foundType);
        }
        return foundTypes;
    }

    /// <summary>call GetTypesInAssembly() for all assemblies that match a predicate</summary>
    /// <param name="assemblyPredicate">Which assemblies to search</param>
    /// <param name="predicate">What type to look for</param>
    public static IEnumerable<Type> GetTypesInLoadedAssemblies(
        Predicate<Assembly> assemblyPredicate, Predicate<Type> predicate)
    {
        Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        assemblies = assemblies.Where((Assembly assembly)
                => { return assemblyPredicate(assembly); }).OrderBy((Assembly ass)
                => { return ass.FullName; }).ToArray();

        List<Type> foundTypes = new List<Type>(100);
        foreach (Assembly assembly in assemblies)
        {
            foreach (Type foundType in GetTypesInAssembly(assembly, predicate))
                foundTypes.Add(foundType);
        }

        return foundTypes;
    }

    public static bool TypeIsDefined(string fullname)
    {
        return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where type.FullName == fullname
                select type).Count() > 0;
    }

}
