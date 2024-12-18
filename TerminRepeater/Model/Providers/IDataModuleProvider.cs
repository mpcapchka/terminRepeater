namespace TerminRepeater.Model
{
    public interface IDataModuleProvider
    {
        /// <summary>
        /// The ID used by database. By default is <see cref="int">0</see> when new.
        /// </summary>
        int Id { get; }
        /// <summary>
        /// The container ID used by database. Cannot be <see cref="int">0</see>.
        /// </summary>
        int ContainerId { get; }
        /// <summary>
        /// The module's name. Must be unique.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The module's description.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// The termins that belongs to the module by ID.
        /// </summary>
        IDataTerminProvider[] Items { get; }
        /// <summary>
        /// Checks changes that could be done by <see cref="void">Merge</see> method or if module is new.
        /// </summary>
        /// <returns><see cref="bool">true</see> if changes provided, otherwise <see cref="bool">false</see>.</returns>
        bool HasChanges();
        /// <summary>
        /// Provides changes in provider if givven <paramref name="item"/> contains changes otherwise doesn't change values.
        /// </summary>
        /// <param name="item"></param>
        void Merge(ModuleItem item);
        /// <summary>
        /// 
        /// </summary>
        /// <returns>The item.</returns>
        ModuleItem ToItem();
        /// <summary>
        /// Saves provided changes to database.
        /// </summary>
        void SaveChanges();
    }
    public class DataModuleProvider : IDataModuleProvider
    {
        #region Methods
        public int Id { get; }
        public int ContainerId { get; }
        public string Name { get; } = string.Empty;
        public string Description { get; } = string.Empty;
        public IDataTerminProvider[] Items { get; } = Array.Empty<IDataTerminProvider>();
        #endregion

        #region Methods
        public ModuleItem ToItem()
        {
            return new ModuleItem()
            {
                Name = this.Name,
                Description = this.Description,
                Items = Items.Select(x => x.ToItem()).ToArray()
            };
        }
        #endregion
    }
}
