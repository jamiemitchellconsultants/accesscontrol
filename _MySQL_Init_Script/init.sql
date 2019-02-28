﻿-- MySQL dump 10.13  Distrib 8.0.15, for Win64 (x86_64)
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
  `ActionId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ActionName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`ActionId`),
  UNIQUE KEY `ActionName` (`ActionName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hu_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `action`
--

LOCK TABLES `action` WRITE;
/*!40000 ALTER TABLE `action` DISABLE KEYS */;
INSERT INTO `action` VALUES ('8429B992-19E4-4799-8BCF-F1C36C0B0564','All'),('BE73BD80-289D-4BCD-B5D4-1F91474236B3','Create'),('6B3379B0-9D21-40F7-A88E-0E11BEBE8B20','Delete'),('A69A0CA4-20D2-4FFB-ACD3-1D240A83E8D9','Edit'),('06FCAE76-3E38-458C-9814-2DA5F9FF4A3B','Get');
/*!40000 ALTER TABLE `action` ENABLE KEYS */;
UNLOCK TABLES;

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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `applicationarea`
--

LOCK TABLES `applicationarea` WRITE;
/*!40000 ALTER TABLE `applicationarea` DISABLE KEYS */;
INSERT INTO `applicationarea` VALUES ('88ede3bb-540b-4263-966b-89b85068badc','Global');
/*!40000 ALTER TABLE `applicationarea` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `group`
--

DROP TABLE IF EXISTS `group`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `group` (
  `GroupId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `GroupName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`GroupId`),
  UNIQUE KEY `GroupName` (`GroupName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_cs_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `group`
--

LOCK TABLES `group` WRITE;
/*!40000 ALTER TABLE `group` DISABLE KEYS */;
INSERT INTO `group` VALUES ('73087588-EFFC-4421-8315-5EAD1EC43B0A','Admin'),('AF6EB078-92BD-4B2A-BFFC-A2E8CF313A8D','Manager'),('6264D8FC-714C-42A8-9347-236D5C47B5DE','Sales'),('F03611F4-4330-4404-854D-CF66A3439D83','Support');
/*!40000 ALTER TABLE `group` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `grouprole`
--

DROP TABLE IF EXISTS `grouprole`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `grouprole` (
  `GroupRoleId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `GroupId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `RoleId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`GroupRoleId`),
  UNIQUE KEY `RoleGroup` (`RoleId`,`GroupId`),
  KEY `GroupId` (`GroupId`),
  KEY `RoleId` (`RoleId`),
  CONSTRAINT `FK_grouprole_group` FOREIGN KEY (`GroupId`) REFERENCES `group` (`GroupId`),
  CONSTRAINT `FK_grouprole_role` FOREIGN KEY (`RoleId`) REFERENCES `role` (`RoleId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_cs_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `grouprole`
--

LOCK TABLES `grouprole` WRITE;
/*!40000 ALTER TABLE `grouprole` DISABLE KEYS */;
INSERT INTO `grouprole` VALUES ('13C6EDA7-E283-4D94-8E94-8F3538B06EE1','6264D8FC-714C-42A8-9347-236D5C47B5DE','01C7E190-E09D-409C-8C7C-3E369D45E5E0'),('C700E2F9-AE2F-4BB4-A2CE-B5B1B675AE8E','73087588-EFFC-4421-8315-5EAD1EC43B0A','1DB28791-3E5F-423C-BAF7-E8ABF315F5D7'),('B0909507-B960-4101-9835-7DF4904FDBFD','AF6EB078-92BD-4B2A-BFFC-A2E8CF313A8D','1DB28791-3E5F-423C-BAF7-E8ABF315F5D7'),('0C39F995-350A-47FC-B93F-1A55709BF540','73087588-EFFC-4421-8315-5EAD1EC43B0A','69CB05CF-0262-4ECB-A587-7A0096310447'),('EB9E33D9-DFFD-4170-8527-1219779D6ACD','73087588-EFFC-4421-8315-5EAD1EC43B0A','7A9CA11F-EE41-440C-A80F-65B01CEE32A7'),('5E90F9E2-46AF-45F6-9539-6FF531511A03','AF6EB078-92BD-4B2A-BFFC-A2E8CF313A8D','7A9CA11F-EE41-440C-A80F-65B01CEE32A7'),('20F019E6-D633-49A6-840C-084B9311A62B','F03611F4-4330-4404-854D-CF66A3439D83','975882B0-E15C-4AC6-B0C1-5646B61C36EB');
/*!40000 ALTER TABLE `grouprole` ENABLE KEYS */;
UNLOCK TABLES;

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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `jwtissuer`
--

LOCK TABLES `jwtissuer` WRITE;
/*!40000 ALTER TABLE `jwtissuer` DISABLE KEYS */;
/*!40000 ALTER TABLE `jwtissuer` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `permission`
--

DROP TABLE IF EXISTS `permission`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `permission` (
  `PermissionId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `RoleId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ResourceActionId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Deny` tinyint(1) NOT NULL,
  PRIMARY KEY (`PermissionId`),
  UNIQUE KEY `uniquepermission` (`RoleId`,`ResourceActionId`),
  KEY `permissionrole` (`RoleId`),
  KEY `permissionresourceaction` (`ResourceActionId`),
  CONSTRAINT `FK_permission_resourceaction` FOREIGN KEY (`ResourceActionId`) REFERENCES `resourceaction` (`ResourceActionId`),
  CONSTRAINT `FK_permission_role` FOREIGN KEY (`RoleId`) REFERENCES `role` (`RoleId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_cs_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `permission`
--

LOCK TABLES `permission` WRITE;
/*!40000 ALTER TABLE `permission` DISABLE KEYS */;
INSERT INTO `permission` VALUES ('14A335E2-F75C-408C-9BFA-2347D88B4327','1DB28791-3E5F-423C-BAF7-E8ABF315F5D7','97604CB4-2773-4E18-963E-20FDC5AA44AA',0),('154696E6-F429-48EB-A69D-ACA3172D8E09','7A9CA11F-EE41-440C-A80F-65B01CEE32A7','4091B938-C9C5-4052-8027-CAD129359AFD',0),('1D4B84CF-0BE3-4B8D-82B3-F5A588095C34','01C7E190-E09D-409C-8C7C-3E369D45E5E0','EDFF5585-6B24-4628-9D4D-D695875012EF',0),('417694CB-FE80-4E79-8980-FF33B04B8081','01C7E190-E09D-409C-8C7C-3E369D45E5E0','03FEDA2B-0EF5-4E3E-94DB-54F02A8C95FC',0),('43321101-3A8C-4004-AA76-1F8124240835','69CB05CF-0262-4ECB-A587-7A0096310447','A0230F01-3780-427A-9EBA-DA3B62167687',0),('47CF22E2-CA60-4A40-BA77-2010A7AC43CC','975882B0-E15C-4AC6-B0C1-5646B61C36EB','97604CB4-2773-4E18-963E-20FDC5AA44AA',0),('4B93BB46-147B-46C9-91B6-2468CAF96ED1','1DB28791-3E5F-423C-BAF7-E8ABF315F5D7','FA840DD5-84CF-44BD-82A4-7971B58643F2',0),('4EDB5B18-4608-49E8-A05A-DEFBCF62CE7F','975882B0-E15C-4AC6-B0C1-5646B61C36EB','FA840DD5-84CF-44BD-82A4-7971B58643F2',0),('54859ECE-C262-4BB8-8FBF-3C2CE316E9C6','7A9CA11F-EE41-440C-A80F-65B01CEE32A7','FA840DD5-84CF-44BD-82A4-7971B58643F2',0),('6F18C987-F6E8-415C-B25F-DB493DAE5225','01C7E190-E09D-409C-8C7C-3E369D45E5E0','19AB0511-3D84-45C8-8F19-9482FA87BFB8',0),('7434EC45-ABC5-49DC-894C-09AA6DDEAD8E','7A9CA11F-EE41-440C-A80F-65B01CEE32A7','6CB47B13-D1E1-4F64-BEE5-5A109C484F25',0),('837453C8-1DA4-4581-BF3F-C8703223A542','975882B0-E15C-4AC6-B0C1-5646B61C36EB','EDFF5585-6B24-4628-9D4D-D695875012EF',0),('8A885BC3-7A5B-435E-A2E7-EA682B79A1AA','1DB28791-3E5F-423C-BAF7-E8ABF315F5D7','E66511A5-DF50-49CE-9DF1-435E7AB45017',0),('8AC73617-4C33-4299-8597-7081850B54F5','7A9CA11F-EE41-440C-A80F-65B01CEE32A7','03FEDA2B-0EF5-4E3E-94DB-54F02A8C95FC',0),('A0392E5E-E52F-4E1E-8AFC-7C0516CD08CA','01C7E190-E09D-409C-8C7C-3E369D45E5E0','76662E66-0D4D-4736-833A-98D460695054',0),('CEE4128C-E5FF-478B-AB49-886A3DAC7F01','1DB28791-3E5F-423C-BAF7-E8ABF315F5D7','03FEDA2B-0EF5-4E3E-94DB-54F02A8C95FC',0),('F216DA28-AB43-402D-AD30-8DB1C9F4D34A','975882B0-E15C-4AC6-B0C1-5646B61C36EB','76662E66-0D4D-4736-833A-98D460695054',0);
/*!40000 ALTER TABLE `permission` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `resource`
--

DROP TABLE IF EXISTS `resource`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `resource` (
  `ResourceId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ResourceName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ApplicationAreaId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`ResourceId`),
  UNIQUE KEY `resourcename` (`ResourceName`),
  KEY `resourceapplicationarea` (`ApplicationAreaId`),
  CONSTRAINT `FK_Resoruce_Apparea` FOREIGN KEY (`ApplicationAreaId`) REFERENCES `applicationarea` (`ApplicationAreaId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_cs_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `resource`
--

LOCK TABLES `resource` WRITE;
/*!40000 ALTER TABLE `resource` DISABLE KEYS */;
INSERT INTO `resource` VALUES ('2FFE6714-5033-4A51-A939-3B5EABBE7705','Location','88ede3bb-540b-4263-966b-89b85068badc'),('34A94DA6-02C5-45E3-8BCA-97354412A506','User','88ede3bb-540b-4263-966b-89b85068badc'),('6F2EAD69-725C-4401-AAB7-AB06D0460E27','Brand','88ede3bb-540b-4263-966b-89b85068badc'),('7E34DF0C-608B-46FB-B667-13943AABE8C4','Offer','88ede3bb-540b-4263-966b-89b85068badc');
/*!40000 ALTER TABLE `resource` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `resourceaction`
--

DROP TABLE IF EXISTS `resourceaction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `resourceaction` (
  `ResourceActionId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ResourceId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ActionId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`ResourceActionId`),
  UNIQUE KEY `uniqueresourceaction` (`ActionId`,`ResourceId`),
  KEY `resourceactionresource` (`ResourceId`),
  KEY `resourceactionaction` (`ActionId`),
  CONSTRAINT `FK_resourceaction_action` FOREIGN KEY (`ActionId`) REFERENCES `action` (`ActionId`),
  CONSTRAINT `FK_resourceaction_resource` FOREIGN KEY (`ResourceId`) REFERENCES `resource` (`ResourceId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_cs_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `resourceaction`
--

LOCK TABLES `resourceaction` WRITE;
/*!40000 ALTER TABLE `resourceaction` DISABLE KEYS */;
INSERT INTO `resourceaction` VALUES ('A304CDFF-28E7-4496-9503-F79BAA023CD0','2FFE6714-5033-4A51-A939-3B5EABBE7705','06FCAE76-3E38-458C-9814-2DA5F9FF4A3B'),('03FEDA2B-0EF5-4E3E-94DB-54F02A8C95FC','34A94DA6-02C5-45E3-8BCA-97354412A506','06FCAE76-3E38-458C-9814-2DA5F9FF4A3B'),('FA840DD5-84CF-44BD-82A4-7971B58643F2','6F2EAD69-725C-4401-AAB7-AB06D0460E27','06FCAE76-3E38-458C-9814-2DA5F9FF4A3B'),('EDFF5585-6B24-4628-9D4D-D695875012EF','7E34DF0C-608B-46FB-B667-13943AABE8C4','06FCAE76-3E38-458C-9814-2DA5F9FF4A3B'),('4204CF6D-7386-48C6-9D12-D91C8BDD4836','7E34DF0C-608B-46FB-B667-13943AABE8C4','6B3379B0-9D21-40F7-A88E-0E11BEBE8B20'),('4091B938-C9C5-4052-8027-CAD129359AFD','2FFE6714-5033-4A51-A939-3B5EABBE7705','8429B992-19E4-4799-8BCF-F1C36C0B0564'),('A0230F01-3780-427A-9EBA-DA3B62167687','34A94DA6-02C5-45E3-8BCA-97354412A506','8429B992-19E4-4799-8BCF-F1C36C0B0564'),('6CB47B13-D1E1-4F64-BEE5-5A109C484F25','7E34DF0C-608B-46FB-B667-13943AABE8C4','8429B992-19E4-4799-8BCF-F1C36C0B0564'),('97604CB4-2773-4E18-963E-20FDC5AA44AA','6F2EAD69-725C-4401-AAB7-AB06D0460E27','A69A0CA4-20D2-4FFB-ACD3-1D240A83E8D9'),('76662E66-0D4D-4736-833A-98D460695054','7E34DF0C-608B-46FB-B667-13943AABE8C4','A69A0CA4-20D2-4FFB-ACD3-1D240A83E8D9'),('E66511A5-DF50-49CE-9DF1-435E7AB45017','6F2EAD69-725C-4401-AAB7-AB06D0460E27','BE73BD80-289D-4BCD-B5D4-1F91474236B3'),('19AB0511-3D84-45C8-8F19-9482FA87BFB8','7E34DF0C-608B-46FB-B667-13943AABE8C4','BE73BD80-289D-4BCD-B5D4-1F91474236B3');
/*!40000 ALTER TABLE `resourceaction` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `role`
--

DROP TABLE IF EXISTS `role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `role` (
  `RoleId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Role Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`RoleId`),
  UNIQUE KEY `rolename` (`Role Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_da_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `role`
--

LOCK TABLES `role` WRITE;
/*!40000 ALTER TABLE `role` DISABLE KEYS */;
INSERT INTO `role` VALUES ('1DB28791-3E5F-423C-BAF7-E8ABF315F5D7','Customer Admin'),('975882B0-E15C-4AC6-B0C1-5646B61C36EB','Customer Support'),('7A9CA11F-EE41-440C-A80F-65B01CEE32A7','Sales Admin'),('01C7E190-E09D-409C-8C7C-3E369D45E5E0','Sales Support'),('69CB05CF-0262-4ECB-A587-7A0096310447','User Admin');
/*!40000 ALTER TABLE `role` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `user` (
  `UserID` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `LocalName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SubjectId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`UserID`),
  UNIQUE KEY `localname` (`LocalName`),
  UNIQUE KEY `usersubjectid` (`SubjectId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_cs_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES ('119881FE-E76D-47A6-B041-8EF9031C8758','James','769ceb3f-d9cf-4aec-88f6-f7cc713d97af'),('4B390225-E31F-4FBC-B3D6-DB747B5E602B','June','0010'),('669E6966-C1B1-4EC3-B81E-BD71AD798C66','Frank','0009'),('81E76546-92D8-4E5C-BFCD-5DAEB4085230','Bill','0001'),('9AF6F9B6-05AB-485A-AD38-B356E5C6F4C9','Anne','0020'),('C538FB5B-0893-4263-99E5-015D7196E141','Joe','0003'),('D94A1434-04A3-49A6-85DF-B787D5064F4C','Tim','0004');
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usergroup`
--

DROP TABLE IF EXISTS `usergroup`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `usergroup` (
  `UserGroupId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `GroupId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UserId` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`UserGroupId`),
  UNIQUE KEY `Index 4` (`UserId`,`GroupId`),

  KEY `Index 2` (`GroupId`),
  KEY `Index 3` (`UserId`),
  CONSTRAINT `FK_usergroup_group` FOREIGN KEY (`GroupId`) REFERENCES `group` (`GroupId`),
  CONSTRAINT `FK_usergroup_user` FOREIGN KEY (`UserId`) REFERENCES `user` (`UserID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_cs_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usergroup`
--

LOCK TABLES `usergroup` WRITE;
/*!40000 ALTER TABLE `usergroup` DISABLE KEYS */;
INSERT INTO `usergroup` VALUES ('C04EE048-6C2D-4DF1-B5A2-493B2D24E40D','73087588-EFFC-4421-8315-5EAD1EC43B0A','119881FE-E76D-47A6-B041-8EF9031C8758'),('9060110C-2B5E-4F38-8EC2-FEDE0523D73E','AF6EB078-92BD-4B2A-BFFC-A2E8CF313A8D','4B390225-E31F-4FBC-B3D6-DB747B5E602B'),('87EF3984-9FF8-48CC-9511-E33D75EFD8B7','AF6EB078-92BD-4B2A-BFFC-A2E8CF313A8D','81E76546-92D8-4E5C-BFCD-5DAEB4085230'),('9A775CA3-A9E9-4C70-A8AA-A5BCE73AAD57','6264D8FC-714C-42A8-9347-236D5C47B5DE','9AF6F9B6-05AB-485A-AD38-B356E5C6F4C9'),('6B3C9EE4-A79D-4A0A-B15F-06B6C25FFA4C','F03611F4-4330-4404-854D-CF66A3439D83','9AF6F9B6-05AB-485A-AD38-B356E5C6F4C9'),('01DCD8BC-8393-443B-9174-A4DA5C1AE348','AF6EB078-92BD-4B2A-BFFC-A2E8CF313A8D','C538FB5B-0893-4263-99E5-015D7196E141'),('5DA3F9AA-A5CD-4A50-B9F0-FC31EF712EB0','6264D8FC-714C-42A8-9347-236D5C47B5DE','D94A1434-04A3-49A6-85DF-B787D5064F4C');
/*!40000 ALTER TABLE `usergroup` ENABLE KEYS */;
UNLOCK TABLES;

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
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
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