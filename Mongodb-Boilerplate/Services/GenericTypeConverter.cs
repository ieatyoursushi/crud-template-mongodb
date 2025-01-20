using System.Collections;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Mongodb_Boilerplate.Services;

//allows the use of dynamically changing or adding a type to a generic
//ex: List<IDocument> -> List<ModelType>
//ex: Method<TypeVariable> TypeVariable more than likely being a model in this context
//limit to Idocuments maybe? (likely not)
public static class GenericTypeConverter
{
    public static IList listTypeConversion(IEnumerable list,Type listType)
    {
        IList changedList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(listType));
        foreach (var item in list)
        {
            changedList.Add(item);
        }

        return changedList;
    }
    //goal: randomMethod<TypeVariable>() / randomMethod<bsonDocumentType> in its first intended use
    //application: user inputting model type like a Book or Movie class. (has no connection to return type which can be generic or not)

    public static Task<object?> GenericTypeConversionAsync<T>(string methodName,Type bsonDocumentType, [FromQuery] object[] parameters)
    {
        return GenericTypeConversionAsync<T>(null, methodName, bsonDocumentType, parameters);
    }

    public static async Task<object?> GenericTypeConversionAsync<T>(object? classInstance,string methodName,Type bsonDocumentType, [FromQuery] object[] parameters)
    {
        MethodInfo method = typeof(T).GetMethod(methodName).MakeGenericMethod(bsonDocumentType);
        object? invokeInstance = method.IsStatic ? null : classInstance;
        var task = (Task)method.Invoke(invokeInstance, parameters); //fires the method
        await task.ConfigureAwait(false);
        PropertyInfo? resultProperty = task.GetType().GetProperty("Result");
        var document = resultProperty.GetValue(task); //gets the return value (if any)
        switch (document)
        {
            //scalable type condition checker and action
            case IDocument:
                return document;
            case IEnumerable<IDocument> documents: 
                return listTypeConversion(documents, bsonDocumentType);
            case null:
                return null;
            default:
                if (method.ReturnType == typeof(Task) || method.ReturnType == typeof(void))
                {
                    return null;
                }

                return document;
        }
    }
    public static object? GenericTypeConversion<T>(object? classInstance,string methodName,Type bsonDocumentType, [FromQuery] object[] parameters)
    {
        MethodInfo method = typeof(T).GetMethod(methodName).MakeGenericMethod(bsonDocumentType);
        object? invokeInstance = method.IsStatic ? null : classInstance;
        var document = method.Invoke(invokeInstance, parameters); //fires the method
 //gets the return value (if any)
        switch (document)
        {
            //scalable type condition checker and action
            case IDocument:
                return document;
            case IEnumerable<IDocument> documents:
                return listTypeConversion(documents, bsonDocumentType);
            case null:
                return null;
            default:
                if (method.ReturnType == typeof(Task) || method.ReturnType == typeof(void))
                {
                    return null;
                }
                return document;
        }
    }
    
}