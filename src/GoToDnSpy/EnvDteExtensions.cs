using System;
using EnvDTE;
using Microsoft.VisualStudio.ProjectSystem.VS;
using Microsoft.VisualStudio.Shell;
using VSLangProj80;

namespace GoToDnSpy
{
    internal static class EnvDteExtensions
    {
        public static T FindByNameOrDefault<T>(this Properties properties, string name)
        {
            if (properties == null || string.IsNullOrEmpty(name))
                return default;

            ThreadHelper.ThrowIfNotOnUIThread();

            foreach (Property property in properties)
            {
                if (property == null || string.CompareOrdinal(property.Name, name) != 0)
                    continue;

                return (T) property.Value;
            }
            return default;
        }

        public static ProjectItem FindByNameOrDefault(this ProjectItems collection, string name, bool recursive = false)
        {
            if (collection == null)
                return null;

            ThreadHelper.ThrowIfNotOnUIThread();

            foreach (ProjectItem item in collection)
            {
                if (string.Equals(item.Name, name, StringComparison.Ordinal))
                    return item;

                if (recursive)
                {
                    var childItem = item.ProjectItems.FindByNameOrDefault(name, recursive);

                    if (childItem != null)
                        return childItem;
                }
            }

            return null;
        }

        public static EnvDTE.ProjectItem FindProjectItemByNameOrDefault(this Project project, string name, bool recursive) // directory tree recursive find of files
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            // if it's real project we can use find
            if (project.Kind != EnvDTE.Constants.vsProjectKindSolutionItems)
            {
                if (project.ProjectItems?.Count > 0)
                    return project.ProjectItems.FindByNameOrDefault(name, recursive);
                else
                    return null;
            }
            // if we in solution folder, one of its ProjectItems might be a real project
            foreach (ProjectItem item in project.ProjectItems)
            {
                if (item.Object is Project realProject)
                {
                    var projectItem = realProject.FindProjectItemByNameOrDefault(name, recursive);
                    if (projectItem != null)
                        return projectItem;
                }
            }
            return null;
        }

        /// <summary>
        /// Workaround from https://github.com/dotnet/project-system/issues/669
        /// </summary>
        private static string GetProjectPropertyNetCoreWorkaround(this Project project, string name)
        {
            var unconfiguredProject = project.AsUnconfiguredProject();
            var configuredProject = unconfiguredProject.GetSuggestedConfiguredProjectAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var properties = configuredProject.Services.ProjectPropertiesProvider.GetCommonProperties();
            return properties.GetEvaluatedPropertyValueAsync(name).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Return value of project property or null
        /// </summary>
        /// <param name="project">EnvDTE proj</param>
        /// <param name="name">The property name.</param>
        /// <returns>string or null</returns>
        public static string GetPropertyOrDefault(this Project project, string name)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return (project.Properties.FindByNameOrDefault<string>(name)
                ?? project.ConfigurationManager?.ActiveConfiguration?.Properties.FindByNameOrDefault<string>(name)
                ?? (project.Properties as ProjectConfigurationProperties3)?.OutputPath
                ?? (project.Properties.Item(name).Value?.ToString())
                ?? project.GetProjectPropertyNetCoreWorkaround(name)
                )?.Trim();
        }

        /// <summary>
        /// Gets the output filename.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>output filepath</returns>
        public static string GetOutputFilename(this Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return (project.Properties.FindByNameOrDefault<string>("OutputFileName") ?? project.GetProjectPropertyNetCoreWorkaround("TargetFileName"))?.Trim();
        }
    }
}
