﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMS_DataService.Queries
{
    class QueryManager
    {
        private QueryManager()
        {
        }

        static public string GetLastInsertID()
        {
            return "SELECT last_insert_id();";
        }

        static public string GetPluginsCountQuery()
        {
            return "SELECT COUNT(*) FROM `plugins`;";
        }

        static public string GetPluginsQuery()
        {
            return "SELECT * FROM `plugins`;";
        }

        static public string GetSpecificPluginQuery(int dwPluginID)
        {
            return "SELECT * FROM `plugins` WHERE `PluginID` = " + dwPluginID + ";";
        }

        static public string NewPluginQuery(string sPluginName,
                                        string sPluginDescription,
                                        bool bPluginEnabled,
                                        string sPluginFilePath)
        {
            return  "INSERT INTO `plugins`(`PluginName`, `PluginDescription`, `PluginEnabled`, `PluginBLOB`) " +
                    "VALUES ( " +
                    "'" + sPluginName + "', " +
                    "'" + sPluginDescription + "', " +
                    ((bPluginEnabled) ? 1 : 0) + ", " +
                    "LOAD_FILE('" + sPluginFilePath + "')" +
                    ");";
        }

        static public string UpdatePluginQuery(int dwPluginID, 
                                        string sPluginName,
                                        string sPluginDescription,
                                        bool bPluginEnabled,
                                        string sPluginFilePath)
        {
            return "UPDATE `plugins` SET " +
                    "`PluginName` = '" + sPluginName + "', " +
                    "`PluginDescription` = '" + sPluginDescription + "', " +
                    "`PluginEnabled` = " + ((bPluginEnabled) ? 1 : 0) + ", " +
                    "`PluginBLOB` = LOAD_FILE('" + sPluginFilePath + "') " +
                    "WHERE `PluginID` = " + dwPluginID +";";
        }

        static public string GetAccountsCountQuery(int dwPluginID)
        {
            return "SELECT COUNT(*) FROM `accounts` WHERE `PluginID` = " + dwPluginID + ";";
        }

        static public string GetAccountsQuery(int dwPluginID)
        {
            return "SELECT * FROM `accounts` WHERE `PluginID` = " + dwPluginID + ";";
        }

        static public string GetSpecificAccountQuery(int dwAccountID)
        {
            return "SELECT * FROM `accounts` WHERE `AccountID` = " + dwAccountID + ";";
        }

        static public string NewAccountQuery(int dwPluginID,
                                        string sAccountName,
                                        string sAccountDescription,
                                        string sAccountUserName,
                                        string sAccountPassword)
        {
            return "INSERT INTO `accounts`(`PluginID`, `AccountName`, `AccountDescription`, `AccountUserName`, `AccountPassword`) " +
                    "VALUES ( " +
                    dwPluginID + ", " +
                    "'" + sAccountName + "', " +
                    "'" + sAccountDescription + "', " +
                    "'" + sAccountUserName + "', " +
                    "'" + sAccountPassword + "'" +
                    ");";
        }

        static public string UpdateAccountQuery(int dwAccountID,
                                            int dwPluginID,
                                            string sAccountName,
                                            string sAccountDescription,
                                            string sAccountUserName,
                                            string sAccountPassword)
        {
            return "UPDATE `accounts` SET " +
                    "`PluginID` = " + dwPluginID + ", " +
                    "`AccountName` = '" + sAccountName + "', " +
                    "`AccountDescription` = '" + sAccountDescription + "', " +
                    "`AccountUserName` = '" + sAccountUserName + "', " +
                    "`AccountPassword` = '" + sAccountPassword + "'" +
                    "WHERE `AccountID` = " + dwAccountID + ";";
        }

        static public string GetConfigurationCountQuery(int dwAccountID)
        {
            return "SELECT COUNT(*) FROM `configurations` WHERE `AccountID` = " + dwAccountID + ";";
        }

        static public string GetConfigurationQuery(int dwAccountID)
        {
            return "SELECT * FROM `configurations` WHERE `AccountID` = " + dwAccountID + ";";
        }

        static public string GetSpecificConfigurationQuery(int dwConfigurationID)
        {
            return "SELECT * FROM `configurations` WHERE `ConfigurationID` = " + dwConfigurationID + ";";
        }

        static public string NewConfigurationQuery(int dwAccountID,
                                                string sConfigurationName,
                                                string sConfigurationDescription,
                                                bool bConfigurationEnabled)
        {
            return "INSERT INTO `configurations`(`AccountID`, `ConfigurationName`, `ConfigurationDescription`, `ConfigurationEnabled`) " +
                    "VALUES ( " +
                    dwAccountID + ", " +
                    "'" + sConfigurationName + "', " +
                    "'" + sConfigurationDescription + "', " +
                    ((bConfigurationEnabled) ? 1 : 0) +
                    ");";
        }

        static public string UpdateConfigurationQuery(int dwConfigurationID,
                                                int dwAccountID,
                                                string sConfigurationName,
                                                string sConfigurationDescription,
                                                bool bConfigurationEnabled)
        {
            return "UPDATE `configurations` SET " +
                    "`AccountID` = " + dwAccountID + ", " +
                    "`ConfigurationName` = '" + sConfigurationName + "', " +
                    "`ConfigurationDescription` = '" + sConfigurationDescription + "', " +
                    "`ConfigurationEnabled` = " + ((bConfigurationEnabled) ? 1 : 0) + " " +
                    "WHERE `ConfigurationID` = " + dwConfigurationID + ";";
        }

        static public string GetSequencesCountQuery(int dwConfigurationID)
        {
            return "SELECT COUNT(*) FROM `configurationsequences` WHERE `ConfigurationID` = " + dwConfigurationID + ";";
        }

        static public string GetSequencesQuery(int dwConfigurationID)
        {
            return "SELECT sequences.SequenceID, sequences.SequenceName, configurationsequences.SequenceEnabled FROM `sequences` LEFT JOIN `configurationsequences` ON sequences.SequenceID = configurationsequences.SequenceID WHERE configurationsequences.ConfigurationID = " + dwConfigurationID + ";";
        }

        static public string GetSequencesForPluginQuery(int dwPluginID)
        {
            return "SELECT * FROM `sequences` WHERE `PluginID` = " + dwPluginID + ";";
        }

        static public string NewSequenceQuery(int dwPluginID,
                                        string sSequenceName)
        {
            return "INSERT INTO `sequences`(`PluginID`, `SequenceName`) " +
                    "VALUES ( " +
                    dwPluginID + ", " +
                    "'" + sSequenceName + "' " +
                    ");";
        }

        static public string UpdateSequenceQuery(int dwSequenceID,
                                                int dwPluginID,
                                                string sSequenceName)
        {
            return "UPDATE `sequences` SET " +
                    "`PluginID` = " + dwPluginID + ", " +
                    "`SequenceName` = '" + sSequenceName + "' " +
                    "WHERE `ConfigurationID` = " + dwSequenceID + ";";
        }

        static public string NewConfigurationSequenceQuery( int dwConfigurationID,
                                                            int dwSequenceID,
                                                            bool bEnabled)
        {
            return "INSERT INTO `configurationsequences`(`ConfigurationID`, `SequenceID`, `SequenceEnabled`) " +
                    "VALUES ( " +
                    dwConfigurationID + ", " +
                    dwSequenceID + ", " +
                    ((bEnabled) ? 1 : 0) + " " +
                    ");";
        }

        static public string UpdateConfigurationSequenceQuery(int dwSequenceID,
                                                                int dwConfigurationID,
                                                                bool bEnabled)
        {
            return "UPDATE `configurationsequences` SET " +
                    "`SequenceEnabled` = " + ((bEnabled) ? 1 : 0) + " " +
                    "WHERE `ConfigurationID` = " + dwConfigurationID + " AND `SequenceID` = " + dwSequenceID + ";";
        }

        static public string GetTriggerTypesCountQuery(int dwConfigurationID)
        {
            return "SELECT COUNT(*) FROM `triggertypes` WHERE `ConfigurationID` = " + dwConfigurationID + ";";
        }

        static public string GetTriggerTypesQuery(int dwConfigurationID)
        {
            return "SELECT * FROM `triggertypes` WHERE `ConfigurationID` = " + dwConfigurationID + ";";
        }

        static public string NewTriggeTypesQuery(int dwConfigurationID,
                                        string sTriggerTypeName,
                                        string sTriggerTypeDescription)
        {
            return "INSERT INTO `triggertypes`(`ConfigurationID`, `TriggerTypeName`, `TriggerTypeDescription`) " +
                    "VALUES ( " +
                    dwConfigurationID + ", " +
                    "'" + sTriggerTypeName + "', " +
                    "'" + sTriggerTypeDescription + "', " +
                    ");";
        }

        static public string UpdateTriggeTypesQuery(int dwTriggerID,
                                            int dwConfigurationID,
                                            string sTriggerTypeName,
                                            string sTriggerTypeDescription)
        {
            return "UPDATE `triggertypes` SET " +
                    "`ConfigurationID` = " + dwConfigurationID + ", " +
                    "`TriggerName` = '" + sTriggerTypeName + "', " +
                    "`TriggerDescription` = '" + sTriggerTypeDescription + ";";
        }

        static public string GetTriggersCountQuery(int dwConfigurationID, int dwTriggerTypeID)
        {
            return "SELECT COUNT(*) FROM `triggers` WHERE `ConfigurationID` = " + dwConfigurationID + " AND `TriggerTypeID` = " + dwTriggerTypeID + ";";
        }

        static public string GetTriggersQuery(int dwConfigurationID, int dwTriggerTypeID)
        {
            return "SELECT * FROM `triggers` WHERE `ConfigurationID` = " + dwConfigurationID + " AND `TriggerTypeID` = " + dwTriggerTypeID + ";";
        }

        static public string NewTriggerQuery(int dwConfigurationID,
                                        string sTriggerName,
                                        string sTriggerDescription,
                                        string sTriggerValue,
                                        int dwReactionSequenceID,
                                        string sReactionValue,
                                        bool bTriggerEnabled)
        {
            return "INSERT INTO `triggers`(`ConfigurationID`, `TriggerName`, `TriggerDescription`,`TriggerValue`, `ReactionSequenceID`, `ReactionValue`,`TriggerEnabled`) " +
                    "VALUES ( " +
                    dwConfigurationID + ", " +
                    "'" + sTriggerName + "', " +
                    "'" + sTriggerDescription + "', " +
                    "'" + sTriggerValue + "', " +
                    dwReactionSequenceID + ", " +
                    "'" + sReactionValue + "', " +
                    ((bTriggerEnabled) ? 1 : 0) +
                    ");";
        }

        static public string UpdateTriggerQuery(int dwTriggerID,
                                            int dwConfigurationID,
                                            string sTriggerName,
                                            string sTriggerDescription,
                                            string sTriggerValue,
                                            int dwReactionSequenceID,
                                            string sReactionValue,
                                            bool bTriggerEnabled)
        {
            return "UPDATE `triggers` SET " +
                    "`ConfigurationID` = " + dwConfigurationID + ", " +
                    "`TriggerName` = '" + sTriggerName + "', " +
                    "`TriggerDescription` = '" + sTriggerDescription + "', " +
                    "`TriggerValue` = '" + sTriggerValue + "', " +
                    "`ReactionSequenceID` = " + dwReactionSequenceID + ", " +
                    "`ReactionValue` = '" + sReactionValue + "', " +
                    "`TriggerEnabled` = " + ((bTriggerEnabled) ? 1 : 0) + " " +
                    "WHERE `TriggerID` = " + dwTriggerID + ";";
        }

        static public string GetUsersCountQuery()
        {
            return "SELECT COUNT(*) FROM `users`;";
        }

        static public string GetUsersQuery()
        {
            return "SELECT * FROM `users`;";
        }

        static public string NewUserQuery(string sUserName,
                                    string sUserHashedPassword,
                                    int dwUserTypeID,
                                    bool bUserEnableRead,
                                    bool bUserEnableWrite)
        {
            return "INSERT INTO `users`(`UserName`, `UserHashedPassword`, `UserTypeID`, `UserEnableRead`, `UserEnableWrite`) " +
                    "VALUES ( " +
                    "'" + sUserName + "', " +
                    "'" + sUserHashedPassword + "', " +
                    dwUserTypeID + ", " +
                    ((bUserEnableRead) ? 1 : 0) + ", " +
                    ((bUserEnableWrite) ? 1 : 0) +
                    ");";
        }

        static public string UpdateUserQuery(int dwUserID,
                                        string sUserName,
                                        string sUserHashedPassword,
                                        int dwUserTypeID,
                                        bool bUserEnableRead,
                                        bool bUserEnableWrite)
        {
            return "UPDATE `triggers` SET " +
                    "`UserName` = '" + sUserName + "', " +
                    "`UserHashedPassword` = '" + sUserHashedPassword + "', " +
                    "`UserTypeID` = " + dwUserTypeID + ", " +
                    "`UserEnableRead` = " + ((bUserEnableRead) ? 1 : 0) + ", " +
                    "`UserEnableWrite` = " + ((bUserEnableWrite) ? 1 : 0) + " " +
                    "WHERE `UserID` = " + dwUserID + ";";
        }

        static public string GetUserTypesCountQuery()
        {
            return "SELECT COUNT(*) FROM `userTypes`;";
        }

        static public string GetUserTypesQuery()
        {
            return "SELECT * FROM `userTypes`;";
        }

        static public string NewUserTypeQuery(int dwUserTypeID,
                                        string sUserTypeName )
        {
            return "INSERT INTO `usertypes`(`UserTypeID`, `UserTypeName`) " +
                    "VALUES ( " +
                    dwUserTypeID + ", " +
                    "'" + sUserTypeName + "', " +
                    ");";
        }

        static public string UpdateUserQuery(int dwUserTypeID,
                                        string sUserTypeName)
        {
            return "UPDATE `usertypes` SET " +
                    "`UserTypeName` = '" + sUserTypeName + "', " +
                    "WHERE `UserTypeID` = " + dwUserTypeID + ";";
        }
    }
}
