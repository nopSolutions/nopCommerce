using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Demo.CreateRoom.Domains;

namespace Nop.Plugin.Demo.CreateRoom.Mapping.Builders
{
    internal class RoomBuilder : NopEntityBuilder<Room>
    {
        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            //table.WithColumn(nameof(Room.Id)).AsInt32().PrimaryKey().ForeignKey<Product>(onDelete: Rule.Cascade)
            table.WithColumn(nameof(Room.Id)).AsInt32().PrimaryKey()
            .WithColumn(nameof(Room.Name)).AsString(400)
            .WithColumn(nameof(Room.Description)).AsString(int.MaxValue)
            .WithColumn(nameof(Room.Price)).AsDecimal(18, 4)
            .WithColumn(nameof(Room.MaximumDate)).AsInt32()
            .WithColumn(nameof(Room.IsActive)).AsBoolean();
        }
    }
}