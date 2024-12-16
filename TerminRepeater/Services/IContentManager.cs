using TerminRepeater.Model;

namespace TerminRepeater.Services
{
    public interface IContentManager
    {
        /// <summary>
        /// Opens a module editor async for a specified module (<paramref name="item"/>).
        /// </summary>
        /// <param name="item"></param>
        /// <param name="existingModuleNames"/>
        /// <returns><see cref="bool">true</see> if changes applied, otherwise <see cref="bool">false</see>.</returns>
        Task<bool> OpenModuleEditor(IEnumerable<string> existingModuleNames, ModuleItem item);

        /// <summary>
        /// Shows an error dialog in current thread with provided <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        void ShowErrorDialog(string message);

        /// <summary>
        /// Shows a warning dialog in current thread with provided <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        void ShowWarnDialog(string message);

        /// <summary>
        /// Shows a prompt dialog async with provided <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Sepcified <see cref="string">text</see> if OK button clicked, otherwise <see cref="Nullable">null</see>.</returns>
        Task<string> ShowPromptDialog(string message);
    }
}
