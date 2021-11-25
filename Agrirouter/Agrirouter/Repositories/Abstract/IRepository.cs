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

namespace Agrirouter.Repositories.Abstract
{
    public interface IRepository<T>
    {
        Task<T> GetAsync();

        Task SetAsync(T value);

        void Clear();
        
        event EventHandler<RepositoryDataChangedEventArgs> OnDataChanged;
    }
}