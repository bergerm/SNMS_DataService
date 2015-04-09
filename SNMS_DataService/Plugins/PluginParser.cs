using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SNMS_DataService.Plugins
{
    public class Variable
    {
        public string sName;
        public string sType;
    }

    public class Sequence
    {
        public string sName;
    }

    public class Plugin
    {
        public string sName;
        public string sDescription;
        public List<Variable> valiableList;
        public List<Sequence> sequenceList;

        public Plugin()
        {
            valiableList = new List<Variable>();
            sequenceList = new List<Sequence>();
        }
    }

    class PluginParser
    {
        static public Plugin ParsePlugin(string sFilePath, ref string errorString)
        {
            Plugin plugin = new Plugin();

            // Load XmlDocument
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(sFilePath);

            // Get Plugin main node
            XmlNodeList pluginNodeList = xmlDocument.DocumentElement.SelectNodes("/Plugin");
            if (pluginNodeList.Count != 1)
            {
                errorString = "More than one plugin found in file";
                return null;
            }
            XmlNode pluginNode = pluginNodeList.Item(0);

            // Get Plugin Name
            XmlNodeList pluginNameNodes = pluginNode.SelectNodes("PluginName");
            if (pluginNameNodes.Count != 1)
            {
                errorString = "More than one plugin name found for plugin";
                return null;
            }
            plugin.sName = pluginNameNodes.Item(0).InnerText;

            // Get Plugin Description
            XmlNodeList pluginDescriptionNodes = pluginNode.SelectNodes("PluginDescription");
            if (pluginDescriptionNodes.Count != 1)
            {
                errorString = "More than one plugin description found for plugin";
                return null;
            }
            plugin.sDescription = pluginDescriptionNodes.Item(0).InnerText;

            // Get Variables
            XmlNodeList pluginVariables = pluginNode.SelectNodes("Variable");
            foreach (XmlNode variableNode in pluginVariables)
            {
                string varName = variableNode.Attributes["name"].Value.ToLower();
                string varType = variableNode.SelectSingleNode("VariableType").InnerText.ToLower();

                if (varName == "" || varType == "")
                {
                    errorString = "Invalid variable \"" + varName + "\" type \"" + varType + "\"";
                    return null;
                }

                Variable variable = new Variable();
                variable.sName = varName;
                variable.sType = varType;

                plugin.valiableList.Add(variable);
            }

            // Get Sequences
            XmlNodeList pluginSequences = pluginNode.SelectNodes("Sequence");
            foreach (XmlNode sequenceNode in pluginSequences)
            {
                string sequenceName = sequenceNode.Attributes["name"].Value;

                if (sequenceName == "")
                {
                    errorString = "Invalid sequence name \"" + sequenceName + "\"";
                    return null;
                }

                Sequence sequence = new Sequence();
                sequence.sName = sequenceName;

                plugin.sequenceList.Add(sequence);
            }

            return plugin;
        }
    }
}