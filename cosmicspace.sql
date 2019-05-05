-- phpMyAdmin SQL Dump
-- version 4.8.4
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Czas generowania: 05 Maj 2019, 13:38
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

CREATE DEFINER=`root`@`localhost` PROCEDURE `getitems` ()  NO SQL
SELECT *
FROM items i$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `getmaps` ()  NO SQL
SELECT *
FROM maps$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `getpilotitems` (IN `inuserid` BIGINT UNSIGNED)  NO SQL
SELECT *
FROM pilotsitems pi
WHERE pi.userid=inuserid AND pi.issold=0$$

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

CREATE DEFINER=`root`@`localhost` PROCEDURE `getrewarditems` (IN `inrewardid` INT UNSIGNED)  NO SQL
SELECT *
FROM itemreward ir
LEFT JOIN rewards r ON r.rewardid=ir.rewardid
WHERE ir.rewardid=inrewardid$$

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

CREATE DEFINER=`root`@`localhost` PROCEDURE `saveplayerdata` (IN `inuserid` BIGINT UNSIGNED, IN `inpositionx` FLOAT, IN `inpositiony` FLOAT, IN `inshipid` INT UNSIGNED, IN `inexperience` BIGINT UNSIGNED, IN `inlevel` INT, IN `inscrap` DOUBLE, IN `inmetal` DOUBLE, IN `inhitpoints` BIGINT UNSIGNED, IN `inshields` BIGINT UNSIGNED, IN `inmapid` BIGINT UNSIGNED, IN `inammunition0` BIGINT UNSIGNED, IN `inammunition1` BIGINT UNSIGNED, IN `inammunition2` BIGINT UNSIGNED, IN `inammunition3` BIGINT UNSIGNED, IN `inrocket0` BIGINT UNSIGNED, IN `inrocket1` BIGINT UNSIGNED, IN `inrocket2` BIGINT UNSIGNED, IN `inisdead` TINYINT(1), IN `inkillerby` VARCHAR(20), IN `inammunitionid` INT UNSIGNED, IN `inrocketid` INT UNSIGNED)  NO SQL
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
ammunitionid=inammunitionid,
rocketid=inrocketid,
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

CREATE DEFINER=`root`@`localhost` PROCEDURE `saveplayeritem` (IN `inuserid` BIGINT UNSIGNED, IN `initemid` BIGINT, IN `inupgradelevel` INT)  NO SQL
BEGIN

INSERT INTO pilotsitems (userid, itemid, upgradelevel)
VALUES (inuserid, initemid, inupgradelevel);

SELECT LAST_INSERT_ID();

END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `saveplayeritems` (IN `inrelationid` BIGINT UNSIGNED, IN `inupgradelevel` INT, IN `inisequipped` TINYINT(1), IN `inissold` TINYINT(1))  NO SQL
UPDATE pilotsitems
SET
upgradelevel=inupgradelevel,
isequipped=inisequipped,
issold=inissold
WHERE relationid=inrelationid$$

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
  `skillid` tinyint(3) UNSIGNED DEFAULT NULL,
  `isammunition` tinyint(1) NOT NULL DEFAULT '1',
  `basedamage` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Zrzut danych tabeli `ammunitions`
--

INSERT INTO `ammunitions` (`ammunitionid`, `ammunitionname`, `multiplierplayer`, `multiplierenemy`, `scrapprice`, `metalprice`, `skillid`, `isammunition`, `basedamage`) VALUES
(100, 'ammunition0', 1, 1, 1, NULL, NULL, 1, NULL),
(101, 'ammunition1', 2, 2, NULL, 1, NULL, 1, NULL),
(102, 'ammunition2', 3.5, 3.5, NULL, 3, NULL, 1, NULL),
(103, 'ammunition3', 5, 5, NULL, 5, NULL, 1, NULL),
(104, 'rocket0', 1, 1, 10, NULL, NULL, 0, 100),
(105, 'rocket1', 1, 1, 15, NULL, NULL, 0, 800),
(106, 'rocket2', 1, 1, NULL, 10, NULL, 0, 2000);

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
  `rewardid` int(10) UNSIGNED NOT NULL,
  `ammunitionid` int(10) UNSIGNED DEFAULT '100',
  `rocketid` int(10) UNSIGNED DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Zrzut danych tabeli `enemies`
--

INSERT INTO `enemies` (`enemyid`, `enemyname`, `prefabid`, `hitpoints`, `shields`, `speed`, `damage`, `shotdistance`, `isaggressive`, `rewardid`, `ammunitionid`, `rocketid`) VALUES
(1, 'Kingfisher', 49, 200, 100, 6, 15, 25, 0, 10, 100, NULL),
(2, 'Comet', 50, 400, 200, 8, 30, 27, 0, 11, 100, NULL),
(3, 'Unicorn', 51, 800, 400, 10, 70, 28, 1, 12, 100, NULL),
(4, 'Roosevelt', 52, 1200, 600, 9, 110, 26, 0, 13, 100, NULL),
(5, 'Herminia', 53, 2000, 1000, 7, 170, 30, 0, 14, 100, NULL),
(6, 'Lancaster', 54, 2600, 1300, 12, 250, 30, 1, 15, 100, NULL),
(7, 'Meteor', 55, 3200, 1600, 11, 400, 30, 0, 16, 100, NULL),
(8, 'Starhammer', 56, 4000, 2000, 10, 550, 30, 1, 17, 100, NULL),
(9, 'Elba', 57, 6000, 3000, 13, 800, 30, 0, 18, 100, NULL);

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
-- Struktura tabeli dla tabeli `itemreward`
--

CREATE TABLE `itemreward` (
  `itemrewardid` bigint(20) NOT NULL,
  `rewardid` int(11) UNSIGNED NOT NULL,
  `itemid` bigint(20) NOT NULL,
  `upgradelevel` int(11) NOT NULL DEFAULT '1',
  `chance` int(11) NOT NULL DEFAULT '1000'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Zrzut danych tabeli `itemreward`
--

INSERT INTO `itemreward` (`itemrewardid`, `rewardid`, `itemid`, `upgradelevel`, `chance`) VALUES
(1, 10, 1, 1, 500),
(2, 10, 2, 1, 450),
(3, 11, 3, 1, 400),
(4, 12, 4, 1, 350),
(5, 13, 5, 1, 300),
(6, 14, 6, 1, 250);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `items`
--

CREATE TABLE `items` (
  `itemid` bigint(20) NOT NULL,
  `name` varchar(100) COLLATE utf8_polish_ci NOT NULL,
  `prefabname` varchar(100) COLLATE utf8_polish_ci NOT NULL,
  `itemtypeid` int(11) NOT NULL,
  `requiredlevel` int(11) NOT NULL DEFAULT '1',
  `laser_damage_pvp` bigint(20) DEFAULT NULL,
  `laser_damage_pve` bigint(20) DEFAULT NULL,
  `laser_shotrange` int(11) DEFAULT '50',
  `laser_shotdispersion` float DEFAULT '0.15',
  `generator_speed` float DEFAULT NULL,
  `generator_shield` bigint(20) DEFAULT NULL,
  `generator_shield_division` float DEFAULT '0.7',
  `generator_shield_repair` int(11) DEFAULT '20',
  `scrapprice` double DEFAULT NULL,
  `metalprice` double DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Zrzut danych tabeli `items`
--

INSERT INTO `items` (`itemid`, `name`, `prefabname`, `itemtypeid`, `requiredlevel`, `laser_damage_pvp`, `laser_damage_pve`, `laser_shotrange`, `laser_shotdispersion`, `generator_speed`, `generator_shield`, `generator_shield_division`, `generator_shield_repair`, `scrapprice`, `metalprice`) VALUES
(1, 'Laser0', 'Laser0', 1, 1, 30, 100, 50, 0.15, NULL, NULL, NULL, NULL, 100, NULL),
(2, 'Shield0', 'Shield0', 2, 1, NULL, NULL, NULL, NULL, NULL, 150, 0.7, 20, NULL, 70),
(3, 'Speed0', 'Speed0', 2, 1, NULL, NULL, NULL, NULL, 5, NULL, NULL, NULL, 75, NULL),
(4, 'Laser1', 'Laser1', 1, 2, 170, 170, 50, 0.15, NULL, NULL, NULL, NULL, 50, NULL),
(5, 'Shield1', 'Shield1', 2, 1, NULL, NULL, NULL, NULL, NULL, 300, 0.7, 20, NULL, 100),
(6, 'Speed1', 'Speed1', 2, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 150),
(7, 'Laser2', 'Laser2', 1, 3, 100, 95, 50, 0.15, NULL, NULL, NULL, NULL, 100, 100),
(8, 'Laser3', 'Laser3', 1, 1, 100, 95, 50, 0.15, NULL, NULL, NULL, NULL, 100, 100),
(9, 'Laser4', 'Laser4', 1, 1, 100, 95, 50, 0.15, NULL, NULL, NULL, NULL, 100, 100),
(10, 'Laser5', 'Laser5', 1, 1, 100, 95, 50, 0.15, NULL, NULL, NULL, NULL, 100, 100),
(11, 'Rocket0', 'Rocket0', 1, 1, 100, 95, 50, 0.15, NULL, NULL, NULL, NULL, 100, 100),
(12, 'Rocket1', 'Rocket1', 1, 1, 100, 95, 50, 0.15, NULL, NULL, NULL, NULL, 100, 100),
(13, 'Rocket2', 'Rocket2', 1, 1, 100, 95, 50, 0.15, NULL, NULL, NULL, NULL, 100, 100),
(14, 'Speed2', 'Speed2', 2, 1, NULL, NULL, NULL, NULL, 5, NULL, NULL, NULL, 10, 10),
(15, 'Speed3', 'Speed3', 2, 2, NULL, NULL, NULL, NULL, 5, NULL, NULL, NULL, 10, 10),
(16, 'Shield2', 'Shield2', 2, 1, NULL, NULL, NULL, NULL, NULL, 1000, 0.7, 20, 100, 100),
(17, 'Shield3', 'Shield3', 2, 1, NULL, NULL, NULL, NULL, NULL, 1000, 0.7, 20, 100, 100),
(18, 'Shield4', 'Shield4', 2, 2, NULL, NULL, NULL, NULL, NULL, 1000, 0.7, 20, 100, 100),
(19, 'Shield5', 'Shield5', 2, 2, NULL, NULL, NULL, NULL, NULL, 1000, 0.7, 20, 100, 100),
(20, 'Shield6', 'Shield6', 2, 3, NULL, NULL, NULL, NULL, NULL, 1000, 0.7, 20, 100, 100),
(21, 'Shield7', 'Shield7', 2, 1, NULL, NULL, NULL, NULL, NULL, 1000, 0.7, 20, 100, 100),
(22, 'Shield8', 'Shield8', 2, 1, NULL, NULL, NULL, NULL, NULL, 1000, 0.7, 20, 100, 100),
(23, 'Shield9', 'Shield9', 2, 1, NULL, NULL, NULL, NULL, NULL, 1000, 0.7, 20, 100, 100),
(24, 'Shield10', 'Shield10', 2, 1, NULL, NULL, NULL, NULL, NULL, 1000, 0.7, 20, 100, 100),
(25, 'Shield11', 'Shield11', 2, 1, NULL, NULL, NULL, NULL, NULL, 1000, 0.7, 20, 100, 100);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `itemtypes`
--

CREATE TABLE `itemtypes` (
  `itemtypeid` int(11) NOT NULL,
  `itemtypename` varchar(50) COLLATE utf8_polish_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Zrzut danych tabeli `itemtypes`
--

INSERT INTO `itemtypes` (`itemtypeid`, `itemtypename`) VALUES
(1, 'Laser'),
(2, 'Generator'),
(3, 'Extra');

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
(102, 1000037275, 16998, 16989, 1000016633, 17000, 1000016999, 17000),
(103, 500, 0, 0, 0, 0, 0, 0),
(104, 500, 0, 0, 0, 0, 0, 0),
(105, 500, 0, 0, 0, 0, 0, 0),
(106, 500, 0, 0, 0, 0, 0, 0),
(107, 500, 0, 0, 0, 0, 0, 0),
(108, 500, 0, 0, 0, 0, 0, 0),
(109, 500, 0, 0, 0, 0, 0, 0),
(110, 500, 0, 0, 0, 0, 0, 0);

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
  `ammunitionid` int(10) UNSIGNED NOT NULL DEFAULT '100',
  `rocketid` int(10) UNSIGNED NOT NULL DEFAULT '104',
  `isdead` tinyint(1) NOT NULL DEFAULT '0',
  `killerby` varchar(20) COLLATE utf8_polish_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Zrzut danych tabeli `pilots`
--

INSERT INTO `pilots` (`userid`, `nickname`, `mapid`, `positionx`, `positiony`, `shipid`, `experience`, `level`, `scrap`, `metal`, `hitpoints`, `shields`, `ammunitionid`, `rocketid`, `isdead`, `killerby`) VALUES
(100, 'test1', 100, 319.613, -275.955, 100, 2, 1, 0, 0, 874, 706, 100, 104, 0, NULL),
(101, 'test2', 100, 881.718, -861.051, 101, 1356, 1, 57, 3, 1600, 0, 100, 104, 0, NULL),
(102, 'test3', 100, 909, -300, 129, 165602, 100, 69999902544, 8869993383, 200000, 0, 103, 104, 0, NULL),
(103, 'test4', 101, 81.0209, -74.1459, 103, 28355, 1, 57, 0, 3700, 0, 100, 104, 0, NULL),
(104, 'test5', 100, 910, -929, 104, 791, 1, 36, 1, 5000, 150, 100, 104, 0, NULL),
(105, 'test6', 100, 284.76, -360.721, 105, 0, 1, 0, 0, 41166, 0, 100, 104, 0, NULL),
(106, 'test7', 100, 154.802, -85.7609, 100, 2174, 1, 44, 11, 952, 0, 100, 104, 0, NULL),
(107, 'Rosol', 100, 100, -100, 100, 0, 1, 0, 0, 1000, 0, 100, 104, 0, NULL),
(108, 'sasa', 100, 100, -100, 100, 0, 1, 0, 0, 1000, 0, 100, 104, 0, NULL),
(109, 'Blant', 100, 100, -100, 100, 0, 1, 0, 0, 1000, 0, 100, 104, 0, NULL),
(110, 'test8', 100, 151.994, -102.105, 100, 369, 1, 8, 2, 971, 0, 100, 104, 0, NULL);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `pilotsitems`
--

CREATE TABLE `pilotsitems` (
  `relationid` bigint(20) UNSIGNED NOT NULL,
  `userid` bigint(20) UNSIGNED NOT NULL,
  `itemid` bigint(20) NOT NULL,
  `upgradelevel` int(11) NOT NULL DEFAULT '1',
  `isequipped` tinyint(4) NOT NULL DEFAULT '0',
  `issold` tinyint(4) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Zrzut danych tabeli `pilotsitems`
--

INSERT INTO `pilotsitems` (`relationid`, `userid`, `itemid`, `upgradelevel`, `isequipped`, `issold`) VALUES
(1, 102, 1, 1, 0, 1),
(2, 102, 1, 1, 0, 1),
(3, 102, 1, 1, 1, 0),
(4, 102, 2, 1, 0, 0),
(5, 102, 2, 1, 0, 1),
(6, 102, 3, 1, 1, 0),
(7, 102, 3, 1, 0, 1),
(8, 102, 1, 1, 0, 0),
(9, 102, 2, 1, 0, 0),
(10, 102, 1, 1, 0, 0),
(11, 102, 2, 1, 0, 0),
(12, 102, 1, 1, 0, 0),
(13, 102, 1, 1, 0, 1),
(14, 102, 2, 1, 0, 0),
(15, 102, 3, 1, 1, 0),
(16, 102, 2, 1, 0, 0),
(17, 102, 1, 1, 0, 0),
(18, 102, 3, 1, 0, 1),
(19, 102, 2, 1, 0, 0),
(20, 106, 1, 1, 1, 0),
(21, 106, 2, 1, 1, 0),
(22, 106, 2, 1, 0, 0),
(23, 106, 2, 1, 0, 0),
(24, 102, 1, 1, 0, 0),
(25, 102, 2, 1, 0, 1),
(26, 102, 2, 1, 0, 0),
(27, 102, 1, 1, 1, 0),
(28, 102, 3, 1, 0, 1),
(29, 102, 3, 1, 1, 0),
(30, 102, 2, 1, 0, 0),
(31, 102, 1, 1, 0, 0),
(32, 102, 3, 1, 1, 0),
(33, 102, 1, 1, 0, 0),
(34, 102, 1, 1, 0, 0),
(35, 102, 1, 1, 0, 1),
(36, 102, 3, 1, 0, 1),
(37, 102, 2, 1, 0, 0),
(38, 102, 1, 1, 0, 1),
(39, 102, 1, 1, 0, 1),
(40, 102, 1, 1, 0, 0),
(41, 102, 2, 1, 0, 1),
(42, 102, 1, 1, 1, 0),
(43, 102, 2, 1, 0, 0),
(44, 102, 1, 1, 0, 1),
(45, 102, 1, 1, 0, 0),
(46, 102, 1, 1, 0, 0),
(47, 102, 1, 1, 0, 0),
(48, 102, 2, 1, 0, 1),
(49, 102, 1, 1, 0, 1),
(50, 102, 2, 1, 0, 0),
(51, 102, 2, 1, 0, 0),
(52, 102, 3, 1, 1, 0),
(53, 102, 1, 1, 0, 0),
(54, 102, 3, 1, 1, 0),
(55, 102, 2, 1, 0, 0),
(56, 102, 3, 1, 1, 0),
(57, 102, 1, 1, 0, 0),
(58, 102, 3, 1, 1, 0),
(59, 102, 1, 1, 0, 0),
(60, 102, 1, 1, 0, 0),
(61, 102, 2, 1, 0, 0),
(62, 102, 1, 1, 0, 0),
(63, 102, 2, 1, 0, 0),
(64, 102, 3, 1, 1, 0),
(65, 102, 1, 1, 0, 0),
(66, 102, 1, 1, 0, 0),
(67, 102, 3, 1, 1, 0),
(68, 102, 1, 1, 0, 0),
(69, 102, 2, 1, 0, 0),
(70, 102, 2, 1, 0, 0),
(71, 102, 2, 1, 0, 0),
(72, 102, 1, 1, 0, 0),
(73, 102, 2, 1, 0, 1),
(74, 102, 1, 1, 0, 0),
(75, 102, 3, 1, 1, 0),
(76, 102, 1, 1, 0, 0),
(77, 102, 3, 1, 0, 1),
(78, 102, 3, 1, 1, 0),
(79, 102, 1, 1, 0, 0),
(80, 102, 1, 1, 0, 0),
(81, 102, 1, 1, 0, 0),
(82, 102, 3, 1, 1, 0),
(83, 102, 1, 1, 0, 0),
(84, 102, 3, 1, 1, 0),
(85, 102, 1, 1, 0, 0),
(86, 102, 2, 1, 0, 1),
(87, 102, 1, 1, 0, 0),
(88, 102, 3, 1, 1, 0),
(89, 102, 1, 1, 0, 0),
(90, 102, 2, 1, 0, 0),
(91, 102, 3, 1, 1, 0),
(92, 102, 3, 1, 1, 0),
(93, 102, 1, 1, 0, 0),
(94, 102, 1, 1, 0, 0),
(95, 102, 2, 1, 0, 0),
(96, 102, 1, 1, 0, 0),
(97, 102, 3, 1, 1, 0),
(98, 102, 1, 1, 0, 0),
(99, 102, 4, 1, 0, 0),
(100, 110, 1, 1, 1, 0),
(101, 100, 1, 1, 0, 0),
(102, 101, 1, 1, 1, 0),
(103, 102, 1, 1, 0, 0),
(104, 103, 1, 1, 0, 0),
(105, 104, 1, 1, 1, 0),
(106, 105, 1, 1, 0, 0),
(107, 106, 1, 1, 0, 0),
(108, 107, 1, 1, 0, 0),
(109, 108, 1, 1, 0, 0),
(110, 109, 1, 1, 0, 0),
(111, 101, 3, 1, 1, 0),
(112, 102, 2, 1, 0, 0),
(113, 104, 2, 1, 1, 0),
(114, 102, 1, 1, 0, 0),
(115, 102, 4, 1, 0, 0),
(117, 102, 4, 1, 0, 0),
(118, 102, 4, 1, 0, 1),
(119, 102, 4, 1, 0, 0),
(120, 102, 4, 1, 0, 0),
(121, 102, 6, 1, 0, 0),
(122, 102, 5, 1, 0, 0),
(123, 102, 5, 1, 0, 0),
(124, 102, 5, 1, 0, 0),
(125, 102, 4, 1, 0, 0),
(126, 102, 4, 1, 0, 0),
(127, 102, 6, 1, 1, 0),
(128, 102, 6, 1, 1, 0),
(129, 102, 6, 1, 0, 0),
(130, 102, 6, 1, 0, 0),
(131, 102, 5, 1, 0, 0),
(132, 102, 5, 1, 0, 0),
(133, 102, 1, 1, 0, 0),
(134, 102, 3, 1, 0, 1),
(135, 101, 1, 1, 0, 0),
(136, 101, 2, 1, 0, 0),
(137, 102, 1, 1, 0, 0),
(138, 102, 1, 1, 0, 0),
(139, 102, 4, 1, 0, 0),
(140, 102, 3, 1, 0, 0),
(141, 102, 2, 1, 0, 0),
(142, 102, 1, 1, 0, 1),
(143, 102, 3, 1, 0, 0),
(144, 102, 1, 1, 0, 0),
(145, 102, 2, 1, 0, 0),
(146, 102, 1, 1, 0, 0),
(147, 102, 3, 1, 0, 0),
(148, 102, 2, 1, 0, 0),
(149, 102, 1, 1, 0, 1),
(150, 102, 2, 1, 0, 1),
(151, 102, 2, 1, 0, 1),
(152, 102, 3, 1, 0, 0),
(153, 102, 1, 1, 0, 0),
(154, 102, 2, 1, 0, 0),
(155, 102, 3, 1, 0, 0),
(156, 102, 25, 1, 0, 0),
(157, 102, 25, 1, 0, 0),
(158, 102, 25, 1, 0, 1),
(159, 102, 13, 1, 0, 1),
(160, 102, 10, 1, 0, 0),
(161, 102, 9, 1, 0, 0),
(162, 102, 15, 1, 0, 0),
(163, 102, 14, 1, 0, 1),
(164, 102, 24, 1, 0, 0),
(165, 102, 22, 1, 0, 1),
(166, 102, 3, 1, 0, 1),
(167, 102, 7, 1, 0, 1),
(168, 102, 2, 1, 0, 0),
(169, 102, 1, 1, 0, 0),
(170, 102, 4, 1, 0, 0),
(171, 102, 1, 1, 0, 0),
(172, 102, 2, 1, 0, 0),
(173, 102, 22, 1, 0, 0),
(174, 102, 24, 1, 0, 0),
(175, 102, 7, 1, 0, 0),
(176, 102, 7, 1, 0, 0),
(177, 102, 15, 1, 0, 0),
(178, 102, 15, 1, 0, 0),
(179, 102, 1, 1, 0, 0),
(180, 102, 2, 1, 0, 0),
(181, 102, 3, 1, 0, 0),
(182, 102, 1, 1, 0, 0),
(183, 102, 1, 1, 0, 0),
(184, 102, 2, 1, 0, 0),
(185, 102, 1, 1, 0, 0),
(186, 102, 1, 1, 0, 0),
(187, 102, 2, 1, 0, 0),
(188, 102, 1, 1, 0, 0),
(189, 102, 2, 1, 0, 0),
(190, 102, 1, 1, 0, 0),
(191, 102, 4, 1, 0, 0),
(192, 102, 1, 1, 0, 0),
(193, 102, 2, 1, 0, 0),
(194, 102, 3, 1, 0, 0),
(195, 102, 3, 1, 0, 0),
(196, 102, 3, 1, 0, 0),
(197, 102, 2, 1, 0, 0),
(198, 102, 1, 1, 0, 0),
(199, 102, 3, 1, 0, 0),
(200, 102, 1, 1, 0, 0),
(201, 102, 1, 1, 0, 0),
(202, 102, 2, 1, 0, 0),
(203, 102, 2, 1, 0, 0),
(204, 102, 1, 1, 0, 0),
(205, 102, 1, 1, 0, 0),
(206, 102, 2, 1, 0, 0),
(207, 102, 1, 1, 0, 0),
(208, 102, 2, 1, 0, 0),
(209, 102, 3, 1, 0, 0),
(210, 102, 4, 1, 0, 0),
(211, 102, 1, 1, 0, 0),
(212, 102, 2, 1, 0, 0),
(213, 102, 2, 1, 0, 0),
(214, 102, 1, 1, 0, 0),
(215, 102, 2, 1, 0, 0),
(216, 102, 2, 1, 0, 0),
(217, 102, 2, 1, 0, 0),
(218, 102, 1, 1, 0, 0),
(219, 102, 1, 1, 0, 0),
(220, 102, 2, 1, 0, 0),
(221, 102, 1, 1, 0, 0),
(222, 102, 4, 1, 0, 0),
(223, 102, 1, 1, 0, 0),
(224, 102, 3, 1, 0, 0),
(225, 102, 1, 1, 0, 0),
(226, 102, 3, 1, 0, 0),
(227, 102, 3, 1, 0, 0),
(228, 102, 3, 1, 0, 0),
(229, 102, 3, 1, 0, 0),
(230, 102, 4, 1, 0, 0),
(231, 102, 2, 1, 0, 0),
(232, 102, 3, 1, 0, 0),
(233, 102, 1, 1, 0, 0),
(234, 102, 3, 1, 0, 0),
(235, 102, 2, 1, 0, 0),
(236, 102, 4, 1, 0, 0),
(237, 102, 1, 1, 0, 0),
(238, 102, 3, 1, 0, 0),
(239, 102, 4, 1, 0, 0),
(240, 102, 3, 1, 0, 0),
(241, 102, 3, 1, 0, 0),
(242, 102, 1, 1, 0, 0),
(243, 102, 2, 1, 0, 0),
(244, 102, 1, 1, 0, 0),
(245, 102, 1, 1, 0, 0),
(246, 102, 2, 1, 0, 0),
(247, 102, 4, 1, 0, 0),
(248, 102, 2, 1, 0, 0),
(249, 102, 3, 1, 0, 0),
(250, 102, 1, 1, 0, 0),
(251, 102, 1, 1, 0, 0),
(252, 102, 7, 1, 0, 0),
(253, 102, 7, 1, 0, 0),
(254, 102, 4, 1, 0, 0),
(255, 102, 4, 1, 0, 0),
(256, 102, 1, 1, 1, 0),
(257, 102, 2, 1, 0, 0),
(258, 102, 3, 1, 0, 0),
(259, 102, 3, 1, 0, 0),
(260, 102, 1, 1, 0, 0);

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
(1, 31, 100, 42, 101, 1),
(2, 31, 101, 1, 100, 42),
(3, 37, 101, 116, 102, 110),
(5, 43, 102, 110, 101, 116),
(6, 31, 101, 55, 103, 16),
(7, 31, 103, 16, 101, 55),
(10, 31, 101, 40, 104, 119),
(11, 31, 104, 119, 101, 40),
(60, 31, 102, 85, 105, 115),
(61, 31, 105, 115, 102, 85),
(62, 31, 102, 35, 106, 1),
(63, 31, 106, 1, 102, 35),
(64, 31, 105, 35, 106, 119),
(65, 31, 106, 119, 105, 35),
(66, 31, 106, 38, 107, 118),
(67, 31, 107, 118, 106, 38),
(68, 31, 103, 75, 107, 1),
(69, 31, 107, 1, 103, 75),
(70, 31, 103, 39, 108, 3),
(71, 31, 108, 3, 103, 39),
(72, 31, 107, 110, 108, 95),
(73, 31, 108, 95, 107, 110),
(74, 31, 107, 38, 109, 5),
(75, 31, 109, 5, 107, 38),
(76, 31, 108, 35, 109, 21),
(77, 31, 109, 21, 108, 35),
(78, 31, 108, 43, 110, 116),
(79, 31, 110, 116, 108, 43),
(80, 31, 104, 35, 110, 1),
(81, 31, 110, 1, 104, 35),
(82, 31, 104, 43, 111, 4),
(83, 31, 111, 4, 104, 43),
(84, 31, 110, 110, 111, 90),
(85, 31, 111, 90, 110, 110),
(86, 31, 111, 39, 112, 21),
(87, 31, 112, 21, 111, 39),
(88, 31, 110, 40, 112, 5),
(89, 31, 112, 5, 110, 40),
(90, 31, 112, 95, 113, 110),
(91, 31, 113, 110, 112, 95),
(92, 31, 109, 110, 113, 116),
(93, 31, 113, 116, 109, 110),
(94, 31, 113, 85, 114, 115),
(95, 31, 114, 115, 113, 85),
(96, 31, 113, 40, 115, 121),
(97, 31, 115, 121, 113, 40),
(98, 31, 114, 40, 116, 120),
(99, 31, 116, 120, 114, 40),
(100, 31, 115, 85, 116, 114),
(101, 31, 116, 114, 115, 85),
(102, 31, 110, 35, 113, 1),
(103, 31, 113, 1, 110, 35);

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
(7, 'Oregon', 1, ''),
(8, 'Scavenger', 1, ''),
(9, 'Herald', 1, ''),
(10, 'Colossus', 1, ''),
(11, 'Trenxal', 1, ''),
(12, 'Spectator', 1, ''),
(13, 'Cain', 1, ''),
(14, 'Neutron', 1, ''),
(15, 'Determination', 1, ''),
(16, 'Ambition', 1, ''),
(17, 'Twilight', 1, ''),
(18, 'Corsair', 1, ''),
(19, 'Escorial', 1, ''),
(20, 'Phobos', 1, ''),
(21, 'Achilles', 1, ''),
(22, 'Valkyrie', 1, ''),
(23, 'Intrepid', 1, ''),
(24, 'Ravana', 1, ''),
(25, 'Berserk', 1, ''),
(26, 'Crusher', 1, ''),
(27, 'Andromeda', 1, ''),
(28, 'Prennia', 1, ''),
(29, 'Navigator', 1, ''),
(30, 'Zeus', 1, ''),
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
  `scrap` double DEFAULT NULL,
  `ammunitionid` int(10) UNSIGNED DEFAULT NULL,
  `ammunition_quantity` bigint(20) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Zrzut danych tabeli `rewards`
--

INSERT INTO `rewards` (`rewardid`, `experience`, `metal`, `scrap`, `ammunitionid`, `ammunition_quantity`) VALUES
(1, 1, NULL, NULL, NULL, NULL),
(2, 2, NULL, NULL, NULL, NULL),
(3, 4, NULL, NULL, NULL, NULL),
(4, 8, NULL, NULL, NULL, NULL),
(5, 16, NULL, NULL, NULL, NULL),
(6, 32, NULL, NULL, NULL, NULL),
(7, 64, NULL, NULL, NULL, NULL),
(8, 128, NULL, NULL, NULL, NULL),
(9, 256, NULL, NULL, NULL, NULL),
(10, 307, 1, 4, 100, 2),
(11, 369, 2, 8, 100, 4),
(12, 442, 3, 12, NULL, NULL),
(13, 531, 4, 16, NULL, NULL),
(14, 637, 5, 20, NULL, NULL),
(15, 764, 6, 24, NULL, NULL),
(16, 917, 7, 28, NULL, NULL),
(17, 1101, 8, 32, NULL, NULL),
(18, 1321, 9, 36, NULL, NULL),
(19, 1585, 10, 50, NULL, NULL),
(20, 1902, 14, 70, NULL, NULL),
(21, 2283, 18, 90, NULL, NULL),
(22, 2739, 22, 121, NULL, NULL),
(23, 3287, 26, 143, NULL, NULL),
(24, 3944, 30, 180, NULL, NULL),
(25, 4339, 33, 215, NULL, NULL),
(26, 4772, 38, 266, NULL, NULL),
(27, 5250, 41, 308, NULL, NULL),
(28, 5775, 44, 352, NULL, NULL),
(29, 6352, 47, 400, NULL, NULL),
(30, 6987, 50, 450, NULL, NULL);

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
(100, 'Avius', 1, 1, 1, 1, 1, 1, 1, 65, 40, 1000, 1),
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
(105, 'B4FDF8DD90B9FF39276EF1C959D08DED6369FD6FF36E231924AD3A8F5A3689DB93CAF54DF37A63A07C64E2F712190379AEC9399A2189439ACC7829B1486D1B93', '6FD332AD992E8A0DC450533F2729EB8DFBAAFF697911546F495AD6E7E60772136724594626C81BED60188277F506800FB962198F7BD3021A251FCEFC30089EB3', 'test6', 1, 1, 0, '2018-12-29 21:49:37'),
(106, '739EA2D8AE0E866A5E98370E93660351A17B7B6CE8169F50B276E95859621A746227F11C025755FBA7C9292B420FF8B3D221CEDED3F01065B844906EEA550E43', '6FD332AD992E8A0DC450533F2729EB8DFBAAFF697911546F495AD6E7E60772136724594626C81BED60188277F506800FB962198F7BD3021A251FCEFC30089EB3', 'test7', 1, 1, 0, '2019-04-22 15:20:28'),
(107, 'B8D7FDF374776ADD969216DA4CD7D549ED925FB113FED8369C0D7A7B3A27616A658469A7003C738B80E5AEC1AAF57BF481422AA5290C43DDE68D8CB15106E74A', '3C41F133F5647ACDE1099150E1D71B6A778DEB43608E4941451150518A60E4E3482E07041EE98BEA8FF81664E652DD952007E2A7AAF9362B77791FCE81C3DD7C', 'rosol2k9@gmail.com', 1, 1, 0, '2019-04-15 15:28:48'),
(108, 'ABD6CCDC0104685A81654AA77EB6DBA7CCD354B94CB5044479AB437B8A55B87D203BD9F4A293C94DFB0410763CF1F254689909DA89E0269EDC0DD932F0FED773', 'ABD6CCDC0104685A81654AA77EB6DBA7CCD354B94CB5044479AB437B8A55B87D203BD9F4A293C94DFB0410763CF1F254689909DA89E0269EDC0DD932F0FED773', 'sasa@sasa.pl', 1, 1, 0, '2019-04-15 17:59:21'),
(109, '4AE67A3775486C4AA95F48500EB1E30BA42ABADF07ACAB3F4F0FA0F3D96BB014F74AA8063C280416CF99B5B8FD4AB3F7EE331E721501F4849E8973FFE8B68E45', 'FC5493A71AFFC8247888DC63F3C60B172DE9D1C43B1B2AC6FB8260D16B5B974790AABF0B1A10D34776BA7044E541D671357191C58A34B37395A57876F760D7FD', 'lewyp4@o2.pl', 0, 1, 0, '2019-04-18 16:05:10'),
(110, '5A66EA8468E426A22CA642BFFCDC351159F3C017A4F439F1E35A902D70D66734101E0D5FCBCD03E4D35C7C2BEB97D95661F7B81AD6F604F3F386804C928EB2EA', '6FD332AD992E8A0DC450533F2729EB8DFBAAFF697911546F495AD6E7E60772136724594626C81BED60188277F506800FB962198F7BD3021A251FCEFC30089EB3', 'test8', 0, 1, 0, '2019-04-22 16:30:06');

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
(1161, 102, '2019-04-14 18:37:48', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1162, 102, '2019-04-19 17:58:33', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1163, 101, '2019-04-19 17:58:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1164, 102, '2019-04-19 18:15:20', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1165, 101, '2019-04-19 18:15:23', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1166, 102, '2019-04-19 18:22:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1167, 101, '2019-04-19 18:22:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1168, 102, '2019-04-19 18:26:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1169, 101, '2019-04-19 18:26:04', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1170, 102, '2019-04-19 18:35:58', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1171, 101, '2019-04-19 18:36:02', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1172, 101, '2019-04-19 18:36:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1173, 102, '2019-04-19 18:36:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1174, 102, '2019-04-19 18:41:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1175, 101, '2019-04-19 18:41:47', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1176, 102, '2019-04-19 18:45:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1177, 102, '2019-04-19 18:46:29', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1178, 102, '2019-04-19 18:47:13', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1179, 101, '2019-04-19 18:47:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1180, 101, '2019-04-19 18:51:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1181, 102, '2019-04-19 18:51:52', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1182, 102, '2019-04-19 18:52:45', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1183, 102, '2019-04-19 18:59:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1184, 101, '2019-04-19 18:59:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1185, 102, '2019-04-19 19:01:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1186, 101, '2019-04-19 19:02:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1187, 102, '2019-04-19 19:17:25', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1188, 102, '2019-04-19 19:18:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1189, 102, '2019-04-19 19:18:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1190, 102, '2019-04-19 19:20:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1191, 102, '2019-04-19 19:21:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1192, 102, '2019-04-19 19:24:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1193, 102, '2019-04-19 19:26:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1194, 102, '2019-04-19 19:27:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1195, 102, '2019-04-19 19:42:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1196, 102, '2019-04-19 19:47:38', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1197, 102, '2019-04-19 19:51:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1198, 102, '2019-04-19 19:52:23', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1199, 102, '2019-04-19 23:24:07', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1200, 102, '2019-04-19 23:27:02', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1201, 102, '2019-04-19 23:27:51', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1202, 102, '2019-04-19 23:28:19', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1203, 102, '2019-04-19 23:31:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1204, 102, '2019-04-19 23:32:21', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1205, 102, '2019-04-19 23:34:25', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1206, 102, '2019-04-19 23:38:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1207, 102, '2019-04-19 23:39:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1208, 102, '2019-04-19 23:40:06', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1209, 102, '2019-04-19 23:40:59', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1210, 102, '2019-04-19 23:41:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1211, 102, '2019-04-19 23:43:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1212, 102, '2019-04-19 23:45:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1213, 102, '2019-04-19 23:46:23', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1214, 102, '2019-04-19 23:47:21', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1215, 102, '2019-04-19 23:56:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1216, 102, '2019-04-19 23:57:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1217, 102, '2019-04-20 08:05:02', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1218, 102, '2019-04-20 08:12:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1219, 102, '2019-04-20 08:29:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1220, 102, '2019-04-20 08:30:23', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1221, 102, '2019-04-20 08:31:13', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1222, 102, '2019-04-20 08:32:11', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1223, 102, '2019-04-20 08:33:55', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1224, 102, '2019-04-20 08:34:59', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1225, 102, '2019-04-20 08:46:01', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1226, 102, '2019-04-20 08:55:07', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1227, 102, '2019-04-20 08:56:33', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1228, 102, '2019-04-20 10:22:48', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1229, 102, '2019-04-20 10:42:29', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1230, 102, '2019-04-20 10:50:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1231, 102, '2019-04-20 10:50:29', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1232, 102, '2019-04-20 10:56:04', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1233, 102, '2019-04-20 10:56:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1234, 102, '2019-04-20 11:07:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1235, 102, '2019-04-20 11:07:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1236, 102, '2019-04-20 11:09:38', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1237, 102, '2019-04-20 11:12:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1238, 102, '2019-04-20 11:24:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1239, 102, '2019-04-20 11:41:51', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1240, 102, '2019-04-20 11:42:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1241, 102, '2019-04-20 11:51:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1242, 102, '2019-04-20 12:06:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1243, 102, '2019-04-20 12:07:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1244, 102, '2019-04-20 12:07:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1245, 102, '2019-04-20 12:08:07', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1246, 102, '2019-04-20 12:08:30', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1247, 102, '2019-04-20 12:09:19', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1248, 102, '2019-04-20 12:09:59', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1249, 102, '2019-04-21 09:06:02', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1250, 102, '2019-04-21 09:10:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1251, 102, '2019-04-21 09:22:29', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1252, 102, '2019-04-21 10:24:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1253, 102, '2019-04-21 10:33:09', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1254, 102, '2019-04-21 10:35:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1255, 102, '2019-04-21 10:45:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1256, 102, '2019-04-21 10:47:23', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1257, 102, '2019-04-21 10:49:16', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1258, 102, '2019-04-21 10:51:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1259, 102, '2019-04-21 10:52:45', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1260, 102, '2019-04-21 10:54:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1261, 102, '2019-04-21 10:54:55', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1262, 102, '2019-04-21 11:02:23', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1263, 102, '2019-04-21 11:03:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1264, 102, '2019-04-21 11:08:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1265, 102, '2019-04-21 11:10:52', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1266, 102, '2019-04-21 11:11:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1267, 102, '2019-04-21 11:13:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1268, 102, '2019-04-21 11:14:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1269, 102, '2019-04-21 11:16:20', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1270, 102, '2019-04-21 11:20:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1271, 102, '2019-04-21 11:23:19', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1272, 102, '2019-04-21 11:28:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1273, 102, '2019-04-21 11:29:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1274, 102, '2019-04-21 11:31:54', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1275, 102, '2019-04-21 11:35:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1276, 102, '2019-04-21 11:37:51', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1277, 102, '2019-04-21 11:38:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1278, 102, '2019-04-21 11:40:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1279, 102, '2019-04-21 11:41:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1280, 102, '2019-04-21 11:41:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1281, 102, '2019-04-21 11:42:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1282, 102, '2019-04-21 11:43:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1283, 102, '2019-04-21 11:45:30', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1284, 102, '2019-04-21 11:46:34', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1285, 102, '2019-04-21 11:47:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1286, 102, '2019-04-21 11:48:44', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1287, 102, '2019-04-21 11:57:39', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1288, 102, '2019-04-21 11:59:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1289, 102, '2019-04-21 12:00:09', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1290, 102, '2019-04-21 12:00:48', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1291, 102, '2019-04-21 12:02:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1292, 102, '2019-04-21 12:03:23', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1293, 102, '2019-04-21 12:04:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1294, 102, '2019-04-21 12:06:09', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1295, 102, '2019-04-21 12:06:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1296, 102, '2019-04-21 12:08:26', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1297, 102, '2019-04-21 12:10:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1298, 102, '2019-04-21 12:11:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1299, 102, '2019-04-21 12:14:45', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1300, 102, '2019-04-21 12:17:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1301, 102, '2019-04-21 12:18:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1302, 102, '2019-04-21 12:19:27', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1303, 102, '2019-04-21 12:19:59', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1304, 102, '2019-04-21 12:20:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1305, 102, '2019-04-21 12:26:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1306, 102, '2019-04-21 12:27:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1307, 102, '2019-04-21 12:28:27', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1308, 102, '2019-04-21 12:30:11', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1309, 102, '2019-04-21 12:33:23', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1310, 102, '2019-04-21 12:35:13', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1311, 102, '2019-04-21 12:37:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1312, 102, '2019-04-21 12:54:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1313, 102, '2019-04-21 12:57:01', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1314, 102, '2019-04-21 12:58:25', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1315, 102, '2019-04-21 13:00:04', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1316, 102, '2019-04-21 13:01:06', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1317, 102, '2019-04-21 13:01:44', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1318, 102, '2019-04-21 13:10:51', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1319, 102, '2019-04-21 13:14:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1320, 102, '2019-04-21 13:15:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1321, 102, '2019-04-21 13:16:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1322, 102, '2019-04-21 13:16:44', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1323, 102, '2019-04-21 13:17:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1324, 102, '2019-04-21 13:19:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1325, 102, '2019-04-21 15:28:04', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1326, 102, '2019-04-21 15:55:27', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1327, 102, '2019-04-21 15:56:38', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1328, 102, '2019-04-21 16:00:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1329, 102, '2019-04-21 16:02:26', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1330, 102, '2019-04-21 16:03:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1331, 102, '2019-04-21 16:04:11', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1332, 102, '2019-04-21 16:04:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1333, 102, '2019-04-21 16:22:01', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1334, 102, '2019-04-21 16:23:23', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1335, 102, '2019-04-21 16:25:34', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1336, 102, '2019-04-21 16:25:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1337, 102, '2019-04-21 16:26:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1338, 102, '2019-04-21 16:28:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1339, 102, '2019-04-21 16:29:30', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1340, 102, '2019-04-21 16:30:01', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1341, 102, '2019-04-21 16:33:09', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1342, 102, '2019-04-21 16:34:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1343, 102, '2019-04-21 16:35:38', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1344, 102, '2019-04-21 19:28:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1345, 102, '2019-04-21 19:29:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1346, 102, '2019-04-21 19:31:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1347, 102, '2019-04-21 19:32:45', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1348, 102, '2019-04-21 19:43:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1349, 102, '2019-04-21 19:45:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1350, 102, '2019-04-21 19:46:07', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1351, 102, '2019-04-21 19:46:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1352, 102, '2019-04-21 19:47:21', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1353, 102, '2019-04-21 19:49:51', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1354, 102, '2019-04-21 19:51:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1355, 102, '2019-04-21 19:52:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1356, 102, '2019-04-21 19:53:44', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1357, 102, '2019-04-21 19:56:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1358, 102, '2019-04-21 20:01:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1359, 102, '2019-04-21 20:03:04', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1360, 102, '2019-04-21 20:04:59', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1361, 102, '2019-04-21 20:08:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1362, 102, '2019-04-21 20:10:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1363, 102, '2019-04-21 20:12:26', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1364, 102, '2019-04-21 20:14:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1365, 102, '2019-04-21 20:16:30', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1366, 102, '2019-04-21 20:20:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1367, 102, '2019-04-21 20:26:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1368, 102, '2019-04-21 20:28:02', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1369, 102, '2019-04-21 20:29:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1370, 102, '2019-04-21 20:30:55', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1371, 102, '2019-04-21 21:13:26', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1372, 102, '2019-04-21 21:14:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1373, 102, '2019-04-21 21:19:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1374, 102, '2019-04-21 21:19:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1375, 102, '2019-04-21 21:39:48', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1376, 102, '2019-04-21 21:48:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1377, 102, '2019-04-21 21:51:20', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1378, 102, '2019-04-21 21:52:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1379, 102, '2019-04-21 21:53:38', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1380, 102, '2019-04-21 21:57:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1381, 102, '2019-04-21 21:58:33', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1382, 102, '2019-04-21 22:03:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1383, 102, '2019-04-21 22:04:47', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1384, 102, '2019-04-21 22:09:54', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1385, 102, '2019-04-21 22:24:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1386, 102, '2019-04-21 22:25:58', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1387, 102, '2019-04-21 22:37:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1388, 102, '2019-04-21 22:52:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1389, 102, '2019-04-21 22:52:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1390, 102, '2019-04-21 23:13:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1391, 102, '2019-04-21 23:15:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1392, 102, '2019-04-21 23:17:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1393, 102, '2019-04-21 23:21:06', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1394, 102, '2019-04-21 23:22:44', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1395, 102, '2019-04-21 23:27:02', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1396, 102, '2019-04-21 23:30:01', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1397, 102, '2019-04-21 23:31:59', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1398, 102, '2019-04-21 23:34:20', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1399, 102, '2019-04-21 23:35:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1400, 102, '2019-04-21 23:38:02', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1401, 102, '2019-04-21 23:39:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1402, 102, '2019-04-21 23:41:06', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1403, 102, '2019-04-21 23:46:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1404, 102, '2019-04-21 23:47:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1405, 102, '2019-04-21 23:49:29', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1406, 102, '2019-04-21 23:51:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1407, 102, '2019-04-21 23:52:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1408, 102, '2019-04-21 23:54:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1409, 102, '2019-04-22 08:32:33', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1410, 102, '2019-04-22 08:34:44', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1411, 102, '2019-04-22 08:36:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1412, 102, '2019-04-22 08:38:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1413, 102, '2019-04-22 08:41:58', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1414, 102, '2019-04-22 08:44:47', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1415, 102, '2019-04-22 08:49:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1416, 102, '2019-04-22 08:49:48', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1417, 102, '2019-04-22 08:51:34', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1418, 102, '2019-04-22 08:55:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1419, 102, '2019-04-22 08:56:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1420, 102, '2019-04-22 09:00:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1421, 102, '2019-04-22 09:01:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1422, 102, '2019-04-22 09:03:13', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1423, 102, '2019-04-22 09:04:51', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1424, 102, '2019-04-22 09:07:58', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1425, 102, '2019-04-22 09:08:19', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1426, 102, '2019-04-22 09:10:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1427, 102, '2019-04-22 09:26:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1428, 102, '2019-04-22 10:13:38', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1429, 102, '2019-04-22 10:16:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1430, 102, '2019-04-22 10:17:13', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1431, 102, '2019-04-22 10:19:23', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1432, 102, '2019-04-22 10:28:34', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1433, 102, '2019-04-22 10:29:34', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1434, 102, '2019-04-22 10:39:06', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1435, 102, '2019-04-22 10:39:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1436, 102, '2019-04-22 10:40:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1437, 102, '2019-04-22 10:41:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1438, 102, '2019-04-22 10:41:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1439, 102, '2019-04-22 10:46:25', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1440, 102, '2019-04-22 10:49:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1441, 102, '2019-04-22 10:50:09', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1442, 102, '2019-04-22 10:57:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1443, 102, '2019-04-22 10:57:45', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1444, 102, '2019-04-22 10:59:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1445, 102, '2019-04-22 11:01:59', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1446, 102, '2019-04-22 11:04:51', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1447, 102, '2019-04-22 11:07:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1448, 102, '2019-04-22 11:08:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1449, 102, '2019-04-22 11:11:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1450, 102, '2019-04-22 11:11:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1451, 102, '2019-04-22 11:12:52', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1452, 102, '2019-04-22 11:15:20', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1453, 102, '2019-04-22 11:16:55', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1454, 102, '2019-04-22 11:17:25', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1455, 102, '2019-04-22 11:17:38', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1456, 102, '2019-04-22 11:18:25', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1457, 102, '2019-04-22 11:20:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1458, 102, '2019-04-22 11:22:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1459, 102, '2019-04-22 11:23:48', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1460, 102, '2019-04-22 11:24:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1461, 102, '2019-04-22 11:25:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1462, 102, '2019-04-22 11:26:30', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1463, 102, '2019-04-22 11:28:06', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1464, 102, '2019-04-22 11:28:16', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1465, 102, '2019-04-22 11:28:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1466, 102, '2019-04-22 11:29:51', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1467, 102, '2019-04-22 11:31:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1468, 102, '2019-04-22 11:33:20', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1469, 102, '2019-04-22 11:44:16', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1470, 102, '2019-04-22 11:50:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1471, 102, '2019-04-22 12:05:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1472, 102, '2019-04-22 12:05:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1473, 102, '2019-04-22 12:09:01', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1474, 102, '2019-04-22 12:11:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1475, 102, '2019-04-22 12:13:30', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1476, 102, '2019-04-22 12:13:58', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1477, 102, '2019-04-22 12:14:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1478, 102, '2019-04-22 12:15:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1479, 102, '2019-04-22 12:16:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1480, 102, '2019-04-22 12:16:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1481, 102, '2019-04-22 12:40:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1482, 102, '2019-04-22 12:41:23', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1483, 102, '2019-04-22 12:42:48', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1484, 102, '2019-04-22 12:43:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1485, 102, '2019-04-22 12:44:21', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1486, 102, '2019-04-22 12:44:55', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1487, 102, '2019-04-22 13:12:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1488, 102, '2019-04-22 13:20:04', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1489, 102, '2019-04-22 13:20:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1490, 102, '2019-04-22 13:32:13', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1491, 102, '2019-04-22 13:37:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1492, 102, '2019-04-22 13:38:06', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1493, 102, '2019-04-22 13:39:39', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1494, 102, '2019-04-22 13:40:54', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1495, 102, '2019-04-22 13:41:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1496, 102, '2019-04-22 13:41:47', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1497, 102, '2019-04-22 14:29:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1498, 102, '2019-04-22 14:35:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1499, 102, '2019-04-22 14:35:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1500, 102, '2019-04-22 14:38:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1501, 102, '2019-04-22 14:42:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1502, 102, '2019-04-22 14:46:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1503, 102, '2019-04-22 14:46:51', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1504, 102, '2019-04-22 14:48:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1505, 102, '2019-04-22 14:48:34', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1506, 102, '2019-04-22 14:49:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1507, 102, '2019-04-22 14:50:24', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1508, 102, '2019-04-22 14:52:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1509, 102, '2019-04-22 14:53:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1510, 102, '2019-04-22 14:54:39', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1511, 102, '2019-04-22 14:57:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1512, 102, '2019-04-22 15:01:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1513, 102, '2019-04-22 15:03:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1514, 102, '2019-04-22 15:19:24', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1515, 106, '2019-04-22 15:20:28', 'Register', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1516, 106, '2019-04-22 15:20:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1517, 106, '2019-04-22 15:20:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1518, 106, '2019-04-22 15:28:52', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1519, 102, '2019-04-22 15:29:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1520, 102, '2019-04-22 15:30:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1521, 102, '2019-04-22 15:32:13', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1522, 102, '2019-04-22 15:32:58', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1523, 102, '2019-04-22 15:41:07', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1524, 102, '2019-04-22 15:43:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1525, 102, '2019-04-22 15:46:01', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1526, 102, '2019-04-22 15:46:25', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1527, 102, '2019-04-22 15:49:47', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1528, 102, '2019-04-22 16:02:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1529, 103, '2019-04-22 16:14:54', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1530, 103, '2019-04-22 16:15:33', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1531, 103, '2019-04-22 16:28:10', 'LogIn', 1, 'websocket-sharp/1.0', '77.55.212.240:24231'),
(1532, 104, '2019-04-22 16:28:26', 'LogIn', 1, 'websocket-sharp/1.0', '77.55.212.240:24231'),
(1533, 104, '2019-04-22 16:28:58', 'LogIn', 1, 'websocket-sharp/1.0', '77.55.212.240:24231'),
(1534, 106, '2019-04-22 16:29:28', 'LogIn', 1, 'websocket-sharp/1.0', '77.55.212.240:24231'),
(1535, 110, '2019-04-22 16:30:06', 'Register', 1, 'websocket-sharp/1.0', '77.55.212.240:24231'),
(1536, 110, '2019-04-22 16:30:06', 'LogIn', 1, 'websocket-sharp/1.0', '77.55.212.240:24231'),
(1537, 101, '2019-04-22 16:33:17', 'LogIn', 1, 'websocket-sharp/1.0', '77.55.212.240:24231'),
(1538, 102, '2019-04-22 16:34:32', 'LogIn', 1, 'websocket-sharp/1.0', '77.55.212.240:24231'),
(1539, 104, '2019-04-22 16:52:07', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1540, 104, '2019-04-22 16:55:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1541, 104, '2019-04-22 17:19:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1542, 102, '2019-04-22 17:20:24', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1543, 102, '2019-04-22 17:27:39', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1544, 102, '2019-04-22 17:39:20', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1545, 102, '2019-04-27 14:28:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1546, 102, '2019-04-27 14:29:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1547, 102, '2019-04-27 14:34:52', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1548, 102, '2019-04-27 14:40:44', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1549, 102, '2019-04-27 14:53:26', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1550, 102, '2019-04-27 15:02:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1551, 102, '2019-04-27 15:02:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1552, 102, '2019-04-27 15:03:20', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1553, 102, '2019-04-27 15:04:44', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1554, 102, '2019-04-27 15:07:52', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1555, 102, '2019-04-27 15:26:26', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1556, 102, '2019-04-27 15:27:19', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1557, 102, '2019-04-27 15:40:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1558, 102, '2019-04-27 15:47:09', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1559, 102, '2019-04-27 16:29:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1560, 102, '2019-04-27 16:30:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1561, 102, '2019-04-27 16:31:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1562, 102, '2019-04-27 16:31:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1563, 102, '2019-04-27 21:01:26', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1564, 102, '2019-04-27 21:02:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1565, 102, '2019-04-27 21:04:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1566, 102, '2019-04-27 21:05:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1567, 102, '2019-04-27 21:06:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1568, 102, '2019-04-27 21:08:01', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1569, 102, '2019-04-27 21:09:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1570, 102, '2019-04-27 21:12:19', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1571, 102, '2019-04-27 21:13:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1572, 102, '2019-04-28 16:27:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1573, 102, '2019-04-28 16:30:06', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1574, 102, '2019-04-28 16:45:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1575, 102, '2019-04-28 17:18:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1576, 102, '2019-04-28 17:21:01', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1577, 102, '2019-04-28 17:23:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1578, 102, '2019-04-28 17:24:24', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1579, 102, '2019-04-28 17:25:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1580, 102, '2019-04-28 17:34:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1581, 102, '2019-04-28 17:36:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1582, 102, '2019-04-28 17:37:04', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1583, 102, '2019-04-28 18:15:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1584, 102, '2019-04-28 18:15:54', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1585, 102, '2019-04-28 20:45:33', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1586, 102, '2019-04-28 20:46:23', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1587, 102, '2019-04-28 21:01:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1588, 102, '2019-04-28 21:02:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1589, 102, '2019-04-28 21:04:26', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1590, 102, '2019-04-28 21:05:02', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1591, 102, '2019-04-28 21:08:47', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1592, 102, '2019-04-28 21:11:19', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1593, 102, '2019-04-28 21:12:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1594, 102, '2019-04-28 21:16:02', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1595, 102, '2019-04-28 21:17:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1596, 102, '2019-04-28 21:19:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1597, 102, '2019-05-01 10:17:55', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1598, 102, '2019-05-01 10:21:01', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1599, 102, '2019-05-01 10:22:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1600, 102, '2019-05-01 10:26:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1601, 102, '2019-05-01 10:32:20', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1602, 102, '2019-05-01 10:34:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1603, 102, '2019-05-01 10:35:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1604, 102, '2019-05-01 10:36:11', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1605, 102, '2019-05-01 11:39:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1606, 102, '2019-05-01 11:41:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1607, 102, '2019-05-01 11:42:25', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1608, 102, '2019-05-01 11:43:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1609, 102, '2019-05-01 11:44:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1610, 102, '2019-05-01 12:16:47', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1611, 102, '2019-05-01 12:18:29', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1612, 102, '2019-05-01 12:18:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1613, 102, '2019-05-01 12:19:07', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1614, 102, '2019-05-01 12:19:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1615, 102, '2019-05-01 12:20:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1616, 102, '2019-05-01 12:20:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1617, 102, '2019-05-01 12:25:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1618, 102, '2019-05-01 12:33:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1619, 102, '2019-05-01 12:34:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1620, 102, '2019-05-01 12:35:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1621, 101, '2019-05-01 12:35:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1622, 102, '2019-05-01 12:35:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1623, 101, '2019-05-01 12:40:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1624, 102, '2019-05-01 12:40:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1625, 102, '2019-05-01 12:40:54', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1626, 102, '2019-05-01 12:41:34', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1627, 102, '2019-05-01 12:41:55', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1628, 101, '2019-05-01 12:43:52', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1629, 102, '2019-05-01 12:43:54', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1630, 102, '2019-05-01 14:02:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1631, 102, '2019-05-01 14:02:44', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1632, 102, '2019-05-01 14:03:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1633, 102, '2019-05-01 14:05:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1634, 102, '2019-05-01 14:05:34', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1635, 102, '2019-05-01 14:06:24', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1636, 102, '2019-05-01 14:09:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1637, 102, '2019-05-01 14:19:04', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1638, 102, '2019-05-01 14:20:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1639, 102, '2019-05-01 15:37:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1640, 102, '2019-05-01 15:38:20', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1641, 102, '2019-05-01 15:39:33', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1642, 102, '2019-05-01 15:51:44', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1643, 102, '2019-05-01 15:52:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1644, 102, '2019-05-01 16:06:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1645, 102, '2019-05-01 16:07:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1646, 102, '2019-05-01 16:09:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1647, 102, '2019-05-01 16:11:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1648, 102, '2019-05-01 16:11:39', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1649, 102, '2019-05-01 16:12:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1650, 102, '2019-05-01 16:18:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1651, 102, '2019-05-01 16:19:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1652, 102, '2019-05-01 16:21:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1653, 102, '2019-05-01 16:22:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1654, 102, '2019-05-01 16:25:45', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1655, 102, '2019-05-01 16:26:09', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1656, 102, '2019-05-01 16:26:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1657, 102, '2019-05-01 16:33:09', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1658, 102, '2019-05-01 16:33:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1659, 102, '2019-05-01 16:35:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1660, 102, '2019-05-01 16:36:25', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1661, 101, '2019-05-01 16:37:02', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1662, 102, '2019-05-01 16:37:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1663, 102, '2019-05-01 16:43:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1664, 102, '2019-05-01 16:43:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1665, 102, '2019-05-01 16:45:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1666, 102, '2019-05-01 16:47:20', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1667, 102, '2019-05-01 16:53:23', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1668, 101, '2019-05-01 16:53:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1669, 102, '2019-05-01 16:59:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1670, 102, '2019-05-01 17:14:26', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1671, 102, '2019-05-01 17:21:30', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1672, 102, '2019-05-01 17:22:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1673, 102, '2019-05-01 17:23:09', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1674, 102, '2019-05-01 18:44:20', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1675, 102, '2019-05-01 18:45:34', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1676, 102, '2019-05-01 18:46:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1677, 102, '2019-05-01 18:49:54', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1678, 102, '2019-05-01 18:51:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1679, 102, '2019-05-01 18:52:45', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1680, 102, '2019-05-01 19:32:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1681, 102, '2019-05-01 19:37:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1682, 102, '2019-05-01 19:38:02', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1683, 102, '2019-05-01 19:39:33', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1684, 102, '2019-05-01 19:39:55', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1685, 102, '2019-05-01 19:41:38', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1686, 102, '2019-05-01 19:43:26', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1687, 102, '2019-05-01 19:44:39', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1688, 102, '2019-05-01 19:45:44', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1689, 102, '2019-05-01 19:46:29', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1690, 102, '2019-05-01 19:47:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1691, 102, '2019-05-01 19:48:01', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1692, 102, '2019-05-01 19:49:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1693, 102, '2019-05-01 19:50:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1694, 102, '2019-05-01 19:54:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1695, 102, '2019-05-01 19:58:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1696, 102, '2019-05-01 20:01:33', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1697, 102, '2019-05-01 20:01:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1698, 102, '2019-05-01 20:02:09', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1699, 102, '2019-05-01 20:02:27', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1700, 102, '2019-05-01 20:03:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1701, 102, '2019-05-01 20:04:16', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1702, 102, '2019-05-01 20:04:33', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1703, 102, '2019-05-01 20:05:55', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1704, 102, '2019-05-01 20:07:21', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1705, 102, '2019-05-01 20:08:54', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1706, 102, '2019-05-01 20:12:27', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1707, 102, '2019-05-01 20:15:26', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1708, 102, '2019-05-01 20:17:25', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1709, 102, '2019-05-01 20:19:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1710, 102, '2019-05-01 20:24:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1711, 102, '2019-05-01 20:26:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231');
INSERT INTO `userslog` (`id`, `userid`, `datetime`, `action`, `result`, `useragent`, `host`) VALUES
(1712, 102, '2019-05-01 20:26:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1713, 102, '2019-05-01 20:27:52', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1714, 102, '2019-05-01 20:30:58', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1715, 102, '2019-05-01 20:32:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1716, 102, '2019-05-01 20:33:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1717, 102, '2019-05-01 20:38:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1718, 102, '2019-05-01 20:39:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1719, 102, '2019-05-01 20:42:21', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1720, 102, '2019-05-01 20:45:29', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1721, 102, '2019-05-01 20:47:09', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1722, 102, '2019-05-01 20:48:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1723, 102, '2019-05-01 20:50:59', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1724, 102, '2019-05-01 20:52:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1725, 102, '2019-05-01 20:54:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1726, 102, '2019-05-01 20:56:26', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1727, 102, '2019-05-01 20:56:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1728, 102, '2019-05-01 20:57:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1729, 102, '2019-05-01 21:11:20', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1730, 102, '2019-05-01 21:12:20', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1731, 102, '2019-05-01 21:12:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1732, 102, '2019-05-01 21:13:11', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1733, 102, '2019-05-01 21:13:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1734, 102, '2019-05-01 21:18:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1735, 102, '2019-05-01 21:20:19', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1736, 102, '2019-05-01 21:21:27', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1737, 102, '2019-05-01 21:23:02', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1738, 102, '2019-05-01 21:23:25', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1739, 102, '2019-05-01 21:27:27', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1740, 102, '2019-05-01 21:28:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1741, 102, '2019-05-01 21:29:52', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1742, 102, '2019-05-01 21:32:27', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1743, 102, '2019-05-01 21:36:01', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1744, 102, '2019-05-01 21:52:59', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1745, 102, '2019-05-01 21:54:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1746, 102, '2019-05-01 21:55:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1747, 102, '2019-05-01 21:55:58', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1748, 102, '2019-05-01 21:57:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1749, 102, '2019-05-01 22:00:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1750, 102, '2019-05-01 22:00:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1751, 102, '2019-05-01 22:04:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1752, 102, '2019-05-02 09:14:29', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1753, 102, '2019-05-02 09:20:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1754, 102, '2019-05-02 09:20:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1755, 102, '2019-05-02 09:27:33', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1756, 102, '2019-05-02 09:29:23', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1757, 102, '2019-05-02 09:30:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1758, 102, '2019-05-02 09:30:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1759, 102, '2019-05-02 09:30:38', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1760, 102, '2019-05-02 09:31:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1761, 102, '2019-05-02 09:32:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1762, 102, '2019-05-02 09:32:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1763, 102, '2019-05-02 09:49:45', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1764, 102, '2019-05-02 09:50:07', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1765, 102, '2019-05-02 09:54:26', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1766, 102, '2019-05-02 10:01:58', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1767, 102, '2019-05-02 10:02:11', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1768, 102, '2019-05-02 10:08:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1769, 102, '2019-05-02 12:16:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1770, 102, '2019-05-02 12:20:48', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1771, 102, '2019-05-02 12:28:45', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1772, 102, '2019-05-02 12:29:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1773, 102, '2019-05-02 12:37:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1774, 102, '2019-05-02 12:40:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1775, 102, '2019-05-02 12:41:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1776, 102, '2019-05-02 12:41:33', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1777, 102, '2019-05-02 12:42:26', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1778, 102, '2019-05-02 12:43:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1779, 102, '2019-05-02 12:46:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1780, 102, '2019-05-02 12:47:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1781, 102, '2019-05-02 12:47:11', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1782, 102, '2019-05-02 12:47:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1783, 102, '2019-05-02 12:49:44', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1784, 102, '2019-05-02 12:49:51', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1785, 102, '2019-05-02 12:50:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1786, 102, '2019-05-02 12:52:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1787, 102, '2019-05-02 12:53:21', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1788, 102, '2019-05-02 12:55:58', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1789, 102, '2019-05-02 12:56:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1790, 102, '2019-05-02 12:57:29', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1791, 102, '2019-05-02 13:01:27', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1792, 102, '2019-05-02 13:02:34', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1793, 102, '2019-05-02 13:02:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1794, 102, '2019-05-02 13:02:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1795, 102, '2019-05-02 13:04:07', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1796, 102, '2019-05-02 13:04:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1797, 102, '2019-05-02 13:05:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1798, 102, '2019-05-02 13:08:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1799, 102, '2019-05-02 13:08:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1800, 102, '2019-05-02 13:08:58', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1801, 102, '2019-05-02 13:09:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1802, 102, '2019-05-02 13:10:07', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1803, 102, '2019-05-02 13:10:33', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1804, 102, '2019-05-02 13:11:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1805, 102, '2019-05-02 16:25:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1806, 102, '2019-05-02 16:26:30', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1807, 102, '2019-05-02 16:29:29', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1808, 102, '2019-05-02 16:31:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1809, 102, '2019-05-02 16:32:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1810, 102, '2019-05-02 16:33:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1811, 102, '2019-05-02 16:36:04', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1812, 102, '2019-05-02 16:38:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1813, 102, '2019-05-02 16:44:48', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1814, 102, '2019-05-02 16:45:39', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1815, 102, '2019-05-02 17:01:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1816, 102, '2019-05-02 17:02:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1817, 102, '2019-05-02 17:04:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1818, 102, '2019-05-02 17:05:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1819, 102, '2019-05-02 17:14:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1820, 102, '2019-05-02 17:18:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1821, 102, '2019-05-02 17:19:20', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1822, 102, '2019-05-02 17:20:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1823, 102, '2019-05-02 17:22:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1824, 102, '2019-05-02 17:22:20', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1825, 102, '2019-05-02 17:26:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1826, 102, '2019-05-02 17:47:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1827, 102, '2019-05-02 17:49:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1828, 102, '2019-05-02 17:50:52', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1829, 102, '2019-05-02 18:08:06', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1830, 102, '2019-05-02 18:09:51', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1831, 102, '2019-05-02 18:10:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1832, 102, '2019-05-02 18:12:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1833, 102, '2019-05-02 19:32:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1834, 102, '2019-05-02 19:32:55', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1835, 102, '2019-05-02 19:33:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1836, 102, '2019-05-02 19:33:24', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1837, 102, '2019-05-02 19:51:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1838, 102, '2019-05-02 19:51:39', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1839, 102, '2019-05-02 19:57:51', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1840, 102, '2019-05-02 19:59:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1841, 102, '2019-05-02 20:00:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1842, 102, '2019-05-02 20:02:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1843, 102, '2019-05-02 20:03:25', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1844, 102, '2019-05-02 20:05:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1845, 102, '2019-05-02 20:07:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1846, 102, '2019-05-02 20:08:27', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1847, 102, '2019-05-02 20:09:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1848, 102, '2019-05-02 20:10:59', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1849, 102, '2019-05-02 20:12:01', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1850, 102, '2019-05-02 20:12:48', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1851, 102, '2019-05-02 20:13:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1852, 102, '2019-05-02 20:14:30', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1853, 102, '2019-05-02 20:15:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1854, 102, '2019-05-02 20:17:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1855, 102, '2019-05-02 20:18:52', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1856, 102, '2019-05-02 20:19:52', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1857, 102, '2019-05-02 20:36:06', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1858, 102, '2019-05-02 20:38:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1859, 102, '2019-05-02 20:40:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1860, 102, '2019-05-02 20:41:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1861, 102, '2019-05-02 20:41:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1862, 102, '2019-05-02 20:42:13', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1863, 102, '2019-05-02 20:42:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1864, 102, '2019-05-02 20:44:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1865, 102, '2019-05-02 20:44:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1866, 102, '2019-05-02 20:45:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1867, 102, '2019-05-02 20:46:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1868, 102, '2019-05-02 20:48:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1869, 102, '2019-05-02 20:51:45', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1870, 102, '2019-05-02 20:53:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1871, 102, '2019-05-02 20:56:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1872, 102, '2019-05-02 20:56:52', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1873, 102, '2019-05-02 20:57:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1874, 102, '2019-05-02 21:00:39', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1875, 102, '2019-05-02 21:02:29', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1876, 102, '2019-05-02 21:03:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1877, 102, '2019-05-02 21:05:01', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1878, 102, '2019-05-02 21:05:55', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1879, 102, '2019-05-03 09:14:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1880, 102, '2019-05-03 09:14:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1881, 102, '2019-05-03 09:19:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1882, 102, '2019-05-03 09:20:09', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1883, 102, '2019-05-03 09:23:30', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1884, 102, '2019-05-03 09:24:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1885, 102, '2019-05-03 09:34:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1886, 102, '2019-05-03 09:36:24', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1887, 102, '2019-05-03 09:36:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1888, 102, '2019-05-03 09:37:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1889, 102, '2019-05-03 09:37:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1890, 102, '2019-05-03 09:38:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1891, 102, '2019-05-03 09:40:26', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1892, 102, '2019-05-03 09:42:07', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1893, 102, '2019-05-03 09:43:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1894, 102, '2019-05-03 09:44:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1895, 102, '2019-05-03 10:12:09', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1896, 102, '2019-05-03 10:13:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1897, 102, '2019-05-03 10:15:13', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1898, 102, '2019-05-03 10:46:07', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1899, 102, '2019-05-03 10:47:25', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1900, 102, '2019-05-03 10:48:24', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1901, 102, '2019-05-03 10:52:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1902, 102, '2019-05-03 10:53:48', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1903, 102, '2019-05-03 10:54:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1904, 102, '2019-05-03 10:56:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1905, 102, '2019-05-03 10:57:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1906, 102, '2019-05-03 10:59:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1907, 102, '2019-05-03 11:00:25', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1908, 102, '2019-05-03 11:01:30', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1909, 102, '2019-05-03 11:02:34', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1910, 102, '2019-05-03 11:06:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1911, 102, '2019-05-03 11:07:38', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1912, 102, '2019-05-03 11:08:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1913, 102, '2019-05-03 11:15:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1914, 102, '2019-05-03 11:17:02', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1915, 102, '2019-05-03 11:21:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1916, 102, '2019-05-03 11:21:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1917, 102, '2019-05-03 11:21:55', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1918, 102, '2019-05-03 11:30:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1919, 102, '2019-05-03 11:41:33', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1920, 102, '2019-05-03 11:46:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1921, 102, '2019-05-03 11:50:02', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1922, 102, '2019-05-03 11:57:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1923, 102, '2019-05-03 12:02:29', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1924, 102, '2019-05-03 12:07:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1925, 102, '2019-05-03 12:11:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1926, 102, '2019-05-03 12:12:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1927, 102, '2019-05-03 12:15:18', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1928, 102, '2019-05-03 12:27:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1929, 102, '2019-05-03 12:29:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1930, 102, '2019-05-03 12:31:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1931, 102, '2019-05-03 12:33:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1932, 102, '2019-05-03 12:34:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1933, 102, '2019-05-03 12:37:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1934, 102, '2019-05-03 12:38:27', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1935, 102, '2019-05-03 12:42:11', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1936, 102, '2019-05-03 12:43:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1937, 102, '2019-05-03 12:45:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1938, 102, '2019-05-03 12:45:13', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1939, 102, '2019-05-03 12:46:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1940, 102, '2019-05-03 12:47:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1941, 102, '2019-05-03 12:49:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1942, 102, '2019-05-03 12:49:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1943, 102, '2019-05-03 12:51:09', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1944, 102, '2019-05-03 12:52:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1945, 102, '2019-05-03 14:57:45', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1946, 102, '2019-05-03 14:58:06', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1947, 102, '2019-05-03 14:58:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1948, 102, '2019-05-03 14:59:34', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1949, 102, '2019-05-03 15:04:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1950, 102, '2019-05-03 15:13:20', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1951, 102, '2019-05-03 15:25:59', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1952, 102, '2019-05-03 15:26:31', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1953, 102, '2019-05-03 15:30:59', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1954, 102, '2019-05-03 15:32:45', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1955, 102, '2019-05-03 15:34:24', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1956, 102, '2019-05-03 15:35:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1957, 102, '2019-05-03 15:37:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1958, 102, '2019-05-03 15:39:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1959, 102, '2019-05-03 15:42:20', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1960, 102, '2019-05-03 15:45:04', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1961, 102, '2019-05-03 15:46:44', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1962, 102, '2019-05-03 15:54:21', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1963, 102, '2019-05-03 16:18:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1964, 102, '2019-05-03 16:33:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1965, 102, '2019-05-03 16:34:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1966, 102, '2019-05-03 16:38:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1967, 102, '2019-05-03 16:53:19', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1968, 102, '2019-05-03 17:04:15', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1969, 102, '2019-05-03 17:05:51', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1970, 102, '2019-05-03 17:07:23', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1971, 102, '2019-05-03 17:08:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1972, 102, '2019-05-03 17:08:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1973, 102, '2019-05-03 17:10:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1974, 102, '2019-05-03 17:18:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1975, 102, '2019-05-03 17:20:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1976, 102, '2019-05-03 17:21:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1977, 102, '2019-05-03 17:21:52', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1978, 102, '2019-05-03 17:23:25', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1979, 102, '2019-05-03 17:24:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1980, 102, '2019-05-03 17:25:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1981, 102, '2019-05-03 17:28:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1982, 102, '2019-05-03 17:28:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1983, 102, '2019-05-03 17:31:38', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1984, 102, '2019-05-03 17:32:25', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1985, 102, '2019-05-03 17:33:58', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1986, 102, '2019-05-03 17:36:44', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1987, 102, '2019-05-03 17:38:19', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1988, 102, '2019-05-03 17:38:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1989, 102, '2019-05-03 17:39:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1990, 102, '2019-05-03 17:41:16', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1991, 102, '2019-05-03 17:42:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1992, 102, '2019-05-03 17:42:40', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1993, 102, '2019-05-03 17:43:06', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1994, 102, '2019-05-03 17:43:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1995, 102, '2019-05-03 17:44:48', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1996, 102, '2019-05-03 17:47:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1997, 102, '2019-05-03 17:48:16', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1998, 102, '2019-05-03 17:49:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(1999, 102, '2019-05-03 17:51:27', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2000, 102, '2019-05-03 17:53:01', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2001, 102, '2019-05-03 17:53:39', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2002, 102, '2019-05-03 17:55:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2003, 102, '2019-05-03 17:59:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2004, 102, '2019-05-03 17:59:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2005, 102, '2019-05-03 18:57:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2006, 102, '2019-05-03 18:58:19', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2007, 102, '2019-05-03 19:05:55', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2008, 102, '2019-05-03 19:08:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2009, 102, '2019-05-03 19:11:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2010, 102, '2019-05-03 19:13:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2011, 102, '2019-05-03 19:16:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2012, 102, '2019-05-03 19:17:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2013, 102, '2019-05-03 19:18:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2014, 102, '2019-05-03 19:20:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2015, 102, '2019-05-03 19:20:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2016, 102, '2019-05-03 19:23:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2017, 102, '2019-05-03 19:23:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2018, 102, '2019-05-03 19:25:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2019, 102, '2019-05-03 19:28:27', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2020, 102, '2019-05-03 19:29:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2021, 102, '2019-05-03 19:30:13', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2022, 102, '2019-05-03 19:31:19', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2023, 102, '2019-05-03 19:32:39', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2024, 102, '2019-05-03 19:34:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2025, 102, '2019-05-03 19:34:59', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2026, 102, '2019-05-03 19:35:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2027, 102, '2019-05-03 19:36:13', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2028, 102, '2019-05-03 19:38:22', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2029, 102, '2019-05-03 19:39:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2030, 102, '2019-05-03 19:40:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2031, 102, '2019-05-03 19:41:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2032, 102, '2019-05-03 19:41:58', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2033, 102, '2019-05-03 19:43:48', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2034, 102, '2019-05-03 19:45:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2035, 102, '2019-05-03 19:45:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2036, 102, '2019-05-03 19:48:34', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2037, 102, '2019-05-03 19:49:01', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2038, 102, '2019-05-03 19:49:34', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2039, 102, '2019-05-03 19:53:44', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2040, 102, '2019-05-03 19:54:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2041, 102, '2019-05-03 19:55:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2042, 102, '2019-05-03 19:56:55', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2043, 102, '2019-05-03 19:59:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2044, 102, '2019-05-03 20:00:24', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2045, 102, '2019-05-03 20:02:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2046, 102, '2019-05-03 20:02:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2047, 102, '2019-05-03 20:06:17', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2048, 102, '2019-05-03 20:08:46', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2049, 102, '2019-05-03 20:12:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2050, 102, '2019-05-03 20:14:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2051, 102, '2019-05-03 20:15:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2052, 102, '2019-05-03 20:16:24', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2053, 102, '2019-05-03 20:20:04', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2054, 102, '2019-05-03 20:22:02', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2055, 102, '2019-05-03 20:24:32', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2056, 102, '2019-05-03 20:26:10', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2057, 102, '2019-05-03 20:32:52', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2058, 102, '2019-05-03 20:33:29', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2059, 102, '2019-05-03 20:35:16', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2060, 102, '2019-05-03 20:35:41', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2061, 102, '2019-05-04 13:48:57', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2062, 102, '2019-05-04 13:49:52', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2063, 102, '2019-05-04 13:50:03', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2064, 102, '2019-05-04 13:50:28', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2065, 102, '2019-05-04 13:55:35', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2066, 102, '2019-05-04 13:56:37', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2067, 102, '2019-05-04 13:57:36', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2068, 102, '2019-05-04 15:46:50', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2069, 102, '2019-05-04 15:50:45', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2070, 102, '2019-05-04 16:19:27', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2071, 102, '2019-05-04 16:20:27', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2072, 102, '2019-05-04 16:22:04', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2073, 102, '2019-05-04 16:23:08', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2074, 102, '2019-05-04 16:25:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2075, 102, '2019-05-04 16:25:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2076, 102, '2019-05-04 16:27:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2077, 102, '2019-05-04 16:27:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2078, 102, '2019-05-04 16:33:04', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2079, 102, '2019-05-04 16:33:55', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2080, 102, '2019-05-04 16:36:34', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2081, 102, '2019-05-04 16:41:16', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2082, 102, '2019-05-04 16:47:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2083, 102, '2019-05-04 16:55:34', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2084, 102, '2019-05-04 16:56:58', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2085, 102, '2019-05-04 17:07:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2086, 102, '2019-05-04 17:07:53', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2087, 102, '2019-05-04 17:08:24', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2088, 102, '2019-05-04 17:09:58', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2089, 102, '2019-05-04 17:10:42', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2090, 102, '2019-05-04 17:12:25', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2091, 102, '2019-05-04 17:12:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2092, 102, '2019-05-04 17:13:04', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2093, 102, '2019-05-04 17:16:01', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2094, 102, '2019-05-04 17:18:49', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2095, 102, '2019-05-04 17:21:12', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2096, 102, '2019-05-04 17:21:54', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2097, 102, '2019-05-04 17:25:56', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2098, 102, '2019-05-04 17:32:05', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2099, 102, '2019-05-04 17:35:26', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2100, 102, '2019-05-04 17:35:43', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2101, 102, '2019-05-04 17:40:14', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231'),
(2102, 102, '2019-05-04 17:43:00', 'LogIn', 1, 'websocket-sharp/1.0', '127.0.0.1:24231');

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
  ADD KEY `prefabname` (`prefabid`),
  ADD KEY `ammunitionid` (`ammunitionid`),
  ADD KEY `rocketid` (`rocketid`);

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
-- Indeksy dla tabeli `itemreward`
--
ALTER TABLE `itemreward`
  ADD PRIMARY KEY (`itemrewardid`),
  ADD KEY `itemid` (`itemid`),
  ADD KEY `rewardid` (`rewardid`);

--
-- Indeksy dla tabeli `items`
--
ALTER TABLE `items`
  ADD PRIMARY KEY (`itemid`),
  ADD KEY `itemtypeid` (`itemtypeid`);

--
-- Indeksy dla tabeli `itemtypes`
--
ALTER TABLE `itemtypes`
  ADD PRIMARY KEY (`itemtypeid`);

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
  ADD KEY `shipid` (`shipid`),
  ADD KEY `ammunitionid` (`ammunitionid`),
  ADD KEY `rocketid` (`rocketid`);

--
-- Indeksy dla tabeli `pilotsitems`
--
ALTER TABLE `pilotsitems`
  ADD PRIMARY KEY (`relationid`),
  ADD KEY `pilotid` (`userid`),
  ADD KEY `itemid` (`itemid`);

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
  ADD PRIMARY KEY (`rewardid`),
  ADD KEY `ammunitionid` (`ammunitionid`);

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
  MODIFY `ammunitionid` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=107;

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
-- AUTO_INCREMENT dla tabeli `itemreward`
--
ALTER TABLE `itemreward`
  MODIFY `itemrewardid` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT dla tabeli `items`
--
ALTER TABLE `items`
  MODIFY `itemid` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=26;

--
-- AUTO_INCREMENT dla tabeli `itemtypes`
--
ALTER TABLE `itemtypes`
  MODIFY `itemtypeid` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT dla tabeli `maps`
--
ALTER TABLE `maps`
  MODIFY `mapid` int(11) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=117;

--
-- AUTO_INCREMENT dla tabeli `pilotsitems`
--
ALTER TABLE `pilotsitems`
  MODIFY `relationid` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=261;

--
-- AUTO_INCREMENT dla tabeli `portalpositions`
--
ALTER TABLE `portalpositions`
  MODIFY `portalpositionid` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=122;

--
-- AUTO_INCREMENT dla tabeli `portals`
--
ALTER TABLE `portals`
  MODIFY `portalid` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=104;

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
  MODIFY `userid` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=111;

--
-- AUTO_INCREMENT dla tabeli `userslog`
--
ALTER TABLE `userslog`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2103;

--
-- Ograniczenia dla zrzutów tabel
--

--
-- Ograniczenia dla tabeli `enemies`
--
ALTER TABLE `enemies`
  ADD CONSTRAINT `enemies_ibfk_1` FOREIGN KEY (`rewardid`) REFERENCES `rewards` (`rewardid`),
  ADD CONSTRAINT `enemies_ibfk_2` FOREIGN KEY (`prefabid`) REFERENCES `prefabs` (`prefabid`),
  ADD CONSTRAINT `enemies_ibfk_3` FOREIGN KEY (`ammunitionid`) REFERENCES `ammunitions` (`ammunitionid`),
  ADD CONSTRAINT `enemies_ibfk_4` FOREIGN KEY (`rocketid`) REFERENCES `ammunitions` (`ammunitionid`);

--
-- Ograniczenia dla tabeli `enemymap`
--
ALTER TABLE `enemymap`
  ADD CONSTRAINT `enemymap_ibfk_1` FOREIGN KEY (`enemyid`) REFERENCES `enemies` (`enemyid`),
  ADD CONSTRAINT `enemymap_ibfk_2` FOREIGN KEY (`mapid`) REFERENCES `maps` (`mapid`);

--
-- Ograniczenia dla tabeli `itemreward`
--
ALTER TABLE `itemreward`
  ADD CONSTRAINT `itemreward_ibfk_1` FOREIGN KEY (`itemid`) REFERENCES `items` (`itemid`),
  ADD CONSTRAINT `itemreward_ibfk_2` FOREIGN KEY (`rewardid`) REFERENCES `rewards` (`rewardid`);

--
-- Ograniczenia dla tabeli `items`
--
ALTER TABLE `items`
  ADD CONSTRAINT `items_ibfk_1` FOREIGN KEY (`itemtypeid`) REFERENCES `itemtypes` (`itemtypeid`);

--
-- Ograniczenia dla tabeli `pilotresources`
--
ALTER TABLE `pilotresources`
  ADD CONSTRAINT `pilotresources_ibfk_3` FOREIGN KEY (`userid`) REFERENCES `pilots` (`userid`);

--
-- Ograniczenia dla tabeli `pilots`
--
ALTER TABLE `pilots`
  ADD CONSTRAINT `pilots_ibfk_1` FOREIGN KEY (`userid`) REFERENCES `users` (`userid`),
  ADD CONSTRAINT `pilots_ibfk_2` FOREIGN KEY (`mapid`) REFERENCES `maps` (`mapid`),
  ADD CONSTRAINT `pilots_ibfk_3` FOREIGN KEY (`shipid`) REFERENCES `ships` (`shipid`),
  ADD CONSTRAINT `pilots_ibfk_4` FOREIGN KEY (`ammunitionid`) REFERENCES `ammunitions` (`ammunitionid`),
  ADD CONSTRAINT `pilots_ibfk_5` FOREIGN KEY (`rocketid`) REFERENCES `ammunitions` (`ammunitionid`);

--
-- Ograniczenia dla tabeli `pilotsitems`
--
ALTER TABLE `pilotsitems`
  ADD CONSTRAINT `pilotsitems_ibfk_1` FOREIGN KEY (`userid`) REFERENCES `pilots` (`userid`),
  ADD CONSTRAINT `pilotsitems_ibfk_2` FOREIGN KEY (`itemid`) REFERENCES `items` (`itemid`);

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
-- Ograniczenia dla tabeli `rewards`
--
ALTER TABLE `rewards`
  ADD CONSTRAINT `rewards_ibfk_1` FOREIGN KEY (`ammunitionid`) REFERENCES `ammunitions` (`ammunitionid`);

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
