﻿// ****************************************************************
// Copyright 2011, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System.IO;
using System.Xml;

namespace NUnit.Engine.Internal
{
    public class NUnitProject : IProject
    {
        #region Instance Fields

        /// <summary>
        /// The XmlDocument representing the loaded doc. It
        /// is generated from the text when the doc is loaded
        /// unless an exception is thrown. It is modified as the
        /// user makes changes.
        /// </summary>
        XmlDocument xmlDoc = new XmlDocument();

        /// <summary>
        /// Path to the file storing this project
        /// </summary>
        private string projectPath;

        /// <summary>
        /// Our collection of configs
        /// </summary>
        private ProjectConfigList configs;

        #endregion

        #region IProject Members

        /// <summary>
        /// Gets the path to the file storing this project, if any.
        /// If the project has not been saved, this is null.
        /// </summary>
        public string ProjectPath
        {
            get { return projectPath; }
        }

        /// <summary>
        /// EffectiveBasePath uses the BasePath if present and otherwise
        /// defaults to the directory part of the ProjectPath.
        /// </summary>
        public string EffectiveBasePath
        {
            get
            {
                string appbase = this.GetSetting("appbase");

                if (this.ProjectPath == null)
                    return appbase;

                if (appbase == null)
                    return Path.GetDirectoryName(this.ProjectPath);

                return Path.Combine(
                    Path.GetDirectoryName(this.ProjectPath),
                    appbase);
            }
        }

        /// <summary>
        /// Gets the active configuration, as defined
        /// by the particular project. For an NUnit
        /// project, we use the activeConfig setting
        /// if present and otherwise return the first
        /// config found.
        /// </summary>
        public IProjectConfig ActiveConfig 
        {
            get
            {
                if (configs.Count == 0)
                    return null;

                string activeConfigName = GetSetting("activeconfig");

                return activeConfigName != null
                    ? configs[activeConfigName]
                    : configs[0];
            }
        }

        public IProjectConfigList Configs
        {
            get { return configs; }
        }

        #endregion

        #region Other Public Methods

        public void Load(string filename)
        {
            this.projectPath = Path.GetFullPath(filename);
            xmlDoc.Load(this.projectPath);

            configs = new ProjectConfigList(this);
        }

        public void LoadXml(string xmlText)
        {
            xmlDoc.LoadXml(xmlText);

            configs = new ProjectConfigList(this);
        }

        public string GetSetting(string name)
        {
            return SettingsNode != null
                ? XmlHelper.GetAttribute(SettingsNode, name)
                : null;
        }

        public string GetSetting(string name, string defaultValue)
        {
            string result = GetSetting(name);
            return result == null
                ? defaultValue
                : result;
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// The top-level (NUnitProject) node
        /// </summary>
        internal XmlNode RootNode
        {
            get { return xmlDoc.FirstChild; }
        }

        /// <summary>
        /// The Settings node if present, otherwise null
        /// </summary>
        internal XmlNode SettingsNode
        {
            get { return RootNode.SelectSingleNode("Settings"); }
        }

        /// <summary>
        /// The collection of Config nodes - may be empty
        /// </summary>
        internal XmlNodeList ConfigNodes
        {
            get { return RootNode.SelectNodes("Config"); }
        }

        #endregion
    }
}
