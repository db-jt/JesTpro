-- MySQL dump 10.13  Distrib 8.0.19, for Win64 (x86_64)
--
-- Host: localhost    Database: jestpro_test
-- ------------------------------------------------------
-- Server version	5.7.20-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `audit`
--

DROP TABLE IF EXISTS `audit`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `audit` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `User` varchar(256) DEFAULT NULL,
  `TableName` varchar(256) DEFAULT NULL,
  `DateTime` datetime DEFAULT NULL,
  `KeyValues` longtext,
  `OldValues` longtext,
  `NewValues` longtext,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=9769 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `customer`
--

DROP TABLE IF EXISTS `customer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customer` (
  `Id` char(36) NOT NULL,
  `IdType` char(36) NOT NULL,
  `FirstName` varchar(100) NOT NULL,
  `LastName` varchar(100) NOT NULL,
  `FiscalCode` char(16) DEFAULT NULL,
  `Address` varchar(255) DEFAULT NULL,
  `City` varchar(256) DEFAULT NULL,
  `PostalCode` varchar(256) DEFAULT NULL,
  `State` varchar(256) DEFAULT NULL,
  `Country` varchar(256) DEFAULT NULL,
  `BirthDate` datetime NOT NULL,
  `TutorType` varchar(45) DEFAULT NULL,
  `TutorFirstName` varchar(100) DEFAULT NULL,
  `TutorLastName` varchar(100) DEFAULT NULL,
  `TutorFiscalCode` char(16) DEFAULT NULL,
  `TutorBirthDate` datetime DEFAULT NULL,
  `TutorPhoneNumber` varchar(45) DEFAULT NULL,
  `TutorEmail` varchar(256) DEFAULT NULL,
  `Note` text,
  `MembershipFee` decimal(15,2) DEFAULT NULL,
  `MembershipLastPayDate` datetime DEFAULT NULL,
  `MembershipFeeExpiryDate` datetime DEFAULT NULL,
  `PhoneNumber` varchar(45) DEFAULT NULL,
  `PhoneNumberAlternative` varchar(45) DEFAULT NULL,
  `Email` varchar(256) DEFAULT NULL,
  `MedicalCertificateExpiration` datetime DEFAULT NULL,
  `XCreateDate` datetime DEFAULT NULL,
  `XUpdateDate` datetime DEFAULT NULL,
  `XDeleteDate` datetime DEFAULT NULL,
  `XLastEditUser` varchar(45) DEFAULT NULL,
  `XCreationUser` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `customer_productinstance`
--

DROP TABLE IF EXISTS `customer_productinstance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customer_productinstance` (
  `Id` char(36) NOT NULL,
  `IdCustomer` char(36) NOT NULL,
  `IdProductInstance` char(36) NOT NULL,
  `IdReceipt` char(36) DEFAULT NULL,
  `Name` varchar(256) DEFAULT NULL,
  `Description` longtext,
  `XCreationUser` varchar(45) DEFAULT NULL,
  `Price` decimal(15,2) NOT NULL,
  `Discount` decimal(15,2) DEFAULT NULL,
  `CostAmount` decimal(15,2) NOT NULL,
  `DiscountDescription` text,
  `DiscountReversal` decimal(15,2) DEFAULT NULL,
  `DiscountType` int(11) DEFAULT NULL,
  `IdReversal` char(36) DEFAULT NULL,
  `ReversalDate` datetime DEFAULT NULL,
  `ReversalCredit` decimal(15,2) DEFAULT NULL,
  `ExpirationDate` datetime DEFAULT NULL,
  `RenewedDate` datetime DEFAULT NULL,
  `IdRenewed` char(36) DEFAULT NULL,
  `XCreateDate` datetime DEFAULT NULL,
  `XUpdateDate` datetime DEFAULT NULL,
  `XDeleteDate` datetime DEFAULT NULL,
  `XLastEditUser` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `customer_type`
--

DROP TABLE IF EXISTS `customer_type`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customer_type` (
  `Id` char(36) NOT NULL,
  `Name` varchar(100) NOT NULL,
  `Description` text,
  `CostAmount` decimal(15,2) NOT NULL,
  `Duration` varchar(10) DEFAULT NULL,
  `XCreateDate` datetime DEFAULT NULL,
  `XUpdateDate` datetime DEFAULT NULL,
  `XDeleteDate` datetime DEFAULT NULL,
  `XLastEditUser` varchar(45) DEFAULT NULL,
  `XCreationUser` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `image`
--

DROP TABLE IF EXISTS `image`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `image` (
  `Id` char(36) NOT NULL,
  `Path` longtext NOT NULL,
  `XCreateDate` datetime DEFAULT NULL,
  `XUpdateDate` datetime DEFAULT NULL,
  `XDeleteDate` datetime DEFAULT NULL,
  `XLastEditUser` varchar(45) DEFAULT NULL,
  `XCreationUser` varchar(45) DEFAULT NULL,
  `IsDefault` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `invoice_number`
--

DROP TABLE IF EXISTS `invoice_number`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `invoice_number` (
  `IdReceipt` char(36) NOT NULL,
  `number` int(11) DEFAULT NULL,
  `year` int(11) DEFAULT NULL,
  PRIMARY KEY (`IdReceipt`),
  UNIQUE KEY `IdReceipt_UNIQUE` (`IdReceipt`),
  UNIQUE KEY `UniqueNumber` (`number`,`year`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `payment_receipt`
--

DROP TABLE IF EXISTS `payment_receipt`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payment_receipt` (
  `Id` char(36) NOT NULL,
  `IdCustomer` char(36) NOT NULL,
  `Name` varchar(256) NOT NULL,
  `Description` longtext,
  `IssueDate` datetime NOT NULL,
  `PaymentDate` datetime DEFAULT NULL,
  `CostAmount` decimal(15,2) NOT NULL,
  `ReceiptPath` varchar(512) DEFAULT NULL,
  `InvoiceNumber` varchar(45) DEFAULT NULL,
  `CustomerName` varchar(256) DEFAULT NULL,
  `CustomerFiscalCode` varchar(45) DEFAULT NULL,
  `CustomerAddress` varchar(512) DEFAULT NULL,
  `XCreateDate` datetime DEFAULT NULL,
  `XUpdateDate` datetime DEFAULT NULL,
  `XDeleteDate` datetime DEFAULT NULL,
  `XLastEditUser` varchar(45) DEFAULT NULL,
  `XCreationUser` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `payment_receipt_detail`
--

DROP TABLE IF EXISTS `payment_receipt_detail`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payment_receipt_detail` (
  `Id` char(36) NOT NULL,
  `IdResource` char(36) NOT NULL,
  `IdReceipt` char(36) NOT NULL,
  `ReceiptDetailType` int(11) NOT NULL,
  `Name` varchar(256) NOT NULL,
  `Description` longtext,
  `CostAmount` decimal(15,2) NOT NULL,
  `ProductAmount` int(11) NOT NULL DEFAULT '1',
  `XCreateDate` datetime DEFAULT NULL,
  `XUpdateDate` datetime DEFAULT NULL,
  `XDeleteDate` datetime DEFAULT NULL,
  `XLastEditUser` varchar(45) DEFAULT NULL,
  `XCreationUser` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `product`
--

DROP TABLE IF EXISTS `product`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `product` (
  `Id` char(36) NOT NULL,
  `Name` varchar(256) NOT NULL,
  `Description` longtext,
  `TeacherName` varchar(255) DEFAULT NULL,
  `XCreationUser` varchar(45) DEFAULT NULL,
  `XCreateDate` datetime DEFAULT NULL,
  `XUpdateDate` datetime DEFAULT NULL,
  `XDeleteDate` datetime DEFAULT NULL,
  `XLastEditUser` varchar(45) DEFAULT NULL,
  `StartDate` datetime NOT NULL,
  `EndDate` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `product_instance`
--

DROP TABLE IF EXISTS `product_instance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `product_instance` (
  `Id` char(36) NOT NULL,
  `IdProduct` char(36) NOT NULL,
  `Name` varchar(256) NOT NULL,
  `Description` longtext,
  `Days` int(11) NOT NULL DEFAULT '0',
  `Weeks` int(11) NOT NULL DEFAULT '0',
  `Months` int(11) NOT NULL DEFAULT '0',
  `Years` int(11) NOT NULL DEFAULT '0',
  `Price` decimal(15,2) NOT NULL DEFAULT '0.00',
  `XCreateDate` datetime DEFAULT NULL,
  `XUpdateDate` datetime DEFAULT NULL,
  `XDeleteDate` datetime DEFAULT NULL,
  `XLastEditUser` varchar(45) DEFAULT NULL,
  `XCreationUser` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `role`
--

DROP TABLE IF EXISTS `role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `role` (
  `Name` varchar(255) NOT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `Id` int(11) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Name_UNIQUE` (`Name`),
  UNIQUE KEY `IdInt_UNIQUE` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user` (
  `Id` char(36) NOT NULL,
  `FirstName` varchar(255) DEFAULT NULL,
  `LastName` varchar(255) DEFAULT NULL,
  `UserName` varchar(255) NOT NULL,
  `PasswordHash` longtext NOT NULL,
  `Email` varchar(255) DEFAULT NULL,
  `Disabled` bit(1) NOT NULL DEFAULT b'0',
  `PasswordSalt` longtext NOT NULL,
  `IdRole` int(11) DEFAULT NULL,
  `XCreateDate` datetime DEFAULT NULL,
  `XUpdateDate` datetime DEFAULT NULL,
  `XDeleteDate` datetime DEFAULT NULL,
  `XLastEditUser` varchar(45) DEFAULT NULL,
  `XCreationUser` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`),
  UNIQUE KEY `UserName_UNIQUE` (`UserName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-10-15 23:29:04
