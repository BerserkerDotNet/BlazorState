using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BlazorState.Redux.Interfaces;

namespace BlazorState.Redux.Configuration
{
    public class ReducerMappingBuilder<TRootState>
        where TRootState : new()
    {
        private Dictionary<string, object> _reducerMaps = new Dictionary<string, object>(typeof(TRootState).GetProperties().Length);

        private ReducerMappingBuilder()
        {
        }

        public static ReducerMappingBuilder<TRootState> Create()
        {
            return new ReducerMappingBuilder<TRootState>();
        }

        public ReducerMappingBuilder<TRootState> Map<TProperty>(Expression<Func<TRootState, TProperty>> property, IReducer<TProperty> reducer)
        {
            var propertyName = GetProperty(property);
            _reducerMaps.Add(propertyName, (object)reducer);
            return this;
        }

        public ReducerMappingBuilder<TRootState> Map<TProperty>(Expression<Func<TRootState, TProperty>> property, Type reducer)
        {
            var propertyName = GetProperty(property);
            _reducerMaps.Add(propertyName, (IReducer<object>)reducer);
            return this;
        }

        public IReducer<TRootState> Build()
        {
            return new AggregateReducer<TRootState>(_reducerMaps);
        }

        private string GetProperty<TProperty>(Expression<Func<TRootState, TProperty>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            return memberExpression.Member.Name;
        }
    }
}
