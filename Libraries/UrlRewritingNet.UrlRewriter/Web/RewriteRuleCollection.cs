/* UrlRewritingNet.UrlRewrite
 * Version 2.0
 * 
 * This Library is Copyright 2006 by Albert Weinert and Thomas Bandt.
 * 
 * http://der-albert.com, http://blog.thomasbandt.de
 * 
 * This Library is provided as is. No warrenty is expressed or implied.
 * 
 * You can use these Library in free and commercial projects without a fee.
 * 
 * No charge should be made for providing these Library to a third party.
 * 
 * It is allowed to modify the source to fit your special needs. If you 
 * made improvements you should make it public available by sending us 
 * your modifications or publish it on your site. If you publish it on 
 * your own site you have to notify us. This is not a commitment that we 
 * include your modifications. 
 * 
 * This Copyright notice must be included in the modified source code.
 * 
 * You are not allowed to build a commercial rewrite engine based on 
 * this code.
 * 
 * Based on http://weblogs.asp.net/fmarguerie/archive/2004/11/18/265719.aspx
 * 
 * For further informations see: http://www.urlrewriting.net/
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace UrlRewritingNet.Web
{
    internal class RewriteRuleCollection : List<RewriteRule>
    {
        public void RemoveByName(string ruleName)
        {
            RewriteRule forRemove = null;
            foreach (RewriteRule rule in this)
            {
                if (rule.Name == ruleName)
                {
                    forRemove = rule;
                    break;
                }
            }
            if (forRemove != null)
                Remove(forRemove);
        }

        public void ReplaceRuleByName(string ruleName, RewriteRule rule)
        {
            int idx = GetIndexByName(ruleName);
            if (idx != -1)
            {
                rule.Name = ruleName;
                this[idx] = rule;
            }
            else
            {
                throw new ArgumentException(string.Format("UrlRewritingNet: Unknown ruleName '{0}'", ruleName), "ruleName");
            }
        }

        public void InsertRuleBeforeName(string positionRuleName, string ruleName, RewriteRule rule)
        {
            int idx = GetIndexByName(positionRuleName);
            if (idx != -1)
            {
                rule.Name = ruleName;
                Insert(idx, rule);
            }
            else
            {
                throw new ArgumentException(string.Format("UrlRewritingNet: Unknown ruleName '{0}'", ruleName), "ruleName");
            }
        }

        private int GetIndexByName(string ruleName)
        {
            int foundIndex = -1;
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Name == ruleName)
                {
                    foundIndex = i;
                    break;
                }
            }
            return foundIndex;
        }
    }
}
