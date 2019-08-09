using Microsoft.ML.Probabilistic.Collections;
using Microsoft.ML.Probabilistic.Math;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BayesInferCore.Model
{
    public class CollectionAsObjectResolver : DefaultContractResolver
    {
        private static readonly HashSet<Type> SerializeAsObjectTypes = new HashSet<Type>{
            typeof(Vector),
            typeof(Matrix),
            typeof(IArray<>),
            typeof(ISparseList<>)
        };
        private static readonly ConcurrentDictionary<Type, JsonContract> ResolvedContracts = new ConcurrentDictionary<Type, JsonContract>();
        public override JsonContract ResolveContract(Type type) => ResolvedContracts.GetOrAdd(type, this.ResolveContractInternal);
        private JsonContract ResolveContractInternal(Type type) => IsExcludedType(type) ? this.CreateObjectContract(type) : this.CreateContract(type);
        private static bool IsExcludedType(Type type)
        {
            if (type == null) return false;
            if (SerializeAsObjectTypes.Contains(type)) return true;
            if (type.IsGenericType && SerializeAsObjectTypes.Contains(type.GetGenericTypeDefinition())) return true;
            return IsExcludedType(type.BaseType) || type.GetInterfaces().Any(IsExcludedType);
        }
    }
}