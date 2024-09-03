-- MySQL dump 10.13  Distrib 8.0.37, for Win64 (x86_64)
--
-- Host: localhost    Database: usefulwebapps
-- ------------------------------------------------------
-- Server version	8.0.37

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` VALUES ('20240823173509_init','8.0.8');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetroleclaims`
--

DROP TABLE IF EXISTS `aspnetroleclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetroleclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `RoleId` varchar(255) NOT NULL,
  `ClaimType` longtext,
  `ClaimValue` longtext,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetRoleClaims_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetroleclaims`
--

LOCK TABLES `aspnetroleclaims` WRITE;
/*!40000 ALTER TABLE `aspnetroleclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetroleclaims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetroles`
--

DROP TABLE IF EXISTS `aspnetroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetroles` (
  `Id` varchar(255) NOT NULL,
  `Name` varchar(256) DEFAULT NULL,
  `NormalizedName` varchar(256) DEFAULT NULL,
  `ConcurrencyStamp` longtext,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `RoleNameIndex` (`NormalizedName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetroles`
--

LOCK TABLES `aspnetroles` WRITE;
/*!40000 ALTER TABLE `aspnetroles` DISABLE KEYS */;
INSERT INTO `aspnetroles` VALUES ('82e35db1-2bb2-4c07-b46b-33b6a07d9391','StandardUser','STANDARDUSER',NULL),('c80287ac-73ac-4188-a9e9-c17a150774f4','Admin','ADMIN',NULL);
/*!40000 ALTER TABLE `aspnetroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserclaims`
--

DROP TABLE IF EXISTS `aspnetuserclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` varchar(255) NOT NULL,
  `ClaimType` longtext,
  `ClaimValue` longtext,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetUserClaims_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserclaims`
--

LOCK TABLES `aspnetuserclaims` WRITE;
/*!40000 ALTER TABLE `aspnetuserclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetuserclaims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserlogins`
--

DROP TABLE IF EXISTS `aspnetuserlogins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserlogins` (
  `LoginProvider` varchar(255) NOT NULL,
  `ProviderKey` varchar(255) NOT NULL,
  `ProviderDisplayName` longtext,
  `UserId` varchar(255) NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`),
  KEY `IX_AspNetUserLogins_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserlogins`
--

LOCK TABLES `aspnetuserlogins` WRITE;
/*!40000 ALTER TABLE `aspnetuserlogins` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetuserlogins` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserroles`
--

DROP TABLE IF EXISTS `aspnetuserroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserroles` (
  `UserId` varchar(255) NOT NULL,
  `RoleId` varchar(255) NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IX_AspNetUserRoles_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserroles`
--

LOCK TABLES `aspnetuserroles` WRITE;
/*!40000 ALTER TABLE `aspnetuserroles` DISABLE KEYS */;
INSERT INTO `aspnetuserroles` VALUES ('19e5b54f-e998-4494-90a2-797cfcfc9fc3','82e35db1-2bb2-4c07-b46b-33b6a07d9391'),('818fd1e7-05ab-44f5-9276-68f20ec3c70d','c80287ac-73ac-4188-a9e9-c17a150774f4');
/*!40000 ALTER TABLE `aspnetuserroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetusers`
--

DROP TABLE IF EXISTS `aspnetusers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetusers` (
  `Id` varchar(255) NOT NULL,
  `UserName` varchar(256) DEFAULT NULL,
  `NormalizedUserName` varchar(256) DEFAULT NULL,
  `Email` varchar(256) DEFAULT NULL,
  `NormalizedEmail` varchar(256) DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext,
  `SecurityStamp` longtext,
  `ConcurrencyStamp` longtext,
  `PhoneNumber` longtext,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEnd` datetime DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UserNameIndex` (`NormalizedUserName`),
  KEY `EmailIndex` (`NormalizedEmail`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetusers`
--

LOCK TABLES `aspnetusers` WRITE;
/*!40000 ALTER TABLE `aspnetusers` DISABLE KEYS */;
INSERT INTO `aspnetusers` VALUES ('19e5b54f-e998-4494-90a2-797cfcfc9fc3','BeefCakeTheMightyStandardUser','BEEFCAKETHEMIGHTYSTANDARDUSER','tester1@gmail.com','TESTER1@GMAIL.COM',0,'AQAAAAIAAYagAAAAENXFZ3JnQ+W+bpXIRU0+O8rcWdIcMnZ2lNxJ1WAq30HSIhdTNDplRpFWKOGbaXJisg==','BZHLOJR56FWZESOXKZFEFH6H4PSYSXIK','96b78a22-18b8-4b83-8766-550223c8e455',NULL,0,0,NULL,1,0),('818fd1e7-05ab-44f5-9276-68f20ec3c70d','BeefCakeTheMightyAdmin','BEEFCAKETHEMIGHTYADMIN','tester2@gmail.com','TESTER2@GMAIL.COM',0,'AQAAAAIAAYagAAAAEEAYsmfCYpVU3g3IIpkdYoR1QG+6TSC0uXcHyagblADddGNhZyjpQGbGGzSZZ0t4DQ==','YDTFTSNVFURV72FHWX2DDY7RSYTZGTIS','81054b31-1b12-4a11-86d2-72f9f093b436',NULL,0,0,NULL,1,0);
/*!40000 ALTER TABLE `aspnetusers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetusertokens`
--

DROP TABLE IF EXISTS `aspnetusertokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetusertokens` (
  `UserId` varchar(255) NOT NULL,
  `LoginProvider` varchar(255) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Value` longtext,
  PRIMARY KEY (`UserId`,`LoginProvider`,`Name`),
  CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetusertokens`
--

LOCK TABLES `aspnetusertokens` WRITE;
/*!40000 ALTER TABLE `aspnetusertokens` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetusertokens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `grocery_categories`
--

DROP TABLE IF EXISTS `grocery_categories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `grocery_categories` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Category` varchar(50) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `grocery_categories`
--

LOCK TABLES `grocery_categories` WRITE;
/*!40000 ALTER TABLE `grocery_categories` DISABLE KEYS */;
INSERT INTO `grocery_categories` VALUES (1,'Produce'),(2,'Meat'),(3,'Dairy'),(4,'Deli'),(5,'Canned'),(6,'Pantry'),(7,'Snacks'),(8,'Bakery'),(9,'Beverages'),(10,'Paper Goods'),(11,'Personal Care'),(12,'Cleaners'),(13,'Household'),(14,'Toiletry'),(15,'Frozen'),(16,'Dry/Baking'),(17,'Other');
/*!40000 ALTER TABLE `grocery_categories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `grocery_list`
--

DROP TABLE IF EXISTS `grocery_list`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `grocery_list` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `GroceryItem` varchar(100) NOT NULL,
  `Category` varchar(50) NOT NULL,
  `Complete` tinyint(1) NOT NULL,
  `UserId` varchar(255) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `grocery_list`
--

LOCK TABLES `grocery_list` WRITE;
/*!40000 ALTER TABLE `grocery_list` DISABLE KEYS */;
INSERT INTO `grocery_list` VALUES (1,'Buy Beer','Beverages',0,'818fd1e7-05ab-44f5-9276-68f20ec3c70d'),(2,'Chicken Nuggets','Frozen',0,'818fd1e7-05ab-44f5-9276-68f20ec3c70d'),(3,'Dish soap','Household',0,'818fd1e7-05ab-44f5-9276-68f20ec3c70d'),(4,'Black beans 2 15oz cans','Canned',0,'818fd1e7-05ab-44f5-9276-68f20ec3c70d'),(5,'Pizza','Frozen',0,'818fd1e7-05ab-44f5-9276-68f20ec3c70d'),(6,'Sponges','Household',0,'818fd1e7-05ab-44f5-9276-68f20ec3c70d'),(17,'Pet food','Household',0,'19e5b54f-e998-4494-90a2-797cfcfc9fc3'),(18,'Beer','Beverages',0,'19e5b54f-e998-4494-90a2-797cfcfc9fc3');
/*!40000 ALTER TABLE `grocery_list` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `recipe_categories`
--

DROP TABLE IF EXISTS `recipe_categories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `recipe_categories` (
  `CategoryId` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Category` varchar(50) NOT NULL,
  PRIMARY KEY (`CategoryId`)
) ENGINE=InnoDB AUTO_INCREMENT=51 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `recipe_categories`
--

LOCK TABLES `recipe_categories` WRITE;
/*!40000 ALTER TABLE `recipe_categories` DISABLE KEYS */;
INSERT INTO `recipe_categories` VALUES (1,'Beverages'),(2,'Sides'),(3,'Breakfast'),(4,'Lunch'),(5,'Brunch'),(6,'Dinner'),(7,'Breads'),(8,'Appetizers'),(9,'Main Dish'),(10,'Dessert'),(11,'Soups'),(12,'Stews & Chili'),(13,'Pasta, Sauces, & Noodles'),(14,'Salad & Dressings'),(15,'Grilling'),(16,'Smoked'),(17,'Burgers'),(18,'Sandwiches'),(19,'Pizza'),(20,'Slow & Pressure Cooker'),(21,'Skillet & Stir-Fries'),(22,'Oven Baked & Broiled'),(23,'Beans, Grains, & Rice'),(24,'Tofu'),(25,'Casseroles'),(26,'Diet'),(27,'Meatless & Vegan'),(28,'Eggs'),(29,'Poultry'),(30,'Chicken'),(31,'Duck'),(32,'Turkey'),(33,'Beef'),(34,'Veal'),(35,'Pork'),(36,'Lamb'),(37,'Sausages'),(38,'Other Meat - wild game etc.'),(39,'Fish and Seafood'),(40,'Nuts and Seeds'),(41,'Fruit'),(42,'Vegetables'),(43,'Brownies'),(44,'Cookies & Biscuits'),(45,'Cakes & Cupcakes'),(46,'Custards & Puddings'),(47,'Pies, Tarts, Cobblers, & Crisp'),(48,'Chocolates & Candie'),(49,'Pastries'),(50,'Frozen');
/*!40000 ALTER TABLE `recipe_categories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `recipe_categories_join`
--

DROP TABLE IF EXISTS `recipe_categories_join`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `recipe_categories_join` (
  `RecipeId` bigint unsigned NOT NULL,
  `CategoryId` bigint unsigned NOT NULL,
  PRIMARY KEY (`RecipeId`,`CategoryId`),
  KEY `CategoryId` (`CategoryId`),
  CONSTRAINT `recipe_categories_join_ibfk_1` FOREIGN KEY (`RecipeId`) REFERENCES `recipes` (`RecipeId`),
  CONSTRAINT `recipe_categories_join_ibfk_2` FOREIGN KEY (`CategoryId`) REFERENCES `recipe_categories` (`CategoryId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `recipe_categories_join`
--

LOCK TABLES `recipe_categories_join` WRITE;
/*!40000 ALTER TABLE `recipe_categories_join` DISABLE KEYS */;
INSERT INTO `recipe_categories_join` VALUES (2,1),(2,2),(1,4),(2,4),(29,4),(1,6),(2,6),(4,6),(18,6),(22,6),(3,8),(2,9),(22,9),(3,10),(21,10),(20,12),(2,20),(20,20),(22,20),(1,21),(15,21),(18,21),(4,24),(2,25),(1,27),(2,27),(4,27),(15,27),(16,27),(17,28),(18,28),(20,28),(19,34),(3,35),(21,35),(22,37),(21,41);
/*!40000 ALTER TABLE `recipe_categories_join` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `recipe_courses`
--

DROP TABLE IF EXISTS `recipe_courses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `recipe_courses` (
  `CourseId` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Course` varchar(50) NOT NULL,
  PRIMARY KEY (`CourseId`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `recipe_courses`
--

LOCK TABLES `recipe_courses` WRITE;
/*!40000 ALTER TABLE `recipe_courses` DISABLE KEYS */;
INSERT INTO `recipe_courses` VALUES (1,'Hors D\'Oeuvre'),(2,'Amuse-Bouche'),(3,'Soup'),(4,'Appetizer'),(5,'Salad'),(6,'Palate Cleanser'),(7,'Main Course'),(8,'Dessert'),(9,'Mignardise');
/*!40000 ALTER TABLE `recipe_courses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `recipe_cuisines`
--

DROP TABLE IF EXISTS `recipe_cuisines`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `recipe_cuisines` (
  `CuisineId` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Cuisine` varchar(50) NOT NULL,
  PRIMARY KEY (`CuisineId`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `recipe_cuisines`
--

LOCK TABLES `recipe_cuisines` WRITE;
/*!40000 ALTER TABLE `recipe_cuisines` DISABLE KEYS */;
INSERT INTO `recipe_cuisines` VALUES (1,'Italian'),(2,'Indian'),(3,'Mexican'),(4,'Japanese'),(5,'French'),(6,'Chinese'),(7,'Middle Eastern'),(8,'Thai'),(9,'Greek'),(10,'Brazilian'),(11,'Spanish'),(12,'Vietnamese'),(13,'Korean'),(14,'African'),(15,'Caribbean'),(16,'American'),(17,'Russian');
/*!40000 ALTER TABLE `recipe_cuisines` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `recipe_difficulties`
--

DROP TABLE IF EXISTS `recipe_difficulties`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `recipe_difficulties` (
  `DifficultyId` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Difficulty` varchar(25) NOT NULL,
  PRIMARY KEY (`DifficultyId`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `recipe_difficulties`
--

LOCK TABLES `recipe_difficulties` WRITE;
/*!40000 ALTER TABLE `recipe_difficulties` DISABLE KEYS */;
INSERT INTO `recipe_difficulties` VALUES (1,'Easy'),(2,'Medium'),(3,'Hard'),(4,'Pro Chef');
/*!40000 ALTER TABLE `recipe_difficulties` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `recipes`
--

DROP TABLE IF EXISTS `recipes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `recipes` (
  `RecipeId` bigint unsigned NOT NULL AUTO_INCREMENT,
  `RecipeTitle` varchar(100) NOT NULL,
  `RecipeDescription` varchar(200) DEFAULT NULL,
  `CourseId` bigint unsigned DEFAULT NULL,
  `CuisineId` bigint unsigned DEFAULT NULL,
  `DifficultyId` bigint unsigned DEFAULT NULL,
  `PrepTime` smallint unsigned NOT NULL,
  `CookTime` smallint unsigned NOT NULL,
  `Rating` tinyint unsigned NOT NULL,
  `Servings` tinyint unsigned NOT NULL,
  `Nutrition` varchar(2000) DEFAULT NULL,
  `Ingredients` varchar(3000) NOT NULL,
  `Instructions` varchar(3000) NOT NULL,
  `Notes` varchar(2000) DEFAULT NULL,
  `UserId` varchar(255) NOT NULL,
  `UserName` varchar(256) NOT NULL,
  PRIMARY KEY (`RecipeId`),
  KEY `CourseId` (`CourseId`),
  KEY `CuisineId` (`CuisineId`),
  KEY `DifficultyId` (`DifficultyId`),
  FULLTEXT KEY `fulltext` (`RecipeTitle`,`Ingredients`),
  CONSTRAINT `recipes_ibfk_1` FOREIGN KEY (`CourseId`) REFERENCES `recipe_courses` (`CourseId`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `recipes_ibfk_2` FOREIGN KEY (`CuisineId`) REFERENCES `recipe_cuisines` (`CuisineId`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `recipes_ibfk_3` FOREIGN KEY (`DifficultyId`) REFERENCES `recipe_difficulties` (`DifficultyId`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=30 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `recipes`
--

LOCK TABLES `recipes` WRITE;
/*!40000 ALTER TABLE `recipes` DISABLE KEYS */;
INSERT INTO `recipes` VALUES (1,'Lemon Pepper Chicken',NULL,7,1,2,10,25,7,5,'Serving: 1serving <br/>\r\nCalories: 253.4kcal<br/>\r\nCarbohydrates: 3.53g<br/> \r\nProtein: 34.45g<br/> \r\nFat: 10.43g <br/>\r\nSodium: 656.68mg <br/>\r\nFiber: 0.15g','2 boneless skinless chicken breasts<br/>\r\n2 Tbsp all-purpose flour<br/>\r\n1 Tbsp lemon pepper seasoning<br/>\r\n1 Tbsp cooking oil<br/>\r\n1 clove garlic, minced<br/>\r\n1/2 cup chicken broth<br/>\r\n1 Tbsp butter<br/>\r\n1 tsp lemon juice<br/>\r\n1 Tbsp chopped fresh parsley (optional)<br/>\r\n1/8 tsp freshly cracked black pepper<br/>','<strong>1.</strong> Use a sharp knife to carefully fillet the chicken breasts into two thinner peices (or use thin-cut chicken breasts).<br/>\r\n<strong>2.</strong> Combine the flour and lemon pepper seasoning in a bowl. Sprinkle the mixture over both sides of the chicken breast pieces and then rub it in until the chicken is fully coated.<br/>\r\n<strong>3.</strong> Heat the cooking oil in a large skillet over medium. When the skillet and oil are very hot, add the chicken and cook on each side until golden brown (about 5 minutes per side). Remove the cooked chicken to a clean plate and cover to keep warm.<br/>\r\n<strong>4.</strong> Add the butter and minced garlic to the skillet and sauté for about one minute.<br/>\r\n<strong>5.</strong> Add the chicken broth to the skillet and whisk to dissolve all the browned bits from the bottom of the skillet. Add the lemon juice and allow the sauce to simmer in the skillet for 3-5 minutes, or until it has reduced slightly. Taste the sauce and add salt if needed (I did not add any).<br/>\r\n<strong>6.</strong> Finally, return the chicken to the skillet and spoon the sauce over top. Allow the chicken to heat through. Season with a little freshly cracked pepper and fresh chopped parsley (optional), then serve.','Cook this now<br/>\r\nOr else you go hungry','19e5b54f-e998-4494-90a2-797cfcfc9fc3','BeefCakeTheMightyStandardUser'),(2,'Easy Slow Cooker Buffalo Chicken!!',NULL,7,3,1,10,400,7,8,NULL,'2 - 4  boneless skinless chicken breasts<br/>\r\n1 - 1.5 cups sweet baby rays buffalo sauce<br/>\r\n1 packet of ranch dressing mix<br/>\r\n1 - 2 table spoons butter<br/>','1. place chicken in slow cooker <br/>\r\n2. add in ranch packet<br/>\r\n3. add in hot sauce<br/>\r\n4. cook on low 6-7 hours<br/>\r\n5. shred chicken and stir in butter<br/>\r\n6. cook on low 1 more hour<br/>',NULL,'19e5b54f-e998-4494-90a2-797cfcfc9fc3','BeefCakeTheMightyStandardUser'),(3,'Strawberries Romanoff',NULL,4,17,2,10,0,5,6,'Serving: 1serving<br/> Calories: 253.4 kcal<br/> Carbohydrates: 3.53g <br/> Protein: 34.45g<br/> Fat: 10.43g<br/> Sodium: 656.68mg<br/> Fiber: 0.15g<br/>','2 pint strawberries<br/>\n4 tbs sugar<br/>\n4 tbs Grand Marnier<br/>\n1/4 cup powdered sugar<br/>\n1 cup Cream<br/>\n1/4 cup sour cream','1. In a medium bowl, combine hulled and quartered strawberries, 4 Tbsp sugar and 4 Tbsp liqueur, stir to combine then cover and refrigerate at least 1 hour and up to 2 hours, stirring once or twice.<br/>\n2.  in a large mixing bowl, combine 1 cup cold heavy cream and 1/4 cup powdered sugar, and beat with an electric mixer until stiff peaks form.\nUsing a spatula, fold in 1/4 cup sour cream just until well blended.<br/>\n3. To serve, stir strawberries then divide between 6 serving glasses or bowls.<br/>\n4. Spoon cream over strawberries, dividing evenly<br/>\n5. Serve right away or chill and enjoy within 2 hours of assembly.<br/>','Enjoy','19e5b54f-e998-4494-90a2-797cfcfc9fc3','BeefCakeTheMightyStandardUser'),(4,'Chicken Enchilada Casserole',NULL,7,3,2,20,60,5,8,'Serving: 1serving<br/> Calories: 253.4 kcal<br/> Carbohydrates: 3.53g <br/> Protein: 34.45g<br/> Fat: 10.43g<br/> Sodium: 656.68mg<br/> Fiber: 0.15g<br/>','14 oz jar Enchilada sauce<br/>\n3 Cups shredded Monterey Jack cheese<br/>\n6 corn tortillas<br/>\n2 chicken breasts<br/>','1. Cut each chicken breast in about 3 pieces, so that it cooks faster and put it in a small pot.\nPour Enchilada sauce over it and cook covered on low to medium heat until chicken is cooked through, about 20 minutes.\nNo water is needed, the chicken will cook in the Enchilada sauce.\nMake sure you stir occasionally so that it doesn\'t stick to the bottom.<br/>\n2. Remove chicken from the pot and shred with two forks.\nPreheat oven to 375 F degrees.<br/>\n3. Start layering the casserole.\nStart with about ¼ cup of the leftover Enchilada sauce over the bottom of a baking dish.\nI used a longer baking dish, so that I can put 2 corn tortillas across.\nPlace 2 tortillas on the bottom, top with ⅓ of the chicken and ⅓ of the remaining sauce.\nSprinkle with ⅓ of the cheese and repeat starting with 2 more tortillas, then chicken, sauce, cheese.\nRepeat with last layer with the remaining ingredients, tortillas, chicken, sauce and cheese.<br/>\n4. Bake for 20 to 30 minutes uncovered, until bubbly and cheese has melted and started to brown on top.\nServe warm.<br/>','Enjoy','19e5b54f-e998-4494-90a2-797cfcfc9fc3','BeefCakeTheMightyStandardUser'),(15,'15-MINUTE HONEY GARLIC CHICKEN','This 5-ingredient, 15-minute honey garlic chicken with an addictively delicious sauce makes a perfect quick and easy weeknight dinner recipe!',7,16,1,5,10,5,4,'NUTRITION INFORMATION: YIELD: 4 SERVING SIZE: 1<br/>\r\nAmount Per Serving: \r\nCALORIES: 360\r\nTOTAL FAT: 8g\r\nSATURATED FAT: 2g\r\nTRANS FAT: 0g\r\nUNSATURATED FAT: 5g\r\nCHOLESTEROL: 145mg\r\nSODIUM: 665mg\r\nCARBOHYDRATES: 15g\r\nFIBER: 0g\r\nSUGAR: 13g\r\nPROTEIN: 54g','2 teaspoons extra-virgin olive oil (or canola oil)<br/>\r\n1 ½ pounds boneless, skinless chicken breasts, cut into small cubes (about ½ inch)<br/>\r\nSalt and black pepper<br/>\r\n3 tablespoons honey<br/>\r\n3 tablespoons low-sodium soy sauce<br/>\r\n3 cloves garlic, minced<br/>\r\n¼ teaspoon red pepper flakes (optional, adjust for heat)<br/>\r\nOptional for serving: Brown rice, sliced green onions, sesame seeds, chopped peanuts, lime wedges to squeeze over chicken<br/>','Heat olive oil in a large skillet over medium-high heat.<br/>\r\nLightly season the cubed chicken with salt and pepper. (Go easy because the soy sauce has plenty of sodium.)<br/>\r\nAdd the chicken to the skillet and brown on one side, about 3-4 minutes.<br/>\r\nMeanwhile, make the glaze. Whisk the honey, soy sauce, garlic and red pepper flakes, if using, in a small bowl until well combined.<br/>\r\nAdd the sauce to the pan and toss to coat the chicken pieces. Cook until chicken is cooked through, 4-5 more minutes.<br/>\r\nServe with steamed rice and top with green onions, sesame seeds and a squeeze of lime juice, if desired.<br/>\r\n','Tip: Be careful not to overcook the chicken. The pieces are small so they cook fast, which is kinda the point with this dinner. But you don’t want them to go too far and be overdone. On the other hand, that sauce is seriously so good, it’ll disguise your chicken if it gets a little overdone.\r\n<br/><br/>\r\nYou could substitute boneless, skinless chicken thighs in place of the chicken breasts if you prefer, but the cooking time might be a little longer.\r\n<br/><br/>\r\nI do use and recommend low-sodium soy sauce so you can control the amount of sodium in this recipe. You could also substitute a gluten-free tamari if you need this recipe to be gluten-free. \r\n<br/><br/>\r\nFor serving: To make a complete meal, just add some steamed rice and a veggie. Or serve this over quinoa, cauliflower rice or a thin noodle.\r\n<br/><br/>\r\nLeftovers, once cooled, can be stored in the refrigerator for up to 3-4 days. Reheat in the microwave or in a skillet on the stove until warmed through.','19e5b54f-e998-4494-90a2-797cfcfc9fc3','BeefCakeTheMightyStandardUser'),(16,'Fiesta Chicken','This healthy fiesta chicken and rice is an easy one-pan meal that\'s ready in under 30 minutes. Made with instant brown rice and black beans!\r\n',7,3,1,10,15,8,5,'SERVING: 1(of 6); without toppings\r\nCALORIES: 347kcal\r\nCARBOHYDRATES: 38g\r\nPROTEIN: 28g\r\nFAT: 9g\r\nSATURATED FAT: 4g\r\nPOLYUNSATURATED FAT: 1g\r\nMONOUNSATURATED FAT: 3g\r\nTRANS FAT: 0.01g\r\nCHOLESTEROL: 63mg\r\nPOTASSIUM: 687mg\r\nFIBER: 7g\r\nSUGAR: 4g\r\nVITAMIN A: 1468IU\r\nVITAMIN C: 47mg\r\nCALCIUM: 172mg\r\nIRON: 4mg\r\n','2 teaspoons extra-virgin olive oil<br/>\r\n1 pound boneless skinless chicken breasts cut into bite-size pieces<br/>\r\n½ medium yellow onion or red onion, chopped (about 1 cup) <br/>\r\n1 tablespoon garlic powder<br/>\r\n2 teaspoons onion powder<br/>\r\n1 ½ teaspoons paprika<br/>\r\n1 ½ teaspoons dried dillweed<br/>\r\n½ teaspoon cumin<br/>\r\n½ teaspoon kosher salt<br/>\r\n½ teaspoon cayenne pepper use 1/4 teaspoon or less if sensitive to spice<br/>\r\n1 medium red bell pepper cored and chopped<br/>\r\n1 medium green bell pepper cored and chopped<br/>\r\n1 can reduced-sodium black beans (15 ounces) rinsed and drained<br/>\r\n1 can fire-roasted diced tomatoes in their juices (15 ounces)<br/>\r\n1 ½ cups instant brown rice* do not use white minute rice or regular rice as these will cook differently (see note)<br/>\r\n1 cup low-sodium chicken stock<br/>\r\n¾ cup freshly grated cheddar cheese<br/>\r\nOptional for serving: chopped fresh cilantro, diced avocado, plain nonfat Greek yogurt (or sour cream)\r\n\r\n\r\n','1. Heat the olive oil in a large nonstick skillet over medium-high heat. Add the chicken, onion, garlic powder, onion powder, paprika, dill, salt, cumin, and cayenne pepper. Sauté, stirring often, for 3 minutes, until the chicken begins to brown on the outside.<br/><br/>\r\n2. Stir in the red bell pepper, green bell pepper, black beans, tomatoes, rice, and chicken stock.<br/><br/>\r\n3. Bring to a boil, and then cover and simmer on low heat for 5 minutes, or until the chicken is done and the rice is nearly tender. Let stand, covered, for 5 minutes, until the liquid is absorbed.<br/><br/>\r\n4. Taste and adjust seasonings as desired. Sprinkle with cheese and any additional desired toppings. Serve warm. <br/><br/>','*INGREDIENT NOTE: I have not tried this recipe with regular (not instant) rice or white rice instead of brown rice. If you want to experiment and use regular rice instead, I suggest sautéing the meat and vegetables in one skillet (you can drain the tomatoes first and then add them to the chicken and peppers at the end, heating until most of their liquid evaporates). Then, prepare the rice in its own pot according to the package directions (due to the longer cook time). Combine the two at the end with a bit of extra chicken stock if the mixture is too dry. Again, I haven’t tried the recipe this way, but that is my best suggestion.\r\n<br/><br/>\r\nTO STORE: Store leftovers in an airtight container in the refrigerator for up to 4 days.\r\n<br/><br/>\r\nTO REHEAT: Warm leftover first chicken gently in the microwave or on the stovetop, with a splash of chicken stock or water to keep it from drying out.\r\n<br/><br/>\r\nTO FREEZE: You can freeze fiesta chicken for up to 2 months in an airtight container. Let thaw overnight in the refrigerator before warming.','19e5b54f-e998-4494-90a2-797cfcfc9fc3','BeefCakeTheMightyStandardUser'),(17,'Easy Korean Ground Beef Recipe','This easy Korean ground beef recipe is a tasty dinner idea that is light on the budget but heavy on the bold Korean flavors! You\'re family is going to love it!',7,13,2,10,10,7,8,'Serving: 1cup\r\nCalories: 404kcal\r\nCarbohydrates: 21g\r\nProtein: 21g\r\nFat: 26g\r\nSaturated Fat: 9g\r\nPolyunsaturated Fat: 2g\r\nMonounsaturated Fat: 11gTrans Fat: 1gCholesterol: 81mgSodium: 970mgPotassium: 401mgFiber: 0.3gSugar: 19gVitamin A: 61IUVitamin C: 9mgCalcium: 41mgIron: 3mg','1 Tablespoon Canola or Vegetable Oil<br/>\r\n2 lbs Ground Beef, or ground chuck<br/>\r\n4 Cloves Garlic, minced<br/>\r\n1/2 Cup Soy Sauce<br/>\r\n1/2 Cup Orange Juice<br/>\r\n1/2 Cup Brown Sugar<br/>\r\n1/4 Cup Sweet Chili Sauce<br/>\r\n2 teaspoons Toasted Sesame Oil<br/>\r\n1 Tablespoon Cornstarch<br/>\r\n2 Green Onions, sliced, plus more for garnish<br/>','1. Heat the oil in a cast iron pan over medium high heat. Add the ground beef and garlic, using a meat chopper to break up, cook until browned all the way through, about 3-5 minutes, then drain grease.\r\n2 lbs Ground Beef,4 Cloves Garlic,1 Tablespoon Canola or Vegetable Oil<br/<br/>\r\n2. Whisk together the remaining ingredients.\r\n1/2 Cup Soy Sauce,1/2 Cup Orange Juice,1/2 Cup Brown Sugar,1/4 Cup Sweet Chili Sauce,2 teaspoons Toasted Sesame Oil,1 Tablespoon Cornstarch<br/><br/>\r\n3. Pour the ingredients all over the beef and simmer until thickened slightly.<br/><br/>\r\n4. Serve over rice with steamed veggies and garnish with green onions.\r\n2 Green Onions',NULL,'19e5b54f-e998-4494-90a2-797cfcfc9fc3','BeefCakeTheMightyStandardUser'),(18,'Quick Beef Stir-Fry',NULL,7,16,1,15,10,7,4,'268 Calories\r\n16g Fat\r\n9g Carbs\r\n23g Protein','2 tablespoons vegetable oil<br/>\r\n1 pound beef sirloin, cut into 2-inch strips<br/>\r\n1 ½ cups fresh broccoli florets<br/>\r\n1 red bell pepper, cut into matchsticks<br/>\r\n2 carrots, thinly sliced<br/>\r\n1 green onion, chopped<br/>\r\n1 teaspoon minced garlic<br/>\r\n2 tablespoons soy sauce<br/>\r\n2 tablespoons sesame seeds, toasted','1. Heat vegetable oil in a large wok or skillet over medium-high heat; cook and stir beef until browned, 3 to 4 minutes. <br/><br/>\r\n2. Move beef to the side of the wok and add broccoli, bell pepper, carrots, green onion, and garlic to the center of the wok. Cook and stir vegetables for 2 minutes.<br/><br/>\r\n3. Stir beef into vegetables and season with soy sauce and sesame seeds. Continue to cook and stir until vegetables are tender, about 2 more minutes.',NULL,'818fd1e7-05ab-44f5-9276-68f20ec3c70d','BeefCakeTheMightyAdmin'),(19,'Honey Garlic Salmon',NULL,7,16,1,5,10,7,2,NULL,'SAUCE<br/><br/>\r\n4 tbsp honey<br/>\r\n2 tbsp soy sauce (all purpose or light soy sauce)<br/>\r\n1 tbsp white vinegar (or sub with any other vinegar except balsamic)<br/>\r\n1 large garlic clove (or 2 small) , minced<br/><br/>\r\nSALMON<br/><br/>\r\n2 salmon or trout fillets, skinless (6oz / 200g each)<br/>\r\nOlive oil<br/>\r\nSalt and pepper<br/><br/>\r\nOPTIONAL GARNISHES<br/><br/>\r\nSesame seeds<br/>\r\nFinely sliced chives or shallots/scallions<br/>','1. Take salmon out of the fridge 20 minutes before cooking. Pat salmon skin dry with a paper towel and sprinkle with salt and pepper.<br/>\r\n2. Whisk together the Sauce ingredients in a small bowl.<br/>\r\n3. Drizzle oil in a non stick fry pan and heat over medium high heat (or just under, if your stove runs hot). Place salmon in the pan, and cook the first side for 3 - 4 minutes until golden. Turn, then cook the other side for 2 - 3 minutes or until golden.<br/><br/>\r\n4. Pour Sauce over salmon. Cook for 1 minute or until it starts to thicken slightly. Check the side of the salmon to tell how cooked through the middle is - I like mine medium rare inside. (Note 1). If Sauce thickens too much before your salmon is cooked to your taste, just add water 1 tbsp at a time.<br/><br/>\r\n5. Remove onto serving plates.<br/><br/>\r\n6. Serve salmon drizzled with Sauce, sprinkled with sesame seeds and chives/shallots, if desired.\r\n\r\n','I tell how cooked a salmon is by eye, looking at the side of the salmon. I like mine to be coral inside (rare to medium rare), not pale pink and fully cooked. You can use a thermometer if you want. Insert it into the thickest part - it should be 120F / 50C for medium rare or 130F / 55C for medium.<br/><br/>\r\n','818fd1e7-05ab-44f5-9276-68f20ec3c70d','BeefCakeTheMightyAdmin'),(20,'Easy Crock Pot Chili','This Easy Crockpot Chili recipe is loaded with ground beef, seasonings, & tons of flavor!',7,16,1,15,240,6,10,NULL,'3 pounds lean ground beef *see note <br/>\r\n2 medium onions diced, ½-inch<br/>\r\n4 cloves garlic minced<br/>\r\n1 can light beer or bottle, approx. 12 ounces<br/>\r\n28 ounces whole tomatoes with juice<br/>\r\n14 ounces diced tomatoes with juice<br/>\r\n1 medium green bell pepper ½\" diced, optional<br/>\r\n14 ounces tomato sauce<br/>\r\n15 ounces kidney beans drained and rinsed<br/><br/>\r\nSeasoning Mixture<br/><br/>\r\n4 tablespoons chili powder<br/>\r\n1 tablespoon cumin<br/>\r\n2 teaspoons parsley<br/>\r\n1 teaspoon smoked paprika<br/>\r\n1 teaspoon each salt and pepper<br/>\r\n1 teaspoon oregano<br/>\r\n\r\n','1. Combine half the seasoning mixture with ground beef and mix until well combined.<br/><br/>\r\n2. Brown ground beef*, onions, and garlic until no pink remains. Drain any fat. Add beer and simmer until most of the liquid has evaporated.<br/><br/>\r\n3. Combine beef mixture and all remaining ingredients, including remaining seasoning, in a slow cooker. If desired slightly mash the whole tomatoes.<br/><br/>\r\n4. Cook on high for 4 hours or low 7-8 hours. Once cooked, remove the lid and let cool slightly before serving.<br/>','*This recipe can be made with 2lbs of ground beef if desired and the beans can be doubled. \r\n\r\nGround beef may need to be browned in batches depending on the size of your pan.\r\n\r\nThe chili will be very hot after cooking and will thicken as it cools. I allow it to cool at least 30 minutes with the lid off stirring occasionally (and it is still very hot).\r\n\r\nThis chili has a fairly mild flavor. Add one finely diced jalapeno or a pinch of cayenne pepper to add a little bit of heat.  \r\n\r\nBeer is recommended, but if needed, you can replace it with beef broth. I often use a light beer (such as Budweiser), but use your favorite.\r\n\r\nYou can add vegetables to this crockpot chili recipe. Diced bell peppers, zucchini, and mushrooms contain water so precook them or leave the lid off the slow cooker for the last hour of cooking.\r\n\r\nTo thicken leave the lid off the Crockpot and let some of the liquid evaporate. Or make a slurry of equal amounts of water and corn starch in a jar. Shake the jar until the slurry is blended then slowly stir it into the chili (you might not need all of it).\r\n\r\nIf doubling this recipe, ensure the slow cooker isn’t more than ¾ full.','818fd1e7-05ab-44f5-9276-68f20ec3c70d','BeefCakeTheMightyAdmin'),(21,'Easy Apple Pie','This is hands down the BEST and easiest apple pie recipe! It has a tender, flaky, homemade pie crust with apple slices drenched in sugar and warm spices like cinnamon and nutmeg.',8,16,2,60,60,8,8,NULL,'2 (9\") pie crusts<br/>\r\n7 large Granny Smith apples (peeled, cored and sliced into ½ inch slices)<br/>\r\n½ cup granulated sugar<br/>\r\n½ cup light brown sugar (loosely packed)<br/>\r\n2 tablespoons all-purpose flour<br/>\r\n1 teaspoon ground cinnamon<br/>\r\n⅛ teaspoon ground nutmeg<br/>\r\n1 tablespoon lemon juice (plus the zest of half of a lemon)<br/>\r\n1 large egg (lightly beaten in a small bowl for egg wash)<br/>\r\n2 tablespoons sanding sugar (optional)','1. Start by preparing this <a href=\"https://littlespoonfarm.com/all-butter-pie-crust-recipe/\">flaky pie crust</a> recipe which makes 2 (9\") pie crusts, one for the bottom and one for the top of the pie. The pie dough will need to chill for at least 1 hour before rolling out. Or use a store-bought pie crust and follow package directions.<br/>\r\n\r\n2. Place oven rack in the center position and Preheat the oven to 400°F (204°C).<br/>\r\n\r\n3. In a large bowl, combine the sliced apples, granulated sugar, light brown sugar, flour, cinnamon, nutmeg, and lemon juice and lemon zest; toss to coat evenly.<br/>\r\n\r\n4. Remove the pie crust dough from the fridge and let rest at room temperature for 5-10 minutes. On a lightly floured surface, roll one disc into a 12\" circle that is ⅛\" thick. Carefully lay the crust into the bottom of a deep dish pie plate.<br/>\r\n\r\n5. Spoon the apple filling over the bottom crust and discard juices at the bottom of the bowl. Roll out the second disc of pie crust until it is ⅛\" thick and lay it over the apple filling.<br/>\r\n\r\n6. Use a sharp knife to trim the dough along the outside edge of the pie plate. Lift the edges where the two pie crust meet, gently press to seal and fold them under. Rotate the pie plate and repeat this process until edges are neatly tucked under themselves. Cut 4 slits in the top of the dough to allow steam to vent. Place the pie on a baking sheet.<br/>\r\n\r\n7. Brush the surface of the pie crust with the egg wash and sprinkle with sanding sugar. Cover the edges with a pie shield or a strip of foil to keep them from over browning during the first 25 minutes.<br/>\r\n\r\n8. Bake at 400°F (204°C) for 25 minutes. Carefully remove the pie shield, turn the oven down to 375° and continue to bake for an additional 30-35 minutes or until the top is golden brown and the juices are bubbly. Cool at room temperature for at least 3 hours.','How do you keep the bottom crust of apple pie from getting soggy?\r\nWhen placing the apple mixture into the pie crust, make sure to spoon the apples out of the bowl and discard any excess liquid that is in the bottom of the bowl. The extra liquid can cause the crust to become soggy. The apples will continue to release moisture as the pie bakes.\r\n\r\nWhat is the best way to cut apples for pie?\r\nWhen making the apple pie filling, cut the apples into ½\" thick slices. Thick apple slices create a sturdy pie that keeps the crust from caving into the middle.\r\n\r\nWhat kind of apples should you use for an apple pie?\r\nGranny Smith apples are the best apples for making apple pie because they are super tart and very firm. Firm apples hold up well during the baking process unlike varieties such as McIntosh which kind of turn to mush.\r\nOther great varieties are: Honeycrisp, Jonathan, Jazz, Golden Delicious, Jonagold or Pink Lady.','818fd1e7-05ab-44f5-9276-68f20ec3c70d','BeefCakeTheMightyAdmin'),(22,'Slow Cooker Sausage Pepper and Onion',NULL,7,1,1,15,480,6,10,NULL,'1. 8 to 10 Italian sausages -- sweet or hot or both<br/>\r\n2. 1 onion -- your choice yellow or red<br/>\r\n3. 2 cans diced tomato<br/>\r\n4. 1 can marinara sauce<br/>\r\n5. 2-3 bell peppers -- 1 green 2 red<br/>\r\n6. salt and pepper to taste<br/>\r\n7. red pepper flakes to taste<br/>\r\n8. 1 table spoon Italian seasoning.\r\n','1. add sausages to bottom of pot<br/>\r\n2. chop all veggies and add on top of sausage<br/>\r\n3. add in diced tomato and marinara sauce<br/>\r\n4. add in seasonings<br/>\r\n5. stir up a bit<br/>\r\n6. cook on low 8 hours.<br/>\r\nEnjoy over noodles, on a bun, or by it self',NULL,'818fd1e7-05ab-44f5-9276-68f20ec3c70d','BeefCakeTheMightyAdmin'),(29,'Test recipe',NULL,3,1,2,1,1,1,1,'who cares beef cake','soup stuff','cook it','eat it','19e5b54f-e998-4494-90a2-797cfcfc9fc3','BeefCakeTheMightyStandardUser');
/*!40000 ALTER TABLE `recipes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `to_do_list`
--

DROP TABLE IF EXISTS `to_do_list`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `to_do_list` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ToDoItem` varchar(100) NOT NULL,
  `Complete` tinyint(1) NOT NULL,
  `UserId` varchar(255) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `to_do_list`
--

LOCK TABLES `to_do_list` WRITE;
/*!40000 ALTER TABLE `to_do_list` DISABLE KEYS */;
INSERT INTO `to_do_list` VALUES (5,'mow lawn',0,'19e5b54f-e998-4494-90a2-797cfcfc9fc3'),(6,'fix sink',1,'19e5b54f-e998-4494-90a2-797cfcfc9fc3'),(7,'change oil',0,'19e5b54f-e998-4494-90a2-797cfcfc9fc3'),(8,'drink beer',0,'19e5b54f-e998-4494-90a2-797cfcfc9fc3'),(10,'Fix Mower',0,'818fd1e7-05ab-44f5-9276-68f20ec3c70d'),(11,'Work Out',0,'818fd1e7-05ab-44f5-9276-68f20ec3c70d'),(12,'Buy Beer',0,'818fd1e7-05ab-44f5-9276-68f20ec3c70d'),(13,'Make Apps',0,'818fd1e7-05ab-44f5-9276-68f20ec3c70d');
/*!40000 ALTER TABLE `to_do_list` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-09-03 14:30:17
