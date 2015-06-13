using System;
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
            return  "INSERT INTO `plugins` (`PluginName`, `PluginDescription`, `PluginEnabled`, `PluginBLOB`) " +
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
                                        bool bPluginEnabled)
        {
            return "UPDATE `plugins` SET " +
                    "`PluginName` = '" + sPluginName + "', " +
                    "`PluginDescription` = '" + sPluginDescription + "', " +
                    "`PluginEnabled` = " + ((bPluginEnabled) ? 1 : 0) + " " +
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

        static public string GetSpecificTriggerTypesQuery(int dwTriggerTypesID)
        {
            return "SELECT * FROM `triggertypes` WHERE `TriggerTypeID` = " + dwTriggerTypesID + ";";
        }

        static public string NewTriggeTypesQuery(int dwConfigurationID,
                                        string sTriggerTypeName,
                                        string sTriggerTypeDescription)
        {
            return "INSERT INTO `triggertypes`(`ConfigurationID`, `TriggerTypeName`, `TriggerTypeDescription`) " +
                    "VALUES ( " +
                    dwConfigurationID + ", " +
                    "'" + sTriggerTypeName + "', " +
                    "'" + sTriggerTypeDescription + "' " +
                    ");";
        }

        static public string UpdateTriggeTypesQuery(int dwTriggerID,
                                            int dwConfigurationID,
                                            string sTriggerTypeName,
                                            string sTriggerTypeDescription)
        {
            return "UPDATE `triggertypes` SET " +
                    "`TriggerTypeName` = '" + sTriggerTypeName + "', " +
                    "`TriggerTypeDescription` = '" + sTriggerTypeDescription + "' " +
                    "WHERE `TriggerTypeID` = " + dwTriggerID + ";";
        }

        static public string GetTriggersCountQuery(int dwConfigurationID, int dwTriggerTypeID)
        {
            return "SELECT COUNT(*) FROM `triggers` WHERE `ConfigurationID` = " + dwConfigurationID + " AND `TriggerTypeID` = " + dwTriggerTypeID + ";";
        }

        static public string GetTriggersQuery(int dwConfigurationID, int dwTriggerTypeID)
        {
            return "SELECT * FROM `triggers` WHERE `ConfigurationID` = " + dwConfigurationID + " AND `TriggerTypeID` = " + dwTriggerTypeID + ";";
        }

        static public string GetSpecificTriggerQuery(int dwTriggerID)
        {
            return "SELECT * FROM `triggers` WHERE `TriggerID` = " + dwTriggerID + ";";
        }

        static public string NewTriggerQuery(int dwConfigurationID,
                                        int dwTriggerTypeID,
                                        string sTriggerName,
                                        string sTriggerDescription,
                                        string sTriggerValue,
                                        int dwReactionSequenceID,
                                        string sReactionValue,
                                        bool bTriggerEnabled)
        {
            return "INSERT INTO `triggers`(`ConfigurationID`, `TriggerTypeID`, `TriggerName`, `TriggerDescription`,`TriggerValue`, `ReactionSequenceID`, `ReactionValue`,`TriggerEnabled`) " +
                    "VALUES ( " +
                    dwConfigurationID + ", " +
                    dwTriggerTypeID + ", " +
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
                                            int dwTriggerTypeID,
                                            string sTriggerName,
                                            string sTriggerDescription,
                                            string sTriggerValue,
                                            int dwReactionSequenceID,
                                            string sReactionValue,
                                            bool bTriggerEnabled)
        {
            return "UPDATE `triggers` SET " +
                    "`ConfigurationID` = " + dwConfigurationID + ", " +
                    "`TriggerTypeID` = " + dwTriggerTypeID + ", " +
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

        static public string GetSpecificUserQuery(int dwUserID)
        {
            return "SELECT * FROM `users` WHERE `UserID` = " + dwUserID + ";";
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
            return "UPDATE `users` SET " +
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
                    "'" + sUserTypeName + "' " +
                    ");";
        }

        static public string UpdateUserTypeQuery(int dwUserTypeID,
                                        string sUserTypeName)
        {
            return "UPDATE `usertypes` SET " +
                    "`UserTypeName` = '" + sUserTypeName + "' " +
                    "WHERE `UserTypeID` = " + dwUserTypeID + ";";
        }

        static public string GetVariablesCountQuery(int dwConfigurationID)
        {
            return "SELECT COUNT(*) FROM `configurationvariables` WHERE `ConfigurationID` = " + dwConfigurationID + ";";
        }

        static public string GetVariablesQuery(int dwConfigurationID)
        {
            return "SELECT configurationvariables.ConfigurationVariableID, variables.VariableName, variables.VariableType, configurationvariables.VariableValue FROM `variables` LEFT JOIN `configurationvariables` ON variables.VariableID = configurationvariables.VariableID WHERE configurationvariables.ConfigurationID = " + dwConfigurationID + ";";
        }

        static public string GetVariablesForPluginQuery(int dwPluginID)
        {
            return "SELECT * FROM `variables` WHERE `PluginID` = " + dwPluginID + ";";
        }

        static public string UpdateConfigurationVariableQuery(  int dwConfigurationVariableID,
                                                                string sVariableValue)
        {
            return "UPDATE `configurationvariables` SET " +
                    "`VariableValue` = '" + sVariableValue + "' " +
                    "WHERE `ConfigurationVariableID` = " + dwConfigurationVariableID + ";";
        }

        static public string NewConfigurationVariableQuery(int dwConfigurationID,
                                                            int dwVariableID,
                                                            string sValue)
        {
            return "INSERT INTO `configurationvariables`(`ConfigurationID`, `VariableID`, `VariableValue`) " +
                    "VALUES ( " +
                    dwConfigurationID + ", " +
                    dwVariableID + ", " +
                    "'" + sValue + "' " +
                    ");";
        }

        static public string DeleteAccountQuery(int dwAccountID)
        {
            return "DELETE FROM `accounts` WHERE `AccountID` = " + dwAccountID + ";";
        }

        static public string DeleteConfigurationQuery(int dwConfigurationID)
        {
            return "DELETE FROM `configurations` WHERE `ConfigurationID` = " + dwConfigurationID + ";";
        }

        static public string DeletePluginQuery(int dwPluginID)
        {
            return "DELETE FROM `plugins` WHERE `PluginID` = " + dwPluginID + ";";
        }

        static public string DeleteSequenceQuery(int dwSequenceID)
        {
            return "DELETE FROM `sequences` WHERE `SequenceID` = " + dwSequenceID + ";";
        }

        static public string DeleteTriggerTypeQuery(int dwTriggerTypeID)
        {
            return "DELETE FROM `triggertypes` WHERE `TriggerTypeID` = " + dwTriggerTypeID + ";";
        }

        static public string DeleteTriggerQuery(int dwTriggerID)
        {
            return "DELETE FROM `triggers` WHERE `TriggerID` = " + dwTriggerID + ";";
        }

        static public string DeleteUserTypeQuery(int dwUserTypeID)
        {
            return "DELETE FROM `usertypes` WHERE `UserTypeID` = " + dwUserTypeID + ";";
        }

        static public string DeleteUserQuery(int dwUserID)
        {
            return "DELETE FROM `users` WHERE `UserID` = " + dwUserID + ";";
        }

        static public string ServerUpdatedQuery()
        {
            return "UPDATE `systemvariables` SET `ServerLastTimeUpdated` = CURRENT_TIME() WHERE `SystemVariablesID` = 1;";
        }

        static public string NewDataAvailableQuery()
        {
            return "UPDATE `systemvariables` SET `DataAvailableTime` = CURRENT_TIME() WHERE `SystemVariablesID` = 1;";
        }

        static public string ServerNeedsUpdateQuery()
        {
            return "SELECT `DataAvailableTime` > `ServerLastTimeUpdated` AS UpdateRequired FROM `systemvariables` WHERE `SystemVariablesID` = 1";
        }

        static public string SaveLogMessage (   string sLogType,
                                                string sLogMessage,
                                                string sLogLink,
                                                string sComponent,
                                                string sUser )
        {
            return "INSERT INTO `logs`(`LogType`, `LogMessage`, `LogLink`, `LogTime`, `LogComponent`, `LogUserName`) " +
                    "VALUES ( " +
                    "'" + sLogType + "', " +
                    "'" + sLogMessage + "', " +
                    "'" + sLogLink + "', " +
                    "CURRENT_TIME(), " +
                    "'" + sComponent + "', " +
                    "'" + sUser + "' " +
                    ");";
        }

        static public string NewConfigurationStatus(int dwConfigurationId)
        {
            return "INSERT INTO `configurationstatus` (`ConfigurationID`) VALUES (" + dwConfigurationId + ");";
        }

        static public string SaveConfigurationStatusMessage(int dwConfigurationId,
                                                            string sStatus)
        {
            return "UPDATE `configurationstatus` SET `Status` = '" + sStatus + "', `Time` = CURRENT_TIME() WHERE `ConfigurationID` = " + dwConfigurationId + ";";
        }

        static public string GetConfigurationsStatus()
        {
            return "SELECT ConfigurationID, Status, TIMESTAMPDIFF(SECOND, Time, NOW()) As Expired FROM `configurationstatus`;";
        }

        static string FilteredLastLogsString(string sComponentFilter,
                                            string sUserNameFilter,
                                            string sLogType,
                                            string sMessage,
                                            string sLink)
        {
            string query = "SELECT * FROM `logs` WHERE 1=1 ";

            if (sComponentFilter != "")
            {
                query += " AND `LogComponent` 'LIKE %" + sComponentFilter + "%'";
            }

            if (sUserNameFilter != "")
            {
                query += " AND `LogUserName` LIKE '%" + sUserNameFilter + "%'";
            }

            if (sLogType != "")
            {
                query += " AND `LogType` LIKE '%" + sLogType + "%'";
            }

            if (sMessage != "")
            {
                query += " AND `LogMessage` LIKE '%" + sMessage + "%'";
            }

            if (sLink != "")
            {
                query += " AND `LogLink` LIKE '%" + sLink + "%'";
            }

            return query + " ORDER BY `LogID` DESC LIMIT 100";
        }

        static public string GetLastLogs(   string sComponentFilter,
                                            string sUserNameFilter,
                                            string sLogType,
                                            string sMessage,
                                            string sLink)
        {
            return "SELECT * FROM ( " + FilteredLastLogsString(sComponentFilter,
                                                               sUserNameFilter,
                                                               sLogType,
                                                               sMessage,
                                                               sLink) + " ) sub ORDER BY `LogID` DESC;";
        }

        static public string GetLastLogsCount(  string sComponentFilter,
                                                string sUserNameFilter,
                                                string sLogType,
                                                string sMessage,
                                                string sLink)
        {
            return "SELECT COUNT(*) FROM ( " + FilteredLastLogsString(  sComponentFilter,
                                                                        sUserNameFilter,
                                                                        sLogType,
                                                                        sMessage,
                                                                        sLink) + " ) sub ORDER BY `LogID` DESC;";
        }

        static public string NewPluginVariable(int dwPluginID, string sVariableName, string sVariableType)
        {
            return "INSERT INTO `variables`(`PluginID`, `VariableName`, `VariableType`) " +
                    "VALUES ( " +
                    dwPluginID + ", " +
                    "'" + sVariableName + "', " +
                    "'" + sVariableType + "' " +
                    ");";
        }

        static public string NewPluginSequence(int dwPluginID, string sSequenceName)
        {
            return "INSERT INTO `sequences`(`PluginID`, `SequenceName`) " +
                    "VALUES ( " +
                    dwPluginID + ", " +
                    "'" + sSequenceName + "' " +
                    ");";
        }
    }
}
