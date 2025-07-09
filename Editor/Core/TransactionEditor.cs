using System;
using System.Linq;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;

namespace Soar.Transactions
{
    [CustomEditor(typeof(Transaction), true)]
    [CanEditMultipleObjects]
    public class TransactionEditor : Editor
    {
        protected virtual string[] ExcludedProperties { get; } = { "m_Script" };

        private const float SpaceHeight = 15f;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            AddCustomButtons();
        }

        protected virtual void AddCustomButtons()
        {
            if (target is not Transaction transaction) return;
            
            GUILayout.Space(SpaceHeight);

            const string requestLabel = "Request";
            if (!GUILayout.Button(requestLabel)) return;
            
            var wasRegistered = transaction.IsResponseRegistered;
            if (!wasRegistered)
            {
                Debug.Log($"[{target.GetType().Name}:{target.name}] No response handler registered. Registering temporary empty handler.");
                transaction.RegisterResponse(() => { });
            }

            try
            {
                Debug.Log($"[{target.GetType().Name}:{target.name}] Transaction requested.");
                transaction.Request(() => Debug.Log($"[{target.GetType().Name}:{target.name}] Transaction responded."));
            }
            finally
            {
                if (!wasRegistered)
                {
                    Debug.Log($"[{target.GetType().Name}:{target.name}] Unregistering temporary response handler.");
                    transaction.UnregisterResponse();
                }

                // Mark the object as dirty to save changes in the editor.
                if (!Application.isPlaying)
                {
                    EditorUtility.SetDirty(transaction);
                }
            }
        }
    }

    [CustomEditor(typeof(Transaction<,>), true)]
    [CanEditMultipleObjects]
    public class ValueTransactionEditor : TransactionEditor
    {
        private const string RequestValuePropertyName = "requestValue";
        private const string ResponseValuePropertyName = "responseValue";

        protected override string[] ExcludedProperties => base.ExcludedProperties
            .Concat(new[] { RequestValuePropertyName, ResponseValuePropertyName }).ToArray();
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            DrawCustomProperties();
            AddCustomButtons();

            serializedObject.ApplyModifiedProperties();
        }
        
        protected override void AddCustomButtons()
        {
            base.AddCustomButtons();
            
            if (target is not Transaction transaction) return;

            const string requestLabel = "Value Request";
            if (!GUILayout.Button(requestLabel)) return;

            var requestValueProp = serializedObject.FindProperty(RequestValuePropertyName);
            var requestValue = requestValueProp.GetValue();
            var requestType = requestValue.GetType();
            
            var responseValueProp = serializedObject.FindProperty(ResponseValuePropertyName);
            var responseType = responseValueProp.GetValue().GetType();

            var wasRegistered = transaction.IsResponseRegistered;
            if (!wasRegistered)
            {
                RegisterTemporaryResponseHandler();
            }

            try
            {
                var requestMethod = transaction.GetType().GetMethod("Request",
                    new[] { requestType, typeof(Action<>).MakeGenericType(responseType) });
                if (requestMethod != null)
                {
                    Debug.Log($"[{target.GetType().Name}:{target.name}] Requesting transaction with requestValue: {requestValue}.");

                    var onResponse = CreateOnResponseDelegate();

                    requestMethod.Invoke(transaction, new[] { requestValue, onResponse });
                }
                else
                {
                    Debug.LogError("Could not find appropriate 'Request' method to call from the editor.");
                }
            }
            finally
            {
                // Unregister temporary response handler ONLY if we added one.
                if (!wasRegistered)
                {
                    Debug.Log($"[{target.GetType().Name}:{target.name}] Unregistering temporary response handler.");
                    transaction.UnregisterResponse();
                }

                if (!Application.isPlaying)
                {
                    // Mark the object as dirty in Edit mode to ensure changes get saved
                    EditorUtility.SetDirty(transaction);
                }
            }

            Delegate CreateOnResponseDelegate()
            {
                var actionType = typeof(Action<>).MakeGenericType(responseType);
                
                var param = Expression.Parameter(responseType);
                var call = Expression.Call(
                    Expression.Constant(this), // The caller, which is this editor instance
                    ((Action<object>)SetResponseValue).Method,
                    Expression.Convert(param, typeof(object)) // Cast the specific type to object
                );
                return Expression.Lambda(actionType, call, param).Compile();
            }
            
            void RegisterTemporaryResponseHandler()
            {
                // This handler takes the request and simply returns it as the response.
                Debug.Log($"[{target.GetType().Name}:{target.name}] No response handler registered. Registering temporary echo handler.");

                var funcType = typeof(Func<,>).MakeGenericType(requestType, responseType);
                var param = Expression.Parameter(requestType);

                // Create a lambda like: (request) => (TResponse)request;
                var body = Expression.Convert(param, responseType);
                var tempHandler = Expression.Lambda(funcType, body, param).Compile();

                // Find and invoke RegisterResponse(Func<TRequest, TResponse> func)
                var registerMethod = transaction.GetType().GetMethod("RegisterResponse", new[] { funcType });
                registerMethod?.Invoke(transaction, new object[] { tempHandler });
            }
        }
        
        private void SetResponseValue(object responseValue)
        {
            serializedObject.ApplyModifiedProperties();
            Debug.Log($"[{target.GetType().Name}:{target.name}] Transaction responded with responseValue: {responseValue}.");
        }

        private void DrawProperty(string propertyName)
        {
            using var value = serializedObject.FindProperty(propertyName);
            if (value.propertyType == SerializedPropertyType.Generic && !value.isArray)
            {
                foreach (var child in value.GetChildren())
                {
                    EditorGUILayout.PropertyField(child, true);
                }
            }
            else
            {
                EditorGUILayout.PropertyField(value, true);
            }
        }
        
        private void DrawCustomProperties()
        {
            DrawProperty(RequestValuePropertyName);
            DrawProperty(ResponseValuePropertyName);
            
            DrawPropertiesExcluding(serializedObject, ExcludedProperties);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
