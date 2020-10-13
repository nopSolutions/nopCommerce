using System;
using System.Linq;
using System.Linq.Expressions;
using LinqToDB;
using Nop.Core;
using Nop.Core.Domain.Security;

namespace Nop.Data.DataProviders.SQL
{
    public static class AclSqlExtensions
    {
        /// <summary>
        /// Builds ACL mapping predicate
        /// </summary>
        /// <param name="subjectEntity">Subject to ACL</param>
        /// <param name="acls">Source with ACL records</param>
        /// <param name="customerRoles">Get customer role identifiers</param>
        /// <typeparam name="T">Entity type</typeparam>
        /// <returns></returns>
        [ExpressionMethod(nameof(SubjectToAclImpl))]
		public static bool SubjectToAcl<T>(this T subjectEntity, IQueryable<AclRecord> acls, int[] customerRoles) where T : BaseEntity, IAclSupported
		{
            return !subjectEntity.SubjectToAcl || acls.Any(acl =>
                        acl.EntityName == typeof(T).Name &&
                        acl.EntityId == subjectEntity.Id &&
                        customerRoles.Contains(acl.CustomerRoleId));
		}

		static Expression<Func<T, IQueryable<AclRecord>, int[], bool>> SubjectToAclImpl<T>() where T : BaseEntity, IAclSupported
		{
			return (subjectEntity, acls, customerRoles) => !subjectEntity.SubjectToAcl || 
                acls.Any(acl =>
                        acl.EntityName == typeof(T).Name &&
                        acl.EntityId == subjectEntity.Id &&
                        customerRoles.Contains(acl.CustomerRoleId));
		}
    }
}