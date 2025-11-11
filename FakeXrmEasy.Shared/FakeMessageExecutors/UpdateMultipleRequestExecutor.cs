using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace FakeXrmEasy.FakeMessageExecutors
{
    /// <summary>
    /// Executor for UpdateMultiple requests - optimized bulk update operation
    /// </summary>
    public class UpdateMultipleRequestExecutor : IFakeMessageExecutor
    {
        public bool CanExecute(OrganizationRequest request)
        {
            return request is UpdateMultipleRequest ||
                   (request != null && request.RequestName == "UpdateMultiple");
        }

        public OrganizationResponse Execute(OrganizationRequest request, XrmFakedContext ctx)
        {
            var updateMultipleRequest = request as UpdateMultipleRequest;
            EntityCollection targets = null;

            // Handle both strongly-typed and loosely-typed requests
            if (updateMultipleRequest != null)
            {
                targets = updateMultipleRequest.Targets;
            }
            else if (request.Parameters.Contains("Targets"))
            {
                targets = (EntityCollection)request.Parameters["Targets"];
            }

            if (targets == null)
            {
                throw new FaultException<OrganizationServiceFault>(
                    new OrganizationServiceFault(),
                    "UpdateMultipleRequest must contain a 'Targets' parameter with an EntityCollection.");
            }

            if (targets.Entities == null || targets.Entities.Count == 0)
            {
                throw new FaultException<OrganizationServiceFault>(
                    new OrganizationServiceFault(),
                    "UpdateMultipleRequest 'Targets' collection cannot be empty.");
            }

            var service = ctx.GetOrganizationService();

            // UpdateMultiple is transactional - all succeed or all fail
            try
            {
                foreach (var entity in targets.Entities)
                {
                    if (entity == null)
                    {
                        throw new FaultException<OrganizationServiceFault>(
                            new OrganizationServiceFault(),
                            "UpdateMultipleRequest 'Targets' collection cannot contain null entities.");
                    }

                    if (entity.Id == Guid.Empty)
                    {
                        throw new FaultException<OrganizationServiceFault>(
                            new OrganizationServiceFault(),
                            "UpdateMultipleRequest requires all entities to have a valid Id.");
                    }

                    service.Update(entity);
                }

                // All updates succeeded
                return new UpdateMultipleResponse();
            }
            catch (Exception ex)
            {
                // In a real transactional scenario, this would rollback
                // For now, we'll just throw the exception
                throw new FaultException<OrganizationServiceFault>(
                    new OrganizationServiceFault { Message = $"UpdateMultiple operation failed: {ex.Message}" },
                    ex.Message);
            }
        }

        public Type GetResponsibleRequestType()
        {
            return typeof(UpdateMultipleRequest);
        }
    }
}
