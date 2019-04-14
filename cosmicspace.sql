-- phpMyAdmin SQL Dump
-- version 4.8.4
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Czas generowania: 14 Kwi 2019, 20:14
-- Wersja serwera: 10.1.37-MariaDB
-- Wersja PHP: 7.3.0

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Baza danych: `cosmicspace`
--

DELIMITER $$
--
-- Procedury
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `createportals` (IN `inlimit` INT(5))  BEGIN
   	DECLARE v1 INT DEFAULT inlimit;
    WHILE v1 >= 0 DO
    BEGIN
       	DECLARE v2 INT DEFAULT inlimit;
        WHILE v2 >= 0 DO
        BEGIN
        
            IF((SELECT COUNT(*) FROM portalpositions pp WHERE pp.positionx=v1 AND pp.positiony=v2) > 0) THEN

				SELECT CONCAT(1, v1, ';', v2);
                UPDATE portalpositions
                SET name = CONCAT(v1, ';', v2)
                WHERE positionx=v1 AND positiony=v2;

            ELSE

				SELECT CONCAT(2, v1, ';', v2);
                INSERT INTO portalpositions(name, positionx, positiony)
                VALUES (CONCAT(v1, ';', v2), v1, v2);

            END IF;
        
        END;
		SET v2 = v2 - 1;
       	END WHILE;
	END;
	SET v1 = v1 - 1;
	END WHILE;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `getammunitions` ()  NO SQL
SELECT *
FROM ammunitions$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `getenemies` ()  NO SQL
SELECT *
FROM enemies e
JOIN rewards r ON r.rewardid=e.rewardid
JOIN prefabs ps ON ps.prefabid=e.prefabid
JOIN prefabs_types pt ON pt.prefabtypeid=ps.prefabtypeid$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `getenemymap` (IN `inmapid` INT UNSIGNED)  NO SQL
SELECT *
FROM enemymap
WHERE mapid=inmapid$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `getmaps` ()  NO SQL
SELECT *
FROM maps$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `getpilotresources` (IN `inuserid` BIGINT UNSIGNED)  NO SQL
SELECt *
FROM pilotresources
WHERE userid=inuserid
LIMIT 1$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `getplayerdata` (IN `inuserid` BIGINT)  NO SQL
SELECT *
FROM pilots p
WHERE p.userid=inuserid
LIMIT 1$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `getplayerid` (IN `inusername` VARCHAR(128))  NO SQL
SELECT u.userid
FROM users u
WHERE u.usernamehash=inusername
LIMIT 1$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `getportals` (IN `inmapid` INT(11))  NO SQL
SELECT 
p.*, 
ps.*,
pt.*,
pp.*,
ptp.positionx as target_positionx,
ptp.positiony as target_positiony,
((gs_ms.value - gs_pb.value) / gs_pc.value) as portaldistance,
(gs_pb.value / 2) as portalborder,
mrq.requiredlevel
FROM portals p
JOIN prefabs ps ON ps.prefabid=p.prefabid
JOIN prefabs_types pt ON pt.prefabtypeid=ps.prefabtypeid
JOIN portalpositions pp ON pp.portalpositionid=p.portalpositionid
JOIN portalpositions ptp ON ptp.portalpositionid=p.target_portalpositionid
JOIN maps mrq ON mrq.mapid=p.target_mapid
JOIN gamesettings gs_pc ON gs_pc.key='portalcount'
JOIN gamesettings gs_ms ON gs_ms.key='mapsize'
JOIN gamesettings gs_pb ON gs_pb.key='portalborder'
WHERE p.mapid=inmapid$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `getrockets` ()  NO SQL
SELECT *
FROM rockets$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `getships` ()  NO SQL
SELECT *
FROM ships s
JOIN rewards r ON r.rewardid=s.rewardid
JOIN prefabs ps ON ps.prefabid=s.prefabid
JOIN prefabs_types pt ON pt.prefabtypeid=ps.prefabtypeid$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `loginuser` (IN `inusername` VARCHAR(128), IN `inpassword` VARCHAR(128))  NO SQL
SELECT userid
FROM users u
WHERE u.usernamehash=inusername AND u.passwordhash=inpassword
LIMIT 1$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `loguser` (IN `inaction` VARCHAR(100), IN `inresult` TINYINT(1), IN `inuseragent` VARCHAR(500), IN `inhost` VARCHAR(50), IN `inuserid` BIGINT(20))  NO SQL
INSERT INTO userslog (userid, datetime, action, result, useragent, host)
VALUES (inuserid, CURRENT_TIMESTAMP, inaction, inresult, inuseragent, inhost)$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `occupiedaccount` (IN `inusername` VARCHAR(128), IN `inemail` VARCHAR(128))  NO SQL
    DETERMINISTIC
SELECT COUNT(*) 
FROM users u
WHERE u.usernamehash=inusername OR u.email=inemail$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `ocuppiednickname` (IN `innickname` VARCHAR(20))  NO SQL
SELECT COUNT(*)
FROM pilots
WHERE nickname=innickname$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `registeruser` (IN `inusername` VARCHAR(128), IN `inpassword` VARCHAR(128), IN `inemail` VARCHAR(100), IN `innewsletter` TINYINT(1), IN `inrules` TINYINT(1), IN `innickname` VARCHAR(20))  NO SQL
BEGIN

INSERT INTO users (usernamehash, passwordhash, email, emailnewsletter, acceptrules, registerdate)
VALUES (inusername, inpassword, inemail, innewsletter, inrules, CURRENT_TIMESTAMP);

SET @insertid = LAST_INSERT_ID();

INSERT INTO pilots (userid, nickname)
VALUES (@insertid, innickname);

INSERT INTO pilotresources (userid)
VALUES (@insertid);

END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `saveplayerdata` (IN `inuserid` BIGINT UNSIGNED, IN `inpositionx` FLOAT, IN `inpositiony` FLOAT, IN `inshipid` INT UNSIGNED, IN `inexperience` BIGINT UNSIGNED, IN `inlevel` INT, IN `inscrap` DOUBLE, IN `inmetal` DOUBLE, IN `inhitpoints` BIGINT UNSIGNED, IN `inshields` BIGINT UNSIGNED, IN `inmapid` BIGINT UNSIGNED, IN `inammunition0` BIGINT UNSIGNED, IN `inammunition1` BIGINT UNSIGNED, IN `inammunition2` BIGINT UNSIGNED, IN `inammunition3` BIGINT UNSIGNED, IN `inrocket0` BIGINT UNSIGNED, IN `inrocket1` BIGINT UNSIGNED, IN `inrocket2` BIGINT UNSIGNED, IN `inisdead` TINYINT(1), IN `inkillerby` VARCHAR(20))  NO SQL
BEGIN

UPDATE pilots
SET 
mapid=inmapid,
positionx=inpositionx,
positiony=inpositiony,
shipid=inshipid,
experience=inexperience,
level=inlevel,
scrap=inscrap,
metal=inmetal,
hitpoints=inhitpoints,
shields=inshields,
isdead=inisdead,
killerby=inkillerby
WHERE userid=inuserid
LIMIT 1;

UPDATE pilotresources
SET 
ammunition0=inammunition0,
ammunition1=inammunition1,
ammunition2=inammunition2,
ammunition3=inammunition3,
rocket0=inrocket0,
rocket1=inrocket1,
rocket2=inrocket2
WHERE userid=inuserid
LIMIT 1;

END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `ammunitions`
--

CREATE TABLE `ammunitions` (
  `ammunitionid` int(10) UNSIGNED NOT NULL,
  `ammunitionname` varchar(50) COLLATE utf8_polish_ci NOT NULL,
  `multiplierplayer` float NOT NULL DEFAULT '1',
  `multiplierenemy` float NOT NULL DEFAULT '1',
  `scrapprice` double DEFAULT NULL,
  `metalprice` double DEFAULT NULL,
  `skillid` tinyint(3) UNSIGNED DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Zrzut danych tabeli `ammunitions`
--

INSERT INTO `ammunitions` (`ammunitionid`, `ammunitionname`, `multiplierplayer`, `multiplierenemy`, `scrapprice`, `metalprice`, `skillid`) VALUES
(100, 'laser0', 1, 1, 1, NULL, NULL),
(101, 'laser1', 2, 2, NULL, 1, NULL),
(102, 'laser2', 3.5, 3.5, NULL, 3, NULL),
(103, 'laser3', 5, 5, NULL, 5, NULL);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `enemies`
--

CREATE TABLE `enemies` (
  `enemyid` int(10) UNSIGNED NOT NULL,
  `enemyname` varchar(50) COLLATE utf8_polish_ci NOT NULL,
  `prefabid` int(11) NOT NULL,
  `hitpoints` bigint(20) NOT NULL,
  `shields` bigint(20) NOT NULL,
  `speed` smallint(5) NOT NULL DEFAULT '20',
  `damage` bigint(20) NOT NULL,
  `shotdistance` int(11) NOT NULL DEFAULT '30',
  `isaggressive` tinyint(1) NOT NULL DEFAULT '0',
  `rewardid` int(10) UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Zrzut danych tabeli `enemies`
--

INSERT INTO `enemies` (`enemyid`, `enemyname`, `prefabid`, `hitpoints`, `shields`, `speed`, `damage`, `shotdistance`, `isaggressive`, `rewardid`) VALUES
(1, 'Kingfisher', 49, 400, 400, 6, 15, 25, 0, 1),
(2, 'Comet', 50, 800, 800, 8, 30, 27, 0, 1),
(3, 'Unicorn', 51, 1000, 1000, 10, 70, 28, 1, 1),
(4, 'Roosevelt', 52, 1600, 1600, 9, 110, 26, 0, 18),
(5, 'Herminia', 53, 2200, 2200, 7, 170, 30, 0, 1),
(6, 'Lancaster', 54, 3000, 3000, 12, 250, 30, 1, 17),
(7, 'Meteor', 55, 3800, 3800, 11, 400, 30, 0, 1),
(8, 'Starhammer', 56, 4600, 4600, 10, 550, 30, 1, 1),
(9, 'Elba', 57, 6000, 6000, 13, 800, 30, 0, 1);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `enemymap`
--

CREATE TABLE `enemymap` (
  `id` int(10) UNSIGNED NOT NULL,
  `enemyid` int(10) UNSIGNED NOT NULL,
  `mapid` int(10) UNSIGNED NOT NULL,
  `count` smallint(6) NOT NULL DEFAULT '50'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Zrzut danych tabeli `enemymap`
--

INSERT INTO `enemymap` (`id`, `enemyid`, `mapid`, `count`) VALUES
(1, 1, 100, 25),
(2, 2, 100, 12),
(3, 3, 100, 3),
(4, 1, 101, 7),
(5, 2, 101, 13),
(6, 3, 101, 20);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `gamesettings`
--

CREATE TABLE `gamesettings` (
  `id` int(11) NOT NULL,
  `key` varchar(100) COLLATE utf8_polish_ci NOT NULL,
  `value` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Zrzut danych tabeli `gamesettings`
--

INSERT INTO `gamesettings` (`id`, `key`, `value`) VALUES
(1, 'mapsize', 1000),
(2, 'portalcount', 10),
(3, 'portalborder', 100);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `maps`
--

CREATE TABLE `maps` (
  `mapid` int(11) UNSIGNED NOT NULL,
  `mapname` varchar(20) COLLATE utf8_polish_ci NOT NULL,
  `ispvp` tinyint(4) NOT NULL DEFAULT '0',
  `requiredlevel` int(11) NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Zrzut danych tabeli `maps`
--

INSERT INTO `maps` (`mapid`, `mapname`, `ispvp`, `requiredlevel`) VALUES
(100, '1-1', 0, 1),
(101, '1-2', 0, 2),
(102, '1-3', 0, 3),
(103, '1-4', 0, 4),
(104, '1-5', 0, 5),
(105, '2-1', 1, 10),
(106, '2-2', 1, 14),
(107, '3-1', 1, 18),
(108, '3-2', 1, 26),
(109, '3-3', 1, 33),
(110, '4-1', 1, 22),
(111, '4-2', 1, 30),
(112, '4-3', 1, 36),
(113, '5-1', 0, 39),
(114, '5-2', 1, 42),
(115, '5-3', 1, 45),
(116, '5-4', 1, 50);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `pilotresources`
--

CREATE TABLE `pilotresources` (
  `userid` bigint(20) UNSIGNED NOT NULL,
  `ammunition0` bigint(20) UNSIGNED NOT NULL DEFAULT '500',
  `ammunition1` bigint(20) UNSIGNED NOT NULL DEFAULT '0',
  `ammunition2` bigint(20) UNSIGNED NOT NULL DEFAULT '0',
  `ammunition3` bigint(20) UNSIGNED NOT NULL DEFAULT '0',
  `rocket0` bigint(20) UNSIGNED NOT NULL DEFAULT '0',
  `rocket1` bigint(20) UNSIGNED NOT NULL DEFAULT '0',
  `rocket2` bigint(20) UNSIGNED NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Zrzut danych tabeli `pilotresources`
--

INSERT INTO `pilotresources` (`userid`, `ammunition0`, `ammunition1`, `ammunition2`, `ammunition3`, `rocket0`, `rocket1`, `rocket2`) VALUES
(100, 500, 0, 0, 0, 0, 0, 0),
(101, 500, 0, 0, 0, 0, 0, 0),
(102, 500, 0, 0, 0, 0, 0, 0),
(103, 500, 0, 0, 0, 0, 0, 0),
(104, 500, 0, 0, 0, 0, 0, 0),
(105, 500, 0, 0, 0, 0, 0, 0);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `pilots`
--

CREATE TABLE `pilots` (
  `userid` bigint(20) UNSIGNED NOT NULL,
  `nickname` varchar(20) COLLATE utf8_polish_ci DEFAULT NULL,
  `mapid` int(11) UNSIGNED NOT NULL DEFAULT '100',
  `positionx` float NOT NULL DEFAULT '100',
  `positiony` float NOT NULL DEFAULT '-100',
  `shipid` int(11) UNSIGNED NOT NULL DEFAULT '100',
  `experience` bigint(20) UNSIGNED NOT NULL DEFAULT '0',
  `level` int(11) NOT NULL DEFAULT '1',
  `scrap` double NOT NULL DEFAULT '0',
  `metal` double NOT NULL DEFAULT '0',
  `hitpoints` bigint(20) NOT NULL DEFAULT '1000',
  `shields` bigint(20) NOT NULL DEFAULT '0',
  `isdead` tinyint(1) NOT NULL DEFAULT '0',
  `killerby` varchar(20) COLLATE utf8_polish_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Zrzut danych tabeli `pilots`
--

INSERT INTO `pilots` (`userid`, `nickname`, `mapid`, `positionx`, `positiony`, `shipid`, `experience`, `level`, `scrap`, `metal`, `hitpoints`, `shields`, `isdead`, `killerby`) VALUES
(100, 'test1', 100, 319.613, -275.955, 100, 2, 1, 0, 0, 874, 706, 0, NULL),
(101, 'test2', 100, 384.48, 22.72, 101, 645, 1, 43, 0, 40000, 1000, 0, NULL),
(102, 'test3', 101, 50, -60, 102, 56286, 1, 79, 0, 598, 77, 0, NULL),
(103, 'test4', 101, 68, -81, 103, 28355, 1, 57, 0, 3700, 1000, 0, NULL),
(104, 'test5', 100, 932, -937, 104, 484, 1, 32, 0, 5000, 1000, 0, NULL),
(105, 'test6', 100, 284.76, -360.721, 105, 0, 1, 0, 0, 41166, 0, 0, NULL);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `portalpositions`
--

CREATE TABLE `portalpositions` (
  `portalpositionid` int(11) NOT NULL,
  `name` varchar(50) COLLATE utf8_polish_ci NOT NULL,
  `positionx` float NOT NULL,
  `positiony` float NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Zrzut danych tabeli `portalpositions`
--

INSERT INTO `portalpositions` (`portalpositionid`, `name`, `positionx`, `positiony`) VALUES
(1, '0;0', 0, 0),
(2, '0;1', 0, 1),
(3, '0;2', 0, 2),
(4, '0;3', 0, 3),
(5, '0;4', 0, 4),
(6, '1;0', 1, 0),
(7, '1;1', 1, 1),
(8, '1;2', 1, 2),
(9, '1;3', 1, 3),
(10, '1;4', 1, 4),
(11, '2;0', 2, 0),
(12, '2;1', 2, 1),
(13, '2;2', 2, 2),
(14, '2;3', 2, 3),
(15, '2;4', 2, 4),
(16, '3;0', 3, 0),
(17, '3;1', 3, 1),
(18, '3;2', 3, 2),
(19, '3;3', 3, 3),
(20, '3;4', 3, 4),
(21, '4;0', 4, 0),
(22, '4;1', 4, 1),
(23, '4;2', 4, 2),
(24, '4;3', 4, 3),
(25, '4;4', 4, 4),
(26, '5;5', 5, 5),
(27, '5;4', 5, 4),
(28, '5;3', 5, 3),
(29, '5;2', 5, 2),
(30, '5;1', 5, 1),
(31, '4;5', 4, 5),
(32, '3;5', 3, 5),
(33, '2;5', 2, 5),
(34, '1;5', 1, 5),
(35, '10;10', 10, 10),
(36, '10;9', 10, 9),
(37, '10;8', 10, 8),
(38, '10;7', 10, 7),
(39, '10;6', 10, 6),
(40, '10;5', 10, 5),
(41, '10;4', 10, 4),
(42, '10;3', 10, 3),
(43, '10;2', 10, 2),
(44, '10;1', 10, 1),
(45, '9;10', 9, 10),
(46, '9;9', 9, 9),
(47, '9;8', 9, 8),
(48, '9;7', 9, 7),
(49, '9;6', 9, 6),
(50, '9;5', 9, 5),
(51, '9;4', 9, 4),
(52, '9;3', 9, 3),
(53, '9;2', 9, 2),
(54, '9;1', 9, 1),
(55, '8;10', 8, 10),
(56, '8;9', 8, 9),
(57, '8;8', 8, 8),
(58, '8;7', 8, 7),
(59, '8;6', 8, 6),
(60, '8;5', 8, 5),
(61, '8;4', 8, 4),
(62, '8;3', 8, 3),
(63, '8;2', 8, 2),
(64, '8;1', 8, 1),
(65, '7;10', 7, 10),
(66, '7;9', 7, 9),
(67, '7;8', 7, 8),
(68, '7;7', 7, 7),
(69, '7;6', 7, 6),
(70, '7;5', 7, 5),
(71, '7;4', 7, 4),
(72, '7;3', 7, 3),
(73, '7;2', 7, 2),
(74, '7;1', 7, 1),
(75, '6;10', 6, 10),
(76, '6;9', 6, 9),
(77, '6;8', 6, 8),
(78, '6;7', 6, 7),
(79, '6;6', 6, 6),
(80, '6;5', 6, 5),
(81, '6;4', 6, 4),
(82, '6;3', 6, 3),
(83, '6;2', 6, 2),
(84, '6;1', 6, 1),
(85, '5;10', 5, 10),
(86, '5;9', 5, 9),
(87, '5;8', 5, 8),
(88, '5;7', 5, 7),
(89, '5;6', 5, 6),
(90, '4;10', 4, 10),
(91, '4;9', 4, 9),
(92, '4;8', 4, 8),
(93, '4;7', 4, 7),
(94, '4;6', 4, 6),
(95, '3;10', 3, 10),
(96, '3;9', 3, 9),
(97, '3;8', 3, 8),
(98, '3;7', 3, 7),
(99, '3;6', 3, 6),
(100, '2;10', 2, 10),
(101, '2;9', 2, 9),
(102, '2;8', 2, 8),
(103, '2;7', 2, 7),
(104, '2;6', 2, 6),
(105, '1;10', 1, 10),
(106, '1;9', 1, 9),
(107, '1;8', 1, 8),
(108, '1;7', 1, 7),
(109, '1;6', 1, 6),
(110, '10;0', 10, 0),
(111, '9;0', 9, 0),
(112, '8;0', 8, 0),
(113, '7;0', 7, 0),
(114, '6;0', 6, 0),
(115, '5;0', 5, 0),
(116, '0;10', 0, 10),
(117, '0;9', 0, 9),
(118, '0;8', 0, 8),
(119, '0;7', 0, 7),
(120, '0;6', 0, 6),
(121, '0;5', 0, 5);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `portals`
--

CREATE TABLE `portals` (
  `portalid` int(11) NOT NULL,
  `prefabid` int(11) NOT NULL,
  `mapid` int(11) UNSIGNED NOT NULL,
  `portalpositionid` int(11) DEFAULT NULL,
  `target_mapid` int(11) UNSIGNED NOT NULL,
  `target_portalpositionid` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Zrzut danych tabeli `portals`
--

INSERT INTO `portals` (`portalid`, `prefabid`, `mapid`, `portalpositionid`, `target_mapid`, `target_portalpositionid`) VALUES
(1, 31, 100, 35, 101, 1),
(2, 31, 101, 1, 100, 35),
(3, 37, 101, 85, 102, 110),
(5, 43, 102, 110, 101, 85),
(6, 31, 101, 35, 103, 1),
(7, 31, 103, 1, 101, 35),
(10, 31, 101, 40, 104, 116),
(11, 31, 104, 116, 101, 40),
(60, 31, 102, 116, 105, 110),
(61, 31, 105, 110, 102, 116),
(62, 31, 102, 35, 106, 1),
(63, 31, 106, 1, 102, 35),
(64, 31, 105, 38, 106, 119),
(65, 31, 106, 119, 105, 38),
(66, 31, 106, 35, 107, 116),
(67, 31, 107, 116, 106, 35),
(68, 31, 103, 75, 107, 1),
(69, 31, 107, 1, 103, 75),
(70, 31, 103, 39, 108, 1),
(71, 31, 108, 1, 103, 39),
(72, 31, 107, 110, 108, 116),
(73, 31, 108, 116, 107, 110),
(74, 31, 107, 35, 109, 5),
(75, 31, 109, 5, 107, 35),
(76, 31, 108, 35, 109, 21),
(77, 31, 109, 21, 108, 35),
(78, 31, 108, 110, 110, 116),
(79, 31, 110, 116, 108, 110),
(80, 31, 104, 35, 110, 1),
(81, 31, 110, 1, 104, 35),
(82, 31, 104, 40, 111, 116),
(83, 31, 111, 116, 104, 40),
(84, 31, 110, 110, 111, 85),
(85, 31, 111, 85, 110, 110),
(86, 31, 111, 35, 112, 21),
(87, 31, 112, 21, 111, 35),
(88, 31, 110, 40, 112, 5),
(89, 31, 112, 5, 110, 40),
(90, 31, 112, 116, 113, 1),
(91, 31, 113, 1, 112, 116),
(92, 31, 109, 110, 113, 116),
(93, 31, 113, 116, 109, 110),
(94, 31, 113, 85, 114, 115),
(95, 31, 114, 115, 113, 85),
(96, 31, 113, 40, 115, 121),
(97, 31, 115, 121, 113, 40),
(98, 31, 114, 40, 116, 4),
(99, 31, 116, 4, 114, 40),
(100, 31, 115, 85, 116, 16),
(101, 31, 116, 16, 115, 85);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `prefabs`
--

CREATE TABLE `prefabs` (
  `prefabid` int(11) NOT NULL,
  `prefabname` varchar(100) COLLATE utf8_polish_ci NOT NULL,
  `prefabtypeid` int(11) NOT NULL DEFAULT '1',
  `description` varchar(300) COLLATE utf8_polish_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Zrzut danych tabeli `prefabs`
--

INSERT INTO `prefabs` (`prefabid`, `prefabname`, `prefabtypeid`, `description`) VALUES
(1, 'Avius', 1, ''),
(2, 'Challenger', 1, ''),
(3, 'Executor', 1, ''),
(4, 'Nihilus', 1, ''),
(5, 'Proximo', 1, ''),
(6, 'Verminus', 1, ''),
(7, 'Oregon\r\n', 1, ''),
(8, 'Scavenger\r\n', 1, ''),
(9, 'Herald\r\n', 1, ''),
(10, 'Colossus\r\n', 1, ''),
(11, 'Trenxal\r\n', 1, ''),
(12, 'Spectator\r\n', 1, ''),
(13, 'Cain\r\n', 1, ''),
(14, 'Neutron\r\n', 1, ''),
(15, 'Determination\r\n', 1, ''),
(16, 'Ambition\r\n', 1, ''),
(17, 'Twilight\r\n', 1, ''),
(18, 'Corsair\r\n', 1, ''),
(19, 'Escorial\r\n', 1, ''),
(20, 'Phobos\r\n', 1, ''),
(21, 'Achilles\r\n', 1, ''),
(22, 'Valkyrie\r\n', 1, ''),
(23, 'Intrepid\r\n', 1, ''),
(24, 'Ravana\r\n', 1, ''),
(25, 'Berserk\r\n', 1, ''),
(26, 'Crusher\r\n', 1, ''),
(27, 'Andromeda\r\n', 1, ''),
(28, 'Prennia\r\n', 1, ''),
(29, 'Navigator\r\n', 1, ''),
(30, 'Zeus\r\n', 1, ''),
(31, 'Large_Blue', 2, ''),
(32, 'Large_Gray', 2, ''),
(33, 'Large_Green', 2, ''),
(34, 'Large_Purple', 2, ''),
(35, 'Large_Red', 2, ''),
(36, 'Large_Teal', 2, ''),
(37, 'Medium_Blue', 2, ''),
(38, 'Medium_Gray', 2, ''),
(39, 'Medium_Green', 2, ''),
(40, 'Medium_Purple', 2, ''),
(41, 'Medium_Red', 2, ''),
(42, 'Medium_Teal', 2, ''),
(43, 'Small_Blue', 2, ''),
(44, 'Small_Gray', 2, ''),
(45, 'Small_Green', 2, ''),
(46, 'Small_Purple', 2, ''),
(47, 'Small_Red', 2, ''),
(48, 'Small_Teal', 2, ''),
(49, 'Kingfisher', 1, ''),
(50, 'Comet', 1, ''),
(51, 'Unicorn', 1, ''),
(52, 'Roosevelt', 1, ''),
(53, 'Herminia', 1, ''),
(54, 'Lancaster', 1, ''),
(55, 'Meteor', 1, ''),
(56, 'Starhammer', 1, ''),
(57, 'Elba', 1, ''),
(58, 'Serpent', 1, ''),
(59, 'Fortitude', 1, ''),
(60, 'Firebrand', 1, ''),
(61, 'Neptune', 1, ''),
(62, 'Empress', 1, ''),
(63, 'Relentless', 1, ''),
(64, 'Aquitaine', 1, ''),
(65, 'Aurora', 1, ''),
(66, 'Paradise', 1, ''),
(67, 'Spectrum', 1, ''),
(68, 'Crocodile', 1, ''),
(69, 'Elena', 1, ''),
(70, 'Stalwart', 1, ''),
(71, 'Valor', 1, ''),
(72, 'Tomahawk', 1, ''),
(73, 'Arthas', 1, ''),
(74, 'Fate', 1, ''),
(75, 'Lightning', 1, ''),
(76, 'Sagittarius', 1, ''),
(77, 'Harbinger', 1, ''),
(78, 'Thanatos', 1, ''),
(79, 'Pontiac', 1, ''),
(80, 'Thunderstorm', 1, ''),
(81, 'Gibraltar', 1, ''),
(82, 'Armageddon', 1, ''),
(83, 'Destroyer', 1, ''),
(84, 'Thebes', 1, ''),
(85, 'Phoenix', 1, ''),
(86, 'Leviathan', 1, ''),
(87, 'Excursionist', 1, ''),
(88, 'Nostradamus', 1, ''),
(89, 'Scorpio', 1, ''),
(90, 'Vanquisher', 1, ''),
(91, 'Syracuse', 1, ''),
(92, 'Geisha', 1, ''),
(93, 'Chronos', 1, ''),
(94, 'Cataphract', 1, ''),
(95, 'Millenium', 1, ''),
(96, 'Gauntlet', 1, ''),
(97, 'Saber', 1, ''),
(98, 'Vision', 1, ''),
(99, 'Galatea', 1, ''),
(100, 'Carthage', 1, ''),
(101, 'Jellyfish', 1, ''),
(102, 'Vindicator', 1, ''),
(103, 'Damascus', 1, ''),
(104, 'Lavanda', 1, ''),
(105, 'Karnack', 1, ''),
(106, 'Cyclop', 1, ''),
(107, 'Zion', 1, ''),
(108, 'Eternal', 1, ''),
(109, 'Blade', 1, ''),
(110, 'Goliath', 1, ''),
(111, 'Ingenuity', 1, ''),
(112, 'Hunter', 1, ''),
(113, 'Voyager', 1, ''),
(114, 'Wellington', 1, ''),
(115, 'Minotuar', 1, ''),
(116, 'Immortal', 1, ''),
(117, 'Liberator', 1, ''),
(118, 'Oblivion', 1, '');

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `prefabs_types`
--

CREATE TABLE `prefabs_types` (
  `prefabtypeid` int(11) NOT NULL,
  `prefabtypename` varchar(100) COLLATE utf8_polish_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Zrzut danych tabeli `prefabs_types`
--

INSERT INTO `prefabs_types` (`prefabtypeid`, `prefabtypename`) VALUES
(1, 'Ships'),
(2, 'Jumpgates');

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `rewards`
--

CREATE TABLE `rewards` (
  `rewardid` int(10) UNSIGNED NOT NULL,
  `experience` bigint(20) UNSIGNED DEFAULT NULL,
  `metal` double DEFAULT NULL,
  `scrap` double DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Zrzut danych tabeli `rewards`
--

INSERT INTO `rewards` (`rewardid`, `experience`, `metal`, `scrap`) VALUES
(1, 1, NULL, NULL),
(2, 2, NULL, NULL),
(3, 4, NULL, NULL),
(4, 8, NULL, NULL),
(5, 16, NULL, NULL),
(6, 32, NULL, NULL),
(7, 64, NULL, NULL),
(8, 128, NULL, NULL),
(9, 256, NULL, NULL),
(10, 307, 1, 4),
(11, 369, 2, 8),
(12, 442, 3, 12),
(13, 531, 4, 16),
(14, 637, 5, 20),
(15, 764, 6, 24),
(16, 917, 7, 28),
(17, 1101, 8, 32),
(18, 1321, 9, 36),
(19, 1585, 10, 50),
(20, 1902, 14, 70),
(21, 2283, 18, 90),
(22, 2739, 22, 121),
(23, 3287, 26, 143),
(24, 3944, 30, 180),
(25, 4339, 33, 215),
(26, 4772, 38, 266),
(27, 5250, 41, 308),
(28, 5775, 44, 352),
(29, 6352, 47, 400),
(30, 6987, 50, 450);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `rockets`
--

CREATE TABLE `rockets` (
  `rocketid` int(10) UNSIGNED NOT NULL,
  `rocketname` varchar(50) COLLATE utf8_polish_ci NOT NULL,
  `scrapprice` double DEFAULT NULL,
  `metalprice` double DEFAULT NULL,
  `skillid` tinyint(3) UNSIGNED DEFAULT NULL,
  `damage` int(10) UNSIGNED NOT NULL DEFAULT '1000'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Zrzut danych tabeli `rockets`
--

INSERT INTO `rockets` (`rocketid`, `rocketname`, `scrapprice`, `metalprice`, `skillid`, `damage`) VALUES
(100, 'rocket0', 10, NULL, NULL, 1000),
(101, 'rocket1', 1000, NULL, NULL, 3000),
(102, 'rocket2', NULL, 10, NULL, 7000);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `ships`
--

CREATE TABLE `ships` (
  `shipid` int(11) UNSIGNED NOT NULL,
  `shipname` varchar(100) COLLATE utf8_polish_ci NOT NULL,
  `prefabid` int(11) NOT NULL,
  `requiredlevel` int(11) NOT NULL DEFAULT '1',
  `scrapprice` double DEFAULT NULL,
  `metalprice` double DEFAULT NULL,
  `lasers` tinyint(3) UNSIGNED NOT NULL DEFAULT '1',
  `generators` tinyint(3) UNSIGNED NOT NULL DEFAULT '1',
  `extras` tinyint(3) UNSIGNED NOT NULL DEFAULT '1',
  `speed` smallint(5) UNSIGNED NOT NULL DEFAULT '50',
  `cargo` smallint(5) UNSIGNED NOT NULL DEFAULT '100',
  `hitpoints` bigint(20) UNSIGNED NOT NULL DEFAULT '1000',
  `rewardid` int(10) UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Zrzut danych tabeli `ships`
--

INSERT INTO `ships` (`shipid`, `shipname`, `prefabid`, `requiredlevel`, `scrapprice`, `metalprice`, `lasers`, `generators`, `extras`, `speed`, `cargo`, `hitpoints`, `rewardid`) VALUES
(100, 'Avius', 1, 1, 0, NULL, 1, 1, 1, 65, 40, 1000, 1),
(101, 'Challenger', 2, 2, 1500, NULL, 2, 1, 1, 55, 65, 1600, 2),
(102, 'Executor', 3, 3, 5000, NULL, 2, 2, 1, 400, 80, 2500, 3),
(103, 'Nihilus', 4, 4, 12500, NULL, 3, 2, 1, 70, 100, 3700, 4),
(104, 'Proximo', 5, 5, 21000, NULL, 3, 3, 1, 65, 150, 5000, 5),
(105, 'Verminus', 6, 6, NULL, 2100, 4, 5, 1, 50, 200, 4500, 6),
(106, 'Oregon', 7, 7, 48000, NULL, 5, 4, 2, 60, 250, 7000, 7),
(107, 'Scavenger', 8, 8, 60000, NULL, 7, 5, 2, 65, 350, 6000, 8),
(108, 'Herald', 9, 9, 75000, NULL, 8, 5, 2, 70, 450, 8500, 9),
(109, 'Colossus', 10, 10, NULL, 5000, 10, 6, 2, 70, 500, 10000, 10),
(110, 'Trenxal', 11, 12, 510000, NULL, 11, 6, 2, 80, 450, 12000, 11),
(111, 'Spectator', 12, 14, 675000, NULL, 12, 7, 2, 95, 550, 18000, 12),
(112, 'Cain', 13, 16, 750000, NULL, 13, 7, 2, 135, 300, 22000, 13),
(113, 'Neutron', 14, 18, 825000, NULL, 14, 8, 3, 115, 400, 35000, 14),
(114, 'Determination', 15, 20, NULL, 17500, 15, 9, 5, 125, 350, 42000, 15),
(115, 'Ambition', 16, 22, 3450000, NULL, 16, 10, 4, 110, 700, 65000, 16),
(116, 'Twilight', 17, 24, 4000000, NULL, 17, 11, 3, 100, 650, 77000, 17),
(117, 'Corsair', 18, 26, 4750000, 5000, 19, 12, 2, 120, 500, 95000, 18),
(118, 'Escorial', 19, 28, 5000000, NULL, 18, 13, 4, 145, 800, 115000, 19),
(119, 'Phobos', 20, 30, NULL, 22500, 20, 14, 3, 110, 1000, 100000, 20),
(120, 'Achilles', 21, 32, 7850000, NULL, 21, 15, 4, 100, 950, 130000, 21),
(121, 'Valkyrie', 22, 34, 8350000, NULL, 23, 15, 3, 135, 1100, 110000, 22),
(122, 'Intrepid', 23, 36, 9150000, NULL, 24, 14, 4, 130, 1050, 120000, 23),
(123, 'Ravana', 24, 38, 9999999, NULL, 25, 15, 4, 120, 1250, 135000, 24),
(124, 'Berserk', 25, 40, NULL, 25000, 27, 17, 3, 110, 1400, 150000, 25),
(125, 'Crusher', 26, 42, NULL, 44000, 26, 17, 6, 120, 1750, 165000, 26),
(126, 'Andromeda', 27, 44, 99999999, NULL, 28, 18, 4, 125, 1500, 150000, 27),
(127, 'Prennia', 28, 46, NULL, 65000, 29, 19, 5, 125, 1800, 175000, 28),
(128, 'Navigator', 29, 48, NULL, 88000, 30, 19, 7, 30, 2500, 180000, 29),
(129, 'Zeus', 30, 50, NULL, 130000, 30, 20, 10, 30, 2000, 200000, 30);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `users`
--

CREATE TABLE `users` (
  `userid` bigint(20) UNSIGNED NOT NULL,
  `usernamehash` varchar(128) COLLATE utf8_polish_ci NOT NULL,
  `passwordhash` varchar(128) COLLATE utf8_polish_ci NOT NULL,
  `email` varchar(100) COLLATE utf8_polish_ci NOT NULL,
  `emailnewsletter` tinyint(1) NOT NULL DEFAULT '0',
  `acceptrules` tinyint(1) NOT NULL DEFAULT '0',
  `ban` tinyint(1) NOT NULL DEFAULT '0',
  `registerdate` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci ROW_FORMAT=REDUNDANT;

--
-- Zrzut danych tabeli `users`
--

INSERT INTO `users` (`userid`, `usernamehash`, `passwordhash`, `email`, `emailnewsletter`, `acceptrules`, `ban`, `registerdate`) VALUES
(100, '22C9E677EA7905AB18A8DE3535BB0D4730685795EB1FA26ABC2E2F4B12ED6C8198D979C8F3B61C43FEBCB714BBFAFDD89F4955F6D7FE541722193ED908A3BA1A', '6FD332AD992E8A0DC450533F2729EB8DFBAAFF697911546F495AD6E7E60772136724594626C81BED60188277F506800FB962198F7BD3021A251FCEFC30089EB3', 'test1', 0, 1, 0, '2018-12-29 20:22:19'),
(101, '8E70E934E70EC8DA6BA82C2C6FDF8007ACA4AE8CAAD7FE945AEEAF243D43E5CFB2A65CBCFA6E68ABBDB61143B895B05D8B3075FE40502FB804C2DCC8077EB017', '6FD332AD992E8A0DC450533F2729EB8DFBAAFF697911546F495AD6E7E60772136724594626C81BED60188277F506800FB962198F7BD3021A251FCEFC30089EB3', 'test2', 0, 1, 0, '2018-12-29 20:25:12'),
(102, '9E31434171A164405F2D3546498AE755DBDE85EE9FFC4553AEA816919C6FB6D9868ADF5BD59E26D9686A281AC5ECF5368511DC8D3EB584D1C44EA57197B4FA64', '6FD332AD992E8A0DC450533F2729EB8DFBAAFF697911546F495AD6E7E60772136724594626C81BED60188277F506800FB962198F7BD3021A251FCEFC30089EB3', 'test3', 1, 1, 0, '2018-12-29 20:27:35'),
(103, 'F9DE02FC94D0261D27A2B54210D5FC8795D2BAE4E6A229C8EAC9158999142E3301267F591640BE08C1507889760634A3872D93BCFC13459CB7D7D0D2B593E127', '6FD332AD992E8A0DC450533F2729EB8DFBAAFF697911546F495AD6E7E60772136724594626C81BED60188277F506800FB962198F7BD3021A251FCEFC30089EB3', 'test4', 0, 1, 0, '2018-12-29 21:00:23'),
(104, '368E28420D2F469DFA61A8775A049F5D33E8D498EF4173BA945F2CE73D027C309A450651B7DF73609F8EC2F7BFEDC3DC21EDE8E89BAE5227C6A680A1942EFFB6', '6FD332AD992E8A0DC450533F2729EB8DFBAAFF697911546F495AD6E7E60772136724594626C81BED60188277F506800FB962198F7BD3021A251FCEFC30089EB3', 'test5', 1, 1, 0, '2018-12-29 21:49:24'),
(105, 'B4FDF8DD90B9FF39276EF1C959D08DED6369FD6FF36E231924AD3A8F5A3689DB93CAF54DF37A63A07C64E2F712190379AEC9399A2189439ACC7829B1486D1B93', '6FD332AD992E8A0DC450533F2729EB8DFBAAFF697911546F495AD6E7E60772136724594626C81BED60188277F506800FB962198F7BD3021A251FCEFC30089EB3', 'test6', 1, 1, 0, '2018-12-29 21:49:37');

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `userslog`
--

CREATE TABLE `userslog` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `userid` bigint(20) UNSIGNED DEFAULT NULL,
  `datetime` datetime NOT NULL,
  `action` varchar(100) COLLATE utf8_polish_ci NOT NULL,
  `result` tinyint(1) NOT NULL,
  `useragent` varchar(500) COLLATE utf8_polish_ci NOT NULL,
  `host` varchar(50) COLLATE utf8_polish_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Zrzut danych tabeli `userslog`
--

INSERT INTO `userslog` (`id`, `userid`, `datetime`, `action`, `result`, `useragent`, `host`) VALUES
(1, NULL, '2018-12-29 20:22:09', 'LogIn', 0, 'websocket-sharp/1.0', 'localhost:24231'),
(2, 100, '2018-12-29 20:22:19', 'Register', 0, 'websocket-sharp/1.0', 'localhost:24231'),
(3, 100, '2018-12-29 20:24:06', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(4, 101, '2018-12-29 20:25:12', 'Register', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(5, 101, '2018-12-29 20:25:12', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(6, 100, '2018-12-29 20:27:14', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(7, 102, '2018-12-29 20:27:35', 'Register', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(8, 102, '2018-12-29 20:27:35', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(9, 100, '2018-12-29 20:28:02', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(10, 100, '2018-12-29 20:57:30', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(11, 100, '2018-12-29 20:57:53', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(12, 100, '2018-12-29 20:57:55', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(13, 100, '2018-12-29 20:57:58', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(14, 100, '2018-12-29 20:57:59', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(15, 100, '2018-12-29 20:57:59', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(16, 100, '2018-12-29 20:57:59', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(17, 100, '2018-12-29 20:58:07', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(18, 100, '2018-12-29 20:58:07', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(19, 100, '2018-12-29 20:58:07', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(20, 100, '2018-12-29 20:58:13', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(21, 100, '2018-12-29 21:00:08', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(22, NULL, '2018-12-29 21:00:12', 'LogIn', 0, 'websocket-sharp/1.0', 'localhost:24231'),
(23, 103, '2018-12-29 21:00:23', 'Register', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(24, 103, '2018-12-29 21:00:23', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(25, 103, '2018-12-29 21:01:16', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(26, 103, '2018-12-29 21:02:46', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(27, 103, '2018-12-29 21:04:02', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(28, 103, '2018-12-29 21:05:23', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(29, 103, '2018-12-29 21:06:55', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(30, 103, '2018-12-29 21:06:57', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(31, 103, '2018-12-29 21:06:57', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(32, 103, '2018-12-29 21:06:58', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(33, 103, '2018-12-29 21:07:36', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(34, 103, '2018-12-29 21:07:37', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(35, 103, '2018-12-29 21:07:38', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(36, 103, '2018-12-29 21:08:41', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(37, 103, '2018-12-29 21:11:58', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(38, 103, '2018-12-29 21:21:12', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(39, 103, '2018-12-29 21:26:52', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(40, 103, '2018-12-29 21:26:57', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(41, 103, '2018-12-29 21:27:02', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(42, 103, '2018-12-29 21:48:58', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(43, 104, '2018-12-29 21:49:24', 'Register', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(44, 104, '2018-12-29 21:49:24', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(45, 105, '2018-12-29 21:49:37', 'Register', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(46, 105, '2018-12-29 21:49:37', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(47, 104, '2018-12-29 21:49:57', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(48, 105, '2018-12-29 21:50:05', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(49, 104, '2018-12-29 21:50:26', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(50, 104, '2018-12-29 21:51:51', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(51, 104, '2018-12-29 22:09:56', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(52, 104, '2018-12-29 23:45:09', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(53, 102, '2018-12-29 23:45:20', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(54, 102, '2018-12-29 23:49:41', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(55, 102, '2018-12-30 09:26:22', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(56, 102, '2018-12-30 09:26:31', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(57, 102, '2018-12-30 09:26:46', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(58, 104, '2018-12-30 09:26:47', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(59, 102, '2018-12-30 10:59:59', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(60, 104, '2018-12-30 11:00:15', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(61, 102, '2018-12-30 11:00:19', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(62, 102, '2018-12-30 11:20:11', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(63, 104, '2018-12-30 11:20:27', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(64, 102, '2018-12-30 11:20:29', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(65, 102, '2018-12-30 11:21:50', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(66, 103, '2018-12-30 11:22:18', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(67, 103, '2018-12-30 11:25:01', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(68, 102, '2018-12-30 11:25:03', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(69, 104, '2018-12-30 11:25:37', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(70, 102, '2018-12-30 11:26:20', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(71, 102, '2018-12-30 11:28:33', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(72, 104, '2018-12-30 11:28:34', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(73, 102, '2018-12-30 11:30:21', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(74, 104, '2018-12-30 11:30:22', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(75, 104, '2018-12-30 11:31:32', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(76, 103, '2018-12-30 11:31:41', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(77, 102, '2018-12-30 11:31:52', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(78, 102, '2018-12-30 11:37:16', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(79, 103, '2018-12-30 11:37:17', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(80, 104, '2018-12-30 11:37:36', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(81, 102, '2018-12-30 11:40:01', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(82, 104, '2018-12-30 11:40:02', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(83, 103, '2018-12-30 11:40:23', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(84, 102, '2018-12-30 12:49:02', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(85, 102, '2018-12-30 12:49:18', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(86, 101, '2018-12-30 12:49:21', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(87, 101, '2018-12-30 12:50:46', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(88, 102, '2018-12-30 12:50:47', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(89, 102, '2018-12-30 12:52:09', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(90, 101, '2018-12-30 12:52:12', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(91, 102, '2018-12-30 12:54:47', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(92, 101, '2018-12-30 12:54:50', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(93, 101, '2018-12-30 12:55:27', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(94, 102, '2018-12-30 12:55:28', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(95, 101, '2018-12-30 13:00:01', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(96, 102, '2018-12-30 13:00:02', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(97, 102, '2018-12-30 13:04:20', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(98, 101, '2018-12-30 13:04:27', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(99, 101, '2018-12-30 13:08:34', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(100, 102, '2018-12-30 13:08:35', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(101, 101, '2018-12-30 13:09:08', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(102, 101, '2018-12-30 13:13:12', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(103, 102, '2018-12-30 13:13:12', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(104, 102, '2018-12-30 13:17:30', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(105, 101, '2018-12-30 13:17:32', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(106, 101, '2018-12-30 13:17:51', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(107, 102, '2018-12-30 13:18:44', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(108, 102, '2018-12-30 13:25:29', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(109, 101, '2018-12-30 13:25:29', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(110, 102, '2018-12-30 13:27:11', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(111, 101, '2018-12-30 13:27:19', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(112, 102, '2018-12-30 13:28:07', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(113, 102, '2018-12-30 13:29:58', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(114, 101, '2018-12-30 13:29:59', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(115, 102, '2018-12-30 13:45:29', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(116, 102, '2018-12-30 13:45:56', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(117, 101, '2018-12-30 13:45:57', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(118, 102, '2018-12-30 13:46:20', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(119, 102, '2018-12-30 19:07:12', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(120, 102, '2018-12-30 19:07:34', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(121, 101, '2018-12-30 19:07:35', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(122, 101, '2018-12-30 19:08:39', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(123, 102, '2018-12-30 19:08:41', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(124, 102, '2018-12-30 19:11:08', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(125, 101, '2018-12-30 19:11:10', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(126, 102, '2018-12-30 19:13:12', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(127, 101, '2018-12-30 19:13:14', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(128, 101, '2018-12-30 19:13:58', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(129, 102, '2018-12-30 19:13:58', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(130, 102, '2018-12-30 19:36:11', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(131, 101, '2018-12-30 19:36:17', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(132, 102, '2018-12-30 19:37:28', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(133, 101, '2018-12-30 19:37:35', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(134, 102, '2018-12-30 19:39:48', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(135, 101, '2018-12-30 19:39:50', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(136, 102, '2018-12-30 19:40:07', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(137, 102, '2018-12-30 19:40:22', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(138, 102, '2018-12-30 19:40:41', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(139, 102, '2018-12-30 20:09:26', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(140, 101, '2018-12-30 20:09:28', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(141, 103, '2018-12-30 20:09:46', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(142, 102, '2018-12-30 21:00:55', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(143, 103, '2018-12-30 21:00:56', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(144, 102, '2018-12-30 21:02:34', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(145, 103, '2018-12-30 21:02:36', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(146, 102, '2018-12-30 21:09:02', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(147, 103, '2018-12-30 21:09:05', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(148, 102, '2018-12-30 21:15:39', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(149, 103, '2018-12-30 21:15:52', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(150, 103, '2018-12-30 21:19:54', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(151, 102, '2018-12-30 21:19:55', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(152, 102, '2018-12-30 21:21:14', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(153, 103, '2018-12-30 21:21:16', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(154, 102, '2018-12-30 21:23:23', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(155, 103, '2018-12-30 21:23:37', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(156, 102, '2018-12-30 21:23:51', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(157, 102, '2018-12-30 21:24:04', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(158, 103, '2018-12-30 21:24:29', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(159, 102, '2018-12-30 21:24:30', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(160, 103, '2018-12-30 21:25:40', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(161, 102, '2018-12-30 21:25:41', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(162, 102, '2018-12-30 21:25:46', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(163, 102, '2018-12-30 21:28:15', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(164, 103, '2018-12-30 21:28:22', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(165, 102, '2018-12-30 21:30:12', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(166, 103, '2018-12-30 21:30:15', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(167, 102, '2018-12-30 21:33:05', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(168, 103, '2018-12-30 21:33:08', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(169, 103, '2018-12-30 21:33:22', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(170, 102, '2018-12-30 21:33:31', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(171, 103, '2018-12-30 21:40:52', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(172, 102, '2018-12-30 21:40:53', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(173, 102, '2018-12-30 21:41:58', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(174, 102, '2018-12-30 21:42:13', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(175, 102, '2018-12-30 21:42:22', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(176, 102, '2018-12-30 21:42:36', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(177, 103, '2018-12-30 21:42:56', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(178, 102, '2018-12-30 21:42:59', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(179, 103, '2018-12-30 21:51:47', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(180, 102, '2018-12-30 21:51:48', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(181, 102, '2018-12-30 21:53:49', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(182, 102, '2018-12-30 21:54:20', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(183, 102, '2018-12-30 21:59:06', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(184, 103, '2018-12-30 21:59:54', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(185, 102, '2018-12-30 21:59:54', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(186, 102, '2018-12-30 22:08:50', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(187, 103, '2018-12-30 22:08:51', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(188, 103, '2018-12-30 22:10:48', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(189, 102, '2018-12-30 22:10:50', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(190, 102, '2018-12-30 22:14:48', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(191, 103, '2018-12-30 22:14:49', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(192, 102, '2018-12-30 22:15:03', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(193, 103, '2018-12-30 22:15:11', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(194, 102, '2018-12-30 22:15:12', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(195, 102, '2018-12-30 22:17:51', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(196, 103, '2018-12-30 22:17:53', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(197, 103, '2018-12-30 22:19:12', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(198, 102, '2018-12-30 22:19:13', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(199, 102, '2018-12-30 22:25:31', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(200, 103, '2018-12-30 22:25:34', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(201, 102, '2018-12-31 14:50:59', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(202, 102, '2018-12-31 14:51:16', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(203, 102, '2018-12-31 16:12:44', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(204, 102, '2018-12-31 16:12:44', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(205, 102, '2018-12-31 16:12:49', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(206, 102, '2018-12-31 16:13:51', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(207, 102, '2018-12-31 16:13:53', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(208, 102, '2018-12-31 16:16:44', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(209, 102, '2018-12-31 16:16:55', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(210, 102, '2018-12-31 16:16:55', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(211, 102, '2018-12-31 16:16:55', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(212, 102, '2018-12-31 16:19:49', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(213, 102, '2018-12-31 16:21:00', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(214, 102, '2018-12-31 16:22:33', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(215, 102, '2018-12-31 16:23:02', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(216, 102, '2018-12-31 16:23:49', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(217, 102, '2018-12-31 16:23:55', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(218, 102, '2018-12-31 16:25:09', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(219, 102, '2018-12-31 16:25:15', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(220, 102, '2018-12-31 16:26:16', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(221, 102, '2018-12-31 16:26:22', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(222, 102, '2018-12-31 16:36:17', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(223, 103, '2018-12-31 16:36:18', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(224, 102, '2018-12-31 16:36:35', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(225, 102, '2018-12-31 16:37:37', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(226, 102, '2018-12-31 16:38:27', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(227, 102, '2018-12-31 16:39:32', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(228, 102, '2018-12-31 16:41:47', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(229, 102, '2018-12-31 16:41:56', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(230, 102, '2018-12-31 16:42:07', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(231, 102, '2018-12-31 16:42:21', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(232, 102, '2018-12-31 16:43:01', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(233, 102, '2018-12-31 16:43:26', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(234, 102, '2018-12-31 16:55:20', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(235, 102, '2018-12-31 16:56:04', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(236, 102, '2018-12-31 16:56:16', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(237, 103, '2018-12-31 16:56:28', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(238, 102, '2018-12-31 16:56:29', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(239, 102, '2018-12-31 16:56:51', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(240, 102, '2018-12-31 16:57:33', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(241, 102, '2018-12-31 16:59:50', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(242, 103, '2018-12-31 16:59:52', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(243, 102, '2018-12-31 17:00:17', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(244, 103, '2018-12-31 17:04:08', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(245, 102, '2018-12-31 17:04:09', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(246, 102, '2018-12-31 17:04:40', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(247, 102, '2018-12-31 17:05:07', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(248, 103, '2018-12-31 17:05:09', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(249, 102, '2018-12-31 17:07:59', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(250, 103, '2018-12-31 17:08:02', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(251, 102, '2018-12-31 17:08:13', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(252, 102, '2018-12-31 17:08:35', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(253, 103, '2018-12-31 17:10:11', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(254, 102, '2018-12-31 17:10:12', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(255, 102, '2018-12-31 17:18:51', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(256, 102, '2018-12-31 17:27:41', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(257, 102, '2018-12-31 17:39:30', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(258, 102, '2018-12-31 17:48:49', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(259, 102, '2018-12-31 17:51:53', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(260, 102, '2018-12-31 17:54:50', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(261, 102, '2018-12-31 18:00:58', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(262, 102, '2018-12-31 21:23:28', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(263, 103, '2018-12-31 21:23:45', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(264, 102, '2018-12-31 21:23:46', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(265, 102, '2018-12-31 21:24:21', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(266, 103, '2018-12-31 21:24:43', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(267, 102, '2018-12-31 21:24:47', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(268, 103, '2018-12-31 21:26:42', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(269, 102, '2018-12-31 21:26:43', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(270, 102, '2018-12-31 21:27:03', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(271, 102, '2018-12-31 21:28:34', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(272, 102, '2018-12-31 21:29:11', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(273, 102, '2018-12-31 21:29:39', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(274, 102, '2018-12-31 21:45:04', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(275, 102, '2018-12-31 21:45:14', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(276, 103, '2018-12-31 21:45:31', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(277, 102, '2018-12-31 21:45:33', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(278, 102, '2018-12-31 21:46:13', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(279, 102, '2018-12-31 21:46:53', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(280, 103, '2018-12-31 21:48:32', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(281, 104, '2018-12-31 21:48:34', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(282, 102, '2018-12-31 21:50:06', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(283, 104, '2018-12-31 21:50:09', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(284, 102, '2018-12-31 21:50:16', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(285, 102, '2018-12-31 21:50:36', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(286, 102, '2018-12-31 21:51:52', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(287, 104, '2018-12-31 21:51:55', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(288, 102, '2018-12-31 21:54:50', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(289, 104, '2018-12-31 21:54:52', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(290, 104, '2018-12-31 21:57:45', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(291, 102, '2018-12-31 21:57:46', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(292, 102, '2018-12-31 21:57:57', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(293, 102, '2018-12-31 22:00:58', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(294, 104, '2018-12-31 22:00:59', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(295, 102, '2018-12-31 22:08:19', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(296, 104, '2018-12-31 22:08:29', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(297, 104, '2018-12-31 22:08:30', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(298, 103, '2018-12-31 22:08:47', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(299, 104, '2018-12-31 22:08:48', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(300, 102, '2018-12-31 22:08:51', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(301, 102, '2018-12-31 23:04:51', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(302, 102, '2018-12-31 23:06:24', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(303, 102, '2018-12-31 23:29:47', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(304, 102, '2018-12-31 23:47:07', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(305, 102, '2018-12-31 23:47:35', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(306, 102, '2018-12-31 23:55:56', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(307, 102, '2018-12-31 23:56:33', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(308, 102, '2018-12-31 23:57:00', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(309, 102, '2019-01-01 00:06:33', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(310, 102, '2019-01-01 00:08:06', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(311, 102, '2019-01-01 00:10:00', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(312, 104, '2019-01-01 00:10:40', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(313, 104, '2019-01-01 00:11:49', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(314, 102, '2019-01-01 00:15:40', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(315, 104, '2019-01-01 00:16:29', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(316, 102, '2019-01-01 00:17:10', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(317, 102, '2019-01-01 00:17:44', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(318, 102, '2019-01-01 00:19:53', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(319, 102, '2019-01-01 00:20:01', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(320, 102, '2019-01-01 09:08:47', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(321, 102, '2019-01-01 09:09:39', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(322, 104, '2019-01-01 09:09:41', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(323, 102, '2019-01-01 09:11:21', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(324, 102, '2019-01-01 09:11:40', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(325, 104, '2019-01-01 09:11:41', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(326, 104, '2019-01-01 09:13:21', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(327, 102, '2019-01-01 09:13:22', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(328, 104, '2019-01-01 09:15:50', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(329, 102, '2019-01-01 09:15:51', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(330, 104, '2019-01-01 09:16:33', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(331, 102, '2019-01-01 09:16:34', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(332, 102, '2019-01-01 09:17:10', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(333, 102, '2019-01-01 09:18:02', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(334, 102, '2019-01-01 09:18:30', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(335, 102, '2019-01-01 09:20:16', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(336, 102, '2019-01-01 09:21:44', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(337, 102, '2019-01-01 09:22:42', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(338, 102, '2019-01-01 09:23:04', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(339, 102, '2019-01-01 09:23:25', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(340, 102, '2019-01-01 09:24:38', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(341, 102, '2019-01-01 09:26:51', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(342, 102, '2019-01-01 09:28:32', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(343, 104, '2019-01-01 09:29:22', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(344, 102, '2019-01-01 09:29:23', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(345, 104, '2019-01-01 09:30:10', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(346, 102, '2019-01-01 09:30:11', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(347, 102, '2019-01-01 09:30:55', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(348, 104, '2019-01-01 09:30:56', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(349, 102, '2019-01-01 11:11:46', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(350, 102, '2019-01-01 11:11:56', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(351, 102, '2019-01-01 11:54:38', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(352, 102, '2019-01-01 11:56:09', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(353, 102, '2019-01-01 11:58:21', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(354, 102, '2019-01-01 11:59:25', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(355, 102, '2019-01-01 12:35:04', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(356, 102, '2019-01-01 12:35:31', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(357, 102, '2019-01-01 12:36:25', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(358, 104, '2019-01-01 12:36:54', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(359, 102, '2019-01-01 12:36:56', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(360, 102, '2019-01-01 12:39:05', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(361, 104, '2019-01-01 12:39:14', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(362, 102, '2019-01-01 12:40:19', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(363, 104, '2019-01-01 12:40:57', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(364, 102, '2019-01-01 12:41:09', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(365, 102, '2019-01-01 12:43:39', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(366, 102, '2019-01-01 12:48:31', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(367, 102, '2019-01-01 12:49:56', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(368, 102, '2019-01-01 12:50:52', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(369, 102, '2019-01-01 12:51:24', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(370, 102, '2019-01-01 12:52:37', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(371, 102, '2019-01-01 12:53:01', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(372, 102, '2019-01-01 13:00:34', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(373, 102, '2019-01-01 13:02:01', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(374, 102, '2019-01-01 13:03:08', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(375, 102, '2019-01-01 13:04:26', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(376, 102, '2019-01-01 13:04:50', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(377, 104, '2019-01-01 13:04:54', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(378, 102, '2019-01-01 13:05:33', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(379, 102, '2019-01-01 13:36:47', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(380, 102, '2019-01-01 13:38:03', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(381, 102, '2019-01-01 13:39:25', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(382, 102, '2019-01-01 13:42:48', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(383, 102, '2019-01-01 13:44:28', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(384, 102, '2019-01-01 13:46:23', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(385, 102, '2019-01-01 13:49:35', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(386, 102, '2019-01-01 13:54:42', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(387, 102, '2019-01-01 13:58:22', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(388, 102, '2019-01-01 13:58:35', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(389, 102, '2019-01-01 13:59:28', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(390, 102, '2019-01-01 14:00:03', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(391, 102, '2019-01-01 14:21:12', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(392, 102, '2019-01-01 14:22:00', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(393, 102, '2019-01-01 14:48:49', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(394, 102, '2019-01-01 14:49:26', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(395, 104, '2019-01-01 14:49:54', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(396, 102, '2019-01-01 14:50:01', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(397, 102, '2019-01-01 14:51:07', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(398, 102, '2019-01-01 14:58:37', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(399, 102, '2019-01-01 14:58:58', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(400, 102, '2019-01-01 15:00:26', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(401, 102, '2019-01-01 17:18:29', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(402, 102, '2019-01-01 17:19:00', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(403, 102, '2019-01-01 17:21:22', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(404, 102, '2019-01-01 17:27:02', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(405, 102, '2019-01-01 17:27:20', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(406, 102, '2019-01-01 18:04:51', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(407, 103, '2019-01-01 18:39:35', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(408, 103, '2019-01-01 21:13:17', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(409, 103, '2019-01-01 21:13:50', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(410, 103, '2019-01-01 21:16:20', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(411, 103, '2019-01-01 21:32:37', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(412, 103, '2019-01-01 21:43:31', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(413, 104, '2019-01-01 21:44:11', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(414, 103, '2019-01-01 21:46:46', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(415, 104, '2019-01-01 21:47:47', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(416, 104, '2019-01-01 22:04:27', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(417, 103, '2019-01-01 22:04:40', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(418, 103, '2019-01-06 09:01:53', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(419, 103, '2019-01-06 19:32:03', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(420, 103, '2019-01-06 19:33:42', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(421, 103, '2019-01-06 19:41:56', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(422, 103, '2019-01-06 19:43:45', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(423, 103, '2019-01-06 19:54:13', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(424, 103, '2019-01-06 19:57:06', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(425, 103, '2019-01-06 20:08:25', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(426, 103, '2019-01-06 20:11:09', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(427, 103, '2019-01-06 20:22:45', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(428, 103, '2019-01-06 20:25:33', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(429, 103, '2019-01-06 20:26:33', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(430, 103, '2019-01-06 20:30:34', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(431, 103, '2019-01-06 20:30:58', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(432, 103, '2019-01-06 20:31:57', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(433, 103, '2019-01-06 20:33:06', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(434, 103, '2019-01-06 20:34:09', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(435, 103, '2019-01-06 20:35:05', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(436, 103, '2019-01-06 20:38:03', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(437, 103, '2019-01-06 20:39:01', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(438, 103, '2019-01-06 20:43:03', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(439, 103, '2019-01-06 20:44:01', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(440, 103, '2019-01-06 20:45:25', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(441, 103, '2019-01-06 20:46:09', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(442, 103, '2019-01-06 20:48:37', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(443, 103, '2019-01-06 20:50:07', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(444, 103, '2019-01-06 20:51:56', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(445, 103, '2019-01-06 20:53:30', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(446, 103, '2019-01-06 20:54:23', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(447, 103, '2019-01-12 12:42:15', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(448, 103, '2019-01-12 12:45:24', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(449, 103, '2019-01-12 12:46:28', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(450, 103, '2019-01-12 12:46:58', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(451, 103, '2019-01-12 12:48:13', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(452, 103, '2019-01-12 12:48:36', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(453, 103, '2019-01-12 12:51:15', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(454, 103, '2019-01-12 12:52:11', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(455, 103, '2019-01-12 12:54:14', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(456, 103, '2019-01-12 12:55:50', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(457, 103, '2019-01-12 13:00:09', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(458, 103, '2019-01-12 13:01:01', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(459, 103, '2019-01-12 13:04:17', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(460, 103, '2019-01-12 13:08:35', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(461, 103, '2019-01-12 13:12:00', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(462, 103, '2019-01-12 13:20:01', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(463, 103, '2019-01-12 13:21:30', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(464, 103, '2019-01-12 13:23:22', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(465, 103, '2019-01-12 13:25:25', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(466, 103, '2019-01-12 13:28:14', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(467, 103, '2019-01-12 13:29:12', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(468, 103, '2019-01-12 13:30:16', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(469, 103, '2019-01-12 13:31:52', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(470, 103, '2019-01-12 13:34:15', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(471, 103, '2019-01-12 13:35:11', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(472, 103, '2019-01-12 13:37:57', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(473, 103, '2019-01-12 13:43:20', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(474, 103, '2019-01-12 13:45:44', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(475, 103, '2019-01-12 13:47:10', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(476, 103, '2019-01-12 13:47:32', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(477, 103, '2019-01-12 13:47:50', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(478, 103, '2019-01-12 13:48:50', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(479, 103, '2019-01-12 13:49:22', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(480, 103, '2019-01-12 13:58:49', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(481, 103, '2019-01-12 13:59:43', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(482, 103, '2019-01-12 14:05:02', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(483, 104, '2019-01-12 14:05:03', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(484, 103, '2019-01-12 14:21:26', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(485, 103, '2019-01-12 14:24:54', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(486, 103, '2019-01-12 14:39:35', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(487, 104, '2019-01-12 14:39:36', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(488, 103, '2019-01-12 14:42:46', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(489, 104, '2019-01-12 14:42:49', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(490, 103, '2019-01-12 14:43:42', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(491, 104, '2019-01-12 14:43:51', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(492, 103, '2019-01-12 14:45:55', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(493, 104, '2019-01-12 14:45:57', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(494, 104, '2019-01-12 14:46:38', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(495, 103, '2019-01-12 14:47:37', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(496, 103, '2019-01-12 14:48:34', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(497, 103, '2019-01-12 14:56:34', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(498, 103, '2019-01-12 14:57:14', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(499, 103, '2019-01-12 14:57:52', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(500, 103, '2019-01-12 15:14:39', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(501, 103, '2019-01-12 15:15:13', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(502, 103, '2019-01-12 15:15:43', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(503, 103, '2019-01-12 15:16:33', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(504, 103, '2019-01-12 15:17:56', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(505, 103, '2019-01-12 15:19:40', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(506, 103, '2019-01-12 15:21:44', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(507, 103, '2019-01-12 15:24:37', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(508, 103, '2019-01-12 15:32:29', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(509, 103, '2019-01-12 15:32:58', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(510, 103, '2019-01-12 15:35:35', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(511, 103, '2019-01-12 15:39:07', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(512, 103, '2019-01-12 15:39:30', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(513, 103, '2019-01-12 15:44:18', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(514, 103, '2019-01-12 15:51:51', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(515, 103, '2019-01-12 15:51:57', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(516, 103, '2019-01-12 16:09:27', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(517, 103, '2019-01-12 16:10:49', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(518, 103, '2019-01-12 16:11:11', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(519, 103, '2019-01-12 16:11:48', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(520, 103, '2019-01-12 16:13:05', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(521, 103, '2019-01-12 16:13:35', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(522, 103, '2019-01-12 16:15:06', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(523, 103, '2019-01-12 16:17:11', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(524, 103, '2019-01-12 16:18:01', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(525, 103, '2019-01-12 16:18:44', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(526, 103, '2019-01-12 16:19:11', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(527, 103, '2019-01-12 16:19:39', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(528, 103, '2019-01-12 16:20:08', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(529, 103, '2019-01-12 16:20:24', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(530, 103, '2019-01-12 16:21:15', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(531, 103, '2019-01-12 16:21:58', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(532, 103, '2019-01-12 16:22:17', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(533, 103, '2019-01-12 16:22:36', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(534, 103, '2019-01-12 16:23:57', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(535, 103, '2019-01-12 16:24:56', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(536, 103, '2019-01-12 16:25:11', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(537, 104, '2019-01-12 16:26:01', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(538, 103, '2019-01-12 16:27:29', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(539, 103, '2019-01-12 17:19:22', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(540, 103, '2019-01-12 17:20:32', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(541, 103, '2019-01-12 17:21:14', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(542, 103, '2019-01-12 18:01:08', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(543, 103, '2019-01-12 18:01:09', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(544, 103, '2019-01-12 18:01:09', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(545, 103, '2019-01-12 18:01:14', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(546, 103, '2019-01-12 18:01:32', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(547, 103, '2019-01-12 18:02:23', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(548, 103, '2019-01-12 18:13:22', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(549, 103, '2019-01-12 18:14:49', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(550, 103, '2019-01-12 18:19:20', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(551, 103, '2019-01-12 18:19:54', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(552, 103, '2019-01-12 18:21:05', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(553, 103, '2019-01-12 18:24:29', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(554, 103, '2019-01-12 18:25:36', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(555, 103, '2019-01-12 18:26:03', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(556, 103, '2019-01-12 18:29:56', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(557, 103, '2019-01-12 18:31:26', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(558, 103, '2019-01-12 18:31:32', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(559, 103, '2019-01-12 18:38:52', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(560, 103, '2019-01-12 18:50:25', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(561, 103, '2019-01-12 19:42:50', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(562, 103, '2019-01-12 19:43:27', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(563, 103, '2019-01-12 19:43:38', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(564, 103, '2019-01-12 19:44:02', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(565, 103, '2019-01-12 19:44:36', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(566, 103, '2019-01-12 19:45:02', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(567, 103, '2019-01-12 19:46:13', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(568, 103, '2019-01-12 19:46:27', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(569, 103, '2019-01-12 19:47:29', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(570, 103, '2019-01-12 19:48:08', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(571, 103, '2019-01-12 19:48:27', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(572, 103, '2019-01-12 19:48:55', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(573, 103, '2019-01-12 19:49:52', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(574, 103, '2019-01-12 19:50:06', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231');
INSERT INTO `userslog` (`id`, `userid`, `datetime`, `action`, `result`, `useragent`, `host`) VALUES
(575, 103, '2019-01-12 19:50:20', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(576, 103, '2019-01-12 19:50:47', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(577, 103, '2019-01-12 19:51:57', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(578, 103, '2019-01-12 19:54:21', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(579, 103, '2019-01-12 19:55:19', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(580, 103, '2019-01-12 19:55:54', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(581, 103, '2019-01-12 19:56:28', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(582, 103, '2019-01-12 19:56:54', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(583, 103, '2019-01-12 19:57:27', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(584, 103, '2019-01-12 19:58:15', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(585, 103, '2019-01-12 19:58:26', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(586, 104, '2019-01-12 19:58:53', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(587, 103, '2019-01-12 20:08:46', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(588, 103, '2019-01-12 20:09:57', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(589, 103, '2019-01-12 20:10:50', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(590, 103, '2019-01-12 20:13:54', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(591, 103, '2019-01-12 20:17:01', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(592, 103, '2019-01-12 20:21:15', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(593, 103, '2019-01-12 20:22:46', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(594, 103, '2019-01-12 20:29:37', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(595, 103, '2019-01-12 20:30:37', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(596, 103, '2019-01-12 20:31:08', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(597, 103, '2019-01-12 21:17:44', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(598, 104, '2019-01-12 21:45:49', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(599, 104, '2019-01-12 21:47:21', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(600, 103, '2019-01-13 07:23:46', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(601, 103, '2019-01-13 07:24:06', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(602, 103, '2019-01-13 07:25:31', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(603, 103, '2019-01-13 08:05:55', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(604, 103, '2019-01-13 08:06:36', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(605, 103, '2019-01-13 08:07:10', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(606, 103, '2019-01-13 09:01:53', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(607, 102, '2019-01-13 09:04:10', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(608, 102, '2019-01-13 09:05:40', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(609, 102, '2019-01-13 09:07:51', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(610, 102, '2019-01-13 09:08:28', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(611, 102, '2019-01-13 09:14:08', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(612, 102, '2019-01-13 09:15:11', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(613, 102, '2019-01-13 09:15:51', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(614, 102, '2019-01-13 09:16:29', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(615, 102, '2019-01-13 09:17:27', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(616, 102, '2019-01-13 09:18:35', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(617, 102, '2019-01-13 09:19:21', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(618, 102, '2019-01-13 09:30:40', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(619, 102, '2019-01-13 09:48:22', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(620, 102, '2019-01-13 09:52:05', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(621, 102, '2019-01-13 09:53:52', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(622, 102, '2019-01-13 10:00:39', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(623, 102, '2019-01-13 10:04:38', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(624, 102, '2019-01-13 10:06:29', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(625, 102, '2019-01-13 10:07:27', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(626, 102, '2019-01-13 10:09:05', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(627, 102, '2019-01-13 10:10:21', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(628, 102, '2019-01-13 10:11:46', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(629, 102, '2019-01-13 10:13:55', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(630, 102, '2019-01-13 10:14:52', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(631, 102, '2019-01-13 10:16:05', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(632, 102, '2019-01-13 10:24:36', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(633, 102, '2019-01-13 10:25:16', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(634, 102, '2019-01-13 10:26:22', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(635, 102, '2019-01-13 10:27:43', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(636, 102, '2019-01-13 10:31:51', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(637, 104, '2019-01-13 10:44:06', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(638, 102, '2019-01-13 10:45:48', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(639, 102, '2019-01-13 10:46:44', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(640, 102, '2019-01-13 10:47:19', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(641, 102, '2019-01-13 10:48:02', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(642, 102, '2019-01-13 10:48:48', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(643, 102, '2019-01-13 10:49:39', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(644, 102, '2019-01-13 10:50:09', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(645, 104, '2019-01-13 10:50:11', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(646, 102, '2019-01-13 10:52:27', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(647, 102, '2019-01-13 10:53:11', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(648, 102, '2019-01-13 10:54:32', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(649, 101, '2019-01-13 10:59:47', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(650, 101, '2019-01-13 11:11:46', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(651, 101, '2019-01-13 11:12:04', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(652, 101, '2019-01-13 11:24:06', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(653, 104, '2019-01-13 11:24:52', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(654, 101, '2019-01-13 11:28:40', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(655, 101, '2019-01-13 11:29:46', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(656, 101, '2019-01-13 11:30:02', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(657, 101, '2019-01-13 11:31:17', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(658, 101, '2019-01-13 11:31:54', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(659, 101, '2019-01-13 11:32:44', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(660, 101, '2019-01-13 11:33:08', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(661, 101, '2019-01-13 11:35:14', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(662, 101, '2019-01-13 11:35:36', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(663, 101, '2019-01-13 11:36:26', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(664, 101, '2019-01-13 11:36:59', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(665, 101, '2019-01-13 11:38:06', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(666, 101, '2019-01-13 11:38:41', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(667, 101, '2019-01-13 11:39:40', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(668, 101, '2019-01-13 11:46:38', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(669, 101, '2019-01-13 11:47:59', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(670, 101, '2019-01-13 11:48:47', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(671, 101, '2019-01-13 11:49:47', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(672, 101, '2019-01-13 11:50:25', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(673, 101, '2019-01-13 11:51:18', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(674, 101, '2019-01-13 11:51:33', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(675, 101, '2019-01-13 11:51:58', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(676, 101, '2019-01-13 11:52:19', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(677, 101, '2019-01-13 13:17:43', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(678, 101, '2019-01-13 13:19:22', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(679, 101, '2019-01-13 13:19:58', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(680, 101, '2019-01-13 13:20:25', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(681, 101, '2019-01-13 13:21:49', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(682, 101, '2019-01-13 13:23:16', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(683, 101, '2019-01-13 13:31:53', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(684, 101, '2019-01-13 13:32:45', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(685, 101, '2019-01-13 13:33:47', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(686, 104, '2019-01-13 13:34:25', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(687, 101, '2019-01-13 13:35:40', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(688, 101, '2019-01-13 13:38:40', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(689, 101, '2019-01-13 13:40:47', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(690, 101, '2019-01-13 13:40:59', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(691, 101, '2019-01-13 13:42:09', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(692, 101, '2019-01-13 13:43:15', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(693, 101, '2019-01-13 13:43:32', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(694, 101, '2019-01-13 13:44:39', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(695, 101, '2019-01-13 13:45:06', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(696, 101, '2019-01-13 13:45:29', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(697, 101, '2019-01-13 13:45:37', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(698, 101, '2019-01-13 13:46:16', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(699, 101, '2019-01-13 13:46:36', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(700, 101, '2019-01-13 13:48:01', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(701, 101, '2019-01-13 14:06:27', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(702, 101, '2019-01-13 14:07:04', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(703, 104, '2019-01-13 14:07:06', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(704, 101, '2019-01-13 14:11:17', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(705, 101, '2019-01-13 14:12:30', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(706, 104, '2019-01-13 14:13:10', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(707, 104, '2019-01-13 14:14:19', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(708, 101, '2019-01-13 14:19:32', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(709, 101, '2019-01-13 14:42:41', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(710, 101, '2019-01-13 14:44:01', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(711, 101, '2019-01-13 14:44:57', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(712, 101, '2019-01-13 14:46:46', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(713, 101, '2019-01-13 14:47:33', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(714, 101, '2019-01-13 14:48:07', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(715, 101, '2019-01-13 14:54:03', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(716, 101, '2019-01-13 14:54:48', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(717, 104, '2019-01-19 14:49:23', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(718, 101, '2019-02-15 23:56:01', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(719, 101, '2019-02-15 23:57:13', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(720, 104, '2019-02-16 08:01:36', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(721, 104, '2019-02-16 22:34:58', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(722, 104, '2019-02-17 12:03:39', 'LogIn', 1, 'websocket-sharp/1.0', 'localhost:24231'),
(723, 101, '2019-02-17 13:24:32', 'LogIn', 1, 'websocket-sharp/1.0', '192.168.1.144:24231'),
(724, NULL, '2019-02-17 13:27:16', 'LogIn', 0, 'websocket-sharp/1.0', '192.168.1.144:24231'),
(725, NULL, '2019-02-17 13:27:24', 'LogIn', 0, 'websocket-sharp/1.0', '192.168.1.144:24231'),
(726, NULL, '2019-02-17 13:27:32', 'LogIn', 0, 'websocket-sharp/1.0', '192.168.1.144:24231'),
(727, NULL, '2019-02-17 13:27:32', 'LogIn', 0, 'websocket-sharp/1.0', '192.168.1.144:24231'),
(728, 102, '2019-02-17 13:27:57', 'LogIn', 1, 'websocket-sharp/1.0', '192.168.1.144:24231'),
(729, 101, '2019-02-17 13:28:39', 'LogIn', 1, 'websocket-sharp/1.0', '192.168.1.144:24231'),
(730, 104, '2019-02-17 13:29:59', 'LogIn', 1, 'websocket-sharp/1.0', '192.168.1.144:24231'),
(731, 101, '2019-02-17 13:37:53', 'LogIn', 1, 'websocket-sharp/1.0', '192.168.1.61:24231'),
(732, 101, '2019-02-17 13:39:34', 'LogIn', 1, 'websocket-sharp/1.0', '192.168.1.61:24231'),
(733, 101, '2019-02-17 13:41:01', 'LogIn', 1, 'websocket-sharp/1.0', '192.168.1.144:24231'),
(734, 101, '2019-03-28 16:29:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(735, 101, '2019-03-28 16:32:33', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(736, 101, '2019-03-28 16:33:48', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(737, 101, '2019-03-28 16:34:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(738, 101, '2019-03-28 16:38:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(739, 101, '2019-03-28 16:40:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(740, 101, '2019-03-28 16:42:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(741, 101, '2019-03-28 16:45:09', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(742, 101, '2019-03-28 16:46:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(743, 101, '2019-03-28 16:46:33', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(744, 101, '2019-03-28 16:57:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(745, 101, '2019-03-28 16:57:38', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(746, 101, '2019-03-28 16:58:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(747, 101, '2019-03-28 16:59:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(748, 101, '2019-03-28 16:59:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(749, 101, '2019-03-28 17:00:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(750, 101, '2019-03-28 17:00:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(751, 101, '2019-03-28 17:01:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(752, 101, '2019-03-28 17:01:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(753, 101, '2019-03-28 17:04:04', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(754, 101, '2019-03-28 17:04:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(755, 101, '2019-03-28 17:04:39', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(756, 101, '2019-03-28 17:36:13', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(757, 101, '2019-03-28 17:37:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(758, 101, '2019-03-28 17:42:04', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(759, 101, '2019-03-28 17:46:27', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(760, 101, '2019-03-28 18:00:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(761, 101, '2019-03-28 18:01:59', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(762, 101, '2019-03-28 18:03:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(763, 101, '2019-03-28 18:05:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(764, 101, '2019-03-28 18:06:33', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(765, 101, '2019-03-28 18:11:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(766, 101, '2019-03-28 18:14:30', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(767, 101, '2019-03-28 18:20:51', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(768, 101, '2019-03-28 18:25:59', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(769, 101, '2019-03-28 18:28:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(770, 101, '2019-03-28 18:39:11', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(771, 101, '2019-03-28 20:52:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(772, 101, '2019-03-28 20:56:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(773, 101, '2019-03-28 21:37:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(774, 101, '2019-03-28 21:39:01', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(775, 101, '2019-03-28 21:42:25', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(776, 101, '2019-03-28 21:51:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(777, 101, '2019-03-28 21:53:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(778, 101, '2019-03-28 21:54:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(779, 101, '2019-03-28 21:54:58', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(780, 101, '2019-03-29 13:38:33', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(781, 101, '2019-03-29 13:42:13', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(782, 101, '2019-03-29 14:00:51', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(783, 101, '2019-03-29 14:02:23', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(784, 101, '2019-03-29 14:03:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(785, 101, '2019-03-29 14:05:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(786, 101, '2019-03-29 14:11:13', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(787, 101, '2019-03-29 15:22:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(788, 101, '2019-03-29 15:23:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(789, 104, '2019-03-29 15:28:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(790, 101, '2019-03-29 15:28:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(791, 101, '2019-03-29 16:05:54', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(792, 101, '2019-03-29 16:07:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(793, 101, '2019-03-29 16:09:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(794, 101, '2019-03-29 16:10:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(795, 101, '2019-03-29 16:17:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(796, 101, '2019-03-29 16:21:47', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(797, 101, '2019-03-29 16:22:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(798, 101, '2019-03-29 16:34:55', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(799, 101, '2019-03-29 16:36:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(800, 101, '2019-03-29 16:38:23', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(801, 101, '2019-03-29 16:40:38', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(802, 101, '2019-03-29 16:41:47', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(803, 101, '2019-03-29 16:43:13', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(804, 104, '2019-03-29 16:43:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(805, 101, '2019-03-29 16:43:48', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(806, 104, '2019-03-29 16:46:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(807, 101, '2019-03-29 16:46:44', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(808, 104, '2019-03-29 18:14:21', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(809, 101, '2019-03-29 18:14:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(810, 101, '2019-03-29 18:14:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(811, 101, '2019-03-29 18:15:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(812, 101, '2019-03-29 18:16:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(813, 104, '2019-03-29 18:16:45', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(814, 101, '2019-03-29 18:17:11', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(815, 101, '2019-03-29 18:18:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(816, 101, '2019-03-29 18:21:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(817, 104, '2019-03-29 18:21:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(818, 101, '2019-03-29 18:21:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(819, 101, '2019-03-29 18:21:45', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(820, 101, '2019-03-29 18:22:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(821, 101, '2019-03-29 18:23:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(822, 101, '2019-03-29 18:24:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(823, 104, '2019-03-29 18:28:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(824, 101, '2019-03-29 18:28:34', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(825, 101, '2019-03-29 18:28:48', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(826, 101, '2019-03-29 20:05:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(827, 104, '2019-03-29 20:05:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(828, 101, '2019-03-29 20:11:38', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(829, 104, '2019-03-29 20:11:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(830, 104, '2019-03-29 20:15:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(831, 101, '2019-03-29 20:15:33', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(832, 101, '2019-03-29 20:22:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(833, 104, '2019-03-29 20:22:38', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(834, 101, '2019-03-29 20:30:52', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(835, 101, '2019-03-29 20:33:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(836, 101, '2019-03-29 20:34:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(837, 101, '2019-03-29 20:38:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(838, 101, '2019-03-29 20:44:16', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(839, 101, '2019-03-29 20:44:25', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(840, 101, '2019-03-29 20:45:01', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(841, 101, '2019-03-29 20:46:02', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(842, 101, '2019-03-29 20:46:16', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(843, 101, '2019-03-29 20:53:19', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(844, 101, '2019-03-29 21:07:44', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(845, 101, '2019-03-29 21:10:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(846, 104, '2019-03-29 21:10:58', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(847, 101, '2019-03-29 21:15:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(848, 104, '2019-03-29 21:15:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(849, 104, '2019-03-29 21:17:39', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(850, 101, '2019-03-29 21:17:48', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(851, 101, '2019-03-29 21:22:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(852, 101, '2019-03-30 09:34:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(853, 101, '2019-03-30 09:35:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(854, 101, '2019-03-30 09:37:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(855, 101, '2019-03-30 09:40:52', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(856, 101, '2019-03-30 09:43:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(857, 101, '2019-03-30 09:43:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(858, 101, '2019-03-30 09:45:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(859, 104, '2019-03-30 09:45:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(860, 101, '2019-03-30 09:46:47', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(861, 101, '2019-03-30 09:48:30', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(862, 101, '2019-03-30 09:50:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(863, 101, '2019-03-30 09:50:47', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(864, 101, '2019-03-30 09:51:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(865, 101, '2019-03-30 09:51:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(866, 101, '2019-03-30 09:54:20', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(867, 101, '2019-03-30 09:54:55', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(868, 101, '2019-03-30 09:55:51', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(869, 101, '2019-03-30 09:59:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(870, 101, '2019-03-30 09:59:58', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(871, 101, '2019-03-30 10:01:16', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(872, 101, '2019-03-30 10:01:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(873, 101, '2019-03-30 10:01:58', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(874, 101, '2019-03-30 10:03:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(875, 101, '2019-03-30 10:05:24', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(876, 101, '2019-03-30 10:05:55', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(877, 101, '2019-03-30 10:07:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(878, 101, '2019-03-30 10:07:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(879, 101, '2019-03-30 10:10:47', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(880, 101, '2019-03-30 10:13:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(881, 101, '2019-03-30 10:14:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(882, 101, '2019-03-30 10:15:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(883, 101, '2019-03-30 10:16:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(884, 101, '2019-03-30 10:19:38', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(885, 101, '2019-03-30 10:29:54', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(886, 101, '2019-03-30 10:31:45', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(887, 101, '2019-03-30 10:40:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(888, 101, '2019-03-30 10:45:30', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(889, 101, '2019-03-30 10:48:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(890, 101, '2019-03-30 10:49:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(891, 101, '2019-03-30 10:51:29', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(892, 101, '2019-03-30 10:52:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(893, 101, '2019-03-30 10:56:07', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(894, 101, '2019-03-30 11:00:51', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(895, 101, '2019-03-30 11:01:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(896, 101, '2019-03-30 11:09:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(897, 101, '2019-03-30 11:11:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(898, 101, '2019-03-30 11:12:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(899, 101, '2019-03-30 11:14:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(900, 101, '2019-03-30 11:15:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(901, 101, '2019-03-30 11:15:34', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(902, 101, '2019-03-30 11:16:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(903, 101, '2019-03-30 11:19:25', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(904, 101, '2019-03-30 11:24:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(905, 101, '2019-03-30 11:26:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(906, 101, '2019-03-30 11:27:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(907, 101, '2019-03-30 11:27:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(908, 101, '2019-03-30 16:32:04', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(909, 101, '2019-03-30 16:37:51', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(910, 101, '2019-03-30 16:51:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(911, 101, '2019-03-30 20:05:51', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(912, 104, '2019-03-30 20:05:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(913, 101, '2019-04-06 19:09:11', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(914, 101, '2019-04-06 19:09:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(915, 102, '2019-04-06 19:10:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(916, 102, '2019-04-06 19:12:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(917, 102, '2019-04-06 19:12:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(918, 102, '2019-04-06 19:13:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(919, 102, '2019-04-06 19:15:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(920, 102, '2019-04-06 19:16:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(921, 102, '2019-04-06 19:17:39', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(922, 102, '2019-04-06 19:18:26', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(923, 102, '2019-04-06 19:18:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(924, 102, '2019-04-06 19:21:09', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(925, 102, '2019-04-06 19:21:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(926, 102, '2019-04-06 19:23:30', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(927, 102, '2019-04-06 19:23:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(928, 102, '2019-04-06 19:25:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(929, 102, '2019-04-06 19:27:54', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(930, 102, '2019-04-06 21:05:23', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(931, 102, '2019-04-06 21:06:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(932, 102, '2019-04-10 21:02:02', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(933, 102, '2019-04-13 10:39:13', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(934, 102, '2019-04-13 10:43:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(935, 102, '2019-04-13 10:44:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(936, 102, '2019-04-13 10:49:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(937, 102, '2019-04-13 10:49:55', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(938, 102, '2019-04-13 10:51:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(939, 102, '2019-04-13 10:51:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(940, 102, '2019-04-13 10:52:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(941, 102, '2019-04-13 10:56:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(942, 102, '2019-04-13 10:57:04', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(943, 102, '2019-04-13 10:57:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(944, 102, '2019-04-13 10:59:19', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(945, 102, '2019-04-13 10:59:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(946, 102, '2019-04-13 11:03:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(947, 102, '2019-04-13 11:03:27', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(948, 102, '2019-04-13 11:03:38', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(949, 102, '2019-04-13 11:04:19', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(950, 102, '2019-04-13 11:05:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(951, 102, '2019-04-13 11:06:13', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(952, 102, '2019-04-13 11:06:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(953, 102, '2019-04-13 11:07:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(954, 102, '2019-04-13 11:08:27', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(955, 102, '2019-04-13 11:08:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(956, 102, '2019-04-13 11:12:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(957, 102, '2019-04-13 11:12:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(958, 102, '2019-04-13 11:12:06', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(959, 102, '2019-04-13 11:12:07', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(960, 102, '2019-04-13 11:12:07', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(961, 102, '2019-04-13 11:12:07', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(962, 102, '2019-04-13 11:12:07', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(963, 102, '2019-04-13 11:18:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(964, 102, '2019-04-13 11:19:24', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(965, 102, '2019-04-13 11:20:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(966, 102, '2019-04-13 11:21:20', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(967, 102, '2019-04-13 11:41:20', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(968, 102, '2019-04-13 11:52:16', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(969, 102, '2019-04-13 11:55:21', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(970, 102, '2019-04-13 11:55:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(971, 102, '2019-04-13 11:57:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(972, 102, '2019-04-13 12:00:59', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(973, 102, '2019-04-13 12:40:04', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(974, 104, '2019-04-13 12:41:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(975, 102, '2019-04-13 12:52:25', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(976, 102, '2019-04-13 12:53:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(977, 102, '2019-04-13 12:57:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(978, 102, '2019-04-13 13:01:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(979, 102, '2019-04-13 13:08:09', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(980, 102, '2019-04-13 13:08:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(981, 102, '2019-04-13 13:29:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(982, 102, '2019-04-13 13:31:02', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(983, 102, '2019-04-13 13:34:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(984, 102, '2019-04-13 13:40:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(985, 102, '2019-04-13 13:42:26', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(986, 104, '2019-04-13 13:45:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(987, 102, '2019-04-13 13:47:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(988, 102, '2019-04-13 13:54:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(989, 102, '2019-04-13 13:54:21', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(990, 102, '2019-04-13 13:54:38', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(991, 102, '2019-04-13 14:24:48', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(992, 102, '2019-04-13 14:27:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(993, 102, '2019-04-13 14:28:55', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(994, 102, '2019-04-13 14:29:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(995, 102, '2019-04-13 14:31:30', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(996, 102, '2019-04-13 14:35:13', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(997, 102, '2019-04-13 14:35:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(998, 102, '2019-04-13 14:40:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(999, 102, '2019-04-13 14:41:55', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1000, 102, '2019-04-13 14:42:44', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1001, 102, '2019-04-13 14:49:54', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1002, 102, '2019-04-13 14:50:21', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1003, 102, '2019-04-13 14:50:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1004, 102, '2019-04-13 14:52:47', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1005, 102, '2019-04-13 14:53:33', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1006, 102, '2019-04-13 14:54:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1007, 102, '2019-04-13 14:57:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1008, 102, '2019-04-13 14:57:54', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1009, 102, '2019-04-13 14:58:59', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1010, 102, '2019-04-13 15:01:27', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1011, 102, '2019-04-13 15:01:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1012, 102, '2019-04-13 15:03:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1013, 102, '2019-04-13 15:04:13', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1014, 102, '2019-04-13 15:06:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1015, 102, '2019-04-13 15:19:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1016, 102, '2019-04-13 15:23:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1017, 102, '2019-04-13 15:23:44', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1018, 102, '2019-04-13 15:27:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1019, 102, '2019-04-13 15:28:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1020, 102, '2019-04-13 15:30:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1021, 102, '2019-04-13 15:34:07', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1022, 102, '2019-04-13 15:35:21', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1023, 102, '2019-04-13 15:38:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1024, 102, '2019-04-13 15:56:39', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1025, 102, '2019-04-13 15:59:24', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1026, 102, '2019-04-13 17:39:27', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1027, 102, '2019-04-13 18:09:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1028, 102, '2019-04-13 18:23:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1029, 102, '2019-04-13 18:23:55', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1030, 102, '2019-04-13 18:27:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1031, 102, '2019-04-13 19:01:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1032, 102, '2019-04-13 19:02:25', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1033, 102, '2019-04-13 19:02:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1034, 102, '2019-04-13 19:06:45', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1035, 102, '2019-04-13 19:07:39', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1036, 102, '2019-04-13 19:08:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1037, 102, '2019-04-13 19:09:21', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1038, 102, '2019-04-13 19:12:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1039, 102, '2019-04-13 19:16:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1040, 102, '2019-04-13 19:17:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1041, 102, '2019-04-13 19:18:19', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1042, 102, '2019-04-13 19:20:20', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1043, 102, '2019-04-13 19:21:30', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1044, 102, '2019-04-13 19:22:38', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1045, 102, '2019-04-13 19:23:04', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1046, 102, '2019-04-13 19:24:38', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1047, 102, '2019-04-13 19:25:59', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1048, 102, '2019-04-13 19:29:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1049, 102, '2019-04-13 19:30:02', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1050, 102, '2019-04-13 19:32:13', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1051, 102, '2019-04-13 19:36:09', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1052, 102, '2019-04-13 19:38:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1053, 102, '2019-04-13 19:39:07', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1054, 102, '2019-04-13 19:40:51', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1055, 102, '2019-04-13 19:41:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1056, 102, '2019-04-13 19:41:24', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1057, 102, '2019-04-13 19:43:06', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1058, 102, '2019-04-13 19:46:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1059, 102, '2019-04-13 19:52:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1060, 102, '2019-04-13 19:53:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1061, 102, '2019-04-13 19:54:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1062, 102, '2019-04-13 19:57:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1063, 102, '2019-04-13 19:58:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1064, 102, '2019-04-13 20:00:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1065, 102, '2019-04-13 20:04:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1066, 102, '2019-04-13 20:06:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1067, 102, '2019-04-13 20:09:06', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1068, 102, '2019-04-13 20:11:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1069, 102, '2019-04-13 20:18:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1070, 102, '2019-04-13 20:21:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1071, 102, '2019-04-13 20:23:58', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1072, 102, '2019-04-13 20:24:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1073, 102, '2019-04-13 20:25:24', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1074, 102, '2019-04-13 20:26:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1075, 102, '2019-04-13 20:27:44', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1076, 102, '2019-04-13 20:33:33', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1077, 102, '2019-04-13 20:36:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1078, 102, '2019-04-13 20:39:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1079, 102, '2019-04-13 20:41:33', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1080, 102, '2019-04-13 20:42:58', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1081, 102, '2019-04-13 20:47:39', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1082, 102, '2019-04-13 20:50:25', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1083, 102, '2019-04-13 20:52:38', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1084, 102, '2019-04-13 21:06:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1085, 102, '2019-04-13 21:11:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1086, 102, '2019-04-13 21:13:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1087, 102, '2019-04-13 21:32:24', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1088, 102, '2019-04-13 21:32:52', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1089, 102, '2019-04-13 21:37:19', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1090, 102, '2019-04-13 21:40:52', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1091, 102, '2019-04-13 21:42:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1092, 102, '2019-04-13 21:43:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1093, 102, '2019-04-13 21:47:23', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1094, 102, '2019-04-13 21:50:09', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1095, 102, '2019-04-13 21:53:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1096, 102, '2019-04-13 21:55:24', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1097, 102, '2019-04-13 21:56:51', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1098, 102, '2019-04-13 21:58:04', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1099, 102, '2019-04-13 21:59:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1100, 102, '2019-04-13 21:59:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1101, 102, '2019-04-13 22:02:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1102, 102, '2019-04-13 22:03:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1103, 102, '2019-04-13 22:03:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1104, 102, '2019-04-13 22:05:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1105, 102, '2019-04-13 22:06:39', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1106, 102, '2019-04-14 10:36:39', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1107, 104, '2019-04-14 10:37:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1108, 102, '2019-04-14 10:38:16', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1109, 102, '2019-04-14 10:56:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1110, 103, '2019-04-14 10:57:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1111, 102, '2019-04-14 11:01:16', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1112, 103, '2019-04-14 11:01:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1113, 102, '2019-04-14 11:19:48', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1114, 102, '2019-04-14 11:22:45', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1115, 102, '2019-04-14 11:24:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1116, 102, '2019-04-14 11:26:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1117, 102, '2019-04-14 11:27:07', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1118, 102, '2019-04-14 11:36:07', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1119, 102, '2019-04-14 11:38:27', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1120, 102, '2019-04-14 11:39:30', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1121, 102, '2019-04-14 11:48:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1122, 102, '2019-04-14 11:59:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1123, 102, '2019-04-14 12:04:09', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1124, 102, '2019-04-14 12:47:59', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1125, 102, '2019-04-14 12:49:11', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1126, 102, '2019-04-14 12:51:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1127, 102, '2019-04-14 12:57:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1128, 102, '2019-04-14 12:58:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1129, 102, '2019-04-14 13:02:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1130, 102, '2019-04-14 13:05:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1131, 102, '2019-04-14 13:10:58', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1132, 102, '2019-04-14 13:12:19', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1133, 102, '2019-04-14 13:13:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1134, 102, '2019-04-14 13:15:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1135, 102, '2019-04-14 15:27:20', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1136, 103, '2019-04-14 15:27:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1137, 102, '2019-04-14 15:31:33', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1138, 103, '2019-04-14 15:31:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1139, 100, '2019-04-14 15:42:04', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1140, 100, '2019-04-14 15:55:06', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1141, 100, '2019-04-14 15:56:24', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1142, 100, '2019-04-14 15:58:26', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1143, 102, '2019-04-14 16:04:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1144, 102, '2019-04-14 16:08:11', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1145, 102, '2019-04-14 16:08:45', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231');
INSERT INTO `userslog` (`id`, `userid`, `datetime`, `action`, `result`, `useragent`, `host`) VALUES
(1146, 102, '2019-04-14 16:09:52', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1147, 102, '2019-04-14 16:16:09', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1148, 102, '2019-04-14 16:16:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1149, 102, '2019-04-14 16:17:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1150, 102, '2019-04-14 16:23:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1151, 102, '2019-04-14 17:20:13', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1152, 102, '2019-04-14 17:45:04', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1153, 102, '2019-04-14 17:47:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1154, 102, '2019-04-14 17:52:19', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1155, 102, '2019-04-14 18:27:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1156, 102, '2019-04-14 18:28:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1157, 102, '2019-04-14 18:29:54', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1158, 102, '2019-04-14 18:31:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1159, 102, '2019-04-14 18:33:02', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1160, 102, '2019-04-14 18:34:09', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1161, 102, '2019-04-14 18:37:48', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231');

--
-- Indeksy dla zrzutów tabel
--

--
-- Indeksy dla tabeli `ammunitions`
--
ALTER TABLE `ammunitions`
  ADD PRIMARY KEY (`ammunitionid`);

--
-- Indeksy dla tabeli `enemies`
--
ALTER TABLE `enemies`
  ADD PRIMARY KEY (`enemyid`),
  ADD KEY `rewardid` (`rewardid`),
  ADD KEY `prefabname` (`prefabid`);

--
-- Indeksy dla tabeli `enemymap`
--
ALTER TABLE `enemymap`
  ADD PRIMARY KEY (`id`),
  ADD KEY `enemyid` (`enemyid`),
  ADD KEY `mapid` (`mapid`);

--
-- Indeksy dla tabeli `gamesettings`
--
ALTER TABLE `gamesettings`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `key` (`key`);

--
-- Indeksy dla tabeli `maps`
--
ALTER TABLE `maps`
  ADD PRIMARY KEY (`mapid`);

--
-- Indeksy dla tabeli `pilotresources`
--
ALTER TABLE `pilotresources`
  ADD UNIQUE KEY `userid_2` (`userid`),
  ADD KEY `userid` (`userid`);

--
-- Indeksy dla tabeli `pilots`
--
ALTER TABLE `pilots`
  ADD UNIQUE KEY `userid` (`userid`),
  ADD UNIQUE KEY `nickname` (`nickname`),
  ADD KEY `mapid` (`mapid`),
  ADD KEY `shipid` (`shipid`);

--
-- Indeksy dla tabeli `portalpositions`
--
ALTER TABLE `portalpositions`
  ADD UNIQUE KEY `portalpositionid` (`portalpositionid`),
  ADD UNIQUE KEY `name` (`name`);

--
-- Indeksy dla tabeli `portals`
--
ALTER TABLE `portals`
  ADD PRIMARY KEY (`portalid`),
  ADD KEY `prefabid` (`prefabid`),
  ADD KEY `mapid` (`mapid`),
  ADD KEY `target_mapid` (`target_mapid`),
  ADD KEY `portalpositionid` (`portalpositionid`),
  ADD KEY `target_portalpositionid` (`target_portalpositionid`);

--
-- Indeksy dla tabeli `prefabs`
--
ALTER TABLE `prefabs`
  ADD PRIMARY KEY (`prefabid`),
  ADD UNIQUE KEY `prefabname` (`prefabname`),
  ADD KEY `prefab_type_id` (`prefabtypeid`);

--
-- Indeksy dla tabeli `prefabs_types`
--
ALTER TABLE `prefabs_types`
  ADD PRIMARY KEY (`prefabtypeid`);

--
-- Indeksy dla tabeli `rewards`
--
ALTER TABLE `rewards`
  ADD PRIMARY KEY (`rewardid`);

--
-- Indeksy dla tabeli `rockets`
--
ALTER TABLE `rockets`
  ADD PRIMARY KEY (`rocketid`);

--
-- Indeksy dla tabeli `ships`
--
ALTER TABLE `ships`
  ADD PRIMARY KEY (`shipid`),
  ADD UNIQUE KEY `shipname` (`shipname`),
  ADD KEY `rewardid` (`rewardid`),
  ADD KEY `prefabname` (`prefabid`);

--
-- Indeksy dla tabeli `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`userid`),
  ADD UNIQUE KEY `usernamehash` (`usernamehash`),
  ADD UNIQUE KEY `email` (`email`);

--
-- Indeksy dla tabeli `userslog`
--
ALTER TABLE `userslog`
  ADD PRIMARY KEY (`id`),
  ADD KEY `userid` (`userid`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT dla tabeli `ammunitions`
--
ALTER TABLE `ammunitions`
  MODIFY `ammunitionid` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=104;

--
-- AUTO_INCREMENT dla tabeli `enemies`
--
ALTER TABLE `enemies`
  MODIFY `enemyid` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT dla tabeli `enemymap`
--
ALTER TABLE `enemymap`
  MODIFY `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT dla tabeli `gamesettings`
--
ALTER TABLE `gamesettings`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT dla tabeli `maps`
--
ALTER TABLE `maps`
  MODIFY `mapid` int(11) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=117;

--
-- AUTO_INCREMENT dla tabeli `portalpositions`
--
ALTER TABLE `portalpositions`
  MODIFY `portalpositionid` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=122;

--
-- AUTO_INCREMENT dla tabeli `portals`
--
ALTER TABLE `portals`
  MODIFY `portalid` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=102;

--
-- AUTO_INCREMENT dla tabeli `prefabs`
--
ALTER TABLE `prefabs`
  MODIFY `prefabid` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=119;

--
-- AUTO_INCREMENT dla tabeli `prefabs_types`
--
ALTER TABLE `prefabs_types`
  MODIFY `prefabtypeid` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT dla tabeli `rewards`
--
ALTER TABLE `rewards`
  MODIFY `rewardid` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=31;

--
-- AUTO_INCREMENT dla tabeli `rockets`
--
ALTER TABLE `rockets`
  MODIFY `rocketid` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=103;

--
-- AUTO_INCREMENT dla tabeli `ships`
--
ALTER TABLE `ships`
  MODIFY `shipid` int(11) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=130;

--
-- AUTO_INCREMENT dla tabeli `users`
--
ALTER TABLE `users`
  MODIFY `userid` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=106;

--
-- AUTO_INCREMENT dla tabeli `userslog`
--
ALTER TABLE `userslog`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=1162;

--
-- Ograniczenia dla zrzutów tabel
--

--
-- Ograniczenia dla tabeli `enemies`
--
ALTER TABLE `enemies`
  ADD CONSTRAINT `enemies_ibfk_1` FOREIGN KEY (`rewardid`) REFERENCES `rewards` (`rewardid`),
  ADD CONSTRAINT `enemies_ibfk_2` FOREIGN KEY (`prefabid`) REFERENCES `prefabs` (`prefabid`);

--
-- Ograniczenia dla tabeli `enemymap`
--
ALTER TABLE `enemymap`
  ADD CONSTRAINT `enemymap_ibfk_1` FOREIGN KEY (`enemyid`) REFERENCES `enemies` (`enemyid`),
  ADD CONSTRAINT `enemymap_ibfk_2` FOREIGN KEY (`mapid`) REFERENCES `maps` (`mapid`);

--
-- Ograniczenia dla tabeli `pilotresources`
--
ALTER TABLE `pilotresources`
  ADD CONSTRAINT `pilotresources_ibfk_3` FOREIGN KEY (`userid`) REFERENCES `pilots` (`userid`);

--
-- Ograniczenia dla tabeli `pilots`
--
ALTER TABLE `pilots`
  ADD CONSTRAINT `pilots_ibfk_1` FOREIGN KEY (`userid`) REFERENCES `users` (`userId`),
  ADD CONSTRAINT `pilots_ibfk_2` FOREIGN KEY (`mapid`) REFERENCES `maps` (`mapid`),
  ADD CONSTRAINT `pilots_ibfk_3` FOREIGN KEY (`shipid`) REFERENCES `ships` (`shipid`);

--
-- Ograniczenia dla tabeli `portals`
--
ALTER TABLE `portals`
  ADD CONSTRAINT `portals_ibfk_1` FOREIGN KEY (`prefabid`) REFERENCES `prefabs` (`prefabid`),
  ADD CONSTRAINT `portals_ibfk_2` FOREIGN KEY (`mapid`) REFERENCES `maps` (`mapid`),
  ADD CONSTRAINT `portals_ibfk_3` FOREIGN KEY (`target_mapid`) REFERENCES `maps` (`mapid`),
  ADD CONSTRAINT `portals_ibfk_4` FOREIGN KEY (`portalpositionid`) REFERENCES `portalpositions` (`portalpositionid`),
  ADD CONSTRAINT `portals_ibfk_5` FOREIGN KEY (`target_portalpositionid`) REFERENCES `portalpositions` (`portalpositionid`);

--
-- Ograniczenia dla tabeli `prefabs`
--
ALTER TABLE `prefabs`
  ADD CONSTRAINT `prefabs_ibfk_1` FOREIGN KEY (`prefabtypeid`) REFERENCES `prefabs_types` (`prefabtypeid`);

--
-- Ograniczenia dla tabeli `ships`
--
ALTER TABLE `ships`
  ADD CONSTRAINT `ships_ibfk_1` FOREIGN KEY (`rewardid`) REFERENCES `rewards` (`rewardid`),
  ADD CONSTRAINT `ships_ibfk_2` FOREIGN KEY (`prefabid`) REFERENCES `prefabs` (`prefabid`);

--
-- Ograniczenia dla tabeli `userslog`
--
ALTER TABLE `userslog`
  ADD CONSTRAINT `userslog_ibfk_1` FOREIGN KEY (`userid`) REFERENCES `users` (`userid`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
