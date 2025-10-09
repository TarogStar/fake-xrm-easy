using Microsoft.Xrm.Sdk;
using System;
using System.Runtime.Serialization;

namespace FakeXrmEasy
{
    /// <summary>
    /// Holds custom properties of a IPluginExecutionContext
    /// Extracted from https://msdn.microsoft.com/es-es/library/microsoft.xrm.sdk.ipluginexecutioncontext_properties.aspx
    /// Implements IPluginExecutionContext4 for full D365 v9+ compatibility
    /// </summary>
    [DataContract(Name = "PluginExecutionContext", Namespace = "")]
    public class XrmFakedPluginExecutionContext : IPluginExecutionContext4
    {
        [DataMember(Order = 1)]
        public Guid BusinessUnitId { get; set; }

        [DataMember(Order = 2)]
        public Guid CorrelationId { get; set; }

        [DataMember(Order = 3)]
        public int Depth { get; set; }

        [DataMember(Order = 4)]
        public Guid InitiatingUserId { get; set; }

        [DataMember(Order = 5)]
        public ParameterCollection InputParameters { get; set; }

        [DataMember(Order = 6)]
        public bool IsExecutingOffline { get; set; }

        [DataMember(Order = 7)]
        public bool IsInTransaction
        {
            get
            {
                return Stage == (int)ProcessingStepStage.Preoperation || Stage == (int)ProcessingStepStage.Postoperation && Mode == (int)ProcessingStepMode.Synchronous;
            }
            set {  /* This property is writable only to correctly support serialization/deserialization */ }
        }

        [DataMember(Order = 8)]
        public bool IsOfflinePlayback { get; set; }

        [DataMember(Order = 9)]
        public int IsolationMode { get; set; }

        [DataMember(Order = 10)]
        public string MessageName { get; set; }

        [DataMember(Order = 11)]
        public int Mode { get; set; }

        [DataMember(Order = 12)]
        public DateTime OperationCreatedOn { get; set; }

        [DataMember(Order = 13)]
        public Guid OperationId { get; set; }

        [DataMember(Order = 14)]
        public Guid OrganizationId { get; set; }

        [DataMember(Order = 15)]
        public string OrganizationName { get; set; }

        [DataMember(Order = 16)]
        public ParameterCollection OutputParameters { get; set; }

        [DataMember(Order = 17)]
        public EntityReference OwningExtension { get; set; }

        [DataMember(Order = 18)]
        public EntityImageCollection PostEntityImages { get; set; }

        [DataMember(Order = 19)]
        public EntityImageCollection PreEntityImages { get; set; }

        [DataMember(Order = 20)]
        public Guid PrimaryEntityId { get; set; }

        [DataMember(Order = 21)]
        public string PrimaryEntityName { get; set; }

        [DataMember(Order = 22)]
        public Guid? RequestId { get; set; }

        [DataMember(Order = 23)]
        public string SecondaryEntityName { get; set; }

        [DataMember(Order = 24)]
        public ParameterCollection SharedVariables { get; set; }

        [DataMember(Order = 25)]
        public Guid UserId { get; set; }

        [DataMember(Order = 26)]
        public IPluginExecutionContext ParentContext { get; set; }

        [DataMember(Order = 27)]
        public int Stage { get; set; }

        // IPluginExecutionContext2 properties

        /// <summary>
        /// Gets the Azure Active Directory Object Id for the initiating user (IPluginExecutionContext2)
        /// </summary>
        [DataMember(Order = 28)]
        public Guid InitiatingUserAzureActiveDirectoryObjectId { get; set; }

        /// <summary>
        /// Gets the Azure Active Directory Object Id for the user (IPluginExecutionContext2)
        /// </summary>
        [DataMember(Order = 29)]
        public Guid UserAzureActiveDirectoryObjectId { get; set; }

        /// <summary>
        /// Gets the application ID of the initiating user (IPluginExecutionContext2)
        /// </summary>
        [DataMember(Order = 30)]
        public Guid InitiatingUserApplicationId { get; set; }

        /// <summary>
        /// Gets the Portals Contact ID (IPluginExecutionContext2)
        /// </summary>
        [DataMember(Order = 31)]
        public Guid PortalsContactId { get; set; }

        /// <summary>
        /// Indicates whether the call is from a Portals client (IPluginExecutionContext2)
        /// </summary>
        [DataMember(Order = 32)]
        public bool IsPortalsClientCall { get; set; }

        // IPluginExecutionContext3 properties

        /// <summary>
        /// Gets the authenticated user ID (IPluginExecutionContext3)
        /// </summary>
        [DataMember(Order = 33)]
        public Guid AuthenticatedUserId { get; set; }

        /// <summary>
        /// Gets a custom DataCollection of properties from the plugin step registration (IPluginExecutionContext3)
        /// </summary>
        [DataMember(Order = 34)]
        public ParameterCollection ParentContextProperties { get; set; }

        // IPluginExecutionContext4 properties

        /// <summary>
        /// Gets a value indicating whether the execution is happening in a transaction (IPluginExecutionContext4)
        /// </summary>
        [DataMember(Order = 35)]
        public bool IsTransactionIntegrationMessage { get; set; }

        /// <summary>
        /// Gets the collection of pre-entity images (IPluginExecutionContext4)
        /// </summary>
        [DataMember(Order = 36)]
        public EntityImageCollection[] PreEntityImagesCollection { get; set; }

        /// <summary>
        /// Gets the collection of post-entity images (IPluginExecutionContext4)
        /// </summary>
        [DataMember(Order = 37)]
        public EntityImageCollection[] PostEntityImagesCollection { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public XrmFakedPluginExecutionContext()
        {
            Depth = 1;
            IsExecutingOffline = false;
            MessageName = "Create"; //Default value,
            IsolationMode = 1;

            // IPluginExecutionContext2
            InitiatingUserAzureActiveDirectoryObjectId = Guid.Empty;
            UserAzureActiveDirectoryObjectId = Guid.Empty;
            InitiatingUserApplicationId = Guid.Empty;
            PortalsContactId = Guid.Empty;
            IsPortalsClientCall = false;

            // IPluginExecutionContext3
            AuthenticatedUserId = Guid.Empty;
            ParentContextProperties = new ParameterCollection();

            // IPluginExecutionContext4
            IsTransactionIntegrationMessage = false;
            PreEntityImagesCollection = new EntityImageCollection[0];
            PostEntityImagesCollection = new EntityImageCollection[0];
        }
    }
}