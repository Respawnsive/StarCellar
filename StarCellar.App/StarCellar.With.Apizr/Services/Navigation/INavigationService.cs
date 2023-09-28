using CommunityToolkit.Maui.Core;

namespace StarCellar.With.Apizr.Services.Navigation
{
    public interface INavigationService
    {
        /// <param name="state">To be added.</param>
        /// <summary>To be added.</summary>
        /// <returns>To be added.</returns>
        /// <remarks>To be added.</remarks>
        Task GoToAsync(ShellNavigationState state);

        /// <param name="state">To be added.</param>
        /// <param name="animate">To be added.</param>
        /// <summary>Asynchronously navigates to <paramref name="state" />, optionally animating.</summary>
        /// <returns>To be added.</returns>
        /// <remarks>
        ///           <para>Note that <see cref="T:Microsoft.Maui.Controls.ShellNavigationState" /> has implicit conversions from <see langword="string" /> and <see cref="T:System.Uri" />, so developers may write code such as the following, with no explicit instantiation of the <see cref="T:Microsoft.Maui.Controls.ShellNavigationState" />:</para>
        ///           <example>
        ///             <code lang="csharp lang-csharp"><![CDATA[
        /// await Shell.Current.GoToAsync("app://xamarin.com/xaminals/animals/monkeys");
        ///     ]]></code>
        ///           </example>
        ///         </remarks>
        Task GoToAsync(ShellNavigationState state, bool animate);

        /// <param name="state">To be added.</param>
        /// <summary>To be added.</summary>
        /// <returns>To be added.</returns>
        /// <remarks>To be added.</remarks>
        Task GoToAsync(ShellNavigationState state, IDictionary<string, object> parameters);

        /// <param name="state">To be added.</param>
        /// <param name="animate">To be added.</param>
        /// <summary>Asynchronously navigates to <paramref name="state" />, optionally animating.</summary>
        /// <returns>To be added.</returns>
        /// <remarks>
        ///           <para>Note that <see cref="T:Microsoft.Maui.Controls.ShellNavigationState" /> has implicit conversions from <see langword="string" /> and <see cref="T:System.Uri" />, so developers may write code such as the following, with no explicit instantiation of the <see cref="T:Microsoft.Maui.Controls.ShellNavigationState" />:</para>
        ///           <example>
        ///             <code lang="csharp lang-csharp"><![CDATA[
        /// await Shell.Current.GoToAsync("app://xamarin.com/xaminals/animals/monkeys");
        ///     ]]></code>
        ///           </example>
        ///         </remarks>
        Task GoToAsync(
          ShellNavigationState state,
          bool animate,
          IDictionary<string, object> parameters);

        /// <param name="title">Title of the displayed action sheet. Must not be <see langword="null" />.</param>
        /// <param name="cancel">Text to be displayed in the 'Cancel' button. Can be <see langword="null" /> to hide the cancel action.</param>
        /// <param name="destruction">Text to be displayed in the 'Destruct' button.  Can be <see langword="null" /> to hide the destructive option.</param>
        /// <param name="buttons">Text labels for additional buttons. Must not be <see langword="null" />.</param>
        /// <summary>Displays a native platform action sheet, allowing the application user to choose from several buttons.</summary>
        /// <returns>An awaitable Task that displays an action sheet and returns the Text of the button pressed by the user.</returns>
        /// <remarks>
        ///   <para>Developers should be aware that Windows' line endings, CR-LF, only work on Windows systems, and are incompatible with iOS and Android. A particular consequence of this is that characters that appear after a CR-LF, (For example, in the title.) may not be displayed on non-Windows platforms. Developers must use the correct line endings for each of the targeted systems.</para>
        /// </remarks>
        public Task<string> DisplayActionSheet(
          string title,
          string cancel,
          string destruction,
          params string[] buttons);

        public Task<string> DisplayActionSheet(
          string title,
          string cancel,
          string destruction,
          FlowDirection flowDirection,
          params string[] buttons);

        /// <param name="title">The title of the alert dialog.</param>
        /// <param name="message">The body text of the alert dialog.</param>
        /// <param name="cancel">Text to be displayed on the 'Cancel' button.</param>
        /// <summary>Presents an alert dialog to the application user with a single cancel button.</summary>
        /// <returns>To be added.</returns>
        /// <remarks>To be added.</remarks>
        public Task DisplayAlert(string title, string message, string cancel);

        /// <param name="title">The title of the alert dialog.</param>
        /// <param name="message">The body text of the alert dialog.</param>
        /// <param name="accept">Text to be displayed on the 'Accept' button.</param>
        /// <param name="cancel">Text to be displayed on the 'Cancel' button.</param>
        /// <summary>Presents an alert dialog to the application user with an accept and a cancel button.</summary>
        /// <returns>A task that contains the user's choice as a Boolean value. <see langword="true" /> indicates that the user accepted the alert. <see langword="false" /> indicates that the user cancelled the alert.</returns>
        /// <remarks>To be added.</remarks>
        public Task<bool> DisplayAlert(string title, string message, string accept, string cancel);

        /// <param name="title">The title of the alert dialog.</param>
        /// <param name="message">The body text of the alert dialog.</param>
        /// <param name="cancel">Text to be displayed on the 'Cancel' button.</param>
        /// <summary>Presents an alert dialog to the application user with a single cancel button.</summary>
        /// <returns>To be added.</returns>
        /// <remarks>To be added.</remarks>
        public Task DisplayAlert(
          string title,
          string message,
          string cancel,
          FlowDirection flowDirection);

        /// <param name="title">The title of the alert dialog.</param>
        /// <param name="message">The body text of the alert dialog.</param>
        /// <param name="accept">Text to be displayed on the 'Accept' button.</param>
        /// <param name="cancel">Text to be displayed on the 'Cancel' button.</param>
        /// <summary>Presents an alert dialog to the application user with an accept and a cancel button.</summary>
        /// <returns>A task that contains the user's choice as a Boolean value. <see langword="true" /> indicates that the user accepted the alert. <see langword="false" /> indicates that the user cancelled the alert.</returns>
        /// <remarks>To be added.</remarks>
        public Task<bool> DisplayAlert(
          string title,
          string message,
          string accept,
          string cancel,
          FlowDirection flowDirection);

        /// <param name="title">To be added.</param>
        /// <param name="message">To be added.</param>
        /// <param name="accept">To be added.</param>
        /// <param name="cancel">To be added.</param>
        /// <param name="placeholder">To be added.</param>
        /// <param name="maxLength">To be added.</param>
        /// <param name="keyboard">To be added.</param>
        /// <param name="initialValue">To be added.</param>
        /// <summary>To be added.</summary>
        /// <returns>To be added.</returns>
        /// <remarks>To be added.</remarks>
        public Task<string> DisplayPromptAsync(
          string title,
          string message,
          string accept = "OK",
          string cancel = "Cancel",
          string placeholder = null,
          int maxLength = -1,
          Keyboard keyboard = null,
          string initialValue = "");

        /// <summary>
        /// Create new Toast
        /// </summary>
        /// <param name="message">Toast message</param>
        /// <param name="duration">Toast duration</param>
        /// <param name="textSize">Toast font size</param>
        /// <returns>New instance of Toast</returns>
        Task ShowToast(
            string message,
            ToastDuration duration = ToastDuration.Short,
            double textSize = AlertDefaults.FontSize, 
            CancellationToken token = default);
    }
}
