using FakeXrmEasy.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using Xunit;

namespace FakeXrmEasy.Tests.FakeContextTests.AlternateKeyTests
{
#if !FAKE_XRM_EASY && !FAKE_XRM_EASY_2013 && !FAKE_XRM_EASY_2015
    /// <summary>
    /// Tests for Issue #521 - Composite Alternate Keys + Uniqueness Enforcement
    /// </summary>
    public class CompositeAlternateKeyTests
    {
        #region Create Tests

        [Fact]
        public void Create_Should_Throw_When_Composite_Key_Already_Exists()
        {
            // Arrange
            var context = new XrmFakedContext();

            // Set up composite alternate key for contact (firstname + lastname)
            context.AddAlternateKey("contact", new[] { "firstname", "lastname" });

            // Create the first contact
            var existingContact = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                ["firstname"] = "John",
                ["lastname"] = "Doe"
            };
            context.Initialize(new List<Entity> { existingContact });

            var service = context.GetOrganizationService();

            // Create a new contact with the same composite key
            var duplicateContact = new Entity("contact")
            {
                ["firstname"] = "John",
                ["lastname"] = "Doe"
            };

            // Act & Assert
            var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(() =>
                service.Create(duplicateContact));

            Assert.Contains("already exists", ex.Message);
            Assert.Contains("firstname", ex.Message);
            Assert.Contains("lastname", ex.Message);
        }

        [Fact]
        public void Create_Should_Succeed_When_One_Key_Attribute_Differs()
        {
            // Arrange
            var context = new XrmFakedContext();

            // Set up composite alternate key for contact (firstname + lastname)
            context.AddAlternateKey("contact", new[] { "firstname", "lastname" });

            // Create the first contact
            var existingContact = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                ["firstname"] = "John",
                ["lastname"] = "Doe"
            };
            context.Initialize(new List<Entity> { existingContact });

            var service = context.GetOrganizationService();

            // Create a new contact with different lastname (one attribute differs)
            var newContact = new Entity("contact")
            {
                ["firstname"] = "John",
                ["lastname"] = "Smith"  // Different lastname
            };

            // Act
            var newId = service.Create(newContact);

            // Assert
            Assert.NotEqual(Guid.Empty, newId);
            Assert.NotEqual(existingContact.Id, newId);
        }

        [Fact]
        public void Create_Should_Succeed_When_All_Key_Attributes_Differ()
        {
            // Arrange
            var context = new XrmFakedContext();

            // Set up composite alternate key for contact (firstname + lastname)
            context.AddAlternateKey("contact", new[] { "firstname", "lastname" });

            // Create the first contact
            var existingContact = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                ["firstname"] = "John",
                ["lastname"] = "Doe"
            };
            context.Initialize(new List<Entity> { existingContact });

            var service = context.GetOrganizationService();

            // Create a completely different contact
            var newContact = new Entity("contact")
            {
                ["firstname"] = "Jane",
                ["lastname"] = "Smith"
            };

            // Act
            var newId = service.Create(newContact);

            // Assert
            Assert.NotEqual(Guid.Empty, newId);
            Assert.NotEqual(existingContact.Id, newId);
        }

        [Fact]
        public void Create_Should_Succeed_When_Key_Attribute_Is_Null()
        {
            // Arrange
            var context = new XrmFakedContext();

            // Set up composite alternate key for contact (firstname + lastname)
            context.AddAlternateKey("contact", new[] { "firstname", "lastname" });

            // Create the first contact
            var existingContact = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                ["firstname"] = "John",
                ["lastname"] = "Doe"
            };
            context.Initialize(new List<Entity> { existingContact });

            var service = context.GetOrganizationService();

            // Create a new contact with null lastname
            var newContact = new Entity("contact")
            {
                ["firstname"] = "John"
                // lastname is null - should not trigger uniqueness check
            };

            // Act
            var newId = service.Create(newContact);

            // Assert - should succeed because null values don't participate in key check
            Assert.NotEqual(Guid.Empty, newId);
        }

        [Fact]
        public void Create_Should_Throw_For_Single_Attribute_Key()
        {
            // Arrange
            var context = new XrmFakedContext();

            // Set up single-attribute alternate key for account (accountnumber)
            context.AddAlternateKey("account", "accountnumber");

            // Create the first account
            var existingAccount = new Entity("account")
            {
                Id = Guid.NewGuid(),
                ["accountnumber"] = "ACC001"
            };
            context.Initialize(new List<Entity> { existingAccount });

            var service = context.GetOrganizationService();

            // Create an account with the same accountnumber
            var duplicateAccount = new Entity("account")
            {
                ["accountnumber"] = "ACC001"
            };

            // Act & Assert
            var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(() =>
                service.Create(duplicateAccount));

            Assert.Contains("already exists", ex.Message);
            Assert.Contains("accountnumber", ex.Message);
        }

        #endregion

        #region Update Tests

        [Fact]
        public void Update_Should_Throw_When_Change_Creates_Duplicate_Key()
        {
            // Arrange
            var context = new XrmFakedContext();

            // Set up composite alternate key for contact (firstname + lastname)
            context.AddAlternateKey("contact", new[] { "firstname", "lastname" });

            // Create two contacts with different keys
            var contact1 = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                ["firstname"] = "John",
                ["lastname"] = "Doe"
            };
            var contact2 = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                ["firstname"] = "Jane",
                ["lastname"] = "Smith"
            };
            context.Initialize(new List<Entity> { contact1, contact2 });

            var service = context.GetOrganizationService();

            // Try to update contact2 to have the same key as contact1
            var updateContact = new Entity("contact")
            {
                Id = contact2.Id,
                ["firstname"] = "John",
                ["lastname"] = "Doe"  // Now matches contact1's key
            };

            // Act & Assert
            var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(() =>
                service.Update(updateContact));

            Assert.Contains("already exists", ex.Message);
        }

        [Fact]
        public void Update_Should_Succeed_When_Updating_Non_Key_Attributes()
        {
            // Arrange
            var context = new XrmFakedContext();

            // Set up composite alternate key for contact (firstname + lastname)
            context.AddAlternateKey("contact", new[] { "firstname", "lastname" });

            var contact = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                ["firstname"] = "John",
                ["lastname"] = "Doe",
                ["emailaddress1"] = "john@example.com"
            };
            context.Initialize(new List<Entity> { contact });

            var service = context.GetOrganizationService();

            // Update non-key attribute
            var updateContact = new Entity("contact")
            {
                Id = contact.Id,
                ["emailaddress1"] = "john.doe@example.com"
            };

            // Act & Assert - should not throw
            service.Update(updateContact);

            // Verify the update
            var updated = context.Data["contact"][contact.Id];
            Assert.Equal("john.doe@example.com", updated["emailaddress1"]);
        }

        [Fact]
        public void Update_Should_Succeed_When_Same_Record_Has_Same_Key()
        {
            // Arrange
            var context = new XrmFakedContext();

            // Set up composite alternate key for contact (firstname + lastname)
            context.AddAlternateKey("contact", new[] { "firstname", "lastname" });

            var contact = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                ["firstname"] = "John",
                ["lastname"] = "Doe"
            };
            context.Initialize(new List<Entity> { contact });

            var service = context.GetOrganizationService();

            // Update with the same key values (should succeed - same record)
            var updateContact = new Entity("contact")
            {
                Id = contact.Id,
                ["firstname"] = "John",
                ["lastname"] = "Doe",
                ["emailaddress1"] = "john@example.com"
            };

            // Act & Assert - should not throw
            service.Update(updateContact);
        }

        [Fact]
        public void Update_Should_Succeed_When_Partial_Key_Update_Is_Unique()
        {
            // Arrange
            var context = new XrmFakedContext();

            // Set up composite alternate key for contact (firstname + lastname)
            context.AddAlternateKey("contact", new[] { "firstname", "lastname" });

            var contact1 = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                ["firstname"] = "John",
                ["lastname"] = "Doe"
            };
            var contact2 = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                ["firstname"] = "Jane",
                ["lastname"] = "Doe"
            };
            context.Initialize(new List<Entity> { contact1, contact2 });

            var service = context.GetOrganizationService();

            // Update contact1's firstname only - new key (Mike, Doe) should be unique
            var updateContact = new Entity("contact")
            {
                Id = contact1.Id,
                ["firstname"] = "Mike"
            };

            // Act & Assert - should not throw
            service.Update(updateContact);

            // Verify the update
            var updated = context.Data["contact"][contact1.Id];
            Assert.Equal("Mike", updated["firstname"]);
            Assert.Equal("Doe", updated["lastname"]);
        }

        #endregion

        #region Multiple Entity Types Tests

        [Fact]
        public void Multiple_Entity_Types_With_Different_Keys_Should_Work_Independently()
        {
            // Arrange
            var context = new XrmFakedContext();

            // Set up different alternate keys for different entity types
            context.AddAlternateKey("contact", new[] { "firstname", "lastname" });
            context.AddAlternateKey("account", "accountnumber");

            var contact = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                ["firstname"] = "John",
                ["lastname"] = "Doe"
            };
            var account = new Entity("account")
            {
                Id = Guid.NewGuid(),
                ["accountnumber"] = "ACC001",
                ["name"] = "John Doe Corp"  // Using same values doesn't conflict with contact
            };
            context.Initialize(new List<Entity> { contact, account });

            var service = context.GetOrganizationService();

            // Create another account with different key
            var newAccount = new Entity("account")
            {
                ["accountnumber"] = "ACC002"  // Different key
            };

            // Act
            var newAccountId = service.Create(newAccount);

            // Assert
            Assert.NotEqual(Guid.Empty, newAccountId);

            // But duplicate account number should fail
            var duplicateAccount = new Entity("account")
            {
                ["accountnumber"] = "ACC001"
            };

            var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(() =>
                service.Create(duplicateAccount));

            Assert.Contains("already exists", ex.Message);
        }

        #endregion

        #region AddAlternateKey Helper Method Tests

        [Fact]
        public void AddAlternateKey_Should_Throw_When_EntityLogicalName_Is_Null()
        {
            // Arrange
            var context = new XrmFakedContext();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                context.AddAlternateKey(null, new[] { "attr1" }));
        }

        [Fact]
        public void AddAlternateKey_Should_Throw_When_EntityLogicalName_Is_Empty()
        {
            // Arrange
            var context = new XrmFakedContext();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                context.AddAlternateKey("", new[] { "attr1" }));
        }

        [Fact]
        public void AddAlternateKey_Should_Throw_When_KeyAttributes_Is_Null()
        {
            // Arrange
            var context = new XrmFakedContext();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                context.AddAlternateKey("contact", (string[])null));
        }

        [Fact]
        public void AddAlternateKey_Should_Throw_When_KeyAttributes_Is_Empty()
        {
            // Arrange
            var context = new XrmFakedContext();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                context.AddAlternateKey("contact", Array.Empty<string>()));
        }

        [Fact]
        public void AddAlternateKey_Should_Create_Metadata_If_Not_Exists()
        {
            // Arrange
            var context = new XrmFakedContext();

            // Act
            context.AddAlternateKey("newentity", new[] { "attr1", "attr2" });

            // Assert
            var metadata = context.GetEntityMetadataByName("newentity");
            Assert.NotNull(metadata);
            Assert.NotNull(metadata.Keys);
            Assert.Single(metadata.Keys);
            Assert.Equal(new[] { "attr1", "attr2" }, metadata.Keys[0].KeyAttributes);
        }

        [Fact]
        public void AddAlternateKey_Should_Add_Multiple_Keys()
        {
            // Arrange
            var context = new XrmFakedContext();

            // Act - Add two different alternate keys
            context.AddAlternateKey("contact", new[] { "firstname", "lastname" });
            context.AddAlternateKey("contact", new[] { "emailaddress1" });

            // Assert
            var metadata = context.GetEntityMetadataByName("contact");
            Assert.NotNull(metadata);
            Assert.NotNull(metadata.Keys);
            Assert.Equal(2, metadata.Keys.Length);
        }

        [Fact]
        public void AddAlternateKey_Single_Attribute_Overload_Should_Work()
        {
            // Arrange
            var context = new XrmFakedContext();

            // Act
            context.AddAlternateKey("account", "accountnumber");

            // Assert
            var metadata = context.GetEntityMetadataByName("account");
            Assert.NotNull(metadata);
            Assert.NotNull(metadata.Keys);
            Assert.Single(metadata.Keys);
            Assert.Single(metadata.Keys[0].KeyAttributes);
            Assert.Equal("accountnumber", metadata.Keys[0].KeyAttributes[0]);
        }

        #endregion

        #region SDK Type Comparison Tests

        [Fact]
        public void Create_Should_Handle_EntityReference_Key_Attribute()
        {
            // Arrange
            var context = new XrmFakedContext();

            // Set up alternate key with EntityReference attribute
            context.AddAlternateKey("contact", new[] { "parentcustomerid", "lastname" });

            var accountId = Guid.NewGuid();
            var existingContact = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                ["parentcustomerid"] = new EntityReference("account", accountId),
                ["lastname"] = "Doe"
            };
            context.Initialize(new List<Entity> { existingContact });

            var service = context.GetOrganizationService();

            // Create a duplicate with same EntityReference key
            var duplicateContact = new Entity("contact")
            {
                ["parentcustomerid"] = new EntityReference("account", accountId),
                ["lastname"] = "Doe"
            };

            // Act & Assert
            var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(() =>
                service.Create(duplicateContact));

            Assert.Contains("already exists", ex.Message);
        }

        [Fact]
        public void Create_Should_Handle_OptionSetValue_Key_Attribute()
        {
            // Arrange
            var context = new XrmFakedContext();

            // Set up alternate key with OptionSetValue attribute
            context.AddAlternateKey("contact", new[] { "gendercode", "lastname" });

            var existingContact = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                ["gendercode"] = new OptionSetValue(1),
                ["lastname"] = "Doe"
            };
            context.Initialize(new List<Entity> { existingContact });

            var service = context.GetOrganizationService();

            // Create a duplicate with same OptionSetValue key
            var duplicateContact = new Entity("contact")
            {
                ["gendercode"] = new OptionSetValue(1),
                ["lastname"] = "Doe"
            };

            // Act & Assert
            var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(() =>
                service.Create(duplicateContact));

            Assert.Contains("already exists", ex.Message);
        }

        [Fact]
        public void Create_Should_Handle_Different_EntityReference_As_Unique()
        {
            // Arrange
            var context = new XrmFakedContext();

            // Set up alternate key with EntityReference attribute
            context.AddAlternateKey("contact", new[] { "parentcustomerid", "lastname" });

            var accountId1 = Guid.NewGuid();
            var existingContact = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                ["parentcustomerid"] = new EntityReference("account", accountId1),
                ["lastname"] = "Doe"
            };
            context.Initialize(new List<Entity> { existingContact });

            var service = context.GetOrganizationService();

            // Create a contact with different EntityReference (different account)
            var newContact = new Entity("contact")
            {
                ["parentcustomerid"] = new EntityReference("account", Guid.NewGuid()),
                ["lastname"] = "Doe"
            };

            // Act
            var newId = service.Create(newContact);

            // Assert - should succeed because the EntityReference is different
            Assert.NotEqual(Guid.Empty, newId);
        }

        #endregion

        #region No Keys Defined Tests

        [Fact]
        public void Create_Should_Not_Check_Keys_When_No_Keys_Defined()
        {
            // Arrange
            var context = new XrmFakedContext();

            // No alternate key defined - just initialize entity metadata
            var metadata = new EntityMetadata { LogicalName = "contact" };
            context.SetEntityMetadata(metadata);

            var existingContact = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                ["firstname"] = "John",
                ["lastname"] = "Doe"
            };
            context.Initialize(new List<Entity> { existingContact });

            var service = context.GetOrganizationService();

            // Create a contact with same attribute values
            var newContact = new Entity("contact")
            {
                ["firstname"] = "John",
                ["lastname"] = "Doe"
            };

            // Act - should succeed because no alternate key is defined
            var newId = service.Create(newContact);

            // Assert
            Assert.NotEqual(Guid.Empty, newId);
        }

        [Fact]
        public void Create_Should_Not_Check_Keys_When_No_Metadata_Defined()
        {
            // Arrange
            var context = new XrmFakedContext();

            // No metadata at all
            var existingContact = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                ["firstname"] = "John",
                ["lastname"] = "Doe"
            };
            context.Initialize(new List<Entity> { existingContact });

            var service = context.GetOrganizationService();

            // Create a contact with same attribute values
            var newContact = new Entity("contact")
            {
                ["firstname"] = "John",
                ["lastname"] = "Doe"
            };

            // Act - should succeed because no metadata/keys are defined
            var newId = service.Create(newContact);

            // Assert
            Assert.NotEqual(Guid.Empty, newId);
        }

        #endregion
    }
#endif
}
