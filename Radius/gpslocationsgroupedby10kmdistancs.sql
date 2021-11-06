-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server Version:               8.0.20 - MySQL Community Server - GPL
-- Server Betriebssystem:        Win64
-- HeidiSQL Version:             11.3.0.6295
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Exportiere Datenbank Struktur für reversegeocoding
CREATE DATABASE IF NOT EXISTS `reversegeocoding` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `reversegeocoding`;

-- Exportiere Struktur von Tabelle reversegeocoding.gpslocationsgroupedby10kmdistancs
CREATE TABLE IF NOT EXISTS `gpslocationsgroupedby10kmdistancs` (
  `Latitude` decimal(23,20) NOT NULL,
  `Longitude` decimal(23,20) NOT NULL,
  `FileName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `CityID` int DEFAULT NULL,
  `CountryID` int DEFAULT NULL,
  PRIMARY KEY (`Latitude`,`Longitude`) USING BTREE,
  KEY `City10KmFK` (`CityID`),
  KEY `Country10KmFK` (`CountryID`),
  CONSTRAINT `City10KmFK` FOREIGN KEY (`CityID`) REFERENCES `cities` (`ID`),
  CONSTRAINT `Country10KmFK` FOREIGN KEY (`CountryID`) REFERENCES `countries` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Daten Export vom Benutzer nicht ausgewählt

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
