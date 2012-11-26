﻿// <copyright file="ToolsetFactory.cs" company="Mark Final">
//  Opus
// </copyright>
// <summary>Opus Core</summary>
// <author>Mark Final</author>
namespace Opus.Core
{
    public static class ToolsetFactory
    {
        private static System.Collections.Generic.Dictionary<System.Type, IToolset> map = new System.Collections.Generic.Dictionary<System.Type, IToolset>();

        public static IToolset GetInstance(System.Type type)
        {
            if (map.ContainsKey(type))
            {
                return map[type];
            }

            if (!typeof(IToolset).IsAssignableFrom(type))
            {
                throw new Exception(System.String.Format("Type '{0}' does not implement the interface {1}", type.ToString(), typeof(IToolset).ToString()), false);
            }

            if (type.IsAbstract)
            {
                throw new Exception(System.String.Format("Type '{0}' is abstract", type.ToString()), false);
            }

            IToolset created = System.Activator.CreateInstance(type) as IToolset;
            map[type] = created;
            return created;
        }
    }
}