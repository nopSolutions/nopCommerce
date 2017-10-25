using System.Collections.Generic;
using System.Linq;

namespace Nop.Core.Data
{
    /// <summary>
    /// 泛型的资源库接口
    /// Repository
    /// 抽象出一个实体常用的一些操作，据此定义一个泛型的接口，T限制为是必须继承于基类实体BaseEntity。
    /// 这样接口就可能适用于所有的实体，接口中定义了实体的一些常用的公用方法，比如：增、删、查、改。
    /// </summary>
    /// <remarks>
    /// Repository是一个独立的层，介于领域层与数据映射层（数据访问层）之间。
    /// 它的存在让领域层感觉不到数据访问层的存在，它提供一个类似集合的接口提供给领域层进行领域对象的访问。
    /// Repository是仓库管理员，领域层需要什么东西只需告诉仓库管理员，由仓库管理员把东西拿给它，并不需要知道东西实际放在哪。
    /// 
    /// 1. Repository模式是架构模式，在设计架构时，才有参考价值；
    /// 2. Repository模式主要是封装数据查询和存储逻辑；
    /// 3. Repository模式实际用途：更换、升级ORM引擎，不影响业务逻辑；
    /// 4. Repository模式能提高测试效率，单元测试时，用Mock对象代替实际的数据库存取，可以成倍地提高测试用例运行速度。
    /// 评估：应用Repository模式所带来的好处，远高于实现这个模式所增加的代码。只要项目分层，都应当使用这个模式。
    /// 
    /// 关于泛型Repository接口：
    ///     仅使用泛型Repository接口并不太合适，因为Repository接口是提供给Domain层的操作契约，不同的entity对于Domain来说可能有不同的操作约束。
    /// 因此Repository接口还是应该单独针对每个Eneity类来定义。
    ///     泛型的Repository<T>类仍然用来减少重复代码，只是不能被UserRepository类直接继承，因为这样Delete方法将侵入User类，
    /// 所以改为在UserRepository中组合一个Repository<T>，将开放给domain可见且又能使用泛型重用的功能委托给这个Repository<T>
    /// 
    /// Repository与Dal的区别：
    ///     Repository是DDD中的概念，强调Repository是受Domain驱动的，Repository中定义的功能要体现Domain的意图和约束，
    /// 而Dal更纯粹的就是提供数据访问的功能,并不严格受限于Business层。
    ///     使用Repository，隐含着一种意图倾向，就是 Domain需要什么我才提供什么，不该提供的功能就不要提供，
    /// 一切都是以Domain的需求为核心；而使用Dal，其意图倾向在于我Dal层能使用的数据库访问操作提供给Business层，
    /// 你Business要用哪个自己选。换一个Business也可以用我这个Dal，一切是以我Dal能提供什么操作为核心。
    /// 
    /// </remarks>
    public partial interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// 通过id获取实体
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        T GetById(object id);

        /// <summary>
        /// 新增实体
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Insert(T entity);

        /// <summary>
        /// 批量新增实体
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        void Insert(IEnumerable<T> entities);

        /// <summary>
        /// 更新实体
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Update(T entity);

        /// <summary>
        /// 批量更新实体
        /// Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        void Update(IEnumerable<T> entities);

        /// <summary>
        /// 删除实体
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Delete(T entity);

        /// <summary>
        /// 批量删除实体
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        void Delete(IEnumerable<T> entities);

        /// <summary>
        /// 返回实体集合
        /// Gets a table
        /// </summary>
        IQueryable<T> Table { get; }

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        IQueryable<T> TableNoTracking { get; }
    }
}
