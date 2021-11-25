/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */

using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace Agrirouter.Repositories.Abstract
{
    public abstract class Repository<T> : IRepository<T>
    {
        private string _key;

        public Repository()
        {
            _key = typeof(T).Name;
        }

        public void Clear()
        {
            SecureStorage.Remove(_key);
        }
        
        public async Task<T> GetAsync()
        {
            try
            {
                var value = await SecureStorage.GetAsync(_key);
                if (value is null)
                {
                    return Initialize();
                }
            
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception e)
            {
                return default;
            }
        }

        public async Task SetAsync(T value)
        {
            await SecureStorage.SetAsync(_key, JsonConvert.SerializeObject(value));
            OnDataChanged?.Invoke(this, new RepositoryDataChangedEventArgs(value));
        }

        public event EventHandler<RepositoryDataChangedEventArgs> OnDataChanged;

        public virtual T Initialize()
        {
            return default;
        }
    }
    
    public class RepositoryDataChangedEventArgs : EventArgs
    {
        public object NewData { get; }

        public RepositoryDataChangedEventArgs(object newData)
        {
            NewData = newData;
        }
    }
}