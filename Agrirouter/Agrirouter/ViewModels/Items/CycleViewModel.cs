/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium f�r Ern�hrung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */
namespace Agrirouter.ViewModels.Items
{
    public class CycleViewModel
    {
        public CycleViewModel(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        
        public int Value { get; }
    }
}