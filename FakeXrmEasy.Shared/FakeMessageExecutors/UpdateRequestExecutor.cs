using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;

namespace FakeXrmEasy.FakeMessageExecutors
{
    /// <summary>
    /// Handles the execution of <see cref="UpdateRequest"/> messages in the faked CRM context.
    /// This executor simulates updating existing entity records in Dynamics 365/CRM.
    /// </summary>
    /// <remarks>
    /// The executor modifies an existing entity record in the in-memory context based on
    /// the attributes provided in the Target entity. Only the attributes included in the
    /// Target are updated; other attributes on the existing record remain unchanged.
    /// </remarks>
    public class UpdateRequestExecutor : IFakeMessageExecutor
    {
        /// <summary>
        /// Determines whether this executor can handle the specified organization request.
        /// </summary>
        /// <param name="request">The <see cref="OrganizationRequest"/> to evaluate.</param>
        /// <returns>
        /// <c>true</c> if the request is an <see cref="UpdateRequest"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is UpdateRequest;
        }

        /// <summary>
        /// Executes the update operation, modifying an existing entity record in the faked CRM context.
        /// </summary>
        /// <param name="request">The <see cref="UpdateRequest"/> containing the entity with updated attributes.</param>
        /// <param name="ctx">The <see cref="XrmFakedContext"/> that provides the in-memory CRM simulation.</param>
        /// <returns>
        /// An <see cref="UpdateResponse"/> indicating successful completion of the update operation.
        /// </returns>
        /// <remarks>
        /// The Target property of the <see cref="UpdateRequest"/> must contain an entity with a valid Id
        /// and the attributes to be updated. The entity must already exist in the context.
        /// </remarks>
        public OrganizationResponse Execute(OrganizationRequest request, XrmFakedContext ctx)
        {
            var updateRequest = (UpdateRequest)request;

            var target = (Entity)request.Parameters["Target"];

            var service = ctx.GetOrganizationService();
            service.Update(target);

            return new UpdateResponse();
        }

        /// <summary>
        /// Gets the type of organization request that this executor handles.
        /// </summary>
        /// <returns>
        /// The <see cref="Type"/> of <see cref="UpdateRequest"/>.
        /// </returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(UpdateRequest);
        }
    }
}
