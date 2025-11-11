using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace FakeXrmEasy.FakeMessageExecutors
{
    /// <summary>
    /// Executor for CreateMultiple requests - optimized bulk create operation
    /// </summary>
    public class CreateMultipleRequestExecutor : IFakeMessageExecutor
    {
        public bool CanExecute(OrganizationRequest request)
        {
            return request is CreateMultipleRequest ||
                   (request != null && request.RequestName == "CreateMultiple");
        }

        public OrganizationResponse Execute(OrganizationRequest request, XrmFakedContext ctx)
        {
            var createMultipleRequest = request as CreateMultipleRequest;
            EntityCollection targets = null;

            // Handle both strongly-typed and loosely-typed requests
            if (createMultipleRequest != null)
            {
                targets = createMultipleRequest.Targets;
            }
            else if (request.Parameters.Contains("Targets"))
            {
                targets = (EntityCollection)request.Parameters["Targets"];
            }

            if (targets == null)
            {
                throw new FaultException<OrganizationServiceFault>(
                    new OrganizationServiceFault(),
                    "CreateMultipleRequest must contain a 'Targets' parameter with an EntityCollection.");
            }

            if (targets.Entities == null || targets.Entities.Count == 0)
            {
                throw new FaultException<OrganizationServiceFault>(
                    new OrganizationServiceFault(),
                    "CreateMultipleRequest 'Targets' collection cannot be empty.");
            }

            var service = ctx.GetOrganizationService();
            var createdIds = new List<Guid>();

            // CreateMultiple is transactional - all succeed or all fail
            // We'll simulate this by collecting all creates and only committing if all succeed
            try
            {
                foreach (var entity in targets.Entities)
                {
                    if (entity == null)
                    {
                        throw new FaultException<OrganizationServiceFault>(
                            new OrganizationServiceFault(),
                            "CreateMultipleRequest 'Targets' collection cannot contain null entities.");
                    }

                    var id = service.Create(entity);
                    createdIds.Add(id);
                }

                // All creates succeeded
                var response = new CreateMultipleResponse();
                response.Results["Ids"] = createdIds.ToArray();

                return response;
            }
            catch (Exception ex)
            {
                // In a real transactional scenario, this would rollback
                // For now, we'll just throw the exception
                // Note: FakeXrmEasy doesn't support full transaction rollback by default
                throw new FaultException<OrganizationServiceFault>(
                    new OrganizationServiceFault { Message = $"CreateMultiple operation failed: {ex.Message}" },
                    ex.Message);
            }
        }

        public Type GetResponsibleRequestType()
        {
            return typeof(CreateMultipleRequest);
        }
    }
}
