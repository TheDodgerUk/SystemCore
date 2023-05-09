using System;
using System.IO;
using UnityEngine;

namespace JsonPlugin
{
    public abstract class JsonLibrary
    {
        public T ReadFromFile<T>(string filePath, bool displayError = true) where T : class
        {
            return ReadFromText<T>(ReadFile(filePath, displayError));
        }
        public object ReadFromFile(string filePath, Type type)
        {
            return ReadFromText(ReadFile(filePath), type);
        }
        public bool FillFromFile(string filePath, object obj)
        {
            return FillFromText(ReadFile(filePath), obj);
        }

        public T ReadFromText<T>(string jsonText)
        {
            return TryDeserialise(typeof(T), jsonText, () => DeserialiseData<T>(jsonText));
        }
        public object ReadFromText(string jsonText, Type type)
        {
            return TryDeserialise(type, jsonText, () => DeserialiseData(jsonText, type));
        }
        public bool FillFromText(string jsonText, object obj)
        {
            return TryDeserialise(obj.GetType(), jsonText, () => DeserialiseData(jsonText, obj));
        }

        public void ReadFromFileAsync<T>(string filePath, Action<T> callback) where T : class
        {
            Arbiter.CreateJobWithResult(() => ReadFromFile<T>(filePath), callback);
        }

        public void ReadFromFileAsyncNoErrorMessage<T>(string filePath, Action<T> callback) where T : class
        {
            Arbiter.CreateJobWithResult(() => ReadFromFile<T>(filePath, false), callback);
        }

        public void ReadFromFileAsync(string filePath, Type type, Action<object> callback)
        {
            Arbiter.CreateJobWithResult(() => ReadFromFile(filePath, type), callback);
        }

        public void ReadFromTextAsync<T>(string jsonText, Action<T> callback)
        {
            Arbiter.CreateJobWithResult(() => ReadFromText<T>(jsonText), callback);
        }
        public void ReadFromTextAsync(string jsonText, Type type, Action<object> callback)
        {
            Arbiter.CreateJobWithResult(() => ReadFromText(jsonText, type), callback);
        }

        public void WriteToFile<T>(T data, string filePath, bool prettyPrint)
        {
            filePath = AppendExtension(filePath);
            string jsonText = WriteToText(data, prettyPrint);

            try
            {
                Utils.IO.ForceFolder(Path.GetDirectoryName(filePath));
                File.WriteAllText(filePath, jsonText);
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to write text to path \"{filePath}\"\n\"{e.Message}\" text: {jsonText}");
            }
        }

        public string WriteToText<T>(T data, bool prettyPrint)
        {
            try
            {
                return SerialiseData(data, prettyPrint);
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to serialise JSON to type {{{typeof(T).Name}}}.\n\"{e.Message}\" from data:\n{data.ToString()}");
            }
            return null;
        }

        protected abstract T DeserialiseData<T>(string jsonText);
        protected abstract object DeserialiseData(string jsonText, Type type);
        protected abstract bool DeserialiseData(string jsonText, object obj);

        protected abstract string SerialiseData<T>(T data, bool prettyPrint);

        private static T TryDeserialise<T>(Type type, string text, Func<T> deserialise)
        {
            try
            {
                if (string.IsNullOrEmpty(text) == false)
                {
                    return deserialise();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse JSON to {{{typeof(T).Name}}} from:\n{text}   \n {e.Message}");
                Debug.LogException(e);
            };
            return default(T);
        }

        private static string ReadFile(string filePath, bool displayError = true) => Utils.IO.ReadFromFile(AppendExtension(filePath), displayError);

        private static string AppendExtension(string filePath)
        {
            filePath = filePath.SanitiseSlashes();

            int index = filePath.IndexOf(".");
            if (index >= 0)
            {
                filePath = filePath.Substring(0, index);
            }

            filePath += ".json";
            return filePath;
        }
    }
}
