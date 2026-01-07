using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Linq;

namespace FakeXrmEasy.FakeMessageExecutors
{
    /// <summary>
    /// Fake message executor that handles <see cref="RetrieveAttributeRequest"/> messages.
    /// Retrieves attribute metadata for a specific entity attribute from the faked CRM context's metadata cache.
    /// </summary>
    public class RetrieveAttributeRequestExecutor : IFakeMessageExecutor
    {
        /// <summary>
        /// Determines whether this executor can handle the specified organization request.
        /// </summary>
        /// <param name="request">The organization request to evaluate.</param>
        /// <returns><c>true</c> if the request is a <see cref="RetrieveAttributeRequest"/>; otherwise, <c>false</c>.</returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is RetrieveAttributeRequest;
        }

        /// <summary>
        /// Executes the <see cref="RetrieveAttributeRequest"/> and returns the corresponding attribute metadata.
        /// </summary>
        /// <param name="request">The organization request to execute. Must be a <see cref="RetrieveAttributeRequest"/>.</param>
        /// <param name="ctx">The faked XRM context containing the entity and attribute metadata cache.</param>
        /// <returns>
        /// A <see cref="RetrieveAttributeResponse"/> containing the <see cref="Microsoft.Xrm.Sdk.Metadata.AttributeMetadata"/>
        /// for the requested attribute in the Results collection.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when the EntityLogicalName property is not provided,
        /// when the LogicalName property (attribute name) is not provided,
        /// when the entity metadata is not found in the cache,
        /// or when the attribute is not found in the entity's metadata.
        /// </exception>
        public OrganizationResponse Execute(OrganizationRequest request, XrmFakedContext ctx)
        {
            var req = request as RetrieveAttributeRequest;

            if (string.IsNullOrWhiteSpace(req.EntityLogicalName))
            {
                throw new Exception("The EntityLogicalName property must be provided in this request");
            }

            if (string.IsNullOrWhiteSpace(req.LogicalName))
            {
                throw new Exception("The LogicalName property must be provided in this request");
            }

            var entityMetadata = ctx.GetEntityMetadataByName(req.EntityLogicalName);
            if(entityMetadata == null)
            {
                throw new Exception(string.Format("The entity metadata with logical name {0} wasn't initialized. Please use .InitializeMetadata", req.EntityLogicalName));
            }

            if(entityMetadata.Attributes == null)
            {
                throw new Exception(string.Format("The attribute {0} wasn't found in entity metadata with logical name {1}. ", req.LogicalName, req.EntityLogicalName));
            }

            var attributeMetadata = entityMetadata.Attributes
                                    .FirstOrDefault(a => a.LogicalName.Equals(req.LogicalName));

            if (attributeMetadata == null)
            {
                throw new Exception(string.Format("The attribute {0} wasn't found in entity metadata with logical name {1}. ", req.LogicalName, req.EntityLogicalName));
            }

            var response = new RetrieveAttributeResponse()
            {
                Results = new ParameterCollection
                {
                    { "AttributeMetadata", attributeMetadata }
                }
            };

            return response;
        }

        /// <summary>
        /// Gets the type of organization request that this executor is responsible for handling.
        /// </summary>
        /// <returns>The <see cref="Type"/> of <see cref="RetrieveAttributeRequest"/>.</returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(RetrieveAttributeRequest);
        }
    }
}