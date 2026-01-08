#if FAKE_XRM_EASY_9
using System.Linq;
using System.ServiceModel;
using Crm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Xunit;

namespace FakeXrmEasy.Tests.FakeContextTests.TranslateQueryExpressionTests.OperatorTests.MultiSelectOptionSet
{
    public class MultiSelectOptionSetTests
    {
        [Fact]
        public void When_executing_a_query_expression_equal_operator_returns_exact_matches_for_int_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.Equal, 2);

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Single(entities);
            Assert.Equal("2", entities[0]["firstname"]);
        }

        [Fact]
        public void When_executing_a_query_expression_equal_operator_returns_exact_matches_for_string_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.Equal, "2");

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Single(entities);
            Assert.Equal("2", entities[0]["firstname"]);
        }

        [Fact]
        public void When_executing_a_query_expression_equal_operator_throws_exception_for_optionsetvalue_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });

            var qe = new QueryExpression("contact");
            qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.Equal, new OptionSetValue(2));

            Assert.Throws<FaultException>(() => service.RetrieveMultiple(qe));
        }

        [Fact]
        public void When_executing_a_query_expression_equal_operator_throws_exception_for_optionsetvaluecollection_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });

            var qe = new QueryExpression("contact");
            qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.Equal, new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) });

            Assert.Throws<FaultException<OrganizationServiceFault>>(() => service.RetrieveMultiple(qe));
        }

        [Fact]
        public void When_executing_a_query_expression_equal_operator_returns_exact_matches_for_single_int_array_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.Equal, new[] { 2 });

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Single(entities);
            Assert.Equal("2", entities[0]["firstname"]);
        }

        [Fact]
        public void When_executing_a_query_expression_equal_operator_throws_exception_for_int_array_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });

            var qe = new QueryExpression("contact");
            qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.Equal, new[] { 1, 2, 3 });

            Assert.Throws<FaultException<OrganizationServiceFault>>(() => service.RetrieveMultiple(qe));
        }

        [Fact]
        public void When_executing_a_query_expression_equal_operator_throws_exception_for_string_array_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });

            var qe = new QueryExpression("contact");
            qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.Equal, new[] { "1", "2", "3" });

            Assert.Throws<FaultException<OrganizationServiceFault>>(() => service.RetrieveMultiple(qe));
        }

        [Fact]
        public void When_executing_a_query_expression_notequal_operator_excludes_exact_matches_for_int_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.NotEqual, 2);

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Equal(4, entities.Count);
            Assert.True(entities.All(e => (e["firstname"] as string) != "2"));
        }

        [Fact]
        public void When_executing_a_query_expression_notequal_operator_excludes_exact_matches_for_single_int_array_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.NotEqual, new[] { 2 });

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Equal(4, entities.Count);
            Assert.True(entities.All(e => (e["firstname"] as string) != "2"));
        }

        [Fact]
        public void When_executing_a_query_expression_in_operator_returns_exact_matches_for_int_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.In, 2);

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Single(entities);
            Assert.Equal("2", entities[0]["firstname"]);
        }

        [Fact]
        public void When_executing_a_query_expression_in_operator_returns_exact_matches_for_string_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.In, "2");

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Single(entities);
            Assert.Equal("2", entities[0]["firstname"]);
        }

        [Fact]
        public void When_executing_a_query_expression_in_operator_throws_exception_for_optionsetvalue_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });

            var qe = new QueryExpression("contact");
            qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.In, new OptionSetValue(2));

            Assert.Throws<FaultException>(() => service.RetrieveMultiple(qe));
        }

        [Fact]
        public void When_executing_a_query_expression_in_operator_throws_exception_for_optionsetvaluecollection_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });

            var qe = new QueryExpression("contact");
            qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.In, new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) });

            Assert.Throws<FaultException>(() => service.RetrieveMultiple(qe));
        }

        [Fact]
        public void When_executing_a_query_expression_in_operator_returns_exact_matches_for_single_int_array_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.In, new[] { 2 });

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Single(entities);
            Assert.Equal("2", entities[0]["firstname"]);
        }

        [Fact]
        public void When_executing_a_query_expression_in_operator_returns_exact_matches_for_int_array_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.In, new[] { 2, 3 });

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Single(entities);
            Assert.Equal("2,3", entities[0]["firstname"]);
        }

        [Fact]
        public void When_executing_a_query_expression_in_operator_returns_exact_matches_for_out_of_order_int_array_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.In, new[] { 3, 1, 2 });

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Single(entities);
            Assert.Equal("1,2,3", entities[0]["firstname"]);
        }

        [Fact]
        public void When_executing_a_query_expression_in_operator_returns_exact_matches_for_out_of_order_int_params_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.In, 3, 1, 2);

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Single(entities);
            Assert.Equal("1,2,3", entities[0]["firstname"]);
        }

        [Fact]
        public void When_executing_a_query_expression_in_operator_returns_exact_matches_for_string_array_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.In, new[] { "2", "3" });

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Single(entities);
            Assert.Equal("2,3", entities[0]["firstname"]);
        }

        [Fact]
        public void When_executing_a_query_expression_in_operator_returns_exact_matches_for_string_params_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.In, "2", "3");

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Single(entities);
            Assert.Equal("2,3", entities[0]["firstname"]);
        }

        [Fact]
        public void When_executing_a_query_expression_in_operator_returns_exact_matches_for_out_of_order_string_array_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.In, new[] { "3", "2" });

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Single(entities);
            Assert.Equal("2,3", entities[0]["firstname"]);
        }

        [Fact]
        public void When_executing_a_query_expression_notin_operator_excludes_exact_matches_for_int_array_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.NotIn, new[] { 2, 3 });

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Equal(4, entities.Count);
            Assert.True(entities.All(e => (e["firstname"] as string) != "2,3"));
        }


        [Fact]
        public void When_executing_a_query_expression_notin_operator_excludes_exact_matches_for_out_of_order_string_params_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.NotIn, "3", "2");

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Equal(4, entities.Count);
            Assert.True(entities.All(e => (e["firstname"] as string) != "2,3"));
        }

        [Fact]
        public void When_executing_a_query_expression_containvalues_operator_returns_partial_matches_for_int_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.ContainValues, 1);

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Equal(2, entities.Count);
            Assert.True(entities.All(e => (e["firstname"] as string).Contains("1")));
        }

        [Fact]
        public void When_executing_a_query_expression_containvalues_operator_returns_partial_matches_for_string_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.ContainValues, "1");

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Equal(2, entities.Count);
            Assert.True(entities.All(e => (e["firstname"] as string).Contains("1")));
        }

        [Fact]
        public void When_executing_a_query_expression_containvalues_operator_throws_exception_for_optionsetvalue_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });

            var qe = new QueryExpression("contact");
            qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.ContainValues, new OptionSetValue(2));

            Assert.Throws<FaultException>(() => service.RetrieveMultiple(qe));
        }

        [Fact]
        public void When_executing_a_query_expression_containvalues_operator_throws_exception_for_optionsetvaluecollection_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });

            var qe = new QueryExpression("contact");
            qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.ContainValues, new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) });

            Assert.Throws<FaultException>(() => service.RetrieveMultiple(qe));
        }

        [Fact]
        public void When_executing_a_query_expression_containvalues_operator_returns_partial_matches_for_single_int_array_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.ContainValues, new[] { 1 });

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Equal(2, entities.Count);
            Assert.True(entities.All(e => (e["firstname"] as string).Contains("1")));
        }

        [Fact]
        public void When_executing_a_query_expression_containvalues_operator_returns_partial_matches_for_int_array_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.ContainValues, new[] { 1, 3 });

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Equal(3, entities.Count);
            Assert.True(entities.All(e => (e["firstname"] as string).Contains("1") || (e["firstname"] as string).Contains("3")));
        }

        [Fact]
        public void When_executing_a_query_expression_containvalues_operator_returns_partial_matches_for_int_params_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.ContainValues, 1, 3);

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Equal(3, entities.Count);
            Assert.True(entities.All(e => (e["firstname"] as string).Contains("1") || (e["firstname"] as string).Contains("3")));
        }

        [Fact]
        public void When_executing_a_query_expression_containvalues_operator_returns_partial_matches_for_out_of_order_int_array_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.ContainValues, new[] { 3, 2 });

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Equal(4, entities.Count);
            Assert.True(entities.All(e => (e["firstname"] as string).Contains("3") || (e["firstname"] as string).Contains("2")));
        }

        [Fact]
        public void When_executing_a_query_expression_containvalues_operator_returns_partial_matches_for_string_array_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.ContainValues, new[] { "1", "3" });

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Equal(3, entities.Count);
            Assert.True(entities.All(e => (e["firstname"] as string).Contains("1") || (e["firstname"] as string).Contains("3")));
        }

        [Fact]
        public void When_executing_a_query_expression_containvalues_operator_returns_partial_matches_for_out_of_order_string_array_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.ContainValues, new[] { "3", "1" });

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Equal(3, entities.Count);
            Assert.True(entities.All(e => (e["firstname"] as string).Contains("3") || (e["firstname"] as string).Contains("1")));
        }

        [Fact]
        public void When_executing_a_query_expression_doesnotcontainvalues_operator_excludes_partial_matches_for_int_array_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.DoesNotContainValues, new[] { 2, 3 });

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Single(entities);
            Assert.Equal("null", entities[0]["firstname"]);
        }

        [Fact]
        public void When_executing_a_query_expression_doesnotcontainvalues_operator_excludes_partial_matches_for_out_of_order_string_params_right_hand_side()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();
            service.Create(new Contact { FirstName = "1,2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2) } });
            service.Create(new Contact { FirstName = "2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "1,2,3", new_MultiSelectAttribute = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2), new OptionSetValue(3) } });
            service.Create(new Contact { FirstName = "null" });

      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.DoesNotContainValues, "3", "1");

            var entities = service.RetrieveMultiple(qe).Entities;

            Assert.Equal(2, entities.Count);
            Assert.True(entities.All(e => !(e["firstname"] as string).Contains("3") && !(e["firstname"] as string).Contains("1")));
        }

        #region Issue #467 - Late-bound entity tests for ContainValues/DoesNotContainValues

        [Fact]
        public void When_executing_containvalues_with_late_bound_entity_should_return_matching_results()
        {
            // Arrange - Issue #467: NRE when using ContainValues with late-bound entities
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            // Create late-bound entities (no early-bound types, no ProxyTypesAssembly)
            var entity1 = new Entity("contact") { Id = System.Guid.NewGuid() };
            entity1["firstname"] = "1,2";
            entity1["new_multiselectattribute"] = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) };
            service.Create(entity1);

            var entity2 = new Entity("contact") { Id = System.Guid.NewGuid() };
            entity2["firstname"] = "2";
            entity2["new_multiselectattribute"] = new OptionSetValueCollection() { new OptionSetValue(2) };
            service.Create(entity2);

            var entity3 = new Entity("contact") { Id = System.Guid.NewGuid() };
            entity3["firstname"] = "2,3";
            entity3["new_multiselectattribute"] = new OptionSetValueCollection() { new OptionSetValue(2), new OptionSetValue(3) };
            service.Create(entity3);

            var entity4 = new Entity("contact") { Id = System.Guid.NewGuid() };
            entity4["firstname"] = "null";
            // No multi-select attribute
            service.Create(entity4);

      // Act
      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.ContainValues, 1);

            var entities = service.RetrieveMultiple(qe).Entities;

            // Assert
            Assert.Single(entities);
            Assert.Equal("1,2", entities[0]["firstname"]);
        }

        [Fact]
        public void When_executing_containvalues_with_late_bound_entity_and_null_attribute_should_return_no_results()
        {
            // Arrange - Issue #467: NRE when attribute value is null
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            // Create late-bound entity with null multi-select attribute
            var entity1 = new Entity("contact") { Id = System.Guid.NewGuid() };
            entity1["firstname"] = "has null multiselect";
            entity1["new_multiselectattribute"] = null;
            service.Create(entity1);

            var entity2 = new Entity("contact") { Id = System.Guid.NewGuid() };
            entity2["firstname"] = "no multiselect attribute";
            // Attribute not set at all
            service.Create(entity2);

      // Act - should not throw NRE
      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.ContainValues, 1);

            var entities = service.RetrieveMultiple(qe).Entities;

            // Assert - no results should match when attribute is null
            Assert.Empty(entities);
        }

        [Fact]
        public void When_executing_doesnotcontainvalues_with_late_bound_entity_should_return_correct_results()
        {
            // Arrange - Issue #467: NRE when using DoesNotContainValues with late-bound entities
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            // Create late-bound entities
            var entity1 = new Entity("contact") { Id = System.Guid.NewGuid() };
            entity1["firstname"] = "1,2";
            entity1["new_multiselectattribute"] = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) };
            service.Create(entity1);

            var entity2 = new Entity("contact") { Id = System.Guid.NewGuid() };
            entity2["firstname"] = "2";
            entity2["new_multiselectattribute"] = new OptionSetValueCollection() { new OptionSetValue(2) };
            service.Create(entity2);

            var entity3 = new Entity("contact") { Id = System.Guid.NewGuid() };
            entity3["firstname"] = "3";
            entity3["new_multiselectattribute"] = new OptionSetValueCollection() { new OptionSetValue(3) };
            service.Create(entity3);

            var entity4 = new Entity("contact") { Id = System.Guid.NewGuid() };
            entity4["firstname"] = "null";
            // No multi-select attribute - should be included in DoesNotContainValues results
            service.Create(entity4);

      // Act
      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.DoesNotContainValues, 1);

            var entities = service.RetrieveMultiple(qe).Entities;

            // Assert - entities 2, 3, and 4 should be returned (those that don't contain value 1)
            Assert.Equal(3, entities.Count);
            Assert.True(entities.All(e => (e["firstname"] as string) != "1,2"));
        }

        [Fact]
        public void When_executing_doesnotcontainvalues_with_late_bound_entity_and_null_attribute_should_include_in_results()
        {
            // Arrange - Issue #467: entities with null multi-select attribute should be included in DoesNotContainValues
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            var entity1 = new Entity("contact") { Id = System.Guid.NewGuid() };
            entity1["firstname"] = "has value";
            entity1["new_multiselectattribute"] = new OptionSetValueCollection() { new OptionSetValue(1) };
            service.Create(entity1);

            var entity2 = new Entity("contact") { Id = System.Guid.NewGuid() };
            entity2["firstname"] = "has null";
            entity2["new_multiselectattribute"] = null;
            service.Create(entity2);

            var entity3 = new Entity("contact") { Id = System.Guid.NewGuid() };
            entity3["firstname"] = "no attribute";
            // Attribute not set at all
            service.Create(entity3);

      // Act
      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.DoesNotContainValues, 1);

            var entities = service.RetrieveMultiple(qe).Entities;

            // Assert - entities with null/missing attribute should be included since they don't contain value 1
            Assert.Equal(2, entities.Count);
            Assert.True(entities.All(e => (e["firstname"] as string) != "has value"));
        }

        [Fact]
        public void When_executing_containvalues_with_late_bound_entity_multiple_values_should_work()
        {
            // Arrange - Issue #467: Testing with multiple values in ContainValues
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            var entity1 = new Entity("contact") { Id = System.Guid.NewGuid() };
            entity1["firstname"] = "1,2";
            entity1["new_multiselectattribute"] = new OptionSetValueCollection() { new OptionSetValue(1), new OptionSetValue(2) };
            service.Create(entity1);

            var entity2 = new Entity("contact") { Id = System.Guid.NewGuid() };
            entity2["firstname"] = "2";
            entity2["new_multiselectattribute"] = new OptionSetValueCollection() { new OptionSetValue(2) };
            service.Create(entity2);

            var entity3 = new Entity("contact") { Id = System.Guid.NewGuid() };
            entity3["firstname"] = "3,4";
            entity3["new_multiselectattribute"] = new OptionSetValueCollection() { new OptionSetValue(3), new OptionSetValue(4) };
            service.Create(entity3);

      // Act - search for entities containing 1 or 3
      var qe = new QueryExpression("contact")
      {
        ColumnSet = new ColumnSet(new[] { "firstname" })
      };
      qe.Criteria.AddCondition("new_multiselectattribute", ConditionOperator.ContainValues, 1, 3);

            var entities = service.RetrieveMultiple(qe).Entities;

            // Assert - should return entities 1 and 3 (those containing either 1 or 3)
            Assert.Equal(2, entities.Count);
            Assert.Contains(entities, e => (e["firstname"] as string) == "1,2");
            Assert.Contains(entities, e => (e["firstname"] as string) == "3,4");
        }

        #endregion
    }
}
#endif