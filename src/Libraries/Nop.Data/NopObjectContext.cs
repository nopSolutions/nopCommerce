//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain;


namespace Nop.Data
{
    /// <summary>
    /// Object context
    /// </summary>
    public class NopObjectContext : DbContext
    {
        public NopObjectContext()
            : base("name=NopSqlConnection")
        {
            
        }

        public DbSet<Language> Languages { get; set; }

        public DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TODO: Use Fluent API Here 

            // TODO: move this Fluent API configuration to Setting class
            // just loop through each class of existing DbSet and invoke appropriate method (e.g. GenerateModel(ModelBuilder modelBuilder))


            // TODO: rename columns into database to match properties (e.g. rename SettingID to SettingId)

            //setting
            modelBuilder.Entity<Setting>().ToTable("Nop_Setting");
            modelBuilder.Entity<Setting>().HasKey(s => s.SettingId);
            modelBuilder.Entity<Setting>().Property(s => s.SettingId).HasColumnName("SettingID");
            modelBuilder.Entity<Setting>().Property(s => s.Name).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<Setting>().Property(s => s.Value).IsRequired().HasMaxLength(2000);
            modelBuilder.Entity<Setting>().Property(s => s.Description).IsRequired();

            //language
            modelBuilder.Entity<Language>().ToTable("Nop_Language");
            modelBuilder.Entity<Language>().HasKey(l => l.LanguageId);
            modelBuilder.Entity<Language>().Property(l => l.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Language>().Property(l => l.LanguageCulture).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Language>().Property(l => l.FlagImageFileName).IsRequired().HasMaxLength(50);
        }
    }
}