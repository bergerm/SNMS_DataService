using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMS_DataService.Queries
{
    class QueryManager
    {
        public string GetPluginsQuery()
        {
            return "SELECT * FROM `queries`;";
        }

        public string NewPluginQuery(   string sPluginName,
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

        public string UpdatePluginQuery(int dwPluginID, 
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

        public string GetAccountsQuery(int dwPluginID)
        {
            return "SELECT * FROM `accounts` WHERE `PluginID` = " + dwPluginID + ";";
        }

        public string NewAccountQuery(  int dwPluginID,
                                        string sAccountName,
                                        string sAccountDescription,
                                        string sAccountUserName,
                                        string sAccountPassword)
        {
            return "INSERT INTO `accounts`(`AccountName`, `AccountDescription`, `AccountUserName`, `AccountPassword`) " +
                    "VALUES ( " +
                    dwPluginID +
                    "'" + sAccountName + "', " +
                    "'" + sAccountDescription + "', " +
                    "'" + sAccountUserName + "', " +
                    "'" + sAccountPassword + "'," +
                    ");";
        }

        public string UpdateAccountQuery(   int dwAccountID,
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

        public string GetConfigurationQuery(int dwAccountID)
        {
            return "SELECT * FROM `configurations` WHERE `AccountID` = " + dwAccountID + ";";
        }

        public string NewConfigurationQuery(    int dwAccountID,
                                                string sConfigurationName,
                                                string sConfigurationDescription,
                                                bool bConfigurationEnabled)
        {
            return "INSERT INTO `configurations`(`AccountID`, `ConfigurationName`, `ConfigurationDescription`, `ConfiguratonEnabled`) " +
                    "VALUES ( " +
                    dwAccountID + ", " +
                    "'" + sConfigurationName + ", " +
                    "'" + sConfigurationDescription + ", " +
                    ((bConfigurationEnabled) ? 1 : 0) +
                    ");";
        }

        public string UpdateConfigurationQuery( int dwConfigurationID,
                                                int dwAccountID,
                                                string sConfigurationName,
                                                string sConfigurationDescription,
                                                bool bConfigurationEnabled)
        {
            return "UPDATE `configurations` SET " +
                    "`AccountID` = " + dwAccountID + ", " +
                    "`ConfigurationName` = '" + sConfigurationName + "', " +
                    "`ConfigurationDescription` = '" + sConfigurationDescription + "', " +
                    "`ConfiguratonEnabled` = " + ((bConfigurationEnabled) ? 1 : 0) + " " +
                    "WHERE `ConfigurationID` = " + dwConfigurationID + ";";
        }

        public string GetSequencesQuery(int dwConfigurationID)
        {
            return "SELECT * FROM `sequences` WHERE `ConfigurationID` = " + dwConfigurationID + ";";
        }

        public string NewSequenceQuery( int dwConfigurationID,
                                        string sSequenceName,
                                        bool bSequenceEnabled)
        {
            return "INSERT INTO `sequences`(`ConfigurationID`, `SequenceName`, `SequenceEnabled`) " +
                    "VALUES ( " +
                    dwConfigurationID + ", " +
                    "'" + sSequenceName + ", " +
                    ((bSequenceEnabled) ? 1 : 0) +
                    ");";
        }

        public string UpdateConfigurationQuery( int dwSequenceID,
                                                int dwConfigurationID,
                                                string sSequenceName,
                                                bool bSequenceEnabled)
        {
            return "UPDATE `sequences` SET " +
                    "`ConfigurationID` = " + dwConfigurationID + ", " +
                    "`SequenceName` = '" + sSequenceName + "', " +
                    "`SequenceEnabled` = " + ((bSequenceEnabled) ? 1 : 0) + " " +
                    "WHERE `ConfigurationID` = " + dwSequenceID + ";";
        }

        public string GetTriggersQuery(int dwConfigurationID)
        {
            return "SELECT * FROM `triggers` WHERE `ConfigurationID` = " + dwConfigurationID + ";";
        }

        public string NewTriggerQuery(  int dwConfigurationID,
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

        public string UpdateTriggerQuery(   int dwTriggerID,
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

        public string GetUsersQuery()
        {
            return "SELECT * FROM `users`;";
        }

        public string NewUserQuery( string sUserName,
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

        public string UpdateUserQuery(  int dwUserID,
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

        public string GetUserTypesQuery()
        {
            return "SELECT * FROM `userTypes`;";
        }

        public string NewUserTypeQuery( int dwUserTypeID,
                                        string sUserTypeName )
        {
            return "INSERT INTO `usertypes`(`UserTypeID`, `UserTypeName`) " +
                    "VALUES ( " +
                    dwUserTypeID + ", " +
                    "'" + sUserTypeName + "', " +
                    ");";
        }

        public string UpdateUserQuery(  int dwUserTypeID,
                                        string sUserTypeName)
        {
            return "UPDATE `usertypes` SET " +
                    "`UserTypeName` = '" + sUserTypeName + "', " +
                    "WHERE `UserTypeID` = " + dwUserTypeID + ";";
        }
    }
}
