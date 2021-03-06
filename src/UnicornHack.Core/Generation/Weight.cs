﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CSharpScriptSerialization;
using UnicornHack.Systems.Levels;

namespace UnicornHack.Generation
{
    public abstract class Weight
    {
        public readonly int DefaultWeight = 1;

        protected static readonly ParameterExpression BranchParameter =
            Expression.Parameter(typeof(string), name: "branch");

        protected static readonly ParameterExpression
            DepthParameter = Expression.Parameter(typeof(byte), name: "depth");

        protected static readonly ParameterExpression InstancesParameter =
            Expression.Parameter(typeof(int), name: "instances");

        protected static readonly ParameterExpression TagInstancesParameter =
            Expression.Parameter(typeof(int), name: "tagInstances");

        protected static readonly ParameterExpression ConnectionParameter =
            Expression.Parameter(typeof(ConnectionComponent), name: "connection");

        public Func<string, byte, int, int, float> CreateFragmentWeightFunction()
        {
            var parameters = new[] {BranchParameter, DepthParameter, InstancesParameter, TagInstancesParameter};

            return Expression.Lambda<Func<string, byte, int, int, float>>(GetExpression(parameters), parameters)
                .Compile();
        }

        public Func<string, byte, int, int, ConnectionComponent, float> CreateConnectingFragmentWeightFunction()
        {
            var parameters = new[]
                {BranchParameter, DepthParameter, InstancesParameter, TagInstancesParameter, ConnectionParameter};

            return Expression
                .Lambda<Func<string, byte, int, int, ConnectionComponent, float>>(GetExpression(parameters), parameters)
                .Compile();
        }

        public Func<string, byte, int, float> CreateCreatureWeightFunction()
        {
            var parameters = new[] {BranchParameter, DepthParameter, InstancesParameter};

            return Expression.Lambda<Func<string, byte, int, float>>(GetExpression(parameters), parameters).Compile();
        }

        public Func<string, byte, int, float> CreateItemWeightFunction()
        {
            var parameters = new[] {BranchParameter, DepthParameter, InstancesParameter};

            return Expression.Lambda<Func<string, byte, int, float>>(GetExpression(parameters), parameters).Compile();
        }

        protected static void Check(ParameterExpression requiredParameter,
            IReadOnlyDictionary<string, ParameterExpression> parameters)
        {
            if (!parameters.ContainsKey(requiredParameter.Name))
            {
                throw new InvalidOperationException($"Parameter '{requiredParameter.Name}' not found.");
            }
        }

        private Expression GetExpression(IEnumerable<ParameterExpression> parameters)
        {
            var parameterLookup = new Dictionary<string, ParameterExpression>();

            foreach (var parameterExpression in parameters)
            {
                parameterLookup[parameterExpression.Name] = parameterExpression;
            }

            return GetExpression(parameterLookup);
        }

        public abstract Expression GetExpression(IReadOnlyDictionary<string, ParameterExpression> parameters);
    }

    public class DefaultWeight : Weight, ICSScriptSerializable
    {
        public float Multiplier { get; set; } = 1;

        public override Expression GetExpression(IReadOnlyDictionary<string, ParameterExpression> parameters) =>
            Expression.Multiply(Expression.Constant(Multiplier), Expression.Constant((float)DefaultWeight));

        private static readonly CSScriptSerializer Serializer = new PropertyCSScriptSerializer<DefaultWeight>(
            new Dictionary<string, Func<DefaultWeight, object, bool>>
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                {nameof(Multiplier), (o, v) => (float)v != 1}
            });

        public ICSScriptSerializer GetSerializer() => Serializer;
    }

    public class InfiniteWeight : Weight, ICSScriptSerializable
    {
        public override Expression GetExpression(IReadOnlyDictionary<string, ParameterExpression> parameters) =>
            Expression.Constant(float.PositiveInfinity);

        private static readonly CSScriptSerializer Serializer = new ConstructorCSScriptSerializer<InfiniteWeight>();
        public ICSScriptSerializer GetSerializer() => Serializer;
    }

    public class BranchWeight : Weight
    {
        public Weight Matched { get; set; }
        public Weight NotMatched { get; set; }
        public string Name { get; set; }
        public short MinDepth { get; set; }
        public short MaxDepth { get; set; }

        //TODO: Distribution

        public override Expression GetExpression(IReadOnlyDictionary<string, ParameterExpression> parameters)
        {
            Check(DepthParameter, parameters);
            Check(BranchParameter, parameters);

            var condition = Expression.Equal(Expression.Constant(Name), BranchParameter);

            // TODO: Negative depth
            if (MinDepth > 1)
            {
                condition = Expression.AndAlso(condition,
                    Expression.LessThanOrEqual(Expression.Constant((byte)MinDepth), DepthParameter));
            }

            if (MaxDepth >= 1)
            {
                condition = Expression.AndAlso(condition,
                    Expression.LessThanOrEqual(DepthParameter, Expression.Constant((byte)MaxDepth)));
            }

            return Expression.Condition(condition, Matched?.GetExpression(parameters) ?? Expression.Constant(0f),
                NotMatched?.GetExpression(parameters) ?? Expression.Constant(0f));
        }
    }

    public class InstancesWeight : Weight
    {
        public Weight W { get; set; }
        public int Max { get; set; }

        //TODO: Distribution
        //TODO: Scope: Game, Branch, Level

        public override Expression GetExpression(IReadOnlyDictionary<string, ParameterExpression> parameters)
        {
            Check(InstancesParameter, parameters);

            return Expression.Condition(Expression.LessThan(InstancesParameter, Expression.Constant(Max)),
                W.GetExpression(parameters), Expression.Constant(0f));
        }
    }

    public class TagInstancesWeight : Weight
    {
        public Weight W { get; set; }

        public string Name { get; set; }
        public int Max { get; set; }

        //TODO: Distribution
        //TODO: Scope: Game, Branch, Level

        public override Expression GetExpression(IReadOnlyDictionary<string, ParameterExpression> parameters)
        {
            Check(TagInstancesParameter, parameters);

            return Expression.Condition(Expression.LessThan(TagInstancesParameter, Expression.Constant(Max)),
                W.GetExpression(parameters), Expression.Constant(0f));
        }
    }

    public class MaxWeight : Weight, ICollection<Weight>
    {
        public IList<Weight> W { get; set; } = new List<Weight>();

        public void Add(Weight item) => W.Add(item);
        IEnumerator<Weight> IEnumerable<Weight>.GetEnumerator() => W.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)W).GetEnumerator();
        void ICollection<Weight>.Clear() => W.Clear();
        bool ICollection<Weight>.Contains(Weight item) => W.Contains(item);
        void ICollection<Weight>.CopyTo(Weight[] array, int arrayIndex) => W.CopyTo(array, arrayIndex);
        bool ICollection<Weight>.Remove(Weight item) => W.Remove(item);
        int ICollection<Weight>.Count => W.Count;
        bool ICollection<Weight>.IsReadOnly => W.IsReadOnly;

        private static readonly MethodInfo MaxMethod = typeof(Math).GetRuntimeMethods()
            .Single(m => m.Name == nameof(Math.Max) && m.ReturnParameter.ParameterType == typeof(float));

        public override Expression GetExpression(IReadOnlyDictionary<string, ParameterExpression> parameters)
        {
            if (W.Count < 2)
            {
                throw new InvalidOperationException();
            }

            var expression = W[0].GetExpression(parameters);
            for (var i = 1; i < W.Count; i++)
            {
                expression = Expression.Call(null, MaxMethod, expression, W[i].GetExpression(parameters));
            }

            return expression;
        }
    }

    public class MinWeight : Weight, ICollection<Weight>
    {
        public IList<Weight> W { get; set; } = new List<Weight>();

        public void Add(Weight item) => W.Add(item);
        IEnumerator<Weight> IEnumerable<Weight>.GetEnumerator() => W.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => W.GetEnumerator();
        void ICollection<Weight>.Clear() => W.Clear();
        bool ICollection<Weight>.Contains(Weight item) => W.Contains(item);
        void ICollection<Weight>.CopyTo(Weight[] array, int arrayIndex) => W.CopyTo(array, arrayIndex);
        bool ICollection<Weight>.Remove(Weight item) => W.Remove(item);
        int ICollection<Weight>.Count => W.Count;
        bool ICollection<Weight>.IsReadOnly => W.IsReadOnly;

        private static readonly MethodInfo MinMethod = typeof(Math).GetRuntimeMethods()
            .Single(m => m.Name == nameof(Math.Min) && m.ReturnParameter.ParameterType == typeof(float));

        public override Expression GetExpression(IReadOnlyDictionary<string, ParameterExpression> parameters)
        {
            if (W.Count < 2)
            {
                throw new InvalidOperationException();
            }

            var expression = W[0].GetExpression(parameters);
            for (var i = 1; i < W.Count; i++)
            {
                expression = Expression.Call(null, MinMethod, expression, W[i].GetExpression(parameters));
            }

            return expression;
        }
    }
}
