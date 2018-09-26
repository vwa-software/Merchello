namespace Merchello.Core.Formatters
{
    /// <summary>
    /// Defines the NotificationFormatter
    /// </summary>
    public interface IFormatter
    {
        /// <summary>
        /// Formats a message
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// A formatted string
        /// </returns>
        string Format(string value);

		/// <summary>
		/// Formats a subject
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		string FormatSubject(string value);
    }
}