using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
namespace GoToDnSpy
{

    internal static class EnvDteExtensions
    {
        public static T FindByNameOrDefault<T>(this Properties properties, string name)
        {
            if (properties == null || string.IsNullOrEmpty(name))
                return default(T);


            foreach (Property property in properties)
            {
                if (property == null || string.CompareOrdinal(property.Name, name) != 0)
                    continue;

                return (T) property.Value;
            }
            return default(T);
        }


        public static ProjectItem FindByNameOrDefault(this ProjectItems collection, string name, bool recursive = false)
        {
            if (collection == null)
                return null;
            
            foreach (ProjectItem item in collection)
            {
                if (string.Compare(item.Name, name, StringComparison.Ordinal) == 0)
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
            // if it's real project we can use find
            if (project.Kind != EnvDTE.Constants.vsProjectKindSolutionItems)
            {
                if (project.ProjectItems != null && project.ProjectItems.Count > 0)
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


    }
}
