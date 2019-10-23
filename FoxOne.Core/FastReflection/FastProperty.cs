using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxOne.Core
{
    public class FastProperty
    {
        private string _name;
        private PropertyInfo _prop;
        private Type _type;
        private Func<object, object> _getter;
        private Action<object, object[]> _setter;

        public FastProperty(string name, PropertyInfo property)
        {
            this._name = name;
            this._prop = property;
            this._type = _prop.PropertyType;
            this.InitializeGetter(_prop);
            this.InitilizeSetter(_prop);
        }

        public String Name
        {
            get { return _name; }
        }

        public Type Type
        {
            get { return _type; }
        }

        public PropertyInfo Info
        {
            get { return _prop; }
        }

        public object GetValue(object instance)
        {
            return _getter(instance);
        }

        public void SetValue(object instance, object value)
        {
            _setter(instance, new object[] { value });
        }

        private void InitializeGetter(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanRead) return;

            // Target: (object)(((TInstance)instance).Property)

            // preparing parameter, object type
            var instance = Expression.Parameter(typeof(object), "instance");

            // non-instance for static method, or ((TInstance)instance)
            var instanceCast = propertyInfo.GetGetMethod(true).IsStatic ? 
                                null : Expression.Convert(instance, propertyInfo.ReflectedType);

            // ((TInstance)instance).Property
            var propertyAccess = Expression.Property(instanceCast, propertyInfo);

            // (object)(((TInstance)instance).Property)
            var castPropertyValue = Expression.Convert(propertyAccess, typeof(object));

            // Lambda expression
            var lambda = Expression.Lambda<Func<object, object>>(castPropertyValue, instance);

            this._getter = lambda.Compile();
        }

        private void InitilizeSetter(PropertyInfo prop)
        {
            if (!prop.CanWrite) return;

            MethodInfo methodInfo = prop.GetSetMethod(true);
            // Target: ((TInstance)instance).Method((T0)parameters[0], (T1)parameters[1], ...)

            // parameters to execute
            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            // build parameter list
            var parameterExpressions = new List<Expression>();
            var paramInfos = methodInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                // (Ti)parameters[i]
                BinaryExpression valueObj = Expression.ArrayIndex(
                    parametersParameter, Expression.Constant(i));
                UnaryExpression valueCast = Expression.Convert(
                    valueObj, paramInfos[i].ParameterType);

                parameterExpressions.Add(valueCast);
            }

            // non-instance for static method, or ((TInstance)instance)
            var instanceCast = methodInfo.IsStatic ? null :
                                                              Expression.Convert(instanceParameter, methodInfo.ReflectedType);

            // static invoke or ((TInstance)instance).Method
            var methodCall = Expression.Call(instanceCast, methodInfo, parameterExpressions);

            // ((TInstance)instance).Method((T0)parameters[0], (T1)parameters[1], ...)
            var lambda = Expression.Lambda<Action<object, object[]>>(
                methodCall, instanceParameter, parametersParameter);
            _setter = lambda.Compile();
        }
    }
}