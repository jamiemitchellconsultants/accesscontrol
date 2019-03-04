-- MySQL dump 10.13  Distrib 8.0.15, for Win64 (x86_64)
--
-- Host: localhost    Database: accesscontrol
-- ------------------------------------------------------
-- Server version	8.0.15

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
 SET NAMES utf8mb4 ;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `action`
--

DROP TABLE IF EXISTS `action`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `action` (
  `ActionId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `ActionName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`ActionId`),
  UNIQUE KEY `ActionName` (`ActionName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;


--
-- Table structure for table `applicationarea`
--

DROP TABLE IF EXISTS `applicationarea`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `applicationarea` (
  `ApplicationAreaId` char(36) NOT NULL,
  `ApplicationAreaName` char(255) NOT NULL,
  PRIMARY KEY (`ApplicationAreaId`),
  UNIQUE KEY `ApplicationAreaName` (`ApplicationAreaName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;


--
-- Table structure for table `group`
--

DROP TABLE IF EXISTS `group`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `group` (
  `GroupId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `GroupName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`GroupId`),
  UNIQUE KEY `GroupName` (`GroupName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;


--
-- Table structure for table `grouprole`
--

DROP TABLE IF EXISTS `grouprole`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `grouprole` (
  `GroupRoleId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `GroupId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `RoleId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`GroupRoleId`),
  UNIQUE KEY `RoleGroup` (`RoleId`,`GroupId`),
  KEY `GroupId` (`GroupId`),
  KEY `RoleId` (`RoleId`),
  CONSTRAINT `FK_grouprole_group` FOREIGN KEY (`GroupId`) REFERENCES `group` (`GroupId`),
  CONSTRAINT `FK_grouprole_role` FOREIGN KEY (`RoleId`) REFERENCES `role` (`RoleId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;


--
-- Table structure for table `jwtissuer`
--

DROP TABLE IF EXISTS `jwtissuer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `jwtissuer` (
  `JWTissuerId` char(36) NOT NULL,
  `IssuerName` char(255) NOT NULL,
  `SubjectClaimName` char(255) NOT NULL,
  PRIMARY KEY (`JWTissuerId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `jwtissuer`
--

--
-- Table structure for table `permission`
--

DROP TABLE IF EXISTS `permission`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `permission` (
  `PermissionId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `RoleId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `ResourceActionId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Deny` tinyint(1) NOT NULL,
  PRIMARY KEY (`PermissionId`),
  UNIQUE KEY `uniquepermission` (`RoleId`,`ResourceActionId`),
  KEY `permissionrole` (`RoleId`),
  KEY `permissionresourceaction` (`ResourceActionId`),
  CONSTRAINT `FK_permission_resourceaction` FOREIGN KEY (`ResourceActionId`) REFERENCES `resourceaction` (`ResourceActionId`),
  CONSTRAINT `FK_permission_role` FOREIGN KEY (`RoleId`) REFERENCES `role` (`RoleId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;


--
-- Table structure for table `resource`
--

DROP TABLE IF EXISTS `resource`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `resource` (
  `ResourceId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `ResourceName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `ApplicationAreaId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`ResourceId`),
  UNIQUE KEY `resourcename` (`ResourceName`),
  KEY `resourceapplicationarea` (`ApplicationAreaId`),
  CONSTRAINT `FK_Resoruce_Apparea` FOREIGN KEY (`ApplicationAreaId`) REFERENCES `applicationarea` (`ApplicationAreaId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `resourceaction`
--

DROP TABLE IF EXISTS `resourceaction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `resourceaction` (
  `ResourceActionId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `ResourceId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `ActionId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`ResourceActionId`),
  UNIQUE KEY `uniqueresourceaction` (`ActionId`,`ResourceId`),
  KEY `resourceactionresource` (`ResourceId`),
  KEY `resourceactionaction` (`ActionId`),
  CONSTRAINT `FK_resourceaction_action` FOREIGN KEY (`ActionId`) REFERENCES `action` (`ActionId`),
  CONSTRAINT `FK_resourceaction_resource` FOREIGN KEY (`ResourceId`) REFERENCES `resource` (`ResourceId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `role`
--

DROP TABLE IF EXISTS `role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `role` (
  `RoleId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Role Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`RoleId`),
  UNIQUE KEY `rolename` (`Role Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;


--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `user` (
  `UserID` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `LocalName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `SubjectId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`UserID`),
  UNIQUE KEY `localname` (`LocalName`),
  UNIQUE KEY `usersubjectid` (`SubjectId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;


--
-- Table structure for table `usergroup`
--

DROP TABLE IF EXISTS `usergroup`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `usergroup` (
  `UserGroupId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `GroupId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `UserId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`UserGroupId`),
  UNIQUE KEY `Index 4` (`UserId`,`GroupId`),

  KEY `Index 2` (`GroupId`),
  KEY `Index 3` (`UserId`),
  CONSTRAINT `FK_usergroup_group` FOREIGN KEY (`GroupId`) REFERENCES `group` (`GroupId`),
  CONSTRAINT `FK_usergroup_user` FOREIGN KEY (`UserId`) REFERENCES `user` (`UserID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;


--
-- Temporary view structure for view `userpermissions`
--

DROP TABLE IF EXISTS `userpermissions`;
/*!50001 DROP VIEW IF EXISTS `userpermissions`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8mb4;
/*!50001 CREATE VIEW `userpermissions` AS SELECT 
 1 AS `LocalName`,
 1 AS `PermissionId`,
 1 AS `SubjectId`,
 1 AS `Deny`,
 1 AS `ResourceActionId`,
 1 AS `ActionId`,
 1 AS `ActionName`,
 1 AS `ResourceId`,
 1 AS `ResourceName`*/;
SET character_set_client = @saved_cs_client;

--
-- Final view structure for view `userpermissions`
--

/*!50001 DROP VIEW IF EXISTS `userpermissions`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_general_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `userpermissions` AS select `user`.`LocalName` AS `LocalName`,`permission`.`PermissionId` AS `PermissionId`,`user`.`SubjectId` AS `SubjectId`,`permission`.`Deny` AS `Deny`,`resourceaction`.`ResourceActionId` AS `ResourceActionId`,`action`.`ActionId` AS `ActionId`,`action`.`ActionName` AS `ActionName`,`resource`.`ResourceId` AS `ResourceId`,`resource`.`ResourceName` AS `ResourceName` from ((((((((`user` join `usergroup` on((`user`.`UserID` = `usergroup`.`UserId`))) join `group` on((`usergroup`.`GroupId` = `group`.`GroupId`))) join `grouprole` on((`group`.`GroupId` = `grouprole`.`GroupId`))) join `role` on((`grouprole`.`RoleId` = `role`.`RoleId`))) join `permission` on((`role`.`RoleId` = `permission`.`RoleId`))) join `resourceaction` on((`permission`.`ResourceActionId` = `resourceaction`.`ResourceActionId`))) join `resource` on((`resourceaction`.`ResourceId` = `resource`.`ResourceId`))) join `action` on((`resourceaction`.`ActionId` = `action`.`ActionId`))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2019-02-26 10:00:32
