using SQLite;
using SQLiteQueryBuilder.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SQLiteQueryBuilder
{
    internal static class Util
    {
        public static TAttribute GetPropertyAttribute<TAttribute>(PropertyInfo prop, bool inherit = true) where TAttribute : Attribute
        {
            var attr = prop.GetCustomAttribute<TAttribute>(inherit);

            return attr != null ? (attr as TAttribute) : default(TAttribute);
        }

        public static string[] GetDatabaseColumnsForType(Type EntityType, string alias = null)
        {
            List<string> ret = new List<string>();
            var props = EntityType.GetRuntimeProperties().Where(p => p.CanWrite);
            foreach (var prop in props)
                if (GetPropertyAttribute<IgnoreAttribute>(prop) == default(IgnoreAttribute) &&
                    GetPropertyAttribute<ManyToManyAttribute>(prop) == default(ManyToManyAttribute) &&
                    GetPropertyAttribute<OneToManyAttribute>(prop) == default(OneToManyAttribute) &&
                    GetPropertyAttribute<OneToOneAttribute>(prop) == default(OneToOneAttribute))
                    ret.Add((string.IsNullOrWhiteSpace(alias) ? string.Empty : (alias + ".")) + prop.Name);

            return ret.ToArray();
        }
        /*
         *   ESTE CÓDIGO SERÁ USADO NA CAMADA QUE IMPLEMENTA O REPOSITÓRIO RECURSIVO
         * 
        internal static string GetPrimaryKeyPropertyName(object orign)
        {
            string ret = null;
            var props = orign.GetType().GetRuntimeProperties();

            foreach (var prop in props)
                if (GetPropertyAttribute<PrimaryKeyAttribute>(prop) != null)
                {
                    ret = prop.Name;
                    break;
                }

            return ret;
        }

        internal static Relationship[] GetRelationships(object orign)
        {
            List<Relationship> ret = new List<Relationship>();
            var props = orign.GetType().GetRuntimeProperties().Where(p => p.CanWrite);
            foreach (var prop in props)
            {
                var oneToMany = GetPropertyAttribute<OneToManyAttribute>(prop);
                var oneToOne = GetPropertyAttribute<OneToOneAttribute>(prop);
                var manyToOne = GetPropertyAttribute<ManyToOneAttribute>(prop);

                // Se o relacionamento é para 1 item somente.
                if ((oneToOne != null || manyToOne != null))
                    ret.Add(new Relationship(prop.Name, prop.PropertyType, EnumRelationshipType.Single));

                // Quando é lista, verificar o tipo de atributo genérico é convertível para BaseEntity.
                if (oneToMany != null && prop.PropertyType.IsConstructedGenericType)
                {
                    var genericArgs = prop.PropertyType.GenericTypeArguments;
                    if (genericArgs?.Count() == 1)
                        ret.Add(new Relationship(prop.Name, genericArgs[0], EnumRelationshipType.Multiple));
                }
            }

            return ret.ToArray();
        }

        private static Type GetRelationshipPropertyType(object orign, string foreignEntityPropertyName)
        {
            Type ret = null;

            var prop = orign.GetType().GetRuntimeProperty(foreignEntityPropertyName);
            if (prop != null)
                ret = prop.PropertyType;

            return ret;
        }

        private static object GetForeignKeyValue(object orign, string foreignEntityPropertyName)
        {
            object ret = null;
            var prop = orign.GetType().GetRuntimeProperty(foreignEntityPropertyName);
            var pType = prop.PropertyType;

            string fkProp = GetForeignKeyPropertyName(orign, pType);
            if (!string.IsNullOrWhiteSpace(fkProp))
                ret = orign.GetType().GetRuntimeProperty(fkProp).GetValue(orign);

            return ret;
        }

        private static string GetForeignKeyPropertyName(object orign, Type foreignEntityPropertyType)
        {
            string ret = null;
            var props = orign.GetType().GetRuntimeProperties();

            foreach (var prop in props)
            {
                var fkAttr = GetPropertyAttribute<ForeignKeyAttribute>(prop);
                if (fkAttr != null && fkAttr.ForeignType == foreignEntityPropertyType)
                {
                    ret = prop.Name;
                    break;
                }
            }

            return ret;
        } 
        
        
        
        */
    }
}
