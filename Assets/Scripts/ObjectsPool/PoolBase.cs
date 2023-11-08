using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.ObjectsPool
{
    public class PoolBase<T>
    {
        #region fields

        private readonly Func<T> _preloadFunc;
        private readonly Action<T> _getAction;
        private readonly Action<T> _returnAction;

        private Queue<T> _pool = new();
        private readonly List<T> _active = new();

        #endregion

        #region constructor

        public PoolBase(Func<T> preloadFunc, Action<T> getAction, Action<T> returnAction, int preloadCount)
        {
            _preloadFunc = preloadFunc;
            _getAction = getAction;
            _returnAction = returnAction;
     
            if (preloadFunc == null)
            {
                Debug.LogError("Preload func is null");
                return;
            }

            for (var i = 0; i < preloadCount; i++)
                Return(preloadFunc());
        }

        #endregion

        #region methods

        public void Add(T item)
        {
            if (item != null)
            {
                _pool.Enqueue(item);
                _returnAction(item);
                _active.Remove(item);
            }
            else
            {
                Debug.LogError("Cannot add a null item to the pool.");
            }
        }
        
        public T Get()
        {
            var item = _pool.Count > 0 ? _pool.Dequeue() : _preloadFunc();
            _getAction(item);
            _active.Add(item);

            return item;
        }

        public void Return(T item)
        {
            _returnAction(item);
            _pool.Enqueue(item);
            _active.Remove(item);
        }

        public bool HasItem(T item)
        {
            return _active.Contains(item);
        }

        public void ReturnAll()
        {
            foreach (var item in _active.ToArray()) Return(item);
        }

        #endregion
    }
}