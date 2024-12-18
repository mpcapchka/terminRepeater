namespace TerminRepeater.Model
{
    public interface IDataTerminProvider
    {
        /// <summary>
        /// The ID used by database. By default is <see cref="int">0</see> when new.
        /// </summary>
        int Id { get; }
        /// <summary>
        /// The module ID used by database. Cannot be <see cref="int">0</see>.
        /// </summary>
        int ModuleId { get; }
        /// <summary>
        /// The termin on language of <see cref="IDataContainerProvider.LanguageTerm"/>.
        /// </summary>
        string Termin { get; }
        /// <summary>
        /// The description on language of <see cref="IDataContainerProvider.LanguageDescription"/>.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Provides changes in provider if givven <paramref name="item"/> contains changes otherwise doesn't change values.
        /// </summary>
        /// <param name="item"></param>
        void Merge(TerminItem item);
        /// <summary>
        /// Checks changes that could be done by <see cref="void">Merge</see> method or if module is new.
        /// </summary>
        /// <returns><see cref="bool">true</see> if changes provided, otherwise <see cref="bool">false</see>.</returns>
        bool HasChanges();
        /// <summary>
        /// 
        /// </summary>
        /// <returns>The item.</returns>
        TerminItem ToItem();
    }
    public class DataTerminProvider : IDataTerminProvider
    {
        #region Fields
        private bool hasChanges = true;
        #endregion

        #region Constructor
        public DataTerminProvider() { }
        public DataTerminProvider(TerminItem item)
        {
            Id = item.Id;
            ModuleId = default;
            Termin = item.Termin;
            Description = item.Description;
            hasChanges = false;
        }
        #endregion

        #region Properties
        public int Id { get; init; } = default;
        public int ModuleId { get; init; } = default;
        public string Termin { get; } = string.Empty;
        public string Description { get; } = string.Empty;
        #endregion

        #region Methods
        public TerminItem ToItem()
        {
            return new TerminItem()
            {
                Id = this.Id,
                Termin = this.Termin,
                Description = this.Description,
            };
        }
        public bool HasChanges()
        {
            throw new NotImplementedException();
        }
        public void Merge(TerminItem item)
        {
            var terminDifferent = item.Termin != this.Termin;
            var descriptionDifferent = item.Description != this.Description;
            
        }
        #endregion
    }
}
