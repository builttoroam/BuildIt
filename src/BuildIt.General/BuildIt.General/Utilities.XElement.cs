using System.Linq;
using System.Xml.Linq;

namespace BuildIt
{
    /// <summary>
    /// General helper methods.
    /// </summary>
    public static partial class Utilities
    {
        /// <summary>
        /// Searches descendents for element that matches the name.
        /// </summary>
        /// <param name="element">The root element to search from.</param>
        /// <param name="name">The name of element to search for.</param>
        /// <returns>The value of the matching element, or null.</returns>
        public static string SafeDescendentValue(this XElement element, string name)
        {
            var dec = element?.Descendants(name).FirstOrDefault();
            return dec?.Value ?? string.Empty;
        }

        /// <summary>
        /// Searches descendents for element that matches the name.
        /// </summary>
        /// <param name="element">The root element to search from.</param>
        /// <param name="name">The name of element to search for.</param>
        /// <returns>The value of the matching element, or null.</returns>
        public static string SafeDescendentValue(this XElement element, XName name)
        {
            var dec = element?.Descendants(name).FirstOrDefault();
            return dec?.Value ?? string.Empty;
        }

        /// <summary>
        /// Searches elements for element that matches the name.
        /// </summary>
        /// <param name="element">The root element to search from.</param>
        /// <param name="name">The name of element to search for.</param>
        /// <returns>The value of the matching element, or null.</returns>
        public static string SafeElementValue(this XElement element, string name)
        {
            if (element?.Element(name) == null)
            {
                return string.Empty;
            }

            // ReSharper disable PossibleNullReferenceException
            return element.Element(name).Value;

            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Searches elements for element that matches the name.
        /// </summary>
        /// <param name="element">The root element to search from.</param>
        /// <param name="name">The name of element to search for.</param>
        /// <returns>The value of the matching element, or null.</returns>
        public static string SafeElementValue(this XElement element, XName name)
        {
            if (element?.Element(name) == null)
            {
                return string.Empty;
            }

            // ReSharper disable PossibleNullReferenceException
            return element.Element(name).Value;

            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Extracts the attribute value, if it exists.
        /// </summary>
        /// <param name="element">The element which contains the attribute.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <returns>The value of the attribute, or null.</returns>
        public static string SafeAttributeValue(this XElement element, XName name)
        {
            if (element?.Attribute(name) == null)
            {
                return string.Empty;
            }

            // ReSharper disable PossibleNullReferenceException
            return element.Attribute(name).Value;

            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Extracts the attribute value, if it exists.
        /// </summary>
        /// <param name="element">The element which contains the attribute.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <returns>The value of the attribute, or null.</returns>
        public static string SafeAttributeValue(this XElement element, string name)
        {
            if (element?.Attribute(name) == null)
            {
                return string.Empty;
            }

            // ReSharper disable PossibleNullReferenceException
            return element.Attribute(name).Value;

            // ReSharper restore PossibleNullReferenceException
        }
    }
}