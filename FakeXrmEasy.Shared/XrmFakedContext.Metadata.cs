using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using FakeXrmEasy.Extensions;
using System.Reflection;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using FakeXrmEasy.Metadata;

namespace FakeXrmEasy
{
    public partial class XrmFakedContext : IXrmContext
    {
        /// <summary>
        /// Stores some minimal metadata info if dynamic entities are used and no injected metadata was used
        /// </summary>
        protected internal Dictionary<string, Dictionary<string, string>> AttributeMetadataNames { get; set; }

        /// <summary>
        /// Stores fake global option set metadata
        /// </summary>
        public Dictionary<string, OptionSetMetadata> OptionSetValuesMetadata { get; set; }

        /// <summary>
        /// Stores fake global status values metadata
        /// </summary>
        public Dictionary<string, StatusAttributeMetadata> StatusAttributeMetadata { get; set; }

        /// <summary>
        /// Stores fake entity metadata
        /// </summary>
        protected internal Dictionary<string, EntityMetadata> EntityMetadata { get; set; }


        public void InitializeMetadata(IEnumerable<EntityMetadata> entityMetadataList)
        {
            if (entityMetadataList == null)
            {
                throw new Exception("Entity metadata parameter can not be null");
            }

            //  this.EntityMetadata = new Dictionary<string, EntityMetadata>();
            foreach (var eMetadata in entityMetadataList)
            {
                if (string.IsNullOrWhiteSpace(eMetadata.LogicalName))
                {
                    throw new Exception("An entity metadata record must have a LogicalName property.");
                }

                if (EntityMetadata.ContainsKey(eMetadata.LogicalName))
                {
                    throw new Exception("An entity metadata record with the same logical name was previously added. ");
                }
                EntityMetadata.Add(eMetadata.LogicalName, eMetadata.Copy());
            }

            // Auto-register relationships from metadata
            AutoRegisterRelationshipsFromMetadata(entityMetadataList);
        }

        public void InitializeMetadata(EntityMetadata entityMetadata)
        {
            this.InitializeMetadata(new List<EntityMetadata>() { entityMetadata });
        }

        /// <summary>
        /// Automatically registers relationships found in entity metadata
        /// This eliminates the need to manually call AddRelationship for each N:N and 1:N relationship
        /// </summary>
        private void AutoRegisterRelationshipsFromMetadata(IEnumerable<EntityMetadata> entityMetadataList)
        {
            foreach (var entityMetadata in entityMetadataList)
            {
                // Register Many-to-Many relationships
                if (entityMetadata.ManyToManyRelationships != null)
                {
                    foreach (var manyToMany in entityMetadata.ManyToManyRelationships)
                    {
                        if (string.IsNullOrEmpty(manyToMany.SchemaName) ||
                            string.IsNullOrEmpty(manyToMany.IntersectEntityName))
                        {
                            continue; // Skip incomplete relationship metadata
                        }

                        // Check if relationship already registered
                        if (Relationships.ContainsKey(manyToMany.SchemaName))
                        {
                            continue; // Already registered
                        }

                        var relationship = new XrmFakedRelationship
                        {
                            IntersectEntity = manyToMany.IntersectEntityName,
                            Entity1LogicalName = manyToMany.Entity1LogicalName,
                            Entity1Attribute = manyToMany.Entity1IntersectAttribute,
                            Entity2LogicalName = manyToMany.Entity2LogicalName,
                            Entity2Attribute = manyToMany.Entity2IntersectAttribute,
                            RelationshipType = XrmFakedRelationship.enmFakeRelationshipType.ManyToMany
                        };

                        AddRelationship(manyToMany.SchemaName, relationship);
                    }
                }

                // Register One-to-Many relationships
                if (entityMetadata.OneToManyRelationships != null)
                {
                    foreach (var oneToMany in entityMetadata.OneToManyRelationships)
                    {
                        if (string.IsNullOrEmpty(oneToMany.SchemaName))
                        {
                            continue; // Skip incomplete relationship metadata
                        }

                        // Check if relationship already registered
                        if (Relationships.ContainsKey(oneToMany.SchemaName))
                        {
                            continue; // Already registered
                        }

                        var relationship = new XrmFakedRelationship
                        {
                            Entity1LogicalName = oneToMany.ReferencedEntity,
                            Entity1Attribute = oneToMany.ReferencedAttribute,
                            Entity2LogicalName = oneToMany.ReferencingEntity,
                            Entity2Attribute = oneToMany.ReferencingAttribute,
                            RelationshipType = XrmFakedRelationship.enmFakeRelationshipType.OneToMany
                        };

                        AddRelationship(oneToMany.SchemaName, relationship);
                    }
                }

                // Register Many-to-One relationships
                if (entityMetadata.ManyToOneRelationships != null)
                {
                    foreach (var manyToOne in entityMetadata.ManyToOneRelationships)
                    {
                        if (string.IsNullOrEmpty(manyToOne.SchemaName))
                        {
                            continue; // Skip incomplete relationship metadata
                        }

                        // Check if relationship already registered
                        if (Relationships.ContainsKey(manyToOne.SchemaName))
                        {
                            continue; // Already registered
                        }

                        var relationship = new XrmFakedRelationship
                        {
                            Entity1LogicalName = manyToOne.ReferencedEntity,
                            Entity1Attribute = manyToOne.ReferencedAttribute,
                            Entity2LogicalName = manyToOne.ReferencingEntity,
                            Entity2Attribute = manyToOne.ReferencingAttribute,
                            RelationshipType = XrmFakedRelationship.enmFakeRelationshipType.OneToMany
                        };

                        AddRelationship(manyToOne.SchemaName, relationship);
                    }
                }
            }
        }

        public void InitializeMetadata(Assembly earlyBoundEntitiesAssembly)
        {
            IEnumerable<EntityMetadata> entityMetadatas = MetadataGenerator.FromEarlyBoundEntities(earlyBoundEntitiesAssembly);
            if (entityMetadatas.Any())
            {
                this.InitializeMetadata(entityMetadatas);
            }
        }

        public IQueryable<EntityMetadata> CreateMetadataQuery()
        {
            return this.EntityMetadata.Values
                    .Select(em => em.Copy())
                    .ToList()
                    .AsQueryable();
        }

        public EntityMetadata GetEntityMetadataByName(string sLogicalName)
        {
            if (EntityMetadata.ContainsKey(sLogicalName))
                return EntityMetadata[sLogicalName].Copy();

            return null;
        }

        public void SetEntityMetadata(EntityMetadata em)
        {
            if (this.EntityMetadata.ContainsKey(em.LogicalName))
                this.EntityMetadata[em.LogicalName] = em.Copy();
            else
                this.EntityMetadata.Add(em.LogicalName, em.Copy());
        }

        public AttributeMetadata GetAttributeMetadataFor(string sEntityName, string sAttributeName, Type attributeType)
        {
            if (EntityMetadata.ContainsKey(sEntityName))
            {
                var entityMetadata = GetEntityMetadataByName(sEntityName);
                var attribute = entityMetadata.Attributes
                                .Where(a => a.LogicalName.Equals(sAttributeName))
                                .FirstOrDefault();

                if (attribute != null)
                    return attribute;
            }

            if (attributeType == typeof(string))
            {
                return new StringAttributeMetadata(sAttributeName);
            }
            //Default
            return new StringAttributeMetadata(sAttributeName);
        }

    }
}