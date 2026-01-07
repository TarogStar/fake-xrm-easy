using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace FakeXrmEasy.Extensions
{
    /// <summary>
    /// Extension helpers that add convenience operations to <see cref="QueryExpression"/> instances.
    /// </summary>
    public static class QueryExpressionExtensions
    {
        /// <summary>
        /// Resolves the logical entity name associated with a FetchXML alias in a query.
        /// </summary>
        /// <param name="qe">The query expression to inspect.</param>
        /// <param name="sAlias">The alias to resolve.</param>
        /// <returns>The linked entity logical name when found; otherwise the alias itself.</returns>
        public static string GetEntityNameFromAlias(this QueryExpression qe, string sAlias)
        {
            if (sAlias == null)
                return qe.EntityName;

            var linkedEntity = qe.LinkEntities
                            .Where(le => le.EntityAlias != null && le.EntityAlias.Equals(sAlias))
                            .FirstOrDefault();

            if (linkedEntity != null)
            {
                return linkedEntity.LinkToEntityName;
            }

            //If the alias wasn't found, it means it  could be any of the EntityNames
            return sAlias;
        }

        /// <summary>
        /// Creates a deep clone of the query expression so callers can mutate the copy safely.
        /// </summary>
        /// <param name="qe">The source query expression.</param>
        /// <returns>A new <see cref="QueryExpression"/> that contains the same structure and values.</returns>
        public static QueryExpression Clone(this QueryExpression qe)
        {
            return qe.Copy();
        }
    }
}