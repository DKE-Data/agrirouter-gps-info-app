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

namespace Agrirouter.Models.Abstract
{
    public abstract class MessageModel<T>
    {
        public Guid Id { get; set; }
        
        public DateTime CreatingDate { get; set; }
        
        public T Data { get; set; }
    }
}