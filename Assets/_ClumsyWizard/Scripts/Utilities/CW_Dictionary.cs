using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ClumsyWizard.Utilities
{
    [Serializable]
    public struct Pair<TKey, TValue>
    {
        public TKey key;
        public TValue value;

        public Pair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }
    }

    [Serializable]
    public class CW_Dictionary<TKey, TValue> : ISerializationCallbackReceiver, IEnumerable
    {
        [SerializeField] private List<Pair<TKey, TValue>> entries = new List<Pair<TKey, TValue>>();

        //Keys
        private List<TKey> keys;
        public List<TKey> Keys => keys;

        //Values
        private List<TValue> values;
        public List<TValue> Values => values;

        //Helper Variables
        public int Count => keys.Count;
        public bool IsReadOnly => false;

        //Init
        public CW_Dictionary()
        {
            keys = new List<TKey>();
            values = new List<TValue>();
        }

        public CW_Dictionary(CW_Dictionary<TKey, TValue> copy)
        {
            entries = copy.entries.ToList();
            keys = copy.keys.ToList();
            values = copy.values.ToList();
        }

        //Serialization
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            keys.Clear();
            values.Clear();

            foreach (Pair<TKey, TValue> pair in entries)
            {
                keys.Add(pair.key);
                values.Add(pair.value);
            }
        }

        //Init

        public TValue this[TKey key]
        {
            get
            {
                if (keys.Contains(key))
                    return values[keys.IndexOf(key)];

                return default;
            }

            set
            {
                int index = keys.IndexOf(key);

                if(index < 0)
                {
                    Debug.LogError($"{key} does not exist in this Dictionary.");
                    return;
                }

                values[index] = value;

                entries[index] = new Pair<TKey, TValue>(key, value);
            }
        }

        //Dictionary Modification
        public void Add(TKey key, TValue value)
        {
            keys.Add(key);
            values.Add(value);

            entries.Add(new Pair<TKey, TValue>(key, value));
        }
        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);


        public bool Remove(TKey key)
        {
            var index = keys.IndexOf(key);

            if (index < 0) return false;

            keys.RemoveAt(index);
            values.RemoveAt(index);

            entries.RemoveAt(index);

            return true;
        }
        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        public void Clear()
        {
            keys.Clear();
            values.Clear();

            entries.Clear();
        }

        //Helper Functions
        public TKey GetKeyAt(int index)
        {
            if(index < keys.Count)
                return keys[index];

            return default;
        }
        public TValue GetValueAt(int index)
        {
            if (index < values.Count)
                return values[index];

            return default;
        }
        public bool Contains(KeyValuePair<TKey, TValue> item) => ContainsKey(item.Key);
        public bool ContainsKey(TKey key) => keys.Contains(key);

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (keys.Contains(key))
            {
                var index = keys.IndexOf(key);
                value = values[index];
                return true;
            }

            value = default;
            return false;
        }

        public void CopyTo(Pair<TKey, TValue>[] array, int startIndex)
        {
            entries.CopyTo(array, startIndex);
        }

        public IEnumerator GetEnumerator()
        {
            return entries.GetEnumerator();
        }
    }
}