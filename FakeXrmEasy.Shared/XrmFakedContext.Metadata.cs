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
    /// <summary>
    /// Partial class containing entity and attribute metadata functionality for the faked CRM context.
    /// Provides methods to initialize and query Dynamics 365 metadata for entity definitions,
    /// attributes, relationships, and option sets in an in-memory test environment.
    /// </summary>
    public partial class XrmFakedContext : IXrmContext
    {
        /// <summary>
        /// Gets or sets the dictionary storing attribute metadata names for dynamic entities.
        /// Used when no explicit metadata has been injected. Maps entity logical names to
        /// dictionaries of attribute logical names and their display names.
        /// </summary>
        protected internal Dictionary<string, Dictionary<string, string>> AttributeMetadataNames { get; set; }

        /// <summary>
        /// Gets or sets the dictionary storing fake global option set metadata.
        /// Maps option set names to their <see cref="OptionSetMetadata"/> definitions,
        /// including available options and their values.
        /// </summary>
        public Dictionary<string, OptionSetMetadata> OptionSetValuesMetadata { get; set; }

        /// <summary>
        /// Gets or sets the dictionary storing fake status attribute metadata.
        /// Maps entity logical names to their <see cref="StatusAttributeMetadata"/> definitions,
        /// which define the available status and state values for entities.
        /// </summary>
        public Dictionary<string, StatusAttributeMetadata> StatusAttributeMetadata { get; set; }

        /// <summary>
        /// Gets or sets the dictionary storing fake entity metadata.
        /// Maps entity logical names to their <see cref="EntityMetadata"/> definitions,
        /// including attributes, relationships, and other entity schema information.
        /// </summary>
        protected internal Dictionary<string, EntityMetadata> EntityMetadata { get; set; }


        /// <summary>
        /// Initializes the faked context with a collection of entity metadata definitions.
        /// Also automatically registers any relationships (Many-to-Many, One-to-Many, Many-to-One) found in the metadata.
        /// </summary>
        /// <param name="entityMetadataList">The collection of entity metadata definitions to initialize.</param>
        /// <exception cref="Exception">Thrown when entityMetadataList is null, an entity lacks a LogicalName, or a duplicate entity is added.</exception>
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

        /// <summary>
        /// Initializes the faked context with a single entity metadata definition.
        /// This is a convenience method that wraps the entity in a list and calls <see cref="InitializeMetadata(IEnumerable{EntityMetadata})"/>.
        /// </summary>
        /// <param name="entityMetadata">The entity metadata definition to initialize.</param>
        public void InitializeMetadata(EntityMetadata entityMetadata)
        {
            this.InitializeMetadata(new List<EntityMetadata>() { entityMetadata });
        }

        /// <summary>
        /// Automatically registers relationships found in entity metadata.
        /// This eliminates the need to manually call AddRelationship for each N:N and 1:N relationship.
        /// Processes Many-to-Many, One-to-Many, and Many-to-One relationships defined in the metadata.
        /// </summary>
        /// <param name="entityMetadataList">The collection of entity metadata containing relationship definitions.</param>
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

        /// <summary>
        /// Initializes the faked context with entity metadata generated from early-bound entity types in the specified assembly.
        /// Uses <see cref="MetadataGenerator"/> to reflect over the assembly and create metadata for all entity types found.
        /// </summary>
        /// <param name="earlyBoundEntitiesAssembly">The assembly containing early-bound entity classes decorated with appropriate attributes.</param>
        public void InitializeMetadata(Assembly earlyBoundEntitiesAssembly)
        {
            IEnumerable<EntityMetadata> entityMetadatas = MetadataGenerator.FromEarlyBoundEntities(earlyBoundEntitiesAssembly);
            if (entityMetadatas.Any())
            {
                this.InitializeMetadata(entityMetadatas);
            }
        }

        /// <summary>
        /// Creates a queryable collection of all entity metadata registered in this context.
        /// Returns copies of the metadata to prevent modification of the internal state.
        /// </summary>
        /// <returns>An <see cref="IQueryable{EntityMetadata}"/> containing copies of all registered entity metadata.</returns>
        public IQueryable<EntityMetadata> CreateMetadataQuery()
        {
            return this.EntityMetadata.Values
                    .Select(em => em.Copy())
                    .ToList()
                    .AsQueryable();
        }

        /// <summary>
        /// Retrieves the entity metadata for the specified entity logical name.
        /// Returns a copy of the metadata to prevent modification of the internal state.
        /// </summary>
        /// <param name="sLogicalName">The logical name of the entity to retrieve metadata for.</param>
        /// <returns>A copy of the <see cref="EntityMetadata"/> for the entity, or null if not found.</returns>
        public EntityMetadata GetEntityMetadataByName(string sLogicalName)
        {
            if (EntityMetadata.ContainsKey(sLogicalName))
                return EntityMetadata[sLogicalName].Copy();

            return null;
        }

        /// <summary>
        /// Sets or updates the entity metadata for a specific entity.
        /// If metadata already exists for the entity, it is replaced; otherwise, it is added.
        /// A copy of the metadata is stored to prevent external modification.
        /// </summary>
        /// <param name="em">The entity metadata to set or update.</param>
        public void SetEntityMetadata(EntityMetadata em)
        {
            if (this.EntityMetadata.ContainsKey(em.LogicalName))
                this.EntityMetadata[em.LogicalName] = em.Copy();
            else
                this.EntityMetadata.Add(em.LogicalName, em.Copy());
        }

        /// <summary>
        /// Retrieves the attribute metadata for a specific attribute of an entity.
        /// First checks if explicit metadata exists; if not, creates default metadata based on the attribute type.
        /// </summary>
        /// <param name="sEntityName">The logical name of the entity containing the attribute.</param>
        /// <param name="sAttributeName">The logical name of the attribute to retrieve metadata for.</param>
        /// <param name="attributeType">The CLR type of the attribute, used to create default metadata if explicit metadata is not found.</param>
        /// <returns>The <see cref="AttributeMetadata"/> for the attribute, or a default <see cref="StringAttributeMetadata"/> if not found.</returns>
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